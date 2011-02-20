using System;
using System.Collections.Generic;


namespace Ivony.Html
{
  public interface IHtmlFragment : IHtmlContainer
  {

    HtmlFragment AddNode( int index, IFreeNode node );

    IEnumerable<IFreeNode> Nodes();

    IHtmlNodeFactory Factory
    {
      get;
    }

  }
}
