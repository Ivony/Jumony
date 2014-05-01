using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ivony.Html.Forms
{


  /// <summary>
  /// IFormControl 的抽象实现
  /// </summary>
  public abstract class FormControl : IFormControl
  {



    /// <summary>
    /// 构建 FormControl 实例
    /// </summary>
    /// <param name="form">控件所属表单</param>
    protected FormControl( HtmlForm form )
    {

      Form = form;

    }




    /// <summary>
    /// 控件所属表单
    /// </summary>
    public HtmlForm Form
    {
      get;
      private set;
    }



    /// <summary>
    /// 控件名称
    /// </summary>
    public abstract string Name { get; }




    /// <summary>
    /// 实现 Value 属性，调用 SetValue 和 GetValue 方法
    /// </summary>
    public virtual string Value
    {
      get
      {
        return GetValue();
      }
      set
      {
        EnsureValue( value );
        SetValue( value );
      }
    }



    /// <summary>
    /// 确保设置的值是合法的
    /// </summary>
    /// <param name="value">要设置的值</param>
    protected virtual void EnsureValue( string value )
    {
      string message;
      if ( !CanSetValue( value, out message ) )
        throw new FormValueFormatException( this, message );
    }


    /// <summary>
    /// 派生类实现此方法设置控件值
    /// </summary>
    /// <param name="value">要设置的值</param>
    protected abstract void SetValue( string value );


    /// <summary>
    /// 派生类实现此方法获取控件设置的值
    /// </summary>
    /// <returns>控件设置的值</returns>
    protected abstract string GetValue();



    /// <summary>
    /// 确定能否设置指定的文本值
    /// </summary>
    /// <param name="value">要设置的文本值</param>
    /// <returns>是否能够设置</returns>
    public virtual bool CanSetValue( string value )
    {
      string message = null;
      return CanSetValue( value, out message );
    }


    /// <summary>
    /// 派生类实现此方法确定指定的文本值是否能够被设置
    /// </summary>
    /// <param name="value">要设置的文本值</param>
    /// <param name="message">不能设置的错误信息</param>
    /// <returns>是否能够设置</returns>
    protected abstract bool CanSetValue( string value, out string message );



  }
}
