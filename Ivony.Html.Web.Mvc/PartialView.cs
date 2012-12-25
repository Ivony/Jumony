using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Web;

namespace Ivony.Html.Web.Mvc
{

  /// <summary>
  /// 部分视图
  /// </summary>
  public abstract class PartialView : ViewBase
  {

    /// <summary>
    /// 创建部分视图实例
    /// </summary>
    protected PartialView()
    {

    }


    /// <summary>
    /// 创建部分视图实例
    /// </summary>
    /// <param name="virtualPath">部分视图的虚拟路径</param>
    protected PartialView( string virtualPath )
    {
      Initialize( virtualPath );
    }



    /// <summary>
    /// 初始化部分视图
    /// </summary>
    /// <param name="virtualPath"></param>
    protected void Initialize( string virtualPath )
    {
      base.Initialize( virtualPath, true );
    }




    /// <summary>
    /// 需要处理的容器
    /// </summary>
    public IHtmlContainer Container
    {
      get;
      private set;
    }

    /// <summary>
    /// 处理部分视图
    /// </summary>
    protected abstract void ProcessContainer();


    protected override void Process( IHtmlContainer scope )
    {
      Container = scope;
      ProcessContainer();
    }
  }


  /// <summary>
  /// 标准部分视图处理程序，当没有指定部分视图处理类时使用
  /// </summary>
  public class GenericPartialView : PartialView
  {

    /// <summary>
    /// 创建 GenericPartialView 实例
    /// </summary>
    /// <param name="virtualPath"></param>
    public GenericPartialView( string virtualPath )
      : base( virtualPath )
    { }




    /// <summary>
    /// 处理部分视图
    /// </summary>
    protected override void ProcessContainer()
    {
      return;
    }
  }



}
