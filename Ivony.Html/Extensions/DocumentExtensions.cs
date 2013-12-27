using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using Ivony.Fluent;


namespace Ivony.Html
{
  /// <summary>
  /// 提供应用于 Document 特有的扩展方法
  /// </summary>
  public static class DocumentExtensions
  {

    /// <summary>
    /// 在文档中通过ID来查找元素
    /// </summary>
    /// <param name="document">要查找元素的文档</param>
    /// <param name="id">元素ID</param>
    /// <returns>找到的元素，没有符合要求的则返回null</returns>
    /// <exception cref="System.InvalidOperationException">找到多个ID相同的元素</exception>
    public static IHtmlElement GetElementById( this IHtmlDocument document, string id )
    {
      if ( document == null )
        throw new ArgumentNullException( "document" );
      if ( id == null )
        throw new ArgumentNullException( "id" );

      return AllElements( document ).SingleOrDefault( element => element.Attribute( "id" ).Value() == id );
    }


    /// <summary>
    /// 返回元素的唯一ID，如果没有ID属性，或者有但非唯一，返回null
    /// </summary>
    /// <param name="element">要标识的元素</param>
    /// <returns>元素的唯一ID。</returns>
    public static string Identity( this IHtmlElement element )
    {
      return Identity( element, false );
    }

    /// <summary>
    /// 返回元素的唯一ID，如果没有ID属性，或者有但非唯一，返回null
    /// </summary>
    /// <param name="element">要标识的元素</param>
    /// <param name="create">指示当没有唯一ID时是否创建一个</param>
    /// <returns>元素的唯一ID。</returns>
    public static string Identity( this IHtmlElement element, bool create )
    {
      return Identity( element, create, false );
    }

    /// <summary>
    /// 返回元素的唯一ID，没有ID属性，或者有但非唯一，返回null
    /// </summary>
    /// <param name="element">要标识的元素</param>
    /// <param name="create">指示当没有唯一ID时是否创建一个</param>
    /// <param name="ancestorsCreate">在创建ID的过程中，是否为没有唯一ID的父级也创建ID</param>
    /// <returns>元素的唯一ID。</returns>
    public static string Identity( this IHtmlElement element, bool create, bool ancestorsCreate )
    {

      if ( element == null )
        throw new ArgumentNullException( "element" );


      EnsureAllocated( element );

      var id = element.Attribute( "id" ).Value();

      if ( string.IsNullOrEmpty( id ) || !element.Document.AllElements().Where( e => e.Attribute( "id" ).Value() == id ).IsSingle() )
        id = null;

      if ( create && id == null )
        element.SetAttribute( "id", id = CreateIdentity( element, ancestorsCreate ) );


      return id;
    }


    /// <summary>
    /// 获取元素的唯一标识
    /// </summary>
    /// <param name="element">要获取标识的元素</param>
    /// <returns>唯一标识</returns>
    /// <remarks>
    /// 元素的唯一标识仅在文档结构不被修改时唯一，当文档结构变化时，元素的唯一标识将会改变，也不能确保唯一性
    /// </remarks>
    public static string Unique( this IHtmlElement element )
    {
      var id = element.Identity();

      if ( id == null )
        return CreateIdentity( element, false );
      else
        return id;
    }

    private static string CreateIdentity( IHtmlElement element, bool ancestorsCreate )
    {
      string parentId;

      var parentElement = element.Parent();
      if ( parentElement != null )
      {
        parentId = Identity( parentElement );
        if ( parentId == null )
        {
          if ( ancestorsCreate )
            parentId = Identity( parentElement, true, true );
          else
            parentId = CreateIdentity( parentElement, false );
        }
      }
      else
      {
        if ( element.Container is IHtmlDocument )
          parentId = null;
        else
          throw new InvalidOperationException();
      }

      var name = GetElementName( element );

      var builder = new StringBuilder();

      if ( !string.IsNullOrEmpty( parentId ) )
        builder.AppendFormat( "{0}_", parentId );

      builder.Append( name );

      if ( element.Siblings().Where( e => GetElementName( e ).EqualsIgnoreCase( GetElementName( element ) ) ).IsSingle() )
        return builder.ToString();



      var index = element.SiblingsBeforeSelf().Count( e => GetElementName( e ).EqualsIgnoreCase( GetElementName( element ) ) );

      builder.Append( index + 1 );

      var identity = builder.ToString();

      return EnsureUniqueness( identity, element.Document );
    }

