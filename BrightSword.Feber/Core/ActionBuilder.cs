using System;
using System.Linq.Expressions;

namespace BrightSword.Feber.Core;

public abstract class ActionBuilder<TProto, TInstance> : UnaryOperationBuilderBase<TProto, TInstance>
{
    private Action<TInstance> _action;

    public virtual Action<TInstance> Action => _action ??= BuildAction();

    // Allow overrides in derived builders if they want to customise the generated delegate
    protected virtual Action<TInstance> BuildAction()
    {
        return Expression.Lambda<Action<TInstance>>(Expression.Block(OperationExpressions), InstanceParameterExpression).Compile();
    }
}

public abstract class ActionBuilder<TProto, TLeftInstance, TRightInstance> :
    BinaryOperationBuilderBase<TProto, TLeftInstance, TRightInstance>
{
    private Action<TLeftInstance, TRightInstance> _action;

    public virtual Action<TLeftInstance, TRightInstance> Action => _action ??= BuildAction();

    protected virtual Action<TLeftInstance, TRightInstance> BuildAction()
    {
        return Expression.Lambda<Action<TLeftInstance, TRightInstance>>(Expression.Block(OperationExpressions), LeftInstanceParameterExpression, RightInstanceParameterExpression).Compile();
    }
}
