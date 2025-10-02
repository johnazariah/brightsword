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
