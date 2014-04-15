using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ivony.Html;
using System.IO;
using Ivony.Html.Parser;
using Ivony.Html.Binding;
using System.Linq;

namespace BindingTest
{
  [TestClass]
  public class ListTest
  {
    [TestMethod]
    public void SimpleList()
    {

      var document = LoadDocument( "ListTest1.html" );

      document.DataBind( new int[] { 1, 2, 3 } );

      Assert.AreEqual( document.Find( "div" ).Count(), 3 );

      Assert.AreEqual( document.Find( "div span" ).ElementAt( 0 ).InnerText(), "1" );
      Assert.AreEqual( document.Find( "div span" ).ElementAt( 1 ).InnerText(), "2" );
      Assert.AreEqual( document.Find( "div span" ).ElementAt( 2 ).InnerText(), "3" );

    }


    protected static IHtmlDocument LoadDocument( string filename )
    {
      return new JumonyParser().LoadDocument( Path.Combine( Environment.CurrentDirectory, filename ) );
    }
  }
}
