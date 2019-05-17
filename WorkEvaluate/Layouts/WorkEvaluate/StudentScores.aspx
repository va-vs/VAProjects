<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Import Namespace="Microsoft.SharePoint.ApplicationPages" %>
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="StudentScores.aspx.cs" Inherits="WorkEvaluate.Layouts.WorkEvaluate.StudentScores" DynamicMasterPageFile="~masterurl/default.master" %>

<asp:Content ID="PageHead" ContentPlaceHolderID="PlaceHolderAdditionalPageHead" runat="server">
 <style type="text/css">
        .h3css
        {
            font-family: 微软雅黑;
            color: #4141a4;
            text-decoration: underline;
        }
        .tdone
        {
             width:80px;
            text-align:left;
            padding-left:10px;
            color: #4a4a4a;
        }
        .txtcss
        {
            width: 300px;
            border: 1px #bebee1 solid;
            height: 25px;
            vertical-align: middle;
            line-height: 25px;
            padding: 0 10px;
            color: Black;
        }
        .txtdoenlist
        {
            width: 90px;
            height: 25px;
            line-height: 25px;
            vertical-align: middle;
            padding: 3px;
            border: 1px #bebee1 solid;
            color: #494b4c;
        }
        .txtdoenlista
        {
            width: 300px;
            height: 25px;
            line-height: 25px;
            vertical-align: middle;
            padding: 3px;
            border: 1px #bebee1 solid;
            color: #494b4c;
        }
        .buttoncss
        {
            width: 120px;
            height: 40px;
            background-color: #61a4da;
            color: #fff;
            font-family: 微软雅黑;
            font-size: 20px;
            border: 1px solid #3776a9;
            cursor: pointer;
        }
        .buttoncssa
        {
            width: 120px;
            height: 40px;
            background-color: #81c70e;
            color: #fff;
            font-family: 微软雅黑;
            font-size: 20px;
            border: 1px solid #679c11;
            cursor: pointer;
        }
        .buttoncssb
        {
            color: #4141a4;
            width: 50px;
            height: 25px;
            background: #fff;
            font-size: 14px;
            cursor: pointer;
            text-decoration: underline;
            float: left;
        }
        .buttonc
        {
            color: red;
            width: 50px;
            height: 25px;
            background: #fff;
            font-size: 14px;
            cursor: pointer;
            text-decoration: underline;
            float: left;
        }
        .grouptd
        {
            text-align: left;
            height: 25px;
            padding:0;
            margin:0;
        }
        #showSchoolDiv
        {
            width: 100%;
            height: 180px;
            border: 1px solid #ccc;
            color: #333333;
            line-height: 24px;
            text-align: left;
        }
        .hidden
        {
            display: none;
        }
        .buttoncssv
        {  
            width: 100px;
            height: 40px;
            background-color: #666;
            color: #fff;
            font-family: 微软雅黑;
            font-size: 20px;
            border: 1px solid #3776a9;
            cursor: pointer;
        }
    </style>
</asp:Content>

<asp:Content ID="Main" ContentPlaceHolderID="PlaceHolderMain" runat="server">
     <table cellpadding="3" cellspacing="0" border="0" style="font-size: 14px;color: #494b4c; width:670px ">
           <tr><td>课程期次：&nbsp;&nbsp;&nbsp;&nbsp;<asp:DropDownList ID="ddlQiCi" Width="200"  runat="server"  AutoPostBack="True"></asp:DropDownList></td><td>
</td></tr>
      <tr><td colspan="2" style="border: 1px solid #999; background-color: #ffffe1">
      <asp:GridView ID="gvWorks" runat="server" AutoGenerateColumns="False"  AllowPaging="True"
            BorderStyle="None"  CssClass="grouptd"  GridLines="None" 
               HeaderStyle-HorizontalAlign="Left">
            <PagerSettings Mode="NumericFirstLast" />
            <EmptyDataTemplate>
                当前没有数据
            </EmptyDataTemplate>
            <Columns>
                <asp:BoundField HeaderText=""  HeaderStyle-CssClass="hidden"  FooterStyle-CssClass="hidden" HeaderStyle-Width="0" DataField="WorksID">
                    <ItemStyle Width="0" CssClass="hidden"></ItemStyle>
                </asp:BoundField>
                <asp:BoundField HeaderText="作品名称" DataField="WorksName"  HeaderStyle-HorizontalAlign="Left">
                    <ItemStyle Width="250px"></ItemStyle>
                </asp:BoundField>
               
                 <asp:BoundField HeaderText="作品编号" DataField="WorksCode" HeaderStyle-HorizontalAlign="Left">

                    <ItemStyle Width="150px"></ItemStyle>
                </asp:BoundField>
                 <asp:BoundField HeaderText="作品组别" DataField="WorksTypeName" FooterStyle-HorizontalAlign="Left" HeaderStyle-HorizontalAlign="Left">
                    <ItemStyle Width="150px" ForeColor="#cc6f13"></ItemStyle>
                </asp:BoundField>
                 <asp:TemplateField HeaderText="评分"  ItemStyle-Width="120px" ItemStyle-HorizontalAlign="Center">
                   <ItemTemplate>
                     <asp:HyperLink  Target="_blank" runat="server"  ID ="lnkScore"  >评分</asp:HyperLink>
                   </ItemTemplate>
                </asp:TemplateField>
            </Columns>
            <PagerStyle HorizontalAlign="Center" />
            
        </asp:GridView>
      </td></tr>
     </table>
</asp:Content>

<asp:Content ID="PageTitle" ContentPlaceHolderID="PlaceHolderPageTitle" runat="server">
作品互评
</asp:Content>
<asp:Content ID="PageTitleInTitleArea" ContentPlaceHolderID="PlaceHolderPageTitleInTitleArea" runat="server" >
我要评的作品
</asp:Content>
