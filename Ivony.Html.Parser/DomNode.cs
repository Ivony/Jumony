using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ivony.Html.Parser
{

  /// <summary>
  /// IHtmlNode 的实现
  /// </summary>
  public abstract class DomNode : DomObject, IHtmlNode
  {

    private IDomContainer _container;

    /// <summary>
    /// 获取节点的容器
    /// </summary>
    /// <remarks>
    /// 若节点已从 DOM 移除，则此属性返回null
    /// </remarks>
    public virtual IHtmlContainer Container
    {
      get
      {
        //CheckDisposed(); 若节点已从 DOM 移除，则此属性返回null。

        return _container;
      }

      internal set
      {
        lock ( SyncRoot )
        {
          CheckDisposed();


          if ( _container is DomFragment && value == null )//从碎片中移除的逻辑
          {
            _container = null;
            return;
          }

          if ( _container != null )
            throw new InvalidOperationException();


          var domContainer = value as IDomContainer;

          if ( domContainer == null )
            throw new InvalidOperationException();

          _container = domContainer;

        }
      }
    }

    /// <summary>
    /// 尝试从 DOM 中移除此节点
    /// </summary>
    /// <remarks>
    /// 一旦从 DOM 中移除节点，则节点及其子节点均会被标为已销毁
    /// </remarks>
    public virtual void Remove()
    {
      lock ( SyncRoot )
      {
        if ( removed )
          return;

        if ( Container == null )
          throw new InvalidOperationException();

        _container.NodeCollection.Remove( this );
        _container = null;
        removed = true;
      }
    }

    /// <summary>
    /// 获取节点所属的文档
    /// </summary>
    public override IHtmlDocument Document
    {
      get
      {
        CheckDisposed();

        return Container.Document;
      }
    }


    protected bool removed = false;

    /// <summary>
    /// 检查对象是否已被销毁，如果已被销毁则抛出异常
    /// </summary>
    protected void CheckDisposed()
    {
      if ( removed )
        throw new ObjectDisposedException( ObjectName );
    }


    /// <summary>
    /// 派生类实现此属性提供对象名称，当抛出 ObjectDisposedException 异常时将使用此名称
    /// </summary>
    protected abstract string ObjectName
    {
      get;
    }


    /// <summary>
    /// 获取节点的 HTML 表达形式，默认将调用OuterHtml方法
    /// </summary>
    /// <returns>节点的 HTML 表达形式</returns>
    public override string ToString()
    {
      CheckDisposed();

      return this.OuterHtml();
    }

  }
}
