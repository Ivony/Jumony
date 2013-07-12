using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ivony.Html.Parser
{

  /// <summary>
  /// 标准 Jumony DOM 模型提供程序
  /// </summary>
  public class DomProvider : IHtmlDomProvider
  {


    private IDomFragmentParserProvider _fragmentParserProvider;

    /// <summary>
    /// 创建 DomProvider 对象
    /// </summary>
    /// <param name="provider">文档碎片解析器提供程序</param>
    public DomProvider( IDomFragmentParserProvider provider )
    {
      _fragmentParserProvider = provider;
    }

    /// <summary>
    /// 创建文档
    /// </summary>
    /// <returns></returns>
    public IHtmlDocument CreateDocument()
    {
      return new DomDocument( null, _fragmentParserProvider );
    }

    /// <summary>
    /// 创建文档
    /// </summary>
    /// <param name="url">文档的 URL</param>
    /// <returns></returns>
    public IHtmlDocument CreateDocument( Uri url )
    {
      return new DomDocument( url, _fragmentParserProvider );
    }



    /// <summary>
    /// 设置文档所采用的文档规范
    /// </summary>
    /// <param name="document">文档</param>
    /// <param name="specificationType">所采用的文档规范名称</param>
    public HtmlSpecificationBase SetHtmlSpecification( IHtmlDocument document, string specificationType )
    {

      if ( document == null )
        throw new ArgumentNullException( "document" );

      var domDocument = document as DomDocument;
      if ( domDocument == null )
        throw new NotSupportedException( "只能处理指定类型的文档" );

      if ( domDocument.HtmlSpecification != null )
        throw new InvalidOperationException();

      return domDocument.HtmlSpecification = SelectSpecification( specificationType );
    }

    private HtmlSpecificationBase SelectSpecification( string specificationType )
    {
      return new Html41Specification();
    }


    internal static IDomContainer EnsureDomContainer( IHtmlContainer container )
    {
      var domContainer = container as IDomContainer;

      if ( domContainer == null )
        throw new NotSupportedException( "只能向指定类型容器添加节点" );

      return domContainer;
    }


    /// <summary>
    /// 向容器中添加元素
    /// </summary>
    /// <param name="container">要添加元素的容器</param>
    /// <param name="name">元素名</param>
    /// <param name="attributes">属性列表</param>
    /// <returns>添加好的元素</returns>
    public IHtmlElement AddElement( IHtmlContainer container, string name, IDictionary<string, string> attributes )
    {
      return EnsureDomContainer( container ).AddNode( new DomElement( name, attributes ) );
    }

    /// <summary>
    /// 向容器中添加文本节点
    /// </summary>
    /// <param name="container">要添加文本节点的容器</param>
    /// <param name="htmlText">HTML 文本</param>
    /// <returns>添加好的文本节点</returns>
    public IHtmlTextNode AddTextNode( IHtmlContainer container, string htmlText )
    {
      return EnsureDomContainer( container ).AddNode( new DomTextNode( htmlText ) );
    }

    /// <summary>
    /// 向容器中添加注释
    /// </summary>
    /// <param name="container">要添加注释的容器</param>
    /// <param name="comment">要添加的注释</param>
    /// <returns>添加好的注释</returns>
    public IHtmlComment AddComment( IHtmlContainer container, string comment )
    {
      return EnsureDomContainer( container ).AddNode( new DomComment( comment ) );
    }

    /// <summary>
    /// 向容器中添加特殊节点
    /// </summary>
    /// <param name="container">要添加特殊节点的容器</param>
    /// <param name="html">特殊节点的 HTML</param>
    /// <returns>添加好的特殊节点</returns>
    public IHtmlSpecial AddSpecial( IHtmlContainer container, string html )
    {
      return EnsureDomContainer( container ).AddNode( new DomSpecial( html ) );
    }

    /// <summary>
    /// 完成文档的创建
    /// </summary>
    /// <remarks>将在此步骤注入 FragmentParserProvider 对象</remarks>
    /// <param name="document">已经完成 DOM 结构部署的文档</param>
    /// <returns>创建完成的文档</returns>
    public IHtmlDocument CompleteDocument( IHtmlDocument document )
    {
      var domDocument = document as DomDocument;
      if ( domDocument == null )
        throw new InvalidOperationException();

      if ( domDocument.HtmlSpecification == null )
        throw new InvalidOperationException( "尚未设置文档所使用的 HTML 规范！" );

      return domDocument;
    }

  }
}
