using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ivony.Fluent;

namespace Ivony.Html.Web.Binding
{

  /// <summary>
  /// 脚本绑定器，用于解析脚本中的绑定表达式。
  /// </summary>
  public class ScriptBinder : IHtmlBinder
  {




    public bool BindElement( HtmlBindingContext context, IHtmlElement element )
    {
      if ( !element.Name.EqualsIgnoreCase( "script" ) )
        return false;


      var script = element.InnerHtml();

      return false;

    }
  }
}
