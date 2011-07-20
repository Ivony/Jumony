using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DatabaseModel;

/// <summary>
///TodoController 的摘要说明
/// </summary>
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

}