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
      throw new NotImplementedException();
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
