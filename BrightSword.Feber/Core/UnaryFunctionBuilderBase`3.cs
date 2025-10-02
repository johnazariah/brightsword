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
