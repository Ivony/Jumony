using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ivony.Html
{
  public interface IFreeNode : IHtmlNode
  {

    IHtmlNode InsertTo( IHtmlContainer container, int index );
    IHtmlNodeFactory Factory { get; }

    //bool CanInsertTo( IHtmlContainer container );
  }


  public interface IFreeElement : IHtmlElement, IFreeNode { }
  public interface IFreeTextNode : IHtmlTextNode, IFreeNode { }
  public interface IFreeComment : IHtmlComment, IFreeNode { }
}
