using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Ivony.Html.Parser
{
  public class DomSpecial : DomNode, IHtmlSpecial, IHtmlComment
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

    #region IHtmlComment 成员

    string IHtmlComment.Comment
    {
      get { return specialTagRegex.Match( raw ).Groups["specialText"].Value; }
    }

    #endregion

  }




}
