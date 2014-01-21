using Ivony.Html;
using Ivony.Html.Binding;
using Ivony.Html.Parser;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Collections.Generic;

namespace BindingTest
{
  [TestClass]
  public class ExpressionBinderTest
  {
    [TestMethod]
    public void AttributeTest1()
    {
      var document = new JumonyParser().LoadDocument( Path.Combine( Environment.CurrentDirectory, "Test1.html" ) );
      HtmlBinding.Create( document, null, new Dictionary<string, object>() { { "StyleClass", "Test" } } ).DataBind();


      Assert.AreEqual( document.FindFirst( "body" ).Attribute( "class" ).Value(), "Test", "针对属性的表达式绑定不成功" );
    }
  }
}
