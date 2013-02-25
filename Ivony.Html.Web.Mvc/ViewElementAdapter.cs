using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Web.UI;
using System.Web.Mvc;
using System.Collections;
using System.Web.Script.Serialization;

namespace Ivony.Html.Web
{

  /// <summary>
  /// 用于渲染 view 标签的元素渲染代理
  /// </summary>
  public class ViewElementAdapter : HtmlElementAdapter
  {

    protected ViewContext ViewContext { get; private set; }
    protected JumonyUrlHelper Url { get; private set; }

    /// <summary>
    /// 创建 ViewElementAdapter 对象
    /// </summary>
    /// <param name="context"></param>
    public ViewElementAdapter( ViewContext context, JumonyUrlHelper urlHelper )
    {
      ViewContext = context;
      Url = urlHelper;
    }


    /// <summary>
    /// 渲染 view 标签
    /// </summary>
    /// <param name="element">view 标签元素</param>
    /// <param name="context">渲染上下文</param>
    protected override void Render( IHtmlElement element, HtmlRenderContext context )
    {

      if ( element.Attribute( "action" ) != null )
      {

        var url = Url.GetRouteUrl( element );

        var nextElement = element.NextElement();
        if ( nextElement == null )
          return;

        switch ( nextElement.Name )
        {
          case "a":
          case "link":
            nextElement.SetAttribute( "href", url );
            return;

          case "img":
          case "script":
            nextElement.SetAttribute( "src", url );
            return;

          case "form":
            nextElement.SetAttribute( "action", url );
            return;

          default:
            return;
        }
      }



      var key = element.Attribute( "key" ).Value() ?? element.Attribute( "name" ).Value();

      object dataObject;
      if ( key != null )
        ViewContext.ViewData.TryGetValue( key, out dataObject );
      else
        dataObject = GetDataObject( context.Data ) ?? ViewContext.ViewData.Model;


      if ( dataObject == null )
        return;


      var path = element.Attribute( "path" ).Value();

      if ( path != null )
        dataObject = DataBinder.Eval( dataObject, path );


      var format = element.Attribute( "format" ).Value();


      IEnumerable listValue = dataObject as IEnumerable;

      if ( listValue != null && element.Exists( "view" ) && format == null )
      {

        foreach ( var dataItem in listValue )
        {

          PushData( context.Data, dataItem );

          element.RenderChilds( context );

          PopData( context.Data );
        }

        return;
      }



      var variableName = element.Attribute( "var" ).Value() ?? element.Attribute( "variable" ).Value();
      if ( variableName != null )
      {

        if ( format != null )
          dataObject = string.Format( format, dataObject );


        var hostName = element.Attribute( "host" ).Value();
        if ( hostName == null )
          context.Writer.WriteLine( "<script type=\"text/javascript\">(function(){{ this['{0}'] = {1} }})();</script>", variableName, ToJson( dataObject ) );

        else
          context.Writer.WriteLine( "<script type=\"text/javascript\">(function(){{ this['{0}']['{1}'] = {2} }})();</script>", hostName, variableName, ToJson( dataObject ) );

        return;
      }


      var bindValue = string.Format( format ?? "{0}", dataObject );

      var attributeName = element.Attribute( "attribute" ).Value() ?? element.Attribute( "attr" ).Value();
      if ( attributeName != null )
      {
        var nextElement = element.NextElement();
        if ( nextElement == null )
          return;
        nextElement.SetAttribute( attributeName, bindValue );
        return;
      }

      context.Write( bindValue );

    }


    private object GetDataObject( Hashtable data )
    {
      var dataStack = data[this] as Stack;

      if ( dataStack != null && dataStack.Count > 0 )
        return dataStack.Peek();

      else
        return null;
    }


    private void PushData( Hashtable data, object dataObject )
    {
      var dataStack = data[this] as Stack;
      if ( dataStack == null )
        data[this] = dataStack = new Stack();

      dataStack.Push( dataObject );
    }


    private void PopData( Hashtable data )
    {
      var dataStack = data[this] as Stack;
      if ( dataStack == null )
        throw new InvalidOperationException();

      dataStack.Pop();
    }




    private string ToJson( object dataObject )
    {
      var serializer = new JavaScriptSerializer();
      return serializer.Serialize( dataObject );
    }




    /// <summary>
    /// 用于匹配 view 标签的 CSS 选择器
    /// </summary>
    protected override string CssSelector
    {
      get { return "view"; }
    }

  }
}
