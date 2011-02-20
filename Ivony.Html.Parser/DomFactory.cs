using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ivony.Html;

namespace Ivony.Html.Parser
{
  public class DomFactory : IHtmlNodeFactory
  {


    internal DomFactory( DomDocument document )
    {
      Document = document;
    }


    internal DomDocument Document
    {
      get;
      private set;
    }



    public HtmlFragment CreateFragment()
    {
      return new DomFragment( this );
    }


    public IFreeElement CreateElement( string name )
    {
      if ( string.IsNullOrEmpty( name ) )//TODO 检查name的合法性
        throw new ArgumentNullException( "name" );

      return new DomFreeElement( this, name );
    }

    public IFreeTextNode CreateTextNode( string htmlText )
    {
      if ( string.IsNullOrEmpty( htmlText ) )
        throw new ArgumentNullException( "htmlText" );

      return new DomFreeTextNode( this, htmlText );
    }

    public IFreeComment CreateComment( string comment )
    {
      return new DomFreeComment( this, comment );
    }


    public HtmlFragment ParseHtml( string html )
    {
      if ( html == null )
        throw new ArgumentNullException( "html" );

      return this.MakeFragment( new JumonyParser().Parse( html, null ) );
    }


    IHtmlDocument IHtmlNodeFactory.Document
    {
      get { return Document; }
    }


  }

  public class DomFragment : HtmlFragment, IDomContainer
  {

    public DomFragment( DomFactory factory )
      : base( factory )
    {
      _nodeCollection = new DomNodeCollection( this );
    }


    private DomNodeCollection _nodeCollection;


    public DomNodeCollection NodeCollection
    {
      get { return _nodeCollection; }
    }
  }

}


