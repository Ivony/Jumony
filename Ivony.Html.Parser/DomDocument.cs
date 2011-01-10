using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ivony.Fluent;

namespace Ivony.Html.Parser
{
  public class DomDocument : DomContainer, IHtmlDocument
  {

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


    public IHtmlContainer Container
    {
      get { return null; }
    }

    public string RawHtml
    {
      get { return null; }
    }

    public void Remove()
    {
      throw new NotSupportedException();
    }

    protected override string ObjectName
    {
      get { return "Document"; }
    }
  }
}
