using System;
namespace Ivony.Html.Forms
{

  /// <summary>
  /// 表示 HTML 输入组控件的抽象，例如多选单选按钮组和SELECT元素
  /// </summary>
  public interface IHtmlGroupControl : IHtmlInputControl
  {
    /// <summary>
    /// 是否支持多选
    /// </summary>
    bool AllowMultipleSelections
    {
      get;
    }


    /// <summary>
    /// 所有的输入组项
    /// </summary>
    IHtmlInputGroupItem[] Items
    {
      get;
    }
  }

  /// <summary>
  /// 表示 HTML 输入组项的抽象，例如OPTION或是多选单选按钮。
  /// </summary>
  public interface IHtmlInputGroupItem : IHtmlFormElement
  {

    /// <summary>
    /// 输入组项所对应的元素
    /// </summary>
    IHtmlElement Element
    {
      get;
    }

    /// <summary>
    /// 输入组项所对应的输入组控件
    /// </summary>
    IHtmlGroupControl Group
    {
      get;
    }

    /// <summary>
    /// 该项是否已被选中
    /// </summary>
    bool Selected
    {
      get;
      set;
    }

    /// <summary>
    /// 该项的值
    /// </summary>
    string Value
    {
      get;
    }

    /// <summary>
    /// 该项的显示文本
    /// </summary>
    string Text
    {
      get;
    }
  }
}
