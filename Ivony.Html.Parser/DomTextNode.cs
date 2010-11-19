using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Ivony.Html.Parser
{
  public class DomTextNode : DomNode, IHtmlTextNode
  {

    private readonly string raw;

    public DomTextNode( DomContainer parent, string rawHtml )
      : base( parent )
    {
      raw = rawHtml;
    }


    protected override string ObjectName
    {
      get { return "TextNode"; }
    }



    public string HtmlText
    {
      get
      {
        var element = DomContainer as IHtmlElement;
        if ( element != null )
        {
          if ( HtmlSpecification.cdataTags.Contains( element.Name, StringComparer.InvariantCultureIgnoreCase ) )
            return raw;

          else
            return HtmlEncoding.HtmlEncode( HtmlEncoding.HtmlDecode( raw ) );
        }

        return raw;
      }
    }


    public override string RawHtml
    {
      get
      {
        CheckDisposed();

        return raw;
      }
    }

  }



  internal class DomFreeTextNode : HtmlNodeWrapper, IFreeTextNode
  {

    private DomFactory _factory;
    private DomTextNode textNode;

    private bool disposed = false;
    private void CheckDisposed()
    {
      if ( disposed )
        throw new ObjectDisposedException( "FreeTextNode" );
    }


    public DomFreeTextNode( DomFactory factory, string html )
    {
      _factory = factory;
      textNode = new DomTextNode( null, html );
    }



    IHtmlDocument IHtmlObject.Document
    {
      get
      {
        CheckDisposed();

        return _factory.Document;
      }
    }


    #region IHtmlTextNode 成员

    public string HtmlText
    {
      get
      {
        CheckDisposed();

        return textNode.HtmlText;
      }
    }

    #endregion

    #region IFreeNode 成员

    public IHtmlNode Into( IHtmlNodeContainer container, int index )
    {
      CheckDisposed();

      if ( container == null )
        throw new ArgumentNullException( "container" );

      var domContainer = container as DomContainer;
      if ( domContainer == null )
        throw new InvalidOperationException();

      domContainer.InsertNode( index, textNode );

      disposed = true;
      return textNode;
    }

    public IHtmlNodeFactory Factory
    {
      get
      {
        CheckDisposed();

        return _factory;
      }
    }

    #endregion

    protected override IHtmlNode Node
    {
      get
      {
        CheckDisposed();

        return textNode;
      }
    }
  }

}
