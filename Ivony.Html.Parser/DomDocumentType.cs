using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ivony.Html.Parser
{
 
  public class DomDocumentType : DomNode, IHtmlSpecial
  {

    private const string DocumentTypePattern = @"\<!DOCTYPE .+?\>";


    private string _declares;

    internal DomDocumentType( string declares )
    {
      _declares = declares;
    }

    protected override string ObjectName
    {
      get { return "DocumentType Declaration"; }
    }
  }
}
