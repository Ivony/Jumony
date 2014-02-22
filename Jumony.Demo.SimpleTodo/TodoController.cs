using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Ivony.Html;
using Ivony.Html.Web;
using Ivony.Data;
using Ivony.Web;

namespace Jumony.Demo.SimpleTodo
{

  public class TodoController : Controller
  {



    private SqlDbUtility dbUtility = SqlDbUtility.Create( "Database" );

    public ActionResult Index()
    {
      return View( "index", dbUtility.T( "SELECT ID, Title, Completed FROM Tasks" ).ExecuteEntities<Task>() );
    }


    public ActionResult Add( string title )
    {

      dbUtility.T( "INSERT Tasks ( Title, Completed ) VALUES ( {...} )", title, false ).ExecuteNonQuery();

      return RedirectToAction( "Index" );
    }

    public ActionResult Complete( int taskId )
    {
      dbUtility.T( "UPDATE Tasks SET Completed = 1 WHERE ID = {0}", taskId ).ExecuteNonQuery();

      return RedirectToAction( "Index" );
    }

    public ActionResult Revert( int taskId )
    {
      dbUtility.T( "UPDATE Tasks SET Completed = 0 WHERE ID = {0}", taskId ).ExecuteNonQuery();

      return RedirectToAction( "Index" );
    }

    public ActionResult Remove( int taskId )
    {
      dbUtility.T( "DELETE Tasks WHERE ID = {0}", taskId ).ExecuteNonQuery();

      return RedirectToAction( "Index" );
    }

    [HttpGet]
    public ActionResult Modify( int taskId )
    {

      return View( "modify", dbUtility.T( "SELECT ID, Title, Completed FROM Tasks WHERE ID = {0}", taskId ).ExecuteEntities<Task>() );

    }

    [HttpPost]
    public ActionResult Modify( int taskId, string title )
    {

      if ( !ViewData.ModelState.IsValid )
        return View( "Index" );


      dbUtility.T( "UPDATE Tasks SET Title = {1} WHERE ID = {0}", taskId, title ).ExecuteNonQuery();

      return RedirectToAction( "Index" );
    }

  }


  public class TodoCachePolicyProvider : ControllerCachePolicyProvider
  {
    public CachePolicy Index( ControllerContext context, IDictionary<string, object> args )
    {

      var token = CacheToken.FromCookies( context.HttpContext ) + CacheToken.CreateToken( "Index" );

      return null;
      return new StandardCachePolicy( context.HttpContext, token, this, TimeSpan.FromMinutes( 1 ), true );

    }
  }
}