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
  public class ScriptBinder : IHtmlBinder
  {





    private static Regex scriptBindingExpression = new Regex( @"^(?<declare>\s*var\s+[a-zA-z_][a-zA-Z_0-9]+\s*=).*?//\s*(?<expression>{.*})\s*$", RegexOptions.Compiled | RegexOptions.Multiline | RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture );



    private JavaScriptSerializer serializer = new JavaScriptSerializer();

    public bool BindElement( HtmlBindingContext context, IHtmlElement element )
    {
      if ( !element.Name.EqualsIgnoreCase( "script" ) )
        return false;


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

      return false;

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
