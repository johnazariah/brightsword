// Decompiled with JetBrains decompiler
// Type: BrightSword.Feber.Core.FunctionBuilder`4
// Assembly: BrightSword.Feber, Version=1.0.16.0, Culture=neutral, PublicKeyToken=null
// MVID: 1BF3E9E6-CA53-4387-8A58-C9EE67A8EFB2
// Assembly location: C:\Users\johnaz\Downloads\brightsword.feber.1.0.16\lib\net40\BrightSword.Feber.dll

using System;
using System.Linq;
using System.Linq.Expressions;

#nullable disable
namespace BrightSword.Feber.Core;

public abstract class FunctionBuilder<TProto, TLeftInstance, TRightInstance, TResult> : 
  BinaryFunctionBuilderBase<TProto, TLeftInstance, TRightInstance, TResult>
{
  private Func<TLeftInstance, TRightInstance, TResult> _function;

  public virtual Func<TLeftInstance, TRightInstance, TResult> Function
  {
    get => this._function ?? (this._function = this.BuildFunction());
  }

  private Func<TLeftInstance, TRightInstance, TResult> BuildFunction()
  {
    return Expression.Lambda<Func<TLeftInstance, TRightInstance, TResult>>(this.OperationExpressions.Aggregate<Expression, Expression>((Expression) Expression.Constant((object) this.Seed), this.Conjunction), this.LeftInstanceParameterExpression, this.RightInstanceParameterExpression).Compile();
  }
}
