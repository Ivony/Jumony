using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Compilation;
using System.Web;
using System.CodeDom;
using System.Text.RegularExpressions;
using Ivony.Html.Parser;
using System.Collections;

namespace Ivony.Html.Web
{

  /// <summary>
  /// 将 HTML 文件编译成 IHtmlDocumentProvider 对象的 BuildProvider
  /// </summary>
  public class HtmlBuildProvider : BuildProvider
  {

    /// <summary>
    /// 此方法已被重写以产生代码
    /// </summary>
    /// <param name="assemblyBuilder">程序集构建器</param>
    public override void GenerateCode( AssemblyBuilder assemblyBuilder )
    {

      if ( _type == null )
      {
        _type = CreateDocumentProviderType();
      }

      var @namespace = new CodeNamespace( "HTML" );
      @namespace.Types.Add( _type );

      var unit = new CodeCompileUnit();
      unit.Namespaces.Add( @namespace );

      assemblyBuilder.AddCodeCompileUnit( this, unit );

      AddReferences( assemblyBuilder );

    }

    
    /// <summary>
    /// 添加依赖项
    /// </summary>
    /// <param name="assemblyBuilder"></param>
    protected virtual void AddReferences( AssemblyBuilder assemblyBuilder )
    {
      var buildTypeAssembly = GetType().Assembly;
      var domProviderTypeAssembly = GetDomProviderType().Assembly;


      assemblyBuilder.AddAssemblyReference( buildTypeAssembly );
      if ( buildTypeAssembly != domProviderTypeAssembly )
        assemblyBuilder.AddAssemblyReference( domProviderTypeAssembly );
    }


    /// <summary>
    /// 此属性已被重写以获取默认编译设置
    /// </summary>
    public override CompilerType CodeCompilerType
    {
      get
      {
        return GetDefaultCompilerTypeForLanguage( "C#" );
      }
    }


    /// <summary>
    /// 此方法已被重写以获取编译后的类型
    /// </summary>
    /// <param name="results"></param>
    /// <returns></returns>
    public override Type GetGeneratedType( System.CodeDom.Compiler.CompilerResults results )
    {
      var typeName = CreateName();
      return results.CompiledAssembly.GetType( "HTML." + typeName );
    }


    private CodeTypeDeclaration _type;

    /// <summary>
    /// 创建 IDocumentProvider 实现类
    /// </summary>
    /// <returns></returns>
    protected virtual CodeTypeDeclaration CreateDocumentProviderType()
    {
      var parser = GetParser();
      IHtmlDocument document;
      using ( var reader = OpenReader() )
      {
        document = parser.Parse( reader.ReadToEnd() );
      }


      var name = CreateName();
      var type = new CodeTypeDeclaration( name );
      type.BaseTypes.Add( new CodeTypeReference( typeof( IHtmlDocumentProvider ) ) );

      type.Members.Add( document.GenerateCodeMethod( "CreateDocument" ) );

      var createDocument = new CodeMemberMethod();
      createDocument.Name = "CreateDocument";
      createDocument.ReturnType = new CodeTypeReference( typeof( IHtmlDocument ) );

      var createDomProvider = new CodeObjectCreateExpression( new CodeTypeReference( GetDomProviderType() ) );
      var invokeCreateDocument = new CodeMethodInvokeExpression( new CodeMethodReferenceExpression( null, "CreateDocument" ), createDomProvider );
      createDocument.Statements.Add( new CodeMethodReturnStatement( invokeCreateDocument ) );

      createDocument.PrivateImplementationType = new CodeTypeReference( typeof( IHtmlDocumentProvider ) );

      type.Members.Add( createDocument );
      return type;
    }


    /// <summary>
    /// 创建类型名称
    /// </summary>
    /// <returns></returns>
    protected virtual string CreateName()
    {
      return Regex.Replace( VirtualPath, @"\W", "_" );
    }


    /// <summary>
    /// 获取解析器
    /// </summary>
    /// <returns></returns>
    protected virtual IHtmlParser GetParser()
    {
      return new WebParser();
    }


    /// <summary>
    /// 获取 HTML Dom 提供程序
    /// </summary>
    /// <returns></returns>
    protected virtual Type GetDomProviderType()
    {
      return typeof( DomProvider );
    }


  }
}
