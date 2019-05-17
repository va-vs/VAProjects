<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Import Namespace="Microsoft.SharePoint.ApplicationPages" %>
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ChangePassword.aspx.cs" Inherits="ChangePassword.Layouts.ChangePassword.ChangePassword" DynamicMasterPageFile="~masterurl/default.master" %>
<asp:Content ID="PageHead" ContentPlaceHolderID="PlaceHolderAdditionalPageHead" runat="server">
    <script  type="text/javascript">
  <%--  document.onload()
   {
       var txt=document.getElementById('<%=tboldPass.ClientID%>')
       txt.focus();
    }--%>
        //返回上次页面
        function retUrl(key) {
            var svalue = window.location.search.match(new RegExp("[\?\&]" + key + "=([^\&]*)(\&?)", "i"));
            return svalue ? svalue[1] : svalue;
        }
        //重新登录
        function reLogin(strName, strPWD) {
            //var strName = userid.value;//登陆的名字建议使用：域\名字
            //var strPWD = password.value;//登陆密码
            var location = '/login/SitePages/default.aspx';
            var auth = new ActiveXObject('msxml2.xmlhttp');
            strName = strName.toLowerCase();
            auth.open('post', location, false, strName, strPWD);
            auth.send();
            switch (auth.status) {
                case 200: window.location.href = '/login/SitePages/default.aspx'; // 登陆页面
                    //if (document.referrer != '')
                    //    window.location.href = document.referrer;
                    //else {
                    var c = retUrl("Resource");
                    if (c == null)
                        window.location.href = '/default.aspx';
                    else
                        window.location.href = c;//"/";

                    //}
            }
        }
        function IsValid() {
            var name = document.getElementById('<%=tboldPass.ClientID%>');
        if (name.value.length == 0) {
            alert("旧密码不能为空！");
            name.select();
            return false;
        }
        name = document.getElementById('<%=tbnewPass.ClientID%>');
        if (name.value.length == 0) {
            alert("新密码不能为空！");
            name.select();
            return false;
        }
        var name1 = document.getElementById('<%=tbnewPass2.ClientID%>');
        if (name1.value.length == 0) {
            alert("确认新密码不能为空！");
            name1.select();
            return false;
        }
        if (name.value != name1.value) {
            alert("新密码与确认新密码不一致！");
            name.focus();
            return false;
        }
    }
 </script>   
    <style type="text/css">
        .h3css{ font-family:微软雅黑; color:#4141a4; text-decoration:underline}
    .tdone{text-align:right; padding-right:10px; color:#4a4a4a; width:150px}
    .txtcss{ width:200px; border:1px #bebee1 solid; height:25px; vertical-align:middle; line-height:25px; padding:0 10px;
            color:Black;}
</style>
</asp:Content>
<asp:Content ID="Main" ContentPlaceHolderID="PlaceHolderMain" runat="server">

    <table style="font-size:14px; color:#494b4c"  cellspacing="0"  cellpadding="3">
        <tr>
            <td  style="padding-left:10px;"  colspan="2"><asp:Label ID="lblTitle" runat="server"  Text="Label" Font-Bold="True"></asp:Label></td>
        </tr>
        <tr runat="server"  id="trAccount">
            <td  class="tdone"><asp:Label ID="lblAccount" runat="server" Text="输入帐号：" ></asp:Label></td>
            <td><asp:TextBox ID="txtAccount" class="txtcsstxtcss"  runat="server" BorderStyle="Groove" ></asp:TextBox></td>
        </tr>
        <tr>
            <td class="tdone"><asp:Label ID="lbloldPass"  runat="server" Text="输入旧密码："></asp:Label></td>
            <td><asp:TextBox ID="tboldPass" class="txtcsstxtcss"  runat="server" BorderStyle="Groove" TextMode="Password"></asp:TextBox></td>
        </tr>
        <tr>
            <td  class="tdone"><asp:Label ID="lblnewPass" runat="server" Text="输入新密码："></asp:Label></td>
            <td><asp:TextBox ID="tbnewPass" class="txtcsstxtcss"  runat="server" BorderStyle="Groove" TextMode="Password"></asp:TextBox></td>
        </tr>
        <tr>
            <td class="tdone"><asp:Label ID="lblnewPass1" runat="server" Text="确认新密码："></asp:Label></td>
            <td><asp:TextBox ID="tbnewPass2" class="txtcsstxtcss"  runat="server" BorderStyle="Groove" TextMode="Password"></asp:TextBox></td>
        </tr>
         <tr>
            <td colspan="2"><asp:Label ID="lblInfo" runat="server" Text="" Font-Bold="True" ForeColor="Red"></asp:Label></td>
         </tr>
         <tr>                 
            <td></td><td><asp:Button ID="btOK"  OnClientClick="return IsValid()" runat="server" Text="确定"  /></td>
        </tr>
        <tr>
            <td colspan="2">
                 <div style="color: #0072c6; padding-left: 10px;width:400px; "  id="udesp" runat="server"><ul><li>首先请确认账号正确，两次密码必须一致</li><li>密码长度不得小于 8 位</li><li>至少包含以下四类字符中的三类：大写字母、小写字母、数字、以及英文标点符号</li></ul></div>
            </td>
        </tr>
    </table>
</asp:Content>

<asp:Content ID="PageTitle" ContentPlaceHolderID="PlaceHolderPageTitle" runat="server">
更改密码
</asp:Content>

<asp:Content ID="PageTitleInTitleArea" ContentPlaceHolderID="PlaceHolderPageTitleInTitleArea" runat="server" >

</asp:Content>
