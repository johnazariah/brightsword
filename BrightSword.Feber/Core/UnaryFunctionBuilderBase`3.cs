// Decompiled with JetBrains decompiler
// Type: BrightSword.Feber.Core.UnaryFunctionBuilderBase`3
// Assembly: BrightSword.Feber, Version=1.0.16.0, Culture=neutral, PublicKeyToken=null
// MVID: 1BF3E9E6-CA53-4387-8A58-C9EE67A8EFB2
// Assembly location: C:\Users\johnaz\Downloads\brightsword.feber.1.0.16\lib\net40\BrightSword.Feber.dll

using System;
using System.Linq.Expressions;

#nullable disable
namespace BrightSword.Feber.Core;

public abstract class UnaryFunctionBuilderBase<TProto, TInstance, TResult> : 
  UnaryOperationBuilderBase<TProto, TInstance>
{
  protected abstract TResult Seed { get; }

  protected abstract Func<Expression, Expression, Expression> Conjunction { get; }
}
