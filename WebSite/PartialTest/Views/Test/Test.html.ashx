<%@ WebHandler Language="C#" Class="Test_html" %>

using System;
using System.Web;
using Ivony.Html.Web;

public class Test_html : JumonyViewHandler
{


  public string Partial_Test()
  {
    return "测试成功!";
  }

}