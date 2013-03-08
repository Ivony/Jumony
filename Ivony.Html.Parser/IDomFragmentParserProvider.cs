using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ivony.Html.Parser
{
  
  /// <summary>
  /// 定义 JumonyParser DOM 模型的文档碎片解析器提供程序
  /// </summary>
  /// <remarks>JumonyParser 实现此接口并将自身实例作为碎片解析器提供</remarks>
  public interface IDomFragmentParserProvider
  {
    IDomFragmentParser GetFragmentParser( DomDocument document );
  }


  /// <summary>
  /// 定义 JumonyParser DOM 模型的文档碎片解析器
  /// </summary>
  /// <remarks>JumonyParser 已实现此接口作为碎片解析器</remarks>
  public interface IDomFragmentParser
  {
    void ParseToFragment( string html, DomFragment fragment );
  }
}
