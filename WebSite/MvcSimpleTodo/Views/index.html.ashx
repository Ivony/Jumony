<%@ WebHandler Language="C#" Class="index_html" %>

using System;
using System.Web;
using System.Linq;

using Ivony.Fluent;
using Ivony.Html;
using Ivony.Html.Web;
using Ivony.Html.Styles;
using Ivony.Html.Forms;
using Ivony.Html.Forms.Validation;
using Ivony.Html.Templates;


public class index_html : ViewHandler<Task[]>
{

  protected override void ProcessScope()
  {
    //FindSingle( "form" ).SetAttribute( "action", Url.Action( "Add" ) );

    var items = FindSingle( "ul > li" ).Repeat( Model.Count() );//绑定数据

    Model.BindTo( items, BindTaskItem );
  }

  [HandleElement( "form" )]
  public void Test( IHtmlElement element )
  {
    element.SetAttribute( "action", Url.Action( "Add" ) );
  }

  public void BindTaskItem( Task task, IHtmlElement taskElement )
  {
    taskElement.Elements( "span" ).Single().InnerText( task.Title );

    taskElement.Find( "a" ).SetAttribute( "_taskId", task.ID.ToString() );

    if ( task.Completed )
    {
      taskElement.SetAttribute( "class", "finished" );
      taskElement.Find( "a[action=complete]" ).Remove();
    }
    else
    {
      taskElement.Find( "a[action=revert]" ).Remove();
    }
  }
}