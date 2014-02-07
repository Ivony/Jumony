using System;
using Ivony.Html.Parser;
using Ivony.Html;
using Ivony.Fluent;
using System.Linq;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Collections.Generic;

namespace SelectorTest
{
  //[TestClass]
  public class CssSelectorTest
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
    [TestCategory( "选择器" )]
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


    [TestMethod]
    [TestCategory( "选择器" )]
    public void SelectorTest1()
    {
      var document = LoadDocument( "CssSelectorTest1.html" );

      SelectorAssert( document, "html", elements => elements.Count() == 1 );
      SelectorAssert( document, "body", elements => elements.Count() == 1 );
      SelectorAssert( document, "html > head", elements => elements.Count() == 1 );
      SelectorAssert( document, "html > head > meta", elements => elements.Count() == 1 );
      SelectorAssert( document, "html > head > meta:empty", elements => elements.Count() == 1 );
      SelectorAssert( document, "html title", elements => elements.Count() == 1 );
      SelectorAssert( document, "html body:empty", elements => elements.Count() == 1 );
      SelectorAssert( document, "html head:empty", elements => elements.Count() == 0 );
      SelectorAssert( document, "html head:only-of-type", elements => elements.Count() == 1 );
      SelectorAssert( document, "html head:last-of-type", elements => elements.Count() == 1 );
      SelectorAssert( document, "html head:first-of-type", elements => elements.Count() == 1 );
      SelectorAssert( document, "html head:first-child", elements => elements.Count() == 1 );
      SelectorAssert( document, "html head:last-child", elements => elements.Count() == 0 );
      SelectorAssert( document, "html body:first-child", elements => elements.Count() == 0 );
      SelectorAssert( document, "html body:last-child", elements => elements.Count() == 1 );
      SelectorAssert( document, "html, body", elements => elements.Count() == 2 );
      SelectorAssert( document, "body, head", elements => elements.Count() == 2 );
      SelectorAssert( document, "html >*", elements => elements.Count() == 2 );
    }

    [TestMethod]
    [TestCategory( "选择器" )]
    public void NegationPseudoClassTest()
    {
      var document = LoadDocument( "NegationPseudoClassTest.html" );

      SelectorAssert( document, "p:not(.test)", elements => elements.Count() == 2 );
      SelectorAssert( document, "p:not( :empty )", elements => elements.Count() == 2 && elements.All( e => e.InnerHtml() != "" ) );
      SelectorAssert( document, "p:not( :first-of-type)", elements => elements.Count() == 1 && elements.All( e => e.ElementsIndexOfSelf() > 0 ) );

    }


    private static IHtmlDocument LoadDocument( string filename )
    {
      var document = new JumonyParser().LoadDocument( Path.Combine( Environment.CurrentDirectory, filename ) );
      return document;
    }

    private void SelectorAssert( IHtmlContainer container, string selector, Predicate<IEnumerable<IHtmlElement>> assert )
    {
      Assert.IsTrue( assert( container.Find( selector ) ), string.Format( "选择器 \"{0}\" 有问题", selector ) );
    }

  }
}
