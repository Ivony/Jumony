using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ivony.Fluent;

namespace Ivony.Html.Indexing
{
  public class IndexManager
  {

    public IndexManager( IHtmlContainer container )
    {
      Container = container;

    }

    /// <summary>
    /// 索引管理器所管理的容器
    /// </summary>
    public IHtmlContainer Container
    {
      get;
      private set;
    }


    private List<ElementIndex> _indexes = new List<ElementIndex>();

    public IEnumerable<ElementIndex> ElementIndexes
    {
      get { return _indexes; }
    }


    protected void Rebuild()
    {
      InitializeIndexes();


      Container.Descendants()
        .ForAll( element =>
          {
            ElementIndexes.ForAll( index => index.AddElement( element ) );
            _descendants.Add( element );
          }
        );

      foreach ( var index in ElementIndexes )
      {

      }
    }



    private HashSet<IHtmlElement> _descendants = new HashSet<IHtmlElement>();

    protected ICollection<IHtmlElement> DescendantElements
    {
      get { return _descendants; }
    }


    public bool InScope( IHtmlNode node )
    {

      if ( node == null )
        throw new ArgumentNullException( "node" );

      if ( node.Container.Equals( Container ) )
        return true;

      var element = node.Container as IHtmlElement;
      if ( element == null )
        return false;

      return DescendantElements.Contains( element );
    }

    private void InitializeIndexes()
    {
      throw new NotImplementedException();
    }


  }
}
