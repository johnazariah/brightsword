// Decompiled with JetBrains decompiler
// Type: BrightSword.Feber.Core.ActionBuilder`3
// Assembly: BrightSword.Feber, Version=1.0.16.0, Culture=neutral, PublicKeyToken=null
// MVID: 1BF3E9E6-CA53-4387-8A58-C9EE67A8EFB2
// Assembly location: C:\Users\johnaz\Downloads\brightsword.feber.1.0.16\lib\net40\BrightSword.Feber.dll

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