    private static string EnsureUniqueness( string identity, IHtmlDocument document )
    {
      return EnsureUniqueness( identity, document.Descendants().Select( element => element.Attribute( "id" ).Value() ).NotNull() );
    }

    private static string EnsureUniqueness( string identity, IEnumerable<string> ExistsIdentities )
    {
      var id = identity;
      var postfix = 0;

      while ( ExistsIdentities.Any( _id => _id == id ) )
        id = identity + "_" + postfix++;

      return id;
    }


    /// <summary>
    /// 确保节点是已分配在一个固定的文档上（未从 DOM 移除，也不是以碎片形式存在）
    /// </summary>
    /// <param name="node">要检查的节点</param>
    /// <exception cref="System.InvalidOperationException">如果节点没有被分配在一个固定的文档。</exception>
    public static void EnsureAllocated( this IHtmlNode node )
    {

      if ( node == null )
        throw new ArgumentNullException( "node" );

      if ( !IsAllocated( node ) )
        throw new InvalidOperationException( "无法对没有被分配在文档上的元素或节点进行操作" );

    }


    /// <summary>
    /// 确定节点被分配在一个固定的文档上（未从 DOM 移除，也不是以碎片形式存在）
    /// </summary>
    /// <param name="node">要确定的节点</param>
    /// <returns>是否被分配在一个固定的文档上</returns>
    public static bool IsAllocated( this IHtmlNode node )
    {

      if ( node == null )
        throw new ArgumentNullException( "node" );


      if ( node.Container == null )
        return false;

      if ( node.Container is IHtmlDocument )
        return true;

      if ( node.Container is IHtmlFragment )
        return false;

      node = node.Container as IHtmlNode;
      if ( node == null )
        return false;

      return IsAllocated( node );

    }



    private static string GetElementName( IHtmlElement element )
    {

      switch ( element.Name.ToLowerInvariant() )
      {

        case "html":
        //case "head":
        case "body":
          return null;

        case "a":
          if ( element.Attribute( "name" ) != null )
            return "link";
          else
            return "anchor";

        case "li":
          return "item";

        case "ul":
        case "ol":
          return "list";

        case "h1":
        case "h2":
        case "h3":
        case "h4":
        case "h5":
        case "h6":
          return "header";

        default:
          return element.Name;

      }
    }


    /// <summary>
    /// 返回文档所有节点，包括已分配和游离的
    /// </summary>
    /// <param name="document">查找节点的文档</param>
    /// <returns>文档的所有节点</returns>
    public static IEnumerable<IHtmlNode> AllNodes( this IHtmlDocument document )
    {
      if ( document == null )
        throw new ArgumentNullException( "document" );


      var nodes = new HashSet<IHtmlNode>();

      nodes.UnionWith( document.DescendantNodes() );

      var manager = document.FragmentManager;
      if ( manager != null )
        nodes.UnionWith( manager.AllFragments.SelectMany( fragment => fragment.DescendantNodes() ) );

      return nodes;
    }



    /// <summary>
    /// 返回文档所有元素，包括已分配和游离的
    /// </summary>
    /// <param name="document">查找元素的文档</param>
    /// <returns>文档的所有元素</returns>
    public static IEnumerable<IHtmlElement> AllElements( this IHtmlDocument document )
    {
      if ( document == null )
        throw new ArgumentNullException( "document" );

      return AllNodes( document ).OfType<IHtmlElement>();
    }



