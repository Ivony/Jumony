using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Specialized;
using Ivony.Html;
using Ivony.Html.Parser;
using Ivony.Html.Forms;
using System.IO;

namespace FormsTest
{
  [TestClass]
  public class FormsTest1
  {
    [TestMethod]
    public void TestNVC()
    {


      var document = new JumonyParser().LoadDocument( Path.Combine( Environment.CurrentDirectory, "FormsTest1.html" ) );


      Exception e = null;

      try
      {

        document.FindFirst( "form" ).AsForm();

      }
      catch ( Exception exception )
      {
        e = exception;
      }


      Assert.IsNotNull( e, "表单中存在重复的文本输入框未能引发异常" );
      Assert.IsInstanceOfType( e, typeof( InvalidOperationException ), "表单中存在重复的文本输入框未能引发正确的异常" );


    }
  }
}
