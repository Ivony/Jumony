using Ivony.Html.Binding;
using Ivony.Html.Parser;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Ivony.Html;
using Newtonsoft.Json.Linq;

namespace BindingTest
{
  [TestClass]
  public class EvalExpressionTest
  {

    [TestMethod]
    public void EvalTest1()
    {

      var document = new JumonyParser().LoadDocument( Path.Combine( Environment.CurrentDirectory, "Test1.html" ) );


      {
        var data = new { A = 123, B = "ABC" };
        var context = HtmlBinding.Create( document, data );
        {
          var expression = BindingExpression.ParseExpression( "{eval path=A}" );
          Assert.AreEqual( context.GetValue( expression ), 123 );
        }

        {
          var expression = BindingExpression.ParseExpression( "{eval path=B}" );
          Assert.AreEqual( context.GetValue( expression ), "ABC" );
        }
      }

      {
        var data = JObject.Parse( "{A:123,B:'ABC'}" );
        var context = HtmlBinding.Create( document, data );
        {
          var expression = BindingExpression.ParseExpression( "{eval path=A}" );
          Assert.AreEqual( context.GetValue( expression ).ToString(), "123" );
        }

        {
          var expression = BindingExpression.ParseExpression( "{eval path=B}" );
          Assert.AreEqual( context.GetValue( expression ).ToString(), "ABC" );
        }
      }


    }

  }
}
