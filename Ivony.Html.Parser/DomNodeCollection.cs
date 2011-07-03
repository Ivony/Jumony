using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using Ivony.Fluent;

namespace Ivony.Html.Parser
{

  /// <summary>
  /// 实现一个DomNode的容器
  /// </summary>
  public class DomNodeCollection : SynchronizedCollection<DomNode>
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

      if ( item.Container != null )
        throw new InvalidOperationException();

      base.InsertItem( index, item );

      item.Container = Container;

    }


    public IEnumerable<IHtmlNode> HtmlNodes
    {
      get { return this.Cast<IHtmlNode>().AsReadOnly(); }
    }

  }
}
