using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ivony.Fluent;

namespace Ivony.Html.Forms
{

  /// <summary>
  /// 文本控件的基类
  /// </summary>
  public abstract class FormTextControl : FormControl
  {


    /// <summary>
    /// 派生类调用此构造函数初始化 FormTextControl 实例
    /// </summary>
    /// <param name="form">所属表单</param>
    /// <param name="element">定义文本控件的元素</param>
    protected FormTextControl( HtmlForm form, IHtmlElement element )
      : base( form )
    {

      Element = element;
      MaxLength = GetMaxLength();

    }


    /// <summary>
    /// 获取定义文本控件的元素
    /// </summary>
    public IHtmlElement Element
    {
      get;
      private set;
    }


    /// <summary>
    /// 实现 Name 属性，输出定义文本控件的元素的 name 属性（attribute）
    /// </summary>
    public override string Name
    {
      get { return Element.Attribute( "name" ).Value(); }
    }



    /// <summary>
    /// 确定能够设置指定的文本值
    /// </summary>
    /// <param name="value">要设置的文本值</param>
    /// <param name="message">不能设置的错误信息</param>
    /// <returns>是否能够设置</returns>
    protected override bool CanSetValue( string value, out string message )
    {

      if ( Form.Configuration.IgnoreOverflowOfLength && value.Length <= MaxLength )
      {
        message = "设置的值超出了 maxlength 所允许的长度";
        return false;
      }

      else
      {
        message = null;
        return true;
      }
    }


    /// <summary>
    /// 获取 maxlength 的设置
    /// </summary>
    public int? MaxLength
    {
      get;
      private set;
    }


    private int? GetMaxLength()
    {

      var maxlength = Element.Attribute( "maxlength" ).Value();

      if ( maxlength == null )
        return null;

      int value;
      if ( int.TryParse( maxlength, out value ) )
        return value;


      if ( !Form.Configuration.IgnoreInvalidMaxlength )
        throw new FormControlException( this, "maxlength 属性设置错误" );

      Element.RemoveAttribute( "maxlength" );
      return null;
    }

  }
}