    /// <summary>
    /// 将文档中所有的uri属性转换为绝对的uri。
    /// </summary>
    /// <param name="document">要执行转换的文档</param>
    public static void ResolveUriToAbsoluate( this IHtmlDocument document )
    {
      ResolveUriToAbsoluate( document, false );
    }


    /// <summary>
    /// 将文档中所有的uri属性转换为绝对的uri。
    /// </summary>
    /// <param name="document">要执行转换的文档</param>
    /// <param name="resolveInternalReference">是否转换页内 uri</param>
    public static void ResolveUriToAbsoluate( this IHtmlDocument document, bool resolveInternalReference )
    {
      if ( document == null )
        throw new ArgumentNullException( "document" );


      var baseUri = document.DocumentUri;

      if ( baseUri == null )
        throw new InvalidOperationException();

      foreach ( var attribute in document.AllElements().SelectMany( e => e.Attributes() ).Where( a => document.HtmlSpecification.IsUriValue( a ) ).ToArray() )
      {

        var value = attribute.Value();
        if ( string.IsNullOrEmpty( value ) )
          continue;

        Uri uri;

        if ( !Uri.TryCreate( baseUri, value, out uri ) )
          continue;

        if ( uri.Equals( baseUri ) && !resolveInternalReference )
          continue;


        attribute.Element.SetAttribute( attribute.Name, uri.AbsoluteUri );

      }

    }



    /// <summary>
    /// 获取当前节点的文本模式
    /// </summary>
    /// <param name="node">要获取文本模式的节点</param>
    /// <returns>节点当前适用的文本模式</returns>
    public static TextMode ElementTextMode( this IHtmlNode node )
    {
      if ( node == null )
        throw new ArgumentNullException( "node" );

      var specification = node.Document.HtmlSpecification;
      var element = node as IHtmlElement ?? node.Parent();

      if ( element == null )
        return TextMode.Normal;

      return specification.ElementTextMode( element );
    }







    /// <summary>
    /// 根据文档结构产生一个方法，文档结构可以由此方法的执行结果复原
    /// </summary>
    /// <param name="document">要编译的文档</param>
    /// <param name="methodName">产生的方法名称</param>
    /// <returns>可以复原文档的方法代码</returns>
    public static CodeMemberMethod GenerateCodeMethod( this IHtmlDocument document, string methodName )
    {
      return CodeGenerator.GenerateCodeMethod( document, methodName );
    }

    private static class CodeGenerator
    {



      public static CodeMemberMethod GenerateCodeMethod( IHtmlDocument document, string methodName )
      {
        var constructor = new CodeMemberMethod();
        constructor.Name = methodName;
        constructor.Attributes = MemberAttributes.Public | MemberAttributes.Static;

        constructor.Parameters.Add( new CodeParameterDeclarationExpression( typeof( IHtmlDomProvider ), "provider" ) );
        constructor.ReturnType = new CodeTypeReference( typeof( IHtmlDocument ) );

        var providerVariable = new CodeVariableReferenceExpression( "provider" );


        CodeExpression urlExpression;

        if ( document.DocumentUri != null )
          urlExpression = new CodeObjectCreateExpression( typeof( Uri ), new CodePrimitiveExpression( document.DocumentUri.AbsoluteUri ) );
        else
          urlExpression = new CodePrimitiveExpression( null );

        constructor.Statements.Add( new CodeVariableDeclarationStatement( typeof( IHtmlDocument ), "document", new CodeMethodInvokeExpression( providerVariable, "CreateDocument", urlExpression ) ) );//var document = provider.CreateDocument();

        var documentVariable = new CodeVariableReferenceExpression( "document" );
        
        constructor.Statements.Add( new CodeMethodInvokeExpression( providerVariable, "SetHtmlSpecification", documentVariable, new CodePrimitiveExpression( document.HtmlSpecification.ToString() ) ) );// provider.SetHtmlSpecification( document, spec );

        constructor.Statements.Add( new CodeVariableDeclarationStatement( typeof( IDictionary<string, string> ), "attributes" ) );//var attributes

        BuildChildNodesStatement( document, documentVariable, constructor.Statements, new List<string>() );//build document

        constructor.Statements.Add( new CodeMethodReturnStatement( new CodeMethodInvokeExpression( providerVariable, "CompleteDocument", documentVariable ) ) );

        return constructor;
      }

