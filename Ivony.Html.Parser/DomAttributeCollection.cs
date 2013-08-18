using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ivony.Html.Parser
{
  /// <summary>
  /// 元素属性容器
  /// </summary>
  public sealed class DomAttributeCollection : SynchronizedCollection<DomAttribute>, IHtmlAttributeCollection
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
    private string GetKeyForItem( DomAttribute item )
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
}
