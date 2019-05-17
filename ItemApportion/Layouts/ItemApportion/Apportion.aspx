<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Import Namespace="Microsoft.SharePoint.ApplicationPages" %>
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Apportion.aspx.cs" Inherits="ItemApportion.Layouts.ItemApportion.Apportion" DynamicMasterPageFile="~masterurl/default.master" %>

<asp:Content ID="PageHead" ContentPlaceHolderID="PlaceHolderAdditionalPageHead" runat="server">
       <style type="text/css">
        .Apportion {
            box-shadow: 0 1px 3px #ccc;
            background-color: white;
            background-color: rgba(255, 255, 255, 0.6);
            padding: 0 20px;
            margin-right: 10px;
            margin-top: 5px;
            width: 400px;
        }
        
        .Apportion .title {
            height: 50px;
            line-height: 50px;
            font-family: "微软雅黑", "PingFang SC", sans;
            font-weight: 600;
            color: #000;
            font-size: 20px;
            border-bottom: 1px solid #ccc;
        }
        
        .Apportion .title a.more {
            font-weight: 400;
            font-size: 12px;
            color: #888;
            vertical-align: baseline;
            float: right;
        }
        
        .formul {
            list-style: none;
        }
        
        .formul li {
            line-height: 25px;
            padding-bottom: 10px;
        }
        
        .formul li button {
            font-size: 14px;
        }
        
        .att {
            background-color: #cfcfcf;
            color: red;
            font-size: 14px;
        }
    </style>
</asp:Content>

<asp:Content ID="Main" ContentPlaceHolderID="PlaceHolderMain" runat="server">
    
<div id="AppraiseDiv" class="Apportion" runat ="server">
 <div class="title">
        <asp:Label ID="lbItemTitle" runat="server" Text="任务名称"></asp:Label>
  </div>
    <div id="AppAction" runat="server" visible="true">
         <ul class="formul">
             <li >
                  分配对象:<SharePoint:PeopleEditor ID="tbName" SelectionSet="User" ValidatorEnabled="true" MultiSelect="false" runat="server"/>
             </li>
            <li>
                计划时长:<asp:TextBox ID="tbDurings" runat="server" TextMode="Number"></asp:TextBox>
            </li>
             <li style="padding-left:22%;">
                <asp:Button ID="btnAppraise" runat="server" Text="保 存" />&nbsp;&nbsp;
                <asp:Button ID="btnCancle" runat="server" Text="返 回" />
            </li>
            <li>
                <asp:Label ID="lblMsg" ForeColor="Red" runat="server" Text=""></asp:Label>
            </li>
           

        </ul>
    </div>
</div>
</asp:Content>

<asp:Content ID="PageTitle" ContentPlaceHolderID="PlaceHolderPageTitle" runat="server">
分配日程
</asp:Content>

<asp:Content ID="PageTitleInTitleArea" ContentPlaceHolderID="PlaceHolderPageTitleInTitleArea" runat="server" >
为项目分配日程
</asp:Content>
