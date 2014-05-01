using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ivony.Fluent;

namespace Ivony.Html.Forms
{

  /// <summary>
  /// 表示一个 &lt;select&gt; 元素
  /// </summary>
  public class HtmlSelect : FormGroupControl
  {

    internal HtmlSelect( HtmlForm form, IHtmlElement element )
      : base( form )
    {

      Element = element;
      options = element.Find( "option" ).Select( e => new HtmlOption( this, e ) ).ToArray();

    }


    /// <summary>
    /// 获取 DOM 上的 &lt;select&gt; 元素
    /// </summary>
    public IHtmlElement Element
    {
      get;
      private set;
    }



    private readonly HtmlOption[] options;



    /// <summary>
    /// 获取输入控件名
    /// </summary>
    public override string Name
    {
      get { return Element.Attribute( "name" ).Value(); }
    }


    /// <summary>
    /// 是否允许多选
    /// </summary>
    public override bool AllowMultiple
    {
      get { return Element.Attribute( "multiple" ) != null; }
    }



    /// <summary>
    /// 选项列表
    /// </summary>
    protected override FormGroupControlItem[] Items
    {
      get { return options; }
    }

  }



  /// <summary>
  /// 表示一个 &lt;option&gt; 元素
  /// </summary>
  public class HtmlOption : FormGroupControlItem
  {

    /// <summary>
    /// 创建 HtmlOption 对象
    /// </summary>
    /// <param name="select">所属的 HtmlSelect 对象</param>
    /// <param name="element">DOM 上对应的 &lt;option&gt; 元素</param>
    public HtmlOption( HtmlSelect select, IHtmlElement element )
      : base( select )
    {
      Element = element;
    }


    /// <summary>
    /// 获取 &lt;option&gt; 元素
    /// </summary>
    public IHtmlElement Element
    {
      get;
      private set;
    }


    /// <summary>
    /// 是否为选中状态
    /// </summary>
    public override bool Selected
    {
      get { return Element.Attribute( "selected" ) != null; }
      set
      {
        if ( value )
        {
          Element.SetAttribute( "selected", "selected" );
        }
        else
        {
          var attribute = Element.Attribute( "selected" );
          if ( attribute != null )
            attribute.Remove();
        }
      }
    }


    /// <summary>
    /// 当前项的值
    /// </summary>
    public override string Value
    {
      get
      {
        var value = Element.Attribute( "value" ).Value();

        if ( value == null )
          return Element.InnerText();
        else
          return value;
      }
    }


    /// <summary>
    /// 当前项的文本表现形式
    /// </summary>
    public string Text
    {
      get { return Element.InnerText(); }
    }

  }

}
