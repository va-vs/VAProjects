<%@ Assembly Name="FSCWebParts, Version=1.0.0.0, Culture=neutral, PublicKeyToken=0a9f3aab874d6c3e" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Register Tagprefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="PerformanceSummaryUserControl.ascx.cs" Inherits="FSCWebParts.PerformanceSummary.PerformanceSummaryUserControl" %>
 <style type="text/css">
     .perfomancediv{
         margin:10px;
         min-width:450px;
         max-width:820px;
         text-align:center;
     }
     .perfomancediv button{
         cursor:pointer;
     }
        .mytable
        {
            border-collapse: collapse;
            margin: 0 auto;
            text-align: center;
            min-width:400px;
            max-width:900px;
            width:100%;
            border:2px solid #000000;

        }
        .mytable td, .mytable th
        {
            border: 1px solid #cad9ea;
            color: #666;
            line-height:25px;
            /*min-width:100px;*/
            max-width:240px;
        }
        .mytable td span{
            text-decoration:solid;
            color:#0072C6;
        }
         .mytable td p{
            display:none;
            position:absolute;
            z-index:999;
            background-color: #EBEBEB;
        }
        .mytable td a, .mytable th a{
            text-decoration:solid;
            color:#0072C6;
        }
        .mytable td a:hover, .mytable th a:hover{
            color:orangered;
        }
        .mytable thead th
        {
            background-color: #CCE8EB;
            width: 100px;
        }
        .mytable tr:nth-child(odd)
        {
            background: #fff;
        }
        .mytable tr:nth-child(even)
        {
            background: #F5FAFA;
        }

        .prtBtn{
            float:right;
            cursor:pointer;
            font-weight:600;
            font-size:14px;
        }

        .pbtn{
            cursor:pointer;
            font-size:14px;
        }
        .pbtn:hover{
            background-color:#CCE8EB;
            font-weight:600;
        }
    </style>

    <script type="text/JavaScript">
        function getmall(){
            var lis = document.getElementsByName("tooltip");
            var div_top = lis.offsetTop;
            window.scrollTo(0, div_top - 200);
        }

        function printdiv(printpage) {
            var headhtml = "<html><head><title></title></head><body>";
            var foothtml = "</body>";
            // 获取div中的html内容
            var newhtml = document.all.item(printpage).innerHTML;
            // 获取div中的html内容，jquery写法如下
            // var newhtml= $("#" + printpage).html();

            // 获取原来的窗口界面body的html内容，并保存起来
            var oldhtml = document.body.innerHTML;

            // 给窗口界面重新赋值，赋自己拼接起来的html内容
            document.body.innerHTML = headhtml + newhtml + foothtml;
            // 调用window.print方法打印新窗口
            window.print();

            // 将原来窗口body的html值回填展示
            document.body.innerHTML = oldhtml;
            return false;
        }

        function showimg(title,lyId,sUrl) {
            document.getElementById(lyId).innerHTML = title + "<br/><img border='0' src=\"" + sUrl + "\">";
            document.getElementById(lyId).style.display = "block";
        }
        function hidimg(lyId) {
            document.getElementById(lyId).innerHTML = "";
            document.getElementById(lyId).style.display = "none";
        }
    </script>

