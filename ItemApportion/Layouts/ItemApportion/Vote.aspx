<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Import Namespace="Microsoft.SharePoint.ApplicationPages" %>
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Vote.aspx.cs" Inherits="ItemApportion.Layouts.ItemApportion.Vote" %>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta name="viewport" content="width=device-width"/>
    <title>创意投票</title>

</head>
<body>
    <form id="form1" runat="server"><asp:Label ID="lbIP" runat="server" Text="0.0.0.0"></asp:Label>
    <div style="width:98%;padding:5px;overflow:auto;">
        <table style="width: 100%;" >
            <tr>
                <td style="text-align:center">
                    <h2>智慧校园大数据管理服务平台</h2>
                </td>
            </tr>
            <tr>
                <td style="text-align:center">
                    作者：谢新强&nbsp;&nbsp;&nbsp;&nbsp;发布时间：2017-10-06 9:50
                </td>
            </tr>
            <tr>
                <td style="text-align:center">
                    <hr/>
                </td>
            </tr>
            <tr>
                <td>
                    <div><ul><li><strong>需求描述：</strong></li></ul><p>目前，随着高校信息化建设的提速,智慧校园已逐渐成为高校信息化建设的热点， 随着RFID、二维码、视频监控、普适计算等智慧校园信息技术的应用，高校信息管理系统、一卡通系统、校园BBS 论坛、贴吧、网站点击流量、通信设备、监控系统等产生的数据量呈几何级数快速增长、数据结构也日趋复杂。基于大数据分析技术对校园大数据进行有效分析和处理，提炼出有价值的数据信息供学校管理部门参考决策，不仅能够助推高校教育教学管理变革创新，提升学校教学、人才培养质量、使得智慧理念在高校管理中真正得以实现，而且能够推动我校在大数据应用核心技术研发和应用方面的创新能力、提升我校综合影响力。</p><ul><li><strong>解决方案</strong>：</li></ul><p>针对上述需求，拟基于本研究组在大数据分析相关技术研究成果，针对校园大数据（包括各类传感器、网络设备、管理系统、日志等）进行收集、汇聚、检索、分析挖掘、可视化展现等全过程管理，构建东北大学"智慧校园大数据服务平台"，拟研究和开发的主要内容包括如下5个方面：<br>1） 学生综合分析<br>通过大数据分析对学生进行预测，包括：挂科预测、失联预测、亲密度预测、贫困生预测等。<br>2） 行为统计分析<br>通过对各类终端数据进行统计分析，可以得出：学生兴趣、学生行为、学生行为与学习成绩关联分析、学生习惯等信息等。<br>3） 综合舆情分析<br>挖掘各种教育数据之间的关系，并对其进行统计分析，可以有效的分析出学生的关系网、社交网等。<br>4） 教育画像<br>通过算法分析，建立教育信息模型、知识图谱等，比如：学生画像、教师画像等。通过画像可以快速进行标准化分析，包括：交际能力，学习能力、教师水平等，通过知识图谱支持快速检索和定位存在问题和风险的学生、教师等。<br>5） 综合校情分析<br>通过大数据汇总分析，得出各类教育指标、能源消耗等业务分析报表，并支持数据报表的实时更新等。</p><ul><li><strong>核心价值</strong>：</li></ul><p>智慧校园大数据分析是未来高校大数据环境下信息化建设的必然趋势，目前国内清华大学、南京大学、同济大学等高校都在计划或正在实施智慧校园大数据分析服务平台建设。本方案的提出旨在提供一个既有创意、适用、又可真正落地的智慧校园大数据服务平台，通过此平台的建设，不仅能够满足东北大学智慧校园建设的需求、降低学校信息化建设成本，而且有助于建立完整的校园大数据服务结构，面向不同用户提供数据服务，打破校园各系统间的"信息孤岛"，充分发挥历史数据资产的作用，为学校提供趋势分析和决策支持，辅助领导决策，使学校在激烈的竞争中，处于不败之地。</p><p>更详细方案描述请下载附件PPT：<a href="/SmartNEU/Documents/附件：智慧校园大数据服务平台-创意方案-谢新强.pptx"><img width="16" height="16" class="ms-asset-icon ms-rtePosition-4" alt="" src="/_layouts/15/images/icpptx.png">附件：智慧校园大数据服务平台-创意方案-谢新强.pptx</a></p></div>
                </td>
            </tr>
            <tr>
                <td style="text-align:center">
                    <asp:ImageButton ID="IBVote" runat="server" ImageUrl="imgVote.png" Width="48" Height="48" AlternateText="投票" />
                    
                </td>
            </tr>
        </table>

        <div>
            <a href="cs.aspx">返回所有</a>
        </div>
    </div>
    </form>
</body>
</html>
