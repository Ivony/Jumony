using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ivony.Fluent;

namespace Ivony.Html.Forms
{
  public class FormButtonGroupItem : FormGroupControlItem
  {

    internal FormButtonGroupItem( FormButtonGroup groupControl, IHtmlElement element )
      : base( groupControl )
    {

      if ( groupControl == null )
        throw new ArgumentNullException( "groupControl" );

      if ( element == null )
        throw new ArgumentNullException( "element" );

      if ( !element.Name.EqualsIgnoreCase( "input" ) )
        throw new InvalidOperationException();

      if ( !element.Attribute( "name" ).Value().EqualsIgnoreCase( groupControl.Name ) )
        throw new InvalidOperationException();

      var type = element.Attribute( "type" ).Value();

      if ( type.EqualsIgnoreCase( "radio" ) )
        ButtonType = FormGroupButtonType.RadioButton;

      else if ( type.EqualsIgnoreCase( "checkbox" ) )
        ButtonType = FormGroupButtonType.RadioButton;

      else
        throw new InvalidOperationException();

      Element = element;
    }



    public IHtmlElement Element
    {
      get;
      private set;
    }

    public override string Value
    {
      get { throw new NotImplementedException(); }
    }

    public override bool Selected
    {
      get
      {
        return Element.Attribute( "checked" ) != null;
      }
      set
      {
        if ( value )
          Element.SetAttribute( "checked" );
        else
          Element.RemoveAttribute( "checked" );
      }
    }

    public FormGroupButtonType ButtonType
    {
      get;
      private set;
    }
  }

  public enum FormGroupButtonType
  {
    RadioButton,
    CheckBox
  }

}
