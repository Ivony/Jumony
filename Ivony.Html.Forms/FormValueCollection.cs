using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;

namespace Ivony.Html.Forms
{
  public class FormValueCollection
  {


    private HtmlForm _form;

    internal FormValueCollection( HtmlForm form )
    {
      _form = form;
    }

    public string this[string name]
    {
      get { return GetValue( name ); }
      set { SetValue( name, value ); }
    }


    public string GetValue( string name )
    {
      var control = _form.InputControls[name];
      if ( control == null )
        return null;

      else
        return control.Value();
    }


    public string[] GetValues( string name )
    {
      var control = _form.InputControls[name];
      if ( control == null )
        return null;


      var groupControl = control as IHtmlGroupControl;
      if ( groupControl != null )
        return groupControl.CurrentValues();

      else
        return new[] { control.Value() };
    }


    public void SetValue( string name, string value )
    {
      var control = _form.InputControls[name];
      if ( control == null )
        throw new InvalidOperationException( string.Format( "表单中找不到名为 \"{0}\" 的控件", name ) );

      control.SetValue( value );

    }

    public void SetValues( string name, string[] values )
    {
      var control = _form.InputControls[name];
      if ( control == null )
        throw new InvalidOperationException( string.Format( "表单中找不到名为 \"{0}\" 的控件", name ) );

      var groupControl = control as IHtmlGroupControl;
      if ( groupControl != null )
        groupControl.SetValue( values );
    }






  }
}
