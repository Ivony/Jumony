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
  public class ElementBinder : IHtmlElementBinder
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


      //如果数据源是列表，检测是否适用列表绑定

      if ( element.Exists( "view, binding" ) && format == null )
      {
        IEnumerable listValue = dataObject as IEnumerable;



        if ( listValue != null )
          dataContext = listValue;

        else
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
        if ( hostName == null )
          element.ReplaceWith( string.Format( "<script type=\"text/javascript\">(function(){{ this['{0}'] = {1} }})();</script>", variableName, ToJson( dataObject ) ) );

        else
          element.ReplaceWith( string.Format( "<script type=\"text/javascript\">(function(){{ this['{0}']['{1}'] = {2} }})();</script>", hostName, variableName, ToJson( dataObject ) ) );

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



  }
}
