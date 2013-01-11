using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace Ivony.Html.Web.Mvc
{

  /// <summary>
  /// 母板页视图
  /// </summary>
  public class MasterView : ViewBase, IView
  {


    /// <summary>
    /// 创建 MasterView 实例
    /// </summary>
    public MasterView()
    {
      RenderAdapters = new List<IHtmlAdapter>();
    }


    private bool _initialized = false;

    /// <summary>
    /// 初始化母板页视图
    /// </summary>
    /// <param name="virtualPath">母板页路径</param>
    internal void Initialize( string virtualPath )
    {
      base.Initialize( virtualPath, false );

    }


    /// <summary>
    /// 自定义渲染过程的 HTML 转换器
    /// </summary>
    protected virtual IList<IHtmlAdapter> RenderAdapters
    {
      get;
      private set;
    }


    /// <summary>
    /// 母板页文档
    /// </summary>
    public IHtmlDocument Document
    {
      get;
      private set;
    }

    /// <summary>
    /// 处理母板页
    /// </summary>
    internal void ProcessCore()
    {
      Document = (IHtmlDocument) InitializeScope( VirtualPath, false );
      ProcessMaster();

    }


    /// <summary>
    /// 处理母板页
    /// </summary>
    protected virtual void ProcessMaster()
    {

    }


    /// <summary>
    /// 合并页面头数据
    /// </summary>
    /// <param name="page">要合并的页面视图</param>
    internal protected virtual void MergeHeader( IHtmlDocument page )
    {
    }



    /// <summary>
    /// 渲染页面
    /// </summary>
    /// <param name="writer"></param>
    internal void RenderCore( TextWriter writer, IHtmlAdapter contentAdapter )
    {
      Document.Render( writer, new[] { contentAdapter } );
    }




    protected override string RenderCore( IHtmlContainer scope )
    {
      throw new NotSupportedException( "母板页不能当作视图生成" );
    }

    void IView.Render( ViewContext viewContext, System.IO.TextWriter writer )
    {
      throw new NotSupportedException( "母板页不能当作视图生成" );
    }
  }
}
