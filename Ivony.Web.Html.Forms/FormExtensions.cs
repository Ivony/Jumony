using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ivony.Web.Html.Forms
{
  public static class FormExtensions
  {
    public static IHtmlForm AsForm( this IHtmlElement element )
    {
      return new HtmlForm( element );
    }
  }
}
