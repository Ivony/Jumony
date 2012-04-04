using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using Ivony.Fluent;
using System.Globalization;

namespace Ivony.Html.Parser
{
  /// <summary>
  /// IHtmlElement 的实现
  /// </summary>
  public class DomElement : DomNode, IHtmlElement, IDomContainer
  {

    private readonly string _name;
    private readonly DomAttributeCollection _attributes;


    /// <summary>
    /// 创建 DomElement 实例
    /// </summary>
    /// <param name="name"></param>
    /// <param name="attributes"></param>
    public DomElement( string name, IDictionary<string, string> attributes )
    {
      if ( name == null )
        throw new ArgumentNullException( "name" );

      _name = name;
      _attributes = new DomAttributeCollection( this );

      if ( attributes != null )
        attributes.ForAll( item => _attributes.Add( new DomAttribute( this, item.Key, item.Value ) ) );
    }


    /// <summary>
    /// 获取一个名称，用于在抛出 ObjectDisposedException 异常时说明自己
    /// </summary>
    protected override string ObjectName
    {
      get { return "Element"; }
    }


    /// <summary>
    /// 获取元素名
    /// </summary>
    public string Name
    {
      get
      {
        return _name;
      }
    }


    /// <summary>
    /// 获取所有 HTML 属性
    /// </summary>
    /// <returns>所有属性对象</returns>
    public IEnumerable<IHtmlAttribute> Attributes()
    {
      return _attributes.Cast<IHtmlAttribute>().AsReadOnly();
    }


    /// <summary>
    /// 添加一个属性
    /// </summary>
    /// <param name="name">属性名</param>
    /// <param name="value">属性值</param>
    /// <returns>添加后的属性</returns>
    internal IHtmlAttribute AddAttribute( string name, string value )
    {
      lock ( SyncRoot )
      {
        if ( _attributes.Contains( name ) )//容器自身会执行不区分大小写的查找
          throw new InvalidOperationException( string.Format( CultureInfo.InvariantCulture, "元素已经存在名为 \"{0}\" 的属性。", name ) );

        var attribute = new DomAttribute( this, name, value );
        _attributes.Add( attribute );

        return attribute;
      }
    }


    /// <summary>
    /// 元素属性容器
    /// </summary>
    protected class DomAttributeCollection : SynchronizedKeyedCollection<string, DomAttribute>
    {

      /// <summary>
      /// 创建 DomAttributeCollection 实例
      /// </summary>
      /// <param name="element">所属的元素对象</param>
      public DomAttributeCollection( DomElement element )
        : base( element.SyncRoot, StringComparer.OrdinalIgnoreCase )
      {
      }

      /// <summary>
      /// 获取元素属性的 Key ，即 AttributeName
      /// </summary>
      /// <param name="item">元素属性</param>
      /// <returns>属性的 Key ，即属性名</returns>
      protected override string GetKeyForItem( DomAttribute item )
      {
        return item.Name.ToLowerInvariant();
      }
    }



    private DomNodeCollection _nodeCollection;

    /// <summary>
    /// 获取子节点容器
    /// </summary>
    public DomNodeCollection NodeCollection
    {
      get
      {
        lock ( SyncRoot )
        {
          if ( _nodeCollection == null )
            _nodeCollection = new DomNodeCollection( this );

          return _nodeCollection;
        }
      }
    }

    /// <summary>
    /// 获取所有子节点
    /// </summary>
    /// <returns>所有子节点列表</returns>
    public IEnumerable<IHtmlNode> Nodes()
    {
      lock ( SyncRoot )
      {
        if ( _nodeCollection == null )
          _nodeCollection = new DomNodeCollection( this );

        return _nodeCollection.HtmlNodes;
      }
    }


    /// <summary>
    /// 移除这个元素
    /// </summary>
    public override void Remove()
    {
      this.ClearNodes();

      base.Remove();
    }

    internal void RemoveAttribute( DomAttribute attribute )
    {
      lock ( SyncRoot )
      {
        _attributes.Remove( attribute );
      }
    }


    private object _sync = new object();

    /// <summary>
    /// 获取用于同步的对象
    /// </summary>
    public object SyncRoot
    {
      get { return _sync; }
    }

  }

}
