<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %> 
<%@ Register Tagprefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="PerformAttachmentUserControl.ascx.cs" Inherits="UploadAttachment.PerformAttachment.PerformAttachmentUserControl" %>
<style type="text/css">
    .Appraise {
    box-shadow: 0 1px 3px #ccc;
    background-color: white;
    background-color: rgba(255, 255, 255, 0.6);
    padding: 0 20px;
    margin-right: 10px;
    margin-top: 5px;
    width:500px;
}
    .Appraise .title {
    height: 50px;
    line-height: 50px;
    font-family: "微软雅黑", "PingFang SC", sans;
    font-weight: 600;
    color: #000;
    font-size: 20px;
    border-bottom: 1px solid #ccc;
}
      
        .formul{
        list-style:none;
        margin-left:auto;
        }
        .formul li{
        line-height:25px;
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
     </style>
<div id="AppraiseDiv" class="Appraise" runat ="server">
 <div class="title">
        <asp:Label ID="lbAppraise" runat="server" Text="附件"></asp:Label>
  </div>
    <div id="AppAction" runat="server" visible="true">

        <ul class="formul">
             <li style ="text-align:left ">
               <asp:Table ID="tbContent" runat="server"></asp:Table>  

             </li>
             <li >
                 <div runat ="server" id ="divUpload">
                     <asp:FileUpload ID="FileUpload1" runat="server" />&nbsp;&nbsp;<asp:Button ID="btnAppraise" runat="server" Text="上传" /></div>
            </li>
            <li>
                <asp:Label ID="lblMsg" ForeColor="Red" runat="server" Text=""></asp:Label>
            </li>
           

        </ul>
    </div>
</div>