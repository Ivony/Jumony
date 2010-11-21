using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Ivony.Html.Parser
{
  public class DomSpecial : DomNode, IHtmlSpecial, IHtmlTextNode
  {

    private string raw;

    public DomSpecial( string rawHtml )
    {
      raw = rawHtml;
    }

    public override string RawHtml
    {
      get
      {
        CheckDisposed();

        return raw;
      }
    }

    public string EvaluateHtml()
    {
      CheckDisposed();

      return raw;
    }


    protected override string ObjectName
    {
      get { return "Special Node"; }
    }


    private static readonly Regex specialTagRegex = new Regex( Regulars.specialTagPattern, RegexOptions.Compiled );

    #region IHtmlTextNode 成员

    string IHtmlTextNode.HtmlText
    {
      get { return raw; }
    }

    #endregion

  }




}
