using System.Linq.Expressions;

namespace BrightSword.Feber.Core;

public abstract class ActionBuilder<TProto, TInstance> : UnaryOperationBuilderBase<TProto, TInstance>
{
  private System.Action<TInstance> _action;

  public virtual System.Action<TInstance> Action
  {
    get => this._action ?? (this._action = this.BuildAction());
  }

  private System.Action<TInstance> BuildAction()
  {
    return Expression.Lambda<System.Action<TInstance>>((Expression) Expression.Block(this.OperationExpressions), this.InstanceParameterExpression).Compile();
  }
}

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
