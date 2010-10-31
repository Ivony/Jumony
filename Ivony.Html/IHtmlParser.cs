using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ivony.Html
{

  public class ParserAttribute : Attribute
  {


  }

  public interface IHtmlParser
  {

    IHtmlDocument ParseHtml( string html );

  }
}
