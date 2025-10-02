// Decompiled with JetBrains decompiler
// Type: BrightSword.SwissKnife.ConcurrentDictionary`3
// Assembly: BrightSword.SwissKnife, Version=1.0.9.0, Culture=neutral, PublicKeyToken=null
// MVID: CD8E9696-E577-443F-8EA9-5929CF204282
// Assembly location: C:\Users\johnaz\Downloads\brightsword.swissknife.1.0.9\lib\net40\BrightSword.SwissKnife.dll

using System.Collections.Concurrent;

#nullable disable
namespace BrightSword.SwissKnife;

public class ConcurrentDictionary<TKey1, TKey2, TValue> : 
  ConcurrentDictionary<TKey1, ConcurrentDictionary<TKey2, TValue>>
{
  public TValue this[TKey1 key1, TKey2 key2]
  {
    get => this[key1][key2];
    set => this.GetOrAdd(key1, new ConcurrentDictionary<TKey2, TValue>()).GetOrAdd(key2, value);
  }
}
