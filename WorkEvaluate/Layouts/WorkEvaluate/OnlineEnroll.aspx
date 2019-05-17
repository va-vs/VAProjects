<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Import Namespace="Microsoft.SharePoint.ApplicationPages" %>
<%@ Register TagPrefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="SharePointPublish" Namespace="Microsoft.SharePoint.Publishing.WebControls" Assembly="Microsoft.SharePoint.Publishing, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Import Namespace="Microsoft.SharePoint.Publishing" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>

<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="OnlineEnroll.aspx.cs" Inherits="WorkEvaluate.Layouts.WorkEvaluate.OnlineEnroll" DynamicMasterPageFile="~masterurl/default.master" %>

<asp:Content ID="PageHead" ContentPlaceHolderID="PlaceHolderAdditionalPageHead" runat="server">
    <script type="text/javascript" src="JS/jquery-1.9.1.min.js"></script>
    <script type="text/javascript">
        function ValidateTitle()
        {
            var txt = document.getElementById('<%=txtWorksName.ClientID%>');
            if (txt.value.length == 0) {
                document.getElementById('errWorksName').value = '作品名称不能为空';
                txt.select();
                return false;
            }
            else
                document.getElementById('errWorksName').value = '';
            return true
        }
        function onfts() {
            var spansCard2 = document.getElementById("spts").className = "warna"
        }
        function onbts() {
            var spansCard2 = document.getElementById("spts").className = "warn"
        }
        function onftsCard2() {
            var spansCard2 = document.getElementById("sCard2").className = "warna";
        }
        function onbtsCard2() {
            var spansCard2 = document.getElementById("sCard2").className = "warn";
        }
        function onftsCard3() {
            var spansCard3 = document.getElementById("sCard3").className = "warna";
        }
        function onbtsCard3() {
            var spansCard3 = document.getElementById("sCard3").className = "warn";
        }

        $(document).ready(function () {
            setTimeout("count_down()", 1000);//设置每一秒调用一次倒计时函数

            //根据天，时，分，秒的ID找到相对应的元素
        });

        //定义倒计时函数

        function count_down() {
            var time_now = new Date();  // 获取当前时间
            time_now = time_now.getTime();
            var time_endStr = document.getElementById("<%=hfEndTime.ClientID %>").value;  // 设定活动结束结束时间
            var time_end = Date.parse(time_endStr);


            var time_day = document.getElementById("times_day");
            var time_hour = document.getElementById("times_hour");
            var time_minute = document.getElementById("times_minute");
            var time_second = document.getElementById("second");
            var time_distance = time_end - time_now;  // 时间差：活动结束时间减去当前时间   
            var int_day, int_hour, int_minute, int_second;
            if (time_distance >= 0) {
                // 相减的差数换算成天数
                int_day = Math.floor(time_distance / 86400000)
                time_distance -= int_day * 86400000;
                // 相减的差数换算成小时
                int_hour = Math.floor(time_distance / 3600000)
                time_distance -= int_hour * 3600000;
                // 相减的差数换算成分钟   
                int_minute = Math.floor(time_distance / 60000)
                time_distance -= int_minute * 60000;
                // 相减的差数换算成秒数  
                int_second = Math.floor(time_distance / 1000)
                // 判断小时小于10时，前面加0进行占位
                if (int_hour < 10)
                    int_hour = "0" + int_hour;
                // 判断分钟小于10时，前面加0进行占位
                if (int_minute < 10)
                    int_minute = "0" + int_minute;
                // 判断秒数小于10时，前面加0进行占位 
                if (int_second < 10)
                    int_second = "0" + int_second;
                // 显示倒计时效果   
                time_day.innerHTML = int_day;
                time_hour.innerHTML = int_hour;
                time_minute.innerHTML = int_minute;
                time_second.innerHTML = int_second;
                setTimeout("count_down()", 1000);
            } else {
                time_day.innerHTML = 0;
                time_hour.innerHTML = 0;
                time_minute.innerHTML = 0;
                time_second.innerHTML = 0;
            }
        }


    </script>
    <script src="JS/jquery-1.9.1.min.js" type="text/javascript"></script>
    <script src="JS/FaceJScript.js" type="text/javascript"></script>
    <link rel="stylesheet" type="text/css" href="css/base.css" />
    <link rel="stylesheet" type="text/css" href="css/page.css" />
</asp:Content>

