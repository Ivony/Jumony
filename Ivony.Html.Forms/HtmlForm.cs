using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Ivony.Fluent;
using System.Collections.Specialized;
using System.Web;

namespace Ivony.Html.Forms
{

  /// <summary>
  /// 抽象化 HTML 表单，提供有用的功能。
  /// </summary>
  public class HtmlForm
  {
    private IHtmlElement _element;

    /// <summary>
    /// 表单元素
    /// </summary>
    public IHtmlElement Element
    {
      get { return _element; }
    }



    private IHtmlTextControl[] textControls;
    private IHtmlGroupControl[] groupControls;

    private HtmlLabel[] labels;

    private Hashtable labelsTable = Hashtable.Synchronized( new Hashtable() );



    /// <summary>
    /// 创建一个 HTML 表单对象
    /// </summary>
    /// <param name="element"></param>
    public HtmlForm( IHtmlElement element, FormConfiguration configuration = null, IFormControlProvider[] providers = null )
    {
      _element = element;

      Configuration = configuration ?? FormConfiguration.Default;

      Providers = providers ?? new[] { new StandardFormControlProvider() };
    }


    /// <summary>
    /// 表单控件提供程序
    /// </summary>
    protected IFormControlProvider[] Providers
    {
      get;
      private set;
    }



    /// <summary>
    /// 重新扫描表单中所有控件
    /// </summary>
    public void RefreshForm()
    {

      InputControls = new InputControlCollection( this );

    }



    /// <summary>
    /// 获取表单所有的输入控件
    /// </summary>
    public InputControlCollection InputControls
    {
      get;
      private set;
    }


    private FormValueCollection _formValues;


    public FormValueCollection Values
    {
      get { return _formValues; }
    }


    /// <summary>
    /// 检索指定HTML元素绑定的 Label
    /// </summary>
    /// <param name="element">要检索 Label 的元素</param>
    /// <returns></returns>
    internal HtmlLabel[] FindLabels( string elementId )
    {
      if ( elementId == null )
        return new HtmlLabel[0];

      return labelsTable[elementId].CastTo<HtmlLabel[]>().IfNull( new HtmlLabel[0] );
    }


    public FormConfiguration Configuration { get; private set; }
  }
}
