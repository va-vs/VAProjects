<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Import Namespace="Microsoft.SharePoint.ApplicationPages" %>
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="MyWorks.aspx.cs" Inherits="WorkEvaluate.Layouts.WorkEvaluate.MyWorks" DynamicMasterPageFile="~masterurl/default.master" %>

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
            width: 80px;
            text-align: right;
            padding-right: 10px;
            color: #4a4a4a;
        }
        .txtcss
        {
            width: 200px;
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
            width: 320px;
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
    </style>
    <link rel="stylesheet" type="text/css" href="css/Combox.css" />
</asp:Content>
<asp:Content ID="Main" ContentPlaceHolderID="PlaceHolderMain" runat="server">
   <%-- <span style="font-size: 16px;font-weight: bold">我发布的作品:</span>
    <br/>--%>
    <div>
        <asp:DropDownList ID="ddlCourses" runat="server" Width="200px" AutoPostBack="True" OnSelectedIndexChanged="ddlCourses_OnSelectedIndexChanged"></asp:DropDownList>
        <asp:DropDownList ID="ddlPeriods" runat="server" Width="200px" AutoPostBack="True" OnSelectedIndexChanged="ddlPeriods_OnSelectedIndexChanged"></asp:DropDownList>
        <asp:label runat="server" text="该课程没有期次" ForeColor="red" Visible="False" ID="showperiod"></asp:label>
        <asp:Button runat="server" Text="重选" ID="clearSets" OnClick="clearSets_OnClick" Visible="False"></asp:Button>
    </div>
    <br/>
    <div>

        <asp:GridView ID="gvMyWorks" runat="server" AutoGenerateColumns="False" AllowPaging="True" BorderStyle="None" CssClass="grouptd" GridLines="None" PageSize="5" AlternatingRowStyle-HorizontalAlign="Center" HorizontalAlign="Left">
            <AlternatingRowStyle BackColor="#D9D9D9" /> 
            <EmptyDataTemplate>
                 <table>
                    <tr>
                        <td style="font-weight:bold;font-size: 15px;color: red">友情提示:
                        </td>
                    </tr>
                    <tr>
                        <td colspan="5">所选课程或者期次下你还没有发布作品
                        </td>
                    </tr>
                </table>
            </EmptyDataTemplate>
            <Columns>
                <asp:BoundField HeaderText="作品ID" DataField="WorksID" FooterStyle-HorizontalAlign="Left">
                    <FooterStyle CssClass="hidden" />
                    <HeaderStyle CssClass="hidden" />
                    <ItemStyle Width="60px" CssClass="hidden"></ItemStyle>
                </asp:BoundField>
                <asp:BoundField HeaderText="作品名称" DataField="WorksName" ItemStyle-Width="180px" FooterStyle-HorizontalAlign="Left">
                    <FooterStyle HorizontalAlign="Left"></FooterStyle>
                    <ItemStyle Width="180px"></ItemStyle>
                </asp:BoundField>
                <asp:BoundField HeaderText="所属课程" DataField="CourseName" FooterStyle-HorizontalAlign="Left">
                     <HeaderStyle/>
                    <FooterStyle HorizontalAlign="Left"></FooterStyle>
                    <ItemStyle Width="100px"></ItemStyle>
                </asp:BoundField>
                <asp:BoundField HeaderText="作品期次" DataField="PeriodTitle" FooterStyle-HorizontalAlign="Left">
                    <HeaderStyle CssClass="hidden" />
                    <FooterStyle HorizontalAlign="Left" CssClass="hidden"></FooterStyle>
                    <ItemStyle Width="100px" CssClass="hidden"></ItemStyle>
                </asp:BoundField>
                <asp:BoundField HeaderText="作品类别" DataField="WorksTypeName" FooterStyle-HorizontalAlign="Center">
                    <HeaderStyle HorizontalAlign="Center"/>
                    <FooterStyle HorizontalAlign="Center"/>
                    <ItemStyle Width="100px" HorizontalAlign="Center"/>
                </asp:BoundField>
               <asp:BoundField HeaderText="作品得分" DataField="Score" FooterStyle-HorizontalAlign="Center">
                   <HeaderStyle HorizontalAlign="Center"/>
                    <FooterStyle HorizontalAlign="Center"></FooterStyle>
                    <ItemStyle Width="100px" HorizontalAlign="Center"></ItemStyle>
                </asp:BoundField>
                <asp:BoundField HeaderText="我的身份" DataField="Relationship" FooterStyle-HorizontalAlign="Center">
                    <HeaderStyle HorizontalAlign="Center"/>
                    <FooterStyle HorizontalAlign="Center"></FooterStyle>
                    <ItemStyle Width="100px" HorizontalAlign="Center"></ItemStyle>
                </asp:BoundField>
                <asp:BoundField HeaderText="作品状态" DataField="StateName" FooterStyle-HorizontalAlign="Center">
                    <HeaderStyle HorizontalAlign="Center"/>
                    <FooterStyle HorizontalAlign="Center"></FooterStyle>
                    <ItemStyle Width="100px" HorizontalAlign="Center"></ItemStyle>
                </asp:BoundField>
                <asp:TemplateField HeaderText="修改作品" ControlStyle-Width="100px" FooterStyle-HorizontalAlign="Center">
                    <ItemTemplate>
                        <asp:Button ID="btnEdit" runat="server" CommandName="EditWorks" Text="修改作品"
                            CssClass="buttoncssb" BorderStyle="None" />
                    </ItemTemplate>
                    <ControlStyle Width="100px"></ControlStyle>
                    <HeaderStyle HorizontalAlign="Center"/>
                    <FooterStyle HorizontalAlign="Center"></FooterStyle>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="查看作品" ControlStyle-Width="100px" FooterStyle-HorizontalAlign="Center">
                    <ItemTemplate>
                        <asp:Button ID="btnView" runat="server" CommandName="ViewWorks" Text="查看作品" CssClass="buttoncssb"
                            BorderStyle="None" />
                    </ItemTemplate>
                    <ControlStyle Width="100px"></ControlStyle>
                    <HeaderStyle HorizontalAlign="Center"/>
                    <FooterStyle HorizontalAlign="Center"></FooterStyle>
                </asp:TemplateField>

            </Columns>
            <HeaderStyle BackColor="#494B4C" ForeColor="#E6E6E6" HorizontalAlign="Left" />
            <AlternatingRowStyle HorizontalAlign="Left"></AlternatingRowStyle>
        </asp:GridView>
    </div>
</asp:Content>

<asp:Content ID="PageTitle" ContentPlaceHolderID="PlaceHolderPageTitle" runat="server">
个人作品列表
</asp:Content>

<asp:Content ID="PageTitleInTitleArea" ContentPlaceHolderID="PlaceHolderPageTitleInTitleArea" runat="server" >
我的作品
</asp:Content>
