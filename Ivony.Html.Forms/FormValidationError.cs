using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ivony.Html.Forms
{

  /// <summary>
  /// 定义表单验证错误
  /// </summary>
  public class FormValidationError
  {


    /// <summary>
    /// 创建 FormValidationError 对象
    /// </summary>
    /// <param name="name">字段名</param>
    /// <param name="messages">错误消息</param>
    public FormValidationError( string name, params string[] messages )
    {

      if ( name == null )
        throw new ArgumentNullException( "name" );

      if ( messages == null )
        throw new ArgumentNullException( "messages" );

      if ( !messages.Any() )
        throw new ArgumentException( "必须包含一条错误信息", "messages" );

      if ( messages.Any( m => string.IsNullOrEmpty( m ) ) )
        throw new ArgumentException( "不允许存在空的错误信息", "messages" );


      Name = name;
      Messages = messages;
    }


    /// <summary>
    /// 字段名称
    /// </summary>
    public string Name { get; private set; }


    /// <summary>
    /// 错误消息
    /// </summary>
    public string[] Messages { get; private set; }

  }
}
