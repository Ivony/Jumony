using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ivony.Html.Controls
{
  public class ControlRenderAdapter : IHtmlRenderAdapter
  {
    public bool Render( IHtmlNode node, IHtmlRenderContext context )
    {

      var element = node as IHtmlElement;
      if ( element == null )              //只处理元素
        return false;

      var control = CreateControl( element );


      throw new NotImplementedException();
    }

    private HtmlControl CreateControl( IHtmlElement element )
    {
      throw new NotImplementedException();
    }
  }
}
