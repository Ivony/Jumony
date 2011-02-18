using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ivony.Fluent;

namespace Ivony.Html.Parser
{
  public class DomDocument : DomContainer, IHtmlDocument
  {

    public DomDocument( Uri url )
    {
      _url = url;
    }

    private Uri _url;

    public Uri DocumentUri
    {
      get { return _url; }
    }

    public override IHtmlDocument Document
    {
      get { return this; }
    }

    public string DocumentDeclaration
    {
      get { return null; }
    }

    public IHtmlNodeFactory GetNodeFactory()
    {
      return new DomFactory( this );
    }

  }
}
