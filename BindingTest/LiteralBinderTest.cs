using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ivony.Html.Parser;
using Ivony.Html;
using System.IO;
using Ivony.Html.Binding;

namespace BindingTest
{
  [TestClass]
  public class LiteralBinderTest
  {
    [TestMethod]
    public void Test1()
    {

      var document = new JumonyParser().LoadDocument( Path.Combine( Environment.CurrentDirectory, "Test1.html" ) );
      HtmlBinding.Create( document, null ).DataBind();


      Assert.AreEqual( document.FindFirst( "title" ).InnerHtml(), "Test Title abc text", "对 title 元素内容的文本替换测试失败" );

    }
  }
}
