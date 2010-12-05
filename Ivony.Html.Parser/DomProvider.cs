using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ivony.Html.Parser
{
  public class DomProvider : IHtmlDomProvider
  {
    public IHtmlDocument CreateDocument()
    {
      return new DomDocument();
    }


    private static IDomContainer EnsureDomContainer( IHtmlContainer container )
    {
      var domContainer = container as IDomContainer;

      if ( domContainer == null )
        throw new NotSupportedException( "只能向指定类型容器添加节点" );

      return domContainer;
    }


    private static T AddNode<T>( int index, IDomContainer domContainer, T element ) where T : DomNode
    {
      domContainer.NodeCollection.Insert( index, element );
      return element;
    }


    public IHtmlElement AddElement( IHtmlContainer container, int index, string name, IDictionary<string, string> attributes )
    {
      return AddNode( index, EnsureDomContainer( container ), new DomElement( name, attributes ) );
    }


    public IHtmlTextNode AddTextNode( IHtmlContainer container, int index, string htmlText )
    {
      return AddNode( index, EnsureDomContainer( container ), new DomTextNode( htmlText ) );
    }

    public IHtmlComment AddComment( IHtmlContainer container, int index, string comment )
    {
      return AddNode( index, EnsureDomContainer( container ), new DomComment( comment ) );
    }

    public IHtmlSpecial AddSpecial( IHtmlContainer container, int index, string html )
    {
      throw new NotImplementedException();
    }
  }
}
