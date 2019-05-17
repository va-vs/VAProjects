<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Register Tagprefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ExportToExcelUserControl.ascx.cs" Inherits="SPListToExcel.ExportToExcel.ExportToExcelUserControl" %>
<style type="text/css">
    .Appraise {
    box-shadow: 0 1px 3px #ccc;
    background-color: white;
    background-color: rgba(255, 255, 255, 0.6);
    margin-right: 10px;
    margin-top: 5px;
    width:800px;
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
        .btn{
            font-weight:bold;
            cursor:pointer;
        }
/*加载中。。。。。*/
        .loading{
            width: 150px;
            height: 8px;
            border-radius: 4px;
            margin: 0 auto;
            margin-top:100px;
            position: relative;
            background: lightblue;
            overflow: hidden;
        }
        .loading span{
            display:block;
            width: 100%;
            height: 100%;
            border-radius: 3px;
            background: lightgreen;
            -webkit-animation: changePosition 4s linear infinite;
        }
        @-webkit-keyframes changePosition{
            0%{
                -webkit-transform: translate(-150px);
            }
            50%{
                -webkit-transform: translate(0);
            }
            100%{
                -webkit-transform: translate(150px);
            }
        }
     </style>
<div id="AppraiseDiv" class="Appraise" runat ="server">

    <div id="AppAction" runat="server" visible="true">
        <ul class="formul">
             <li style ="text-align:left ">
                 <table>
                    <tr>
                        <td style="padding:10px;">年度：<asp:DropDownList ID="ddlYear" runat="server" AutoPostBack="False"></asp:DropDownList></td><td>&nbsp;&nbsp;系部：<asp:DropDownList ID="ddlDep" runat="server" AutoPostBack="False"></asp:DropDownList></td>
                        <td style="padding:10px;">教师：</td>
                        <td style="padding:10px;"><SharePoint:PeopleEditor id="UserID" runat="server" SelectionSet="User" ValidatorEnabled="true" Width="140" AllowEmpty = "true" MultiSelect = "false" /></td>
                    </tr>
                     <tr>
                        <td colspan="4" style="padding:10px;">
                            <asp:CheckBoxList runat="server" ID ="chkLists" RepeatColumns="3" CellSpacing="2" CellPadding="2">

                            </asp:CheckBoxList>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="4" style="align-content:center;padding:10px;">
                            <asp:GridView runat="server" ID ="GridView1" Width ="100%" CellPadding="1" HorizontalAlign="Left"></asp:GridView>
                        </td>
                    </tr>
                    <tr>
                        <td style="padding:10px;text-align:right">
                            <asp:Button ID="btnOpt"  runat="server" Text="全 清" Width="50" Height="30" CssClass="btn"/>
                        </td>
                        <td style="padding:10px;text-align:left">
                            <asp:Button ID="btnExport" runat="server" Text="导出到Excel" Width="100" Height="30" CssClass="btn" />
                        </td>
                        <td colspan="2">
                            <asp:Button ID="btnExportNo" runat="server" Text="导出不全数据" Width="100" Height="30" CssClass="btn" />
                        </td>
                    </tr>
                     <tr>
                         <td colspan ="4"  style="padding:10px;">
                             <div class="loading" id="loading" runat="server" visible="false">
                                 <span></span>
                            </div>
                             <div runat="server" id ="divSaveAs" style="text-align:center;padding-left:20px;color :red"></div> </td>
                     </tr>
                </table>

             </li>
            

        </ul>
    </div>
        <asp:Label ID="lblMsg" ForeColor="Red" runat="server" Text=""></asp:Label>
</div>