using System;
using Ivony.Html.Parser;
using Ivony.Html;
using Ivony.Fluent;
using System.Linq;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SelectorTest
{
  [TestClass]
  public class UnitTest1
  {


    protected IHtmlDocument Document { get; set; }

    [TestInitialize]
    public void TestInitialize()
    {
      Document = new JumonyParser().LoadDocument( "http://www.sina.com.cn/" );
    }

    [ClassInitialize]
    public static void Initialize( TestContext context )
    {
    }


    public TestContext TestContext { get; set; }




    [TestMethod]
    public void Test1()
    {

      TestSelector( Document, "body p a" );
      TestSelector( Document, "p > a" );
      TestSelector( Document, "p[class] a" );
      TestSelector( Document, "p a[href]" );
      TestSelector( Document, "p + a" );
      TestSelector( Document, "div a" );
      TestSelector( Document, "p div a" );
      TestSelector( Document, "a img[src]" );
      TestSelector( Document, "div img" );
      TestSelector( Document, "body img[src]" );
    }

    private void TestSelector( IHtmlDocument document, string selector )
    {
      TestContext.WriteLine( "Selector \"{0}\" seleted {1} elements", selector, document.Find( selector ).Count() );
    }
  }
}
