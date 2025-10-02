using System.Linq.Expressions;

#nullable disable
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
