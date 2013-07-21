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

    /// <summary>
    /// 获取指定字段的值
    /// </summary>
    /// <param name="name">字段名</param>
    /// <returns>值</returns>
    public string this[string name]
    {
      get { return GetValue( name ); }
      set { SetValue( name, value ); }
    }


    /// <summary>
    /// 获取指定字段的值
    /// </summary>
    /// <param name="name">字段名</param>
    /// <returns>值</returns>
    public string GetValue( string name )
    {
      var control = _form.InputControls[name];
      if ( control == null )
        return null;

      else
        return control.Value();
    }


    /// <summary>
    /// 获取指定多选字段的值
    /// </summary>
    /// <param name="name">字段名</param>
    /// <returns>所有的值（如果是单值字段则返回只包含一个值的数组）</returns>
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


    /// <summary>
    /// 为表单设置值
    /// </summary>
    /// <param name="name">字段名</param>
    /// <param name="value">字段值</param>
    public void SetValue( string name, string value )
    {
      var control = _form.InputControls[name];
      if ( control == null )
        throw new InvalidOperationException( string.Format( "表单中找不到名为 \"{0}\" 的控件", name ) );

      control.SetValue( value );

    }

  }
}
