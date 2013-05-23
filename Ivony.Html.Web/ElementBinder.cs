using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;
using Ivony.Fluent;
using Ivony.Html.ExpandedAPI;

namespace Ivony.Html.Web
{
  public class ElementBinder : IHtmlElementBinder
  {
    public void BindElement( HtmlBindingContext context )
    {

      var element = context.CurrentElement;
      if ( !element.Name.EqualsIgnoreCase( "view" ) && !element.Name.EqualsIgnoreCase( "binding" ) )
        return;

      //获取绑定数据源
      var key = element.Attribute( "key" ).Value() ?? element.Attribute( "name" ).Value();

      object dataObject;
      if ( key != null )
        context.Data.TryGetValue( key, out dataObject );
      else
        dataObject = context.DataContext;


      if ( dataObject == null )
        return;

      var path = element.Attribute( "path" ).Value();

      if ( path != null )
        dataObject = DataBinder.Eval( dataObject, path );



      var format = element.Attribute( "format" ).Value();


      //如果数据源是列表，检测是否适用列表绑定

      if ( element.Exists( "view, binding" ) && format == null )
      {
        IEnumerable listValue = dataObject as IEnumerable;



        if ( listValue != null )
          context.SetListDataContext( dataObject );

        else
          context.SetDataContext( dataObject );

        return;
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

        return;
      }



      //绑定为 HTML 文本
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

      element.ReplaceWith( bindValue );
    }


    private string ToJson( object dataObject )
    {
      var serializer = new JavaScriptSerializer();
      return serializer.Serialize( dataObject );
    }


  }
}
