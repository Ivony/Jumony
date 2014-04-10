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



    /// <summary>
    /// 获取所有表单控件
    /// </summary>
    public FormControlCollection Controls
    {
      get;
      private set;
    }


    /// <summary>
    /// 创建一个 HTML 表单对象
    /// </summary>
    /// <param name="element">表单元素</param>
    /// <param name="configuration">表单配置</param>
    /// <param name="provider">表单控件提供程序</param>
    public HtmlForm( IHtmlElement element, FormConfiguration configuration = null, IFormProvider provider = null )
    {

      if ( element == null )
        throw new ArgumentNullException( "element" );

      var document = element.Document;

      if ( document == null )
        throw new InvalidOperationException();


      var modifier = document.DomModifier as ISynchronizedDomModifier;
      if ( modifier == null )
        throw new InvalidOperationException();


      SyncRoot = modifier.SyncRoot;
      
      _element = element;

      Configuration = configuration ?? new FormConfiguration();
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

      lock ( SyncRoot )
      {
        Controls = new FormControlCollection( Provider.DiscoveryControls( this ) );
      }

    }




    /// <summary>
    /// 提供一个容器，可以对当前表单数据进行暂存，
    /// </summary>
    public Hashtable FormData
    {
      get;
      private set;
    }



    /// <summary>
    /// 获取该表单的配置对象
    /// </summary>
    public FormConfiguration Configuration { get; private set; }




    /// <summary>
    /// 用于 HTML DOM 同步的对象，当对表单进行扫描时，将锁住 HTML DOM 避免出现问题
    /// </summary>
    public object SyncRoot
    {
      get;
      private set;
    }


  }
}
