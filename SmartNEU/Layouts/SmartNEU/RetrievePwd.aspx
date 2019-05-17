<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Import Namespace="Microsoft.SharePoint.ApplicationPages" %>
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="RetrievePwd.aspx.cs" Inherits="SmartNEU.Layouts.SmartNEU.RetrievePwd" DynamicMasterPageFile="~masterurl/default.master" %>

<asp:Content ID="PageHead" ContentPlaceHolderID="PlaceHolderAdditionalPageHead" runat="server">
 <script type ="text/javascript" src="Validate.js" ></script>
    <script  type="text/javascript">
        function isValid() {
            var name = document.getElementById('<%=txtAccount.ClientID%>');
            if (name.value.length == 0) {
                alert("学号/工号不能为空！");
                name.select();
                return false;
            }
            var email = document.getElementById('<%=txtEmail.ClientID%>');
            if (email.value.length == 0) {
                alert("预留邮箱不能为空！");
                email.select();
                return false;
            }
            else if (!CheckEmail(email.value)) {
                email.select();
                return false;
            }

            return true;
        }
        </script>
     <style type="text/css">
         .h3css{ font-family:微软雅黑; color:#4141a4; text-decoration:underline}
       .tdone{text-align:right; padding-right:10px; color:#4a4a4a; width:120px}
       .txtcss{ width:200px; border:1px #bebee1 solid; height:25px; vertical-align:middle; line-height:25px; padding:0 10px;
                color:Black;}
        .txtdoenlist{ width:103px; height:25px; line-height:25px; vertical-align:middle; padding:3px;border:1px #bebee1 solid;color:#494b4c; }
        .txtdoenlista{ width:210px; height:25px; line-height:25px; vertical-align:middle; padding:3px;border:1px #bebee1 solid;color:#494b4c; }
        .buttoncss{ width:120px;height:40px; background-color:#3776a9; color:#fff;font-family:微软雅黑; font-size:20px ; border:1px solid #3776a9; cursor:pointer }
     </style>
</asp:Content>

<asp:Content ID="Main" ContentPlaceHolderID="PlaceHolderMain" runat="server">
<table style=" font-size:14px; color:#494b4c" cellspacing="0"  cellpadding="3">
    <tr>
        <td  class="tdone">学号/工号:<span style="color:red">*</span></td>
        <td>
        <asp:TextBox ID="txtAccount" runat="server" AutoPostBack="true"  CssClass="txtcss" onfocus="onftxtAccount()" onblur="onbtxtAccount()"></asp:TextBox></td>
    </tr>
    <tr>
        
        <td  class="tdone">预留邮箱:<span style="color:red">*</span></td>
        <td><asp:TextBox ID="txtEmail" runat="server"  CssClass="txtcss" onfocus="onftxtAccount()" onblur="onbtxtAccount()"></asp:TextBox>
            <br/>
            <asp:Label ID="lblMsg" ForeColor="Red" runat="server" Text=""></asp:Label></td>
    </tr>
    <tr>
        <td>&nbsp;</td>
        <td  style="text-align:left"><asp:Button ID="btnSubmit" OnClientClick="return isValid();" runat="server" Text="找回密码"  BorderStyle="None" style="background-color:#8395fc" CssClass="buttoncss" /></td>
    </tr>
</table>

     <div style="padding:5px;border-radius:5px;font-family:'Microsoft YaHei UI';font-size:13px;">
                忘记了预留邮箱，请将您的<b>学号（工号）、姓名、校园卡正反面照片发送邮件至</b><a href="mailto:smartneu@mail.neu.edu.cn?subject=找回密码">smartneu@mail.neu.edu.cn</a>找回密码
                </div>
</asp:Content>

<asp:Content ID="PageTitle" ContentPlaceHolderID="PlaceHolderPageTitle" runat="server">
忘记密码
</asp:Content>

<asp:Content ID="PageTitleInTitleArea" ContentPlaceHolderID="PlaceHolderPageTitleInTitleArea" runat="server" >
找回密码
</asp:Content>
