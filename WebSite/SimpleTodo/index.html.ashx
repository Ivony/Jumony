<%@ WebHandler Language="C#" Class="index_html" %>

using System;
using System.Web;
using System.Linq;

using Ivony.Fluent;
using Ivony.Html;
using Ivony.Html.Web;
using Ivony.Html.Templates;
using Ivony.Html.Styles;
using Ivony.Html.Forms;
using Ivony.Html.Forms.Validation;

using DatabaseModel;


public class index_html : JumonyHandler
{


  protected override void ProcessDocument()
  {

    Document.Find( "form" ).SetAttribute( "action", Request.Url.AbsoluteUri );//设置表单回发



    using ( var context = new DatabaseEntities() )
    {

      var _taskId = Request.QueryString["task"];

      if ( _taskId != null )
      {
        var taskId = _taskId.ConvertTo<int>();
        var task = context.Tasks.Single( t => t.ID == taskId );


        switch ( Request.QueryString["action"] )
        {
          case "delete":
            context.DeleteObject( task );
            break;

          case "complete":
            task.Completed = true;
            break;

          case "restore":
            task.Completed = false;
            break;

          case "modify":
            if ( Request.HttpMethod.EqualsIgnoreCase( "post" ) )//POST提交，表示修改
            {
              task.Content = Request.Form["content"];
            }                                                   //否则显示编辑界面。
            else
            {
              Document.FindSingle( ".todos" ).Remove();
              Document.FindSingle( ".post h2" ).InnerText( "编辑" );
              Document.FindSingle( "input[name=content]" ).SetAttribute( "value", task.Content );
              
              return;
            }
            break;

        }

        context.SaveChanges();
        Response.Redirect( "index.html" );


      }
      else if ( Request.HttpMethod.EqualsIgnoreCase( "post" ) )//POST提交，表示添加
      {
        context.Tasks.AddObject( new Task() { Content = Request.Form["content"], Completed = false } );
        context.SaveChanges();
      }




      var items = Document.FindSingle( "ul > li" ).Repeat( context.Tasks.Count() );

      context.Tasks.BindTo( items, BindTaskItem );

      return;


    }

  }

  private void BindTaskItem( Task task, IHtmlElement taskElement )
  {
    taskElement.Elements( "span" ).Single().InnerText( task.Content );

    taskElement.Find( "a" ).SetAttribute( "href", url => url + "&task=" + task.ID );

    if ( task.Completed )
    {
      taskElement.Style( ".finished;" );
      taskElement.Find( "a[href*=complete]" ).Remove();
    }
    else
    {
      taskElement.Find( "a[href*=restore]" ).Remove();
    }
  }


  private class MyFormValidator : HtmlFormValidator
  {

    public MyFormValidator( HtmlForm form )
      : base( form )
    {
      AddFieldValidation( "content", new RequiredValidator() );
    }

    protected override IHtmlElement FailedMessageContainer( IHtmlInputControl input )
    {
      return input.CastTo<HtmlInputText>().Element.Parent().FindSingle( ".tips" );
    }
  }



}