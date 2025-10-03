using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using System.Threading;
using System.Globalization;

using BrightSword.Feber.Samples;
using BrightSword.Squid.API;
using BrightSword.Squid.Behaviours;
using BrightSword.SwissKnife;

using INotifyPropertyChanged = BrightSword.Squid.API.INotifyPropertyChanged;
using INotifyPropertyChanging = BrightSword.Squid.API.INotifyPropertyChanging;

namespace BrightSword.Squid.TypeCreators
{
    // Suppress CA1707 for some legacy protected/private identifiers that use underscores.
    // These names are intentionally preserved to avoid large API/behavioral changes
    // in the reflection/emit code. We'll revisit these individually if we choose
    // to do an API-breaking rename later.
    public class BasicDataTransferObjectTypeCreator<T> : ITypeCreator<T>
        where T : class
    {
        protected const MethodAttributes PropertyHiddenMethodAttributes = MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.HideBySig | MethodAttributes.Final | MethodAttributes.NewSlot | MethodAttributes.SpecialName;

        private readonly List<Func<Type, Type>> _typeMaps;

        private string _assemblyName;

        private IDictionary<PropertyInfo, PropertyBackingFieldMap> _backingFieldPropertyMap;
        private Type _baseType;
        private string _className;
        private IEnumerable<Type> _facetInterfaces;
        private FieldValueSetInstructionHelper _fieldValueSetInstructionHelper;
        private Func<T> _instanceFactory;
        private string _interfaceName;
        private Type[] _interfacesWithSpecialBehaviours;
        private bool _saveAssemblyToDisk;
        private IEnumerable<KeyValuePair<Type, IBehaviour>> _specialBehaviours;
        private bool _trackReadonlyPropertyInitialized;
        private Type _type;
        // Cached Type[] arrays used for reflection GetMethod calls to avoid repeated allocations (CA1861)
    private static readonly Type[] OnPropertyChangingArgTypes = { typeof(string), typeof(Type), typeof(object), typeof(object) };
    private static readonly Type[] GetTypeFromHandleArgTypes = { typeof(RuntimeTypeHandle) };

        protected BasicDataTransferObjectTypeCreator()
        {
            _saveAssemblyToDisk = true;

            _trackReadonlyPropertyInitialized = false;

            _interfaceName = typeof(T).IsInterface
                                 ? typeof(T).PrintableName()
                                 : String.Empty;

            _className = typeof(T).RenameToConcreteType();

            _baseType = typeof(T).IsClass
                            ? typeof(T)
                            : typeof(Object);

            _assemblyName = $"Dynamic.{GetType().GetNonGenericPartOfClassName()}.{_interfaceName}";

            _facetInterfaces = Array.Empty<Type>();

            _fieldValueSetInstructionHelper = new FieldValueSetInstructionHelper();

            _specialBehaviours = new[]
                                 {
                                     new KeyValuePair<Type, IBehaviour>(typeof(ICloneable), new CloneBehaviour())
                                 };

            _typeMaps = new List<Func<Type, Type>>
                        {
                            _ => _.MapGenericTypeIfPossible(typeof (Dictionary<,>),
                                                            typeof (IDictionary<,>)),
                            _ => _.MapGenericTypeIfPossible(typeof (HashSet<>),
                                                            typeof (ISet<>)),
                            _ => _.MapGenericTypeIfPossible(typeof (List<>),
                                                            typeof (IList<>),
                                                            typeof (ICollection<>),
                                                            typeof (IEnumerable<>))
                        };
        }

        public BasicDataTransferObjectTypeCreator(params Func<Type, Type>[] userSuppliedTypeMaps)
            : this()
        {
            foreach (var userSuppliedTypeMap in userSuppliedTypeMaps)
            {
                _typeMaps.Add(userSuppliedTypeMap);
            }
        }

        #region Configurable Properties

        public virtual bool TrackReadonlyPropertyInitialized
        {
            get => _trackReadonlyPropertyInitialized;
            set => _trackReadonlyPropertyInitialized = value;
        }

        public virtual bool SaveAssemblyToDisk
        {
            get => _saveAssemblyToDisk;
            set => _saveAssemblyToDisk = value;
        }

        public virtual string AssemblyName
        {
            get => _assemblyName;
            set => _assemblyName = value;
        }

        public virtual string InterfaceName
        {
            get => _interfaceName;
            set => _interfaceName = value;
        }

        public virtual string ClassName
        {
            get => _className;
            set => _className = value;
        }

        public virtual Type BaseType
        {
            get => _baseType;
            set => _baseType = value;
        }

        public virtual IEnumerable<Type> FacetInterfaces
        {
            get => _facetInterfaces;
            set => _facetInterfaces = value;
        }

        public virtual FieldValueSetInstructionHelper FieldValueSetInstructionHelper
        {
            get => _fieldValueSetInstructionHelper;
            set => _fieldValueSetInstructionHelper = value;
        }

        public virtual IEnumerable<KeyValuePair<Type, IBehaviour>> SpecialBehaviours => _specialBehaviours;

        #endregion

        #region Overridable Properties

    protected virtual IEnumerable<Func<Type, Type>> TypeMaps => _typeMaps;

        protected virtual IDictionary<PropertyInfo, PropertyBackingFieldMap> BackingFieldProperties
        {
            get
            {
                return _backingFieldPropertyMap ??= (from _propertyInfo in typeof(T).GetAllNonExcludedProperties()
                                                                                let isReadonlyProperty = !_propertyInfo.CanWrite && _propertyInfo.GetSetMethod() == null
                                                                                let mappedType = GetMappedType(_propertyInfo)
                                                                                let backingFieldType = isReadonlyProperty
                                                                                                           ? mappedType ?? _propertyInfo.PropertyType
                                                                                                           : _propertyInfo.PropertyType
                                                                                select new PropertyBackingFieldMap
                                                                                {
                                                                                    IsReadOnly = isReadonlyProperty,
                                                                                    MappedType = mappedType,
                                                                                    PropertyInfo = _propertyInfo,
                                                                                    BackingFieldType = backingFieldType,
                                                                                    BackingField = null
                                                                                }).ToDictionary(_ => _.PropertyInfo,
                                                                                                       _ => _);
            }
        }

        protected virtual IEnumerable<Func<PropertyBuilder, PropertyInfo, PropertyBuilder>> PropertyOperations
        {
            get { yield break; }
        }

        protected virtual IEnumerable<Func<MethodBuilder, MethodInfo, MethodBuilder>> MethodOperations
        {
            get { yield break; }
        }

        protected virtual IEnumerable<Func<EventBuilder, EventInfo, EventBuilder>> EventOperations
        {
            get { yield break; }
        }

        protected virtual IEnumerable<Func<FieldBuilder, FieldInfo, FieldBuilder>> EventFieldOperations
        {
            get
            {
                yield return (builder,
                              info) => builder.AddCustomAttribute<NonSerializedAttribute>();
            }
        }

        protected virtual IEnumerable<Func<TypeBuilder, TypeBuilder>> CustomClassOperations
        {
            get { yield break; }
        }

        protected virtual IEnumerable<Func<TypeBuilder, TypeBuilder>> ClassOperations
        {
            get
            {
                yield return AddFields;

                if (TrackReadonlyPropertyInitialized)
                {
                    yield return AddInitializePropertyTrackingSupport;
                }

                yield return AddEvents;
                yield return AddMethods;
                yield return AddProperties;
                yield return AddDefaultConstructor;
                yield return AddNonDefaultConstructors;

                foreach (var op in CustomClassOperations)
                {
                    yield return op;
                }
            }
        }

        protected virtual IEnumerable<Type> ImplementedInterfaces
        {
            get
            {
                if (typeof(T).IsInterface)
                {
                    yield return typeof(T);
                }

                foreach (var _interface in FacetInterfaces)
                {
                    yield return _interface;
                }
            }
        }

        protected virtual string GetBackingFieldName(PropertyInfo propertyInfo) => $"_{propertyInfo.Name.ToLowerInvariant()}";

        #endregion

        protected Type[] InterfacesWithSpecialBehaviours
        {
            get
            {
                return _interfacesWithSpecialBehaviours ??= SpecialBehaviours.Select(_ => _.Key)
                                                                                                                .ToArray();
            }
        }

        protected Type GetMappedType(PropertyInfo propertyInfo)
        {
            var mappedTypes = from mapper in TypeMaps
                              let mappedType = mapper(propertyInfo.PropertyType)
                              select mappedType;

            return mappedTypes.FirstOrDefault(_ => _ != null);
        }

        #region ClassOperations

        protected virtual TypeBuilder AddNonDefaultConstructors(TypeBuilder typeBuilder)
        {
            return typeBuilder;
        }

        #region Default Constructor

        protected virtual IEnumerable<Action<ILGenerator>> DefaultConstructorInstructionsCallBaseClassConstructor
        {
            get
            {
                yield return _ => _.Emit(OpCodes.Ldarg_0);
                yield return _ => _.Emit(OpCodes.Call,
                                         BaseType.GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
                                                                 null,
                                                                 Type.EmptyTypes,
                                                                 null));
            }
        }

