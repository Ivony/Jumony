using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Ivony.Html.Parser
{
  public abstract class HtmlParserBase : IHtmlParser
  {


    private static readonly string tagPattern = string.Format( @"(?<beginTag>{0})|(?<endTag>{1})|(?<comment>{2})|(?<special>{3})", Regulars.beginTagPattern, Regulars.endTagPattern, Regulars.commentPattern, Regulars.specialTagPattern );

    protected static readonly Regex tagRegex = new Regex( tagPattern, RegexOptions.Compiled );

    private static readonly IDictionary<string, Regex> endTagRegexes = new Dictionary<string, Regex>( StringComparer.InvariantCultureIgnoreCase );


    static HtmlParserBase()
    {
      foreach ( var tagName in HtmlSpecification.cdataTags )//为所有的CDATA标签准备匹配结束标记的正则表达式
      {
        endTagRegexes.Add( tagName, new Regex( @"</#tagName\s*>".Replace( "#tagName", tagName ), RegexOptions.IgnoreCase | RegexOptions.Compiled ) );
      }
    }


    /// <summary>
    /// 容器堆栈
    /// </summary>
    protected Stack<IHtmlContainer> ContainerStack
    {
      get;
      private set;
    }

    /// <summary>
    /// 初始化容器堆栈
    /// </summary>
    protected virtual void InitializeStack()
    {
      ContainerStack = new Stack<IHtmlContainer>();
    }


    /// <summary>
    /// 派生类提供 Provider 用于创建 DOM 结构
    /// </summary>
    protected abstract IHtmlProvider Provider
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
    /// 分析 HTML 文本并创建文档
    /// </summary>
    /// <param name="html">HTML 文本</param>
    /// <returns>分析好的 HTML 文档</returns>
    public virtual IHtmlDocument Parse( string html )
    {

      lock ( SyncRoot )
      {

        InitializeStack();

        var document = Provider.CreateDocument();

        if ( string.IsNullOrEmpty( html ) )
          return document;

        ContainerStack.Push( document );

        ParseInternal( html );

        return document;

      }
    }


    /// <summary>
    /// 分析 HTML 文本
    /// </summary>
    /// <param name="html">要分析的 HTML 文本</param>
    protected void ParseInternal( string html )
    {
      int index = 0;

      while ( true )
      {

        //CData标签处理
        var element = CurrentContainer as IHtmlElement;

        if ( element != null && HtmlSpecification.cdataTags.Contains( element.Name, StringComparer.InvariantCultureIgnoreCase ) )//如果在CData标签内。
        {

          Regex endTagRegex = endTagRegexes[element.Name];
          var endTagMatch = endTagRegex.Match( html, index );


          //将所有内容当作文本节点处理
          index = ProcessText( html, index, endTagMatch );


          ContainerStack.Pop();
          continue;

        }


        var match = tagRegex.Match( html, index );

        if ( !match.Success )//如果不再有标签的匹配
          break;


        //处理文本节点
        index = ProcessText( html, index, match );



        if ( match.Groups["beginTag"].Success )
          ProcessBeginTag( match );
        else if ( match.Groups["endTag"].Success )
          ProcessEndTag( match );
        else if ( match.Groups["comment"].Success )
          ProcessComment( match );
        else if ( match.Groups["special"].Success )
          ProcessSpecial( match );
        else
          throw new InvalidOperationException();
      }


      //处理末尾的文本
      if ( index != html.Length )
        CreateTextNode( html.Substring( index ) );
    }



    /// <summary>
    /// 处理文本节点
    /// </summary>
    /// <param name="html">正在分析的 HTML 文本</param>
    /// <param name="index">文本节点开始位置</param>
    /// <param name="match">下一个匹配（非文本节点）</param>
    protected int ProcessText( string html, int index, Match match )
    {
      var text = html.Substring( index, match.Index - index );
      if ( text.Length > 0 )
        CreateTextNode( text );

      index = match.Index + match.Length;
      return index;
    }


    /// <summary>
    /// 创建文本节点添加到当前容器
    /// </summary>
    /// <param name="text">HTML 文本</param>
    /// <returns></returns>
    protected virtual IHtmlTextNode CreateTextNode( string text )
    {
      return Provider.AddTextNode( CurrentContainer, CurrentContainer.Nodes().Count(), text );
    }




    /// <summary>
    /// 处理元素开始标签
    /// </summary>
    /// <param name="match">捕获到的开始标签匹配</param>
    protected void ProcessBeginTag( Match match )
    {
      string tagName = match.Groups["tagName"].Value;
      bool selfClosed = match.Groups["selfClosed"].Success;

      if ( HtmlSpecification.selfCloseTags.Contains( tagName, StringComparer.InvariantCultureIgnoreCase ) )
        selfClosed = true;


      //检查父标签是否可选结束标记，并相应处理
      {
        var element = CurrentContainer as IHtmlElement;
        if ( element != null && HtmlSpecification.optionalCloseTags.Contains( element.Name, StringComparer.InvariantCultureIgnoreCase ) )
        {
          if ( ImmediatelyClose( tagName, element ) )
            ContainerStack.Pop();
        }
      }



      //处理所有属性
      var attributes = new Dictionary<string, string>();

      foreach ( Capture capture in match.Groups["attribute"].Captures )
      {
        string name = capture.FindCaptures( match.Groups["attrName"] ).Single().Value;
        string value = capture.FindCaptures( match.Groups["attrValue"] ).Select( c => c.Value ).SingleOrDefault();

        value = HtmlEncoding.HtmlDecode( value );

        if ( attributes.ContainsKey( name ) )
          continue;

        attributes.Add( name, value );
      }


      //创建元素
      {
        var element = CreateElement( tagName, attributes );


        //加入容器堆栈
        if ( !selfClosed )
          ContainerStack.Push( element );
      }
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
      return Provider.AddElement( CurrentContainer, CurrentContainer.Nodes().Count(), tagName, attributes );
    }



    /// <summary>
    /// 处理结束标签
    /// </summary>
    /// <param name="match">结束标签的匹配</param>
    protected void ProcessEndTag( Match match )
    {
      string tagName = match.Groups["tagName"].Value;



      if ( ContainerStack.OfType<DomElement>().Select( e => e.Name ).Contains( tagName, StringComparer.InvariantCultureIgnoreCase ) )
      {
        while ( true )
        {
          var element = ContainerStack.Pop() as DomElement;
          if ( element.Name.Equals( tagName, StringComparison.InvariantCultureIgnoreCase ) )
            break;
        }
      }
      else
      {
        ProcessEndTagMissingBeginTag( match );
      }
    }

    /// <summary>
    /// 处理丢失了开始标签的结束标签
    /// </summary>
    /// <param name="match">结束标签的匹配</param>
    protected virtual void ProcessEndTagMissingBeginTag( Match match )
    {
      //如果堆栈中没有对应的开始标签，则将这个结束标签解释为文本
      CreateTextNode( match.Value );
    }





    /// <summary>
    /// 处理 HTML 注释
    /// </summary>
    /// <param name="match">HTML 注释匹配</param>
    protected void ProcessComment( Match match )
    {
      CreateCommet( match.Groups["commentText"].Value );
    }

    /// <summary>
    /// 创建注释节点并加入当前容器
    /// </summary>
    /// <param name="comment">注释内容</param>
    /// <returns>创建的注释节点</returns>
    protected virtual IHtmlComment CreateCommet( string comment )
    {
      return Provider.AddComment( CurrentContainer, CurrentContainer.Nodes().Count(), comment );
    }



    /// <summary>
    /// 处理特殊节点
    /// </summary>
    /// <param name="match">特殊节点的匹配</param>
    protected IHtmlSpecial ProcessSpecial( Match match )
    {
      return null;//不处理该节点（将其从DOM树中抹去）。
    }



  }
}
