using Ivony.Html.Web;
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

      var globalService = "global";

      WebServiceLocator.RegisterService( service1, "~/test/1.html" );
      WebServiceLocator.RegisterService( service2, "~/test/" );
      WebServiceLocator.RegisterService( service3, "~/" );
      WebServiceLocator.RegisterService( globalService );

      string path = "~/test/1.html";
      var services = WebServiceLocator.GetServices<string>( path );


      Assert.Contains( services, service1, "注册服务在页面失败" );
      Assert.Contains( services, service2, "注册服务在目录失败" );
      Assert.Contains( services, service3, "注册服务在根目录失败" );
      Assert.Contains( services, globalService, "注册全局服务失败" );

      WebServiceLocator.UnregisterService( service1, path );
      Assert.NotContains( WebServiceLocator.GetServices<string>( path ), service1, "在页面解除注册服务失败" );

      WebServiceLocator.UnregisterService( service2, path );
      Assert.NotContains( WebServiceLocator.GetServices<string>( path ), service2, "在目录解除注册服务失败" );

      WebServiceLocator.UnregisterService( service3, path );
      Assert.NotContains( WebServiceLocator.GetServices<string>( path ), service3, "在根目录解除注册服务失败" );

      WebServiceLocator.UnregisterService( globalService );
      Assert.NotContains( WebServiceLocator.GetServices<string>( path ), globalService, "解除注册全局服务失败" );




    }


    public void ContentService()
    {

      var provider = new TestContentService();

      WebServiceLocator.RegisterService( provider, VirtualPathUtility.GetDirectory( testContentPath ) );
      var result = HtmlServices.LoadContent( testContentPath );

      Assert.AreEqual( result.Content, testContent, "测试内容提供程序失败" );

      Assert.AreEqual( result.Provider, provider, "内容结果中的提供程序错误" );
      Assert.AreEqual( result.VirtualPath, testContentPath, "内容结果中的虚拟路径错误" );

    }




    private const string testContentPath = "~/test/content/1.html";
    private const string testContent = "Hello World";

    private class TestContentService : IHtmlContentProvider
    {


      public HtmlContentResult LoadContent( string virtualPath )
      {
        if ( virtualPath == testContentPath )
          return new HtmlContentResult( testContent, virtualPath );

        else
          return null;
      }
    }


  }
}