<asp:UpdatePanel ID="udpPerformance" runat="server">
    <ContentTemplate>
        <div class="perfomancediv" style="margin-bottom: 10px; text-align: left; border-bottom: 1px solid #000000; padding-bottom: 10px;" id="divQuery" runat="server">
            <asp:Label ID="lbYear" runat="server" Font-Size="12" Text="选择年度："></asp:Label>
            <asp:DropDownList ID="ddlYears" runat="server" AutoPostBack="True" OnSelectedIndexChanged="ddlYears_SelectedIndexChanged" Style="height: 20px;">
            </asp:DropDownList>
            &nbsp;&nbsp;
                <asp:Label ID="lbID" runat="server" Font-Size="12" Text="管理员可查询他人，请输入工号："></asp:Label>
            <asp:TextBox ID="tbID" runat="server" ToolTip="请输入要查询的工号" Style="height: 20px;"></asp:TextBox>
            <asp:Button ID="btnQuery" runat="server" Text="查 询" CssClass="pbtn" OnClick="btnQuery_Click" />
            <asp:Button ID="btnPrint" runat="server" Text="打 印" CssClass="prtBtn" OnClientClick="javascript: printdiv('divPrint')" />

        </div>

        <div id="divPerformance" runat="server" class="perfomancediv" visible="false">
            <div id="divPrint">
                <div style="text-align: center; margin-bottom: 10px; color: #0072c6">
                    <asp:Label ID="lbTitle" runat="server" Font-Size="18" Text="2018年度个人业绩汇总"></asp:Label>
                </div>

                <table class="mytable" cellspacing="0" cellpadding="0" border="1" bordercolor="#cad9ea">
                    <tr>
                        <th colspan="2" style="border-top: 2px solid #333; border-left: 2px solid #333;">姓名</th>
                        <td colspan="2" style="border-top: 2px solid #333;">
                            <asp:Label ID="lbName" runat="server" Text="张老师" ToolTip="姓名"></asp:Label></td>
                        <th colspan="2" style="border-top: 2px solid #333;">工号</th>
                        <td colspan="2" style="border-top: 2px solid #333;border-right: 2px solid #333;">
                            <asp:Label ID="lbIDCard" runat="server" Text="00001" ToolTip="工号"></asp:Label>
                        </td>

                    </tr>
                    <tr>
                        <th colspan="2" style="border-bottom: 2px solid #333; border-left: 2px solid #333;">职称</th>
                        <td colspan="2" style="border-bottom: 2px solid #333;">
                            <asp:Label ID="lbProf" runat="server" Text="教授" ToolTip="职称"></asp:Label>
                        </td>
                        <th colspan="2" style="border-bottom: 2px solid #333;">系部</th>
                        <td colspan="2" style="border-bottom: 2px solid #333;border-right: 2px solid #333;">
                            <asp:Label ID="lbDept" runat="server" Text="英语系" ToolTip="系部"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <th colspan="7" style="border-bottom: 2px solid #333; border-left: 2px solid #333;">人头费</th>
                        <td style="border-bottom: 2px solid #333;border-right: 2px solid #333;">
                            <asp:Label ID="lbOne" runat="server" Text="尚未公布" Font-Size="12" ToolTip="每个教师的职称绩效金额"></asp:Label></td>

                    </tr>
                    <tr>
                        <th rowspan="17" style="border-left: 2px #000 solid; width: 25px; border-bottom: 2px solid #333;">教学课时</th>
                        <th rowspan="7">本科教学课时</th>
                        <th>公外计划总学时</th>
                        <td onmousemove="showimg('本科教学公外计划总学时当量计算规则','ly1','http://www.fsc.neu.edu.cn/Performance/SiteAssets/images/ComputeRules/本科公外计划总学时.jpg')" onmouseout="hidimg('ly1')">
                            <asp:Label ID="lbgwjh_bk" runat="server" Text="0.00"></asp:Label>
                            <p id="ly1"></p>
                        </td>
                        <th colspan="3" rowspan="17" style="border-bottom: 2px solid #333;" width="20px">课时总金额</th>
                        <td rowspan="17" style="border-bottom: 2px solid #333;border-right: 2px solid #333;">
                            <asp:Label ID="lbkszje" runat="server" Text="尚未公布" Font-Size="12" ToolTip="教学课时总金额"></asp:Label></td>
                    </tr>
                         <tr>
                        <th>专业计划总学时</th>
                        <td onmousemove="showimg('本科教学专业计划总学时当量计算规则','ly2','http://www.fsc.neu.edu.cn/Performance/SiteAssets/images/ComputeRules/本科专业计划总学时.jpg')" onmouseout="hidimg('ly2')">
                            <asp:Label ID="lbzyjh_bk" runat="server" Text="0.00"></asp:Label>
                            <p id="ly2">
                            </p>
                        </td>
                    </tr>
                    <tr>
                        <th>公外折算总学时</th>
                        <td onmousemove="showimg('本科公外折算总学时当量计算规则','ly3','http://www.fsc.neu.edu.cn/Performance/SiteAssets/images/ComputeRules/本科公外折算总学时.jpg')" onmouseout="hidimg('ly3')">
                            <asp:Label ID="lbgwzs_bk" runat="server" Text="0.00"></asp:Label>
                            <p id="ly3">
                            </p>
                        </td>
                    </tr>
                    <tr>
                        <th>专业折算总学时</th>
                        <td onmousemove="showimg('本科专业折算总学时当量计算规则','ly4','http://www.fsc.neu.edu.cn/Performance/SiteAssets/images/ComputeRules/本科专业折算总学时.jpg')" onmouseout="hidimg('ly4')">
                            <asp:Label ID="lbzyzs_bk" runat="server" Text="0.00"></asp:Label>
                            <p id="ly4"></p>

                        </td>
                    </tr>
                    <tr>
                        <th>指导社会实践</th>
                        <td onmousemove="showimg('指导社会实践当量计算规则','ly5','http://www.fsc.neu.edu.cn/Performance/SiteAssets/images/ComputeRules/指导社会实践.jpg')" onmouseout="hidimg('ly5')">
                            <asp:Label ID="lbzdshsj" runat="server" Text="0.00" ></asp:Label>
                            <p id="ly5"></p>
                        </td>
                    </tr>
                    <tr>
                        <th>指导毕业论文</th>
                        <td onmousemove="showimg('本科指导毕业论文当量计算规则','ly6','http://www.fsc.neu.edu.cn/Performance/SiteAssets/images/ComputeRules/本科指导毕业论文.jpg')" onmouseout="hidimg('ly6')">
                            <asp:Label ID="lbzdbylw_bk" runat="server" Text="0.00"></asp:Label>
                           <p id="ly6"></p>
                        </td>
                    </tr>
                    <tr>
                        <th>新开课</th>
                        <td onmousemove="showimg('本科新开课当量计算规则','ly7','http://www.fsc.neu.edu.cn/Performance/SiteAssets/images/ComputeRules/本科新开课.jpg')" onmouseout="hidimg('ly7')">
                            <asp:Label ID="lbxkk_bk" runat="server" Text="0.00"></asp:Label>
                            <p id="ly7"></p>
                        </td>
                    </tr>
                    <tr>
                        <th rowspan="7">研究生教学课时</th>
                        <th>公外计划总学时</th>
                        <td onmousemove="showimg('研究生公外计划总学时当量计算规则','ly8','http://www.fsc.neu.edu.cn/Performance/SiteAssets/images/ComputeRules/研究生公外计划总学时.jpg')" onmouseout="hidimg('ly8')">
                            <asp:Label ID="lbgwjh_yjs" runat="server" Text="0.00"></asp:Label>
					        <p id="ly8"></p>

                        </td>
                    </tr>
                    <tr>
                        <th>专业计划总学时</th>
                        <td onmousemove="showimg('研究生专业计划总学时当量计算规则','ly9','http://www.fsc.neu.edu.cn/Performance/SiteAssets/images/ComputeRules/研究生专业计划总学时.jpg')" onmouseout="hidimg('ly9')">
                            <asp:Label ID="lbzyjh_yjs" runat="server" Text="0.00"></asp:Label>
                            <p id="ly9"></p>
                        </td>
                    </tr>
                    <tr>
                        <th>公外折算总学时</th>
                        <td onmousemove="showimg('研究生公外折算总学时当量计算规则','ly10','http://www.fsc.neu.edu.cn/Performance/SiteAssets/images/ComputeRules/研究生公外折算总学时.jpg')" onmouseout="hidimg('ly10')">
                            <asp:Label ID="lbgwzs_yjs" runat="server" Text="0.00"></asp:Label>
                            <p id="ly10"></p>
                        </td>
                    </tr>
                    <tr>
                        <th>专业折算总学时</th>
                        <td onmousemove="showimg('研究生专业折算总学时当量计算规则','ly11','http://www.fsc.neu.edu.cn/Performance/SiteAssets/images/ComputeRules/研究生专业折算总学时.jpg')" onmouseout="hidimg('ly11')">
                            <asp:Label ID="lbzyzs_yjs" runat="server" Text="0.00"></asp:Label>
                            <p id="ly11"></p>
                        </td>
                    </tr>
                    <tr>
                        <th>指导研究生</th>
                        <td onmousemove="showimg('指导研究生当量计算规则','ly12','http://www.fsc.neu.edu.cn/Performance/SiteAssets/images/ComputeRules/指导研究生.jpg')" onmouseout="hidimg('ly12')">
                            <asp:Label ID="lbzdyjs" runat="server" Text="0.00"></asp:Label>
                           <p id="ly12"></p>
                        </td>
                    </tr>
                    <tr>
                        <th>指导毕业论文</th>
                        <td onmousemove="showimg('研究生指导毕业论文当量计算规则','ly13','http://www.fsc.neu.edu.cn/Performance/SiteAssets/images/ComputeRules/研究生指导毕业论文.jpg')" onmouseout="hidimg('ly13')">
                            <asp:Label ID="lbzdbylw_yjs" runat="server" Text="0.00"></asp:Label>
                           <p id="ly13"></p>
                        </td>
                    </tr>
                    <tr>
                        <th>研究生批卷</th>
                        <td onmousemove="showimg('研究生批卷当量计算规则','ly14','http://www.fsc.neu.edu.cn/Performance/SiteAssets/images/ComputeRules/研究生批卷.jpg')" onmouseout="hidimg('ly14')">
                            <asp:Label ID="lbyjspj" runat="server" Text="0.00"></asp:Label>
                           <p id="ly14"></p>
                        </td>
                    </tr>
                    <tr>
                        <th rowspan="3" style="border-bottom: 2px solid #333;">课时汇总</th>
                        <th>公外计划总学时<br />
                            （本科+研究生）</th>
                        <td onmousemove="showimg('公外计划总学时当量计算规则','ly15','http://www.fsc.neu.edu.cn/Performance/SiteAssets/images/ComputeRules/公外计划总学时.jpg')" onmouseout="hidimg('ly15')">
                            <asp:Label ID="lbgwjh" runat="server" Text="0.00"></asp:Label>
                           <p id="ly15"></p>
                        </td>
                    </tr>
                    <tr>
                        <th>专业计划总学时<br />
                            （本科+研究生）</th>
                        <td onmousemove="showimg('专业计划总学时当量计算规则','ly16','http://www.fsc.neu.edu.cn/Performance/SiteAssets/images/ComputeRules/专业计划总学时.jpg')" onmouseout="hidimg('ly16')">
                            <asp:Label ID="lbzyjh" runat="server" Text="0.00"></asp:Label>
                           <p id="ly16"></p>
                        </td>
                    </tr>
                    <tr>
                        <th style="border-bottom: 2px solid #333;">计划外总学时<br />
                            （公外+专业）</th>
                        <td style="border-bottom: 2px solid #333;" onmousemove="showimg('计划外总学时当量计算规则','ly17','http://www.fsc.neu.edu.cn/Performance/SiteAssets/images/ComputeRules/计划外总学时.jpg')" onmouseout="hidimg('ly17')">
                            <asp:Label ID="lbjhw" runat="server" Text="0.00"></asp:Label>
                            <p id="ly17"></p>
                        </td>
                    </tr>
                    <tr>
                        <th rowspan="17" width="20px" style="border-left: 2px solid #333; border-bottom: 2px solid #333;">业绩点</th>
                        <th rowspan="15">业绩点项</th>
                        <th>教学立项</th>
                        <td onmousemove="showimg('教学立项当量计算规则','ly18','http://www.fsc.neu.edu.cn/Performance/SiteAssets/images/ComputeRules/教学立项.jpg')" onmouseout="hidimg('ly18')">
                            <asp:Label ID="lbjxlx" runat="server" Text="0.00"></asp:Label>
                            <p id="ly18"></p>
                        </td>
                        <th rowspan="15" width="50px">业绩点项<br />
                            总金额</th>
                        <td rowspan="15">

                            <asp:Label ID="lbyjdz" runat="server" Text="尚未公布" Font-Size="12"></asp:Label>
                        </td>
                        <th rowspan="17" width="50px" style="border-bottom: 2px solid #333;">业绩点合计<br />
                            总金额</th>
                        <td rowspan="17" style="border-bottom: 2px solid #333;border-right: 2px solid #333;">
                            <asp:Label ID="lbyjdhjz" runat="server" Text="尚未公布" Font-Size="12"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <th>教材</th>
                        <td onmousemove="showimg('教材当量计算规则','ly19','http://www.fsc.neu.edu.cn/Performance/SiteAssets/images/ComputeRules/教材.jpg')" onmouseout="hidimg('ly19')">
                            <asp:Label ID="lbjc" runat="server" Text="0.00"></asp:Label>
                            <p id="ly19"></p>
                        </td>
                    </tr>
                    <tr>
                        <th>论文</th>
                        <td onmousemove="showimg('论文当量计算规则','ly20','http://www.fsc.neu.edu.cn/Performance/SiteAssets/images/ComputeRules/论文.jpg')" onmouseout="hidimg('ly20')">
                            <asp:Label ID="lblw" runat="server" Text="0.00"></asp:Label>
                            <p id="ly20"></p>
                        </td>
                    </tr>
                    <tr>
                        <th>专著</th>
                        <td onmousemove="showimg('专著当量计算规则','ly21','http://www.fsc.neu.edu.cn/Performance/SiteAssets/images/ComputeRules/专著.jpg')" onmouseout="hidimg('ly21')">
                            <asp:Label ID="lbzz" runat="server" Text="0.00"></asp:Label>
                            <p id="ly21"></p>
                        </td>
                    </tr>
                    <tr>
                        <th>译著</th>
                        <td onmousemove="showimg('译著当量计算规则','ly22','http://www.fsc.neu.edu.cn/Performance/SiteAssets/images/ComputeRules/译著.jpg')" onmouseout="hidimg('ly22')">
                            <asp:Label ID="lbyz" runat="server" Text="0.00"></asp:Label>
                            <p id="ly22"></p>
                        </td>
                    </tr>
                    <tr>
                        <th>教学获奖</th>
                        <td onmousemove="showimg('教学获奖当量计算规则','ly23','http://www.fsc.neu.edu.cn/Performance/SiteAssets/images/ComputeRules/教学获奖.jpg')" onmouseout="hidimg('ly23')">
                            <asp:Label ID="lbjxhj" runat="server" Text="0.00"></asp:Label>
                            <p id="ly23"></p>
                        </td>
                    </tr>
                    <tr>
                        <th>教学竞赛</th>
                        <td onmousemove="showimg('教学竞赛当量计算规则','ly24','http://www.fsc.neu.edu.cn/Performance/SiteAssets/images/ComputeRules/教学竞赛.jpg')" onmouseout="hidimg('ly24')">
                            <asp:Label ID="lbjxjs" runat="server" Text="0.00"></asp:Label>
                            <p id="ly24"></p>
                        </td>
                    </tr>
                    <tr>
                        <th>科研立项</th>
                        <td onmousemove="showimg('科研立项当量计算规则','ly25','http://www.fsc.neu.edu.cn/Performance/SiteAssets/images/ComputeRules/科研立项.jpg')" onmouseout="hidimg('ly25')">
                            <asp:Label ID="lbkylx" runat="server" Text="0.00"></asp:Label>
                            <p id="ly25"></p>
                        </td>
                    </tr>
                    <tr>
                        <th>科研成果</th>
                        <td onmousemove="showimg('科研成果当量计算规则','ly26','http://www.fsc.neu.edu.cn/Performance/SiteAssets/images/ComputeRules/科研成果.jpg')" onmouseout="hidimg('ly26')">
                            <asp:Label ID="lbkycg" runat="server" Text="0.00"></asp:Label>
                            <p id="ly26"></p>
                        </td>
                    </tr>
                    <tr>
                        <th>行政职务</th>
                        <td onmousemove="showimg('行政职务当量计算规则','ly27','http://www.fsc.neu.edu.cn/Performance/SiteAssets/images/ComputeRules/行政职务.jpg')" onmouseout="hidimg('ly27')">
                            <asp:Label ID="lbxzzw" runat="server" Text="0.00"></asp:Label>
                            <p id="ly27"></p>
                        </td>
                    </tr>
                    <tr>
                        <th>学科建设</th>
                        <td onmousemove="showimg('学科建设当量计算规则','ly28','http://www.fsc.neu.edu.cn/Performance/SiteAssets/images/ComputeRules/学科建设.jpg')" onmouseout="hidimg('ly28')">
                            <asp:Label ID="lbxkjs" runat="server" Text="0.00"></asp:Label>
                            <p id="ly28"></p>
                        </td>
                    </tr>
                    <tr>
                        <th>人才引进</th>
                        <td onmousemove="showimg('人才引进当量计算规则','ly29','http://www.fsc.neu.edu.cn/Performance/SiteAssets/images/ComputeRules/人才引进.jpg')" onmouseout="hidimg('ly29')">
                            <asp:Label ID="lbrcyj" runat="server" Text="0.00"></asp:Label>
                            <p id="ly29"></p>
                        </td>
                    </tr>
                    <tr>
                        <th>学术兼职</th>
                        <td onmousemove="showimg('学术兼职当量计算规则','ly30','http://www.fsc.neu.edu.cn/Performance/SiteAssets/images/ComputeRules/学术兼职.jpg')" onmouseout="hidimg('ly30')">
                            <asp:Label ID="lbxsjz" runat="server" Text="0.00"></asp:Label>
                           <p id="ly30"></p>
                        </td>
                    </tr>
                    <tr>
                        <th>加分</th>
                        <td onmousemove="showimg('加分当量计算规则','ly31','http://www.fsc.neu.edu.cn/Performance/SiteAssets/images/ComputeRules/加分.jpg')" onmouseout="hidimg('ly31')">
                            <asp:Label ID="lbjiafen" runat="server" Text="0.00"></asp:Label>
                            <p id="ly31"></p>
                        </td>
                    </tr>
                    <tr>
                        <th>减分</th>
                        <td onmousemove="showimg('减分当量计算规则','ly32','http://www.fsc.neu.edu.cn/Performance/SiteAssets/images/ComputeRules/减分.jpg')" onmouseout="hidimg('ly32')">
                            <asp:Label ID="lbjianfen" runat="server" Text="0.00"></asp:Label>
                            <p id="ly32"></p>
                        </td>
                    </tr>
                    <tr>
                        <th rowspan="2" style="border-bottom: 2px solid #333;">金额调整</th>
                        <th colspan="2">助教补贴</th>
                        <th>+</th>
                        <td>
                            <asp:Label ID="lbzjbt" runat="server" Text="尚未公布" Font-Size="12" ToolTip="助教补贴调整增加金额"></asp:Label></td>
                    </tr>
                    <tr>
                        <th colspan="2" style="border-bottom: 2px solid #333;">缺勤</th>
                        <th style="border-bottom: 2px solid #333;">-</th>
                        <td style="border-bottom: 2px solid #333;">
                            <asp:Label ID="lbqueqin" runat="server" Text="尚未公布" Font-Size="12" ToolTip="缺勤扣减金额"></asp:Label></td>
                    </tr>
                    <tr>
                    <th colspan="7" style="border-top: 2px solid #333;border-left: 2px solid #333;height:30px;">
                    总金额（人头+课时+业绩点合计）
                    </th>
                    <td style="border-top: 2px solid #333;border-right: 2px solid #333;">
                     <asp:Label ID="lbzong" runat="server" Text="尚未公布" Font-Size="12" ToolTip="总金额（人头+课时+业绩点合计）"></asp:Label></td>
                    <tr>
                        <th colspan="7" style="border-bottom: 2px solid #333;border-left: 2px solid #333;height:30px;">月核拨 </th>
                        <td style="border-bottom: 2px solid #333;border-right: 2px solid #333;">
                            <asp:Label ID="lbyhb" runat="server" Font-Size="12" Text="尚未公布" ToolTip="月核拨金额"></asp:Label>
                        </td>
                </table>
            </div>

            <div id="pStatus" runat="server" style="text-align: left; color: #0072c6; line-height: 25px;">
                Tips:鼠标放置在当量值上时，可以查看该当量的计算规则
            </div>

            <div class="perfomancediv" id="GPACheck" runat="server" visible="false">
                <asp:Button ID="btnGPANoPass" runat="server" Text="绩点数有异议,请点击后与院办联系" Width="240px" OnClick="btnGPANoPass_Click" CssClass="pbtn" />&nbsp;&nbsp;
                        <asp:Button ID="btnGPAPass" runat="server" Text="绩点无异议，提交确认" Width="240px" OnClick="btnGPAPass_Click" CssClass="pbtn" />

            </div>
            <div class="perfomancediv" id="MoneyCheck" runat="server" visible="false">
                <asp:Button ID="btnMoneyNoPass" runat="server" Text="业绩金额有异议,请点击后与院办联系" CssClass="pbtn" Width="240px" OnClick="btnMoneyNoPass_Click" />&nbsp;&nbsp;
                <asp:Button ID="btnMoneyPass" runat="server" Text="业绩金额无异议，提交确认" CssClass="pbtn" Width="240px" OnClick="btnMoneyPass_Click" />
            </div>

        </div>

        <div id="divErr" runat="server" style="color: red; font-size: 14px; margin: 10px;"></div>
        <asp:HiddenField ID="hdFlag" runat="server" />
        <asp:HiddenField ID="hdId" runat="server" />
        <asp:HiddenField ID="hdIdCard" runat="server" />
    </ContentTemplate>
</asp:UpdatePanel>