        protected virtual IEnumerable<Action<ILGenerator>> DefaultConstructorInstructionsInitializeDefaultValueFromAttributes
        {
            get
            {
                object ResolveDefaultValue(PropertyInfo property, DefaultValueAttribute attribute)
                {
                    // If the attribute exposes a value directly, use it
                    if (attribute.Value != null)
                    {
                        return attribute.Value;
                    }

                    // If no public Value is available, attempt to inspect the attribute constructor args
                    // to support the (Type, string) constructor. If we can convert the string to the
                    // target property type via a TypeConverter, return the converted value. Otherwise
                    // treat this as an unsupported default and throw NotSupportedException so that
                    // type creation fails (matching legacy behavior expected by the tests).
                    var cad = CustomAttributeData.GetCustomAttributes(property).FirstOrDefault(a => a.AttributeType == typeof(DefaultValueAttribute));
                    if (cad != null && cad.ConstructorArguments.Count == 2)
                    {
                        var ctorArg0 = cad.ConstructorArguments[0];
                        var ctorArg1 = cad.ConstructorArguments[1];

                        if (ctorArg0.ArgumentType == typeof(Type) && ctorArg1.ArgumentType == typeof(string))
                        {
                            var targetType = ctorArg0.Value as Type;
                            var text = ctorArg1.Value as string;

                            // Try converting the string to the property's mapped/backing type
                            var propertyTargetType = BackingFieldProperties[property].BackingFieldType;
                            var converter = TypeDescriptor.GetConverter(propertyTargetType);
                            if (converter != null && converter.CanConvertFrom(typeof(string)))
                            {
                                try
                                {
                                    return converter.ConvertFrom(null, CultureInfo.InvariantCulture, text);
                                }
                                catch
                                {
                                    throw new NotSupportedException($"Cannot set default value for {property.Name}");
                                }
                            }

                            // No suitable converter -> unsupported
                            throw new NotSupportedException($"Cannot set default value for {property.Name}");
                        }
                    }

                    // No public value and no convertible ctor-data -> treat as null
                    return null;
                }

                var instructions = from property in typeof(T).GetAllProperties()
                                   let defaultValueAttribute = property.GetCustomAttribute<DefaultValueAttribute>()
                                   where defaultValueAttribute != null
                                   let defaultValue = ResolveDefaultValue(property, defaultValueAttribute)
                                   from instruction in FieldValueSetInstructionHelper.GenerateCodeToSetFieldValue(BackingFieldProperties[property].BackingField,
                                                                                                                  defaultValue)
                                   select instruction;

                return instructions;
            }
        }

