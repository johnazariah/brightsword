using System.Linq.Expressions;

#nullable disable
namespace BrightSword.Feber.Core;

public abstract class ActionBuilder<TProto, TLeftInstance, TRightInstance> : 
  BinaryOperationBuilderBase<TProto, TLeftInstance, TRightInstance>
{
  private System.Action<TLeftInstance, TRightInstance> _action;

  public virtual System.Action<TLeftInstance, TRightInstance> Action
  {
    get => this._action ?? (this._action = this.BuildAction());
  }

  protected virtual System.Action<TLeftInstance, TRightInstance> BuildAction()
  {
    return Expression.Lambda<System.Action<TLeftInstance, TRightInstance>>((Expression) Expression.Block(this.OperationExpressions), this.LeftInstanceParameterExpression, this.RightInstanceParameterExpression).Compile();
  }
}
