<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Import Namespace="Microsoft.SharePoint.ApplicationPages" %>
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ManReg.aspx.cs" Inherits="SmartNEU.Layouts.User.ManReg" DynamicMasterPageFile="~masterurl/default.master" %>

<asp:Content ID="PageHead" ContentPlaceHolderID="PlaceHolderAdditionalPageHead" runat="server">
     <script type ="text/javascript" src="../SmartNEU/Validate.js" ></script>
    <script  type="text/javascript">
        window.onbeforeunload = function () {
            var n = window.event.screenX - window.screenLeft;
            var b = n > document.documentElement.scrollWidth - 20;
            if (b && window.event.clientY < 0 || window.event.altKey)  // 是关闭而非刷新 
            {
                document.getElementById('btnClose').click();
            }
        }
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
            code=returnCode();
            checkCode.value = code;//把code值赋给验证码  
        }
        function createCode1() {

            var checkCode = document.getElementById("code");
            code = returnCode();
            checkCode.value = code;//把code值赋给验证码  
        }
       
        function IsValid() {

           <%-- var chkValue = getCookie("CheckCode")
            if (chkValue == null) {
                alert("您的浏览器设置已被禁用 Cookies，您必须设置浏览器允许使用 Cookies 选项后才能使用本系统。");
                return false;
            }
            var checkCode = document.getElementById('<%=txtCheckCode.ClientID%>');

            if (checkCode.value.length == 0 || checkCode.value.toLowerCase() != chkValue.toLowerCase()) {
                alert("验证码错误");
                return false;
            }--%>

         var name = document.getElementById('<%=txtAccount.ClientID%>');
            if (name.value.length == 0) {
                //alert("登录帐号不能为空！");
                //document.getElementById("spanAcc").style.display = "";
                //document.getElementById("spanAcc").innerHTML = "登录帐号不能为空！";
                spanAcc.innerHTML = "登录帐号不能为空！";
                name.select();
                return false;
            }
            //else if (!check_Number(name.value)) {
            //    spanAcc.innerHTML = "只能输入工号或学号！";
            //    name.select();
            //    return false;
            //}
            else
                spanAcc.innerHTML = "";

            var pass = document.getElementById('<%=txtPwd.ClientID%>');
         if (!pass.getAttribute("readonly")) {
             if (pass.value.length < 8) {
                 //alert("密码长度不能小于6！");
                 spanPwd.innerHTML = "密码长度不能小于8,至少包含以下四类字符中的三类：大写字母、小写字母、数字、以及英文标点符号！";
                 pass.select();
                 return false;
             }
             if (pass.value != document.getElementById('<%=txtPwd1.ClientID%>').value) {
                 //alert("密码与确认密码不一致！");
                 spanPwd1.innerHTML = "密码与确认密码不一致！";
                document.getElementById('<%=txtPwd1.ClientID%>').select();
                return false;
            }
        }
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
            return true;
        }
    </script>

     <style type="text/css">
         .h3css{ font-family:微软雅黑; color:#4141a4; text-decoration:underline}
       .tdone{text-align:right; padding-right:10px; color:#4a4a4a; width:120px;vertical-align:top;}
       .txtcss{ width:200px; border:1px #bebee1 solid; height:25px; vertical-align:middle; line-height:25px; padding:0 10px;
                color:Black;}
        .txtdoenlist{ width:103px; height:25px; line-height:25px; vertical-align:middle; padding:3px;border:1px #bebee1 solid;color:#494b4c; }
        .txtdoenlista{ width:210px; height:25px; line-height:25px; vertical-align:middle; padding:3px;border:1px #bebee1 solid;color:#494b4c; }
        .buttoncss{ width:120px;height:40px; background-color:#3776a9; color:#fff;font-family:微软雅黑; font-size:20px ; border:1px solid #3776a9; cursor:pointer }
     </style>
    <style type="text/css">
            .stable {
                width: 800px;
                margin: 0px auto;
                text-align: left;
                position: relative;
                border-top-left-radius: 5px;
                border-top-right-radius: 5px;
                border-bottom-right-radius: 5px;
                border-bottom-left-radius: 5px;
                font-size: 14px;
                font-family: 微软雅黑, 黑体;
                line-height: 1.5;
                box-shadow: rgb(153, 153, 153) 0px 0px 5px;
                border-collapse: collapse;
                background-position: initial initial;
                background-repeat: initial initial;
                background: #fff;
            }
            
            .stable th {
                height: 25px;
                text-align: center;
                line-height: 25px;
                padding: 15px 35px;
                border-bottom-width: 1px;
                border-bottom-style: solid;
                border-bottom-color: #C46200;
                background-color: #FEA138;
                border-top-left-radius: 5px;
                border-top-right-radius: 5px;
                border-bottom-right-radius: 0px;
                border-bottom-left-radius: 0px;
            }
            
            .f_blue:link {
                color: #0000ff;
                text-decoration: none;
            }
            
            .f_blue:visited {
                color: #0000ff;
                text-decoration: none;
            }
            
            .f_blue:hover {
                color: #ff2020;
                text-decoration: underline;
            }
            
            .f_blue:actived {
                color: #0000ff;
                text-decoration: none;
            }
        </style>
</asp:Content>

<asp:Content ID="Main" ContentPlaceHolderID="PlaceHolderMain" runat="server">

<div id="divregInfo" runat="server">
    <table style=" font-size:14px; color:#494b4c" cellspacing="0" cellpadding="3">
        <tr>
            <td class="tdone">登录帐号：<span style="color:red">*</span></td>
            <td>
                <asp:TextBox ID="txtAccount" runat="server" CssClass="txtcss" onfocus="onftxtAccount()" onblur="onbtxtAccount()" ToolTip="必填项！注册账户为字母或数字组合" AutoPostBack="True"></asp:TextBox> <br/><span  style="font-size:14px; color:red;  margin-left:10px" id="spanAcc"></span><div id="divAccountMsg"  style=" color :red "   runat="server"  ></div></td>

        </tr>
        <tr>
            <td class="tdone">密 码：<span style="color:red">*</span></td>
            <td>
                <asp:TextBox ID="txtPwd" runat="server" TextMode="Password" CssClass="txtcss" onfocus="onfocusjs()" onblur="onblurjs()" ToolTip="必填项！密码最少8位,至少包含以下四类字符中的三类：大写字母、小写字母、数字、以及英文标点符号" ></asp:TextBox><span  style="font-size:14px; color:red;  margin-left:10px" id="spanPwd"></span><br/>
               <asp:Label ID="lblPwdMsg" ForeColor="Red" runat="server" Text=""></asp:Label>
            </td>
        </tr>
        <tr>
            <td class="tdone">确认密码：<span style="color:red">*</span></td>
            <td>
                <asp:HiddenField ID="HiddenField1" Value="SmartNEU Ccc2008neu" runat="server" />
                <asp:TextBox ID="txtPwd1" runat="server" TextMode="Password" CssClass="txtcss" onfocus="onft(this)" onblur="onblurjs()" ToolTip="必填项！两次密码必须一致" ></asp:TextBox><span  style="font-size:14px; color:red;  margin-left:10px" id="spanPwd1"></span><br/>
               <asp:Label ID="lblPwd1Msg" ForeColor="Red" runat="server" Text=""></asp:Label>
            </td>
        </tr>
        <tr>
            <td class="tdone">姓名：<span style="color:red">*</span></td>
            <td>
                <asp:TextBox ID="txtName" runat="server" CssClass="txtcss" onfocus="onft(this)" onblur="onbtName()" ToolTip="必填项！姓名只能输入中文"  ></asp:TextBox><span  style="font-size:14px; color:red;  margin-left:10px" id="spanName"></span><br/>
               <asp:Label ID="lblNameMsg" ForeColor="Red" runat="server" Text=""></asp:Label>
            </td>
        </tr>
        <tr>
            <td class="tdone">性别：</td>
            <td style="text-align:center;">
                <asp:RadioButtonList ID="rblSex" runat="server" RepeatDirection="Horizontal" Width="160px">
                    <asp:ListItem Selected="True" Value="1">男</asp:ListItem>
                    <asp:ListItem Value="0">女</asp:ListItem>
                </asp:RadioButtonList>
            </td>
        </tr>
        <tr>
            <td class="tdone">单位名称：</td>
            <td>
                <asp:DropDownList ID="ddlCity" runat="server" AutoPostBack="True" CssClass="txtdoenlist" Width="210px">
                </asp:DropDownList>
            </td>
        </tr>
        <tr>
            <td class="tdone">联系电话：</td>
            <td>
                <asp:TextBox ID="txtTelephone" runat="server" CssClass="txtcss" onfocus="onft(this)" onblur="onbtTel()" ToolTip="选填！电话格式必须正确，比如：18888888888" ></asp:TextBox><span  style="font-size:14px; color:red;  margin-left:10px" id="spanTel"></span><br/>
               <asp:Label ID="lblTelMsg" ForeColor="Red" runat="server" Text=""></asp:Label>
            </td>
        </tr>
        <tr>
            <td class="tdone">电子邮箱：<span style="color:red">*</span></td>
            <td>
                <asp:TextBox ID="txtEmail" runat="server" CssClass="txtcss" onfocus="onft(this)" onblur="onbtEmail()" ToolTip="必填项！邮箱格式必须正确，比如：example@mail.com"></asp:TextBox><span  style="font-size:14px; color:red;  margin-left:10px" id="spanEmail"></span><br/>
               <asp:Label ID="lblEmailMsg" ForeColor="Red" runat="server" Text=""></asp:Label>
            </td>
        </tr>

        <tr>
            <td class="tdone">验证码：<span style="color:red">*</span></td>
            <td>
                <input type="text" id="input" style="width:100px;height:25px;" />
                <input type="button" id="code" style="background-color: #ebebeb;cursor:pointer;font-size:13px;" onclick="createCode1()" value="a1b2" title="点此刷新验证码" /><span  style="font-size:14px; color:red;  margin-left:10px" id="spanValid"></span>
            </td>
        </tr>
        <tr>
            <td colspan="2" style="height:20px;"></td>
        </tr>
        <tr>
            <td>&nbsp;</td>
            <td style="text-align:left">
                <asp:Button ID="btnSave" OnClientClick="return IsValid();" runat="server" Text="注 册" BorderStyle="None" style="background-color:#88C4FF;cursor:pointer;" /> &nbsp; &nbsp;
                <asp:Button ID="btnClose" runat="server" Text="取 消" BorderStyle="None" style="background-color:#dedede;cursor:pointer;" />
            </td>
        </tr>
        <tr><td colspan ="2">
            <asp:Label ID="lblMsg" ForeColor="Red" runat="server" Text=""></asp:Label></td></tr>

    </table>
</div>
     <div id="divregSuccess" runat="server" visible="false">
            <table cellpadding="0" align="center" class="stable">
                <tbody>
                    <tr>
                        <th valign="middle">
                            <font face="微软雅黑" size="5" style="color: rgb(255, 255, 255); ">国际满学研究</font>
                        </th>
                    </tr>
                    <tr>
                        <td>
                            <div style="padding:25px 35px 40px; background-color:#fff;">
                                <h2 style="margin: 5px 0px; ">
                                    <font color="#333333" style="line-height: 20px; ">
                                        <font style="line-height: 22px; " size="4">尊敬的<asp:Label ID="lbuserName" runat="server" Text=""></asp:Label>：</font>
                                    </font>
                                </h2>
                                <div style="line-height:25px;">首先感谢您加入国际满学研究！<br/> 您的登录账号是<asp:Label ID="lbuseAcc" runat="server" Text=""></asp:Label>，请在登录页面中输入“<asp:Label ID="lbuserADAcc" runat="server" Text=""></asp:Label>”账号格式和密码登录。
                                    <br/> 请您在发表内容时，遵守当地法律法规及。
                                    <br/> <br/>
                                    <div style="text-align:center;" runat ="server" id="divResp">
                                       
                                    </div>
                                    <br/><br/>
                                </div>
                                <p align="right">国际满学研究</p>
                                <p align="right">
                                    <asp:Label ID="lbDateNow" runat="server" Text=""></asp:Label>
                                </p>
                            </div>
                        </td>
                    </tr>
                </tbody>
            </table>
        </div>
</asp:Content>

<asp:Content ID="PageTitle" ContentPlaceHolderID="PlaceHolderPageTitle" runat="server">
用户注册
</asp:Content>

<asp:Content ID="PageTitleInTitleArea" ContentPlaceHolderID="PlaceHolderPageTitleInTitleArea" runat="server" >
用户注册
</asp:Content>
