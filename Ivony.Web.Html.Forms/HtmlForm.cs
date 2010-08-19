using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ivony.Fluent;

namespace Ivony.Web.Html.Forms
{
  public class HtmlForm : IHtmlForm
  {
    private IHtmlElement _element;

    public IHtmlElement Element
    {
      get { return _element; }
    }

    public HtmlForm( IHtmlElement element )
    {
      _element = element;

      Initialize();

    }

    private void Initialize()
    {
      var textInputs =
        Element.Find( "input[type=text]" )
          .Union( Element.Find( "input[type=password]" ) )
          .Union( Element.Find( "input[type=hidden]" ) )
          .Union( Element.Find( "textarea" ) )
          .Select( element => new HtmlInputText( element ) );


      var groupInputs = HtmlInputGroup.CaptureInputGroups( this );
    }
  }
}
