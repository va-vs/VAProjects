<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Import Namespace="Microsoft.SharePoint.ApplicationPages" %>
<%@ Register TagPrefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>

<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AllMentors.aspx.cs" Inherits="FSCAppPages.Layouts.FSCAppPages.AllMentors" DynamicMasterPageFile="~masterurl/default.master" %>

<asp:Content ID="PageHead" ContentPlaceHolderID="PlaceHolderAdditionalPageHead" runat="server">
    <script src="js/jquery-1.8.3.min.js" type="text/javascript"></script>
    <link href="css/style.css" type="text/css" rel="stylesheet" />
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />

    <style type="text/css">
         .pp a:link {
        padding: 0 10px 0 10px;
        color: #0072C6;
        text-decoration: none;
        font-size: 15px;
        min-width: 100px;
    }

    .pp a:visited {
        color: #0072C6;
        text-decoration: none;
    }

    .pp a:hover {
        color: #FF0000;
        text-decoration: none;
    }

    .pp a:active {
        color: #FFFFFF;
        background-color: #002663;
        text-decoration: none;
    }
    </style>

</asp:Content>

<asp:Content ID="Main" ContentPlaceHolderID="PlaceHolderMain" runat="server">
    <div id="addNew" runat="server" style="width: 100%; text-align: right; padding: 10px;"></div>
    <div class="mylist" id="mylist" runat="server">
        <div class="listtitle">
            1.英语语言文学
        </div>
        <div id="div1" runat="server" class="pp">
            <a href="/Mentors/Lists/posts/Viewpost.aspx?ID=16">刘卓</a>
        </div>

    </div>


    <%-- 下面是准备使用头像及个人信息的视图显示导师个人信息，尚未完成 --%>
    <div class="page" style="display:none;">
        <%-- 标签区域 --%>
        <section class="demo">
            <ul class="nav nav-tabs" id="myTab">
                <li class="active">
                    <a href="#ctl00_PlaceHolderMain_content1">英语语言文学</a>
                </li>
                <li>
                    <a href="#ctl00_PlaceHolderMain_content2">俄语语言文学</a>
                </li>
                <li>
                    <a href="#ctl00_PlaceHolderMain_content3">日语语言文学</a>
                </li>
                <li>
                    <a href="#ctl00_PlaceHolderMain_content4">外国语言学及应用语言学</a>
                </li>
                <li>
                    <a href="#ctl00_PlaceHolderMain_content5">英语笔译</a>
                </li>
                <li>
                    <a href="#ctl00_PlaceHolderMain_content6">英语口译</a>
                </li>
                <li>
                    <a href="#ctl00_PlaceHolderMain_content7">日语笔译</a>
                </li>
            </ul>


            <%-- 内容区域 --%>
            <div class="tab-content">
                <div  class="tab-pane active" id="content1" runat="server">
                    无
                </div>
                <div class="tab-pane" id="content2" runat="server">
                    无
                </div>
                <div class="tab-pane" id="content3" runat="server">
                    无
                </div>

                <div class="tab-pane" id="content4" runat="server">
                    无
                </div>
                <div class="tab-pane" id="content5" runat="server">
                    无
                </div>
                <div class="tab-pane" id="content6" runat="server">
                    无
                </div>
                <div class="tab-pane" id="content7" runat="server">
                    无
                </div>
            </div>
        </section>
    </div>
    <script type="text/javascript" src="js/jquery.js"></script>
    <script type="text/javascript" src="js/bootstrap-tab.js"></script>
    <script type="text/javascript">
        $(function () {
            $('#myTab a').click(function (e) {
                e.preventDefault();
                $(this).tab('show');
            })
        });
    </script>

</asp:Content>

<asp:Content ID="PageTitle" ContentPlaceHolderID="PlaceHolderPageTitle" runat="server">
导师名录
</asp:Content>

<asp:Content ID="PageTitleInTitleArea" ContentPlaceHolderID="PlaceHolderPageTitleInTitleArea" runat="server">
导师名录
</asp:Content>
