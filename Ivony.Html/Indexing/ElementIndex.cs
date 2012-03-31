using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ivony.Fluent;

namespace Ivony.Html.Indexing
{
  /// <summary>
  /// 所有元素索引的基类型
  /// </summary>
  public abstract class ElementIndex
  {


    /// <summary>
    /// 创建元素索引实例
    /// </summary>
    /// <param name="manager">所属的管理器</param>
    protected ElementIndex( IndexManager manager )
    {
      Manager = manager;

      InitializeData();
    }


    /// <summary>
    /// 索引所依附的文档
    /// </summary>
    public IndexManager Manager
    {
      get;
      private set;
    }


    /// <summary>
    /// 派生类重写此方法初始化数据
    /// </summary>
    protected virtual void InitializeData()
    { 
    
    }


    internal void AddElement( IHtmlElement element )
    {
      OnAddElement( element );
    }

    internal void RemoveElement( IHtmlElement element )
    {
      OnRemoveElement( element );
    }

    internal void AddAttribute( IHtmlElement element, IHtmlAttribute attribute )
    {
      OnAddAttribute( element, attribute );
    }

    internal void RemoveAttribute( IHtmlElement element, IHtmlAttribute attribute )
    {
      OnRemoveAttribute( element, attribute );
    }

    /// <summary>
    /// 添加一个元素到索引
    /// </summary>
    /// <param name="element">要添加到索引的元素</param>
    protected abstract void OnAddElement( IHtmlElement element );

    /// <summary>
    /// 从索引中移除一个元素
    /// </summary>
    /// <param name="element">要从索引中移除的元素</param>
    protected abstract void OnRemoveElement( IHtmlElement element );


    /// <summary>
    /// 当元素被添加属性时
    /// </summary>
    /// <param name="element">添加属性的元素</param>
    /// <param name="attribute">被添加的属性</param>
    protected abstract void OnAddAttribute( IHtmlElement element, IHtmlAttribute attribute );

    /// <summary>
    /// 当元素被移除属性时
    /// </summary>
    /// <param name="element">移除属性的元素</param>
    /// <param name="attribute">被移除的属性</param>
    protected abstract void OnRemoveAttribute( IHtmlElement element, IHtmlAttribute attribute );

  }
}
