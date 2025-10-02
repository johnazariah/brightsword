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
