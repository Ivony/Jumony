using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Collections;
using System.ComponentModel;
using System.Web;

namespace Ivony.Web.Html
{


  /// <summary>
  /// 保存数据绑定操作的上下文
  /// </summary>
  public class HtmlBindingContext : IDisposable
  {

    private HtmlBindingActionCollection _actionList = new HtmlBindingActionCollection();


    /// <summary>
    /// 保存的所有绑定操作
    /// </summary>
    protected HtmlBindingActionCollection Actions
    {
      get { return _actionList; }
    }


    /// <summary>
    /// 添加一个绑定操作
    /// </summary>
    /// <typeparam name="T">绑定对象类型</typeparam>
    /// <param name="target">绑定对象</param>
    /// <param name="action">绑定操作方法</param>
    public void Action<T>( T target, Action<T> action ) where T : IHtmlElement
    {
      _actionList.Add( new HtmlBindingAction<T>()
      {
        Target = target,
        Action = action
      } );
    }

    /// <summary>
    /// 添加一个绑定操作
    /// </summary>
    /// <typeparam name="T">绑定对象类型</typeparam>
    /// <param name="action">一个 BindAction 类型的对象，他保存了执行绑定操作所需的数据</param>
    public void Action<T>( HtmlBindingAction<T> action ) where T : IHtmlElement
    {
      _actionList.Add( action );
    }



    private Hashtable _dataContexts = new Hashtable();

    /// <summary>
    /// 为一个容器对象设置数据上下文
    /// </summary>
    /// <param name="container">要设置数据上下文的容器对象</param>
    /// <param name="dataContext">数据上下文</param>
    internal void SetDataContext( IHtmlContainer container, object dataContext )
    {
      _dataContexts[container] = dataContext;
    }

    /// <summary>
    /// 获取指定节点或元素最近的数据上下文
    /// </summary>
    /// <returns>最近的数据上下文，将递归查找父级和父级上下文，如果都没找到则返回null</returns>
    internal object GetDataContext( IHtmlNode node )
    {

      object data = GetDataContextCore( node );
      if ( data != null )
        return data;

      IHtmlContainer container;
      while ( (container = node.Parent) != null )
      {
        data = GetDataContextCore( container );
        if ( data != null )
          return data;

        node = container;
      }

      return null;
    }


    /// <summary>
    /// 查找指定节点的数据上下文，将递归向父级BindingContext查询
    /// </summary>
    /// <param name="node">指定节点</param>
    /// <returns>查找到的数据上下文，如果没有找到则返回null</returns>
    private object GetDataContextCore( IHtmlNode node )
    {
      object data = _dataContexts[node];

      if ( data == null && AboveContext != null )
        return AboveContext.GetDataContextCore( node );

      return data;
    }



    /// <summary>
    /// 后序遍历所有元素用于绑定
    /// </summary>
    /// <param name="container">根元素</param>
    /// <returns></returns>
    private static IEnumerable<IHtmlElement> PostOrderTraverse( IHtmlContainer container )
    {
      var elementList = new List<IHtmlElement>();

      PostOrderTraverse( container, elementList );
      return elementList;
    }


    /// <summary>
    /// 后序遍历所有元素，并将找到的元素放入集合中
    /// </summary>
    /// <param name="container">根元素</param>
    /// <param name="elementList">存放元素的集合</param>
    /// <returns></returns>
    private static void PostOrderTraverse( IHtmlContainer container, IList<IHtmlElement> elementList )
    {
      foreach ( var node in container.Nodes().OfType<IHtmlContainer>() )
      {
        PostOrderTraverse( node, elementList );

        IHtmlElement element = node as IHtmlElement;
        if ( element != null )
          elementList.Add( element );
      }
    }



    /// <summary>
    /// 绑定的范围
    /// </summary>
    public IHtmlContainer Scope
    {
      get;
      private set;
    }


    /// <summary>
    /// 绑定上下文的友好名称
    /// </summary>
    public string Name
    {
      get;
      private set;
    }

    private HtmlBindingContext( IHtmlContainer scope, string name, BindingContextExitBehavior exitBehavior )
    {
      Scope = scope;
      Name = name;
      _exitBehavior = exitBehavior;
    }

    /// <summary>
    /// 获取当前的绑定上下文
    /// </summary>
    public static HtmlBindingContext Current
    {
      get
      {
        return System.Runtime.Remoting.Messaging.CallContext.GetData( contextName ) as HtmlBindingContext;
      }
      private set
      {
        System.Runtime.Remoting.Messaging.CallContext.SetData( contextName, value );
      }
    }



    /// <summary>
    /// 上一级绑定上下文
    /// </summary>
    public HtmlBindingContext AboveContext
    {
      get;
      private set;
    }


