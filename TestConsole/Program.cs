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



      var document = new JumonyParser().LoadDocument( "http://www.sina.com.cn/", Encoding.UTF8 );

      Stopwatch watch = new Stopwatch();
      watch.Restart();
      for ( int i = 0; i < 200; i++ )
      {

        var elements = document.Descendants().ToArray();

        document.Descendants().FilterBy( "body p a" ).FirstOrDefault();
        document.Descendants().FilterBy( "p > a" ).FirstOrDefault();
        document.Descendants().FilterBy( "p[class] a" ).FirstOrDefault();
        document.Descendants().FilterBy( "p a[href]" ).FirstOrDefault();
        document.Descendants().FilterBy( "p + a" ).FirstOrDefault();
        document.Descendants().FilterBy( "div a" ).FirstOrDefault();
        document.Descendants().FilterBy( "p div a" ).FirstOrDefault();
        document.Descendants().FilterBy( "a img[src]" ).FirstOrDefault();
        document.Descendants().FilterBy( "div img" ).FirstOrDefault();
        document.Descendants().FilterBy( "body img[src]" ).FirstOrDefault();
      }
      watch.Stop();

      Console.WriteLine( watch.Elapsed );

      watch.Restart();
      for ( int i = 0; i < 200; i++ )
      {

        var elements = document.Descendants().ToArray();

        document.Descendants().ToArray().FilterBy( "body p a" ).FirstOrDefault();
        document.Descendants().ToArray().FilterBy( "p > a" ).FirstOrDefault();
        document.Descendants().ToArray().FilterBy( "p[class] a" ).FirstOrDefault();
        document.Descendants().ToArray().FilterBy( "p a[href]" ).FirstOrDefault();
        document.Descendants().ToArray().FilterBy( "p + a" ).FirstOrDefault();
        document.Descendants().ToArray().FilterBy( "div a" ).FirstOrDefault();
        document.Descendants().ToArray().FilterBy( "p div a" ).FirstOrDefault();
        document.Descendants().ToArray().FilterBy( "a img[src]" ).FirstOrDefault();
        document.Descendants().ToArray().FilterBy( "div img" ).FirstOrDefault();
        document.Descendants().ToArray().FilterBy( "body img[src]" ).FirstOrDefault();
      }
      watch.Stop();
      Console.WriteLine( watch.Elapsed );
      watch.Restart();
      for ( int i = 0; i < 200; i++ )
      {

        var elements = document.Descendants().ToArray();

        document.Find( "body p a" ).FirstOrDefault();
        document.Find( "p > a" ).FirstOrDefault();
        document.Find( "p[class] a" ).FirstOrDefault();
        document.Find( "p a[href]" ).FirstOrDefault();
        document.Find( "p + a" ).FirstOrDefault();
        document.Find( "div a" ).FirstOrDefault();
        document.Find( "p div a" ).FirstOrDefault();
        document.Find( "a img[src]" ).FirstOrDefault();
        document.Find( "div img" ).FirstOrDefault();
        document.Find( "body img[src]" ).FirstOrDefault();
      }
      watch.Stop();
      Console.WriteLine( watch.Elapsed );



      Console.ReadKey();


    }
  }
}
