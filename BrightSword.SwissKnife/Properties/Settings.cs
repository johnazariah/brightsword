using System.CodeDom.Compiler;
using System.Configuration;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace BrightSword.SwissKnife.Properties
{
    [GeneratedCode("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "11.0.0.0")]
    [CompilerGenerated]
    internal sealed class Settings : ApplicationSettingsBase
    {
        private static Settings defaultInstance = (Settings)SettingsBase.Synchronized((SettingsBase)new Settings());

        public static Settings Default
        {
            get
            {
                Settings defaultInstance = Settings.defaultInstance;
                return defaultInstance;
            }
        }

        [UserScopedSetting]
        [DebuggerNonUserCode]
        [DefaultSettingValue("23")]
        public uint Realm_UniqueID
        {
            get => (uint)this[nameof(Realm_UniqueID)];
            set => this[nameof(Realm_UniqueID)] = (object)value;
        }

        [DefaultSettingValue("97")]
        [UserScopedSetting]
        [DebuggerNonUserCode]
        public ushort Server_UniqueID
        {
            get => (ushort)this[nameof(Server_UniqueID)];
            set => this[nameof(Server_UniqueID)] = (object)value;
        }

        [DebuggerNonUserCode]
        [UserScopedSetting]
        [DefaultSettingValue("11")]
        public ushort Application_UniqueID
        {
            get => (ushort)this[nameof(Application_UniqueID)];
            set => this[nameof(Application_UniqueID)] = (object)value;
        }
    }
}
