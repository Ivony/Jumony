using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ivony.Html.Web.Binding
{
  internal class ExpressionBinderCollection : SynchronizedKeyedCollection<string, IExpressionBinder>
  {


    /// <summary>
    /// 创建 ExpressionBinderCollection 对象
    /// </summary>
    public ExpressionBinderCollection() : base( new object(), StringComparer.OrdinalIgnoreCase ) { }

    /// <summary>
    /// 创建 ExpressionBinderCollection 对象
    /// </summary>
    public ExpressionBinderCollection( IEnumerable<IExpressionBinder> binders )
      : this()
    {
      foreach ( var item in binders )
        Add( item );
    }


    protected override string GetKeyForItem( IExpressionBinder item )
    {
      return item.ExpressionName;
    }
  }
}
