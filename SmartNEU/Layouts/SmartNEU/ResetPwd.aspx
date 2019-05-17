<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Import Namespace="Microsoft.SharePoint.ApplicationPages" %>
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ResetPwd.aspx.cs" Inherits="SmartNEU.Layouts.SmartNEU.ResetPwd" DynamicMasterPageFile="~masterurl/default.master" %>

<asp:Content ID="PageHead" ContentPlaceHolderID="PlaceHolderAdditionalPageHead" runat="server">
<script type ="text/javascript" src="Validate.js" ></script>
    <script  type="text/javascript">
        function isValid() {
            var pass = document.getElementById('<%=txtPwd.ClientID%>');
            if (!pass.getAttribute("readonly")) {
                if (pass.value.length < 6) {
                    alert("密码长度不能小于6！");
                    pass.select();
                    return false;
                }
                if (pass.value != document.getElementById('<%=txtPwd1.ClientID%>').value) {
                    alert("两次密码不一致！");
                    document.getElementById('<%=txtPwd1.ClientID%>').select();
                return false;
            }
        }
        var name = document.getElementById('<%=txtCheckCode.ClientID%>');
            if (name.value.length == 0) {
                alert("验证码不能为空！");
                name.select();
                return false;
            }
            return true;
        }
        </script>
    <style type="text/css">
         .h3css{ font-family:微软雅黑; color:#4141a4; text-decoration:underline}
       .tdone{text-align:right; padding-right:10px; color:#4a4a4a; width:150px}
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
        <td colspan="2">你正在找回密码的帐号为：&nbsp;
            <asp:Label ID="txtAccount" runat="server" Text="" ForeColor="blue"></asp:Label>
            <%--<asp:TextBox ID="txtAccount"  runat="server" ReadOnly="True"  CssClass="txtcss" onfocus="onftxtAccount()" onblur="onbtxtAccount()"></asp:TextBox>--%>
        </td>
    </tr>
    <tr>
        <td  class="tdone">请设置你的新密码:</td>
        <td>
        <asp:TextBox ID="txtPwd"  TextMode="Password" runat="server"  CssClass="txtcss" onfocus="onftxtAccount()" onblur="onbtxtAccount()"></asp:TextBox></td>
    </tr>
    <tr>
        <td  class="tdone">请再次输入你的新密码:</td>
        <td>
        <asp:TextBox ID="txtPwd1"  TextMode="Password" runat="server"  CssClass="txtcss" onfocus="onftxtAccount()" onblur="onbtxtAccount()"></asp:TextBox></td>
    </tr>
    <tr>
        <td  class="tdone">请输入收到的验证码:</td>
        <td><asp:TextBox ID="txtCheckCode" runat="server"   CssClass="txtcss" onfocus="onftxtAccount()" onblur="onbtxtAccount()"></asp:TextBox></td>
    </tr>
    <tr>
        <td colspan="2">
            <span style="color:#00bfff">请在20分钟之内登陆邮箱查收验证码，并在本页完成密码找回操作！</span>
        </td>
    </tr>
    <tr>
        <td>&nbsp;</td>
        <td  style="text-align:left"><asp:Button ID="btnSubmit" OnClientClick="return isValid();" runat="server" Text="提 交"  BorderStyle="None" style="background-color:#8395fc" CssClass="buttoncss" /></td>
    </tr>
</table>
</asp:Content>

<asp:Content ID="PageTitle" ContentPlaceHolderID="PlaceHolderPageTitle" runat="server">
重置密码
</asp:Content>

<asp:Content ID="PageTitleInTitleArea" ContentPlaceHolderID="PlaceHolderPageTitleInTitleArea" runat="server" >
重置密码
</asp:Content>
