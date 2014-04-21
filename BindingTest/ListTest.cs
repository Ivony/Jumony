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



    [TestMethod]
    public void AdvancedList()
    {


      var document = LoadDocument( "ListTest2.html" );

      document.DataBind( new int[] { 1, 2, 3 } );


      var container = document.FindFirst( "body > div" );

      Assert.IsTrue( container.Elements().ElementAt( 0 ).Class().Contains( "header" ), "高级列表绑定测试失败" );
      Assert.IsTrue( container.Elements().ElementAt( 1 ).Class().Contains( "item" ), "高级列表绑定测试失败" );
      Assert.IsTrue( container.Elements().ElementAt( 2 ).Class().Contains( "separator" ), "高级列表绑定测试失败" );
      Assert.IsTrue( container.Elements().ElementAt( 3 ).Class().Contains( "item" ), "高级列表绑定测试失败" );
      Assert.IsTrue( container.Elements().ElementAt( 4 ).Class().Contains( "separator" ), "高级列表绑定测试失败" );
      Assert.IsTrue( container.Elements().ElementAt( 5 ).Class().Contains( "item" ), "高级列表绑定测试失败" );
      Assert.IsTrue( container.Elements().ElementAt( 6 ).Class().Contains( "footer" ), "高级列表绑定测试失败" );

      Assert.AreEqual( container.Elements( ".item" ).ElementAt( 0 ).InnerText(), "1", "高级列表绑定测试失败" );
      Assert.AreEqual( container.Elements( ".item" ).ElementAt( 1 ).InnerText(), "2", "高级列表绑定测试失败" );
      Assert.AreEqual( container.Elements( ".item" ).ElementAt( 2 ).InnerText(), "3", "高级列表绑定测试失败" );

    }

  }
}