        protected virtual IEnumerable<Action<ILGenerator>> DefaultConstructorInstructionsAddCustomConstructionInstructions
        {
            get
            {
                yield break;
            }
        }

        protected virtual IEnumerable<Action<ILGenerator>> DefaultConstructorInstructionsAddTrackReadonlyPropertyInitializedSupport
        {
            get
            {
                if (!TrackReadonlyPropertyInitialized)
                {
                    yield break;
                }

                yield return _ => _.Emit(OpCodes.Ldarg_0);
                yield return _ => _.Emit(OpCodes.Newobj,
                                         typeof(HashSet<>).MakeGenericType(typeof(String))
                                                           .GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
                                                                           null,
                                                                           Type.EmptyTypes,
                                                                           null));
                yield return _ => _.Emit(OpCodes.Stfld,
                                         InitializePropertyField);
            }
        }

        protected virtual IEnumerable<Action<ILGenerator>> DefaultConstructorInstructionsInitializeMappedReadonlyProperties
        {
            get
            {
                foreach (var readonlyProperty in BackingFieldProperties.Values.Where(_ => _.MappedType != null))
                {
                    var _readonlyProperty = readonlyProperty;

                    yield return _ => _.Emit(OpCodes.Ldarg_0);
                    yield return _ => _.Emit(OpCodes.Newobj,
                                             _readonlyProperty.MappedType.GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
                                                                                         null,
                                                                                         Type.EmptyTypes,
                                                                                         null));

                    yield return _ => _.Emit(OpCodes.Stfld,
                                             _readonlyProperty.BackingField);
                }
            }
        }

        protected virtual TypeBuilder AddDefaultConstructor(TypeBuilder typeBuilder)
        {
            var constructorBuilder = typeBuilder.DefineConstructor(MethodAttributes.Public | MethodAttributes.HideBySig,
                                                                   CallingConventions.Standard,
                                                                   null);

            var gen = constructorBuilder.GetILGenerator();

            foreach (var instruction in DefaultConstructorInstructionsInitializeMappedReadonlyProperties)
            {
                instruction(gen);
            }

            foreach (var instruction in DefaultConstructorInstructionsInitializeDefaultValueFromAttributes)
            {
                instruction(gen);
            }

            foreach (var instruction in DefaultConstructorInstructionsCallBaseClassConstructor)
            {
                instruction(gen);
            }

            foreach (var instruction in DefaultConstructorInstructionsAddTrackReadonlyPropertyInitializedSupport)
            {
                instruction(gen);
            }

            foreach (var instruction in DefaultConstructorInstructionsAddCustomConstructionInstructions)
            {
                instruction(gen);
            }

            gen.Emit(OpCodes.Nop);
            gen.Emit(OpCodes.Ret);

            return typeBuilder;
        }

        #endregion

        #region Fields

        protected virtual TypeBuilder AddFields(TypeBuilder typeBuilder)
        {
            foreach (var _backingFieldProperty in BackingFieldProperties.Values)
            {
                var _propertyInfo = _backingFieldProperty.PropertyInfo;

                _backingFieldProperty.BackingField = typeBuilder.DefineField(GetBackingFieldName(_propertyInfo),
                                                                             _propertyInfo.PropertyType,
                                                                             FieldAttributes.Private);
            }

            return typeBuilder;
        }

        #endregion

        #region Events

        protected virtual TypeBuilder AddEvents(TypeBuilder typeBuilder)
        {
            var events = typeof(T).GetAllNonExcludedEvents(InterfacesWithSpecialBehaviours);
            var tb = typeBuilder;
            foreach (var ev in events)
            {
                tb = AddEvent(tb,
                              ev,
                              tb.DefineField(ev.Name,
                                             ev.EventHandlerType,
                                             FieldAttributes.Private));
            }

            return tb;
        }

        protected virtual TypeBuilder AddEvent(TypeBuilder typeBuilder,
                                               EventInfo eventInfo,
                                               FieldBuilder backingFieldInfo)
        {
            var eventBuilder = typeBuilder.DefineEvent(eventInfo.Name,
                                                       EventAttributes.None,
                                                       eventInfo.EventHandlerType);

            AddEventAttachOrDetachMethod(typeBuilder,
                                         eventBuilder,
                                         eventInfo,
                                         backingFieldInfo,
                                         EventOperation.Attach);

            AddEventAttachOrDetachMethod(typeBuilder,
                                         eventBuilder,
                                         eventInfo,
                                         backingFieldInfo,
                                         EventOperation.Detach);

            foreach (var op in EventOperations)
            {
                eventBuilder = op(eventBuilder,
                                   eventInfo);
            }

            foreach (var op in EventFieldOperations)
            {
                backingFieldInfo = op(backingFieldInfo,
                                       backingFieldInfo);
            }

            return typeBuilder;
        }

        protected virtual void AddEventAttachOrDetachMethod(TypeBuilder typeBuilder,
                                                            EventBuilder eventBuilder,
                                                            EventInfo eventInfo,
                                                            FieldInfo backingFieldInfo,
                                                            EventOperation operation)
        {
            var attachOrDetachMethodName = string.Format(System.Globalization.CultureInfo.InvariantCulture,
                                                             operation == EventOperation.Attach
                                                                 ? "add_{0}"
                                                                 : "remove_{0}",
                                                         eventInfo.Name);
            var attachOrDetachDelegateName = operation == EventOperation.Attach
                                                 ? "Combine"
                                                 : "Remove";

            var methodBuilder = typeBuilder.DefineMethod(attachOrDetachMethodName,
                                                         MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.Final | MethodAttributes.HideBySig | MethodAttributes.NewSlot);

            methodBuilder.SetReturnType(typeof(void));
            methodBuilder.SetParameters(eventInfo.EventHandlerType);

            methodBuilder.DefineParameter(1,
                                          ParameterAttributes.None,
                                          "value");

            if (operation == EventOperation.Attach)
            {
                eventBuilder.SetAddOnMethod(methodBuilder);
            }
            else
            {
                eventBuilder.SetRemoveOnMethod(methodBuilder);
            }

            var gen = methodBuilder.GetILGenerator();

            gen.DeclareLocal(eventInfo.EventHandlerType);
            gen.DeclareLocal(eventInfo.EventHandlerType);
            gen.DeclareLocal(eventInfo.EventHandlerType);
            gen.DeclareLocal(typeof(Boolean));

            // Preparing labels
            var label7 = gen.DefineLabel();

            // Writing body
            gen.Emit(OpCodes.Ldarg_0);
            gen.Emit(OpCodes.Ldfld,
                     backingFieldInfo);
            gen.Emit(OpCodes.Stloc_0);
            gen.MarkLabel(label7);
            gen.Emit(OpCodes.Ldloc_0);
            gen.Emit(OpCodes.Stloc_1);
            gen.Emit(OpCodes.Ldloc_1);
            gen.Emit(OpCodes.Ldarg_1);
            gen.Emit(OpCodes.Call,
                     typeof(Delegate).GetMethod(attachOrDetachDelegateName,
                                                 BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic,
                                                 null,
                                                 new[]
                                                 {
                                                     typeof (Delegate),
                                                     typeof (Delegate)
                                                 },
                                                 null));
            gen.Emit(OpCodes.Castclass,
                     eventInfo.EventHandlerType);
            gen.Emit(OpCodes.Stloc_2);
            gen.Emit(OpCodes.Ldarg_0);
            gen.Emit(OpCodes.Ldflda,
                     backingFieldInfo);
            gen.Emit(OpCodes.Ldloc_2);
            gen.Emit(OpCodes.Ldloc_1);
            gen.Emit(OpCodes.Call,
                     typeof(Interlocked).GetGenericMethodOnType("CompareExchange",
                                                                 eventInfo.EventHandlerType));
            gen.Emit(OpCodes.Stloc_0);
            gen.Emit(OpCodes.Ldloc_0);
            gen.Emit(OpCodes.Ldloc_1);
            gen.Emit(OpCodes.Ceq);
            gen.Emit(OpCodes.Ldc_I4_0);
            gen.Emit(OpCodes.Ceq);
            gen.Emit(OpCodes.Stloc_3);
            gen.Emit(OpCodes.Ldloc_3);
            gen.Emit(OpCodes.Brtrue_S,
                     label7);
            gen.Emit(OpCodes.Ret);
        }

        protected enum EventOperation
        {
            Attach,

            Detach
        }

        #endregion

        #region Properties

        protected virtual TypeBuilder AddProperties(TypeBuilder typeBuilder)
        {
            var props = typeof(T).GetAllNonExcludedProperties(InterfacesWithSpecialBehaviours).Where(PropertyFilter);
            var tb = typeBuilder;
            foreach (var p in props)
            {
                tb = AddProperty(tb, p);
            }

            return tb;
        }

        protected virtual bool PropertyFilter(PropertyInfo propertyInfo)
        {
            return true;
        }

        protected virtual TypeBuilder AddProperty(TypeBuilder typeBuilder,
                                                  PropertyInfo propertyInfo)
        {
            var propertyBuilder = typeBuilder.DefineProperty(propertyInfo.Name,
                                                             PropertyAttributes.None,
                                                             propertyInfo.PropertyType,
                                                             null);

            AddPropertyAccessor(typeBuilder,
                                propertyBuilder,
                                propertyInfo);

            AddPropertyMutator(typeBuilder,
                               propertyBuilder,
                               propertyInfo);

            foreach (var op in PropertyOperations)
            {
                propertyBuilder = op(propertyBuilder,
                                     propertyInfo);
            }

            return typeBuilder;
        }

        protected virtual void AddPropertyAccessor(TypeBuilder typeBuilder,
                                                   PropertyBuilder propertyBuilder,
                                                   PropertyInfo propertyInfo)
        {
            var methodBuilder = typeBuilder.DefineMethod(string.Format(System.Globalization.CultureInfo.InvariantCulture, "get_{0}",
                                                                       propertyInfo.Name),
                                                         PropertyHiddenMethodAttributes);

            methodBuilder.SetReturnType(propertyInfo.PropertyType);

            GenerateCodeForAccessor(typeBuilder,
                                    methodBuilder,
                                    propertyInfo);

            propertyBuilder.SetGetMethod(methodBuilder);

            if (propertyInfo.GetGetMethod() != null)
            {
                typeBuilder.DefineMethodOverride(methodBuilder,
                                                 propertyInfo.GetGetMethod());
            }
        }

        protected virtual void GenerateCodeForAccessor(TypeBuilder typeBuilder,
                                                       MethodBuilder methodBuilder,
                                                       PropertyInfo propertyInfo)
        {
            var backingFieldProperty = BackingFieldProperties[propertyInfo];
            if (TrackReadonlyPropertyInitialized
                && backingFieldProperty.IsReadOnly
                && backingFieldProperty.MappedType == null)
            {
                GenerateCodeForTrackedAccessor(typeBuilder,
                                               methodBuilder,
                                               propertyInfo);
            }
            else
            {
                GenerateCodeForNormalAccessor(typeBuilder,
                                              methodBuilder,
                                              propertyInfo);
            }
        }

        protected virtual void GenerateCodeForNormalAccessor(TypeBuilder typeBuilder,
                                                             MethodBuilder methodBuilder,
                                                             PropertyInfo propertyInfo)
        {
            var backingFieldInfo = BackingFieldProperties[propertyInfo].BackingField;

            var gen = methodBuilder.GetILGenerator();

            gen.Emit(OpCodes.Nop);
            gen.Emit(OpCodes.Ldarg_0);
            gen.Emit(OpCodes.Ldfld,
                     backingFieldInfo);
            gen.Emit(OpCodes.Ret);
        }

        protected virtual void GenerateCodeForTrackedAccessor(TypeBuilder typeBuilder,
                                                              MethodBuilder methodBuilder,
                                                              PropertyInfo propertyInfo)
        {
            var gen = methodBuilder.GetILGenerator();
            gen.DeclareLocal(typeof(Int32));
            gen.DeclareLocal(typeof(Boolean));

            var getAndReturnValue = gen.DefineLabel();
            var exit = gen.DefineLabel();

            gen.Emit(OpCodes.Nop);
            gen.Emit(OpCodes.Ldarg_0);
            gen.Emit(OpCodes.Ldstr,
                     propertyInfo.Name);
            gen.Emit(OpCodes.Call,
                     IsPropertyValueInitializedMethod);
            gen.Emit(OpCodes.Stloc_1);
            gen.Emit(OpCodes.Ldloc_1);
            gen.Emit(OpCodes.Brtrue_S,
                     getAndReturnValue);
            gen.Emit(OpCodes.Nop);
            gen.Emit(OpCodes.Ldstr,
                     propertyInfo.Name);
            gen.Emit(OpCodes.Newobj,
                     typeof(MethodAccessException).GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
                                                                   null,
                                                                   new[]
                                                                   {
                                                                       typeof (String)
                                                                   },
                                                                   null));
            gen.Emit(OpCodes.Throw);
            gen.MarkLabel(getAndReturnValue);
            gen.Emit(OpCodes.Ldarg_0);
            gen.Emit(OpCodes.Ldfld,
                     BackingFieldProperties[propertyInfo].BackingField);
            gen.Emit(OpCodes.Stloc_0);
            gen.Emit(OpCodes.Br_S,
                     exit);
            gen.MarkLabel(exit);
            gen.Emit(OpCodes.Ldloc_0);
            gen.Emit(OpCodes.Ret);
        }

        protected virtual void AddPropertyMutator(TypeBuilder typeBuilder,
                                                  PropertyBuilder propertyBuilder,
                                                  PropertyInfo propertyInfo)
        {
            var backingFieldProperty = BackingFieldProperties[propertyInfo];
            if (backingFieldProperty.IsReadOnly
                && (backingFieldProperty.MappedType != null))
            {
                return;
            }

            var methodBuilder = typeBuilder.DefineMethod(string.Format(System.Globalization.CultureInfo.InvariantCulture, "set_{0}",
                                                                       propertyInfo.Name),
                                                         PropertyHiddenMethodAttributes,
                                                         null,
                                                         new[]
                                                         {
                                                             propertyInfo.PropertyType
                                                         });
            methodBuilder.DefineParameter(1,
                                          ParameterAttributes.None,
                                          "value");

            GenerateCodeForMutator(typeBuilder,
                                   methodBuilder,
                                   propertyInfo);

            propertyBuilder.SetSetMethod(methodBuilder);

            if (propertyInfo.GetSetMethod() != null)
            {
                typeBuilder.DefineMethodOverride(methodBuilder,
                                                 propertyInfo.GetSetMethod());
            }
        }

        protected virtual void GenerateCodeForMutator(TypeBuilder typeBuilder,
                                                      MethodBuilder methodBuilder,
                                                      PropertyInfo propertyInfo)
        {
            var backingFieldProperty = BackingFieldProperties[propertyInfo];

            var propertyChangingNotificationRequired = typeof(INotifyPropertyChanging).IsAssignableFrom(BaseType);
            var propertyChangedNotificationRequired = typeof(INotifyPropertyChanged).IsAssignableFrom(BaseType);

            Debug.Assert(!backingFieldProperty.IsReadOnly || backingFieldProperty.MappedType == null);

            Action<TypeBuilder, MethodBuilder, PropertyInfo> changedTrackedMethodGenerator = (_typeBuilder,
                                                                                              _methodBuilder,
                                                                                              _propertyBuilder) => GenerateCodeForChangeTrackedMutator(_typeBuilder,
                                                                                                                                                       _methodBuilder,
                                                                                                                                                       _propertyBuilder,
                                                                                                                                                       propertyChangingNotificationRequired,
                                                                                                                                                       propertyChangedNotificationRequired);
            var mutatorGenerator = backingFieldProperty.IsReadOnly
                                       ? (TrackReadonlyPropertyInitialized
                                              ? (Action<TypeBuilder, MethodBuilder, PropertyInfo>)GenerateCodeForTrackedMutator
                                              : GenerateCodeForNormalMutator)
                                       : (propertyChangingNotificationRequired || propertyChangedNotificationRequired)
                                             ? changedTrackedMethodGenerator
                                             : GenerateCodeForNormalMutator;

            mutatorGenerator(typeBuilder,
                             methodBuilder,
                             propertyInfo);
        }

        protected virtual void GenerateCodeForNormalMutator(TypeBuilder typeBuilder,
                                                            MethodBuilder methodBuilder,
                                                            PropertyInfo propertyInfo)
        {
            var gen = methodBuilder.GetILGenerator();

            gen.Emit(OpCodes.Ldarg_0);
            gen.Emit(OpCodes.Ldarg_1);
            gen.Emit(OpCodes.Stfld,
                     BackingFieldProperties[propertyInfo].BackingField);
            gen.Emit(OpCodes.Ret);
        }

        protected virtual void GenerateCodeForTrackedMutator(TypeBuilder typeBuilder,
                                                             MethodBuilder methodBuilder,
                                                             PropertyInfo propertyInfo)
        {
            var gen = methodBuilder.GetILGenerator();

            // Writing body
            gen.Emit(OpCodes.Nop);

            gen.Emit(OpCodes.Ldarg_0);
            gen.Emit(OpCodes.Ldstr,
                     "Id");
            gen.Emit(OpCodes.Call,
                     InitializePropertyValueMethod);

            gen.Emit(OpCodes.Nop);

            gen.Emit(OpCodes.Ldarg_0);
            gen.Emit(OpCodes.Ldarg_1);
            gen.Emit(OpCodes.Stfld,
                     BackingFieldProperties[propertyInfo].BackingField);
            gen.Emit(OpCodes.Ret);
        }

        protected virtual void GenerateCodeForChangeTrackedMutator(TypeBuilder typeBuilder,
                                                                   MethodBuilder methodBuilder,
                                                                   PropertyInfo propertyInfo,
                                                                   bool propertyChangingNotificationRequired,
                                                                   bool propertyChangedNotificationRequired)
        {
            var backingFieldInfo = BackingFieldProperties[propertyInfo].BackingField;
            var gen = methodBuilder.GetILGenerator();

            // Preparing labels
            var labelExit = gen.DefineLabel();

            gen.Emit(OpCodes.Nop);

            if (propertyChangedNotificationRequired)
            {
                gen.DeclareLocal(propertyInfo.PropertyType);
            }

            if (propertyChangingNotificationRequired)
            {
                gen.DeclareLocal(typeof(Boolean));
            }

            var loadCurrentValueOpCode = OpCodes.Nop;
            var storeCurrentValueOpCode = OpCodes.Nop;
            var storeBooleanFlagOpCode = OpCodes.Nop;
            var loadBooleanFlagOpCode = OpCodes.Nop;

            if (propertyChangingNotificationRequired && propertyChangedNotificationRequired)
            {
                storeCurrentValueOpCode = OpCodes.Stloc_0;
                loadCurrentValueOpCode = OpCodes.Ldloc_0;
                storeBooleanFlagOpCode = OpCodes.Stloc_1;
                loadBooleanFlagOpCode = OpCodes.Ldloc_1;
            }
            else if (propertyChangedNotificationRequired)
            {
                storeCurrentValueOpCode = OpCodes.Stloc_0;
                loadCurrentValueOpCode = OpCodes.Ldloc_0;
            }
            else if (propertyChangingNotificationRequired)
            {
                storeBooleanFlagOpCode = OpCodes.Stloc_0;
                loadBooleanFlagOpCode = OpCodes.Ldloc_0;
            }

            #region OnPropertyChanging support

            if (propertyChangingNotificationRequired)
            {
                #region (push (this.OnPropertyChanging("MutableValue", typeof(decimal), this._mutableValue, value)))

                // "FieldName"
                gen.Emit(OpCodes.Ldarg_0);
                gen.Emit(OpCodes.Ldstr,
                         propertyInfo.Name);

                // typeof() OR propertyInfo.PropertyType.GetType()
                gen.Emit(OpCodes.Ldtoken,
                         propertyInfo.PropertyType);
                gen.Emit(OpCodes.Call,
                         typeof(Type).GetMethod("GetTypeFromHandle",
                                                 BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic,
                                                 null,
                                                 GetTypeFromHandleArgTypes,
                                                 null));

                // this._fieldName
                gen.Emit(OpCodes.Ldarg_0);
                gen.Emit(OpCodes.Ldfld,
                         backingFieldInfo);
                if (propertyInfo.PropertyType.IsValueType)
                {
                    gen.Emit(OpCodes.Box,
                             propertyInfo.PropertyType);
                }

                // value
                gen.Emit(OpCodes.Ldarg_1);
                if (propertyInfo.PropertyType.IsValueType)
                {
                    gen.Emit(OpCodes.Box,
                             propertyInfo.PropertyType);
                }

                // (OnPropertyChanging propertyName propertyType oldValue newValue)
                gen.Emit(OpCodes.Callvirt,
                         BaseType.GetMethod("OnPropertyChanging",
                                            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
                                            null,
                                            OnPropertyChangingArgTypes,
                                            null));

                #endregion

                #region IF ((pop) == 0) <> TRUE) GOTO :exit

                gen.Emit(OpCodes.Ldc_I4_0);
                gen.Emit(OpCodes.Ceq);
                gen.Emit(storeBooleanFlagOpCode);
                gen.Emit(loadBooleanFlagOpCode);
                gen.Emit(OpCodes.Brtrue_S,
                         labelExit);

                #endregion
            }

            #endregion

            if (propertyChangedNotificationRequired)
            {
                gen.Emit(OpCodes.Nop);
                gen.Emit(OpCodes.Ldarg_0);
                gen.Emit(OpCodes.Ldfld,
                         backingFieldInfo);
                gen.Emit(storeCurrentValueOpCode);
            }

            #region this._fieldInfo = value

            gen.Emit(OpCodes.Nop);
            gen.Emit(OpCodes.Ldarg_0);
            gen.Emit(OpCodes.Ldarg_1);
            gen.Emit(OpCodes.Stfld,
                     backingFieldInfo);

            #endregion

            #region OnPropertyChanged support

            if (propertyChangedNotificationRequired)
            {
                #region (push (this.OnPropertyChanging("MutableValue", typeof(decimal), this._mutableValue, value)))

                // "FieldName"
                gen.Emit(OpCodes.Ldarg_0);
                gen.Emit(OpCodes.Ldstr,
                         propertyInfo.Name);

                // typeof() OR propertyInfo.PropertyType.GetType()
                gen.Emit(OpCodes.Ldtoken,
                         propertyInfo.PropertyType);
                gen.Emit(OpCodes.Call,
                         typeof(Type).GetMethod("GetTypeFromHandle",
                                                 BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic,
                                                 null,
                                                 GetTypeFromHandleArgTypes,
                                                 null));

                // <old> value
                gen.Emit(loadCurrentValueOpCode);
                if (propertyInfo.PropertyType.IsValueType)
                {
                    gen.Emit(OpCodes.Box,
                             propertyInfo.PropertyType);
                }

                // value
                gen.Emit(OpCodes.Ldarg_1);
                if (propertyInfo.PropertyType.IsValueType)
                {
                    gen.Emit(OpCodes.Box,
                             propertyInfo.PropertyType);
                }

                // (OnPropertyChanged propertyName propertyType oldValue newValue)
                gen.Emit(OpCodes.Callvirt,
                         BaseType.GetMethod("OnPropertyChanged",
                                            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
                                            null,
                                            OnPropertyChangingArgTypes,
                                            null));

                #endregion
            }

            #endregion

            gen.Emit(OpCodes.Nop);
            gen.MarkLabel(labelExit);
            gen.Emit(OpCodes.Ret);
        }

        #endregion

        #region Methods

        protected virtual TypeBuilder AddMethods(TypeBuilder typeBuilder)
        {
            return typeof(T).GetAllNonExcludedMethods(InterfacesWithSpecialBehaviours)
                             .Where(MethodFilter)
                             .Where(_ => !_.IsSpecialName)
                             .Aggregate(typeBuilder,
                                        AddMethod);
        }

        protected virtual bool MethodFilter(MethodInfo methodInfo)
        {
            return true;
        }

        protected virtual TypeBuilder AddMethod(TypeBuilder typeBuilder,
                                                MethodInfo methodInfo)
        {
            var methodBuilder = typeBuilder.DefineMethod(methodInfo.Name,
                                                         MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.NewSlot | MethodAttributes.SpecialName | MethodAttributes.HideBySig);

            if (methodInfo.IsGenericMethod)
            {
                var genericArguments = methodInfo.GetGenericArguments();

                var genericTypeParameterBuilders = methodBuilder.DefineGenericParameters(genericArguments.Select(_ => _.Name)
                                                                                                         .ToArray());

                foreach (var typeParameter in genericTypeParameterBuilders)
                {
                    var genericArgument = genericArguments.Single(_ => _.Name == typeParameter.Name);
                    typeParameter.SetGenericParameterAttributes(genericArgument.GenericParameterAttributes);

                    var parameterConstraints = genericArgument.GetGenericParameterConstraints();

                    var baseTypeConstraint = parameterConstraints.FirstOrDefault(_ => _.IsClass);
                    var interfaceConstraints = parameterConstraints.Where(_ => _.IsInterface)
                                                                   .ToArray();

                    if (baseTypeConstraint != null)
                    {
                        typeParameter.SetBaseTypeConstraint(baseTypeConstraint);
                    }

                    typeParameter.SetInterfaceConstraints(interfaceConstraints);
                }
            }

            methodBuilder.SetReturnType(methodInfo.ReturnType);

            methodBuilder.SetParameters(methodInfo.GetParameters()
                                                  .Select(_ => _.ParameterType)
                                                  .ToArray());

            foreach (var param in methodInfo.GetParameters()
                                            .OrderBy(_ => _.Position))
            {
                var _p = methodBuilder.DefineParameter(param.Position + 1,
                                                       param.Attributes,
                                                       param.Name);

                const BindingFlags parameterBindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

                if (param.IsOut)
                {
                    _p.SetCustomAttribute(new CustomAttributeBuilder(typeof(OutAttribute).GetConstructor(parameterBindingFlags,
                                                                                                          null,
                                                                                                          Type.EmptyTypes,
                                                                                                          null),
                                                                     []));
                }
                else if (param.IsDefined(typeof(ParamArrayAttribute),
                                         false))
                {
                    _p.SetCustomAttribute(new CustomAttributeBuilder(typeof(ParamArrayAttribute).GetConstructor(parameterBindingFlags,
                                                                                                                 null,
                                                                                                                 Type.EmptyTypes,
                                                                                                                 null),
                                                                     []));
                }
            }

            GenerateMethodBody(methodBuilder);

            foreach (var op in MethodOperations)
            {
                methodBuilder = op(methodBuilder,
                                    methodInfo);
            }

            return typeBuilder;
        }

        protected virtual void GenerateMethodBody(MethodBuilder methodBuilder)
        {
            var gen = methodBuilder.GetILGenerator();

            // Writing body
            gen.Emit(OpCodes.Nop);
            gen.Emit(OpCodes.Newobj,
                     typeof(NotImplementedException).GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
                                                                     null,
                                                                     Type.EmptyTypes,
                                                                     null));
            gen.Emit(OpCodes.Throw);
        }

        #endregion

        #region TrackReadonlyPropertyInitialization

        protected FieldBuilder InitializePropertyField { get; private set; }

        protected MethodBuilder InitializePropertyValueMethod { get; private set; }

        protected MethodBuilder IsPropertyValueInitializedMethod { get; private set; }

        protected TypeBuilder AddInitializePropertyTrackingSupport(TypeBuilder typeBuilder)
        {
            Debug.Assert(TrackReadonlyPropertyInitialized);

            InitializePropertyField = BuildField_InitializedProperties(typeBuilder);

            InitializePropertyValueMethod = BuildMethodInitializePropertyValue(typeBuilder,
                                                                               InitializePropertyField);

            IsPropertyValueInitializedMethod = BuildMethodIsPropertyValueInitialized(typeBuilder,
                                                                                     InitializePropertyField);

            return typeBuilder;
        }

        private static MethodBuilder BuildMethodInitializePropertyValue(TypeBuilder type,
                                                                        FieldInfo initializedPropertiesField)
        {
            var method = type.DefineMethod("InitializePropertyValue",
                                           MethodAttributes.Family | MethodAttributes.HideBySig,
                                           typeof(void),
                                           new[]
                                           {
                                               typeof (string)
                                           });

            method.DefineParameter(1,
                                   ParameterAttributes.None,
                                   "propertyName");

            var gen = method.GetILGenerator();

            gen.Emit(OpCodes.Nop);
            gen.Emit(OpCodes.Ldarg_0);
            gen.Emit(OpCodes.Ldfld,
                     initializedPropertiesField);
            gen.Emit(OpCodes.Ldarg_1);
            gen.Emit(OpCodes.Callvirt,
                     typeof(HashSet<>).MakeGenericType(typeof(string))
                                       .GetMethod("Add",
                                                  BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
                                                  null,
                                                  new[]
                                                  {
                                                      typeof (string)
                                                  },
                                                  null));
            gen.Emit(OpCodes.Pop);
            gen.Emit(OpCodes.Ret);
            return method;
        }

        private static MethodBuilder BuildMethodIsPropertyValueInitialized(TypeBuilder type,
                                                                           FieldInfo initializedPropertiesField)
        {
            var method = type.DefineMethod("IsPropertyValueInitialized",
                                           MethodAttributes.Family | MethodAttributes.HideBySig,
                                           typeof(Boolean),
                                           new[]
                                           {
                                               typeof (String)
                                           });
            method.DefineParameter(1,
                                   ParameterAttributes.None,
                                   "propertyName");
            var gen = method.GetILGenerator();

            gen.DeclareLocal(typeof(Boolean));

            var label16 = gen.DefineLabel();

            gen.Emit(OpCodes.Nop);
            gen.Emit(OpCodes.Ldarg_0);
            gen.Emit(OpCodes.Ldfld,
                     initializedPropertiesField);
            gen.Emit(OpCodes.Ldarg_1);
            gen.Emit(OpCodes.Callvirt,
                     typeof(HashSet<>).MakeGenericType(typeof(String))
                                       .GetMethod("Contains",
                                                  BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
                                                  null,
                                                  new[]
                                                  {
                                                      typeof (string)
                                                  },
                                                  null));
            gen.Emit(OpCodes.Stloc_0);
            gen.Emit(OpCodes.Br_S,
                     label16);
            gen.MarkLabel(label16);
            gen.Emit(OpCodes.Ldloc_0);
            gen.Emit(OpCodes.Ret);

            return method;
        }

        private static FieldBuilder BuildField_InitializedProperties(TypeBuilder type)
        {
            return type.DefineField("_initializedProperties",
                                    typeof(HashSet<>).MakeGenericType(typeof(String)),
                                    FieldAttributes.Private);
        }

        #endregion

        #endregion

        #region Instance Creation

        protected virtual Func<T> InstanceFactory => _instanceFactory ??= BuildInstanceFactory();

        public virtual T CreateInstance(dynamic source = null)
        {
            var result = InstanceFactory();

            if (source != null)
            {
                FastMapper<T>.MapDynamicToStatic(source,
                                                 result);
            }

            return result;
        }

        protected virtual Func<T> BuildInstanceFactory()
        {
            var ctor = Type.GetConstructor(Type.EmptyTypes);
            Debug.Assert(ctor != null);

            var lambda = Expression.Lambda(Expression.New(ctor));

            return (Func<T>)lambda.Compile();
        }

        #endregion

        #region Type Creation

        public virtual Type Type
        {
            get { return _type ??= BuildType(); }
        }

        protected virtual Type BuildType()
        {
            if (typeof(T).IsClass)
            {
                return typeof(T);
            }

            var assemblyName = new AssemblyName(AssemblyName);

            var assemblyBuilder = GetAssemblyBuilder(assemblyName);

            var moduleBuilder = GetModuleBuilder(assemblyBuilder);

            var typeBuilder = BuildTypeBuilder(moduleBuilder);

            try
            {
                return typeBuilder.CreateType();
            }
            finally
            {
                PersistAssembly(assemblyBuilder);
            }
        }

        protected virtual ModuleBuilder GetModuleBuilder(AssemblyBuilder assemblyBuilder)
        {
            // In .NET Core / .NET 5+ the DefineDynamicModule overload that accepts a file name is available
            // on the AssemblyBuilder returned by DefineDynamicAssembly when emitting to disk is supported.
            return assemblyBuilder.DefineDynamicModule(AssemblyName);
        }

        protected virtual string GetAssemblyDllName()
        {
            return string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0}.dll",
                                 AssemblyName);
        }

        protected virtual AssemblyBuilder GetAssemblyBuilder(AssemblyName assemblyName)
        {
            // Use the modern API to define a dynamic assembly
            return AssemblyBuilder.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
        }

        [ExcludeFromCodeCoverage]
        protected virtual void PersistAssembly(AssemblyBuilder assemblyBuilder)
        {
            // Saving dynamic assemblies to disk is not supported on all runtimes; skip when unavailable.
            // If SaveAssemblyToDisk is true and the runtime supports saving, this method can be overridden
            // in a derived type to provide the behavior.
            return;
        }

        protected virtual TypeBuilder BuildTypeBuilder(ModuleBuilder moduleBuilder)
        {
            var implementedInterfaces = ImplementedInterfaces.ToArray();

            var typeBuilder = moduleBuilder.DefineType(ClassName,
                                                       TypeAttributes.Class | TypeAttributes.Public,
                                                       BaseType,
                                                       implementedInterfaces);

            var applicableInterfaces = FacetInterfaces.Union(new[]
                                                             {
                                                                 typeof (T)
                                                             });

            var behaviourOperations = from behaviour in SpecialBehaviours
                                      from _interface in applicableInterfaces
                                      where behaviour.Key.IsAssignableFrom(_interface)
                                      from operation in behaviour.Value.Operations
                                      select operation;

            typeBuilder = behaviourOperations.Aggregate(typeBuilder,
                                                        (_typeBuilder,
                                                         _function) => _function(_typeBuilder));

            return ClassOperations.Aggregate(typeBuilder,
                                             (_typeBuilder,
                                              _function) => _function(_typeBuilder));
        }

        #endregion
    }

    public class PropertyBackingFieldMap
    {
        public bool IsReadOnly { get; set; }
        public Type BackingFieldType { get; set; }
        public PropertyInfo PropertyInfo { get; set; }
        public Type MappedType { get; set; }
        //public Func<TypeBuilder, FieldBuilder> BackingFieldGenerator { get; set; }
        public FieldBuilder BackingField { get; set; }
    }
}
