// Decompiled with JetBrains decompiler
// Type: BrightSword.Feber.Core.FunctionBuilder`3
// Assembly: BrightSword.Feber, Version=1.0.16.0, Culture=neutral, PublicKeyToken=null
// MVID: 1BF3E9E6-CA53-4387-8A58-C9EE67A8EFB2
// Assembly location: C:\Users\johnaz\Downloads\brightsword.feber.1.0.16\lib\net40\BrightSword.Feber.dll

using System;
using System.Linq;
using System.Linq.Expressions;

#nullable disable
namespace BrightSword.Feber.Core;

public abstract class FunctionBuilder<TProto, TInstance, TResult> : 
  UnaryFunctionBuilderBase<TProto, TInstance, TResult>
{
  private Func<TInstance, TResult> _function;

  public virtual Func<TInstance, TResult> Function
  {
    get => this._function ?? (this._function = this.BuildFunction());
  }

  private Func<TInstance, TResult> BuildFunction()
  {
    return Expression.Lambda<Func<TInstance, TResult>>(this.OperationExpressions.Aggregate<Expression, Expression>((Expression) Expression.Constant((object) this.Seed), this.Conjunction), this.InstanceParameterExpression).Compile();
  }
}
