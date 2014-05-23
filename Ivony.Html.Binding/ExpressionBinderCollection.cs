using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ivony.Html.Binding
{


  /// <summary>
  /// 定义表达式绑定器的容器
  /// </summary>
  public class ExpressionBinderCollection : SynchronizedKeyedCollection<string, IExpressionBinder>
  {


    /// <summary>
    /// 创建 ExpressionBinderCollection 对象
    /// </summary>
    public ExpressionBinderCollection() : base( new object(), StringComparer.OrdinalIgnoreCase ) { }

    /// <summary>
    /// 创建 ExpressionBinderCollection 对象
    /// </summary>
    /// <param name="binders">要添加的绑定器</param>
    public ExpressionBinderCollection( IEnumerable<IExpressionBinder> binders )
      : this()
    {
      foreach ( var item in binders )
        Add( item );
    }


    /// <summary>
    /// 重写此方法从表达式绑定器对象中提取键
    /// </summary>
    /// <param name="item">要提取键（即可以处理的绑定表达式的名称）的表达式绑定器</param>
    /// <returns>提取的键</returns>
    protected override string GetKeyForItem( IExpressionBinder item )
    {
      return item.ExpressionName;
    }
  }
}
