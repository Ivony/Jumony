using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using Ivony.Fluent;

namespace Ivony.Html.Selectors
{
  public class CssSelectorIndex
  {

    private IHtmlDocument _document;

    internal CssSelectorIndex( IHtmlDocument document )
    {
      _document = document;
    }



    public CssSelectorIdentityIndex IdentityIndex
    {
      get;
      private set;
    }


    public void Rebuild( bool indexIdentity, bool indexClass, bool indexElementName, bool indexAttribute )
    {

      List<IElementIndexer> indexers = new List<IElementIndexer>();

      if ( indexIdentity )
      {
        IdentityIndex = new CssSelectorIdentityIndex( this );
        indexers.Add( IdentityIndex );
      }

      Rebuild( indexers );
    }

    private void Rebuild( List<IElementIndexer> indexers )
    {
      _document.Descendants().ForAll( element => indexers.ForAll( indexer => indexer.IndexElement( element ) ) );
    }


  }






#if false

  /// <summary>
  /// 文档 class 样式的索引
  /// </summary>
  public class CssSelectorClassIndex : IDictionary<string, IEnumerable<IHtmlElement>>, IElementIndexer
  {

  }

#endif
}
