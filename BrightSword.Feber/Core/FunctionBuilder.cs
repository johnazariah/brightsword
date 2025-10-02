using System;
using System.Linq;
using System.Linq.Expressions;

namespace BrightSword.Feber.Core
{
    public abstract class UnaryFunctionBuilderBase<TProto, TInstance, TResult> : UnaryOperationBuilderBase<TProto, TInstance>
    {
        protected abstract TResult Seed { get; }

        protected abstract Func<Expression, Expression, Expression> Conjunction { get; }
    }

    public abstract class BinaryFunctionBuilderBase<TProto, TLeftInstance, TRightInstance, TResult> : BinaryOperationBuilderBase<TProto, TLeftInstance, TRightInstance>
    {
        protected abstract TResult Seed { get; }

        protected abstract Func<Expression, Expression, Expression> Conjunction { get; }
    }

    public abstract class FunctionBuilder<TProto, TInstance, TResult> : UnaryFunctionBuilderBase<TProto, TInstance, TResult>
    {
        private Func<TInstance, TResult> _function;

        public virtual Func<TInstance, TResult> Function => _function ??= BuildFunction();

        protected virtual Func<TInstance, TResult> BuildFunction()
        {
            var seedExpr = Expression.Constant(Seed);
            var body = OperationExpressions.Aggregate(seedExpr, Conjunction);
            return Expression.Lambda<Func<TInstance, TResult>>(body, InstanceParameterExpression).Compile();
        }
    }

    public abstract class FunctionBuilder<TProto, TLeftInstance, TRightInstance, TResult> : BinaryFunctionBuilderBase<TProto, TLeftInstance, TRightInstance, TResult>
    {
        private Func<TLeftInstance, TRightInstance, TResult> _function;

        public virtual Func<TLeftInstance, TRightInstance, TResult> Function => _function ??= BuildFunction();

        protected virtual Func<TLeftInstance, TRightInstance, TResult> BuildFunction()
        {
            var seedExpr = Expression.Constant(Seed);
            var body = OperationExpressions.Aggregate(seedExpr, Conjunction);
            return Expression.Lambda<Func<TLeftInstance, TRightInstance, TResult>>(body, LeftInstanceParameterExpression, RightInstanceParameterExpression).Compile();
        }
    }
}
