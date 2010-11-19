using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using Ivony.Fluent;


namespace Ivony.Html.Parser
{
  public abstract class DomContainer : DomNode, IHtmlContainer
  {

    protected DomContainer( DomContainer parent )
      : base( parent )
    {
      nodes = new SynchronizedCollection<DomNode>( SyncRoot );
    }


    private readonly IList<DomNode> nodes;


    internal void InsertNode( int index, DomNode node )
    {

      lock ( node.SyncRoot )
      {

        if ( node.Container != null )
          throw new InvalidOperationException();


        nodes.Insert( index, node );
        node.Container = this;

      }
    }

    internal void AddNode( DomNode domNode )
    {
      lock ( domNode.SyncRoot )
      {

        if ( domNode.Container != null )
          throw new InvalidOperationException();

        lock ( SyncRoot )
        {
          nodes.Add( domNode );
          domNode.Container = this;
        }
      }
    }


    internal void RemoveNode( DomNode node )
    {
      nodes.Remove( node );
    }


    #region IHtmlContainer 成员

    public IEnumerable<IHtmlNode> Nodes()
    {
      return nodes.Cast<IHtmlNode>();
    }

    #endregion

  }
}
