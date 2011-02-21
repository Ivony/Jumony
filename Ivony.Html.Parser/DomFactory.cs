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


    public HtmlFragment ParseFragment( string html )
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


  public class DomFragment : IHtmlFragment, IDomContainer
  {

    public DomFragment( DomFactory factory )
    {
      _factory = factory;
      _nodeCollection = new DomNodeCollection( this );
    }

    private object _sync = new object();
    private DomFactory _factory;
    private DomNodeCollection _nodeCollection;

    public DomNodeCollection NodeCollection
    {
      get { return _nodeCollection; }
    }


    public void AddNode( int index, IFreeNode node )
    {
      if ( node == null )
        throw new ArgumentNullException( "node" );

      if ( !Document.Equals( node.Document ) )
        throw new ArgumentException( "不支持添加其他文档的游离节点", "node" );


      lock ( SyncRoot )
      {
        if ( index < 0 || index > _nodeCollection.Count )
          throw new ArgumentOutOfRangeException( "index" );
      }
    }




    public IHtmlNodeFactory Factory
    {
      get { return _factory; }
    }


    public IEnumerable<IHtmlNode> Nodes()
    {
      return _nodeCollection;
    }

    public object RawObject
    {
      get { return this; }
    }

    public string RawHtml
    {
      get { return null; }
    }

    public IHtmlDocument Document
    {
      get { return _factory.Document; }
    }

    public object SyncRoot
    {
      get { return _sync; }
    }
  }
}


