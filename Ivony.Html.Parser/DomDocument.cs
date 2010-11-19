using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ivony.Html.Parser
{
  public class DomDocument : DomContainer, IHtmlDocument
  {

    public DomDocument()  { }


    protected override string ObjectName
    {
      get { return "Document"; }
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
