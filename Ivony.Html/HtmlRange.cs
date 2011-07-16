using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


#if DEBUG

namespace Ivony.Html
{
  public class HtmlRange : IHtmlCollection
  {
    public void AddNode( IHtmlNode node )
    {
      throw new InvalidOperationException();
    }


    private IHtmlNode beginNode;
    private IHtmlNode endNode;
    private bool inclusiveBegin;
    private bool inclusiveEnd;
    private IHtmlContainer container;


    public HtmlRange( IHtmlNode node1, IHtmlNode node2, bool inclusiveNode1, bool inclusiveNode2 )
    {

      if ( node1 == null )
        throw new ArgumentNullException( "node1" );

      if ( node2 == null )
        throw new ArgumentNullException( "node2" );


      container = node1.Container;

      if ( container == null || !container.Equals( node2.Container ) || node1.Equals( node2 ) )
        throw new InvalidOperationException();

      if ( node1.NodesIndexOfSelf() <= node2.NodesIndexOfSelf() )
      {
        beginNode = node1;
        endNode = node2;
        inclusiveBegin = inclusiveNode1;
        inclusiveEnd = inclusiveNode2;
      }
      else
      {
        beginNode = node2;
        endNode = node1;
        inclusiveBegin = inclusiveNode2;
        inclusiveEnd = inclusiveNode1;
      }

    }



    public IEnumerable<IHtmlNode> Nodes()
    {
      throw new NotImplementedException();
    }

    public object SyncRoot
    {
      get { throw new NotImplementedException(); }
    }

    public object RawObject
    {
      get { return null; }
    }

    public string RawHtml
    {
      get { return null; }
    }

    public IHtmlDocument Document
    {
      get { return container.Document; }
    }
  }
}

#endif