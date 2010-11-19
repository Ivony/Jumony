using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace Ivony.Html
{
  public interface IFreeNode : IHtmlNode
  {
    [EditorBrowsable( EditorBrowsableState.Never )]
    IHtmlNode Into( IHtmlNodeContainer container, int index );
    IHtmlNodeFactory Factory { get; }

    //bool CanInsertTo( IHtmlContainer container );
  }


  public interface IFreeElement : IHtmlElement, IFreeNode { }
  public interface IFreeTextNode : IHtmlTextNode, IFreeNode { }
  public interface IFreeComment : IHtmlComment, IFreeNode { }
}
