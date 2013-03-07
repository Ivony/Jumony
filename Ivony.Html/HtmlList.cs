using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ivony.Fluent;

namespace Ivony.Html
{
  public class HtmlList
  {

    private readonly IHtmlElement _element;

    public HtmlList( IHtmlElement element )
    {

      if ( element == null )
        throw new ArgumentNullException( "element" );

      if ( !HtmlSpecification.listElements.Contains( element.Name.ToLowerInvariant() ) )
        throw new ArgumentException( "只能从列表元素创建 HtmlList 对象" );

      if ( element.Elements().Any( e => !e.Name.EqualsIgnoreCase( "li" ) ) )
        throw new FormatException( "HTML 文档格式不正确，列表元素只能包含 li 元素" );

      if ( element.Nodes().OfType<IHtmlTextNode>().Any( n => !n.IsWhiteSpace() ) )
        throw new FormatException( "HTML 文档格式不正确，列表元素只能包含 li 元素，不能包含文本。" );

      _element = element;

    }

    public IHtmlElement Element
    {
      get { return _element; }
    }


    public IEnumerable<HtmlListItem> Items
    {
      get
      {
        return _element.Elements().Select( e => new HtmlListItem( e ) );
      }
    }

  }

  public class HtmlListItem
  {

    private readonly IHtmlElement _element;


    public IHtmlElement Element
    {
      get { return _element; }
    }


    internal HtmlListItem( IHtmlElement element )
    {
      if ( !element.Name.EqualsIgnoreCase( "li" ) )
        throw new FormatException( "HTML 文档格式不正确，列表元素只能包含 li 元素" );

      _element = element;
    }


  }
}
