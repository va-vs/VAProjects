<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %> 
<%@ Register Tagprefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ImportFromExcelUserControl.ascx.cs" Inherits="SPListToExcel.ImportFromExcel.ImportFromExcelUserControl" %>
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
                <table>
                    <tr>
                        <td style="padding:10px;">年度：<asp:DropDownList ID="ddlYear" runat="server" AutoPostBack="False"></asp:DropDownList></td><td>&nbsp;&nbsp;系部：<asp:DropDownList ID="ddlDep" runat="server" AutoPostBack="False"></asp:DropDownList></td> 
                        <td style="padding:10px;">教师：</td> 
                        <td style="padding:10px;"><SharePoint:PeopleEditor id="UserID" runat="server" SelectionSet="User" ValidatorEnabled="true" Width="140" AllowEmpty = "true" MultiSelect = "false" /></td>            
                    </tr>
                    <tr>
                        <td style="padding:10px;" colspan="4" >
                        <div runat ="server" id ="divUpload">
                     <asp:FileUpload ID="FileUpload1"  runat="server"  Width ="280px" />&nbsp;&nbsp;&nbsp;&nbsp;<asp:Button ID="btnImport" runat="server" Text="导入到业绩汇总" />
                 </div>
                        </td>
                    </tr>
                    <tr><td style="padding:10px;" colspan="4">
                    <asp:Label ID="lblMsg" ForeColor="Red" runat="server" Text=""></asp:Label>
                        </td></tr>
                 </table>
            </li>
        </ul>
    </div>
</div>