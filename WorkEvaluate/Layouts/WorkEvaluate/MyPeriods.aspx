<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Import Namespace="Microsoft.SharePoint.ApplicationPages" %>
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="MyPeriods.aspx.cs" Inherits="WorkEvaluate.Layouts.WorkEvaluate.MyPeriods" DynamicMasterPageFile="~masterurl/default.master" %>

<asp:Content ID="PageHead" ContentPlaceHolderID="PlaceHolderAdditionalPageHead" runat="server">
    <link rel="stylesheet" type="text/css" href="css/base.css" />
    <link rel="stylesheet" type="text/css" href="css/page.css" />
    <link rel="stylesheet" type="text/css" href="css/Combox.css" />
    <style type="text/css">
        #showDiv{ 
             margin:0;
             text-align:left;
             background-color:#fff;
             padding:10px 20px 5px;
         }
        .f {
              color:#FFFFFF; 
              text-align:center;
         }
        .f ul{ margin:0; padding:0;text-align:left;margin-left:10px;margin-top:15px;}
        .f ul li{ padding:2px 0;}
        .f ul li a{ font-size:13px; font-family:'微软雅黑'; color:#b7b7b7; text-decoration:none;}
        .f ul li a:hover{ color:#ff6633}
        .grouptd
        {
            text-align: left;
            height: 25px;
            padding:0;
            margin:0;
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
        .liuyan{font-weight:bold; font-size:13px;}
        .fenshu{color:#f00}
        .neirong{ color:#666}
    </style>
    <script type="text/javascript" src="JS/jquery-1.9.1.min.js"></script>
 
</asp:Content>

<asp:Content ID="Main" ContentPlaceHolderID="PlaceHolderMain" runat="server">
    <div>
        <asp:Label ID="error" CssClass="redStar" runat="server" Text=""></asp:Label>
    </div>
    <div class="raised">
        <b class="b1"></b><b class="b2"></b><b class="b3"></b><b class="b4"></b>
        <div class="boxcontent">
            <asp:DropDownList ID="ddlCourses" runat="server" Width="200px" AutoPostBack="True" Height="22px" OnSelectedIndexChanged="ddlCourses_OnSelectedIndexChanged"></asp:DropDownList>        
            <asp:Button runat="server" Text="重选" ID="clearSets" OnClick="clearSets_OnClick" Visible="False"></asp:Button>
        </div>
        <b class="b4b"></b><b class="b3b"></b><b class="b2b"></b><b class="b1b"></b>
    </div>
    <br/>
    <div class="raised">
        <b class="b1"></b><b class="b2"></b><b class="b3"></b><b class="b4"></b>
    <div id="PeriodsList" runat="server" class="boxcontent">
        <asp:GridView ID="gvPeriod" runat="server" AllowPaging="False" AutoGenerateColumns="False"
            CellPadding="0" GridLines="None" Height="25px"
            Width="700px" CssClass="GridViewStyle" AlternatingRowStyle-CssClass="GridViewAlternatingRowStyle">
            <AlternatingRowStyle BackColor="#D9D9D9" /> 
            <EmptyDataTemplate>
                <table>
                    <tr>
                        <td style="font-weight:bold;font-size: 15px;">友情提示:
                        </td>
                    </tr>
                    <tr>
                        <td colspan="5" style="color: red">你当前还没有发布任何期次
                        </td>
                    </tr>
                </table>
            </EmptyDataTemplate>
            <%--<RowStyle BackColor="#EAEAEA" />--%>
             <Columns>
                <asp:BoundField DataField="PeriodID" HeaderText="期次ID">
                    <FooterStyle CssClass="hidden" />
                    <HeaderStyle CssClass="hidden" />
                    <ItemStyle Width="60px" CssClass="hidden"></ItemStyle>
                </asp:BoundField>
                <asp:BoundField DataField="CourseName" HeaderText="课程" >
                    <HeaderStyle HorizontalAlign="Center"/>
                    <FooterStyle HorizontalAlign="Center"/>
                    <ItemStyle  HorizontalAlign="Left"/>
                </asp:BoundField>
                <asp:BoundField DataField="PeriodTitle" HeaderText="期次" >
                    <HeaderStyle HorizontalAlign="Center"/>
                    <FooterStyle HorizontalAlign="Center"/>
                    <ItemStyle HorizontalAlign="Left"/>
                </asp:BoundField>
                <asp:BoundField DataField="WorksCount" HeaderText="作品数" >
                    <HeaderStyle HorizontalAlign="Center"/>
                    <FooterStyle HorizontalAlign="Center"/>
                    <ItemStyle HorizontalAlign="Center"/>
                </asp:BoundField>
                <asp:TemplateField HeaderText="当前状态" ItemStyle-Width="120px">
                    <ItemTemplate>
                        <div style="width: 100px;align-items: center">
                            <asp:Label ID="lbPeriodState" runat="server" Text=""></asp:Label>
                        </div>
                    </ItemTemplate>
                    <HeaderStyle HorizontalAlign="Center"/>
                    <FooterStyle HorizontalAlign="Center"></FooterStyle>
                    <ItemStyle HorizontalAlign="Center"/>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="样例作品" ItemStyle-Width="120px">
                    <ItemTemplate>
                        <asp:Button ID="btnUpload" CausesValidation="false" CommandName="Upload" Text="上传" runat="server"
                            BorderStyle="None"  BackColor="Transparent" CssClass="buttoncssb"/>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="评价指标" ItemStyle-Width="120px">
                    <ItemTemplate>
                        <asp:Button ID="btnStandard" CausesValidation="false" CommandName="Standard" Text="设置" runat="server"
                            BorderStyle="None"  BackColor="Transparent" CssClass="buttoncssb"/>
                    </ItemTemplate>
                    <HeaderStyle HorizontalAlign="Center"/>
                    <FooterStyle HorizontalAlign="Center"></FooterStyle>
                    <ItemStyle HorizontalAlign="Center"/>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="评分分组" ItemStyle-Width="120px">
                    <ItemTemplate>
                        <asp:Button ID="btnAllot" CausesValidation="false" CommandName="Alloting" Text="分组" runat="server"
                            BorderStyle="None" BackColor="Transparent" CssClass="buttoncssb"/>
                    </ItemTemplate>
                    <HeaderStyle HorizontalAlign="Center"/>
                    <FooterStyle HorizontalAlign="Center"></FooterStyle>
                    <ItemStyle HorizontalAlign="Center"/>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="公布成绩" ItemStyle-Width="120px">
                    <ItemTemplate>
                        <asp:Button ID="btnScore" CausesValidation="false" CommandName="ComputingScore" Text="公布" runat="server"
                            BorderStyle="None" BackColor="Transparent" CssClass="buttoncssb"/>
                    </ItemTemplate>
                    <HeaderStyle HorizontalAlign="Center"/>
                    <FooterStyle HorizontalAlign="Center"></FooterStyle>
                    <ItemStyle HorizontalAlign="Center"/>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="查看详情" ItemStyle-Width="120px">
                    <ItemTemplate>
                        <asp:Button ID="btnEdit" CausesValidation="false" CommandName="Down" Text="查看" runat="server"
                            BorderStyle="None"  BackColor="Transparent" CssClass="buttoncssb"/>
                    </ItemTemplate>
                    <HeaderStyle HorizontalAlign="Center"/>
                    <FooterStyle HorizontalAlign="Center"></FooterStyle>
                    <ItemStyle HorizontalAlign="Center"/>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="删除本期" ItemStyle-Width="120px">
                    <ItemTemplate>
                        <asp:Button ID="btnDel" CausesValidation="false" CommandName="Del" Text="删除" runat="server"
                            BorderStyle="None" BackColor="Transparent" CssClass="buttoncssb" ToolTip="请谨慎操作"/>
                    </ItemTemplate>
                    <HeaderStyle HorizontalAlign="Center"/>
                    <FooterStyle HorizontalAlign="Center"></FooterStyle>
                    <ItemStyle HorizontalAlign="Center"/>
                </asp:TemplateField>
            </Columns>
            <HeaderStyle BackColor="#494B4C" ForeColor="#E6E6E6" HorizontalAlign="Left" />
            <PagerStyle HorizontalAlign="Center" />
        </asp:GridView>
    </div>
        <b class="b4b"></b><b class="b3b"></b><b class="b2b"></b><b class="b1b"></b>
    </div>
</asp:Content>

<asp:Content ID="PageTitle" ContentPlaceHolderID="PlaceHolderPageTitle" runat="server">
作品需求期次管理
</asp:Content>

<asp:Content ID="PageTitleInTitleArea" ContentPlaceHolderID="PlaceHolderPageTitleInTitleArea" runat="server" >
我发布的作品需求
</asp:Content>
