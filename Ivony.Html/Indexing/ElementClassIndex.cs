using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Ivony.Fluent;
using System.Text.RegularExpressions;
using System.Collections;

namespace Ivony.Html.Indexing
{

  /// <summary>
  /// 样式类索引
  /// </summary>
  public class ElementClassIndex : ElementIndex
  {


    private object  _sync = new object();


    /// <summary>
    /// 构建索引实例
    /// </summary>
    /// <param name="manager">文档索引管理器</param>
    public ElementClassIndex( IndexManager manager ) : base( manager ) { }

    private Hashtable data;



    protected override void InitializeData()
    {
      data = Hashtable.Synchronized( new Hashtable() );
    }


    /// <summary>
    /// 新增一个元素到索引
    /// </summary>
    /// <param name="element">新增到索引的元素</param>
    protected override void OnAddElement( IHtmlElement element )
    {
      var classes = element.Attribute( "class" ).Value();

      if ( !string.IsNullOrEmpty( classes ) )
        Regulars.whiteSpaceSeparatorRegex.Split( classes ).ForAll( c => AddElement( c, element ) );
    }



    private void AddElement( string className, IHtmlElement element )
    {
      lock ( data.SyncRoot )
      {
        var set = data[className] as List<IHtmlElement>;
        if ( set == null )
          data[className] = set = new List<IHtmlElement>();

        set.Add( element );
      }
    }




    /// <summary>
    /// 从索引中移除一个元素
    /// </summary>
    /// <param name="element"></param>
    protected override void OnRemoveElement( IHtmlElement element )
    {
      RemoveElement( element, element.Attribute( "class" ) );
    }

    private void RemoveElement( IHtmlElement element, IHtmlAttribute attribute )
    {
      var classes = attribute.Value();
      if ( !string.IsNullOrEmpty( classes ) )
        Regulars.whiteSpaceSeparatorRegex.Split( classes ).ForAll( c => RemoveElement( c, element ) );
    }

    /// <summary>
    /// 从索引中移除一个元素
    /// </summary>
    /// <param name="className"></param>
    /// <param name="element"></param>
    protected void RemoveElement( string className, IHtmlElement element )
    {
      var set = data[className] as List<IHtmlElement>;
      set.Remove( element );
    }


    /// <summary>
    /// 当元素被添加属性
    /// </summary>
    /// <param name="element"></param>
    /// <param name="attribute"></param>
    protected override void OnAddAttribute( IHtmlElement element, IHtmlAttribute attribute )
    {
      if ( !attribute.Name.EqualsIgnoreCase( "class" ) )
        return;

      OnAddElement( element );
    }


    /// <summary>
    /// 当元素被移除属性
    /// </summary>
    /// <param name="element"></param>
    /// <param name="attribute"></param>
    protected override void OnRemoveAttribute( IHtmlElement element, IHtmlAttribute attribute )
    {
      if ( !attribute.Name.EqualsIgnoreCase( "class" ) )
        return;

      RemoveElement( element, attribute );
    }


    /// <summary>
    /// 查找具备指定样式类的元素
    /// </summary>
    /// <param name="className">样式类名</param>
    /// <returns>具有此样式类的所有元素</returns>
    public IEnumerable<IHtmlElement> this[string className]
    {
      get
      {
        var set = data[className] as IList<IHtmlElement>;

        if ( set.IsNullOrEmpty() )
          return Enumerable.Empty<IHtmlElement>();

        return set.AsReadOnly();
      }
    }


    /// <summary>
    /// 文档所有的样式类名称
    /// </summary>
    public IEnumerable<string> ClassNames
    {
      get { return data.Keys.Cast<string>(); }
    }


  }
}
