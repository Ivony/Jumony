using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.IO;

namespace Ivony.Html.Web.Mvc
{
  public class JumonyViewResult : ActionResult
  {

    private ViewResultBase _result;

    private IHtmlHandler _handler;

    public JumonyViewResult( ViewResultBase result, IHtmlHandler handler )
    {
      _result = result;
      _handler = handler;
    }

    public override void ExecuteResult( ControllerContext context )
    {
      var response = context.HttpContext.Response;
      var responseWriter = response.Output;          //先保存下标准输出

      string content;

      using ( var writer = new StringWriter() )      //创建StringWriter截获标准输出
      {
        response.Output = writer;

        _result.ExecuteResult( context );            //执行操作

        content = writer.ToString();
      }


      var parser = HtmlProviders.GetParser( context.HttpContext, context.HttpContext.Request.AppRelativeCurrentExecutionFilePath, content );

      var document = parser.Parse( content );

      _handler.ProcessDocument( document );          //处理文档

      response.Output = responseWriter;              //将标准输出复原

      document.Render( responseWriter );             //输出处理后的结果
    }
  }
}
