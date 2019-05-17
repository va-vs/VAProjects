<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Import Namespace="Microsoft.SharePoint.ApplicationPages" %>
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ScoreStandard.aspx.cs" Inherits="WorkEvaluate.Layouts.WorkEvaluate.ScoreStandard" DynamicMasterPageFile="~masterurl/default.master" %>

<asp:Content ID="PageHead" ContentPlaceHolderID="PlaceHolderAdditionalPageHead" runat="server">
   
</asp:Content>
<asp:Content ID="Main" ContentPlaceHolderID="PlaceHolderMain" runat="server">
    <table cellspacing="5">
        <tr>
<td>作品期次：<asp:DropDownList ID="ddlQiCi" AutoPostBack="true"  runat="server"></asp:DropDownList>&nbsp;&nbsp;&nbsp;&nbsp;作品类别：<asp:DropDownList ID="ddlWorksType" AutoPostBack="true" runat="server"></asp:DropDownList>
</td> 
        </tr>
        <tr><td colspan ="2"><div id="divEditContent"  runat= "server" style="width:600px"  >

</div></td></tr>
        <tr><td colspan ="2" align="center"><asp:Button ID="btnSave" runat="server"   Text="保存" /></tr>
        
    </table>
</asp:Content>
<asp:Content ID="PageTitle" ContentPlaceHolderID="PlaceHolderPageTitle" runat="server">
期次指标
</asp:Content>
<asp:Content ID="PageTitleInTitleArea" ContentPlaceHolderID="PlaceHolderPageTitleInTitleArea" runat="server" >
本期次作品评价指标
</asp:Content>