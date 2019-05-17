<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Import Namespace="Microsoft.SharePoint.ApplicationPages" %>
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="EditUserInfo.aspx.cs" Inherits="SmartNEU.Layouts.SmartNEU.EditUserInfo" DynamicMasterPageFile="~masterurl/default.master" %>

<asp:Content ID="PageHead" ContentPlaceHolderID="PlaceHolderAdditionalPageHead" runat="server">
        <script type="text/javascript" src="Validate.js"></script>
    <script type="text/javascript">
        var code; //在全局定义验证码  
        window.onbeforeunload = function () {
            var n = window.event.screenX - window.screenLeft;
            var b = n > document.documentElement.scrollWidth - 20;
            if (b && window.event.clientY < 0 || window.event.altKey)  // 是关闭而非刷新 
            {
                document.getElementById('btnClose').click();
            }
        }
        window.onload = function createCode() {
           
            var checkCode = document.getElementById("code");
            code =returnCode();
            checkCode.value = code;//把code值赋给验证码  
        }
       
     function createCode1() {
           
            var checkCode = document.getElementById("code");
            code =returnCode();
            checkCode.value = code;//把code值赋给验证码  
        }
        //function getimgcode() {
        //    document.getElementById("imag1").src = document.getElementById("imag1").src + '?';

        //}
        //function getCookie(name) {
        //    var cookieValue = null;
        //    var search = name + "=";
        //    if (document.cookie.length > 0) {
        //        offset = document.cookie.indexOf(search);
        //        if (offset != -1) {
        //            offset += search.length;
        //            end = document.cookie.indexOf(";", offset);
        //            if (end == -1) {
        //                end = document.cookie.length;
        //            }
        //            cookieValue = unescape(document.cookie.substring(offset, end))
        //        }
        //    }
        //    return cookieValue;
        //}

        function IsValid() {
            //var chkValue = getCookie("CheckCode")
            //if (chkValue == null) {
            //    alert("您的浏览器设置已被禁用 Cookies，您必须设置浏览器允许使用 Cookies 选项后才能使用本系统。");
            //    return false;
            //}

            var name = document.getElementById('<%=txtName.ClientID%>');
            if (name.value.length == 0) {
                //alert("姓名不能为空！");
                spanName.innerHTML = "姓名不能为空！";
                name.select();
                return false;
            }
            else if (!checkChineseCharacter(name.value)) {
                //alert("姓名只能输入中文汉字！");
                spanName.innerHTML = "姓名只能输入中文汉字！";
                name.select();
                return false;
            }
            else
                spanName.innerHTML = "";


            var tel = document.getElementById('<%=txtTelephone.ClientID%>');
            var tel = document.getElementById('<%=txtTelephone.ClientID%>');
            if ((tel.value.length > 0) && !CheckTelephone(tel.value)) {
                spanTel.innerHTML = "电话输入格式错误";
                tel.select();
                return false;
            }
            else
                spanTel.innerHTML = "";

            var email = document.getElementById('<%=txtEmail.ClientID%>');
            if (email.value.length == 0) {
                //alert("邮箱不能为空！");
                spanEmail.innerHTML = "邮箱不能为空！";
                email.select();
                return false;
            }
            else if (!CheckEmail(email.value)) {
                spanEmail.innerHTML = "E-mail地址格式错误!"
                email.select();
                return false;
            }
            else
                spanEmail.innerHTML = "";
            var inputCode = document.getElementById("input").value.toUpperCase(); //取得输入的验证码并转化为大写        
            if (inputCode.length <= 0) { //若输入的验证码长度为0  
                //alert("请输入验证码！"); //则弹出请输入验证码  
                spanValid.innerHTML = "验证码输入错误！";
                document.getElementById("input").select();
                return false;
            }
            else if (inputCode != code) { //若输入的验证码与产生的验证码不一致时  
                //alert("验证码输入错误！"); //则弹出验证码输入错误  
                spanValid.innerHTML = "验证码输入错误！";
                code = returnCode();
                document.getElementById("code").value = code//刷新验证码  
                //document.getElementById("input").value = "";//清空文本框  
                document.getElementById("input").select();
                return false;
            }
            else
                spanValid.innerHTML = "";
           <%-- var checkCode = document.getElementById('<%=txtCheckCode.ClientID%>');

            if (checkCode.value.length == 0 || checkCode.value.toLowerCase() != chkValue.toLowerCase()) {
                alert("验证码错误");
                return false;
            }--%>
            return true;
        }
        function delValid() {
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
            if (confirm('请核对信息，删除后不能恢复\n    确定删除吗?'))
            {
                return true;
            }
            else {
                return false;
            }
            
        }
    </script>
 
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <style type="text/css">
        .labelTD {
            text-align: right;
        }
    </style>
    <style type="text/css">
         .h3css{ font-family:微软雅黑; color:#4141a4; text-decoration:underline}
       .tdone{text-align:right; padding-right:10px; color:#4a4a4a; width:120px}
       .txtcss{ width:200px; border:1px #bebee1 solid; height:25px; vertical-align:middle; line-height:25px; padding:0 10px;
                color:Black;}
        .txtdoenlist{ width:103px; height:25px; line-height:25px; vertical-align:middle; padding:3px;border:1px #bebee1 solid;color:#494b4c; }
        .txtdoenlista{ width:210px; height:25px; line-height:25px; vertical-align:middle; padding:3px;border:1px #bebee1 solid;color:#494b4c; }
        .buttoncss{ width:120px;height:40px; background-color:#3776a9; color:#fff;font-family:微软雅黑; font-size:20px ; border:1px solid #3776a9; cursor:pointer }
     </style>
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1">
</asp:Content>

<asp:Content ID="Main" ContentPlaceHolderID="PlaceHolderMain" runat="server">
     <%-- 个人信息修改 --%>
    <div id="EditProfile">

        <table style="font-size: 14px; color: #494b4c" cellspacing="0" cellpadding="3">
            <tr style="display: none">
                <td class="tdone">我的帐号：</td>
                <td>
                    <%--<asp:Label ID="txtAccount" runat="server" Text=""></asp:Label>--%>
                    <%--<asp:TextBox ID="txtAccount" runat="server" CssClass="txtcss" onfocus="onftxtAccount()" onblur="onbtxtAccount()"></asp:TextBox>--%>
                </td>
            </tr>
            <asp:HiddenField ID="HiddenField1" Value="SmartNEU Ccc2008neu" runat="server" />
              <tr>
                <td colspan="2" style="text-align:center;">
                    <div id="divAdminuser" runat="server" visible ="false">管理员操作:<br />
                        <table>
                            <tr>
                                <td colspan="2"><span  style="background-color:gold">Tips:管理员账户即可以编辑自己的信息,也可以管理其他账户</span></td>
                                <td></td>
                            </tr>
                           <tr>
                                <td class="tdone">请输入操作账号：</td>
                                <td>
                                  <asp:TextBox ID="txtAccount" runat="server" CssClass="txtcss" onfocus="onftxtAccount()" onblur="onbtxtAccount()"  AutoPostBack="True"></asp:TextBox>(工号或学号)<br/><div id="divAccountMsg"  style=" color :red "   runat="server"  ></div>
                                </td>
                            </tr>
                        </table>
                    </div>
                </td>
            </tr>             

            <tr>
                <td class="tdone">姓 名：</td>
                <td>
                    <asp:TextBox ID="txtName" runat="server" CssClass="txtcss" onfocus="onft(this)" onblur="onbt(this)"></asp:TextBox><span  style="font-size:14px; color:red;  margin-left:10px" id="spanName"></span>
                </td>
            </tr>
            <tr>
                <td class="tdone">性 别：</td>
                <td style="text-align:center;">
                    <asp:RadioButtonList ID="rblSex" runat="server" RepeatDirection="Horizontal"
                        Width="200px">
                        <asp:ListItem Selected="True" Value="1">男</asp:ListItem>
                        <asp:ListItem Value="0">女</asp:ListItem>
                    </asp:RadioButtonList>
                </td>
            </tr>
            <tr><td class="tdone">单位名称：</td><td>
        
        <asp:DropDownList ID="ddlCity" runat="server" AutoPostBack="True" CssClass="txtdoenlist" Width="210px" Height="25px">

        </asp:DropDownList>
    </td></tr>
            <tr>
                <td class="tdone">联系电话：</td>
                <td>
                    <asp:TextBox ID="txtTelephone" runat="server" CssClass="txtcss" onfocus="onft(this)"
                        onblur="onbt(this)"></asp:TextBox><span  style="font-size:14px; color:red;  margin-left:10px" id="spanTel"></span></td>
            </tr>
            <tr>
                <td class="tdone">电子邮箱：</td>
                <td>
                    <asp:TextBox ID="txtEmail" runat="server" CssClass="txtcss" onfocus="onft(this)"
                        onblur="onbt(this)"></asp:TextBox><span  style="font-size:14px; color:red;  margin-left:10px" id="spanEmail"></span></td>
            </tr>
         
            <tr>
                <td class="tdone">验证码：</td>
                <td>
                    <input type = "text" id = "input" style="width:108px;height:25px;"/>  
        <input type = "button" id="code"   style="background-color: #ebebeb;cursor:pointer;font-size:13px;" onclick="createCode1()" title="点击刷新验证码" value="1234" /><span  style="font-size:14px; color:red;  margin-left:10px" id="spanValid"></span>  
             </td>
            </tr>
            <tr>
                <td colspan="2" style="height:20px"></td>
            </tr>
            <tr>
                <td colspan="2" style="text-align: center">
                    <asp:Button ID="btnSave" OnClientClick="return IsValid();" runat="server" Text="保 存" BorderStyle="None" Style="background-color:#88C4FF;" CssClass="buttoncss" ToolTip="保存修改,提交信息"/>
                    <asp:Button ID="btnUnSave" runat="server" Text="取 消" BorderStyle="None" Style="background-color:#ccc;" CssClass="buttoncss" ToolTip="取消修改,返回主页"/>
                    <asp:Button ID="btnClose" OnClientClick="return delValid()"  runat="server" Text="删 除" Style="background-color:#F66;" CssClass="buttoncss" BorderStyle="None" ToolTip="删除用户后不可恢复,请谨慎操作" />
                </td>
            </tr>
          <tr><td colspan ="2">
            <asp:Label ID="lblMsg" ForeColor="Red" runat="server" Text=""></asp:Label></td></tr>
        </table>
    </div>
</asp:Content>

<asp:Content ID="PageTitle" ContentPlaceHolderID="PlaceHolderPageTitle" runat="server">
管理账户
</asp:Content>

<asp:Content ID="PageTitleInTitleArea" ContentPlaceHolderID="PlaceHolderPageTitleInTitleArea" runat="server" >
管理账户
</asp:Content>
