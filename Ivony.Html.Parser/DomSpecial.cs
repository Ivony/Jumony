using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ivony.Html.Parser
{
  public class DomSpecial : DomNode, IHtmlSpecial
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
  }




}
