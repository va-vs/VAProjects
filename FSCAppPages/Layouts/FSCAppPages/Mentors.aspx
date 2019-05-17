<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Import Namespace="Microsoft.SharePoint.ApplicationPages" %>
<%@ Register TagPrefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>

<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Mentors.aspx.cs" Inherits="FSCAppPages.Layouts.FSCAppPages.Mentors" DynamicMasterPageFile="~masterurl/default.master" %>

<asp:Content ID="PageHead" ContentPlaceHolderID="PlaceHolderAdditionalPageHead" runat="server">
    <link href="css/style.css" type="text/css" rel="stylesheet" />
</asp:Content>

<asp:Content ID="Main" ContentPlaceHolderID="PlaceHolderMain" runat="server">
    <div id="editlink" runat="server" style="padding: 10px;"></div>

    <table class="ms-blog-MainArea" width="100%" cellpadding="0" cellspacing="0" border="0">
        <tbody>
            <tr>
                <td valign="top" style="width: 70%">
                     <div class="areatitle"> 专业方向: <span id="divMajorArea" runat="server"></span></div>
                    <div class="mylist">
                        <div class="listtitle">
                            ◇ 基本信息
                        </div>
                        <div class="pp" style="text-align: center;">
                            <table class="mytable">
                                <tr>
                                    <th>姓名
                                    </th>
                                    <td id="lblName" runat="server">
                                        <div id="divName" runat="server">李洋</div>
                                    </td>
                                    <th>性别
                                    </th>
                                    <td id="lblSex" runat="server">男
                                    </td>
                                    <td rowspan="3" id="divPhoto" runat="server">
                                        <img src="images/logo.jpg" />
                                    </td>
                                </tr>
                                <tr>
                                    <th>出生年月
                                    </th>
                                    <td id="lblBirthDate" runat="server">0000-00-00
                                    </td>
                                    <th>学历
                                    </th>
                                    <td id="lblEducation" runat="server">博士研究生
                                    </td>
                                </tr>
                                <tr>
                                    <th>职称
                                    </th>
                                    <td id="lblProfessional" runat="server">教授
                                    </td>
                                    <th>职务
                                    </th>
                                    <td id="lblDuties" runat="server">无
                                    </td>
                                </tr>
                            </table>
                        </div>
                        <div style="text-align: left;padding-left:10px;font-style:italic" runat="server" id="msgdiv" visible="false">注：✲ 表示为秦皇岛分校导师</div>

                        <div class="listtitle">◇ 研究方向</div>
                        <div class="pp" id="divResearch" runat="server">
                        </div>
                        <div class="listtitle">◇ 开设主要课程</div>
                        <div class="pp" id="divCourse" runat="server">
                        </div>
                        <div class="listtitle">◇ 代表性研究成果</div>
                        <div class="pp" id="divResults" runat="server">
                            <span>论文（CSSCI期刊及扩展来源期刊）</span>
                            <ul>
                                <li>李洋. 服务国际合作，充分培养翻译人才[N]. 光明日报, 2017-12-14(14). </li>
                                <li>李洋. 语块使用与学生口译水平的语料库研究——基于汉英交替传译PACCEL语料库的考察[J]. 外语与外语教学, 2017, 296(5): 88-96. </li>
                                <li>李洋. 论中国译者主体性的现代嬗变——以"五四"翻译潮为例[J]. 东北大学学报（社会科学版）, 2017, 19(4): 429-434. </li>
                                <li>李洋. 预制语块语用功能的语料库口译研究[J]. 现代外语, 2016, (2): 246-256. </li>
                                <li>李洋. 基于语料库的口译研究在中国之嬗变与发展：2007-2014[J]. 解放军外国语学院学报, 2016, (3): 109-116. </li>
                                <li>李洋, 刘卓. 基于Prezi云存储构建互动式多媒体课件的探索——以《基础口译》课件设计和制作为例[J]. 现代教育技术, 2014, (5): 52-57. </li>
                                <li>李洋, 王楠. 预制语块对同声传译的缓解效应研究[J]. 外语界, 2012, (1): 61-67.</li>
                            </ul>
                        </div>
                    </div>
                </td>
                <td class="ms-blog-LeftColumn" valign="top" style="padding-left: 20px;">
                    <div id="samelist" runat="server">
                        <div class="rightlist">相同专业方向的导师</div>
                        <div class="mylist" id="rsdirect" runat="server">
                        </div>
                    </div>
                </td>
            </tr>
        </tbody>
    </table>
</asp:Content>

<asp:Content ID="PageTitle" ContentPlaceHolderID="PlaceHolderPageTitle" runat="server">
    导师名录
</asp:Content>

<asp:Content ID="PageTitleInTitleArea" ContentPlaceHolderID="PlaceHolderPageTitleInTitleArea" runat="server">
    <a href="allMentors.aspx" title="查看所有导师">导师名录</a> > <span id="mtName" runat="server" class="mtname">当前导师</span>
</asp:Content>
