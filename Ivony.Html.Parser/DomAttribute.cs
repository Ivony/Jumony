using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ivony.Fluent;

namespace Ivony.Html.Parser
{
  /// <summary>
  /// IHtmlAttribute 的实现
  /// </summary>
  public class DomAttribute : DomObject, IHtmlAttribute
  {

    private readonly DomElement _element;
    private readonly string _name;
    private string _value;

    private bool disposed = false;


    /// <summary>
    /// 创建 DomAttribute 实例
    /// </summary>
    /// <param name="element">所属元素</param>
    /// <param name="name">属性名</param>
    /// <param name="value">属性值</param>
    public DomAttribute( DomElement element, string name, string value )
    {
      _element = element;
      _name = name;
      _value = value;
    }

    /// <summary>
    /// 判定两个属性对象是否相同
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public override bool Equals( object obj )
    {

      var attribute = obj as IHtmlAttribute;

      if ( attribute == null )
        return false;

      if ( !attribute.Element.Equals( this.Element ) )
        return false;

      if ( attribute.Name.EqualsIgnoreCase( this.Name ) && attribute.AttributeValue == this.AttributeValue )
        return true;

      return base.Equals( obj );

    }

    /// <summary>
    /// 用作特定类型的哈希函数
    /// </summary>
    /// <returns>当前对象的哈希代码</returns>
    public override int GetHashCode()
    {
      return Element.GetHashCode() ^ Name.ToLowerInvariant().GetHashCode() ^ AttributeValue.GetHashCode();
    }


    /// <summary>
    /// 获取属性所属的元素
    /// </summary>
    public DomElement Element
    {
      get
      {
        if ( disposed )
          throw new ObjectDisposedException( "Attribute" );

        return _element;
      }
    }


    IHtmlElement IHtmlAttribute.Element
    {
      get { return _element; }
    }


    /// <summary>
    /// 获取属性名
    /// </summary>
    public string Name
    {
      get
      {
        if ( disposed )
          throw new ObjectDisposedException( "Attribute" );

        return _name;
      }
    }

    /// <summary>
    /// 获取属性值
    /// </summary>
    public string AttributeValue
    {
      get
      {
        if ( disposed )
          throw new ObjectDisposedException( "Attribute" );

        return _value;
      }
    }

    /// <summary>
    /// 尝试从元素中移除此属性
    /// </summary>
    public void Remove()
    {

      if ( disposed )
        throw new ObjectDisposedException( "Attribute" );

      lock ( Element.SyncRoot )
      {
        _element.RemoveAttribute( this );
        disposed = true;
      }
    }


    /// <summary>
    /// 获取 DOM 对象所属的文档
    /// </summary>
    public override IHtmlDocument Document
    {
      get { return Element.Document; }
    }
  }
}
