using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ivony.Fluent;
using System.IO;
using System.Text.RegularExpressions;
using System.Web.Script.Serialization;

namespace Ivony.Html.Binding
{

  /// <summary>
  /// 脚本绑定器，用于解析脚本中的绑定表达式。
  /// </summary>
  public class ScriptBinder : IHtmlElementBinder
  {

    private static Regex scriptBindingExpression = new Regex( @"^(?<declare>\s*var\s+[a-zA-z_][a-zA-Z_0-9]+\s*=).*?//\s*(?<expression>{.*})\s*$", RegexOptions.Compiled | RegexOptions.Multiline | RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture );



    private JavaScriptSerializer serializer = new JavaScriptSerializer();


    /// <summary>
    /// 对 HTML 中的 script 元素进行绑定
    /// </summary>
    /// <param name="context">当前绑定上下文</param>
    /// <param name="element">当前绑定的元素（仅会对 script 元素起作用）</param>
    public void BindElement( HtmlBindingContext context, IHtmlElement element )
    {


      context.BindAttributes( element );


      if ( element.ElementTextMode() != TextMode.CData )
        throw new InvalidOperationException();


      var script = element.InnerHtml();

      script = scriptBindingExpression.Replace( script, match =>
        {
          var expression = BindingExpression.ParseExpression( match.Groups["expression"].Value );
          if ( expression == null )
            return match.Value;

          object dataObject = context.GetValue( expression );
          var valueExpression = serializer.Serialize( dataObject );

          return match.Groups["declare"].Value + valueExpression + ";";
        } );


      element.InnerHtml( script );
    }


    public string ElementName
    {
      get { return "script"; }
    }
  }
}
