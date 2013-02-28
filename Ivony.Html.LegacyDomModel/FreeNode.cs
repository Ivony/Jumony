using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ivony.Html
{
  internal class FreeNode : IFreeNode
  {
    internal FreeNode( IHtmlFragment fragment, IHtmlNodeFactory factory )
    {
      _fragment = fragment;
      _factory = factory;
      Node = _fragment.Nodes().First();
    }

    IHtmlFragment _fragment;
    IHtmlNodeFactory _factory;

    protected IHtmlNode Node
    {
      get;
      private set;
    }

    public IHtmlNode Into( IHtmlContainer container, int index )
    {
      return _fragment.Into( container, index ).First();
    }

    public IHtmlNodeFactory Factory
    {
      get { return _factory; }
    }

    public IHtmlContainer Container
    {
      get { return _fragment; }
    }

    public object RawObject
    {
      get { return Node.RawObject; }
    }

    public string RawHtml
    {
      get { return Node.RawHtml; }
    }

    public IHtmlDocument Document
    {
      get { return _fragment.Document; }
    }
  }
}
