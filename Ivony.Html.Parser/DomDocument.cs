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


    public IHtmlContainer Container//UNDONE
    {
      get { return null; }
    }

    public void Remove()//UNDONE
    {
      throw new NotSupportedException();
    }

    protected override string ObjectName
    {
      get { return "Document"; }
    }
  }
}
