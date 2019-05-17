<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %> 
<%@ Register Tagprefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AssistantResultsUserControl.ascx.cs" Inherits="AssistantResults.AssistantResults.AssistantResultsUserControl" %>
<style type="text/css">
    .Appraise {
    box-shadow: 0 1px 3px #ccc;
    background-color: white;
    background-color: rgba(255, 255, 255, 0.6);
    padding: 0 10px;
    margin-right: 10px;
    margin-top: 2px;
    width:500px;
}
    .Appraise .title {
    height: 25px;
    line-height: 25px;
    font-family: "微软雅黑", "PingFang SC", sans;
    font-weight: 600;
    color: #000;
    font-size: 18px;
    border-bottom: 0px solid #ccc;
}
      
        .formul{
        list-style:none;
        margin-left:auto;
        }
        .formul li{
        line-height:20px;
        padding-bottom:10px;
        }
        .formul li button{
        font-size:14px;
        }
        .att{
            background-color:#cfcfcf;
            color:red;
            font-size:14px;
        }
        .mytable{
        word-wrap:break-word;
        width:100%;
        border:none;
        border-collapse: collapse;
        }
        .mytable tr:nth-child(odd){
        background-color:#eee;
        border:1px solid #eee;
        }
        .mytable tr:hover{
        background-color:#bbb;
        color:#fff;
        }
        .mytable tr td{
        max-width:360px;
        word-wrap:break-word;
        min-width:50px;
        border:0px;
        padding:2px 5px 2px 5px;
        }
         .mytable tr td:last-child{
        text-align:left;
        border-right:0px;
        }
     </style>
<div id="divTotal"  class="Appraise" runat ="server">
 <div class="title">
        <asp:Label ID="lbAppraise" runat="server" Text="附件"></asp:Label>
  </div>
    <div id="AppAction" runat="server" visible="true">

        <ul class="formul">

             <li style ="text-align:left ">
                 <asp:Table ID="tbResult" CssClass="mytable"  runat="server"></asp:Table>
                 <asp:Label ID="lblMsgNoResults" ForeColor="Red" runat="server" Text="" Visible="false"></asp:Label>
             </li>
             <li style ="text-align:left ">
              <table  runat ="server" id="tblNavi" border ="0"  visible ="false"  >
                  <tr><td><asp:Button ID="lnk1" runat="server" Text ="已有文档"></asp:Button></td>
                  <td><asp:Button ID="lnk2" runat="server" Text="上传文档"></asp:Button></td>
                  <td><asp:Button ID="lnk3" runat="server" Text="添加链接" Visible="false"></asp:Button></td> </tr>
              </table>
             </li>
             <li style ="text-align:left ">
              <div runat="server" id="divDocs"  style =" height:400px;overflow-x:hidden ;overflow-y:auto "> <asp:Table ID="tbContent" CssClass ="mytable" runat="server"></asp:Table>  </div>
             </li>
             <li >
                 <div runat ="server" id ="divUpload" visible ="false">
                    <table border ="0">
                <tr><td>添加新附件</td></tr>
                        
             <tr><td><asp:FileUpload ID="FileUpload1" runat="server" />&nbsp;&nbsp;<asp:Button ID="btnAppraise" runat="server" Text="上传" /></td></tr> 
                        <tr><td><div style="color: #0072c6;">支持的文件类型：<%=webObj.LnkDescription  %></div></td></tr>
    </table>
                     </div>
                 <div runat="server" id="divLinks" visible ="false"  >
                                          <table border ="0">
                        <tr><td>添加新链接</td></tr>
                         <tr><td>请键入网址：<asp:TextBox Width ="250" ID="txtUrl" runat="server"></asp:TextBox>
                             <asp:Label ID="lblMsgLnks" ForeColor="Red" runat="server" Text=""></asp:Label>
                             </td></tr>
                         <tr><td>请键入说明：<asp:TextBox  Width ="250" ID="txtDescrip" runat="server"></asp:TextBox></td></tr>
                          
                         <tr><td style="text-align:right " ><asp:Button ID="btnAdd" runat="server" Text="添加" />&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</td></tr>
                     </table>
                 </div>
            </li>
            <li>
                
            </li>
           

        </ul>
    </div>
    <asp:Label ID="lblMsg" ForeColor="Red" runat="server" Text=""></asp:Label>
</div>