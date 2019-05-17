<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Import Namespace="Microsoft.SharePoint.ApplicationPages" %>
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="PortalPage.aspx.cs" Inherits="SmartNEU.Layouts.SmartNEU.PortalPage" DynamicMasterPageFile="~masterurl/default.master" %>

<asp:Content ID="PageHead" ContentPlaceHolderID="PlaceHolderAdditionalPageHead" runat="server">

</asp:Content>

<asp:Content ID="Main" ContentPlaceHolderID="PlaceHolderMain" runat="server">
    <asp:TextBox ID="mailbody" runat="server" Visible="false">邮件主题</asp:TextBox>
    <asp:TextBox ID="gettxt" runat="server" Visible="false">收件人</asp:TextBox>
    <asp:TextBox ID="smtptxt" runat="server" Visible="false">smtp</asp:TextBox>
    <asp:TextBox ID="sendtxt" runat="server" Visible="false">发件人</asp:TextBox>
    <asp:TextBox ID="pwdtxt" runat="server" Visible="false">密码</asp:TextBox>
    <asp:Button ID="Button1" runat="server" Text="发邮件" OnClick="Button1_Click" Visible="false"/>
    请输入学号或工号：<asp:TextBox ID="IDTxt" runat="server"></asp:TextBox>
    <asp:Button ID="btnCheck" runat="server" Text="检测账号" />
    <div id="HadAcc" style="background-color:#f3f8f1;padding:5px;border:1px solid #0094ff;border-radius:5px;margin:5px;" runat="server" visible="false">
        <div>恭喜你，你的学号（工号）已经是本站的会员账号，<a href="/SmartNEU/_layouts/15/Authenticate.aspx?Source=/SmartNEU/_layouts/15/SmartNEU/EditUserInfo.aspx">点此登录</a></div>
        <br />
        <div style="padding-left:10px;">
            忘记了密码，<a href="RetrievePwd.aspx">点此找回</a> 
            </div> 
    </div>
    <div id="NotHadAcc" style="background-color:#f0fefe;border:1px solid #0094ff;border-radius:5px;padding:5px;margin:5px;" runat="server" visible="false">
        很抱歉，您尚未注册本站会员，→<a href="Registration.aspx">点此注册</a>
    </div>
    
</asp:Content>

<asp:Content ID="PageTitle" ContentPlaceHolderID="PlaceHolderPageTitle" runat="server">
智慧东大创意大赛
</asp:Content>

<asp:Content ID="PageTitleInTitleArea" ContentPlaceHolderID="PlaceHolderPageTitleInTitleArea" runat="server" >
账户检测
</asp:Content>
