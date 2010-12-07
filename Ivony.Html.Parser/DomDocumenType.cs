using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ivony.Html.Parser
{
  public class DomDocumenType : DomNode, IHtmlSpecial
  {

    private const string DocumentTypePattern = @"\<!DOCTYPE .+?\>";


    private string _declares;

    internal DomDocumenType( string declares )
    {
      _declares = declares;
    }

    protected override string ObjectName
    {
      get { return "DocumentType Declaration"; }
    }
  }
}
