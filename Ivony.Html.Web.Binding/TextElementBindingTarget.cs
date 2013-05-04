using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ivony.Html.Web.Binding
{
  public class TextElementBindingTarget : IBindingTarget
  {

    private IHtmlElement _targetElement;

    public TextElementBindingTarget( IHtmlElement element )
    {
      _targetElement = element;
    }

    public Type ValueType
    {
      get { return typeof( string ); }
    }

    public void BindValue( object value )
    {

      var parent = _targetElement.Parent();

      if ( parent == null )
        throw new NotSupportedException();

      var text = (string) value;

      if ( parent.ElementTextMode() == TextMode.Preformated )
      {
        parent.AddTextNode( _targetElement.NodesIndexOfSelf(), HtmlEncoding.HtmlEncode( text ) );
        _targetElement.Remove();
      }
      else
      {
        var encoded = HtmlEncoding.HtmlEncode( text );

        encoded = encoded.Replace( "  ", "&nbsp; " );

        if ( encoded.EndsWith( "  " ) )
          encoded = encoded.Substring( 0, encoded.Length - 1 ) + "&nbsp;";//如果末尾多出一个空格，则替换为&nbsp;

        encoded = encoded.Replace( "\r\n", "\n" ).Replace( "\r", "\n" );

        encoded = encoded.Replace( "\n", "<br />" );

        _targetElement.ReplaceWith( encoded );
      }
    }
  }
}
