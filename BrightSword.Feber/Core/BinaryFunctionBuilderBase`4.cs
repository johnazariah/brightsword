using System;
using System.Linq.Expressions;

#nullable disable
namespace BrightSword.Feber.Core;

public abstract class BinaryFunctionBuilderBase<TProto, TLeftInstance, TRightInstance, TResult> : 
  BinaryOperationBuilderBase<TProto, TLeftInstance, TRightInstance>
{
  protected abstract TResult Seed { get; }

  protected abstract Func<Expression, Expression, Expression> Conjunction { get; }
}
