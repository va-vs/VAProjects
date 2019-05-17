<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Import Namespace="Microsoft.SharePoint.ApplicationPages" %>
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="test.aspx.cs" Inherits="SmartNEU.Layouts.SmartNEU.test" DynamicMasterPageFile="~masterurl/default.master" %>

<asp:Content ID="PageHead" ContentPlaceHolderID="PlaceHolderAdditionalPageHead" runat="server">
     <script type ="text/javascript" src="Validate.js" ></script>
    <script  type="text/javascript">
         function onbtxtAccount()
        {
            var name = document.getElementById('<%=txtAccount.ClientID%>');
        if (name.value.length == 0) {
            //alert("登录帐号不能为空！");
            spanAcc.innerHTML = "登录帐号不能为空！";
            name.select();
            return false;
        }
        else if (!check_Number(name.value)) {
            spanAcc.innerHTML = "只能输入工号或学号！";
            name.select();
            return false;
        }
        else
            spanAcc.innerHTML = "";

        }
    </script>
</asp:Content>

<asp:Content ID="Main" ContentPlaceHolderID="PlaceHolderMain" runat="server">
      <table style=" font-size:14px; color:#494b4c" cellspacing="0" cellpadding="3">
        <tr>
            <td class="tdone">登录帐号：<span style="color:red">*</span></td>
            <td>
                <asp:TextBox ID="txtAccount" runat="server" CssClass="txtcss"   onblur="onbtxtAccount()" ToolTip="必填项！注册账户必须填写本人的工号或学号"  ></asp:TextBox>(工号或学号) <br/><span  style="font-size:14px; color:red;  margin-left:10px" id="spanAcc"></span><div id="divAccountMsg"  style=" color :red "   runat="server"  ></div></td>

        </tr>
        <tr>
            <td class="tdone">密 码：<span style="color:red">*</span></td>
            <td>
                <asp:TextBox ID="txtPwd" runat="server" TextMode="Password" CssClass="txtcss" onfocus="onfocusjs()" onblur="onblurjs()" ToolTip="必填项！密码最少6位"  ></asp:TextBox><span  style="font-size:14px; color:red;  margin-left:10px" id="spanPwd"></span><br/>
               <asp:Label ID="lblPwdMsg" ForeColor="Red" runat="server" Text=""></asp:Label>
            </td>
        </tr>
        <tr>
            <td class="tdone">确认密码：<span style="color:red">*</span></td>
            <td>
                <asp:HiddenField ID="HiddenField1" Value="SmartNEU Ccc2008neu" runat="server" />
                <asp:TextBox ID="txtPwd1" runat="server" TextMode="Password" CssClass="txtcss" onfocus="onft(this)" onblur="onblurjs()" ToolTip="必填项！两次密码必须一致"  ></asp:TextBox><span  style="font-size:14px; color:red;  margin-left:10px" id="spanPwd1"></span><br/>
               <asp:Label ID="lblPwd1Msg" ForeColor="Red" runat="server" Text=""></asp:Label>
            </td>
        </tr>
          </table>
</asp:Content>

<asp:Content ID="PageTitle" ContentPlaceHolderID="PlaceHolderPageTitle" runat="server">
应用程序页
</asp:Content>

<asp:Content ID="PageTitleInTitleArea" ContentPlaceHolderID="PlaceHolderPageTitleInTitleArea" runat="server" >
我的应用程序页
</asp:Content>
