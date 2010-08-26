using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ivony.Web.Html.Forms
{
  /// <summary>
  /// 表示 label 元素
  /// </summary>
  public class HtmlLabel
  {
    public HtmlLabel( HtmlForm form, IHtmlElement element )
    {
      if ( !element.Name.Equals( "label", StringComparison.InvariantCultureIgnoreCase ) )
        throw new NotSupportedException( "不能将非 label 元素转换为 HtmlLabel 对象" );

      Element = element;

      var forId = Element.Attribute( "for" ).Value();
      if ( forId != null )
      {
        BindElement = Form.Element.Find( "#" + forId ).SingleOrDefault();
      }
    }

    public HtmlForm Form
    {
      get;
      private set;
    }

    public IHtmlElement Element
    {
      get;
      private set;
    }

    public IHtmlElement BindElement
    {
      get;
      private set;
    }

  }
}
