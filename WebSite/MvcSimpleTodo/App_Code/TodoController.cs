using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Ivony.Html;
using Ivony.Html.Web.Mvc;
using Ivony.Html.Web;
using Ivony.Data;

public class TodoController : Controller
{



  private SqlDbUtility dbUtility = SqlDbUtility.Create( "Database" );

  private class MyViewFilter : ViewFilterAttribute
  {

    public override void OnPreRender( ViewContext context, ViewBase view )
    {
      var pageView = view as PageView;

      if ( pageView == null )
        return;

      pageView.Document.Find( "title" ).First().InnerHtml( "Test" );

    }

  }

  [MyViewFilter]
  public ActionResult Index()
  {
    return View( "index", dbUtility.Entities<Task>( "SELECT ID, Title, Completed FROM Tasks" ) );
  }


  public ActionResult Add( string title )
  {

    dbUtility.NonQuery( "INSERT Tasks ( Title, Completed ) VALUES ( {...} )", title, false );

    return RedirectToAction( "Index" );
  }

  public ActionResult Complete( int taskId )
  {
    dbUtility.NonQuery( "UPDATE Tasks SET Completed = 1 WHERE ID = {0}", taskId );

    return RedirectToAction( "Index" );
  }

  public ActionResult Revert( int taskId )
  {
    dbUtility.NonQuery( "UPDATE Tasks SET Completed = 0 WHERE ID = {0}", taskId );

    return RedirectToAction( "Index" );
  }

  public ActionResult Remove( int taskId )
  {
    dbUtility.NonQuery( "DELETE Tasks WHERE ID = {0}", taskId );

    return RedirectToAction( "Index" );
  }

  [HttpGet]
  public ActionResult Modify( int taskId )
  {

    return View( "modify", dbUtility.Entity<Task>( "SELECT TaskId, Title, Content, Completed FROM Tasks WHERE ID = {0}", taskId ) );

  }

  [HttpPost]
  public ActionResult Modify( int taskId, string title )
  {

    if ( !ViewData.ModelState.IsValid )
      return View( "Index" );


    dbUtility.NonQuery( "UPDATE Tasks SET Title = {1} WHERE ID = {0}", taskId, title );

    return RedirectToAction( "Index" );
  }

}


public class TodoCachePolicyProvider : ControllerCachePolicyProvider
{
  public CachePolicy Index( ControllerContext context, IDictionary<string, object> args )
  {

    var token = CacheToken.FromCookies( context.HttpContext ) + CacheToken.CreateToken( "Index" );

    return new StandardCachePolicy( context.HttpContext, token, this, TimeSpan.FromMinutes( 1 ), true );

  }
}