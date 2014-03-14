using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Specialized;

namespace FormsTest
{
  [TestClass]
  public class FormsTest1
  {
    [TestMethod]
    public void TestNVC()
    {

      var collection = new NameValueCollection();

      collection.Add( "Test", "A" );
      collection.Add( "Test", "B" );

      Assert.AreEqual( collection.GetValues( "Test" ).Length, 2 );
      Assert.AreEqual( collection.GetValues( "TEST" ).Length, 0 );


    }
  }
}
