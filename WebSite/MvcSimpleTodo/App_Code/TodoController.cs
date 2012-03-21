using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DatabaseModel;
using System.ComponentModel.DataAnnotations;

using Ivony.Html;
using Ivony.Html.Web.Mvc;

public class TodoController : Controller
{


  protected DatabaseEntities Entities { get; private set; }

  protected override void ExecuteCore()
  {
    using ( Entities = new DatabaseEntities() )
    {
      base.ExecuteCore();

      Entities.SaveChanges();
    }
  }


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
    return View( "index", Entities.Tasks );
  }


  public ActionResult Add( string title )
  {
    Entities.Tasks.AddObject( new Task() { Title = title, Completed = false } );

    return RedirectToAction( "Index" );
  }

  public ActionResult Complete( int taskId )
  {
    Entities.Tasks.First( t => t.ID == taskId ).Completed = true;

    return RedirectToAction( "Index" );
  }

  public ActionResult Revert( int taskId )
  {
    Entities.Tasks.First( t => t.ID == taskId ).Completed = false;

    return RedirectToAction( "Index" );
  }

  public ActionResult Remove( int taskId )
  {
    Entities.DeleteObject( Entities.Tasks.First( t => t.ID == taskId ) );

    return RedirectToAction( "Index" );
  }

  [HttpGet]
  public ActionResult Modify( int taskId )
  {

    return View( "modify", Entities.Tasks.First( t => t.ID == taskId ) );

  }

  [HttpPost]
  public ActionResult Modify( int taskId, string title )
  {

    if ( !ViewData.ModelState.IsValid )
      return View( "Index" );


    Entities.Tasks.First( t => t.ID == taskId ).Title = title;

    return RedirectToAction( "Index" );
  }

}