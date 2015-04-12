using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Ivony.Html.Binding
{
  internal class ExpressionElementBinder : IHtmlElementBinder
  {

    private IElementExpressionBinder _binder;

    public ExpressionElementBinder( IElementExpressionBinder binder )
    {
      _binder = binder;
    }

    public string ElementName
    {
      get { return _binder.ExpressionName; }
    }

    public void BindElement( HtmlBindingContext context, IHtmlElement element )
    {
      var expression = new ElementExpression( element );

      BindWithValue( element, HtmlBindingContext.GetValue( context, expression, _binder ) );
    }


    private void BindWithValue( IHtmlElement element, object value )
    {
      if ( value == null )
      {
        element.Remove();
        return;
      }

      var htmlString = value as IHtmlString;
      if ( htmlString != null )
      {
        element.ReplaceWith( htmlString.ToHtmlString() );
        return;
      }


      var text = value.ToString();

      var mode = ElementTextMode( element );
      var fragmentManager = element.Document.FragmentManager;

      if ( mode == TextMode.Preformated )
      {
        var fragment = fragmentManager.CreateFragment();
        fragment.AddTextNode( HtmlEncoding.HtmlEncode( text ) );
        element.ReplaceWith( fragment );
      }

      else if ( mode == TextMode.Normal )
      {
        var fragment = DomExtensions.ParseText( text, fragmentManager );
        element.ReplaceWith( fragment );
      }

      else if ( mode == TextMode.CData )//不应当发生的情况
        throw new InvalidOperationException();
    }

    private TextMode ElementTextMode( IHtmlElement element )
    {
      var parent = element.Parent();
      if ( parent == null )
        return TextMode.Normal;

      else
        return parent.ElementTextMode();
    }
  }
}
