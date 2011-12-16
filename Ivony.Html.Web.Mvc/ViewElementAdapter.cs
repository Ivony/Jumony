using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Web.UI;
using System.Web.Mvc;

namespace Ivony.Html.Web.Mvc
{

  /// <summary>
  /// 用于渲染 view 标签的元素渲染代理
  /// </summary>
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
    /// <param name="element">view 标签元素</param>
    /// <param name="writer">HTML 编写器</param>
    public override void Render( IHtmlElement element, TextWriter writer )
    {

      var key = element.Attribute( "key" ).Value() ?? element.Attribute( "name" ).Value();

      object dataObject;
      if ( key != null )
        _context.ViewData.TryGetValue( key, out dataObject );
      else
        dataObject = _context.ViewData.Model;


      if ( dataObject == null )
        return;


      string bindValue = null;

      var path = element.Attribute( "path" ).Value();
      var format = element.Attribute( "format" ).Value();


      if ( path == null )
        bindValue = string.Format( format ?? "{0}", dataObject );
      else
        bindValue = DataBinder.Eval( dataObject, path, format ?? "{0}" );


      var attribute = element.Attribute( "attribute" ).Value() ?? element.Attribute( "attr" ).Value();
      if ( attribute != null )
      {
        element.NextElement().SetAttribute( attribute, bindValue );
        return;
      }

      writer.Write( bindValue );

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
