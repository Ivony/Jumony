<%@ WebHandler Language="C#" Class="TestInnerText" %>

using System;
using System.Web;
using System.Linq;
using Ivony.Html;
using Ivony.Html.Web;

public class TestInnerText : JumonyHandler
{

  protected override void ProcessDocument()
  {
    Document.FindSingle( "div" ).InnerText(
@"
    白日依山尽，
    黄河入海流；
    欲穷千里目，
    更上一层楼。
" );


    Document.FindSingle( "pre" ).InnerText(
@"
    白日依山尽，
    黄河入海流；
    欲穷千里目，
    更上一层楼。
" );

    Document.FindSingle( "title" ).InnerText(
@"
    白日依山尽，
    黄河入海流；
    欲穷千里目，
    更上一层楼。
" );

  }
}