using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Web.UI;
using System.Web.Mvc;

namespace Ivony.Html.Web.Mvc
{
  public class ViewElementAdapter : HtmlElementAdapter
  {

    private ViewContext _context;

    public ViewElementAdapter( ViewContext context )
    {
      _context = context;
    }


    /// <summary>
    /// 渲染 view 标签
    /// </summary>
    /// <param name="element"></param>
    /// <param name="writer"></param>
    public override void Render( IHtmlElement element, TextWriter writer )
    {

      var key = element.Attribute( "key" ).Value();
      var dataObject = _context.ViewData[key];

      string bindValue = null;

      if ( dataObject == null )
        return;

      var path = element.Attribute( "path" ).Value();
      var format = element.Attribute( "format" ).Value();


      if ( path == null )
        bindValue = string.Format( format ?? "{0}", dataObject );
      else
        bindValue = DataBinder.Eval( dataObject, path, format ?? "{0}" );


      var attribute = element.Attribute( "attribute" ).Value();
      if ( attribute != null )
      {
        element.NextElement().SetAttribute( attribute, bindValue );
        return;
      }

      writer.Write( bindValue );

    }


    protected override string CssSelector
    {
      get { return "view"; }
    }

  }
}
