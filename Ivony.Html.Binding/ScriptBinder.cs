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
    /// <returns>永远返回 false，表示其他绑定器可以继续执行</returns>
    public void BindElement( HtmlBindingContext context, IHtmlElement element )
    {
      if ( !element.Name.EqualsIgnoreCase( "script" ) )
        return;


      var script = element.InnerHtml();

      script = scriptBindingExpression.Replace( script, match =>
        {
          var expression = BindingExpression.ParseExpression( context, match.Groups["expression"].Value );
          if ( expression == null )
            return match.Value;

          object dataObject = GetValue( context, expression );
          var valueExpression = serializer.Serialize( dataObject );

          return match.Groups["declare"].Value + valueExpression + ";";
        } );


      element.InnerHtml( script );
    }

    private static object GetValue( HtmlBindingContext context, BindingExpression expression )
    {
      object dataObject;
      if ( !context.TryGetDataObject( expression, out dataObject ) )
        dataObject = context.GetValue( expression );

      return dataObject;
    }
  }
}
