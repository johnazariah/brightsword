// Decompiled with JetBrains decompiler
// Type: BrightSword.Feber.Core.OperationBuilderBase`1
// Assembly: BrightSword.Feber, Version=1.0.16.0, Culture=neutral, PublicKeyToken=null
// MVID: 1BF3E9E6-CA53-4387-8A58-C9EE67A8EFB2
// Assembly location: C:\Users\johnaz\Downloads\brightsword.feber.1.0.16\lib\net40\BrightSword.Feber.dll

using BrightSword.SwissKnife;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

#nullable disable
namespace BrightSword.Feber.Core;

public abstract class OperationBuilderBase<TProto>
{
  protected virtual Func<PropertyInfo, bool> PropertyFilter
  {
    get => (Func<PropertyInfo, bool>) (_ => true);
  }

  public virtual IEnumerable<PropertyInfo> FilteredProperties
  {
    get
    {
      return typeof (TProto).GetAllProperties(BindingFlags.Instance | BindingFlags.Public).Where<PropertyInfo>((Func<PropertyInfo, bool>) (_propertyInfo => this.PropertyFilter(_propertyInfo)));
    }
  }

  protected virtual IEnumerable<Expression> OperationExpressions
  {
    get
    {
      return this.FilteredProperties.Select<PropertyInfo, Expression>(new Func<PropertyInfo, Expression>(this.BuildPropertyExpression));
    }
  }

  protected abstract Expression BuildPropertyExpression(PropertyInfo propertyInfo);
}
