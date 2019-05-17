<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Comments.aspx.cs" Inherits="WorkEvaluate.Layouts.WorkEvaluate.Comments" %>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<title>评分</title>
    <script src ="MediaPlayer.js" type ="text/jscript"></script>
     <script src="Validate.js" type="text/jscript"></script>   
       <script type="text/javascript">
           function txtdiva() {
               document.getElementById("txtDiv").style.display = "";
               document.getElementById('btnSubmitShow').style.display = '';
               document.getElementById('inputbutton').style.display = 'none';

           }
           function YiComments() {
               document.getElementById('inputbutton').style.display = 'none';
           }
           function ValidComments() {
               var txt = document.getElementById('<%=txtComments.ClientID%>');
          if (txt.value.length == 0) {
              alert("评语不能为空！");
              txt.select();
              return false;
          }
          return true
      }
      function rate(obj, oEvent) {

          // 图片地址设置
          var imgSrc = 'images/star_gray.gif'; //没有填色的星星
          var imgSrc_2 = 'images/star_red.gif'; //打分后有颜色的星星

          if (obj.rateFlag) return;
          var e = oEvent || window.event;
          var target = e.target || e.srcElement;
          var imgArray = obj.getElementsByTagName("img");
          for (var i = 0; i < imgArray.length; i++) {
              imgArray[i]._num = i;
              imgArray[i].onclick = function () {
                  if (obj.rateFlag) return;
                  obj.rateFlag = true;
                  document.getElementById('HiddenField2').value = this._num + 1;
              };
          }
          if (target.tagName == "IMG") {
              for (var j = 0; j < imgArray.length; j++) {
                  if (j <= target._num) {
                      imgArray[j].src = imgSrc_2;
                  } else {
                      imgArray[j].src = imgSrc;
                  }
              }
          } else {
              for (var k = 0; k < imgArray.length; k++) {
                  imgArray[k].src = imgSrc;
              }
          }
      }

    </script>
    <style type="text/css">
        #showDiv{ margin:0;
                  text-align:left; background-color:#fff; 
                  padding:10px 20px 5px;}
     .f{ color:#FFFFFF; text-align:center; }
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
 
    <script>
        $(document).ready(function () {

           
            /*隐藏*/
            function ml_close_demo() {
                $('.float-news').animate({
                    right: '-450px'
                }, 300, function () {
                    $('.float-open').delay(50).animate({
                        right: '-2px'
                    }, 300);
                });
            }
            function ml_open_demo() {
                $('.float-open').animate({
                    right: '-70px'
                }, 100, function () {
                    $('.float-news').delay(50).animate({
                        right: '0px'
                    }, 300);
                });
            }

            $('.float-close').click(function () {
                ml_close_demo();
                return false;
            });
            $('.open-btn').click(function () {
                ml_open_demo();
                return false;
            });

            setTimeout(function () { ml_close_demo() }, 1000);
        })
        /*滑入弹出下拉*/
        function onft1() {
            document.getElementById('HiddenDiv1').style.display = 'inline';

        }
        function onbt1() {
            document.getElementById('HiddenDiv1').style.display = 'none';

        }
        function onft2() {
            document.getElementById('HiddenDiv2').style.display = 'inline';

        }
        function onbt2() {
            document.getElementById('HiddenDiv2').style.display = 'none';

        }
        function onft3() {
            document.getElementById('HiddenDiv3').style.display = 'inline';

        }
        function onbt3() {
            document.getElementById('HiddenDiv3').style.display = 'none';

        }
        function onft4() {
            document.getElementById('HiddenDiv4').style.display = 'inline';

        }
        function onbt4() {
            document.getElementById('HiddenDiv4').style.display = 'none';

        }
        function onft5() {
            document.getElementById('HiddenDiv5').style.display = 'inline';

        }
        function onbt5() {
            document.getElementById('HiddenDiv5').style.display = 'none';

        }
        /* 关闭*/

        window.onbeforeunload = function () {
            var n = window.event.screenX - window.screenLeft;
            var b = n > document.documentElement.scrollWidth - 20;
            if (b && window.event.clientY < 0 || window.event.altKey)  // 是关闭而非刷新 
            {
                document.getElementById('btnClose').click();
            }
        }
        document.onkeydown = function () {
            if (event.keyCode == 116)//屏蔽 F5 刷新键
            {
                event.keyCode = 0;
                event.returnValue = false;
            }
        }
    </script>
    <style type="text/css">     
        .txtcss
        {
            width: 50px;
            border: 1px #bebee1 solid;
            height: 25px;
            vertical-align: middle;
            line-height: 25px;
            padding: 0 5px;
            font-size:16px;
            color: Black;
        }
      
       .spanfen{ color:#999;}
.divpoint img{ margin:0px 0 0 5px;}
 .lblcss{padding:5;margin:5px;line-height:1.5;text-indent:20px; color:#555;}
        </style>
       <link rel="stylesheet" type="text/css" href="css/page.css"/>
</head>
<body style="background-color:#f3f3f3">
    <form id="form1" runat="server">
       <div id="divWorksShow" runat ="server" class="comments-Main" >
            <div class="comments-Content">
       <!--作品名称 -->
               <div class="comments-Workstitle" >
                   <h1>作品：<asp:Label ID="lblWorksName" runat="server" Text=""></asp:Label></h1>
               </div>
        <!--作品编码 -->
               <div class="comments-WroksCode">
                   <h3>作品编号：<asp:Label ID="lblWorksCode" runat="server" Text=""></asp:Label></h3></div> 
       <!--作品类别 -->
               <div class="comments-WorksType">
                   <h3>作品组别：<asp:Label ID="lblWorksType" runat="server"  Text=""></asp:Label></h3></div>
       <!--  作品展示 -->
               <div class="comments-DocView">
                   <div class="comments-DocView-title"></div>
                   <div runat="server" id="divDocView" class="comments-DocView-inner"></div>
                   <div runat="server" id="divDovVideo" class="comments-DocView-inner" ></div>
               </div>
        <!--外部资源 -->
               <div class="comments-DemoURL">
                   <h3>外部资源：<span class="comments-DemoURL-spanUrl"><asp:Label ID="lblDemoURL" runat="server"  Text=""></asp:Label></span></h3></div>
       <!--作品简介 -->
               <div class="comments-SubmitProfile">
                  <div  class="comments-SubmitProfile-Titlebg">
                      <div class="comments-SubmitProfile-Title" ><h2>作品简介</h2></div>
                  </div>
                  <div class="comments-SubmitProfile-content"><p><asp:Label ID="lblSubmitProfile" runat="server"  Text="" ></asp:Label></p>
                      <div id="divWorksFile" runat="server"  class="comments-SubmitProfile-divWorksFile"></div>
                  </div>             
               </div>
      <!--设计思路 -->       
              <div class="comments-DesignIdeas">
                   <div  class="comments-DesignIdeas-Titlebg">
                     <div class="comments-DesignIdeas-Title"><h2>设计思路</h2></div>
                   </div>
                   <div class="comments-DesignIdeas-content" ><p runat="server" id="divDesignIdeas"></p></div>
              </div>
      <!--创意特色 -->
              <div class="comments-KeyPoints">
                  <div class="comments-KeyPoints-Titlebg">
                   <div class="comments-KeyPoints-Title"><h2>创意特色</h2></div>
                  </div>
                   <div class="comments-KeyPoints-content" ><p id="divKeyPoints"  runat="server"></p></div>
              </div>
      <!-- 讲解视频 -->
               <div class="comments-divViewShow">
                   <div class="comments-divViewShow-title">讲解视频</div>
                   <div runat="server" id="divViewShow" class="comments-divViewShow-Show"></div>
               </div>
      <!-- 作品点评 --> 
           <div style="display:none " runat="server"  id ="divPublicComments" >
           <table cellpadding="2" cellspacing="0" style="font-size: 14px; width:800px; text-align:left;margin-left:100px">

            <tr><td class="titlebga" style="background-color:#6e5328" id="a07"> <div class="titlebgaa">作品点评</div></td></tr>
            <tr><td  height="8px"></td></tr>
             <tr><td style=" padding-left:20; height:20px; border-bottom:1px dotted #555">
                  <div>
                    <span  style="color:#777">当前评分：</span>
                    <span runat="server" id="divScore"><img src="images/star_red.gif" /><img src="images/star_red.gif" /><img src="images/star_gray.gif" /><img 

src="images/star_gray.gif" /><img src="images/star_gray.gif" /></span>&nbsp;
                    <span style="color:#1a66b3">已有<asp:Label  id="lblPersons" runat="server" Text="1" ></asp:Label>人评论</span></div></td></tr>
            <tr><td  height="5px"></td></tr>
            
            <tr><td style="padding-left:20px">
                 
      <asp:GridView ID="gvComments" runat="server" AutoGenerateColumns="False" AllowPaging="True"
            BorderStyle="None"  CssClass="grouptd"  GridLines="None" 
            AlternatingRowStyle-HorizontalAlign="Left" HorizontalAlign="Left">
            <Columns>
                <asp:BoundField DataField="Name" HeaderText="点评者" >
                    <ItemStyle Width="100px" Wrap="False" CssClass="liuyan"/>
                </asp:BoundField>
                <asp:BoundField DataField="Comments" HeaderText="评论内容">
                    <ItemStyle Width="500px" CssClass="neirong"/>
                </asp:BoundField>
                <asp:BoundField DataField="Score" HeaderText="评分" >
                    <ItemStyle Width="60px" Wrap="False" CssClass="fenshu"/>
                </asp:BoundField>
                <asp:BoundField DataField="Created" DataFormatString="{0:yyyy-MM-dd}" 
                    HeaderText="点评时间">
                    <ItemStyle Width="100px" Wrap="False" />
                </asp:BoundField>
            </Columns>
            <PagerStyle HorizontalAlign="Center" />
            <HeaderStyle BackColor="#494B4C" ForeColor="#E6E6E6" HorizontalAlign="Left" />

<AlternatingRowStyle HorizontalAlign="Left" BackColor="#CCFFFF"></AlternatingRowStyle>
        </asp:GridView>
                 
                </td></tr>
            <tr><td  height="5px"></td></tr>
            <tr><td style=" padding-left:20;color:#555">
                 <div id="txtDiv" style="display:none">
                   <div style="margin-bottom:5px"><span  style="color:#777">评分：</span>
                    <span onmouseover="rate(this,event)"  ><img src="images/star_gray.gif" /><img src="images/star_gray.gif" /><img src="images/star_gray.gif" /><img 

src="images/star_gray.gif" /><img src="images/star_gray.gif" /></span>
                    </div>
                   <div>评语：<br />
                       <asp:TextBox ID="txtComments" runat="server" Height="150px" Width="500px" 
                           TextMode="MultiLine" ></asp:TextBox></div>
                 </div>
                </td></tr>
                <tr><td  height="5px"></td></tr>
            <tr>
                <td style=" padding-left:20;">
                    <input type="button" value="我要点评" class="buttoncss" onclick="txtdiva()" id="inputbutton"/>
                    <asp:Button ID="btnSubmitShow"  OnClientClick="return ValidComments()" runat="server" Text="发表" CssClass="buttoncssa" style="display:none"  

BorderStyle="None"/>
                    
                    <asp:HiddenField ID="HiddenField2" Value="0"  runat="server" />
                    
                 </td>
            </tr>
               <tr><td  height="30px"></td></tr>
        </table> 
          </div>    
          </div>
       </div>
        <!-- divWorksShow end-->
<!--点评 -->
        <div runat="server" id="divWorksScore">
    <div  class="float-open" id="float-open" style="right:-2px;background-color:#61a4da"><a class="open-btn" href="javascript:void(0);"></a></div>
    <div class="float-news" id="float-news" style="right:-450px;"> 
	  <a class="float-close" href="javascript:void(0);"></a>  
	  <div class="newslist">
      <div   runat="server" id="divContent" style="font-size: 14px; color: #494b4c; margin:0; padding:0;">
       <!--评分模型 -->
       <!--  
          <div class="comments-scores-model">
              <h6 class="comments-scores-modelTitle">设计内容（20分）</h6>
              <asp:TextBox ID="TeamNametxt" runat="server" Height="25px" CssClass="txtcss" onfocus="onft1()"
                        onblur="onbt1()"></asp:TextBox><span></span>
              <div class="comments-scores-divHidden" id="HiddenDiv1">
                  具体内容
              </div> 
       </div>
          -->
    </div>
        <div class="comments-scores-ModelPoint">
           <h6 class="comments-scores-modelTitle">评语（30字以上）</h6>
        </div>
        <asp:TextBox ID ="txtScorePingYu"  TextMode="MultiLine"  runat="server" Height="80px" Width="280px" BorderColor="#dbdbdb" BorderStyle="Solid" style=" margin-left:5px"></asp:TextBox> 
       <asp:ScriptManager ID="ScriptManager1" runat="server">
       </asp:ScriptManager>
       <!-- 小计 -->
       <div class="divpoint" style="text-align:left;display:block;margin:15px 0;margin-left:5px">
           <span class="comments-deifena" >小计:</span>         
           <asp:Label ID="lblTotalScore" runat="server" class="comments-scoresNamber" Text="0"></asp:Label>
          <span class="comments-deifena">分(满分90)
           </span>

       </div>
           <asp:UpdatePanel ID="UpdatePanel1" runat="server">
           <ContentTemplate>
       <div class="divpoint" style="text-align:left;margin-left:5px">
           <asp:Button ID="btnSubmit" runat="server" Text="提交" CssClass="ButtonBbg" title="提交之后在评分期内可以修改！" OnClientClick="javascript:return checkSubmitSelt()" BorderWidth="0"/>
       </div><div style="display:none" ><asp:Button ID="btnClose"  runat="server" Text="退 出"   Width="0"  CssClass="buttoncss"  BorderStyle="None" />
       </div>
           </ContentTemplate> 
           </asp:UpdatePanel>
     <asp:HiddenField ID="HiddenField1" runat="server" Value="-1" />
    </div> 
   </div>
   </div>
         </form>
    </body>
</html>