      private static void BuildChildNodesStatement( IHtmlContainer container, CodeVariableReferenceExpression containerVariable, CodeStatementCollection statements, IList<string> existsElements )
      {


        var providerVariable = new CodeVariableReferenceExpression( "provider" );
        var attributesVariable = new CodeVariableReferenceExpression( "attributes" );

        int index = 0;

        foreach ( var node in container.Nodes() )
        {

          var textNode = node as IHtmlTextNode;
          if ( textNode != null )
            statements.Add( new CodeMethodInvokeExpression( providerVariable, "AddTextNode", containerVariable, new CodePrimitiveExpression( textNode.HtmlText ) ) );


          var comment = node as IHtmlComment;
          if ( comment != null )
            statements.Add( new CodeMethodInvokeExpression( providerVariable, "AddComment", containerVariable, new CodePrimitiveExpression( comment.Comment ) ) );


          var element = node as IHtmlElement;

          if ( element != null )
          {
            var elementId = CreateIdentity( element, false );

            elementId = EnsureUniqueness( elementId, existsElements );
            existsElements.Add( elementId );

            elementId = "element_" + elementId;


            statements.Add( new CodeCommentStatement( ContentExtensions.GenerateTagHtml( element, false ) ) );

            statements.Add( new CodeAssignStatement( attributesVariable, new CodeObjectCreateExpression( typeof( Dictionary<string, string> ) ) ) );

            foreach ( var attribute in element.Attributes() )
              statements.Add( new CodeMethodInvokeExpression( attributesVariable, "Add", new CodePrimitiveExpression( attribute.Name ), new CodePrimitiveExpression( attribute.AttributeValue ) ) );

            statements.Add( new CodeVariableDeclarationStatement( typeof( IHtmlElement ), elementId, new CodeMethodInvokeExpression( providerVariable, "AddElement", containerVariable, new CodePrimitiveExpression( element.Name ), attributesVariable ) ) );

            var elementVariable = new CodeVariableReferenceExpression( elementId );


            BuildChildNodesStatement( element, elementVariable, statements, existsElements );

          }

          index++;
        }
      }

    }



    /// <summary>
    /// 将文档结构编译成一个方法，文档结构可以由此方法复原
    /// </summary>
    /// <param name="document">要编译的文档</param>
    /// <returns>复原文档的方法</returns>
    public static Func<IHtmlDomProvider, IHtmlDocument> Compile( this IHtmlDocument document )
    {
      return DocumentCompiler.Compile( document );
    }


    /// <summary>
    /// 文档编译器，负责将 HTML DOM 结构编译成代码
    /// </summary>
    private static class DocumentCompiler
    {


      /// <summary>
      /// 用于携带 DynamicMethod 实例的类型
      /// </summary>
      private class DynamicMethodHandler
      {

        private DynamicMethod _method;
        private Func<IHtmlDomProvider, IHtmlDocument> _delegate;
        public DynamicMethodHandler( DynamicMethod method )
        {
          _method = method;//避免 method 被 GC 回收
          _delegate = method.CreateDelegate( typeof( Func<IHtmlDomProvider, IHtmlDocument> ) ).CastTo<Func<IHtmlDomProvider, IHtmlDocument>>();
        }

        public IHtmlDocument Invoke( IHtmlDomProvider provider )
        {
          return _delegate( provider );
        }
      }


