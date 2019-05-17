<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Import Namespace="Microsoft.SharePoint.ApplicationPages" %>
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="MyPerf.aspx.cs" Inherits="FSCAppPages.Layouts.FSCAppPages.MyPerf" DynamicMasterPageFile="~masterurl/default.master" %>

<asp:Content ID="PageHead" ContentPlaceHolderID="PlaceHolderAdditionalPageHead" runat="server">
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
            min-width:100px;
            max-width:240px;
        }
        .mytable td a, .mytable th a{
            text-decoration:none;
            color:red;
            float:right;
            margin-right:5px;
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
    </script>
    <script type="text/JavaScript">
        var imgObj;
        function checkImg(theURL,winName){
          // 对象是否已创建
          if (typeof(imgObj) == "object"){
            // 是否已取得了图像的高度和宽度
            if ((imgObj.width != 0) && (imgObj.height != 0))
              // 根据取得的图像高度和宽度设置弹出窗口的高度与宽度，并打开该窗口
              // 其中的增量 20 和 30 是设置的窗口边框与图片间的间隔量
              OpenFullSizeWindow(theURL,winName, ",width=" + (imgObj.width+20) + ",height=" + (imgObj.height+30));
            else
              // 因为通过 Image 对象动态装载图片，不可能立即得到图片的宽度和高度，所以每隔100毫秒重复调用检查
              setTimeout("checkImg('" + theURL + "','" + winName + "')", 100);
            }
        }

        function OpenFullSizeWindow(theURL,winName,features) {
            var aNewWin, sBaseCmd;
            var e = e || window.event;
            var x = e.clientX + document.body.scrollLeft + document.documentElement.scrollLef;
            var y = e.clientY + document.body.scrollTop + document.documentElement.scrollTop;

          // 弹出窗口外观参数
          sBaseCmd = "toolbar=no,location=no,status=no,menubar=no,scrollbars=no,resizable=no,left="+x+"top="+y;
          // 调用是否来自 checkImg
          if (features == null || features == ""){
            // 创建图像对象
            imgObj = new Image();
            // 设置图像源
            imgObj.src = theURL;
            // 开始获取图像大小
            checkImg(theURL, winName)
          }
          else{
            // 打开窗口
            aNewWin = window.open(theURL,winName, sBaseCmd + features);
            // 聚焦窗口
            aNewWin.focus();
          }
        }
    </script>
</asp:Content>

<asp:Content ID="Main" ContentPlaceHolderID="PlaceHolderMain" runat="server">

        <div class="perfomancediv" style="margin-bottom: 10px; text-align: left; border-bottom: 1px solid #000000; padding-bottom: 10px;" id="divQuery" runat="server">
            <asp:Label ID="lbYear" runat="server" Font-Size="12" Text="选择年度："></asp:Label>
            <asp:DropDownList ID="ddlYears" runat="server" AutoPostBack="True" OnSelectedIndexChanged="ddlYears_SelectedIndexChanged" Style="height: 20px;">
            </asp:DropDownList>
            &nbsp;&nbsp;
                <asp:Label ID="lbID" runat="server" Font-Size="12" Text="管理员可查询他人，请输入工号："></asp:Label>
            <asp:TextBox ID="tbID" runat="server" ToolTip="请输入要查询的工号" Style="height: 20px;"></asp:TextBox>
            <asp:Button ID="btnQuery" runat="server" Text="查 询" OnClick="btnQuery_Click" CssClass="pbtn" />
            <asp:Button ID="btnPrint" runat="server" Text="打 印" CssClass="prtBtn" OnClientClick="javascript: printdiv('divPrint')" />

        </div>

        <div id="divPerformance" runat="server" class="perfomancediv">
            <div id="divPrint">
                <div style="text-align: center; margin-bottom: 10px;">
                    <asp:Label ID="lbTitle" runat="server" Font-Size="18" ForeColor="blue" Text="2018年度个人业绩汇总"></asp:Label>
                </div>
                <asp:HiddenField ID="hdFlag" runat="server" />
                <asp:HiddenField ID="hdId" runat="server" />
                <asp:HiddenField ID="hdIdCard" runat="server" />
                <table class="mytable" cellspacing="0" cellpadding="0" border="1" bordercolor="#cad9ea">
                    <tr style="border-top: 2px solid #000000;">
                        <th style="border-left: 2px solid #000000; border-top: 2px solid #000000;">姓名</th>
                        <td style="border-top: 2px solid #000000;">
                            <asp:Label ID="lbName" runat="server" Text="张老师"></asp:Label></td>
                        <th style="border-top: 2px solid #000000;">工号</th>
                        <td colspan="2" style="border-top: 2px solid #000000;">
                            <asp:Label ID="lbIDCard" runat="server" Text="00001"></asp:Label>
                        </td>
                        <th rowspan="2" style="border-left: 2px solid #000000; border-top: 2px solid #000000;">总金额
                        <br />
                            （人头+课时+业绩点合计）
                        </th>
                        <th rowspan="2" style="border-left: 2px solid #000000; border-right: 2px solid #000000; border-top: 2px solid #000000;">月核拨</th>
                    </tr>

                    <tr>
                        <th style="border-left: 2px solid #000000;">职称</th>
                        <td>
                            <asp:Label ID="lbProf" runat="server" Text="教授"></asp:Label>
                        </td>
                        <th>所属部门</th>
                        <td colspan="2">
                            <asp:Label ID="lbDept" runat="server" Text="英语系"></asp:Label>
                        </td>
                    </tr>

                    <tr>
                        <th colspan="3" style="height: 35px; border-top: 2px solid #000000; border-left: 2px solid #000000;">人头费
                        </th>
                        <th colspan="2" style="border-top: 2px solid #000000;">
                            <asp:Label ID="lbOne" runat="server" Text="尚未公布" ToolTip="每个教师的职称绩效值"></asp:Label>
                        </th>

                        <th rowspan="37" style="border-left: 2px solid #000000; border-bottom: 2px solid #000000;">
                            <asp:Label ID="lbzong" runat="server" Text="尚未公布"></asp:Label>
                        </th>
                        <th rowspan="37" style="border-left: 2px solid #000000; border-right: 2px solid #000000; border-bottom: 2px solid #000000;">
                            <asp:Label ID="lbyhb" runat="server" Text="尚未公布"></asp:Label>
                        </th>

                    </tr>

                    <tr>
                        <th colspan="3" style="border-top: 2px solid #000000; border-left: 2px solid #000000;">教学课时</th>
                        <th colspan="2" style="border-top: 2px solid #000000;">课时总金额</th>
                    </tr>
                    <tr>

                        <th rowspan="7" style="border-left: 2px solid #000000;">本科教学课时</th>
                        <th>公外计划总学时</th>
                        <td>
                            <asp:Label ID="lbgwjh_bk" runat="server" Text="0.00"></asp:Label>
                            &nbsp;<a href="#" onclick="OpenFullSizeWindow('http://www.fsc.neu.edu.cn/Performance/SiteAssets/images/ComputeRules/本科公外计划总学时.jpg','本科教学公外计划总学时当量计算规则','');return false;" title="本科教学公外计划总学时当量计算规则">❉</a>
                        </td>
                        <th rowspan="17" colspan="2" style="border-bottom: 2px solid #000000;">
                            <asp:Label ID="lbkszje" runat="server" Text="尚未公布"></asp:Label>
                        </th>
                    </tr>
                    <tr>
                        <th>专业计划总学时</th>
                        <td>
                            <asp:Label ID="lbzyjh_bk" runat="server" Text="0.00"></asp:Label>
                            &nbsp;<a href="#" onclick="OpenFullSizeWindow('http://www.fsc.neu.edu.cn/Performance/SiteAssets/images/ComputeRules/本科专业计划总学时.jpg','本科教学专业计划总学时当量计算规则','');return false;" title="本科教学专业计划总学时当量计算规则">❉</a>
                        </td>
                    </tr>
                    <tr>
                        <th>公外折算总学时</th>
                        <td>
                            <asp:Label ID="lbgwzs_bk" runat="server" Text="0.00"></asp:Label>
                            &nbsp;<a href="#" onclick="OpenFullSizeWindow('http://www.fsc.neu.edu.cn/Performance/SiteAssets/images/ComputeRules/本科公外折算总学时.jpg','本科公外折算总学时当量计算规则','');return false;" title="本科公外折算总学时当量计算规则">❉</a>
                        </td>
                    </tr>
                    <tr>
                        <th>专业折算总学时</th>
                        <td>
                            <asp:Label ID="lbzyzs_bk" runat="server" Text="0.00"></asp:Label>
                            &nbsp;<a href="#" onclick="OpenFullSizeWindow('http://www.fsc.neu.edu.cn/Performance/SiteAssets/images/ComputeRules/本科专业折算总学时.jpg','本科专业折算总学时当量计算规则','');return false;" title="本科专业折算总学时当量计算规则">❉</a>

                        </td>
                    </tr>
                    <tr>
                        <th>指导社会实践</th>
                        <td>
                            <asp:Label ID="lbzdshsj" runat="server" Text="0.00"></asp:Label>
                            &nbsp;<a href="#" onclick="OpenFullSizeWindow('http://www.fsc.neu.edu.cn/Performance/SiteAssets/images/ComputeRules/指导社会实践.jpg','指导社会实践当量计算规则','');return false;" title="指导社会实践当量计算规则">❉</a>
                        </td>
                    </tr>
                    <tr>
                        <th>指导毕业论文</th>
                        <td>
                            <asp:Label ID="lbzdbylw_bk" runat="server" Text="0.00"></asp:Label>
                            &nbsp;<a href="#" onclick="OpenFullSizeWindow('http://www.fsc.neu.edu.cn/Performance/SiteAssets/images/ComputeRules/本科指导毕业论文.jpg','本科指导毕业论文当量计算规则','');return false;" title="本科指导毕业论文当量计算规则">❉</a>
                        </td>
                    </tr>
                    <tr>
                        <th>新开课</th>
                        <td>
                            <asp:Label ID="lbxkk_bk" runat="server" Text="0.00"></asp:Label>
                            &nbsp;<a href="#" onclick="OpenFullSizeWindow('http://www.fsc.neu.edu.cn/Performance/SiteAssets/images/ComputeRules/本科新开课.jpg','本科新开课当量计算规则','');return false;" title="本科新开课当量计算规则">❉</a>
                        </td>
                    </tr>

                    <tr>
                        <th rowspan="7" style="border-left: 2px solid #000000;">研究生教学课时</th>
                        <th>公外计划总学时</th>
                        <td>
                            <asp:Label ID="lbgwjh_yjs" runat="server" Text="0.00"></asp:Label>
                            &nbsp;<a href="#" onclick="OpenFullSizeWindow('http://www.fsc.neu.edu.cn/Performance/SiteAssets/images/ComputeRules/研究生公外计划总学时.jpg','研究生公外计划总学时当量计算规则','');return false;" title="研究生公外计划总学时当量计算规则">❉</a>
                        </td>
                    </tr>
                    <tr>
                        <th>专业计划总学时</th>
                        <td>
                            <asp:Label ID="lbzyjh_yjs" runat="server" Text="0.00"></asp:Label>
                            &nbsp;<a href="#" onclick="OpenFullSizeWindow('http://www.fsc.neu.edu.cn/Performance/SiteAssets/images/ComputeRules/研究生专业计划总学时.jpg','研究生专业计划总学时当量计算规则','');return false;" title="研究生专业计划学时当量计算规则">❉</a>
                        </td>
                    </tr>
                    <tr>
                        <th>公外折算总学时</th>
                        <td>
                            <asp:Label ID="lbgwzs_yjs" runat="server" Text="0.00"></asp:Label>
                            &nbsp;<a href="#" onclick="OpenFullSizeWindow('http://www.fsc.neu.edu.cn/Performance/SiteAssets/images/ComputeRules/研究生公外折算总学时.jpg','研究生公外折算总学时当量计算规则','');return false;" title="研究生公外折算总学时当量计算规则">❉</a>
                        </td>
                    </tr>
                    <tr>
                        <th>专业折算总学时</th>
                        <td>
                            <asp:Label ID="lbzyzs_yjs" runat="server" Text="0.00"></asp:Label>
                            &nbsp;<a href="#" onclick="OpenFullSizeWindow('http://www.fsc.neu.edu.cn/Performance/SiteAssets/images/ComputeRules/研究生专业折算总学时.jpg','研究生专业折算总学时当量计算规则','');return false;" title="研究生专业折算总学时当量计算规则">❉</a>
                        </td>
                    </tr>
                    <tr>
                        <th>指导研究生</th>
                        <td>
                            <asp:Label ID="lbzdyjs" runat="server" Text="0.00"></asp:Label>
                            &nbsp;<a href="#" onclick="OpenFullSizeWindow('http://www.fsc.neu.edu.cn/Performance/SiteAssets/images/ComputeRules/指导研究生.jpg','指导研究生当量计算规则','');return false;" title="指导研究生当量计算规则">❉</a>
                        </td>
                    </tr>
                    <tr>
                        <th>指导毕业论文</th>
                        <td>
                            <asp:Label ID="lbzdbylw_yjs" runat="server" Text="0.00"></asp:Label>
                            &nbsp;<a href="#" onclick="OpenFullSizeWindow('http://www.fsc.neu.edu.cn/Performance/SiteAssets/images/ComputeRules/研究生指导毕业论文.jpg','研究生指导毕业论文当量计算规则','');return false;" title="研究生指导毕业论文当量计算规则">❉</a>
                        </td>
                    </tr>
                    <tr>
                        <th>研究生批卷</th>
                        <td>
                            <asp:Label ID="lbyjspj" runat="server" Text="0.00"></asp:Label>
                            &nbsp;<a href="#" onclick="OpenFullSizeWindow('http://www.fsc.neu.edu.cn/Performance/SiteAssets/images/ComputeRules/研究生批卷.jpg','研究生批卷当量计算规则','');return false;" title="研究生批卷当量计算规则">❉</a>
                        </td>
                    </tr>
                    <tr>
                        <th rowspan="3" style="border-left: 2px solid #000000; border-bottom: 2px solid #000000;">课时汇总</th>
                        <th>公外计划总学时<br />
                            （本科+研究生）</th>
                        <td>
                            <asp:Label ID="lbgwjh" runat="server" Text="0.00"></asp:Label>
                            &nbsp;<a href="#" onclick="OpenFullSizeWindow('http://www.fsc.neu.edu.cn/Performance/SiteAssets/images/ComputeRules/公外计划总学时.jpg','公外计划总学时当量计算规则','');return false;" title="公外计划总学时当量计算规则">❉</a>
                        </td>
                    </tr>
                    <tr>
                        <th>专业计划总学时<br />
                            （本科+研究生）</th>
                        <td>
                            <asp:Label ID="lbzyjh" runat="server" Text="0.00"></asp:Label>
                            &nbsp;<a href="#" onclick="OpenFullSizeWindow('http://www.fsc.neu.edu.cn/Performance/SiteAssets/images/ComputeRules/专业计划总学时.jpg','专业计划总学时当量计算规则','');return false;" title="专业计划总学时当量计算规则">❉</a>
                        </td>
                    </tr>
                    <tr>
                        <th style="border-bottom: 2px solid #000000;">计划外总学时<br />
                            （公外+专业）</th>
                        <td style="border-bottom: 2px solid #000000;">
                            <asp:Label ID="lbjhw" runat="server" Text="0.00"></asp:Label>
                            &nbsp;<a href="#" onclick="OpenFullSizeWindow('http://www.fsc.neu.edu.cn/Performance/SiteAssets/images/ComputeRules/计划外总学时.jpg','计划外总学时当量计算规则','');return false;" title="计划外总学时当量计算规则">❉</a>
                        </td>
                    </tr>
                    <tr>
                        <th colspan="3" style="border-left: 2px solid #000000;">业绩点</th>
                        <th>业绩点总金额</th>
                        <th>业绩点合计<br />
                            总金额</th>
                    </tr>
                    <tr>

                        <th rowspan="15" style="border-left: 2px solid #000000;">业绩点项</th>
                        <th>教学立项</th>
                        <td>
                            <asp:Label ID="lbjxlx" runat="server" Text="0.00"></asp:Label>
                            &nbsp;<a href="#" onclick="OpenFullSizeWindow('http://www.fsc.neu.edu.cn/Performance/SiteAssets/images/ComputeRules/教学立项.jpg','教学立项当量计算规则','');return false;" title="教学立项当量计算规则">❉</a>
                        </td>
                        <th rowspan="15">
                            <asp:Label ID="lbyjdz" runat="server" Text="尚未公布"></asp:Label>
                        </th>
                        <th rowspan="17" style="border-bottom: 2px solid #000000;">
                            <asp:Label ID="lbyjdhjz" runat="server" Text="尚未公布"></asp:Label>
                        </th>
                    </tr>
                    <tr>
                        <th>教材</th>
                        <td>
                            <asp:Label ID="lbjc" runat="server" Text="0.00"></asp:Label>
                            &nbsp;<a href="#" onclick="OpenFullSizeWindow('http://www.fsc.neu.edu.cn/Performance/SiteAssets/images/ComputeRules/教材.jpg','教材当量计算规则','');return false;" title="教材当量计算规则">❉</a>
                        </td>
                    </tr>
                    <tr>
                        <th>论文</th>
                        <td>
                            <asp:Label ID="lblw" runat="server" Text="0.00"></asp:Label>
                            &nbsp;<a href="#" onclick="OpenFullSizeWindow('http://www.fsc.neu.edu.cn/Performance/SiteAssets/images/ComputeRules/论文.jpg','论文当量计算规则','');return false;" title="论文当量计算规则">❉</a>
                        </td>
                    </tr>
                    <tr>
                        <th>专著</th>
                        <td>
                            <asp:Label ID="lbzz" runat="server" Text="0.00"></asp:Label>
                            &nbsp;<a href="#" onclick="OpenFullSizeWindow('http://www.fsc.neu.edu.cn/Performance/SiteAssets/images/ComputeRules/专著.jpg','专著当量计算规则','');return false;" title="专著当量计算规则">❉</a>
                        </td>
                    </tr>
                    <tr>
                        <th>译著</th>
                        <td>
                            <asp:Label ID="lbyz" runat="server" Text="0.00"></asp:Label>
                            &nbsp;<a href="#" onclick="OpenFullSizeWindow('http://www.fsc.neu.edu.cn/Performance/SiteAssets/images/ComputeRules/译著.jpg','译著当量计算规则','');return false;" title="译著当量计算规则">❉</a>
                        </td>
                    </tr>
                    <tr>
                        <th>教学获奖</th>
                        <td>
                            <asp:Label ID="lbjxhj" runat="server" Text="0.00"></asp:Label>
                            &nbsp;<a href="#" onclick="OpenFullSizeWindow('http://www.fsc.neu.edu.cn/Performance/SiteAssets/images/ComputeRules/教学获奖.jpg','教学获奖当量计算规则','');return false;" title="教学获奖当量计算规则">❉</a>
                        </td>
                    </tr>
                    <tr>
                        <th>教学竞赛</th>
                        <td>
                            <asp:Label ID="lbjxjs" runat="server" Text="0.00"></asp:Label>
                            &nbsp;<a href="#" onclick="OpenFullSizeWindow('http://www.fsc.neu.edu.cn/Performance/SiteAssets/images/ComputeRules/教学竞赛.jpg','教学竞赛当量计算规则','');return false;" title="教学竞赛当量计算规则">❉</a>
                        </td>
                    </tr>
                    <tr>
                        <th>科研立项</th>
                        <td>
                            <asp:Label ID="lbkylx" runat="server" Text="0.00"></asp:Label>
                            &nbsp;<a href="#" onclick="OpenFullSizeWindow('http://www.fsc.neu.edu.cn/Performance/SiteAssets/images/ComputeRules/科研立项.jpg','科研立项当量计算规则','');return false;" title="科研立项当量计算规则">❉</a>
                        </td>
                    </tr>
                    <tr>
                        <th>科研成果</th>
                        <td>
                            <asp:Label ID="lbkycg" runat="server" Text="0.00"></asp:Label>
                            &nbsp;<a href="#" onclick="OpenFullSizeWindow('http://www.fsc.neu.edu.cn/Performance/SiteAssets/images/ComputeRules/科研成果.jpg','科研成果当量计算规则','');return false;" title="科研成果当量计算规则">❉</a>
                        </td>
                    </tr>
                    <tr>
                        <th>行政职务</th>
                        <td>
                            <asp:Label ID="lbxzzw" runat="server" Text="0.00"></asp:Label>
                            &nbsp;<a href="#" onclick="OpenFullSizeWindow('http://www.fsc.neu.edu.cn/Performance/SiteAssets/images/ComputeRules/行政职务.jpg','行政职务当量计算规则','');return false;" title="行政职务当量计算规则">❉</a>
                        </td>
                    </tr>
                    <tr>
                        <th>学科建设</th>
                        <td>
                            <asp:Label ID="lbxkjs" runat="server" Text="0.00"></asp:Label>
                            &nbsp;<a href="#" onclick="OpenFullSizeWindow('http://www.fsc.neu.edu.cn/Performance/SiteAssets/images/ComputeRules/学科建设.jpg','学科建设当量计算规则','');return false;" title="学科建设当量计算规则">❉</a>
                        </td>
                    </tr>
                    <tr>
                        <th>人才引进</th>
                        <td>
                            <asp:Label ID="lbrcyj" runat="server" Text="0.00"></asp:Label>
                            &nbsp;<a href="#" onclick="OpenFullSizeWindow('http://www.fsc.neu.edu.cn/Performance/SiteAssets/images/ComputeRules/人才引进.jpg','人才引进当量计算规则','');return false;" title="人才引进当量计算规则">❉</a>
                        </td>
                    </tr>
                    <tr>
                        <th>学术兼职</th>
                        <td>
                            <asp:Label ID="lbxsjz" runat="server" Text="0.00"></asp:Label>
                            &nbsp;<a href="#" onclick="OpenFullSizeWindow('http://www.fsc.neu.edu.cn/Performance/SiteAssets/images/ComputeRules/学术兼职.jpg','学术兼职当量计算规则','');return false;" title="学术兼职当量计算规则">❉</a>
                        </td>
                    </tr>
                    <tr>
                        <th>加分</th>
                        <td>
                            <asp:Label ID="lbjiafen" runat="server" Text="0.00"></asp:Label>
                            &nbsp;<a href="#" onclick="OpenFullSizeWindow('http://www.fsc.neu.edu.cn/Performance/SiteAssets/images/ComputeRules/加分.jpg','加分当量计算规则','');return false;" title="加分当量计算规则">❉</a>
                        </td>
                    </tr>
                    <tr>
                        <th>减分</th>
                        <td>
                            <asp:Label ID="lbjianfen" runat="server" Text="0.00"></asp:Label>
                            &nbsp;<a href="#" onclick="OpenFullSizeWindow('http://www.fsc.neu.edu.cn/Performance/SiteAssets/images/ComputeRules/减分.jpg','减分当量计算规则','');return false;" title="减分当量计算规则">❉</a>
                        </td>
                    </tr>
                    <tr>
                        <th rowspan="2" style="border-left: 2px solid #000000; border-bottom: 2px solid #000000;">业绩点金额调整</th>
                        <th>增加</th>
                        <td>助教补贴</td>
                        <th>
                            <asp:Label ID="lbzjbt" runat="server" Text="尚未公布"></asp:Label></th>
                    </tr>
                    <tr>

                        <th style="border-bottom: 2px solid #000000;">扣减</th>

                        <td style="border-bottom: 2px solid #000000;">缺勤</td>

                        <th style="border-bottom: 2px solid #000000;">
                            <asp:Label ID="lbqueqin" runat="server" Text="尚未公布"></asp:Label>
                        </th>

                    </tr>


                </table>
            </div>

            <div id="pStatus" runat="server" style="text-align: left; color: red; line-height: 25px;">
                Tips:点击业绩当量值后面的 ❉ 号，可以查看计算规则与说明
            </div>

            <div class="perfomancediv" id="GPACheck" runat="server" visible="false">
                <asp:Button ID="btnGPANoPass" runat="server" Text="绩点数有异议,请点击后与院办联系" Width="240px" OnClick="btnGPANoPass_Click" CssClass="pbtn" />&nbsp;&nbsp;
                        <asp:Button ID="btnGPAPass" runat="server" Text="绩点无异议，提交确认"  Width="240px" OnClick="btnGPAPass_Click" CssClass="pbtn" />

            </div>
            <div class="perfomancediv" id="MoneyCheck" runat="server" visible="false">
                <asp:Button ID="btnMoneyNoPass" runat="server" Text="业绩金额有异议,请点击后与院办联系" CssClass="pbtn"  Width="240px" OnClick="btnMoneyNoPass_Click" />&nbsp;&nbsp;
                <asp:Button ID="btnMoneyPass" runat="server" Text="业绩金额无异议，提交确认" CssClass="pbtn"  Width="240px" OnClick="btnMoneyPass_Click" />
            </div>

        </div>

        <div id="divErr" runat="server" style="color: red; font-size: 14px; margin: 10px;"></div>



</asp:Content>

<asp:Content ID="PageTitle" ContentPlaceHolderID="PlaceHolderPageTitle" runat="server">
个人业绩汇总公示
</asp:Content>

<asp:Content ID="PageTitleInTitleArea" ContentPlaceHolderID="PlaceHolderPageTitleInTitleArea" runat="server" >
个人业绩汇总公示
</asp:Content>
