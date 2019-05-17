<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %> 
<%@ Register Tagprefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ImportFromExcelUserControl.ascx.cs" Inherits="VATaskWebPart.ImportFromExcel.ImportFromExcelUserControl" %>
<style type="text/css">
    .Appraise {
    box-shadow: 0 1px 3px #ccc;
    background-color: white;
    background-color: rgba(255, 255, 255, 0.6);
    margin-right: 5px;
    margin-top: 5px;
    width:600px;
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

    <div id="AppAction" runat="server" visible="true">
        <ul class="formul">
            <li>
                <table border ="0">
                    <tr><td>
                        <asp:RadioButtonList ID="rbOpt" RepeatDirection="Horizontal"  runat="server" AutoPostBack="True"><asp:ListItem Selected="True" Value="1">按日期</asp:ListItem>
                             <asp:ListItem Value="2">按行号</asp:ListItem><asp:ListItem Value="3">全部</asp:ListItem></asp:RadioButtonList> </td>
                        <td>
                         <span id="spanLine" runat ="server" visible ="false"  >开始行号：<asp:TextBox ID="txtStart" runat="server" Width="50"></asp:TextBox>&nbsp;&nbsp &nbsp;&nbsp;结束行号：<asp:TextBox ID="txtEnd" runat="server" Width="50"></asp:TextBox></span></td>
                        </tr>
                    <tr><td colspan="2"><table id ="spanDate" runat ="server" border ="0"><tr><td>开始日期：</td><td><SharePoint:DateTimeControl ID="dtStart" runat="server" DateOnly="True"    /></td><td>结束日期：</td><td><SharePoint:DateTimeControl ID="dtEnd" runat="server" DateOnly="True" /></td></tr></table></td></tr>
                   
                </table>
            </li>
            <li >
                 <div runat ="server" id ="divUpload">
                     <asp:FileUpload ID="FileUpload1" runat="server" />&nbsp;&nbsp;<asp:Button ID="btnImport" runat="server" Text="导入到列表" /></div>
            </li>
          
            <li>
                <asp:Label ID="lblMsg" ForeColor="Red" runat="server" Text=""></asp:Label>
            </li>
           

        </ul>
    </div>
</div>