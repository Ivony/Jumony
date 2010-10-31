using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ivony.Web.Html.Forms
{
  public class HtmlTextInput : IHtmlInput
  {

    private IHtmlElement _element;
    private string _valueAttributeName;

    public HtmlTextInput( IHtmlElement element )
    {

      if ( string.Equals( element.Name, "input", StringComparison.InvariantCultureIgnoreCase ) )
      {
        var inputTypes = new[] { "text", "password", "hidden" };

        if ( !inputTypes.Contains( element.Attribute( "type" ).Value, StringComparer.InvariantCultureIgnoreCase ) )
          throw new InvalidOperationException( "只有type为text、password或hidden的input元素才能转换为HtmlTextInput对象" );

        _element = element;
        _valueAttributeName = "value";
      }
      else if ( string.Equals( element.Name, "textarea", StringComparison.InvariantCultureIgnoreCase ) )
      {
        _element = element;
        _valueAttributeName = ":text";
      }

      if ( _element == null )
        throw new InvalidOperationException( "只有input或textarea元素才能转换为HtmlTextInput对象" );
    }

    public string Name
    {
      get { return _element.Attribute( "name" ).Value; }
    }

    public string Value
    {
      get
      {
        throw new NotImplementedException();
      }
      set
      {
        throw new NotImplementedException();
      }
    }
  }
}
