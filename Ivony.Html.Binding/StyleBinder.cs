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

namespace Ivony.Html.Binding
{

  /// <summary>
  /// 样式绑定器，负责 CSS 样式绑定处理。
  /// </summary>
  public class StyleBinder : IHtmlBinder
  {


    private const string styleAttributePrefix = "binding-style-";
    private const string classAttributeName = "binding-class-";
    private const string classAttributePrefix = "binding-class-";


    /// <summary>
    /// 对元素进行数据绑定
    /// </summary>
    /// <param name="element">需要绑定数据的元素</param>
    /// <param name="context">绑定上下文</param>
    public void BindElement( HtmlBindingContext context, IHtmlElement element )
    {

      if ( element.Attribute( "binding-visible" ) != null )
      {
        var visible = element.Attribute( "binding-visible" ).Value();
        if ( visible.EqualsIgnoreCase( "false" ) || visible.EqualsIgnoreCase( "hidden" ) || visible.EqualsIgnoreCase( "invisible" ) )
        {
          element.Remove();

          context.CancelChildsBinding = context.BindCompleted = true;
        }
      }





      {
        //处理样式类
        var classAttribute = element.Attribute( classAttributePrefix );
        if ( classAttribute != null )
        {
          if ( !string.IsNullOrWhiteSpace( classAttribute.AttributeValue ) )
          {

            var classes = Regulars.whiteSpaceSeparatorRegex.Split( classAttribute.AttributeValue ).Where( c => c != "" ).ToArray();
            if ( classes.Any() )
              element.Class( classes );
          }

          element.RemoveAttribute( classAttributePrefix );
        }

      }

      {
        var classAttributes = element.Attributes().Where( a => a.Name.StartsWith( classAttributePrefix ) ).ToArray();
        BindElementClasses( element, classAttributes );
      }

      //处理CSS样式
      var styleAttributes = element.Attributes().Where( a => a.Name.StartsWith( styleAttributePrefix ) ).ToArray();
      BindElementStyles( element, styleAttributes );
    }

    
    /// <summary>
    /// 绑定元素样式
    /// </summary>
    /// <param name="element">要处理的元素</param>
    /// <param name="styleAttributes">样式属性</param>
    private static void BindElementStyles( IHtmlElement element, IHtmlAttribute[] styleAttributes )
    {
      foreach ( var attribute in styleAttributes )
      {

        var value = attribute.AttributeValue;
        var name = attribute.Name.Substring( styleAttributePrefix.Length );

        attribute.Remove();

        if ( string.IsNullOrEmpty( value ) )
          continue;

        else
          element.Style( name, value );
      }
    }


    /// <summary>
    /// 绑定元素样式类
    /// </summary>
    /// <param name="element">要处理的元素</param>
    /// <param name="classAttributes">样式类属性</param>
    private static void BindElementClasses( IHtmlElement element, IHtmlAttribute[] classAttributes )
    {
      foreach ( var attribute in classAttributes )
      {

        var value = attribute.AttributeValue;
        var name = attribute.Name.Substring( classAttributePrefix.Length );

        attribute.Remove();


        if ( string.IsNullOrEmpty( value ) )
          continue;

        else if ( value.EqualsIgnoreCase( "false" ) )
          element.Class().Remove( name );

        else
          element.Class().Add( name );

      }
    }



    private string ToJson( object dataObject )
    {
      var serializer = new JavaScriptSerializer();
      return serializer.Serialize( dataObject );
    }

  }
}
