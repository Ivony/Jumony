using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ivony.Html.Parser
{

  /// <summary>
  /// IHtmlComment 的实现
  /// </summary>
  public class DomComment : DomNode, IHtmlComment
  {

    private readonly string _comment;

    /// <summary>
    /// 创建 DomComment 实例
    /// </summary>
    /// <param name="comment">注释内容</param>
    public DomComment( string comment )
    {
     
      if ( comment.Contains( "-->" ) )
        throw new ArgumentException( "注释文本中不能包含注释结束符", "comment" );
      
      _comment = comment;
    }

    /// <summary>
    /// 获取一个名称，用于在抛出 ObjectDisposedException 异常时说明自己
    /// </summary>
    protected override string ObjectName
    {
      get { return "CommentNode"; }
    }

    /// <summary>
    /// 获取注释内容
    /// </summary>
    public string Comment
    {
      get { return _comment; }
    }
  }


}
