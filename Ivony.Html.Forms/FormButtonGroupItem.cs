using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ivony.Fluent;

namespace Ivony.Html.Forms
{

  /// <summary>
  /// 定义按钮组组控件的项，即单个按钮
  /// </summary>
  public sealed class FormButtonGroupItem : FormGroupControlItem
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


    /// <summary>
    /// 定义按钮的元素
    /// </summary>
    public IHtmlElement Element
    {
      get;
      private set;
    }

    /// <summary>
    /// 控件值，当按钮处于被选中状态时应当提供的值
    /// </summary>
    public override string Value
    {
      get { return Element.Attribute( "value" ).Value(); }
    }

    /// <summary>
    /// 按钮是否处于选中状态
    /// </summary>
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

    /// <summary>
    /// 按钮类型
    /// </summary>
    public FormGroupButtonType ButtonType
    {
      get;
      private set;
    }
  }


  /// <summary>
  /// 定义按钮组控件类型
  /// </summary>
  public enum FormGroupButtonType
  {

    /// <summary>单选按钮</summary>
    RadioButton,
    /// <summary>多选按钮</summary>
    CheckBox
  }

}
