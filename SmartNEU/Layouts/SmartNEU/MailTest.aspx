<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Import Namespace="Microsoft.SharePoint.ApplicationPages" %>
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="MailTest.aspx.cs" Inherits="SmartNEU.Layouts.SmartNEU.MailTest" DynamicMasterPageFile="~masterurl/default.master" %>

<asp:Content ID="PageHead" ContentPlaceHolderID="PlaceHolderAdditionalPageHead" runat="server">

</asp:Content>

<asp:Content ID="Main" ContentPlaceHolderID="PlaceHolderMain" runat="server">
    <div>
        <asp:Button ID="btnGetUsers" runat="server" Text="获取AD用户列表" OnClick="btnGetUsers_Click"/>

        <div id="usersListDiv" runat="server"></div>
    </div>>
    <div>

         <table align="center" border="0" cellpadding="0" cellspacing="0" width="776">

             <tr>

                 <td>

                     <table align="center" border="0" cellpadding="4" cellspacing="1" width="600" bgcolor="#cccccc">

                         <tr>

                             <td colspan="2" bgcolor="#f0f0f0" align="center">电子邮件发送程序</td>

                         </tr>

                         <tr>

                             <td bgcolor="#f0f0f0" align="right" width="150">发送人：</td>

                             <td bgcolor="#ffffff" align="left"><asp:TextBox ID="fromMail" runat="server" Width="300" /></td>

                         </tr>

                         <tr>

                             <td bgcolor="#f0f0f0" align="right">收件人：</td>

                             <td bgcolor="#ffffff" align="left"><asp:TextBox ID="toMail" runat="server" Width="300" /></td>

                         </tr>

                         <tr>

                             <td bgcolor="#f0f0f0" align="right">抄送人：</td>

                             <td bgcolor="#ffffff" align="left"><asp:TextBox ID="ccMail" runat="server" Width="300" /></td>

                         </tr>

                         <tr>

                             <td bgcolor="#f0f0f0" align="right">暗送人：</td>

                             <td bgcolor="#ffffff" align="left"><asp:TextBox ID="bccMail" runat="server" Width="300" /></td>

                         </tr>

                         <tr>

                             <td bgcolor="#f0f0f0" align="right">主&nbsp;&nbsp;&nbsp;&nbsp;题：</td>

                             <td bgcolor="#ffffff" align="left"><asp:TextBox ID="subject" runat="server" Width="300" /></td>

                         </tr>

                         <tr>

                             <td bgcolor="#f0f0f0" align="right">附&nbsp;&nbsp;&nbsp;&nbsp;件：</td>

                             <td bgcolor="#ffffff" align="left"><input type="file" id="upfile" runat="server" /></td>

                         </tr>

                         <tr>

                             <td bgcolor="#f0f0f0" align="right">内&nbsp;&nbsp;&nbsp;&nbsp;容：</td>

                             <td bgcolor="#ffffff" align="left"><asp:TextBox ID="body" TextMode="multiLine" runat="server" Width="300" Height="200" /></td>

                         </tr>

                         <tr>

                             <td bgcolor="#f0f0f0" align="right">格&nbsp;&nbsp;&nbsp;&nbsp;式：</td>

                             <td bgcolor="#ffffff" align="left"><asp:RadioButtonList ID="format" runat="server"/></td>

                         </tr>

                         <tr>

                             <td bgcolor="#f0f0f0" colspan="2" align="center">

                                 <asp:Button ID="send" runat="server" Text="发送" OnClick="send_Click" />&nbsp;&nbsp;

                                 <asp:Button ID="reset" runat="server" Text="重置" />

                             </td>

                         </tr>

                     </table>

                 </td>

             </tr>

         </table>

     </div>
</asp:Content>

<asp:Content ID="PageTitle" ContentPlaceHolderID="PlaceHolderPageTitle" runat="server">
应用程序页
</asp:Content>

<asp:Content ID="PageTitleInTitleArea" ContentPlaceHolderID="PlaceHolderPageTitleInTitleArea" runat="server" >
我的应用程序页
</asp:Content>
