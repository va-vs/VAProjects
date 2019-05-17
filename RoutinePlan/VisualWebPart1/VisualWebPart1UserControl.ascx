<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Register Tagprefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="VisualWebPart1UserControl.ascx.cs" Inherits="RoutinePlan.VisualWebPart1.VisualWebPart1UserControl" %>
<style type="text/css">
    .Appraise {
    box-shadow: 0 1px 3px #ccc;
    background-color: white;
    background-color: rgba(255, 255, 255, 0.6);
    padding:10px 20px 10px 10px;
    margin-right: 10px;
    margin-top: 5px;
    width:100%;
    max-width:420px;
    line-height:30px;
    font-size:14px;

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

        .formul{
        list-style:none;
        margin-left:auto;
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
.button{
        width: 50px;
        text-align: center;
        font-weight: bold;
        color: #000;
        border-radius: 5px;
        margin:0 5px 5px 0;
        overflow: hidden;
        cursor:pointer;
    }
     </style>
<script type="text/javascript">
        function checkAll() {
            for (var i = 0; i < document.getElementById("cblPlans").getElementsByTagName("input").length; i++) {
                document.getElementById("cblPlans_" + i).checked = true;
            }
        }

        function deleteAll() {
            for (var i = 0; i < document.getElementById("cblPlans").getElementsByTagName("input").length; i++) {
                document.getElementById("cblPlans_" + i).checked = false;
            }
        }

        function ReverseAll() {

            for (var i = 0; i < document.getElementById("cblPlans").getElementsByTagName("input").length; i++) {

                var objCheck = document.getElementById("cblPlans_" + i);

                if (objCheck.checked)

                    objCheck.checked = false;
                else
                    objCheck.checked = true;
            }
        }
    </script>
<div id="AppraiseDiv" class="Appraise" runat="server">
    <div class="title">生成例行计划</div>

    第1步：选择例行计划项目：
    <div id="rplansDiv" style="padding-left: 20px;">
        <asp:CheckBoxList ID="cblPlans" runat="server" RepeatColumns="4" RepeatDirection="Horizontal" ClientIDMode="Static" CellPadding="1" CellSpacing="1"></asp:CheckBoxList>
    </div>
    <div style="text-align: center;">
        <input type="button" onclick="checkAll()" value="全选" class="button" />&nbsp;
        <input type="button" onclick="ReverseAll()" value="反选" class="button" />&nbsp;
        <input type="button" onclick="deleteAll()" value="取消" class="button" />
    </div>
    <hr />
    <div>
        第2步：输入生成天数<asp:Label ID="lbDaysSpan" runat="server" Text="（最少1天，最多7天）"></asp:Label>：<br />
        <div style="padding-left: 20px;">
            从今天起，<asp:TextBox ID="tbDays" runat="server" TextMode="Number" ToolTip="请输入1-7之间的正整数" Width="41px" ClientIDMode="Static">3</asp:TextBox>
            &nbsp;天，&nbsp;&nbsp;<asp:CheckBox ID="cbWeekEnd" runat="server" Text="跳过周末" AutoPostBack="True" />
        </div>
    </div>
    <hr />
    <div style="text-align: center">
        <asp:Button ID="btnMakePlans" runat="server" Text="点击生成" OnClick="btnMakePlans_Click" Font-Bold="True" Width="100px" Font-Size="15px" CssClass="button" Height="35px" /><br />
    </div>
    <asp:Label ID="lbErr" runat="server" Text="" ForeColor="red"></asp:Label>
</div>


<div runat="server" visible="false">
<asp:DropDownList ID="dllRoutinePlan" runat="server"></asp:DropDownList>

<table >
    <tr>
        <th>
            周期
        </th>
        <th>
            模式
        </th>
        <th>
            次数
        </th>

    </tr>
    <tr>
        <td style="vertical-align:top;">
            <asp:RadioButtonList ID="rblistCycle" runat="server" OnSelectedIndexChanged="rblistCycle_SelectedIndexChanged" AutoPostBack="true">
                <asp:ListItem Value="rbDay">按天</asp:ListItem>
                <asp:ListItem Value="rbWeek">按周</asp:ListItem>
                <asp:ListItem Value="rbMonth">按月</asp:ListItem>
                <asp:ListItem Value="rbYear">按年</asp:ListItem>
            </asp:RadioButtonList>
        </td>
        <td style="vertical-align:top;">
            <div id="DayModel" runat="server" visible="true">
                <asp:RadioButton ID="rbDayPerNDay" runat="server" OnCheckedChanged="rbDayPerNDay_CheckedChanged" AutoPostBack="true"/>每<asp:TextBox ID="tbPerDays" runat="server" Width="20px"></asp:TextBox>天<br/>
                <asp:RadioButton ID="rbDayPerWorkDay" runat="server" OnCheckedChanged="rbDayPerWorkDay_CheckedChanged" AutoPostBack="true"/>每个工作日
            </div>

            <div id="WeekModel" runat="server" visible="false">
                每<asp:TextBox ID="tbweeksCount" runat="server" Width="20px"></asp:TextBox>周重复，重复时间：
                <asp:CheckBoxList ID="cblWeek1" runat="server" RepeatColumns="3" RepeatDirection="Horizontal">
                    <asp:ListItem Value="weekMon">周一</asp:ListItem>
                    <asp:ListItem Value="weekTues">周二</asp:ListItem>
                    <asp:ListItem Value="weekWed">周三</asp:ListItem>
                    <asp:ListItem Value="weekThur">周四</asp:ListItem>
                    <asp:ListItem Value="weekFri">周五</asp:ListItem>
                    <asp:ListItem Value="weekSat">周六</asp:ListItem>
                    <asp:ListItem Value="weekSun">周日</asp:ListItem>
                </asp:CheckBoxList>
            </div>
            <div id="MonthModel" runat="server" visible="false">
                <asp:RadioButton ID="rbMonth1" runat="server" OnCheckedChanged="rbMonth1_CheckedChanged" AutoPostBack="True" />每<asp:TextBox ID="tbMonthCount1" runat="server" Width="20px"></asp:TextBox>个月的第<asp:TextBox ID="tbMonthDayCount1" runat="server" Width="20px"></asp:TextBox>日<br/>
                <asp:RadioButton ID="rbMonth2" runat="server" OnCheckedChanged="rbMonth2_CheckedChanged" AutoPostBack="True" />每<asp:TextBox ID="tbMonthCount2" runat="server" Width="20px"></asp:TextBox>个月的
                <asp:DropDownList ID="ddlWeekCount" runat="server">
                    <asp:ListItem Value="1">第一个</asp:ListItem>
                    <asp:ListItem Value="2">第二个</asp:ListItem>
                    <asp:ListItem Value="3">第三个</asp:ListItem>
                    <asp:ListItem Value="4">第四个</asp:ListItem>
                    <asp:ListItem Value="0">最后一个</asp:ListItem>
                </asp:DropDownList>
                <asp:DropDownList ID="ddlWeekDay" runat="server">
                    <asp:ListItem Value="weekMon">周末</asp:ListItem>
                    <asp:ListItem Value="weekMon">工作日</asp:ListItem>
                    <asp:ListItem Value="weekMon">周一</asp:ListItem>
                    <asp:ListItem Value="weekTues">周二</asp:ListItem>
                    <asp:ListItem Value="weekWed">周三</asp:ListItem>
                    <asp:ListItem Value="weekThur">周四</asp:ListItem>
                    <asp:ListItem Value="weekFri">周五</asp:ListItem>
                    <asp:ListItem Value="weekSat">周六</asp:ListItem>
                    <asp:ListItem Value="weekSun">周日</asp:ListItem>
                </asp:DropDownList>
            </div>
            <div id="YearModel" runat="server" visible="false">
                <asp:RadioButton ID="rbYear1" runat="server" OnCheckedChanged="rbYear1_CheckedChanged" AutoPostBack="True" />
                每年
                <asp:DropDownList ID="ddlMonth1" runat="server">
                    <asp:ListItem Value="1">1</asp:ListItem>
                    <asp:ListItem Value="2">2</asp:ListItem>
                    <asp:ListItem Value="3">3</asp:ListItem>
                    <asp:ListItem Value="4">4</asp:ListItem>
                    <asp:ListItem Value="5">5</asp:ListItem>
                    <asp:ListItem Value="6">6</asp:ListItem>
                    <asp:ListItem Value="7">7</asp:ListItem>
                    <asp:ListItem Value="8">8</asp:ListItem>
                    <asp:ListItem Value="9">9</asp:ListItem>
                    <asp:ListItem Value="10">10</asp:ListItem>
                    <asp:ListItem Value="11">11</asp:ListItem>
                    <asp:ListItem Value="12">12</asp:ListItem>
                  </asp:DropDownList>
                月
                <asp:DropDownList ID="ddlDay1" runat="server">
                    <asp:ListItem Value="1">1</asp:ListItem>
                    <asp:ListItem Value="2">2</asp:ListItem>
                    <asp:ListItem Value="3">3</asp:ListItem>
                    <asp:ListItem Value="4">4</asp:ListItem>
                    <asp:ListItem Value="5">5</asp:ListItem>
                    <asp:ListItem Value="6">6</asp:ListItem>
                    <asp:ListItem Value="7">7</asp:ListItem>
                    <asp:ListItem Value="8">8</asp:ListItem>
                    <asp:ListItem Value="9">9</asp:ListItem>
                    <asp:ListItem Value="10">10</asp:ListItem>
                   <asp:ListItem Value="11">11</asp:ListItem>
                    <asp:ListItem Value="12">12</asp:ListItem>
                    <asp:ListItem Value="13">13</asp:ListItem>
                    <asp:ListItem Value="14">14</asp:ListItem>
                    <asp:ListItem Value="15">15</asp:ListItem>
                    <asp:ListItem Value="16">16</asp:ListItem>
                    <asp:ListItem Value="17">17</asp:ListItem>
                    <asp:ListItem Value="18">18</asp:ListItem>
                    <asp:ListItem Value="19">19</asp:ListItem>
                    <asp:ListItem Value="20">20</asp:ListItem>
                   <asp:ListItem Value="21">21</asp:ListItem>
                     <asp:ListItem Value="22">22</asp:ListItem>
                    <asp:ListItem Value="23">23</asp:ListItem>
                    <asp:ListItem Value="24">24</asp:ListItem>
                    <asp:ListItem Value="25">25</asp:ListItem>
                    <asp:ListItem Value="26">26</asp:ListItem>
                    <asp:ListItem Value="27">27</asp:ListItem>
                    <asp:ListItem Value="28">28</asp:ListItem>
                    <asp:ListItem Value="29">29</asp:ListItem>
                    <asp:ListItem Value="30">30</asp:ListItem>
                    <asp:ListItem Value="31">31</asp:ListItem>
                  </asp:DropDownList>
                日<br/>
                <asp:RadioButton ID="rbYear2" runat="server" OnCheckedChanged="rbYear2_CheckedChanged" AutoPostBack="True" />
                <asp:DropDownList ID="ddlMonth2" runat="server">
                    <asp:ListItem Value="1">1</asp:ListItem>
                    <asp:ListItem Value="2">2</asp:ListItem>
                    <asp:ListItem Value="3">3</asp:ListItem>
                    <asp:ListItem Value="4">4</asp:ListItem>
                    <asp:ListItem Value="5">5</asp:ListItem>
                    <asp:ListItem Value="6">6</asp:ListItem>
                    <asp:ListItem Value="7">7</asp:ListItem>
                    <asp:ListItem Value="8">8</asp:ListItem>
                    <asp:ListItem Value="9">9</asp:ListItem>
                    <asp:ListItem Value="10">10</asp:ListItem>
                    <asp:ListItem Value="11">11</asp:ListItem>
                    <asp:ListItem Value="12">12</asp:ListItem>
                  </asp:DropDownList>
                月的
                <asp:DropDownList ID="ddlWeekCount2" runat="server">
                    <asp:ListItem Value="1">第一个</asp:ListItem>
                    <asp:ListItem Value="2">第二个</asp:ListItem>
                    <asp:ListItem Value="3">第三个</asp:ListItem>
                    <asp:ListItem Value="4">第四个</asp:ListItem>
                    <asp:ListItem Value="0">最后一个</asp:ListItem>
                </asp:DropDownList>
                <asp:DropDownList ID="ddlWeekDay2" runat="server">
                    <asp:ListItem Value="weekMon">周末</asp:ListItem>
                    <asp:ListItem Value="weekMon">工作日</asp:ListItem>
                    <asp:ListItem Value="weekMon">周一</asp:ListItem>
                    <asp:ListItem Value="weekTues">周二</asp:ListItem>
                    <asp:ListItem Value="weekWed">周三</asp:ListItem>
                    <asp:ListItem Value="weekThur">周四</asp:ListItem>
                    <asp:ListItem Value="weekFri">周五</asp:ListItem>
                    <asp:ListItem Value="weekSat">周六</asp:ListItem>
                    <asp:ListItem Value="weekSun">周日</asp:ListItem>
                </asp:DropDownList>
            </div>
        </td>
        <td style="vertical-align:top;">

            开始日期：<SharePoint:DateTimeControl ID="dtStart" runat="server" DateOnly="True" />

            <asp:RadioButton ID="rbEndDate" runat="server" OnCheckedChanged="rbEndDate_CheckedChanged" AutoPostBack="true" />截止日期：<SharePoint:DateTimeControl ID="dtEnd" runat="server" DateOnly="True" />
            <br />
            <asp:RadioButton ID="rbEndTimes" runat="server" OnCheckedChanged="rbEndTimes_CheckedChanged" AutoPostBack="true" />重复<asp:TextBox ID="tbCount" runat="server" Width="20px"></asp:TextBox>次后结束
            <br/>
            <asp:RadioButton ID="rbNoEndDate" runat="server" OnCheckedChanged="rbNoEndDate_CheckedChanged" AutoPostBack="true" />无截止日期

        </td>
    </tr>
</table>

<asp:RadioButtonList ID="rbTest" runat="server">
     <asp:ListItem Value="1">
         <select id="Select1" >
             &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; <option value="1">1</option>
             &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; <option value="1">1</option>
             &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; &nbsp;&nbsp;&nbsp;&nbsp; </select></asp:ListItem>
    <asp:ListItem Value="2">第二个</asp:ListItem>
</asp:RadioButtonList>
</div>