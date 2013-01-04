using Ivony.Html;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ivony.Html.Parser;
using System.Diagnostics;

namespace TestConsole
{
  class Program
  {
    static void Main( string[] args )
    {



      var document = new JumonyParser().LoadDocument( "http://www.sina.com.cn/", Encoding.GetEncoding( "GB2312" ) );

      Stopwatch watch = new Stopwatch();
      watch.Start();

      for ( int i = 0; i < 200; i++ )
      {

        var elements = document.Descendants().ToArray();


        CssParser.ParseSelector( "body p a" ).Filter( elements ).FirstOrDefault();
        CssParser.ParseSelector( "p > a" ).Filter( elements ).FirstOrDefault();
        CssParser.ParseSelector( "p[class] a" ).Filter( elements ).FirstOrDefault();
        CssParser.ParseSelector( "p a[href]" ).Filter( elements ).FirstOrDefault();
        CssParser.ParseSelector( "p + a" ).Filter( elements ).FirstOrDefault();
        CssParser.ParseSelector( "div a" ).Filter( elements ).FirstOrDefault();
        CssParser.ParseSelector( "p div a" ).Filter( elements ).FirstOrDefault();
        CssParser.ParseSelector( "a img[src]" ).Filter( elements ).FirstOrDefault();
        CssParser.ParseSelector( "div img" ).Filter( elements ).FirstOrDefault();
        CssParser.ParseSelector( "body img[src]" ).Filter( elements ).FirstOrDefault();
      }
      watch.Stop();

      Console.WriteLine( watch.Elapsed );
      Console.ReadKey();


    }
  }
}
