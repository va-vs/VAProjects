<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Import Namespace="Microsoft.SharePoint.ApplicationPages" %>
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="FormAuth.aspx.cs" Inherits="ItemApportion.Layouts.ItemApportion.FormAuth" DynamicMasterPageFile="~masterurl/default.master" %>
<%@ Assembly Name="Microsoft.SharePoint.IdentityModel, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%--<%@ Assembly Name="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c"%>--%> 
<%--<%@ Page Language="C#" Inherits="Microsoft.SharePoint.IdentityModel.Pages.FormsSignInPage" MasterPageFile="~/_layouts/15/errorv15.master"       %>--%>



<asp:Content ID="PageHead" ContentPlaceHolderID="PlaceHolderAdditionalPageHead" runat="server">
    <SharePoint:EncodedLiteral runat="server"  EncodeMethod="HtmlEncode" Id="ClaimsFormsPageTitle" />
    <link href="CSS/style.css" type="text/css" rel="stylesheet" />
    
</asp:Content>

<asp:Content ID="Main" ContentPlaceHolderID="PlaceHolderMain" runat="server">
    <%--<div id="SslWarning" style="color:red;display:none">
 <SharePoint:EncodedLiteral runat="server"  EncodeMethod="HtmlEncode" Id="ClaimsFormsPageMessage" />
 </div>--%>
    <script type="text/javascript" src="JS/script.js"></script>
<%--  <script type="text/javascript">
	if (document.location.protocol != 'https:')
	{
		var SslWarning = document.getElementById('SslWarning');
		SslWarning.style.display = '';
	}
  </script>--%>

 <asp:login id="signInControl" FailureText="<%$Resources:wss,login_pageFailureText%>" runat="server" width="100%">
	<layouttemplate>
		<asp:label id="FailureText" class="ms-error" runat="server"/>
		<table width="100%">
		<tr>
			<td><SharePoint:EncodedLiteral runat="server" text="<%$Resources:wss,login_pageUserName%>" EncodeMethod='HtmlEncode'/></td>
			<td><asp:textbox id="UserName" autocomplete="off" runat="server" class="ms-inputuserfield" width="99%" /></td>
		</tr>
		<tr>
			<td><SharePoint:EncodedLiteral runat="server" text="<%$Resources:wss,login_pagePassword%>" EncodeMethod='HtmlEncode'/></td>
			<td><asp:textbox id="password" TextMode="Password" autocomplete="off" runat="server" class="ms-inputuserfield" width="99%"/></td>
		</tr>
            <tr>
                <td>验证码:</td>
                <td> 
                    <div>
                        <input type = "text" id = "inputCode" value="" />
                        <input type = "button" id="code" onclick="createCode()"/>
                        <span id="errorCode" style="color:red"></span>
                    </div>
                </td>
            </tr>
		<tr>
			<td colspan="2" align="right"><asp:button id="login" commandname="Login" text="<%$Resources:wss,login_pagetitle%>" runat="server" OnClientClick="validate()"/></td>
		</tr>
		<tr>
			<td colspan="2"><asp:checkbox id="RememberMe" text="<%$SPHtmlEncodedResources:wss,login_pageRememberMe%>" runat="server" /></td>

		</tr>
            <tr>
                <td  colspan="2">
                    <a href="#" style="float:left" >忘记密码?</a>
                    <a href="#" style="float:right" >马上注册</a>

                </td>

            </tr>
		</table>

	</layouttemplate>
 </asp:login>

    <div id="content" style="display:none">
    <div class="login-header">
        <img src="assets/images/logo">
    </div>
    <form>
        <div class="login-input-box">
            <span class="icon icon-user"></span>
            <input type="text" placeholder="Please enter your email/phone">
        </div>
        <div class="login-input-box">
            <span class="icon icon-password"></span>
            <input type="password" placeholder="Please enter your password">
        </div>
    </form>
    <div class="remember-box">
        <label>
            <input type="checkbox">&nbsp;Remember Me
        </label>
    </div>
    <div class="login-button-box">
        <button>Login</button>
    </div>
    <div class="logon-box">
        <a href="">Forgot?</a>
        <a href="">Register</a>
    </div>
</div>

</asp:Content>

<asp:Content ID="PageTitle" ContentPlaceHolderID="PlaceHolderPageTitle" runat="server">
应用程序页
</asp:Content>

<asp:Content ID="PageTitleInTitleArea" ContentPlaceHolderID="PlaceHolderPageTitleInTitleArea" runat="server" >
我的应用程序页
</asp:Content>
