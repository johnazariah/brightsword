using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using System.Threading;
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
    /// <summary>
    /// Produces runtime-generated DTO-like implementations for interface types.
    /// Instances created by this class can implement interfaces, inject facet interfaces,
    /// and include behaviors such as clone support or change-tracking.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This type uses Reflection.Emit to build a concrete class at runtime that implements
    /// the requested interface <typeparamref name="T"/>. The generated class can be
    /// customized by overriding protected members such as <see cref="ClassOperations"/>, 
    /// <see cref="PropertyOperations"/>, and by supplying facet interfaces via the
    /// <see cref="FacetInterfaces"/> property.
    /// </para>
    /// <para>
    /// The generator also supports a small set of behaviours (see <see cref="SpecialBehaviours"/>)
    /// which can add methods, attributes, or other modifications to the emitted type.
    /// </para>
    /// </remarks>
    /// <example>
    /// <code>
    /// // Create a simple implementation for an interface and get a new instance
    /// var creator = new BasicDataTransferObjectTypeCreator<IMyDto>();
    /// var instance = creator.CreateInstance();
    /// // The instance is assignable to the interface
    /// Debug.Assert(instance is IMyDto);
    /// </code>
    /// </example>
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
        private bool _saveAssemblyToDisk;
        private readonly IEnumerable<KeyValuePair<Type, IBehaviour>> _specialBehaviours;
        private bool _trackReadonlyPropertyInitialized;
        private Type _type;
        // Cached Type[] arrays used for reflection GetMethod calls to avoid repeated allocations (CA1861)
        private static readonly Type[] OnPropertyChangingArgTypes = new[] { typeof(string), typeof(Type), typeof(object), typeof(object) };
        private static readonly Type[] GetTypeFromHandleArgTypes = new[] { typeof(RuntimeTypeHandle) };

        private Type[] _interfacesWithSpecialBehaviours;

        /// <summary>
        /// Initializes a new instance of the <see cref="BasicDataTransferObjectTypeCreator{T}"/> class
        /// with default mapping and behaviour registration.
        /// </summary>
        protected BasicDataTransferObjectTypeCreator()
        {
            _saveAssemblyToDisk = true;

            _trackReadonlyPropertyInitialized = false;

            _interfaceName = typeof(T).IsInterface ? typeof(T).PrintableName() : string.Empty;

            _className = typeof(T).RenameToConcreteType();

            _baseType = typeof(T).IsClass ? typeof(T) : typeof(object);

            _assemblyName = $"Dynamic.{GetType().GetNonGenericPartOfClassName()}.{_interfaceName}";

            _facetInterfaces = Array.Empty<Type>();

            _fieldValueSetInstructionHelper = new FieldValueSetInstructionHelper();

            _specialBehaviours = new[]
            {
                new KeyValuePair<Type, IBehaviour>(typeof(ICloneable), new CloneBehaviour())
            };

            _typeMaps = new List<Func<Type, Type>>
            {
                t => t.MapGenericTypeIfPossible(typeof(Dictionary<,>), typeof(IDictionary<,>)),
                t => t.MapGenericTypeIfPossible(typeof(HashSet<>), typeof(ISet<>)),
                t => t.MapGenericTypeIfPossible(typeof(List<>), typeof(IList<>), typeof(ICollection<>), typeof(IEnumerable<>))
            };
        }

        /// <summary>
        /// Initializes a new instance and appends user-supplied type mappers.
        /// </summary>
        /// <param name="userSuppliedTypeMaps">Optional type mapping functions to be used when deciding backing types.</param>
        public BasicDataTransferObjectTypeCreator(params Func<Type, Type>[] userSuppliedTypeMaps)
            : this()
        {
            foreach (var userSuppliedTypeMap in userSuppliedTypeMaps)
            {
                _typeMaps.Add(userSuppliedTypeMap);
            }
        }

        /// <summary>
        /// When true, generated types include support to track initialization of read-only properties.
        /// </summary>
        public virtual bool TrackReadonlyPropertyInitialized
        {
            get => _trackReadonlyPropertyInitialized;
            set => _trackReadonlyPropertyInitialized = value;
        }

        /// <summary>
        /// When true, assemblies may be persisted to disk by overrides of <see cref="PersistAssembly"/>.
        /// </summary>
        public virtual bool SaveAssemblyToDisk
        {
            get => _saveAssemblyToDisk;
            set => _saveAssemblyToDisk = value;
        }

        /// <summary>
        /// Gets or sets the logical name used for the dynamic assembly used during emission.
        /// </summary>
        public virtual string AssemblyName
        {
            get => _assemblyName;
            set => _assemblyName = value;
        }

        /// <summary>
        /// The printable interface name used for diagnostics and assembly naming.
        /// </summary>
        public virtual string InterfaceName
        {
            get => _interfaceName;
            set => _interfaceName = value;
        }

        /// <summary>
        /// The generated class name to use when emitting the concrete type.
        /// </summary>
        public virtual string ClassName
        {
            get => _className;
            set => _className = value;
        }

        /// <summary>
        /// The base type the emitted type will inherit from.
        /// </summary>
        public virtual Type BaseType
        {
            get => _baseType;
            set => _baseType = value;
        }

        /// <summary>
        /// Additional facet interfaces to implement on the emitted type.
        /// </summary>
        public virtual IEnumerable<Type> FacetInterfaces
        {
            get => _facetInterfaces;
            set => _facetInterfaces = value;
        }

        /// <summary>
        /// Helper used to generate IL instructions for setting field default values.
        /// </summary>
        public virtual FieldValueSetInstructionHelper FieldValueSetInstructionHelper
        {
            get => _fieldValueSetInstructionHelper;
            set => _fieldValueSetInstructionHelper = value;
        }

        /// <summary>
        /// Special behaviours registered by the type creator (mapping behaviour key to behaviour instance).
        /// </summary>
        public virtual IEnumerable<KeyValuePair<Type, IBehaviour>> SpecialBehaviours => _specialBehaviours;

        protected virtual IEnumerable<Func<Type, Type>> TypeMaps => _typeMaps;

        protected virtual IDictionary<PropertyInfo, PropertyBackingFieldMap> BackingFieldProperties
        {
            get => _backingFieldPropertyMap ??= (from _propertyInfo in typeof(T).GetAllNonExcludedProperties()
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
                                                 }).ToDictionary(_ => _.PropertyInfo, _ => _);
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
                yield return (builder, info) => builder.AddCustomAttribute<NonSerializedAttribute>();
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
        
        protected Type[] InterfacesWithSpecialBehaviours => _interfacesWithSpecialBehaviours ??= SpecialBehaviours.Select(kvp => kvp.Key).ToArray();

        protected Type GetMappedType(PropertyInfo propertyInfo)
        {
            var mappedTypes = from mapper in TypeMaps
                              let mappedType = mapper(propertyInfo.PropertyType)
                              select mappedType;

            return mappedTypes.FirstOrDefault(_ => _ != null);
        }

        protected virtual TypeBuilder AddNonDefaultConstructors(TypeBuilder typeBuilder) => typeBuilder;

        protected virtual IEnumerable<Action<ILGenerator>> DefaultConstructorInstructionsCallBaseClassConstructor
        {
            get
            {
                yield return _ => _.Emit(OpCodes.Ldarg_0);
                yield return _ => _.Emit(OpCodes.Call, BaseType.GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, Type.EmptyTypes, null));
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

                return
                    from property in typeof(T).GetAllProperties()
                    let defaultValueAttribute = property.GetCustomAttribute<DefaultValueAttribute>()
                    where defaultValueAttribute != null
                    let defaultValue = ResolveDefaultValue(property, defaultValueAttribute)
                    from instruction in FieldValueSetInstructionHelper.GenerateCodeToSetFieldValue(BackingFieldProperties[property].BackingField, defaultValue)
                    select instruction;
            }
        }

        /// <summary>
        /// Allows subclasses to inject arbitrary IL construction instructions into the default constructor.
        /// </summary>
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
                                         typeof(HashSet<string>).GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, Type.EmptyTypes, null));
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
                                             _readonlyProperty.MappedType.GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, Type.EmptyTypes, null));

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

            foreach (var instruction in DefaultConstructorInstructionsAddCustomConstructionInstructions
            )
            {
                instruction(gen);
            }

            gen.Emit(OpCodes.Nop);
            gen.Emit(OpCodes.Ret);

            return typeBuilder;
        }

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
            var attachOrDetachMethodName = string.Format(CultureInfo.InvariantCulture,
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

            _ = methodBuilder.DefineParameter(1,
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

            _ = gen.DeclareLocal(eventInfo.EventHandlerType);
            _ = gen.DeclareLocal(eventInfo.EventHandlerType);
            _ = gen.DeclareLocal(eventInfo.EventHandlerType);
            _ = gen.DeclareLocal(typeof(bool));

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
                                                 new[] { typeof(Delegate), typeof(Delegate) },
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

        /// <summary>
        /// Adds properties declared on the interface to the emitted type.
        /// </summary>
        /// <param name="typeBuilder">The active <see cref="TypeBuilder"/>.</param>
        /// <returns>The modified <see cref="TypeBuilder"/>.</returns>
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

        /// <summary>
        /// Filter invoked for each property discovered on the interface. Subclasses may override
        /// to exclude specific properties from emission.
        /// </summary>
        /// <param name="propertyInfo">The property being considered.</param>
        /// <returns>True to emit the property; false to skip it.</returns>
        protected virtual bool PropertyFilter(PropertyInfo propertyInfo) => true;

        /// <summary>
        /// Adds a single property (get/set accessors) to the emitted type.
        /// </summary>
        /// <param name="typeBuilder">The active <see cref="TypeBuilder"/>.</param>
        /// <param name="propertyInfo">The property metadata to implement.</param>
        /// <returns>The modified <see cref="TypeBuilder"/>.</returns>
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

        /// <summary>
        /// Adds the getter accessor method for a property, wiring up overrides when the
        /// interface declares a specific method implementation requirement.
        /// </summary>
        /// <param name="typeBuilder">The active <see cref="TypeBuilder"/>.</param>
        /// <param name="propertyBuilder">The property builder.</param>
        /// <param name="propertyInfo">The property metadata.</param>
        protected virtual void AddPropertyAccessor(TypeBuilder typeBuilder,
                                                   PropertyBuilder propertyBuilder,
                                                   PropertyInfo propertyInfo)
        {
            var methodBuilder = typeBuilder.DefineMethod(string.Format(CultureInfo.InvariantCulture, "get_{0}",
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

        /// <summary>
        /// Emits IL for a property getter, choosing tracked or normal accessor generation
        /// based on the property's backing configuration.
        /// </summary>
        /// <param name="typeBuilder">The active <see cref="TypeBuilder"/>.</param>
        /// <param name="methodBuilder">The getter <see cref="MethodBuilder"/>.</param>
        /// <param name="propertyInfo">The property metadata.</param>
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

        /// <summary>
        /// Emits a simple getter that loads and returns the backing field value.
        /// </summary>
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

        /// <summary>
        /// Emits a getter that verifies the property has been initialized before returning
        /// its value; throws <see cref="MethodAccessException"/> when the initialization
        /// check fails.
        /// </summary>
        protected virtual void GenerateCodeForTrackedAccessor(TypeBuilder typeBuilder,
                                                              MethodBuilder methodBuilder,
                                                              PropertyInfo propertyInfo)
        {
            var gen = methodBuilder.GetILGenerator();
            _ = gen.DeclareLocal(typeof(int));
            _ = gen.DeclareLocal(typeof(bool));

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
                                                                   new[] { typeof(string) },
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

        /// <summary>
        /// Adds the setter accessor for a property when the interface declares one.
        /// Read-only mapped properties are skipped.
        /// </summary>
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

            var methodBuilder = typeBuilder.DefineMethod(string.Format(CultureInfo.InvariantCulture, "set_{0}",
                                                                       propertyInfo.Name),
                                                         PropertyHiddenMethodAttributes,
                                                         null,
                                                         new[] { propertyInfo.PropertyType });
            _ = methodBuilder.DefineParameter(1,
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

        /// <summary>
        /// Chooses and emits the appropriate mutator implementation depending on read-only status
        /// and whether change notifications are required.
        /// </summary>
        protected virtual void GenerateCodeForMutator(TypeBuilder typeBuilder,
                                                      MethodBuilder methodBuilder,
                                                      PropertyInfo propertyInfo)
        {
            var backingFieldProperty = BackingFieldProperties[propertyInfo];

            var propertyChangingNotificationRequired = typeof(INotifyPropertyChanging).IsAssignableFrom(BaseType);
            var propertyChangedNotificationRequired = typeof(INotifyPropertyChanged).IsAssignableFrom(BaseType);

            Debug.Assert(!backingFieldProperty.IsReadOnly || backingFieldProperty.MappedType == null);

            void changedTrackedMethodGenerator(TypeBuilder _typeBuilder, MethodBuilder _methodBuilder, PropertyInfo _propertyBuilder)
            {
                GenerateCodeForChangeTrackedMutator(_typeBuilder, _methodBuilder, _propertyBuilder, propertyChangingNotificationRequired, propertyChangedNotificationRequired);
            }

            var mutatorGenerator = backingFieldProperty.IsReadOnly
                                       ? (TrackReadonlyPropertyInitialized
                                              ? (Action<TypeBuilder, MethodBuilder, PropertyInfo>)GenerateCodeForTrackedMutator
                                              : GenerateCodeForNormalMutator)
                                       : (propertyChangingNotificationRequired || propertyChangedNotificationRequired)
                                             ? changedTrackedMethodGenerator
                                             : GenerateCodeForNormalMutator;

            mutatorGenerator(typeBuilder, methodBuilder, propertyInfo);
        }

        /// <summary>
        /// Emits a straightforward setter that assigns the incoming value to the backing field.
        /// </summary>
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

        /// <summary>
        /// Emits a tracked setter which records initialization for read-only properties when enabled.
        /// </summary>
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

        /// <summary>
        /// Emits a setter that supports OnPropertyChanging and OnPropertyChanged semantics
        /// by invoking the corresponding methods on the base type when applicable.
        /// </summary>
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
                _ = gen.DeclareLocal(propertyInfo.PropertyType);
            }

            if (propertyChangingNotificationRequired)
            {
                _ = gen.DeclareLocal(typeof(bool));
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

        /// <summary>
        /// Adds methods declared on the interface to the emitted type. Non-special-name methods are implemented
        /// with a default body that throws <see cref="NotImplementedException"/> unless overridden.
        /// </summary>
        /// <param name="typeBuilder">The active <see cref="TypeBuilder"/>.</param>
        /// <returns>The modified <see cref="TypeBuilder"/>.</returns>
        protected virtual TypeBuilder AddMethods(TypeBuilder typeBuilder)
        {
            return typeof(T).GetAllNonExcludedMethods(InterfacesWithSpecialBehaviours)
                             .Where(MethodFilter)
                             .Where(_ => !_.IsSpecialName)
                             .Aggregate(typeBuilder,
                                        AddMethod);
        }

        /// <summary>
        /// Filter invoked for each method discovered on the interface. Subclasses may override
        /// to exclude or transform methods prior to emission.
        /// </summary>
        /// <param name="methodInfo">Method metadata being considered.</param>
        /// <returns>True to emit the method; false to skip it.</returns>
        protected virtual bool MethodFilter(MethodInfo methodInfo) => true;

        /// <summary>
        /// Adds a single method implementation to the emitted type. Generic method parameters
        /// are created on the emitted method and constraints are applied to keep parity with the
        /// interface declaration. The default body raises <see cref="NotImplementedException"/>.
        /// </summary>
        /// <param name="typeBuilder">The active <see cref="TypeBuilder"/>.</param>
        /// <param name="methodInfo">Metadata describing the method to implement.</param>
        /// <returns>The modified <see cref="TypeBuilder"/>.</returns>
        protected virtual TypeBuilder AddMethod(TypeBuilder typeBuilder,
                                                MethodInfo methodInfo)
        {
            var methodBuilder = typeBuilder.DefineMethod(methodInfo.Name,
                                                         MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.NewSlot | MethodAttributes.SpecialName | MethodAttributes.HideBySig);

            if (methodInfo.IsGenericMethod)
            {
                var genericArguments = methodInfo.GetGenericArguments();

                var genericTypeParameterBuilders = methodBuilder.DefineGenericParameters(genericArguments.Select(_ => _.Name).ToArray());

                foreach (var typeParameter in genericTypeParameterBuilders)
                {
                    var genericArgument = genericArguments.Single(_ => _.Name == typeParameter.Name);
                    typeParameter.SetGenericParameterAttributes(genericArgument.GenericParameterAttributes);

                    var parameterConstraints = genericArgument.GetGenericParameterConstraints();

                    var baseTypeConstraint = parameterConstraints.FirstOrDefault(_ => _.IsClass);
                    var interfaceConstraints = parameterConstraints.Where(_ => _.IsInterface).ToArray();

                    if (baseTypeConstraint != null)
                    {
                        typeParameter.SetBaseTypeConstraint(baseTypeConstraint);
                    }

                    typeParameter.SetInterfaceConstraints(interfaceConstraints);
                }
            }

            methodBuilder.SetReturnType(methodInfo.ReturnType);

            methodBuilder.SetParameters(methodInfo.GetParameters().Select(p => p.ParameterType).ToArray());

            foreach (var param in methodInfo.GetParameters()
                                            .OrderBy(_ => _.Position))
            {
                var _p = methodBuilder.DefineParameter(param.Position + 1,
                                                       param.Attributes,
                                                       param.Name);

                const BindingFlags parameterBindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

                if (param.IsOut)
                {
                    _p.SetCustomAttribute(new CustomAttributeBuilder(typeof(OutAttribute).GetConstructor(parameterBindingFlags, null, Type.EmptyTypes, null), Array.Empty<object>()));
                }
                else if (param.IsDefined(typeof(ParamArrayAttribute), false))
                {
                    _p.SetCustomAttribute(new CustomAttributeBuilder(typeof(ParamArrayAttribute).GetConstructor(parameterBindingFlags, null, Type.EmptyTypes, null), Array.Empty<object>()));
                }
            }

            GenerateMethodBody(methodBuilder);

            foreach (var op in MethodOperations)
            {
                methodBuilder = op(methodBuilder, methodInfo);
            }

            return typeBuilder;
        }

        /// <summary>
        /// Default method body generator that throws <see cref="NotImplementedException"/>.
        /// Subclasses can override to provide custom method bodies.
        /// </summary>
        /// <param name="methodBuilder">The method builder for the method body to generate.</param>
        protected virtual void GenerateMethodBody(MethodBuilder methodBuilder)
        {
            var gen = methodBuilder.GetILGenerator();

            // Writing body
            gen.Emit(OpCodes.Nop);
            gen.Emit(OpCodes.Newobj,
                     typeof(NotImplementedException).GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, Type.EmptyTypes, null));
            gen.Emit(OpCodes.Throw);
        }

        /// <summary>
        /// Field used by the emitted type to store the set of initialized read-only properties.
        /// This is populated by <see cref="AddInitializePropertyTrackingSupport"/> when enabled.
        /// </summary>
        protected FieldBuilder InitializePropertyField { get; private set; }

        /// <summary>
        /// Method builder reference for the emitted InitializePropertyValue method.
        /// </summary>
        protected MethodBuilder InitializePropertyValueMethod { get; private set; }

        /// <summary>
        /// Method builder reference for the emitted IsPropertyValueInitialized method.
        /// </summary>
        protected MethodBuilder IsPropertyValueInitializedMethod { get; private set; }

        /// <summary>
        /// Adds support to the emitted type to track initialization of read-only properties.
        /// This will create a backing HashSet and helper methods used by tracked accessors/mutators.
        /// </summary>
        /// <param name="typeBuilder">The active <see cref="TypeBuilder"/>.</param>
        /// <returns>The modified <see cref="TypeBuilder"/>.</returns>
        protected TypeBuilder AddInitializePropertyTrackingSupport(TypeBuilder typeBuilder)
        {
            Debug.Assert(TrackReadonlyPropertyInitialized);

            InitializePropertyField = BuildField_InitializedProperties(typeBuilder);
            InitializePropertyValueMethod = BuildMethodInitializePropertyValue(typeBuilder, InitializePropertyField);
            IsPropertyValueInitializedMethod = BuildMethodIsPropertyValueInitialized(typeBuilder, InitializePropertyField);

            return typeBuilder;
        }

        /// <summary>
        /// Builds the helper method that marks a property as initialized at runtime.
        /// </summary>
        private static MethodBuilder BuildMethodInitializePropertyValue(TypeBuilder type,
                                                                        FieldInfo initializedPropertiesField)
        {
            var method = type.DefineMethod("InitializePropertyValue",
                                           MethodAttributes.Family | MethodAttributes.HideBySig,
                                           typeof(void),
                                           new[] { typeof(string) });

            _ = method.DefineParameter(1,
                                   ParameterAttributes.None,
                                   "propertyName");

            var gen = method.GetILGenerator();

            gen.Emit(OpCodes.Nop);
            gen.Emit(OpCodes.Ldarg_0);
            gen.Emit(OpCodes.Ldfld,
                     initializedPropertiesField);
            gen.Emit(OpCodes.Ldarg_1);
            gen.Emit(OpCodes.Callvirt,
                     typeof(HashSet<string>).GetMethod("Add", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, new[] { typeof(string) }, null));
            gen.Emit(OpCodes.Pop);
            gen.Emit(OpCodes.Ret);
            return method;
        }

        /// <summary>
        /// Builds the helper method that checks whether a property has been marked initialized.
        /// </summary>
        private static MethodBuilder BuildMethodIsPropertyValueInitialized(TypeBuilder type,
                                                                           FieldInfo initializedPropertiesField)
        {
            var method = type.DefineMethod("IsPropertyValueInitialized",
                                           MethodAttributes.Family | MethodAttributes.HideBySig,
                                           typeof(bool),
                                           new[] { typeof(string) });
            _ = method.DefineParameter(1, ParameterAttributes.None, "propertyName");
            var gen = method.GetILGenerator();
            _ = gen.DeclareLocal(typeof(bool));

            var label16 = gen.DefineLabel();

            gen.Emit(OpCodes.Nop);
            gen.Emit(OpCodes.Ldarg_0);
            gen.Emit(OpCodes.Ldfld,
                     initializedPropertiesField);
            gen.Emit(OpCodes.Ldarg_1);
            gen.Emit(OpCodes.Callvirt,
                     typeof(HashSet<string>)
                        .GetMethod("Contains", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, new[] { typeof(string) }, null));
            gen.Emit(OpCodes.Stloc_0);
            gen.Emit(OpCodes.Br_S,
                     label16);
            gen.MarkLabel(label16);
            gen.Emit(OpCodes.Ldloc_0);
            gen.Emit(OpCodes.Ret);

            return method;
        }

        /// <summary>
        /// Builds the private field used to store initialized property names.
        /// </summary>
        private static FieldBuilder BuildField_InitializedProperties(TypeBuilder type) => type.DefineField("_initializedProperties", typeof(HashSet<string>), FieldAttributes.Private);

        /// <summary>
        /// Caches and returns a factory delegate that constructs instances of the emitted type.
        /// </summary>
        protected virtual Func<T> InstanceFactory => _instanceFactory ??= BuildInstanceFactory();

        /// <summary>
        /// Creates a new instance of <typeparamref name="T"/>. When <paramref name="source"/> is supplied the method maps dynamic values into the created instance using <see cref="FastMapper{T}"/>.
        /// </summary>
        /// <param name="source">Optional dynamic source object to map values from.</param>
        /// <returns>A new instance of the generated or concrete type.</returns>
        public virtual T CreateInstance(dynamic source = null)
        {
            var result = InstanceFactory();

            if (source != null)
            {
                FastMapper<T>.MapDynamicToStatic(source, result);
            }

            return result;
        }

        /// <summary>
        /// Builds a simple compiled lambda factory that invokes the parameterless constructor of the generated type.
        /// </summary>
        /// <returns>A delegate that constructs instances of <typeparamref name="T"/>.</returns>
        protected virtual Func<T> BuildInstanceFactory()
        {
            var ctor = Type.GetConstructor(Type.EmptyTypes);
            Debug.Assert(ctor != null);

            var lambda = Expression.Lambda(Expression.New(ctor));

            return (Func<T>)lambda.Compile();
        }

        /// <summary>
        /// Lazily returns the runtime <see cref="Type"/> generated for <typeparamref name="T"/>. Accessing this will trigger type emission on first use.
        /// </summary>
        public virtual Type Type => _type ??= BuildType();

        /// <summary>
        /// Builds and returns the concrete emitted <see cref="Type"/> for the interface <typeparamref name="T"/>.
        /// If <typeparamref name="T"/> is already a class type the method returns <typeparamref name="T"/> unchanged.
        /// </summary>
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

        /// <summary>
        /// Obtains the module builder used for emission. Subclasses can override to influence module naming.
        /// </summary>
        /// <param name="assemblyBuilder">The assembly builder being used to emit types.</param>
        /// <returns>A <see cref="ModuleBuilder"/> used to create types.</returns>
        protected virtual ModuleBuilder GetModuleBuilder(AssemblyBuilder assemblyBuilder) => assemblyBuilder.DefineDynamicModule(AssemblyName);

        /// <summary>
        /// Returns a DLL filename for the dynamic assembly when persisted to disk.
        /// </summary>
        protected virtual string GetAssemblyDllName() => string.Format(CultureInfo.InvariantCulture, "{0}.dll", AssemblyName);

        /// <summary>
        /// Creates the <see cref="AssemblyBuilder"/> used to host emitted types. Override to change assembly access or attributes.
        /// </summary>
        /// <param name="assemblyName">The logical assembly name to define.</param>
        /// <returns>An <see cref="AssemblyBuilder"/> configured for runtime emission.</returns>
        protected virtual AssemblyBuilder GetAssemblyBuilder(AssemblyName assemblyName) => AssemblyBuilder.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);

        /// <summary>
        /// Persist the generated assembly to disk. Default implementation is a no-op because not all runtimes support saving.
        /// Subclasses can override to implement actual persistence when supported.
        /// </summary>
        /// <param name="assemblyBuilder">The assembly builder that was used to create the type.</param>
        [ExcludeFromCodeCoverage]
        protected virtual void PersistAssembly(AssemblyBuilder assemblyBuilder)
        {
            // Saving dynamic assemblies to disk is not supported on all runtimes; skip when unavailable.
            // If SaveAssemblyToDisk is true and the runtime supports saving, this method can be overridden
            // in a derived type to provide the behavior.
            return;
        }

        /// <summary>
        /// Constructs the <see cref="TypeBuilder"/> for the emitted type and applies all configured behaviours, interfaces and class operations.
        /// </summary>
        /// <param name="moduleBuilder">The module builder used to define the type.</param>
        /// <returns>The constructed <see cref="TypeBuilder"/> ready to call <c>CreateType()</c> on.</returns>
        protected virtual TypeBuilder BuildTypeBuilder(ModuleBuilder moduleBuilder)
        {
            var implementedInterfaces = ImplementedInterfaces.ToArray();

            var typeBuilder = moduleBuilder.DefineType(ClassName,
                                                       TypeAttributes.Class | TypeAttributes.Public,
                                                       BaseType,
                                                       implementedInterfaces);

            var applicableInterfaces = FacetInterfaces.Union(new[] { typeof(T) });

            var behaviourOperations = from behaviour in SpecialBehaviours
                                      from _interface in applicableInterfaces
                                      where behaviour.Key.IsAssignableFrom(_interface)
                                      from operation in behaviour.Value.Operations
                                      select operation;

            typeBuilder = behaviourOperations.Aggregate(typeBuilder, (_typeBuilder, _function) => _function(_typeBuilder));

            return ClassOperations.Aggregate(typeBuilder, (_typeBuilder, _function) => _function(_typeBuilder));
        }
    }

    /// <summary>
    /// Mapping information used internally by the type creator to associate a declared property with a backing field on the emitted type.
    /// </summary>
    /// <remarks>
    /// Instances of this type track whether the property is read-only from the interface's perspective, the concrete backing field type used for storage, the optional mapped type for get-only mapped properties, and the runtime FieldBuilder that represents the field once the type is being emitted.
    /// </remarks>
    /// <example>
    /// <code>
    /// // The type creator populates a dictionary keyed by PropertyInfo mapping to PropertyBackingFieldMap
    /// var map = creator.BackingFieldProperties;
    /// foreach (var kv in map)
    /// {
    ///     Console.WriteLine(kv.Key.Name + " -> " + kv.Value.BackingFieldType.Name);
    /// }
    /// </code>
    /// </example>
    public class PropertyBackingFieldMap
    {
        public bool IsReadOnly { get; set; }
        public Type BackingFieldType { get; set; }
        public PropertyInfo PropertyInfo { get; set; }
        public Type MappedType { get; set; }
        public FieldBuilder BackingField { get; set; }
    }
}
