using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ivony.Html;
using System.IO;
using Ivony.Html.Parser;
using Ivony.Html.Binding;
using System.Linq;
using Newtonsoft.Json.Linq;

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



      document.DataBind( JToken.Parse( "[1,2,3]" ) );

      Assert.AreEqual( document.Find( "div" ).Count(), 3 );

      Assert.AreEqual( document.Find( "div span" ).ElementAt( 0 ).InnerText(), "1" );
      Assert.AreEqual( document.Find( "div span" ).ElementAt( 1 ).InnerText(), "2" );
      Assert.AreEqual( document.Find( "div span" ).ElementAt( 2 ).InnerText(), "3" );



    }


    protected static IHtmlDocument LoadDocument( string filename )
    {
      return new JumonyParser().LoadDocument( Path.Combine( Environment.CurrentDirectory, filename ) );
    }



    [TestMethod]
    public void ListGrowth()
    {


      var document = LoadDocument( "ListTest2.html" );

      document.DataBind( new int[] { 1, 2, 3, 4, 5, 6 } );


      var container = document.FindFirst( "body > div" );

      Assert.IsTrue( container.Elements().ElementAt( 0 ).Class().Contains( "header" ), "列表增长绑定测试失败" );
      Assert.IsTrue( container.Elements().ElementAt( 1 ).Class().Contains( "item" ), "列表增长绑定测试失败" );
      Assert.IsTrue( container.Elements().ElementAt( 2 ).Class().Contains( "separator" ), "列表增长绑定测试失败" );
      Assert.IsTrue( container.Elements().ElementAt( 3 ).Class().Contains( "item" ), "列表增长绑定测试失败" );
      Assert.IsTrue( container.Elements().ElementAt( 4 ).Class().Contains( "separator" ), "列表增长绑定测试失败" );
      Assert.IsTrue( container.Elements().ElementAt( 5 ).Class().Contains( "item" ), "列表增长绑定测试失败" );
      Assert.IsTrue( container.Elements().ElementAt( 6 ).Class().Contains( "separator" ), "列表增长绑定测试失败" );
      Assert.IsTrue( container.Elements().ElementAt( 7 ).Class().Contains( "item" ), "列表增长绑定测试失败" );
      Assert.IsTrue( container.Elements().ElementAt( 8 ).Class().Contains( "separator" ), "列表增长绑定测试失败" );
      Assert.IsTrue( container.Elements().ElementAt( 9 ).Class().Contains( "item" ), "列表增长绑定测试失败" );
      Assert.IsTrue( container.Elements().ElementAt( 10 ).Class().Contains( "separator" ), "列表增长绑定测试失败" );
      Assert.IsTrue( container.Elements().ElementAt( 11 ).Class().Contains( "item" ), "列表增长绑定测试失败" );
      Assert.IsTrue( container.Elements().ElementAt( 12 ).Class().Contains( "footer" ), "列表增长绑定测试失败" );

      Assert.AreEqual( container.Elements( ".item" ).ElementAt( 0 ).InnerText(), "1", "列表增长绑定测试失败" );
      Assert.AreEqual( container.Elements( ".item" ).ElementAt( 1 ).InnerText(), "2", "列表增长绑定测试失败" );
      Assert.AreEqual( container.Elements( ".item" ).ElementAt( 2 ).InnerText(), "3", "列表增长绑定测试失败" );
      Assert.AreEqual( container.Elements( ".item" ).ElementAt( 3 ).InnerText(), "4", "列表增长绑定测试失败" );
      Assert.AreEqual( container.Elements( ".item" ).ElementAt( 4 ).InnerText(), "5", "列表增长绑定测试失败" );
      Assert.AreEqual( container.Elements( ".item" ).ElementAt( 5 ).InnerText(), "6", "列表增长绑定测试失败" );

      Assert.AreEqual( container.Elements( ".footer" ).First().InnerText(), "6", "高级列表绑定测试失败" );
    }


    [TestMethod]
    public void ListTruncate()
    {


      var document = LoadDocument( "ListTest3.html" );

      document.DataBind( new int[] { 1, 2, 3 } );


      var container = document.FindFirst( "body > div" );

      Assert.AreEqual( container.Elements().Count(), 5, "列表截断绑定测试失败" );

      Assert.IsTrue( container.Elements().ElementAt( 0 ).Class().Contains( "item" ), "列表截断绑定测试失败" );
      Assert.IsTrue( container.Elements().ElementAt( 1 ).Class().Contains( "separator" ), "列表截断绑定测试失败" );
      Assert.IsTrue( container.Elements().ElementAt( 2 ).Class().Contains( "item" ), "列表截断绑定测试失败" );
      Assert.IsTrue( container.Elements().ElementAt( 3 ).Class().Contains( "separator" ), "列表截断绑定测试失败" );
      Assert.IsTrue( container.Elements().ElementAt( 4 ).Class().Contains( "item" ), "列表截断绑定测试失败" );

      Assert.AreEqual( container.Elements( ".item" ).ElementAt( 0 ).InnerText(), "1", "列表截断绑定测试失败" );
      Assert.AreEqual( container.Elements( ".item" ).ElementAt( 1 ).InnerText(), "2", "列表截断绑定测试失败" );
      Assert.AreEqual( container.Elements( ".item" ).ElementAt( 2 ).InnerText(), "3", "列表截断绑定测试失败" );

    }

  }
}
