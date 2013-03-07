using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ivony.Html
{
  /// <summary>
  /// 针对 DomModifier 对象的扩展方法
  /// </summary>
  public static class DomModifierExtensions
  {

    /// <summary>
    /// 在容器末尾增加一个元素
    /// </summary>
    /// <param name="modifier">DOM 结构修改器</param>
    /// <param name="container">要添加元素的容器</param>
    /// <param name="elementName">元素名</param>
    /// <returns>添加的元素</returns>
    public static IHtmlElement AddElement( this IHtmlDomModifier modifier, IHtmlContainer container, string elementName )
    {
      if ( modifier == null )
        throw new ArgumentNullException( "modifier" );

      if ( container == null )
        throw new ArgumentNullException( "container" );

      if ( elementName == null )
        throw new ArgumentNullException( "elementName" );

      lock ( container.SyncRoot )
      {
        return modifier.AddElement( container, container.Nodes().Count(), elementName );
      }
    }



    /// <summary>
    /// 在容器末尾增加一个文本节点
    /// </summary>
    /// <param name="modifier">DOM 结构修改器</param>
    /// <param name="container">要添加节点的容器</param>
    /// <param name="htmlText">HTML文本</param>
    /// <returns>添加的文本节点</returns>
    public static IHtmlTextNode AddTextNode( this IHtmlDomModifier modifier, IHtmlContainer container, string htmlText )
    {
      if ( modifier == null )
        throw new ArgumentNullException( "modifier" );

      if ( container == null )
        throw new ArgumentNullException( "container" );

      if ( htmlText == null )
        throw new ArgumentNullException( "htmlText" );

      lock ( container.SyncRoot )
      {
        return modifier.AddTextNode( container, container.Nodes().Count(), htmlText );
      }
    }



    /// <summary>
    /// 在容器末尾增加一个注释节点
    /// </summary>
    /// <param name="modifier">DOM 结构修改器</param>
    /// <param name="container">要添加注释的容器</param>
    /// <param name="comment">HTML注释</param>
    /// <returns>添加的注释节点</returns>
    public static IHtmlComment AddComment( this IHtmlDomModifier modifier, IHtmlContainer container, string comment )
    {
      if ( modifier == null )
        throw new ArgumentNullException( "modifier" );

      if ( container == null )
        throw new ArgumentNullException( "container" );

      if ( comment == null )
        throw new ArgumentNullException( "comment" );

      lock ( container.SyncRoot )
      {
        return modifier.AddComment( container, container.Nodes().Count(), comment );
      }
    }


    /// <summary>
    /// 创建指定节点的副本
    /// </summary>
    /// <param name="manager">HTML 碎片管理器，用来创建碎片</param>
    /// <param name="node">要创建副本的节点</param>
    /// <returns>节点的游离副本（ HTML 碎片形式）</returns>
    public static IHtmlFragment MakeCopy( this IHtmlFragmentManager manager, IHtmlNode node )
    {
      if ( manager == null )
        throw new ArgumentNullException( "manager" );

      if ( node == null )
        throw new ArgumentNullException( "node" );


      var fragment = manager.CreateFragment();

      fragment.AddCopy( node );

      return fragment;
    }


  }
}
