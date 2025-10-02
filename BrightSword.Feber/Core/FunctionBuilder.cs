using System;
using System.Linq;
using System.Linq.Expressions;

namespace BrightSword.Feber.Core;

public abstract class UnaryFunctionBuilderBase<TProto, TInstance, TResult> : 
  UnaryOperationBuilderBase<TProto, TInstance>
{
  protected abstract TResult Seed { get; }

  protected abstract Func<Expression, Expression, Expression> Conjunction { get; }
}
public abstract class BinaryFunctionBuilderBase<TProto, TLeftInstance, TRightInstance, TResult> : 
  BinaryOperationBuilderBase<TProto, TLeftInstance, TRightInstance>
{
  protected abstract TResult Seed { get; }

  protected abstract Func<Expression, Expression, Expression> Conjunction { get; }
}

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
    return Expression.Lambda<Func<TInstance, TResult>>(this.OperationExpressions.Aggregate<Expression, Expression>((Expression)Expression.Constant((object)this.Seed), this.Conjunction), this.InstanceParameterExpression).Compile();
  }
}

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
