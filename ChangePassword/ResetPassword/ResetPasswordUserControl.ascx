<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %> 
<%@ Register Tagprefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ResetPasswordUserControl.ascx.cs" Inherits="ChangePassword.ResetPassword.ResetPasswordUserControl" %>
<script  type="text/javascript">
     var code; //在全局定义验证码
     window.onload = function createCode() {

            var checkCode = document.getElementById("code");
            code=returnCode();
            checkCode.value = code;//把code值赋给验证码
            var error = document.getElementById("<%=UserID.ClientID %>" + "_errorLabel");
            error.style.display = "none";
        }
        function createCode1() {

            var checkCode = document.getElementById("code");
            code = returnCode();
            checkCode.value = code;//把code值赋给验证码
        }
            //生成验证码
        function returnCode() {
            var code = "";
            var codeLength = 4;//验证码的长度  
            var random = new Array(0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R',
           'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z');//随机数  
            for (var i = 0; i < codeLength; i++) {//循环操作  
                var index = Math.floor(Math.random() * 36);//取得随机数的索引（0~35）  
                code += random[index];//根据索引取得随机数加到code上  
            }
            return code;
        }
    function isValid() {
        <%-- var name1 = document.getElementById('<%=txtAccount.ClientID%>');
            if (name1.value.length == 0) {
                alert("帐号不能为空！");
                name1.select();
                return false;
            }--%>
          var client = document.getElementById("<%=UserID.ClientID %>" + "_checkNames");
            client.click();
            var error = document.getElementById("<%=UserID.ClientID %>" + "_errorLabel");
            var user = document.getElementById("<%=UserID.ClientID %>" + "_hiddenSpanData");
        
        
        if  (user.value=="" && (error.innerHTML=="" || error.innerHTML == ("必须为此必填字段指定值。"))) {
            error.style.display = "none";
            document.getElementById("<%=UserID.ClientID %>").focus();
            spanUser.innerHTML = "帐号不能为空！";
            return false;
        } else
            spanUser.innerHTML = "";
          
            var pass = document.getElementById('<%=txtPwd.ClientID%>');
            if (!pass.getAttribute("readonly")) {
                if (pass.value.length < 8) {
                    //alert("密码长度不能小于8！");
                    spanPwd.innerHTML = "密码长度不能小于8！";
                    pass.select();
                    return false;
                }
                else
                    spanPwd.innerHTML = "";
                if (pass.value != document.getElementById('<%=txtPwd1.ClientID%>').value) {
                    //alert("两次密码不一致！");
                    spanPwd1.innerHTML = "两次密码不一致！";
                    document.getElementById('<%=txtPwd1.ClientID%>').select();
                    return false;
                }
                else
                    spanPwd1.innerHTML = "";
        }
        var inputCode = document.getElementById("input").value.toUpperCase(); //取得输入的验证码并转化为大写
            if (inputCode.length <= 0) { //若输入的验证码长度为0
                //alert("请输入验证码！"); //则弹出请输入验证码
                spanValid.innerHTML = "请输入验证码！";
                document.getElementById("input").focus();
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
       .tdone{text-align:right; padding-right:10px; color:#4a4a4a; width:150px}
       .txtcss{ width:200px; border:1px #bebee1 solid; height:25px; vertical-align:middle; line-height:25px; padding:0 10px;
                color:Black;}
        .txtdoenlist{ width:103px; height:25px; line-height:25px; vertical-align:middle; padding:3px;border:1px #bebee1 solid;color:#494b4c; }
        .txtdoenlista{ width:210px; height:25px; line-height:25px; vertical-align:middle; padding:3px;border:1px #bebee1 solid;color:#494b4c; }
        .buttoncss{ width:120px;height:40px; background-color:#3776a9; color:#fff;font-family:微软雅黑; font-size:20px ; border:1px solid #3776a9; cursor:pointer }
     </style>
 <table style="font-size:14px; color:#494b4c"  cellspacing="0"  cellpadding="3">
    <tr>
        <td  class="tdone">请输入帐号：<span style="color:red">*</span></td>
        <td>
        <SharePoint:PeopleEditor id="UserID" runat="server" SelectionSet="User"  Width="210px"  ValidatorEnabled="true" AllowEmpty = "False" MultiSelect = "false" />
           <span style="font-size: 14px; color:red; margin-left: 10px" id="spanUser"></span>
        </td>
    </tr>
    <tr>
        <td  class="tdone">请输入新密码：<span style="color:red">*</span></td>
        <td>
        <asp:TextBox ID="txtPwd"  TextMode="Password" runat="server"  CssClass="txtcss"></asp:TextBox>
           <span style="font-size: 14px; color: red; margin-left: 10px" id="spanPwd"></span>
        </td>
    </tr>
    <tr>
        <td  class="tdone">请再次输入新密码：<span style="color:red">*</span></td>
        <td>
        <asp:TextBox ID="txtPwd1"  TextMode="Password" runat="server"  CssClass="txtcss"></asp:TextBox>
            <span style="font-size: 14px; color: red; margin-left: 10px" id="spanPwd1"></span>
        </td>
    </tr>
    <tr>
                <td class="tdone">请输入验证码：<span style="color: red">*</span><asp:HiddenField ID="HiddenField1" Value="Ccc2008neu" runat="server" /></td>
                <td>
                    <input type="text" id="input" style="width: 105px; height: 25px;" />
                    <input type="button" id="code" style="background-color: #ebebeb; cursor: pointer; font-size: 13px;" onclick="createCode1()" value="a1b2" title="点此刷新验证码" /><span style="font-size: 14px; color: red; margin-left: 10px" id="spanValid"></span>
                </td>
   </tr>
   <tr>
            <td colspan="2" style="heght:10px;">
                
            </td>
        </tr>
    <tr>
        <td>&nbsp;</td>
        <td  style="text-align:left;padding-left:30px;"><asp:Button ID="btnSubmit" OnClientClick="return isValid();" runat="server" Text="提交密码重置"   /></td>
    </tr>
    <tr>
        <td colspan="2">
            <div style="color: #0072c6; padding-left: 10px;width:600px; "  id="udesp" runat="server"><ul><li>首先请确认账号正确，两次密码必须一致</li><li>密码长度不得小于 8 位</li><li>至少包含以下四类字符中的三类：大写字母、小写字母、数字、以及英文标点符号</li></ul></div>
        </td>
    </tr>
     <tr>
            <td colspan="2">
                <asp:Label ID="lblMsg" ForeColor="Red" runat="server" Text=""></asp:Label>
            </td>
        </tr>
</table>