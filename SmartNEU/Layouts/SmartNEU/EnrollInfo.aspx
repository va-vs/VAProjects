<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Import Namespace="Microsoft.SharePoint.ApplicationPages" %>
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="EnrollInfo.aspx.cs" Inherits="SmartNEU.Layouts.SmartNEU.EnrollInfo" DynamicMasterPageFile="~masterurl/default.master" %>

<asp:Content ID="PageHead" ContentPlaceHolderID="PlaceHolderAdditionalPageHead" runat="server">
      <script type ="text/javascript" src="Validate.js" ></script>
    <script  type="text/javascript">
        function IsValid()
        {

            var name = document.getElementById('<%=txtName.ClientID%>');
            if (name.value.length == 0) {
                alert("姓名不能为空！");
                name.select();
                return false;
            }
            else if (!checkChineseCharacter(name.value)) {
                alert("姓名只能输入中文汉字！");
                name.select();
                return false;
            }

            //电话
            var tel = document.getElementById('<%=txtTelephone.ClientID%>');
            var tel = document.getElementById('<%=txtTelephone.ClientID%>');
            if ((tel.value.length > 0) && !CheckTelephone(tel.value)) {
                tel.select();
                return false;
            }
            //邮件
            var email = document.getElementById('<%=txtEmail.ClientID%>');
            if (email.value.length == 0) {
                alert("邮箱不能为空！");
                email.select();
                return false;
            }
            else if (!CheckEmail(email.value)) {
                email.select();
                return false;
            }
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
   <div>
    <table style=" font-size:14px; color:#494b4c" cellspacing="0" cellpadding="3">
        <tr>
            <td class="tdone">工号/学号：</td>
            <td>
                <asp:TextBox ID="txtAccount" readonly="True" runat="server" CssClass="txtcss" onfocus="onft(this)" onblur="onbtName()"></asp:TextBox>
            </td>
        </tr>

        <tr>
            <td class="tdone">创意名称：</td>
            <td>
                <asp:TextBox ID="txtCreativeName" runat="server" CssClass="txtcss" onfocus="onftxtAccount()" onblur="onbtxtAccount()"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td class="tdone">申报人姓名：</td>
            <td>
                <asp:TextBox ID="txtName" runat="server" CssClass="txtcss" onfocus="onft(this)" onblur="onbtName()"></asp:TextBox>
            </td>
        </tr>

        <tr>
            <td class="tdone">联系电话：</td>
            <td>
                <asp:TextBox ID="txtTelephone" runat="server" CssClass="txtcss" onfocus="onft(this)" onblur="onbtTel()" TextMode="Phone"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td class="tdone">电子邮箱：</td>
            <td>
                <asp:TextBox ID="txtEmail" runat="server" CssClass="txtcss" onfocus="onft(this)" onblur="onbtEmail()" TextMode="Email"></asp:TextBox>
            </td>
        </tr>


        <tr>
            <td colspan="2" class="tdone">
                <asp:CheckBox ID="chkTongyi" runat="server" Checked="True" />我已阅读参赛须知，同意竞赛规则。
            </td>
        </tr>
        <tr>
            <td colspan="2" style="height:20px;"></td>
        </tr>
        <tr>
            <td>&nbsp;</td>
            <td style="text-align:left">
                <asp:Button ID="btnSave" OnClientClick="return IsValid();" runat="server" Text="报 名" BorderStyle="None" style="background-color:#8395fc" CssClass="buttoncss" />
                <asp:Button ID="btnClose" runat="server" Text="返 回" Width="0" CssClass="buttoncss" BorderStyle="None" style="background-color:#a7f897"/>
            </td>
        </tr>
    </table>
</div>
</asp:Content>

<asp:Content ID="PageTitle" ContentPlaceHolderID="PlaceHolderPageTitle" runat="server">
报名
</asp:Content>

<asp:Content ID="PageTitleInTitleArea" ContentPlaceHolderID="PlaceHolderPageTitleInTitleArea" runat="server" >
报名
</asp:Content>
