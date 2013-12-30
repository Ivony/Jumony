using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;
using System.Web.UI;
using Ivony.Fluent;
using Ivony.Html.ExpandedAPI;

namespace Ivony.Html.Web.Binding
{

  /// <summary>
  /// 默认的元素绑定器，处理 &lt;view&gt; 或者 &lt;binding&gt; 元素，以及属性绑定表达式和绑定属性处理。
  /// </summary>
  public class DefaultElementBinder : IHtmlElementBinder
  {


    private const string styleAttributePrefix = "binding-style-";
    private const string classAttributeName = "binding-class";


    /// <summary>
    /// 对元素进行数据绑定
    /// </summary>
    /// <param name="element">需要绑定数据的元素</param>
    /// <param name="context">绑定上下文</param>
    /// <returns>是否进行了绑定</returns>
    public bool BindElement( HtmlBindingContext context, IHtmlElement element )
    {

      if ( element.Attribute( "binding-visible" ) != null )
      {
        var visible = element.Attribute( "binding-visible" ).Value();
        if ( visible.EqualsIgnoreCase( "false" ) || visible.EqualsIgnoreCase( "hidden" ) || visible.EqualsIgnoreCase( "invisible" ) )
          element.Remove();
        return true;
      }




      //处理样式类
      {
        var classAttribute = element.Attribute( classAttributeName );
        if ( classAttribute != null )
        {
          if ( !string.IsNullOrWhiteSpace( classAttribute.AttributeValue ) )
          {

            var classes = Regulars.whiteSpaceSeparatorRegex.Split( classAttribute.AttributeValue ).Where( c => c != "" ).ToArray();
            if ( classes.Any() )
              element.Class( classes );
          }

          element.RemoveAttribute( classAttributeName );
        }
      }


      //处理CSS样式
      var styleAttributes = element.Attributes().Where( a => a.Name.StartsWith( styleAttributePrefix ) ).ToArray();
      if ( styleAttributes.Any() )
        BindElementStyles( element, styleAttributes );



      if ( element.Name.EqualsIgnoreCase( "Binding" ) )
      {

        var expression = new ElementExpression( element );
        object dataObject = BindingExpressionBinder.GetDataObject( context, expression.Arguments );

        if ( dataObject == null )
        {
          element.Remove();
          return true;
        }


        var format = element.Attribute( "format" ).Value();


        //绑定到客户端脚本对象
        var variableName = element.Attribute( "var" ).Value() ?? element.Attribute( "variable" ).Value();
        if ( variableName != null )
        {

          if ( format != null )
            dataObject = string.Format( format, dataObject );


          var hostName = element.Attribute( "host" ).Value();

          var script = (string) null;

          if ( hostName == null )
            script = string.Format( "(function(){{ this['{0}'] = {1} }})();", variableName, ToJson( dataObject ) );

          else
            script = string.Format( "(function(){{ this['{0}'] = {1} }})();", variableName, ToJson( dataObject ) );


          element.ReplaceWith( string.Format( "<script type=\"text/javascript\">{0}</script>", script ) );
          return true;
        }

        else
        {

          //绑定为 HTML 文本
          var bindValue = string.Format( format ?? "{0}", dataObject );

          element.ReplaceWith( bindValue );
          return true;
        }

      }

      else
        return false;

    }

    /// <summary>
    /// 绑定元素样式
    /// </summary>
    /// <param name="element">要处理的元素</param>
    /// <param name="styleAttributes">样式属性值</param>
    private static void BindElementStyles( IHtmlElement element, IHtmlAttribute[] styleAttributes )
    {
      foreach ( var attribute in styleAttributes )
      {

        var value = attribute.AttributeValue;
        var name = attribute.Name.Substring( styleAttributePrefix.Length );

        if ( string.IsNullOrEmpty( value ) )
          continue;

        else
          element.Style( name, value );


        attribute.Remove();
      }
    }


    private string ToJson( object dataObject )
    {
      var serializer = new JavaScriptSerializer();
      return serializer.Serialize( dataObject );
    }



    /// <summary>
    /// 对元素属性进行绑定操作
    /// </summary>
    /// <param name="attribute">要绑定的元素属性</param>
    /// <param name="context">绑定上下文</param>
    /// <returns>是否成功绑定</returns>
    public bool BindAttribute( HtmlBindingContext context, AttributeExpression expression )
    {

      if ( expression.Attribute.Name == "datacontext" )
        return false;

      var value = BindingExpressionBinder.GetValue( context, expression.Arguments );

      if ( value == null )
        expression.Attribute.Remove();
      
      else
        expression.Attribute.SetValue( value );

      
      return true;
    }

  }
}
