using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace Ivony.Html.Parser
{

  /// <summary>
  /// 实现一个DomNode的容器
  /// </summary>
  public class DomNodeCollection : SynchronizedCollection<DomNode>, IEnumerable<IHtmlNode>
  {

    public IDomContainer Container
    {
      get;
      private set;
    }

    public DomNodeCollection( IDomContainer container )
      : base( container.SyncRoot )
    {
      Container = container;
    }

    protected override void InsertItem( int index, DomNode item )
    {

      lock ( item.SyncRoot )
      {

        if ( item.Container != null )
          throw new InvalidOperationException();

        base.InsertItem( index, item );

        item.Container = Container;
      }

    }



    IEnumerator<IHtmlNode> IEnumerable<IHtmlNode>.GetEnumerator()
    {
      return Items.Cast<IHtmlNode>().GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return GetEnumerator();
    }
  }
}