<asp:Content ID="Main" ContentPlaceHolderID="PlaceHolderMain" runat="server">
    <div>
        <asp:Label ID="error" CssClass="redStar" runat="server"></asp:Label>
        <!--顶部步骤指示 -->
        <!-- 课程期次 -->
        <div class="onlineEnroll-model">
            <div class="wt">
                <span class="redStar">*</span><label for="txtWorksName">课程期次：</label>
            </div>
            <asp:DropDownList ID="ddlPeriod" runat="server" CssClass="txtdoenlista" Height="25px" AutoPostBack="True"></asp:DropDownList>
            <asp:RequiredFieldValidator ID="RequiredFieldValidator4" runat="server" ErrorMessage="必填" CssClass="redStar" ControlToValidate="ddlPeriod"></asp:RequiredFieldValidator>
            <input type="button" id="btnMHidden" class="onlineEnroll-model-txtButton" />
            <%--  <asp:Button ID="btnIsSample" runat="server" CausesValidation="false" class="onlineEnroll-model-txtButton" Text="样例测评" />--%>
            <asp:HyperLink ID="HyperLink1" Target="_blank" runat="server">样例测评</asp:HyperLink>
        </div>
        <!-- 基准分 -->
        <div class="onlineEnroll-model">
            <div id="showScore" style="display: none">
                <div class="wt">基准分：</div>
                <asp:TextBox ID="txtScore" CssClass="txtdoenlista" runat="server" Height="25px"></asp:TextBox>
                <asp:RangeValidator ID="rVScore" CssClass="redStar" runat="server" ErrorMessage="只能输入1-100的数字" Type="Integer" ControlToValidate="txtScore" MinimumValue="1" MaximumValue="100"></asp:RangeValidator>
                <asp:RequiredFieldValidator ID="rFScore" class="redStar" runat="server" ErrorMessage="必填" ControlToValidate="txtScore"></asp:RequiredFieldValidator>
            </div>
        </div>
        <!-- RM隐藏作品要求 -->
        <div class="onlineEnroll-model">
            <div id="divClose" class="onlineEnroll-worksRequirements">
                <!--关闭按钮 -->
                <input type="button" id="btnHClose" value="关闭X" class="redStar" style="border: 0; background-color: Transparent; float: right" />
                <div class="onlineEnroll-model" style="padding-right: 5px; clear: right;">
                    <!--授课教师-->
                    <div class="mt20" style="font-size: 14px">
                        发布教师:&nbsp;<span style="font-size: 16px; font-weight: bold; color: darkblue;"><asp:Label ID="lblTeacher" runat="server" Text=""></asp:Label></span>
                        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                        团队人数:&nbsp;<span style="font-size: 16px; color: red;">≤<asp:Label ID="lblNum" runat="server" Text=""></asp:Label></span>&nbsp;人
                        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                        当前已有:&nbsp;<span style="font-size: 16px; font-weight: bold; color: red;"><asp:Label ID="lblWorksNum" runat="server" Text="0"></asp:Label></span>&nbsp;用户上传作品
                        <asp:HiddenField ID="hfEndTime" runat="server" />
                    </div>
                    <!-- 最多参与人数 -->

                    <br />
                    <!--授课教师-->
                    <!--时间阶段-->
                    <div>
                        <div>
                            作品提交时间:&nbsp;&nbsp;<asp:Label ID="lblSubmit" runat="server" Text=""></asp:Label>&nbsp;&nbsp;&nbsp;&nbsp;
                            <span>距离上传截止还有</span>&nbsp;
                            <span id="times_day" style="font-size: 16px; font-weight: bold; color: red;"></span>&nbsp;天&nbsp;
                            <span id="times_hour" style="font-size: 16px; font-weight: bold; color: red;"></span>&nbsp;时&nbsp;
                            <span id="times_minute" style="font-size: 16px; font-weight: bold; color: red;"></span>&nbsp;分&nbsp;
                            <span id="second" style="font-size: 16px; font-weight: bold; color: red;"></span>&nbsp;秒
                        </div>
                        <div class="mt10">
                            作品评分时间:&nbsp;&nbsp;<asp:Label ID="lblScorelbl" runat="server" Text=""></asp:Label>
                        </div>
                        <div class="mt10">
                            作品公示时间:&nbsp;&nbsp;<asp:Label ID="lblPublic" runat="server" Text=""></asp:Label>
                        </div>
                    </div>
                    <!--期次要求-->
                    <div class="mt20" style="width: 750px;">
                        <label for="txtWorksName"><b style="font-size: 16px; font-weight: bold;">作品要求：</b></label>
                    </div>
                    <div class="w600" style="line-height: 1.5">
                        <asp:Literal ID="lilRequire" runat="server"></asp:Literal>
                    </div>

                    <br />
                </div>

            </div>
        </div>

        <!--作品名称-->
        <div class="onlineEnroll-model">
            <div class="wt">
                <span class="redStar">*</span><label for="txtWorksName">作品名称：</label>
            </div>
            <asp:TextBox ID="txtWorksName"  runat="server" CssClass="onlineEnroll-txtBoxName" CausesValidation="True"></asp:TextBox>
            <input  style="border:0"   id="errWorksName"  class="redStar"  />
        </div>
        <div class="onlineEnroll-model" id="divtype">
            <div class="wt">
                <span class="redStar">*</span><label for="ddlOneWorksType">作品类别：</label>
            </div>
            <asp:DropDownList ID="ddlOneWorksType" runat="server" CssClass="txtdoenlista" Height="25px"
                AutoPostBack="True">
            </asp:DropDownList>
            <asp:DropDownList ID="ddlTwoWorksType" runat="server" CssClass="txtdoenlista" Height="25px">
            </asp:DropDownList>

        </div>
        <!--队员-->
        <div class="onlineEnroll-model" style="display: none" id="showUser">
            <div class="wt">
                <span class="redStar">*</span><label for="user">团队成员：</label>
            </div>
            <SharePoint:PeopleEditor runat="server" ID="user" MultiSelect="true" SelectionSet="User" EntitySeparator="," />

            <div class="onlineEnroll-model" style="margin: 0">
                <div class="GetWarnDiv" style="height: 100px">
                    <span style="font-weight: bord;">注意事项：</span>
                    <ul style="color: red">
                        <li style="border-bottom: 1px solid #666 dotted;">1、输入团队成员学号(不用输入当前登录用户的学号),个人报名无需填写团队成员</li>
                        <li style="border-bottom: 1px solid #666 dotted;">2、成员学号用逗号隔开，填写成员学号后请点击“检查名称”，确保所填写团队成员为有效用户</li>
                        <li style="border-bottom: 1px solid #666 dotted;">3、点击“检查名称”，检查团队成员是否有效；点击“浏览”，搜索团队成员信息</li>
                    </ul>
                </div>
            </div>
        </div>

        <div class="onlineEnroll-model">
            <div class="wt">
                <span class="redStar">*</span><label for="txtSubmitProfile">作品简介：</label>
            </div>
            <asp:TextBox CssClass="onlineEnroll-txtBox" runat="server" ID="txtSubmitProfile" TextMode="MultiLine" onfocus="onfts()"
                onblur="onbts()"></asp:TextBox>
            <span class="warn" id="spts">&#9786字数在50-500字</span>
            <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ControlToValidate="txtSubmitProfile"
                ErrorMessage="必填" CssClass="redStar"></asp:RequiredFieldValidator>
            <asp:Label ID="errorSubmitProfile" CssClass="redStar" runat="server" Text=""></asp:Label>
        </div>
        <div class="onlineEnroll-model">
            <div class="wt">
                <span class="redStar">*</span><label for="txtDesignIdeas">设计思路：</label>
            </div>
            <asp:TextBox ID="txtDesignIdeas" runat="server" CssClass="onlineEnroll-txtBox" TextMode="MultiLine" onfocus="onftsCard2()"
                onblur="onbtsCard2()"></asp:TextBox>
            <span class="warn" id="sCard2">&#9786字数在500-2000字</span>
            <asp:RequiredFieldValidator ID="RequiredFieldValidator5" runat="server" ErrorMessage="必填"
                ControlToValidate="txtDesignIdeas" CssClass="redStar"></asp:RequiredFieldValidator>
            <asp:Label ID="errorDesignIdeas" CssClass="redStar" runat="server" Text=""></asp:Label>
        </div>
        <div class="onlineEnroll-model">
            <div class="wt">
                <span class="redStar">*</span>
                <label for="txtKeyPoints">创意特色：</label>
            </div>
            <asp:TextBox ID="txtKeyPoints" runat="server" CssClass="onlineEnroll-txtBox" TextMode="MultiLine" onfocus="onftsCard3()"
                onblur="onbtsCard3()"></asp:TextBox>
            <span class="warn" id="sCard3">&#9786字数在200-800字</span>
            <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" ControlToValidate="txtKeyPoints"
                ErrorMessage="必填" CssClass="redStar"></asp:RequiredFieldValidator>
            <asp:Label ID="errorKeyPoints" CssClass="redStar" runat="server" Text=""></asp:Label>
        </div>
        <div class="onlineEnroll-model">
            <div class="wt">
                <span class="redStar"></span>
                <label for="sCard2txt">外部资源：</label>
            </div>
            <asp:TextBox ID="txtDemo1URL" runat="server" Style="margin-top: 0px" CssClass="txtcss"
                Width="400px"></asp:TextBox>

        </div>
        <div class="onlineEnroll-model">
            <div class="wt">
                <span class="redStar">*</span>作品文件：
            </div>
            <div>
                <asp:FileUpload ID="imageFileUpload" runat="server" />&nbsp;&nbsp;&nbsp;
                <asp:Button ID="btnImageUpload" runat="server" Text="上传" CausesValidation="False" />
            </div>
            <div id="divImage" style="text-align: left; margin: 0; padding-left: 125px; padding-top: 5px" class="wd600">

                <asp:GridView ID="gvImage" runat="server" AutoGenerateColumns="False" AllowPaging="False" CellPadding="0"
                    Font-Size="14px" ForeColor="#494b4c" GridLines="None" Height="30px" Width="475px"
                    BorderStyle="None" CssClass="grouptd">
                    <EmptyDataTemplate>
                        <table>
                            <tr>
                                <td>作品文件、效果图片、其他附件
                                </td>
                            </tr>
                            <tr>
                                <td colspan="5">当前没有数据
                                </td>
                            </tr>
                        </table>
                    </EmptyDataTemplate>
                    <RowStyle BackColor="#ffffe1" />
                    <Columns>
                        <asp:BoundField DataField="WorksFileID" HeaderText="作品ID">
                            <FooterStyle CssClass="hidden" />
                            <HeaderStyle CssClass="hidden" />
                            <ItemStyle Width="60px" CssClass="hidden"></ItemStyle>
                        </asp:BoundField>
                        <asp:BoundField DataField="FileName" HeaderText="作品文件、效果图片、其他附件" />
                        <asp:BoundField DataField="FilePath" HeaderText="路径">
                            <FooterStyle CssClass="hidden" />
                            <HeaderStyle CssClass="hidden" />
                            <ItemStyle Width="60px" CssClass="hidden"></ItemStyle>
                        </asp:BoundField>
                        <asp:BoundField DataField="FileSize" HeaderText="文件大小">
                            <FooterStyle CssClass="hidden" />
                            <HeaderStyle CssClass="hidden" />
                            <ItemStyle Width="60px" CssClass="hidden"></ItemStyle>
                        </asp:BoundField>
                        <asp:BoundField DataField="Type" HeaderText="文件类型">
                            <FooterStyle CssClass="hidden" />
                            <HeaderStyle CssClass="hidden" />
                            <ItemStyle Width="60px" CssClass="hidden"></ItemStyle>
                        </asp:BoundField>
                        <asp:TemplateField HeaderText="删除" ItemStyle-Width="120px">
                            <ItemTemplate>
                                <asp:Button ID="btnDel" CausesValidation="false" CommandName="Del" Text="删除" runat="server"
                                    CssClass="buttoncssb" BorderStyle="None" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <%-- <asp:TemplateField HeaderText="下载" ItemStyle-Width="120px">
                            <ItemTemplate>
                                <asp:Button ID="btnDown" CausesValidation="false" CommandName="Down" Text="下载" runat="server"
                                    CssClass="buttoncssb" BorderStyle="None" />
                            </ItemTemplate>
                        </asp:TemplateField>--%>
                    </Columns>
                    <HeaderStyle BackColor="#494B4C" ForeColor="#E6E6E6" HorizontalAlign="Left" />
                    <PagerStyle HorizontalAlign="Center" />
                </asp:GridView>
            </div>
        </div>
        <div class="onlineEnroll-model" style="margin: 0">
            <div class="GetWarnDiv" style="height: 100px">
                注意事项：
                <ul style="color: red">
                    <li style="border-bottom: 1px solid #666 dotted;">1、点击上传按钮后请耐心等待，视频文件不大于50M；</li>
                    <li style="border-bottom: 1px solid #666 dotted;">2、上传的图片应重点体现作品的整体、关键点和特效；</li>
                    <li style="border-bottom: 1px solid #666 dotted;">3、要求反映作品整体、关键点和特效的画面；</li>

                </ul>
            </div>
        </div>
        <div class="onlineEnroll-model">
            <div class="wt">
                <span class="redStar">*</span>讲解视频：
            </div>
            <div>
                <asp:FileUpload ID="fileUpload" runat="server" />&nbsp;&nbsp;&nbsp;
                <asp:Button ID="btnUpFile" runat="server" Text="上传" CausesValidation="False" />
                <asp:Label ID="jiangJieError" class="redStar" runat="server" Text=""></asp:Label>
            </div>
            <div id="Attachment" style="text-align: left; margin: 0; padding-left: 125px; padding-top: 5px" class="wd600">

                <asp:GridView ID="gvWorks" runat="server" AutoGenerateColumns="False" AllowPaging="False" CellPadding="0"
                    Font-Size="14px" ForeColor="#494b4c" GridLines="None" Height="30px" Width="475px"
                    BorderStyle="None" CssClass="grouptd">
                    <EmptyDataTemplate>
                        <table>
                            <tr>
                                <td>作品介绍、讲解的音频或视频文件
                                </td>
                            </tr>
                            <tr>
                                <td colspan="5">当前没有数据
                                </td>
                            </tr>
                        </table>
                    </EmptyDataTemplate>
                    <RowStyle BackColor="#ffffe1" />
                    <Columns>
                        <asp:BoundField DataField="WorksFileID" HeaderText="作品ID">
                            <FooterStyle CssClass="hidden" />
                            <HeaderStyle CssClass="hidden" />
                            <ItemStyle Width="60px" CssClass="hidden"></ItemStyle>
                        </asp:BoundField>
                        <asp:BoundField DataField="FileName" HeaderText="作品介绍、讲解的音频或视频文件" />
                        <asp:BoundField DataField="FilePath" HeaderText="路径">
                            <FooterStyle CssClass="hidden" />
                            <HeaderStyle CssClass="hidden" />
                            <ItemStyle Width="60px" CssClass="hidden"></ItemStyle>
                        </asp:BoundField>
                        <asp:BoundField DataField="FileSize" HeaderText="文件大小">
                            <FooterStyle CssClass="hidden" />
                            <HeaderStyle CssClass="hidden" />
                            <ItemStyle Width="60px" CssClass="hidden"></ItemStyle>
                        </asp:BoundField>
                        <asp:BoundField DataField="Type" HeaderText="文件类型">
                            <FooterStyle CssClass="hidden" />
                            <HeaderStyle CssClass="hidden" />
                            <ItemStyle Width="60px" CssClass="hidden"></ItemStyle>
                        </asp:BoundField>
                        <asp:TemplateField HeaderText="删除" ItemStyle-Width="120px">
                            <ItemTemplate>
                                <asp:Button ID="btnDel" CausesValidation="false" CommandName="Del" Text="删除" runat="server"
                                    CssClass="buttoncssb" BorderStyle="None" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <%-- <asp:TemplateField HeaderText="下载" ItemStyle-Width="120px">
                            <ItemTemplate>
                                <asp:Button ID="btnDown" CausesValidation="false" CommandName="Down" Text="下载" runat="server"
                                    CssClass="buttoncssb" BorderStyle="None" />
                            </ItemTemplate>
                        </asp:TemplateField>--%>
                    </Columns>
                    <HeaderStyle BackColor="#494B4C" ForeColor="#E6E6E6" HorizontalAlign="Left" />
                    <PagerStyle HorizontalAlign="Center" />
                </asp:GridView>
            </div>
        </div>
        <div class="onlineEnroll-model" style="margin: 0">
            <div class="GetWarnDiv" style="height: 45px">
                <span style="font-weight: bord;">注意事项：</span>
                <ul style="color: red">
                    <li style="border-bottom: 1px solid #666 dotted;">1、上传视频格式：mp4；</li>
                    <li style="border-bottom: 1px solid #666 dotted;">2、视频文件大小：50M之内；</li>

                </ul>
            </div>
        </div>
        <!-- 按钮 -->
        <div>
            &nbsp;
        </div>
        <div style="margin-left: 135px;">
            <asp:Button ID="btnSubmit" runat="server" OnClientClick="return ValidateTitle()" Text="提交，不可改" Style="width: 133px; height: 35px; background: url(images/ButtonBg.gif); border: 0; color: #fff; font-weight: bolder; font-size: 15px; margin: 0; cursor: pointer;" ToolTip="作品提交后不得更改！" />
            <asp:Button ID="btnSave" runat="server" OnClientClick="return ValidateTitle()" Text="保存，可更改" Style="width: 133px; height: 35px; background: url(images/ButtonGg.gif); border: 0; color: #fff; font-weight: bolder; font-size: 15px; margin: 0; cursor: pointer" CausesValidation="False" />

        </div>
    </div>
    <div>
        &nbsp;
    </div>

</asp:Content>

<asp:Content ID="PageTitle" ContentPlaceHolderID="PlaceHolderPageTitle" runat="server">
    提交作品
</asp:Content>

<asp:Content ID="PageTitleInTitleArea" ContentPlaceHolderID="PlaceHolderPageTitleInTitleArea" runat="server">
    提交作品
</asp:Content>
