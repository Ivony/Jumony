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

    protected FormTextControl( HtmlForm form, IHtmlElement element )
      : base( form )
    {

      Element = element;
      MaxLength = GetMaxLength();

    }

    public IHtmlElement Element
    {
      get;
      private set;
    }


    public override string Name
    {
      get { return Element.Attribute( "name" ).Value(); }
    }

    public override string Value
    {
      get { return GetValue(); }
      set { SetValueInternal( value ); }
    }



    private void SetValueInternal( string value )
    {
      if ( Form.Configuration.CheckMaxLength && value.Length > MaxLength )
        throw new FormValueFormatException( this, "设置的值超出了 maxlength 所允许的长度" );

      SetValue( value );
    }


    /// <summary>
    /// 获取值
    /// </summary>
    /// <returns>该控件目前设置的值</returns>
    protected abstract string GetValue();


    /// <summary>
    /// 设置值
    /// </summary>
    /// <param name="value">需要设置在该控件上的值</param>
    protected abstract void SetValue( string value );


    public override bool CanSetValue( string value )
    {

      if ( Form.Configuration.CheckMaxLength )
        return value.Length <= MaxLength;

      else
        return true;
    }


    public int? MaxLength
    {
      get;
      private set;
    }


    private int? GetMaxLength()
    {
      int value;
      if ( int.TryParse( Element.Attribute( "maxlength" ).Value(), out value ) )
        return value;


      if ( Form.Configuration.ExceptionOnAttributeError )
        throw new FormControlException( this, "maxlength 属性设置错误" );

      Element.RemoveAttribute( "maxlength" );
      return null;
    }

  }
}