    /// <summary>
    /// 下一级绑定上下文
    /// </summary>
    public HtmlBindingContext FollowContext
    {
      get;
      private set;
    }


    private const string contextName = "HtmlBindingContexts";


    private readonly BindingContextExitBehavior _exitBehavior;


    /// <summary>
    /// 创建并进入一个绑定上下文
    /// </summary>
    /// <param name="name">上下文友好的名称，用于Trace识别</param>
    /// <returns></returns>
    public static HtmlBindingContext Enter( string name )
    {
      return Enter( name, BindingContextExitBehavior.Default );
    }


    /// <summary>
    /// 创建并进入一个绑定上下文
    /// </summary>
    /// <param name="name">上下文友好的名称，用于Trace识别</param>
    /// <param name="exitBehavior">退出上下文时的行为</param>
    /// <returns></returns>
    public static HtmlBindingContext Enter( string name, BindingContextExitBehavior exitBehavior )
    {
      var currentContext = Current;
      if ( currentContext == null )
        throw new InvalidOperationException();

      return Enter( currentContext.Scope, name, exitBehavior );
    }

    /// <summary>
    /// 创建并进入一个绑定上下文
    /// </summary>
    /// <param name="scope">绑定范围，超出此范围的绑定都不会被提交</param>
    /// <param name="name">上下文友好的名称，用于Trace识别</param>
    /// <returns></returns>
    public static HtmlBindingContext Enter( IHtmlContainer scope, string name )
    {
      return Enter( scope, name, BindingContextExitBehavior.Default );
    }

    /// <summary>
    /// 创建并进入一个绑定上下文
    /// </summary>
    /// <param name="scope">绑定范围，超出此范围的绑定都不会被提交</param>
    /// <param name="name">上下文友好的名称，用于Trace识别</param>
    /// <param name="exitBehavior">退出上下文时的行为</param>
    /// <returns></returns>
    public static HtmlBindingContext Enter( IHtmlContainer scope, string name, BindingContextExitBehavior exitBehavior )
    {

      var currentContext = Current;

      if ( currentContext != null
           && !currentContext.Scope.Equals( scope )
           && !currentContext.Scope.DescendantNodes().Contains( scope )
        )
        throw new InvalidOperationException();

      var context = new HtmlBindingContext( scope, name, exitBehavior );
      context.EnterCore( exitBehavior );
      return context;
    }


    /// <summary>
    /// 进入上下文
    /// </summary>
    protected void EnterCore( BindingContextExitBehavior behavior )
    {

      var currentContext = Current;
      if ( currentContext != null )
      {
        this.AboveContext = currentContext;
        currentContext.FollowContext = this;
      }

      Current = this;

      HttpContext.Current.Trace.Write( "Binding", string.Format( "Enter BindingContext \"{0}\"", Name ) );
    }




    /// <summary>
    /// 退出当前上下文
    /// </summary>
    public static void Exit()
    {
      Current.ExitCore();
    }


    /// <summary>
    /// 退出当前上下文
    /// </summary>
    /// <param name="discard">是否放弃所有操作</param>
    public static void Exit( bool discard )
    {
      Current.ExitCore( discard );
    }



    /// <summary>
    /// 退出上下文
    /// </summary>
    private void ExitCore()
    {
      if ( FollowContext != null )
        FollowContext.ExitCore();

      switch ( _exitBehavior )
      {
        case BindingContextExitBehavior.Default:
        case BindingContextExitBehavior.Commit:
          Commit();
          ExitPrivate();
          return;

        case BindingContextExitBehavior.Discard:
          Discard();
          ExitPrivate();
          return;

        case BindingContextExitBehavior.CommitImmediate:
          CommitImmediate();
          ExitPrivate();
          return;


        default:
          throw new InvalidOperationException();
      }
    }


    /// <summary>
    /// 退出上下文
    /// </summary>
    /// <param name="discard">是否放弃上下文中存在的绑定操作</param>
    protected void ExitCore( bool discard )
    {
      if ( FollowContext != null )
        FollowContext.ExitCore( discard );

      if ( discard )
        Discard();
      else
        Commit();

      ExitPrivate();
    }

    private void ExitPrivate()
    {
      if ( AboveContext != null )
      {
        if ( AboveContext.FollowContext != this )
          throw new Exception( "未知错误" );

        AboveContext.FollowContext = null;
        Current = AboveContext;
      }
      else

        Current = null;


      HttpContext.Current.Trace.Write( "Binding", string.Format( "Exit BindingContext \"{0}\"", Name ) );
    }


    /// <summary>
    /// 放弃所有的绑定
    /// </summary>
    public void Discard()
    {
      CommitFollow();

      if ( Actions.Any() )
        HttpContext.Current.Trace.Warn( "Binding", string.Format( "Discard BindingContext \"{0}\"", Name ) );

      Actions.Clear();
    }


