// Decompiled with JetBrains decompiler
// Type: BrightSword.SwissKnife.Properties.Settings
// Assembly: BrightSword.SwissKnife, Version=1.0.9.0, Culture=neutral, PublicKeyToken=null
// MVID: CD8E9696-E577-443F-8EA9-5929CF204282
// Assembly location: C:\Users\johnaz\Downloads\brightsword.swissknife.1.0.9\lib\net40\BrightSword.SwissKnife.dll

using System.CodeDom.Compiler;
using System.Configuration;
using System.Diagnostics;
using System.Runtime.CompilerServices;

#nullable disable
namespace BrightSword.SwissKnife.Properties;

[GeneratedCode("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "11.0.0.0")]
[CompilerGenerated]
internal sealed class Settings : ApplicationSettingsBase
{
  private static Settings defaultInstance = (Settings) SettingsBase.Synchronized((SettingsBase) new Settings());

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
    get => (uint) this[nameof (Realm_UniqueID)];
    set => this[nameof (Realm_UniqueID)] = (object) value;
  }

  [DefaultSettingValue("97")]
  [UserScopedSetting]
  [DebuggerNonUserCode]
  public ushort Server_UniqueID
  {
    get => (ushort) this[nameof (Server_UniqueID)];
    set => this[nameof (Server_UniqueID)] = (object) value;
  }

  [DebuggerNonUserCode]
  [UserScopedSetting]
  [DefaultSettingValue("11")]
  public ushort Application_UniqueID
  {
    get => (ushort) this[nameof (Application_UniqueID)];
    set => this[nameof (Application_UniqueID)] = (object) value;
  }
}
