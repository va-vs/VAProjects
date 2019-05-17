<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Register TagPrefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ActivityStasticsUserControl.ascx.cs" Inherits="ActivityStastics.ActivityStastics.ActivityStasticsUserControl" %>
<%@ Register Assembly="System.Web.DataVisualization, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" Namespace="System.Web.UI.DataVisualization.Charting" TagPrefix="asp" %>

<style type="text/css">
    .Appraise {
        box-shadow: 0 1px 3px #ccc;
        background-color: white;
        background-color: rgba(255, 255, 255, 0.6);
        padding: 10px 20px 10px 10px;
        margin-right: 10px;
        margin-top: 5px;
        width: 100%;
        line-height: 30px;
        font-size: 14px;
    }

        .Appraise .title {
            height: 50px;
            line-height: 50px;
            font-family: "微软雅黑", "PingFang SC", sans;
            font-weight: 600;
            color: #000;
            font-size: 20px;
            border-bottom: 1px solid #ccc;
        }

        .Appraise .content {
            text-align: left;
            font-size: 12px;
            padding: 5px;
        }

    .formul {
        list-style: none;
        margin-left: auto;
    }

        .formul li {
            line-height: 25px;
            padding-bottom: 10px;
        }

            .formul li button {
                font-size: 14px;
            }

    .att {
        background-color: #cfcfcf;
        color: red;
        font-size: 14px;
    }

    .button {
        width: 50px;
        text-align: center;
        font-weight: bold;
        color: #000;
        border-radius: 5px;
        margin: 0 5px 5px 0;
        overflow: hidden;
        cursor: pointer;
    }

    .mydtc {
        width: 100px;
        border: 1px solid #ffd800;
        background-color: #cfcfcf;
    }
</style>

