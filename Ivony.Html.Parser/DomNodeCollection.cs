using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ivony.Html.Parser
{
  internal class DomNodeCollection : SynchronizedCollection<DomNode>
  {


    public IHtmlContainer Container
    {
      get;
      private set;
    }

    public DomNodeCollection( IHtmlContainer container )
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

  }
}
