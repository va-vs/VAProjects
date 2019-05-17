<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %> 
<%@ Register Tagprefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="WriteUserControl.ascx.cs" Inherits="PerformanceWrite.Write.WriteUserControl" %>
<style type="text/css">
    .Appraise {
    box-shadow: 0 1px 3px #ccc;
    background-color: white;
    background-color: rgba(255, 255, 255, 0.6);
    padding: 0 20px;
    margin-right: 10px;
    margin-top: 5px;
    width:95%;
}
    .Appraise .title {
    height: 50px;
    line-height: 50px;
    font-family: "微软雅黑", "PingFang SC", sans;
    font-weight: 600;
    color: #000;
    font-size: 20px;
    border-bottom: 1px solid #ccc;
    padding-left:10px;
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
        .appTable{
            width: 100%;
            line-height:30px;
        }
        .appTable th{
            font-weight:normal;
            text-align:right;
        }
        .appTable td{
            font-weight:normal;
            text-align:left;
        }
        .appTable td button{
            padding:5px;
            height:22px;
            width:80px;
        }
     </style>
<div id="AppraiseDiv" class="Appraise" runat ="server">
 <div class="title">
        <asp:Label ID="lbAppraise" runat="server" Text="业绩分配"></asp:Label>
  </div>
    <table style="font-size: 14px; color: #494b4c" cellspacing="0" cellpadding="3" runat ="server" id="tbSettings">
        <tr><td colspan="2">
<asp:GridView ID="gvGoalSetting" runat="server" CellPadding="4" ForeColor="#333333" GridLines="None" Width="558px" AutoGenerateColumns="false" DataKeyNames="ID"  >
    <EmptyDataTemplate>
        <asp:Label ID="Label1" ForeColor="Red" runat="server" Text="没有业绩分配记录！"></asp:Label>
    </EmptyDataTemplate>
    <AlternatingRowStyle BackColor="White" />
    <Columns>
    
    <asp:TemplateField  HeaderText="干系人" ItemStyle-HorizontalAlign="center"><ItemTemplate>
        <SharePoint:PeopleEditor runat="server" Width="160px" Height="20px" SelectionSet="User"  ValidatorEnabled="true" AllowEmpty="true" ID="tbName1" MultiSelect="false" />
      </ItemTemplate>
    </asp:TemplateField>
    <asp:TemplateField  HeaderText="系数" ItemStyle-HorizontalAlign="center"><ItemTemplate>
        <asp:TextBox ID="tbRatio1" Text='<%#Eval("Ratio")%>' runat="server" Width="150px" Height="20px"></asp:TextBox>
      </ItemTemplate>
    </asp:TemplateField>
    <asp:TemplateField  HeaderText="" ItemStyle-HorizontalAlign="center"><ItemTemplate>
        <asp:LinkButton ID="lkbdelete" runat="server" CommandName="Del"  Style="font-size: 12px;" CommandArgument='<%#Eval("ID")%>' OnClientClick="return confirm('确认要删除本条业绩分配记录吗！');">删 除</asp:LinkButton>
      </ItemTemplate>
    </asp:TemplateField>
    </Columns>

    <EditRowStyle BackColor="#2461BF" />
    <FooterStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
    <HeaderStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
    <PagerStyle BackColor="#2461BF" ForeColor="White" HorizontalAlign="Center" />
    <RowStyle BackColor="#EFF3FB" />
    <SelectedRowStyle BackColor="#D1DDF1" Font-Bold="True" ForeColor="#333333" />
    <SortedAscendingCellStyle BackColor="#F5F7FB" />
    <SortedAscendingHeaderStyle BackColor="#6D95E1" />
    <SortedDescendingCellStyle BackColor="#E9EBEF" />
    <SortedDescendingHeaderStyle BackColor="#4870BE" />
</asp:GridView>
   </td></tr>
     <tr><td><asp:Label ID="lbDes" runat="server" Text=""></asp:Label></td><td style ="text-align:right ">
         <asp:Button ID="btnNew" runat="server" Text="新 建"  Visible ="false"/>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<asp:Button ID="btnSave" runat="server"   Text="保  存" />&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</td></tr>
        <tr>
                <td colspan="2">
                    <div style="color: #a0a0a0;padding-left:75px;"><%=webObj.UserDesp %></div>
                </td></tr>
<tr><td colspan="2">
<asp:Label ID="lblMsg" ForeColor="Red" runat="server" Text=""></asp:Label> </td></tr>

 </table>
    <div id="AppAction" runat="server" visible="false">
        <table class="appTable">
            <tr>
                <th>
                    <asp:Label ID="lblAuthors" runat="server" Text="分配记录：" Font-Bold="true"></asp:Label>
                </th>
                <td>
                    <asp:RadioButtonList ID="rblAuthors" runat="server" AutoPostBack="True" RepeatDirection="Horizontal"></asp:RadioButtonList>
                </td>
            </tr>
            <tr>
                <th>
                    <asp:Label ID="lblName" runat="server" Text="干系人："></asp:Label>
                </th>
                <td>
                    <SharePoint:PeopleEditor runat="server" Width="160px" Height="20px" SelectionSet="User" ValidatorEnabled="true" AllowEmpty="true" ID="tbName" MultiSelect="false" />
                </td>
            </tr>
            <tr>
                <th>
                    <asp:Label ID="lblRatio" runat="server" Text="系数："></asp:Label>
                </th>
                <td>
                    <asp:TextBox ID="tbRatio" runat="server" Width="150px" Height="20px"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td colspan="2" style="padding-left:100px;padding-top:10px;">
<%--                    <asp:Button ID="btnSave" runat="server" Text="保 存" />&nbsp;&nbsp;
                    <asp:Button ID="btnNew" runat="server" Text="新 建" />&nbsp;&nbsp;
                    <asp:Button ID="btnRet" runat="server" Text="删 除"  OnClientClick="return confirm('确认要删除本条业绩分配记录吗?')"/>--%>
                </td>
            </tr>
           <%-- <tr>
                <td colspan="2">
                    <div style="color: #a0a0a0;padding-left:75px;"><%=webObj.UserDesp %></div>
                </td></tr>--%>
                <tr>
                    <td colspan="2" style="padding-left:25px;">
                        <%--<asp:Label ID="lblMsg" ForeColor="Red" runat="server" Text=""></asp:Label>--%>
                    </td>
                </tr>
        </table>
    </div>
</div>