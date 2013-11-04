using Ivony.Web;
using Ivony.Web.Test;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebTest
{
  public class WebServiceTest : TestClass
  {

    public void RegisterTest()
    {
      var service1 = "~/test/1.html";
      var service2 = "~/test/";
      var service3 = "~/";

      WebServices.RegisterService( service1, "~/test/1.html" );
      WebServices.RegisterService( service2, "~/test/" );
      WebServices.RegisterService( service3, "~/" );

      var services = WebServices.GetServices<object>( "~/test/1.html" );

      Assert.Contains( services, service1, "注册服务在页面失败" );
      Assert.Contains( services, service2, "注册服务在目录失败" );
      Assert.Contains( services, service3, "注册服务在根目录失败" );


    }

  }
}