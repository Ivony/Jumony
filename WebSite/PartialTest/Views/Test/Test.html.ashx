<%@ WebHandler Language="C#" Class="Test_html" %>

using System;
using System.Web;
using Ivony.Html.Web;

public class Test_html : JumonyViewHandler
{


  public string Partial_Test( int value, string a, int b = 8 )
  {
    if ( value == 5 && a== null && b == 8 )
      return "测试成功!";
    else
      return "失败";
  }

}