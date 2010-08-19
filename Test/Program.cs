using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using HtmlAgilityPack;
using Ivony.Web.Html.HtmlAgilityPackAdaptor;
using Ivony.Web.Html;
using Ivony.Web;

namespace Test
{
  public class Program
  {
    public static void Main( string[] args )
    {

      "123".TryParseTo<Test>();

    }

    private static string LoadHtml()
    {
      WebClient client = new WebClient();

      return client.DownloadString( "http://www.163.com/" );
    }

    public class Test
    {
      public static bool TryParse( string str, out Test value )
      {
        value = new Test();
        return false;
      }
    }

  }
}
