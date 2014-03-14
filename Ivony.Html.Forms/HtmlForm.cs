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



    public FormControlCollection Controls
    {
        get;
        private set;
    }


    /// <summary>
    /// 创建一个 HTML 表单对象
    /// </summary>
    /// <param name="element"></param>
    public HtmlForm( IHtmlElement element, FormConfiguration configuration = null, IFormProvider provider = null )
    {
      _element = element;

      Configuration = configuration ?? FormConfiguration.Default;
      Provider = provider ?? new StandardFormProvider();


      RefreshForm();
    }


    /// <summary>
    /// 表单控件提供程序
    /// </summary>
    protected IFormProvider Provider
    {
      get;
      private set;
    }



    /// <summary>
    /// 重新扫描表单中所有控件
    /// </summary>
    public void RefreshForm()
    {

      Controls = new FormControlCollection( Provider.DiscoveryControls( this ) );


    }



    public FormConfiguration Configuration { get; private set; }
  }
}
