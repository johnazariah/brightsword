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
