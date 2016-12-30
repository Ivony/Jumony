using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Ivony.Html
{

  /// <summary>
  /// 协助实现 IHtmlParser 接口
  /// </summary>
  public abstract class HtmlParserBase : IHtmlParser
  {




    /// <summary>
    /// 容器堆栈
    /// </summary>
    protected Stack<IHtmlContainer> ContainerStack
    {
      get;
      private set;
    }

    /// <summary>
    /// 初始化容器
    /// </summary>
    protected virtual void Initialize()
    {
      HtmlSpecification = null;
      ContainerStack = new Stack<IHtmlContainer>();
    }


    /// <summary>
    /// 派生类提供 HTML 文本读取器
    /// </summary>
    /// <param name="html">要读取分析的 HTML 文本</param>
    /// <returns>文本读取器</returns>
    protected abstract IHtmlReader CreateReader( string html );


    /// <summary>
    /// 当前在使用的 HTML 文本读取器
    /// </summary>
    protected IHtmlReader Reader
    {
      get;
      private set;
    }


    /// <summary>
    /// 派生类提供 Provider 用于创建 DOM 结构
    /// </summary>
    public abstract IHtmlDomProvider DomProvider
    {
      get;
    }


    /// <summary>
    /// 当前容器
    /// </summary>
    protected IHtmlContainer CurrentContainer
    {
      get { return ContainerStack.Peek(); }
    }



    private readonly object _sync = new object();


    /// <summary>
    /// 用于同步的对象，在任何公开方法中应lock，确保分析器始终只在一个线程中运行
    /// </summary>
    protected object SyncRoot
    {
      get { return _sync; }
    }


    /// <summary>
    /// 正在处理的文档
    /// </summary>
    protected HtmlDocument Document
    {
      get;
      private set;
    }


    /// <summary>
    /// 分析 HTML 文本并创建文档
    /// </summary>
    /// <param name="html">HTML 文本</param>
    /// <param name="url">文档的 URL</param>
    /// <returns>分析好的 HTML 文档</returns>
    public virtual HtmlDocument Parse( string html, Uri url )
    {
      if ( html == null )
        throw new ArgumentNullException( "html" );

      if ( url != null && !url.IsAbsoluteUri )
        throw new ArgumentException( "必须是绝对URI", "url" );

      lock ( SyncRoot )
      {

        Initialize();

        Document = DomProvider.CreateDocument( url );

        if ( !string.IsNullOrEmpty( html ) )
        {
          ContainerStack.Push( Document );

          ParseInternal( html );
        }

        if ( Document.HtmlSpecification == null )//若始终没能找到 DTD 声明，则设置默认的规范
          DomProvider.SetHtmlSpecification( Document, null );

        return CompleteDocument( Document );

      }
    }


    /// <summary>
    /// 获取所使用的 HTML 规范
    /// </summary>
    protected HtmlSpecificationBase HtmlSpecification
    {
      get;
      private set;
    }

    /// <summary>
    /// 分析 HTML 文本
    /// </summary>
    /// <param name="html">要分析的 HTML 文本</param>
    protected void ParseInternal( string html )
    {


      Reader = CreateReader( html );

      foreach ( var fragment in Reader.EnumerateContent() )
      {

        var text = fragment as HtmlTextContent;
        if ( text != null )
          ProcessText( text );

        var beginTag = fragment as HtmlBeginTag;
        if ( beginTag != null )
          ProcessBeginTag( beginTag );

        var endTag = fragment as HtmlEndTag;
        if ( endTag != null )
          ProcessEndTag( endTag );

        var comment = fragment as HtmlCommentContent;
        if ( comment != null )
          ProcessComment( comment );

        var special = fragment as HtmlSpecialTag;
        if ( special != null )
          ProcessSpecial( special );

        var doctype = fragment as HtmlDoctypeDeclaration;
        if ( doctype != null )
          ProcessDoctypeDeclaration( doctype );

      }
    }



    /// <summary>
    /// 处理文本节点
    /// </summary>
    /// <param name="textContent">HTML文本信息</param>
    /// <returns>处理过程中所创建的文本节点，若不支持则返回 null</returns>
    protected virtual IHtmlTextNode ProcessText( HtmlTextContent textContent )
    {
      return CreateTextNode( textContent.Html );
    }

    /// <summary>
    /// 创建文本节点添加到当前容器
    /// </summary>
    /// <param name="text">HTML 文本</param>
    /// <returns></returns>
    protected virtual IHtmlTextNode CreateTextNode( string text )
    {
      return DomProvider.AddTextNode( CurrentContainer, text );
    }



    /// <summary>
    /// 处理元素开始标签
    /// </summary>
    /// <param name="beginTag">开始标签信息</param>
    /// <returns>处理过程中所创建的元素对象，若不支持则返回 null</returns>
    protected virtual IHtmlElement ProcessBeginTag( HtmlBeginTag beginTag )
    {

      if ( HtmlSpecification == null )//当解析第一个开始标签时还没有找到 DTD 声明，则强行使用默认的 HTML 规范。
        SetHtmlSpecification( (HtmlDoctypeDeclaration) null );

      string tagName = beginTag.TagName;
      bool selfClosed = beginTag.SelfClosed;

      //检查是否为自结束标签，并作相应处理
      if ( IsSelfCloseElement( beginTag ) )
        selfClosed = true;


      //检查是否为CData标签，并作相应处理
      if ( IsCDataElement( beginTag ) )
        Reader.EnterCDataMode( tagName.ToLowerInvariant() );



      //检查父级是否可选结束标记，并作相应处理
      {
        var element = CurrentContainer as IHtmlElement;
        if ( element != null && HtmlSpecification.IsOptionalEndTag( element.Name ) )
        {
          if ( ImmediatelyClose( tagName, element ) )
            ContainerStack.Pop();
        }
      }




      //处理所有属性
      var attributes = new Dictionary<string, string>( StringComparer.OrdinalIgnoreCase );

      foreach ( var a in beginTag.Attributes )
      {
        string name = a.Name;
        string value = a.Value;

        if ( value != null )
          value = HtmlEncoding.HtmlDecode( value );

        if ( attributes.ContainsKey( name ) )//重复的属性名，只取第一个
          continue;

        attributes.Add( name, value );
      }




      //创建元素
      {
        var element = CreateElement( tagName, attributes );


        //加入容器堆栈
        if ( !selfClosed )
          ContainerStack.Push( element );


        return element;
      }
    }



    /// <summary>
    /// 检查元素是否为自结束标签
    /// </summary>
    /// <param name="tag">元素开始标签</param>
    /// <returns>是否为自结束标签</returns>
    protected virtual bool IsSelfCloseElement( HtmlBeginTag tag )
    {
      return HtmlSpecification.IsForbiddenEndTag( tag.TagName );
    }

    /// <summary>
    /// 检查元素是否为CDATA标签
    /// </summary>
    /// <param name="tag">元素开始标签</param>
    /// <returns>是否为CDATA标签</returns>
    protected virtual bool IsCDataElement( HtmlBeginTag tag )
    {
      return HtmlSpecification.IsCDataElement( tag.TagName );
    }



    /// <summary>
    /// 检查当前开放的可选结束标签是否必须立即关闭
    /// </summary>
    /// <param name="tagName">分析器遇到的标签</param>
    /// <param name="element">当前开放的可选结束标签</param>
    /// <returns></returns>
    protected virtual bool ImmediatelyClose( string tagName, IHtmlElement element )
    {
      return HtmlSpecification.ImmediatelyClose( element.Name, tagName );
    }


    /// <summary>
    /// 创建元素并加入当前容器
    /// </summary>
    /// <param name="tagName">元素名</param>
    /// <param name="attributes">元素属性集合</param>
    /// <returns>创建好的元素</returns>
    protected virtual IHtmlElement CreateElement( string tagName, Dictionary<string, string> attributes )
    {
      return DomProvider.AddElement( CurrentContainer, tagName, attributes );
    }




    /// <summary>
    /// 处理结束标签
    /// </summary>
    /// <param name="endTag">结束标签信息</param>
    /// <returns>相关的元素对象，若不支持则返回null</returns>
    protected virtual IHtmlElement ProcessEndTag( HtmlEndTag endTag )
    {
      var tagName = endTag.TagName;


      if ( ContainerStack.OfType<IHtmlElement>().Select( e => e.Name ).Contains( tagName, StringComparer.OrdinalIgnoreCase ) )
      {

        IHtmlElement element;

        while ( true )
        {
          element = ContainerStack.Pop() as IHtmlElement;
          if ( element.Name.EqualsIgnoreCase( tagName ) )
            break;
        }

        return element;
      }
      else
      {
        ProcessEndTagMissingBeginTag( endTag );

        return null;
      }

      //无需退出CData标签，读取器会自动退出
    }

    /// <summary>
    /// 处理丢失了开始标签的结束标签
    /// </summary>
    /// <param name="endTag">结束标签信息</param>
    protected virtual void ProcessEndTagMissingBeginTag( HtmlEndTag endTag )
    {
      //如果堆栈中没有对应的开始标签，则将这个结束标签解释为文本
      CreateTextNode( endTag.Html );
    }





    /// <summary>
    /// 处理 HTML 注释
    /// </summary>
    /// <param name="commentContent">HTML 注释信息</param>
    /// <returns>处理过程中所创建的注释对象，若不支持则返回 null</returns>
    protected virtual IHtmlComment ProcessComment( HtmlCommentContent commentContent )
    {
      return CreateCommet( commentContent.Comment );
    }


    /// <summary>
    /// 创建注释节点并加入当前容器
    /// </summary>
    /// <param name="comment">注释内容</param>
    /// <returns>创建的注释节点</returns>
    protected virtual IHtmlComment CreateCommet( string comment )
    {
      return DomProvider.AddComment( CurrentContainer, comment );
    }



    /// <summary>
    /// 处理特殊节点
    /// </summary>
    /// <param name="specialTag">特殊的 HTML 标签</param>
    /// <returns>处理过程中所创建的特殊标签节点，若不支持则返回 null</returns>
    protected virtual IHtmlSpecial ProcessSpecial( HtmlSpecialTag specialTag )
    {
      return CreateSpecial( specialTag.Html );
    }

    /// <summary>
    /// 创建特殊标签节点并加入当前容器
    /// </summary>
    /// <param name="html">特殊标签的 HTML 内容</param>
    /// <returns>创建的特殊标签节点</returns>
    protected virtual IHtmlSpecial CreateSpecial( string html )
    {
      return DomProvider.AddSpecial( CurrentContainer, html );
    }


    /// <summary>
    /// 处理文档类型声明
    /// </summary>
    /// <param name="doctype">文档类型声明</param>
    /// <returns></returns>
    protected virtual IHtmlSpecial ProcessDoctypeDeclaration( HtmlDoctypeDeclaration doctype )
    {

      SetHtmlSpecification( doctype );
      return CreateSpecial( doctype.Html );

    }


    /// <summary>
    /// 根据 DTD 声明设置相应的 HTML 规范
    /// </summary>
    /// <param name="doctype">DTD 声明</param>
    /// <returns>所适用的 HTML 规范</returns>
    protected virtual void SetHtmlSpecification( HtmlDoctypeDeclaration doctype )
    {
      if ( HtmlSpecification == null )
      {
        var declaration = doctype.IfNull( null, d => d.Declaration );

        SetHtmlSpecification( DomProvider.SetHtmlSpecification( Document, declaration ) );

      }
    }


    /// <summary>
    /// 设置当前解析器所使用的 HTML 规范
    /// </summary>
    /// <param name="specification">要设置的 HTML 解析规范</param>
    /// <returns></returns>
    protected virtual void SetHtmlSpecification( HtmlSpecificationBase specification )
    {
      if ( specification == null )
        throw new ArgumentNullException( "specification" );

      if ( HtmlSpecification != null )
        throw new InvalidOperationException( "已经设置了当前所使用的 HTML 规范" );

      HtmlSpecification = specification;
    }


    /// <summary>
    /// 完成最终文档解析构建
    /// </summary>
    /// <param name="document">已经完成的文档结构</param>
    /// <returns>最终完成的文档</returns>
    protected virtual IHtmlDocument CompleteDocument( IHtmlDocument document )
    {
      return DomProvider.CompleteDocument( document );
    }




  }
}
