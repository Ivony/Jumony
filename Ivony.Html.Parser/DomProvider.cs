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
      return new DomDocument( null );
    }

    public IHtmlDocument CreateDocument( Uri url )
    {
      return new DomDocument( url );
    }


    internal static IDomContainer EnsureDomContainer( IHtmlContainer container )
    {
      var domContainer = container as IDomContainer;

      if ( domContainer == null )
        throw new NotSupportedException( "只能向指定类型容器添加节点" );

      return domContainer;
    }


    private static DomProvider _instance = new DomProvider();

    public static DomProvider Instance
    {
      get
      {
        return _instance;
      }
    }



    public IHtmlElement AddElement( IHtmlContainer container, string name, IDictionary<string, string> attributes )
    {
      return EnsureDomContainer( container ).AddNode( new DomElement( name, attributes ) );
    }

    public IHtmlTextNode AddTextNode( IHtmlContainer container, string htmlText )
    {
      return EnsureDomContainer( container ).AddNode( new DomTextNode( htmlText ) );
    }

    public IHtmlComment AddComment( IHtmlContainer container, string comment )
    {
      return EnsureDomContainer( container ).AddNode( new DomComment( comment ) );
    }

    public IHtmlSpecial AddSpecial( IHtmlContainer container, string html )
    {
      return EnsureDomContainer( container ).AddNode( new DomSpecial( html ) );
    }

  }
}
