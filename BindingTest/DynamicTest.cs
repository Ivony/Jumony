using Ivony.Html.Binding;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BindingTest
{

  [TestClass]
  public class DynamicTest
  {

    [TestMethod]
    public void DynamicTest1()
    {

      var a = JObject.FromObject( new { A = 1 } );
      Assert.AreEqual( (int) DynamicBinder.GetPropertyValue( a, "A" ), 1 );


    }

  }

}
