using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ivony.Html
{
  public class HtmlRange : IHtmlCollection
  {
    public void AddNode( IHtmlNode node )
    {
      throw new InvalidOperationException();
    }


    private IHtmlNode beginNode;
    private IHtmlNode EndNode;
    private bool inclusiveBegin;
    private bool inclusiveEnd;


    public HtmlRange( IHtmlNode node1, IHtmlNode node2, bool inclusiveNode1, bool inclusiveNode2 )
    {
      if ( node1.Container == null || !node1.Container.Equals( node2.Container ) )
        throw new InvalidOperationException();


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
      get { throw new NotImplementedException(); }
    }
  }
}
