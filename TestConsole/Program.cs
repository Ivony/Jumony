using Ivony.Html;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestConsole
{
  class Program
  {
    static void Main( string[] args )
    {

      var selector = CssParser.ParseSelector( "a.class[href]" );
      Console.WriteLine( selector );

      selector = CssParser.ParseSelector( "a.class[href] p:nth-child( 3n - 5 ) , span >  li#pitem.kk+.k[href^=http:// ]   + \t:nth-child(3) h1[class!=\"abc'\"] a[dsh=!1j ]" );
      Console.WriteLine( selector );

      Console.ReadKey();
    }



  }
}
