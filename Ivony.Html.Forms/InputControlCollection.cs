using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ivony.Html.Forms
{
  public class InputControlCollection : IEnumerable<IHtmlInputControl>
  {


    private IDictionary<string, IHtmlInputControl> _controls = new Dictionary<string, IHtmlInputControl>();


    internal InputControlCollection( HtmlForm form )
    {

      foreach ( var control in form.GroupControls )
        Add( control );

      foreach ( var control in form.TextControls )
        Add( control );

    }


    private void Add( IHtmlInputControl inputControl )
    {
      if ( inputControl == null )
        throw new ArgumentNullException( "inputControl" );

      _controls.Add( inputControl.Name, inputControl );
    }



    /// <summary>
    /// 获取指定名称的输入控件
    /// </summary>
    /// <param name="name">输入控件的名称</param>
    /// <returns>指定名称的输入控件</returns>
    public IHtmlInputControl this[string name]
    {
      get
      {
        IHtmlInputControl control;
        if ( _controls.TryGetValue( name, out control ) )
          return control;

        else
          return null;
      }
    }


    IEnumerator<IHtmlInputControl> IEnumerable<IHtmlInputControl>.GetEnumerator()
    {
      return _controls.Values.GetEnumerator();
    }

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
      return _controls.Values.GetEnumerator();
    }

  }
}
