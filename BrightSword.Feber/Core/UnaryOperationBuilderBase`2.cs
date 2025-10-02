// Decompiled with JetBrains decompiler
// Type: BrightSword.Feber.Core.UnaryOperationBuilderBase`2
// Assembly: BrightSword.Feber, Version=1.0.16.0, Culture=neutral, PublicKeyToken=null
// MVID: 1BF3E9E6-CA53-4387-8A58-C9EE67A8EFB2
// Assembly location: C:\Users\johnaz\Downloads\brightsword.feber.1.0.16\lib\net40\BrightSword.Feber.dll

using System.Linq.Expressions;
using System.Reflection;

#nullable disable
namespace BrightSword.Feber.Core;

public abstract class UnaryOperationBuilderBase<TProto, TInstance> : OperationBuilderBase<TProto>
{
  private static readonly ParameterExpression _parameterExpression = Expression.Parameter(typeof (TInstance), "_instance");

  protected virtual ParameterExpression InstanceParameterExpression
  {
    get => UnaryOperationBuilderBase<TProto, TInstance>._parameterExpression;
  }

  protected override Expression BuildPropertyExpression(PropertyInfo propertyInfo)
  {
    return this.PropertyExpression(propertyInfo, this.InstanceParameterExpression);
  }

  protected abstract Expression PropertyExpression(
    PropertyInfo property,
    ParameterExpression instanceParameterExpression);
}