<div class="Appraise">
    <div class="title">个人学习助手数据可视化</div>
    
    <div id="charts">
        <h3>一、各类活动时长随时间变化趋势</h3>
        <div class="content">
        活动类别：<asp:DropDownList ID="ddlTypes" runat="server" Height="25px" Width="195px" OnSelectedIndexChanged="ddlTypes_SelectedIndexChanged" AutoPostBack="true">
        </asp:DropDownList>
    </div>
    <div class="content" style="display: none;">
        日期范围：
    <asp:RadioButtonList ID="groupRBList" runat="server" RepeatDirection="Horizontal" AutoPostBack="True">
        <asp:ListItem Value="0" Selected="True">近一周</asp:ListItem>
        <asp:ListItem Value="1">近一月</asp:ListItem>
        <asp:ListItem Value="2">近半年</asp:ListItem>
        <asp:ListItem Value="3">近一年</asp:ListItem>
    </asp:RadioButtonList>
    </div>
        <asp:Chart ID="Chart1" runat="server" BackColor="211, 223, 240" ImageLocation="~/TempImages/ChartPic_#SEQ(300,3)" Width="480px" BorderlineDashStyle="Solid" BackGradientStyle="TopBottom" BorderWidth="2px" BorderColor="#B54001">
            <Titles>
                <asp:Title ShadowColor="32, 0, 0, 0" Font="Trebuchet MS, 14.25pt, style=Bold" ShadowOffset="3"
                    Text="近一周每日“项目”活动时长变化趋势" Name="Title1" ForeColor="White">
                </asp:Title>
            </Titles>
            <Legends>
                <asp:Legend IsTextAutoFit="False" Name="时长对比（单位：分钟）" BackColor="Transparent"
                    Font="Trebuchet MS, 8.25pt, style=Bold" Alignment="Center" Docking="Top" Title="时长对比（单位：分钟）" TitleForeColor="Blue">
                </asp:Legend>
            </Legends>
            <BorderSkin SkinStyle="FrameTitle8"></BorderSkin>
            <Series>
                <asp:Series MarkerSize="8" BorderWidth="2" XValueType="Double" Name="计划时长" ChartType="Line"
                    MarkerStyle="Circle" ShadowColor="Black" BorderColor="180, 26, 59, 105" Color="220, 65, 140, 240"
                    ShadowOffset="2" YValueType="Double" IsValueShownAsLabel="true" >
                </asp:Series>
                <asp:Series MarkerSize="8" BorderWidth="2" XValueType="Double" Name="实际时长" ChartType="Line"
                    MarkerStyle="Circle" ShadowColor="#003300" BorderColor="Red" Color="#FFCC66"
                    ShadowOffset="2" YValueType="Double" IsValueShownAsLabel="true" >
                </asp:Series>
            </Series>
            <ChartAreas>
                <asp:ChartArea Name="ca1" ShadowColor="Transparent" BackColor="209, 237, 254" BackSecondaryColor="White">
                    <AxisY>
                        <MajorGrid LineColor="64, 64, 64, 64" />
                    </AxisY>
                    <AxisX>
                        <MajorGrid LineColor="64, 64, 64, 64" />
                    </AxisX>
                </asp:ChartArea>
            </ChartAreas>
        </asp:Chart>
        <hr/>

        <h3>二、各类活动时长指定周期内的雷达分布图</h3>
        <asp:RadioButtonList ID="rbPeriodList" runat="server" AutoPostBack="True" RepeatDirection="Horizontal" OnSelectedIndexChanged="rbPeriodList_SelectedIndexChanged">
            <asp:ListItem Value="1">今日</asp:ListItem>
            <asp:ListItem Value="2">本周</asp:ListItem>
            <asp:ListItem Value="3">本月</asp:ListItem>
            <asp:ListItem Value="0">自定义日期范围</asp:ListItem>            
        </asp:RadioButtonList>
        <div id="divTimeSpan" runat="server" visible="false" class="content">
            开始：<SharePoint:DateTimeControl ID="dtStart" runat="server" DateOnly="True" />
            截至：<SharePoint:DateTimeControl ID="dtEnd" runat="server" DateOnly="True" />
            <asp:Button ID="btnDrawRadar" runat="server" Text="查看雷达图" OnClick="btnDrawRadar_Click" />
         </div>
        
        <br />
        <asp:Chart ID="chartRadar" runat="server" BackColor="211, 223, 240" ImageLocation="~/TempImages/ChartPic_#SEQ(300,3)" Width="480px" BorderlineDashStyle="Solid" BackGradientStyle="TopBottom" BorderWidth="2px" BorderColor="#B54001">
            <Series>
               <asp:Series Name="计划时长" ChartType="Radar" Color="60,255, 0, 0" BorderColor="Red" IsValueShownAsLabel="True" LabelForeColor="Red" BorderWidth="2" LabelBorderWidth="2"></asp:Series>
                <asp:Series Name="实际时长" ChartType="Radar" Color="60,0, 0,255" BorderColor="Blue" IsValueShownAsLabel="True" LabelForeColor="Blue" BorderWidth="2" LabelBorderWidth="2"></asp:Series>
                <%--<asp:Series Name="计划时长" ChartType="Radar"></asp:Series>
                <asp:Series Name="实际时长" ChartType="Radar"></asp:Series>--%>
            </Series>
             <Legends>
                <asp:Legend IsTextAutoFit="False" Name="活动时长雷达图（单位：分钟）" BackColor="Transparent"
                    Font="Trebuchet MS, 8.25pt, style=Bold" Alignment="Center" Docking="Top" Title="活动时长雷达图（单位：分钟）" TitleForeColor="Blue">
                </asp:Legend>
            </Legends>
            <ChartAreas>
                <asp:ChartArea Name="计划时长与实际时长对比" ShadowColor="Transparent" BackColor="209, 237, 254" BackGradientStyle="TopBottom" BackSecondaryColor="White" Area3DStyle-Enable3D="True">
                    <AxisY>
                        <MajorGrid LineColor="64, 64, 64, 64" />
                    </AxisY>
                    <AxisX>
                        <MajorGrid LineColor="64, 64, 64, 64" />
                    </AxisX>
                </asp:ChartArea>
            </ChartAreas>
        </asp:Chart>
    </div>
    <asp:Label ID="lbErr" runat="server" ForeColor="red" Text=""></asp:Label>

    <br />

</div>
