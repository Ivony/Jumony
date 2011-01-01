using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Collections;
using Ivony.Fluent;
using System.Diagnostics;
using System.Globalization;

namespace Ivony.Html
{

  /// <summary>
  /// 代表一个CSS选择器
  /// </summary>
  public sealed class CssSelector
  {

    public static ICssSelector Create( params string[] expressions )
    {
      var selectors = expressions.Select( e => CssCasecadingSelector.Create( e ) ).ToArray();

      return new CssMultipleSelector( selectors );
    }
    public static ICssSelector Create( IHtmlContainer scope, params string[] expressions )
    {
      var selectors = expressions.Select( e => CssCasecadingSelector.Create( e, scope ) ).ToArray();

      return new CssMultipleSelector( selectors );
    }



  }


  public static class SelectorExtensions
  {

    public static IEnumerable<IHtmlElement> Filter( this ICssSelector selector, IEnumerable<IHtmlElement> source )
    {
      return source.Where( e => selector.IsEligible( e ) );
    }


  }

}
