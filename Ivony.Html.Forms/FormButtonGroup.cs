using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ivony.Fluent;

namespace Ivony.Html.Forms
{

  /// <summary>
  /// 为单选框和复选框组实现控件抽象
  /// </summary>
  public class FormButtonGroup : FormGroupControl
  {

    internal FormButtonGroup( HtmlForm form, string name, IHtmlElement[] buttonElements )
      : base( form )
    {
      _name = name;
      ButtonItems = buttonElements.Select( element => CreateGroupItem( element ) ).ToArray();

      _allowMultiple = ButtonItems.Any( item => item.ButtonType == FormGroupButtonType.CheckBox );
    }

    private FormButtonGroupItem CreateGroupItem( IHtmlElement element )
    {
      return new FormButtonGroupItem( this, element );
    }

    /// <summary>
    /// 实现 Items 属性，提供控件项
    /// </summary>
    protected override FormGroupControlItem[] Items
    {
      get { return ButtonItems; }
    }


    /// <summary>
    /// 获取组控件中的按钮项
    /// </summary>
    protected FormButtonGroupItem[] ButtonItems
    {
      get;
      private set;
    }



    private bool _allowMultiple;

    /// <summary>
    /// 获取一个值，指示该控件组是否允许多个值。
    /// </summary>
    public override bool AllowMultiple
    {
      get { throw new NotImplementedException(); }
    }




    private string _name;

    /// <summary>
    /// 获取控件名
    /// </summary>
    public override string Name
    {
      get { return _name; }
    }
  }
}
