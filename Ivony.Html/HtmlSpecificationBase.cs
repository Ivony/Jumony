using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ivony.Html
{
  public abstract class HtmlSpecificationBase
  {

    public abstract bool IsCDataElement( string elementName );

    public abstract bool IsOptionalEndTag( string elementName );

    public abstract bool IsForbiddenEndTag( string elementName );

    public abstract bool ImmediatelyClose( string openTag, string nextTag );


  }
}