      public static Func<IHtmlDocument> Compile( IHtmlDocument document, IHtmlDomProvider provider )
      {
        var method = CompileDynamicMethod( document );
        return method.CreateDelegate( typeof( Func<IHtmlDocument> ), provider ).CastTo<Func<IHtmlDocument>>();

      }

      /// <summary>
      /// 将一个文档编译成一个方法
      /// </summary>
      /// <param name="document">要编译的文档</param>
      /// <returns>编译好的方法，文档可以透过此方法复原</returns>
      public static Func<IHtmlDomProvider, IHtmlDocument> Compile( IHtmlDocument document )
      {
        var method = CompileDynamicMethod( document );
        return new DynamicMethodHandler( method ).Invoke;
      }


      /// <summary>
      /// 将文档编译成一个动态方法，为下一步转换成委托做准备。
      /// </summary>
      /// <param name="document">要编译的文档</param>
      /// <returns>编译好的动态方法</returns>
      private static DynamicMethod CompileDynamicMethod( IHtmlDocument document )
      {
        var method = new DynamicMethod( "", typeof( IHtmlDocument ), new[] { typeof( IHtmlDomProvider ) } );

        var il = method.GetILGenerator();


        il.DeclareLocal( typeof( IHtmlContainer ) );

        il.Emit( OpCodes.Ldarg_0 );                         //ld provider        provider
        il.Emit( OpCodes.Dup );                             //dup                provider provider


        EmitCreateDocument( il, document );                 //create document    provider document


        il.Emit( OpCodes.Callvirt, CompleteDocument );      //complete document  document


        il.Emit( OpCodes.Ret );

        return method;
      }


      /// <summary>
      /// 将文档碎片编译成一个动态方法，为下一步转换成委托做准备。
      /// </summary>
      /// <param name="fragment">要编译的文档碎片</param>
      /// <returns>编译好的动态方法</returns>
      private static DynamicMethod CompileDynamicMethod( IHtmlFragment fragment )
      {

        throw new NotImplementedException();

        var method = new DynamicMethod( "", typeof( IHtmlFragment ), new[] { typeof( IHtmlFragmentManager ) } );

        var il = method.GetILGenerator();


        il.DeclareLocal( typeof( IHtmlContainer ) );

        il.Emit( OpCodes.Ldarg_0 );                         //ld manager         manager


        EmitCreateFragment( il, fragment );                 //create fragment    manager fragment


        il.Emit( OpCodes.Callvirt, CompleteDocument );      //complete document  document


        il.Emit( OpCodes.Ret );

        return method;
      }

      private static void EmitCreateFragment( ILGenerator il, IHtmlFragment fragment )
      {

        throw new NotImplementedException();

        il.Emit( OpCodes.Callvirt, CreateFragment );        //create fragment manager fragment
      }



      private static void EmitCreateDocument( ILGenerator il, IHtmlDocument document )
      {
        //init           provider

        //CreateDocument document
        //dup            document, document   
        //st container   document

        //ld provider    document, provider
        //ld container   document, provider, document
        //ld spec        document, provider, document, specS
        //SetHtmlSpecifi document, spec
        //pop            document



        //begin create element

        //ld provider    document, provider
        //ld container   document, provider, document
        //ld ElementName document, provider, document, name
        //create element document, element

        //dup            document, element, element   
        //st container   document, element

        //begin create textNode

        //ld provider    document, element, provider
        //ld container   document, element, provider, element
        //ld text        document, element, provider, element, text
        //create text    document, element, textNode
        //pop            document, element

        //end create textNode

        //end create element
        //pop            document
        //dup            document, document
        //st container   document


        //end create document
        //complete   document
        //ret


        if ( document.DocumentUri == null )
          il.Emit( OpCodes.Ldnull );
        else
        {
          il.Emit( OpCodes.Ldstr, document.DocumentUri.AbsoluteUri );
          il.Emit( OpCodes.Newobj, NewUri );
        }


        il.Emit( OpCodes.Callvirt, CreateDocument );

        il.Emit( OpCodes.Dup );
        il.Emit( OpCodes.Stloc_0 );// set container;


        il.Emit( OpCodes.Ldarg_0 );
        il.Emit( OpCodes.Ldloc_0 );
        il.Emit( OpCodes.Castclass, typeof( IHtmlDocument ) );
        il.Emit( OpCodes.Ldstr, document.HtmlSpecification.ToString() );
        il.Emit( OpCodes.Callvirt, SetHtmlSpecification );
        il.Emit( OpCodes.Pop );



        foreach ( var node in document.Nodes() )
          EmitCreateNode( il, node );

      }

