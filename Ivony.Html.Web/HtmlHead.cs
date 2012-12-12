using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ivony.Fluent;

namespace Ivony.Html.Web
{
  public class HtmlHead
  {

    private IHtmlElement _element;

    public HtmlHead( IHtmlElement element )
    {

      if ( element == null )
        throw new ArgumentNullException( "element" );

      if ( !element.Name.EqualsIgnoreCase( "head" ) )
        throw new NotImplementedException();

      _element = element;
    }


    public IHtmlElement Element
    {
      get { return _element; }
    }

    public string Title
    {
      get
      {
        var titleElement = Element.Elements( "title" ).SingleOrDefault();
        if ( titleElement == null )
          return null;

        return titleElement.InnerText();
      }
      set
      {
        var titleElement = Element.Elements( "title" ).SingleOrDefault();

        if ( titleElement == null )
        {
          var factory = Element.Document.GetNodeFactory();
          titleElement = (IHtmlElement) factory.CreateElement( "title" ).InsertTo( Element, 0 );
        }

        titleElement.InnerText( value );
      }
    }

    /// <summary>
    /// 获取所有的 meta 元素
    /// </summary>
    /// <returns></returns>
    public IEnumerable<HtmlMeta> Metas()
    {
      return Element.Elements( "meta" ).Select( element => new HtmlMeta( element ) );
    }

    /// <summary>
    /// 获取具有指定名称的 meta 元素
    /// </summary>
    /// <param name="name">meta 名称</param>
    /// <returns>meta 元素</returns>
    public IEnumerable<HtmlMeta> Metas( string name )
    {
      return Element.Elements( "meta" )
        .Where( element => element.Attribute( "name" ).Value() == name )
        .Select( element => new HtmlMeta( element ) );
    }

    /// <summary>
    /// 添加一个 meta 元素
    /// </summary>
    /// <param name="name">名称</param>
    /// <param name="content">内容</param>
    /// <returns>meta 元素</returns>
    public HtmlMeta AppendMeta( string name, string content )
    {
      return HtmlMeta.Create( this, name, content );
    }


  }
}
