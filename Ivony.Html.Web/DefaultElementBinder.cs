using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;
using System.Web.UI;
using Ivony.Fluent;
using Ivony.Html.ExpandedAPI;

namespace Ivony.Html.Web
{

  /// <summary>
  /// 默认的元素绑定器，处理 &lt;view&gt; 或者 &lt;binding&gt; 元素
  /// </summary>
  public class DefaultElementBinder : IHtmlElementBinder
  {
    public bool BindElement( IHtmlElement element, HtmlBindingContext context, out object dataContext )
    {

      dataContext = null;

      if ( !element.Name.EqualsIgnoreCase( "view" ) && !element.Name.EqualsIgnoreCase( "binding" ) )
        return false;

      //获取绑定数据源
      var key = element.Attribute( "key" ).Value() ?? element.Attribute( "name" ).Value();

      object dataObject;
      if ( key != null )
        context.Data.TryGetValue( key, out dataObject );
      else
        dataObject = context.DataContext;


      if ( dataObject == null )
        return false;

      var path = element.Attribute( "path" ).Value();

      if ( path != null )
        dataObject = DataBinder.Eval( dataObject, path );



      var format = element.Attribute( "format" ).Value();



      //如果有嵌套的绑定标签，则认为是数据上下文绑定
      if ( element.Exists( "view, binding" ) )
      {
        dataContext = dataObject;
        return true;
      }




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



      //绑定为 HTML 文本
      var bindValue = string.Format( format ?? "{0}", dataObject );

      var attributeName = element.Attribute( "attribute" ).Value() ?? element.Attribute( "attr" ).Value();
      if ( attributeName != null )
      {
        var nextElement = element.NextElement();
        if ( nextElement == null )
          return false;

        nextElement.SetAttribute( attributeName, bindValue );
        return true;
      }

      element.ReplaceWith( bindValue );
      return true;
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
    /// <returns></returns>
    public bool BindAttribute( IHtmlAttribute attribute, HtmlBindingContext context )
    {
      return false;
    }
  }
}
