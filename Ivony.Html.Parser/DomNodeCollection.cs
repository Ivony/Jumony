using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ivony.Html.Parser
{
  public class DomNodeCollection : SynchronizedCollection<DomNode>
  {

    public DomContainer Container
    {
      get;
      private set;
    }

    public DomNodeCollection( DomContainer container )
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
