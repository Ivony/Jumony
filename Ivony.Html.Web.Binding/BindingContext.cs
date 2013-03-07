using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;

namespace Ivony.Html.Web.Binding
{

  /// <summary>
  /// 绑定上下文
  /// </summary>
  public class BindingContext
  {


    /// <summary>
    /// 创建 BindingContext 对象
    /// </summary>
    /// <param name="manager">绑定管理器</param>
    /// <param name="dataContext">数据上下文</param>
    public BindingContext( BindingManager manager, object dataContext )
    {

      BindingManager = manager;
      SetDataContext( manager.Document, dataContext );

    }

    /// <summary>
    /// 绑定管理器
    /// </summary>
    public BindingManager BindingManager
    {
      get;
      private set;
    }

    /// <summary>
    /// 数据上下文
    /// </summary>
    public object DataContext
    {
      get { return _dataContextStack.Peek().DataContext; }
    }

    /// <summary>
    /// 数据上下文容器
    /// </summary>
    public IHtmlContainer DataContainer
    {
      get { return _dataContextStack.Peek().DataContainer; }
    }

    /// <summary>
    /// 正在进行绑定的元素
    /// </summary>
    public IHtmlElement BindingElement
    {
      get;
      set;
    }


    private class DataContextStackItem
    {
      public IHtmlContainer DataContainer { get; set; }
      public object DataContext { get; set; }
    }


    private Stack<DataContextStackItem> _dataContextStack = new Stack<DataContextStackItem>();

    internal void Enter( IHtmlElement element )
    {
      BindingElement = element;

      return;
    }

    private void SetDataContext( IHtmlContainer element, object dataContext )
    {
      _dataContextStack.Push(
        new DataContextStackItem()
        {
          DataContainer = element,
          DataContext = dataContext
        } );
    }

    internal void Exit( IHtmlElement element )
    {
      BindingElement = null;
      if ( element == DataContainer )
        _dataContextStack.Pop();
    }
  }
}
