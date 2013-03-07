using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ivony.Html.Parser
{
  public interface IDomFragmentParserProvider
  {

    IDomFragmentParser GetFragmentParser( DomDocument document );

  }

  public interface IDomFragmentParser
  {
    void ParseToFragment( string html, DomFragment fragment );
  }
}
