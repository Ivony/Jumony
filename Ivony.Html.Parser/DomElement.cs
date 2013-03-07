using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using Ivony.Fluent;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Ivony.Html.Parser
{
  /// <summary>
  /// IHtmlElement 的实现
  /// </summary>
  public class DomElement : DomNode, IHtmlElement, IDomContainer
  {

    private readonly string _name;
    private readonly DomAttributeCollection _attributes;


    private static readonly Regex tagNameRegex = new Regulars.TagName();
    private static readonly Regex attributeNameRegex = new Regulars.AttributeName();



    /// <summary>
    /// 创建 DomElement 实例
    /// </summary>
    /// <param name="name">元素名</param>
    /// <param name="attributes">属性</param>
    public DomElement( string name, IDictionary<string, string> attributes ) : this( name, attributes, true ) { }


    /// <summary>
    /// 创建 DomElement 实例
    /// </summary>
    /// <param name="name">元素名</param>
    /// <param name="attributes">属性</param>
    /// <param name="argumentCheck">是否进行参数检查</param>
    internal DomElement( string name, IDictionary<string, string> attributes, bool argumentCheck )
    {
      if ( argumentCheck )
      {
        if ( name == null )
          throw new ArgumentNullException( "name" );

        if ( !tagNameRegex.IsMatch( name ) )
          throw new FormatException( "元素名称格式不正确" );
      }


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

        /*
        if ( !attributeNameRegex.IsMatch( name ) )
          throw new FormatException( "属性名称格式不正确" );
        */

        var attribute = new DomAttribute( this, name, value );
        _attributes.Add( attribute );

        return attribute;
      }
    }


    /// <summary>
    /// 元素属性容器
    /// </summary>
    protected class DomAttributeCollection : SynchronizedCollection<DomAttribute>, IHtmlAttributeCollection
    {

      /// <summary>
      /// 创建 DomAttributeCollection 实例
      /// </summary>
      /// <param name="element">所属的元素对象</param>
      public DomAttributeCollection( DomElement element )
        : base( element.SyncRoot )
      {
      }

      private Dictionary<string, DomAttribute> dictionary = new Dictionary<string, DomAttribute>( StringComparer.OrdinalIgnoreCase );


      /// <summary>
      /// 重写 InsertItem 方法，将属性加入索引
      /// </summary>
      /// <param name="index">插入位置</param>
      /// <param name="item">插入项</param>
      protected override void InsertItem( int index, DomAttribute item )
      {
        dictionary.Add( GetKeyForItem( item ), item );
        base.InsertItem( index, item );
      }

      /// <summary>
      /// 重写 SetItem 方法，将属性加入索引
      /// </summary>
      /// <param name="index">插入位置</param>
      /// <param name="item">插入项</param>
      protected override void SetItem( int index, DomAttribute item )
      {
        var key1 = GetKeyForItem( Items[index] );
        var key2 = GetKeyForItem( item );

        if ( dictionary.Comparer.Equals( key1, key2 ) && key1 != null )
          dictionary[key1] = item;


        else
        {
          dictionary.Remove( key1 );
          dictionary.Add( key2, item );
        }

        base.SetItem( index, item );
      }

      /// <summary>
      /// 重写 ClearItems 方法，清空索引
      /// </summary>
      protected override void ClearItems()
      {
        dictionary.Clear();
        base.ClearItems();
      }

      /// <summary>
      /// 重写 RemoveItem 方法，将属性移除索引
      /// </summary>
      /// <param name="index">要移除的属性的位置</param>
      protected override void RemoveItem( int index )
      {
        var key = this.GetKeyForItem( base.Items[index] );
        if ( key != null )
          dictionary.Remove( key );

        base.RemoveItem( index );
      }



      /// <summary>
      /// 获取元素属性的 Key ，即 AttributeName
      /// </summary>
      /// <param name="item">元素属性</param>
      /// <returns>属性的 Key ，即属性名</returns>
      protected virtual string GetKeyForItem( DomAttribute item )
      {
        return item.Name;
      }

      /// <summary>
      /// 检查指定名称的属性是否存在
      /// </summary>
      /// <param name="attributeName"></param>
      /// <returns></returns>
      public bool Contains( string attributeName )
      {
        return dictionary.ContainsKey( attributeName );
      }


      IHtmlAttribute IHtmlAttributeCollection.Get( string name )
      {
        lock ( SyncRoot )
        {
          DomAttribute value;
          if ( dictionary.TryGetValue( name, out value ) )
            return value;

          else
            return null;
        }
      }


      IEnumerator<IHtmlAttribute> IEnumerable<IHtmlAttribute>.GetEnumerator()
      {
        return Items.Cast<IHtmlAttribute>().GetEnumerator();
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

    /// <summary>
    /// 移除一个属性
    /// </summary>
    /// <param name="attribute"></param>
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
