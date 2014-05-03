using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Ivony.Parser
{


  /// <summary>
  /// 协助实现 Tokenizer 的基类
  /// </summary>
  public abstract class TokenizerBase
  {


    /// <summary>
    /// 用于匹配 CName 的正则表达式
    /// </summary>
    protected static readonly Regex CName = new Regex( @"\G[a-zA-z_][a-zA-Z_0-9]*", RegexOptions.Compiled | RegexOptions.CultureInvariant );

    /// <summary>
    /// 用于匹配空白字符的正则表达式
    /// </summary>
    protected static readonly Regex WhiteSpace = new Regex( @"\G\s*", RegexOptions.Compiled | RegexOptions.CultureInvariant );



    /// <summary>
    /// 字符串扫描器
    /// </summary>
    protected TextScaner Scaner
    {
      get;
      private set;
    }


    private object _sync = new object();


    /// <summary>
    /// 获取用于同步的对象
    /// </summary>
    public virtual object SyncRoot
    {
      get { return _sync; }
    }


    /// <summary>
    /// 初始化 Tokenizer
    /// </summary>
    /// <param name="text">要分析的字符串</param>
    /// <param name="index">要开始分析的位置</param>
    protected void Initialize( string text, int index = 0 )
    {
      Scaner = new TextScaner( SyncRoot, text, index );
    }



    /// <summary>
    /// 在扫描器当前位置是否为指定的字符，若匹配到指定字符，则推移扫描器指针
    /// </summary>
    /// <param name="ch">指定的字符</param>
    /// <returns>当前位置是否为指定的字符</returns>
    public char? Match( char ch )
    {
      if ( Scaner.IsEnd )
        return null;

      if ( Scaner.Current != ch )
        return null;

      Scaner.MoveNext();
      return ch;
    }



    /// <summary>
    /// 判断扫描器当前位置是否为指定的字符
    /// </summary>
    /// <param name="ch">指定的字符</param>
    /// <returns>当前位置是否为指定的字符</returns>
    public bool IsMatch( char ch )
    {

      if ( Scaner.IsEnd )
        return false;

      if ( Scaner.Current != ch )
        return false;


      return true;
    }


    /// <summary>
    /// 确定扫描器当前位置必须为指定的字符
    /// </summary>
    /// <param name="ch">指定的字符</param>
    public void EnsureMatch( char ch )
    {

      if ( !IsMatch( ch ) )
        throw FormatError( ch );
    }


    /// <summary>
    /// 判断扫描器当前位置是否为指定的字符
    /// </summary>
    /// <param name="chars">指定的字符</param>
    /// <returns>当前位置是否为指定的字符</returns>
    public bool IsMatchAny( params char[] chars )
    {
      if ( Scaner.IsEnd )
        return false;

      var current = Scaner.Current;
      return chars.Any( ch => ch == current );
    }


    /// <summary>
    /// 确定扫描器当前位置必须为指定的字符
    /// </summary>
    /// <param name="chars">指定的字符</param>
    public void EnsureMatchAny( params char[] chars )
    {

      if ( !IsMatchAny( chars ) )
        throw FormatError();
    }


    /// <summary>
    /// 在扫描器当前位置进行正则匹配。
    /// </summary>
    /// <param name="regularExpression">正则表达式</param>
    /// <returns>若匹配成功，返回匹配对象，否则返回null。</returns>
    public Match Match( Regex regularExpression )
    {
      var match = regularExpression.Match( Scaner.ToString(), Scaner.Offset );
      if ( match.Success )
      {
        if ( match.Index != Scaner.Offset )
          return null;

        Scaner.Skip( match.Length );
        return match;
      }

      else
        return null;
    }

    /// <summary>
    /// 确认在扫描器当前位置是否满足正则匹配
    /// </summary>
    /// <param name="regularExpression">正则表达式</param>
    /// <returns>当前位置是否满足正则匹配</returns>
    public bool IsMatch( Regex regularExpression )
    {
      var match = regularExpression.Match( Scaner.ToString(), Scaner.Offset );
      if ( match.Success && match.Index == Scaner.Offset )
        return true;

      return false;

    }


    /// <summary>
    /// 确保在扫描器当前位置必须满足正则匹配
    /// </summary>
    /// <param name="regularExpression">正则表达式</param>
    /// <param name="description">关于该匹配的描述</param>
    /// <returns>当前位置是否满足正则匹配</returns>
    public Match EnsureMatch( Regex regularExpression, string description = null )
    {
      var match = Match( regularExpression );
      if ( match == null )
        throw FormatError( description );

      return match;
    }



    /// <summary>
    /// 构建一个字符串格式错误的异常，报告解析器在当前位置遇到格式错误
    /// </summary>
    /// <returns>异常信息</returns>
    protected FormatException FormatError()
    {

      string message;
      if ( Scaner.IsEnd )
        message = string.Format( CultureInfo.InvariantCulture, "意外遇到字符串结束，在分析字符串 \"{0}\" 时。", Scaner.ToString() );

      else
        message = string.Format( CultureInfo.InvariantCulture, "意外的字符 '{0}' ，在分析字符串 \"{1}\" 第 {2} 字符处。", Scaner.Current, Scaner.ToString(), Scaner.Offset );

      return new FormatException( message );
    }



    /// <summary>
    /// 构建一个字符串格式错误的异常，报告解析器在当前位置遇到格式错误
    /// </summary>
    /// <param name="desired">在当前位置期望的字符</param>
    /// <returns>异常信息</returns>
    protected FormatException FormatError( char desired )
    {
      string message;
      if ( Scaner.IsEnd )
        message = string.Format( CultureInfo.InvariantCulture, "意外遇到字符串结束，在分析字符串 \"{0}\" 时，期望的字符为 '{1}'。", Scaner.ToString(), desired );

      else
        message = string.Format( CultureInfo.InvariantCulture, "意外的字符 '{0}' ，在分析字符串 \"{1}\" 第 {2} 字符处，期望的字符为 '{3}' 。", Scaner.Current, Scaner.ToString(), Scaner.Offset, desired );


      return new FormatException( message );
    }

    /// <summary>
    /// 构建一个字符串格式错误的异常，报告解析器在当前位置遇到格式错误
    /// </summary>
    /// <param name="description">在当前位置期望遇到的匹配的描述</param>
    /// <returns>异常信息</returns>
    protected FormatException FormatError( string description )
    {
      string message;
      if ( Scaner.IsEnd )
        message = string.Format( CultureInfo.InvariantCulture, "意外遇到字符串结束，在分析字符串 \"{0}\" 时，期望的表达式为{1}。", Scaner.ToString(), description );

      else
        message = string.Format( CultureInfo.InvariantCulture, "意外的字符 '{0}' ，在分析字符串 \"{1}\" 第 {2} 字符处，期望的表达式为 {3} 。", Scaner.Current, Scaner.ToString(), Scaner.Offset, description );

      return new FormatException( message );
    }




    /// <summary>
    /// 定义一个字符枚举器
    /// </summary>
    protected class TextScaner : IEnumerator<char>
    {


      private string _text;

      private int _initializeIndex;

      private int _index;


      /// <summary>
      /// 构建 TextScaner 对象
      /// </summary>
      /// <param name="syncRoot">用于同步的对象</param>
      /// <param name="text">要扫描的字符串</param>
      public TextScaner( object syncRoot, string text ) : this( syncRoot, text, 0 ) { }

      /// <summary>
      /// 构建 TextScaner 对象
      /// </summary>
      /// <param name="syncRoot">用于同步的对象</param>
      /// <param name="text">要扫描的字符串</param>
      /// <param name="index">开始扫描的位置，默认为 0 </param>
      public TextScaner( object syncRoot, string text, int index )
      {
        SyncRoot = syncRoot;
        _text = text;
        _initializeIndex = _index = index;
      }



      /// <summary>
      /// 获取当前索引位置的字符
      /// </summary>
      public char Current
      {
        get
        {
          lock ( SyncRoot )
          {
            if ( _index == _text.Length )
              throw new InvalidOperationException();

            return _text[_index];
          }
        }
      }


      /// <summary>
      /// 销毁当前对象，释放所有资源
      /// </summary>
      public void Dispose()
      {
      }


      object System.Collections.IEnumerator.Current
      {
        get { return Current; }
      }


      /// <summary>
      /// 将索引移动到下一个字符，若已经到字符串末尾，则返回 false ， 否则返回 true
      /// </summary>
      /// <returns>是否到达字符串末尾</returns>
      public bool MoveNext()
      {
        lock ( SyncRoot )
        {
          _index++;
          if ( _index < _text.Length )
            return true;

          if ( _index > _text.Length )
            _index = _text.Length;
          return false;
        }
      }


      /// <summary>
      /// 重置索引到构建对象时传入的位置
      /// </summary>
      public void Reset()
      {
        lock ( SyncRoot )
        {
          _index = _initializeIndex;
        }
      }


      /// <summary>
      /// 当前索引位置
      /// </summary>
      public int Offset
      {
        get { return _index; }
      }



      /// <summary>
      /// 判断当前是指针否已经达到字符串的末尾
      /// </summary>
      public bool IsEnd
      {
        get
        {
          lock ( SyncRoot )
          {
            return _index >= _text.Length;
          }
        }
      }


      /// <summary>
      /// 从扫描的字符串中截取一段
      /// </summary>
      /// <param name="offset">开始位置</param>
      /// <param name="length">要截取的长度</param>
      /// <returns>截取的子字符串</returns>
      public string SubString( int offset, int length )
      {
        return _text.Substring( offset, length );
      }


      /// <summary>
      /// 获取当前扫描的字符串
      /// </summary>
      /// <returns></returns>
      public override string ToString()
      {
        return _text;
      }


      /// <summary>
      /// 跳过指定数量的字符
      /// </summary>
      /// <param name="length">要跳过的字符长度</param>
      public void Skip( int length )
      {
        lock ( SyncRoot )
        {
          _index += length;
        }
      }


      /// <summary>
      /// 获取用于同步的对象
      /// </summary>
      public object SyncRoot { get; private set; }
    }


  }
}
