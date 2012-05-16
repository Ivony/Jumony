using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ivony.Fluent;


namespace Ivony.Html.Indexing
{

  /// <summary>
  /// 元素名称的索引
  /// </summary>
  public class ElementNameIndex : ElementIndex
  {


    /// <summary>
    /// 创建元素名称的索引
    /// </summary>
    /// <param name="manager">文档索引管理器</param>
    public ElementNameIndex( IndexManager manager ) : base( manager ) { }


    private IDictionary<string,List<IHtmlElement>> data;


    /// <summary>
    /// 初始化索引数据
    /// </summary>
    protected override void InitializeData()
    {
      data = new Dictionary<string, List<IHtmlElement>>();
    }



    /// <summary>
    /// 向索引中添加一个元素
    /// </summary>
    /// <param name="element">要添加的元素</param>
    protected override void OnAddElement( IHtmlElement element )
    {
      var name = element.Name;

      AddElement( name, element );
    }


    /// <summary>
    /// 向索引中添加一个元素
    /// </summary>
    /// <param name="name">元素名</param>
    /// <param name="element">元素对象</param>
    private void AddElement( string name, IHtmlElement element )
    {
      var set = data[name] as List<IHtmlElement>;
      if ( set == null )
        set = data[name] = new List<IHtmlElement>();

      set.Add( element );
    }


    /// <summary>
    /// 从索引中移除一个元素
    /// </summary>
    /// <param name="element">要移除的元素</param>
    protected override void OnRemoveElement( IHtmlElement element )
    {
      var set = data[element.Name] as List<IHtmlElement>;
      set.Remove( element );
    }




    protected override void OnAddAttribute( IHtmlElement element, IHtmlAttribute attribute )
    {
    }

    protected override void OnRemoveAttribute( IHtmlElement element, IHtmlAttribute attribute )
    {
    }
  }
}
