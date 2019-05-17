<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Import Namespace="Microsoft.SharePoint.ApplicationPages" %>
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="cs.aspx.cs" Inherits="ItemApportion.Layouts.ItemApportion.cs"  %>


<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta name="viewport" content="width=device-width"/>
    <title>创意投票</title>
</head>
<body>
    <form id="form1" runat="server">
   <div style="width:98%;padding:5px;overflow:auto;">
<asp:GridView ID="gvCs" runat="server" AutoGenerateColumns="false"  DataKeyNames="ID"  EmptyDataText="微信投票活动已结束。" Width="50em" AllowPaging="True" PageSize="30" AllowSorting="True" CellPadding="4" ForeColor="#333333" GridLines="None" >
<AlternatingRowStyle BackColor="White" />
    <Columns>
     <asp:HyperLinkField DataNavigateUrlFields="ID" DataNavigateUrlFormatString="Details.aspx?ListItemId={0}" DataTextField="Title" HeaderText="创意名称" />
    	<asp:BoundField HeaderText="ID" DataField="ID" Visible="False"  />
        <asp:BoundField HeaderText="作者" DataField="Author"  Visible="False" />
        <asp:BoundField HeaderText="Title" DataField="Title" Visible="False"  />
        <asp:BoundField HeaderText="发布时间" DataField="PublishedDate" />
   </Columns>
   <FooterStyle BackColor="#717171" Font-Bold="True" ForeColor="White" />
        <HeaderStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
        <PagerStyle BackColor="#717171" ForeColor="White" HorizontalAlign="Center" />
        <RowStyle BackColor="#EFF3FB" />
        <SelectedRowStyle BackColor="#D1DDF1" Font-Bold="True" ForeColor="#333333" />
        <SortedAscendingCellStyle BackColor="#F5F7FB" />
        <SortedAscendingHeaderStyle BackColor="#6D95E1" />
        <SortedDescendingCellStyle BackColor="#E9EBEF" />
        <SortedDescendingHeaderStyle BackColor="#4870BE" />
</asp:GridView>
</div>
        <div>
            <table style="width: 100%;">
                <tr>
                    <th>创意名称</th>
                    <th>票数</th>
                </tr>
                <tr>
                    <td><a href="Vote.aspx">智慧校园大数据管理服务平台</a></td>
                    <td>10</td>
                </tr>
                <tr>
                    <td><a href="Vote.aspx">信息共享编辑与定向推送</a></td>
                    <td>8</td>
                </tr>
            </table>
            <asp:HyperLink ID="toVote" runat="server" NavigateUrl="Vote.aspx">投票页面</asp:HyperLink>
        </div>
    </form>
</body>
</html>