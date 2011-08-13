using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ivony.Html;
using Ivony.Fluent;
using Ivony.Html.Parser.ContentModels;

namespace Ivony.Html.Parser
{


  public class DomFragment : DomObject, IHtmlFragment, IDomContainer
  {



    public DomFragment( DomFragmentManager manager )
    {
      _manager = manager;
      _nodeCollection = new DomNodeCollection( this );
    }

    public DomFragment( DomFragmentManager manager, string html )
      : this( manager )
    {

      var parser = new FragmentParser();

      parser.ProcessFragment( html, this );

    }



    protected class FragmentParser : JumonyParser
    {
      private DomProvider _provider = new DomProvider();

      protected override IHtmlReader CreateReader( string html )
      {
        return new JumonyReader( html );
      }

      protected override IHtmlDomProvider Provider
      {
        get
        {
          return _provider;
        }
      }


      public override IHtmlDocument Parse( string html, Uri url )
      {
        throw new NotSupportedException();
      }

      public virtual void ProcessFragment( string html, DomFragment fragment )
      {

        if ( string.IsNullOrEmpty( html ) )
          return;

        lock ( SyncRoot )
        {
          InitializeStack();

          ContainerStack.Push( fragment );

          ParseInternal( html );

          fragment.ContentFragment = new HtmlContentFragment( Reader, 0, Reader.HtmlText.Length );
        }
      }


    }

    private object _sync = new object();
    private DomFragmentManager _manager;
    private DomNodeCollection _nodeCollection;

    public DomNodeCollection NodeCollection
    {
      get { return _nodeCollection; }
    }


    public IEnumerable<IHtmlNode> Nodes()
    {
      return _nodeCollection.HtmlNodes;
    }

    public override IHtmlDocument Document
    {
      get { return _manager.Document; }
    }

    public object SyncRoot
    {
      get { return _sync; }
    }






    public IEnumerable<IHtmlNode> Into( IHtmlContainer container, int index )
    {

      if ( container == null )
        throw new ArgumentNullException( "container" );

      if ( !object.Equals( container.Document, Document ) )
        throw new InvalidOperationException();

      var domContainer = container as IDomContainer;
      if ( domContainer == null )
        throw new InvalidOperationException();

      lock ( SyncRoot )
      {

        _manager.Allocated( this );


        var nodeList = NodeCollection.ToArray();

        lock ( container.SyncRoot )
        {
          foreach ( var node in nodeList.Reverse() )
          {
            node.Container = null;
            NodeCollection.Remove( node );

            domContainer.NodeCollection.Insert( index, node );
          }
        }


        return nodeList.Cast<IHtmlNode>();

      }
    }
  }




}


