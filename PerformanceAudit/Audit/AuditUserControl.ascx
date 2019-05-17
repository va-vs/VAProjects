<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %> 
<%@ Register Tagprefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AuditUserControl.ascx.cs" Inherits="PerformanceAudit.Audit.AuditUserControl" %>
<style type="text/css">
    .Appraise {
    box-shadow: 0 1px 3px #ccc;
    background-color: white;
    background-color: rgba(255, 255, 255, 0.6);
    padding: 0 20px;
    margin-right: 10px;
    margin-top: 5px;
    width:400px;
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
    .Appraise .title a.more {
    font-weight: 400;
    font-size: 12px;
    color: #888;
    vertical-align: baseline;
    float: right;
}

       .FormList {
    }
        .showc{

            }
        .formul{
        list-style:none;
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
        <asp:Label ID="lbAppraise" runat="server" Text="业绩审核"></asp:Label>
  </div>
    <div id="AppAction" runat="server" visible="true">
        <ul class="formul">
             <li style ="text-align:left ">
               <asp:Table ID="tbContent" runat="server"></asp:Table>  
             </li>
             <li style="padding-left:22%;">
                <asp:Button ID="btnAppraise" runat="server" Text="通 过" />&nbsp;&nbsp;
                 <asp:Button ID="btnNoPass" runat="server" Text="不通过" />&nbsp;&nbsp;
                <asp:Button ID="btnCancle" runat="server" Text="返 回" />
            </li>
            <li>
                <asp:Label ID="lblMsg" ForeColor="Red" runat="server" Text=""></asp:Label>
            </li>
           

        </ul>
    </div>
</div>