using Ivony.Html;
using Ivony.Html.Binding;
using Ivony.Html.Parser;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace BindingTest
{
  [TestClass]
  public class ScriptBinderTest
  {

    [TestMethod]
    public void Test1()
    {
      var document = new JumonyParser().LoadDocument( Path.Combine( Environment.CurrentDirectory, "Test1.html" ) );
      var data = new Dictionary<string, object>();
      data.Add( "StyleClass", null );
      data.Add( "ThisTime", null );
      data.Add( "ScriptValue1", "TestValue" );

      HtmlBinding.Create( document, data ).DataBind();

      StringAssert.Contains( document.FindFirst( "script" ).InnerHtml(), "var value1 =\"TestValue\";" );
    }

  }
}