      private static void EmitCreateNode( ILGenerator il, IHtmlNode node )
      {

        il.Emit( OpCodes.Ldarg_0 );       //ld provider
        il.Emit( OpCodes.Ldloc_0 );       //ld container
        //        il.Emit( OpCodes.Ldc_I4, index ); //ld index

        var textNode = node as IHtmlTextNode;
        if ( textNode != null )
        {
          il.Emit( OpCodes.Ldstr, textNode.HtmlText );
          il.Emit( OpCodes.Callvirt, AddTextNode );
          il.Emit( OpCodes.Pop );
          return;
        }


        var comment = node as IHtmlComment;
        if ( comment != null )
        {
          il.Emit( OpCodes.Ldstr, comment.Comment );
          il.Emit( OpCodes.Callvirt, AddComment );
          il.Emit( OpCodes.Pop );
          return;
        }


        var element = node as IHtmlElement;
        if ( element != null )
        {
          il.Emit( OpCodes.Ldstr, element.Name );

          il.Emit( OpCodes.Newobj, NewDictionary );

          foreach ( IHtmlAttribute attribute in element.Attributes() )
          {
            il.Emit( OpCodes.Dup );

            il.Emit( OpCodes.Ldstr, attribute.Name );

            var value = attribute.AttributeValue;

            if ( value != null )
              il.Emit( OpCodes.Ldstr, value );
            else
              il.Emit( OpCodes.Ldnull );

            il.Emit( OpCodes.Callvirt, DictionaryAdd );
          }

          il.Emit( OpCodes.Callvirt, AddElement );

          il.Emit( OpCodes.Dup );
          il.Emit( OpCodes.Stloc_0 );// set container;

          foreach ( var childNode in element.Nodes() )
            EmitCreateNode( il, childNode );

          il.Emit( OpCodes.Pop );    //pop element
          il.Emit( OpCodes.Dup );
          il.Emit( OpCodes.Stloc_0 );//set container
          return;
        }
      }

      private static readonly ConstructorInfo NewUri = typeof( Uri ).GetConstructor( new[] { typeof( string ) } );
      private static readonly MethodInfo CreateDocument = typeof( IHtmlDomProvider ).GetMethod( "CreateDocument" );
      private static readonly MethodInfo SetHtmlSpecification = typeof( IHtmlDomProvider ).GetMethod( "SetHtmlSpecification" );
      private static readonly MethodInfo CreateFragment = typeof( IHtmlFragmentManager ).GetMethod( "CreateFragment" );
      private static readonly MethodInfo AddTextNode = typeof( IHtmlDomProvider ).GetMethod( "AddTextNode" );
      private static readonly MethodInfo AddComment = typeof( IHtmlDomProvider ).GetMethod( "AddComment" );
      private static readonly MethodInfo AddElement = typeof( IHtmlDomProvider ).GetMethod( "AddElement" );
      private static readonly MethodInfo CompleteDocument = typeof( IHtmlDomProvider ).GetMethod( "CompleteDocument" );
      private static readonly ConstructorInfo NewDictionary = typeof( Dictionary<string, string> ).GetConstructor( new Type[0] );
      private static readonly MethodInfo DictionaryAdd = typeof( IDictionary<string, string> ).GetMethod( "Add" );
    }






  }
}
