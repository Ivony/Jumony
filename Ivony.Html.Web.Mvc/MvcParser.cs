using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ivony.Html.Parser;
using Ivony.Fluent;

namespace Ivony.Html.Web.Mvc
{
  public class MvcParser : JumonyParser
  {

    protected override bool IsCDataElement( Parser.ContentModels.HtmlBeginTag tag )
    {

      if ( tag.TagName.EqualsIgnoreCase( "partial" ) )
        return true;
      
      return base.IsCDataElement( tag );
    }

  }
}