    /// <summary>
    /// 提交上下文
    /// </summary>
    public void Commit()
    {
      if ( AboveContext == null )//如果是顶层上下文，则应该直接提交
        CommitImmediate();
      else
        CommitAbove();
    }


    /// <summary>
    /// 提交所有绑定操作到上级
    /// </summary>
    protected void CommitAbove()
    {

      if ( AboveContext == null )
        throw new InvalidOperationException();

      if ( FollowContext != null )
        FollowContext.CommitAbove();


      AboveContext.Actions.AddRange( Actions );

      Actions.Clear();

      HttpContext.Current.Trace.Write( "Binding", string.Format( "Commit to above, BindingContext: \"{0}\"", Name ) );
    }

    /// <summary>
    /// 立即执行所有绑定操作
    /// </summary>
    protected void CommitImmediate()
    {

      CommitFollow();

      HttpContext.Current.Trace.Write( "Binding", string.Format( "Begin Commit BindingContext \"{0}\"", Name ) );

      foreach ( var element in PostOrderTraverse( Scope ) )
      {
        var actions = Actions.GetActions( element );

        foreach ( var a in actions )
          a.Bind();
      }

      Actions.Clear();

      HttpContext.Current.Trace.Write( "Binding", string.Format( "End Commit BindingContext \"{0}\"", Name ) );
    }


    private void CommitFollow()
    {
      if ( FollowContext != null )
        FollowContext.CommitAbove();
    }



    public void Dispose()
    {
      if ( FollowContext != null )
        FollowContext.Dispose();

      switch ( _exitBehavior )
      {
        case BindingContextExitBehavior.Default:
        case BindingContextExitBehavior.Discard:
          Discard();
          ExitPrivate();
          return;

        case BindingContextExitBehavior.Commit:
          Commit();
          ExitPrivate();
          return;

        case BindingContextExitBehavior.CommitImmediate:
          CommitImmediate();
          ExitPrivate();
          return;


        default:
          throw new InvalidOperationException();
      }
    }




    /// <summary>
    /// 保存绑定操作的容器
    /// </summary>
    protected class HtmlBindingActionCollection
    {

      private Dictionary<object, List<IHtmlBindingAction>> _targetSet = new Dictionary<object, List<IHtmlBindingAction>>();

      public void Add( IHtmlBindingAction action )
      {
        if ( _targetSet.ContainsKey( action.Target ) )
          _targetSet[action.Target].Add( action );
        else
          _targetSet.Add( action.Target, new List<IHtmlBindingAction>() { action } );
      }


      public bool Contains( object target )
      {
        return _targetSet.ContainsKey( target );
      }


      public IEnumerable<IHtmlBindingAction> GetActions( object target )
      {
        List<IHtmlBindingAction> actions;
        if ( _targetSet.TryGetValue( target, out actions ) )
          return actions;
        else
          return Enumerable.Empty<IHtmlBindingAction>();
      }


      public IEnumerable<IHtmlBindingAction> GetActions()
      {
        return _targetSet.Values.SelectMany( value => value );
      }

      public void Clear()
      {
        _targetSet.Clear();
      }

      internal void Remove( IHtmlBindingAction a )
      {
        throw new NotImplementedException();
      }

      internal void AddRange( HtmlBindingActionCollection actions )
      {
        foreach ( var a in actions.GetActions() )
          Add( a );
      }

      internal bool Any()
      {
        return _targetSet.Any();
      }
    }

  }


  /// <summary>
  /// 定义绑定上下文调用Exit或Dispose方法的时候的行为
  /// </summary>
  public enum BindingContextExitBehavior
  {
    /// <summary>默认行为，对于Exit而言，会尝试Commit，对于Dispose而言，则会Discard</summary>
    Default,
    /// <summary>放弃所有绑定操作</summary>
    Discard,
    /// <summary>尝试提交到上一级上下文，如果上下文是顶层，则立即应用所有修改</summary>
    Commit,
    /// <summary>立即应用所有更改</summary>
    CommitImmediate
  }



  public interface IHtmlBindingAction
  {
    object Target { get; }

    void Bind();
  }

  public class HtmlBindingAction<T> : IHtmlBindingAction where T : IHtmlElement
  {

    public HtmlBindingAction()
    {
      DataBound = false;
    }

    public T Target
    {
      get;
      internal set;
    }

    public Action<T> Action
    {
      get;
      internal set;
    }

    object IHtmlBindingAction.Target { get { return Target; } }

    void IHtmlBindingAction.Bind() { Action( Target ); }

    public bool DataBound
    {
      get;
      internal set;
    }
  }
}
