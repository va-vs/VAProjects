<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Import Namespace="Microsoft.SharePoint.ApplicationPages" %>
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="neulc.aspx.cs" Inherits="FSCAppPages.Layouts.FSCAppPages.Corpus.neulc" DynamicMasterPageFile="~masterurl/default.master" EnableEventValidation="false" %>

<%@ Register Assembly="System.Web.DataVisualization, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" Namespace="System.Web.UI.DataVisualization.Charting" TagPrefix="asp" %>

<asp:Content ID="PageHead" ContentPlaceHolderID="PlaceHolderAdditionalPageHead" runat="server">
	<link rel="stylesheet" href="http://202.118.11.33/NEU_EC/SiteAssets/css/stylelc.css" type="text/css" charset="utf-8"  />
	<link rel="stylesheet" href="../css/tab.css"  type="text/css" charset="utf-8" />

	<style type="text/css">
             .divBlock{
            width:100%;
            clear:both;
            padding:5px;
            border:1px solid #808080;
            display:inline-block;
        }

             .imgul {
	list-style: none; /* 去掉ul前面的符号 */
	margin: 0px; /* 与外界元素的距离为0 */
	padding: 0px; /* 与内部元素的距离为0 */
	width: auto; /* 宽度根据元素内容调整 */
}

	.imgul li {
		float: left; /* 向左漂移，将竖排变为横排 */
	}
    .imgul li input{
        margin-left:10px;
        margin-right:10px;
    }

		.imgul li a, .imgul li a:visited {
			display: block; /* 此元素将显示为块级元素，此元素前后会带有换行符 */
			line-height: 1.35em;
			padding: 4px 20px;
			text-decoration: none;
			white-space: nowrap;
		}

			.imgul li a:hover {
				background-color: #eeeeee;
				text-decoration: none;
			}
		.qfld {
			border: none;
			line-height: 25px;
		}

		.qfld fieldset {
			border: 1px solid #808080;
			margin: 5px;
			line-height: 30px;
		}

		.qfld fieldset legend {
			text-align: left;
			font-weight: bold;
			font-size: 15px;
		}
        .qfld fieldset legend input{
			margin-left:10px;
		}
        .qfld fieldset legend span{
			width:100px;
		}
        .qfld fieldset legend label{
			font-weight:normal;
            font-size:10px;
		}
        .qfld table{
            width:100%;
            margin:5px;
            border:none;
        }
        .qfld table tr{
            padding:5px;
        }
        .qfld table tr th{
            padding:5px;
            text-align:right;
        }
        .qfld table tr td{
            padding:5px;
            text-align:left;
        }

		.cb td {
			width: 160px;
		}

		.cb label {
			height: 25px;
			width: 160px;
			display: inline-block;
			vertical-align: middle;
			margin-top: -4px;
		}

		.cb label:before {
			display: inline-block;
			background: #fff;
			vertical-align: middle;
			-webkit-transition: background ease-in .5s;
			-o-transition: background ease-in .5s;
			transition: background ease-in .5s;
		}

		.cb label:after {
			display: inline-block;
			background: rgb(54, 85, 5);
			vertical-align: middle;
			-webkit-transition: background ease-in .5s;
			-o-transition: background ease-in .5s;
			transition: background ease-in .5s;
		}
	 .flexbox_div {
            vertical-align: middle;
            margin:5px;
            text-align:left;
        }
        .flexbox_text{
            height:20px;
            text-align:center;
        }
        .flexbox_asub{
            height:16px;
            width:16px;
            text-align: center;
            color:black;
            text-decoration:none;
            border:1px solid #808080;
            background-color:#f0f0f0;
            border-radius: 50%;
            -moz-border-radius: 50%;
            -webkit-border-radius: 50%;
            vertical-align:central;
            padding-left:6px;
            padding-right:6px;
            padding-bottom:2px;
            font-weight:bold;
        }
        .flexbox_asub:visited {text-decoration:none;}
        .flexbox_asub:hover{
            cursor:pointer;
            text-decoration:none;
            background-color:#808080;
            color:#ffffff;
        }
        .flexbox_asub:active {color:#ba2636;}
        .flexbox_a{
            height:16px;
            width:16px;
            text-align: center;
            color:black;
            text-decoration:none;
            border:1px solid #808080;
            background-color:#f0f0f0;
            border-radius: 50%;
            -moz-border-radius: 50%;
            -webkit-border-radius: 50%;
            vertical-align:central;
            padding-left:5px;
            padding-right:5px;
            padding-bottom:2px;
            font-weight:bold;
        }
        .flexbox_a:visited {text-decoration:none;}
        .flexbox_a:hover{
            cursor:pointer;
            text-decoration:none;
            background-color:#808080;
            color:#ffffff;
        }
         .flexbox_a:active{
            text-decoration:none;
        }


    </style>

     <script type="text/javascript">
         function add(txtId, diff, max) {
             var txt = document.getElementById(txtId);
             var a = parseInt(txt.value);
             a = a + diff;
             if (a > max) {
                 a = max;
             }
             txt.value = a;
         }

         function sub(txtId, diff, min) {
             var txt = document.getElementById(txtId);
             var a = parseInt(txt.value);
             a = a - diff;
             if (a < min) {
                 a = min;
             }
             txt.value = a;
         }
		</script>

	<script type="text/javascript">
		function Uncheck_All() {
			var cbl0 = document.getElementById("<%=cblGenre.ClientID%>");
		   var input = cbl0.getElementsByTagName("input");
		   for (var i = 0; i < input.length; i++) {
			   input[i].checked = false;
		   }
		   var cbl1 = document.getElementById("<%=cblLevel.ClientID%>");
		   var input = cbl1.getElementsByTagName("input");
		   for (var i = 0; i < input.length; i++) {
			   input[i].checked = false;
		   }
		   var cbl2 = document.getElementById("<%=cblTopic.ClientID%>");
			var input = cbl2.getElementsByTagName("input");
			for (var i = 0; i < input.length; i++) {
				input[i].checked = false;
			}
            document.getElementById("<%=divforCorpusResult.ClientID%>").style.display = "none";
			document.getElementById("<%=divNEULC.ClientID%>").style.display = "block";
		}

		function HighLightthis(val) {
			var setValue = val.getAttribute("data-Level");
			var obj = js("<%=divContext.ClientID%>");
			var oldClass = "";
			var newClass = "";
			for (i in obj) {
				oldClass = obj[i].className;
				var classArr = oldClass.trim().split(/\s+/);
				if (classArr.indexOf(setValue) > -1) {
					if (classArr.indexOf("highlight") > -1) { //已经是加颜色,本次操作将去掉颜色
						newClass = oldClass.replace("highlight", "");
						obj[i].classList.remove("highlight");
						document.getElementById("btnLight").setAttribute("data-i", "1");
						document.getElementById("btnLight").setAttribute("value", "HighLight");
					} else { //未颜色,本次操作将增加颜色
						newClass = oldClass + "highlight";
						obj[i].classList.add("highlight");
						document.getElementById("btnLight").setAttribute("data-i", "0");
						document.getElementById("btnLight").setAttribute("value", "Clear");
					}
				}
			}
		}

		function js(id) {
			return document.getElementById(id).getElementsByTagName("span");
		}

		function HighLightAll(val) {
			var setValue = val.getAttribute("data-i");
			var obj = js("<%=divContext.ClientID%>");
			if (setValue == 1) {
				val.setAttribute("data-i", "0");
				val.setAttribute("value", "Clear");
				for (i in obj) {
					obj[i].classList.add("highlight");
				}
			}
			else {
				val.setAttribute("data-i", "1");
				val.setAttribute("value", "HighLight");
				for (i in obj) {
					obj[i].classList.remove("highlight");
				}
			}
		}

		function upload(input) {
			var txt = document.getElementById("<%=txtcontent.ClientID%>");
			//支持chrome IE10
			if (window.FileReader) {
				var file = input.files[0];
				filename = file.name.split(".")[0];
				var reader = new FileReader();
				reader.onload = function () {
					console.log(this.result)
					//alert(this.result);
					txt.value = this.result;
					wordStatic(txt);
				}
				reader.readAsText(file);
			}
			//支持IE 7 8 9 10
			else if (typeof window.ActiveXObject != 'undefined') {
				var xmlDoc;
				xmlDoc = new ActiveXObject("Microsoft.XMLDOM");
				xmlDoc.async = false;
				xmlDoc.load(input.value);
				//alert(xmlDoc.xml);
				txt.value = xmlDoc.xml;
				wordStatic(txt);
			}
			//支持FF
			else if (document.implementation && document.implementation.createDocument) {
				var xmlDoc;
				xmlDoc = document.implementation.createDocument("", "", null);
				xmlDoc.async = false;
				xmlDoc.load(input.value);
				//alert(xmlDoc.xml);
				txt.value = xmlDoc.xml;
				wordStatic(txt);
			} else {
				//alert('error');
				txt.value = "文件导入失败，请确认你是导入的文本文件！";
			}
		}
		function wordStatic(input) {
			// 获取文本框对象
			var el = document.getElementById('wcount');
			var ll = document.getElementById('lcount');
			var mw = document.getElementById('mywords');
			if (el && input) {
				// 获取输入内容长度并更新到界面
				var value = input.value;
				// 替换中文字符为空格
				value = value.replace(/[\u4e00-\u9fa5]+/g, " ");
				// 将换行符，前后空格不计算为单词数
				value = value.replace(/\n|\r|^\s+|\s+$/gi, "");
				// 多个空格替换成一个空格
				value = value.replace(/\s+/gi, " ");
				// 更新计数
				var length = 0;
				var match = value.match(/\s/g);
				if (match) {
					length = match.length + 1;
				} else if (value) {
					length = 1;
				}
				mw.style.display = "";
				el.innerText = "Entered：" + length + " Words;";
				if (length <= 100000) {
					ll.innerHTML = "Remaining: " + (100000 - length) + " Words;";
				} else {
					ll.innerHTML = "exceeded: " + (length - 100000) + " Words;";
				}

			}
		}
	</script>

    <script type="text/javascript">
                                function checkOrNot(cblId, val) {
                                    for (var i = 0; i < document.getElementById(cblId).getElementsByTagName("input").length; i++) {
                                        document.getElementById(cblId + "_" + i).checked = val.checked;
                                    }
                                }
                            </script>
</asp:Content>

<asp:Content ID="Main" ContentPlaceHolderID="PlaceHolderMain" runat="server">
    <div class="divneulc">
        <%-- 快捷菜单 --%>
        <div id="divNav" class="divBlock">
            <ul class="imgul">
                <li>
                     <asp:Label ID="lbCorpus" runat="server" Text="NEULC" Font-Size="18" Font-Bold="true"></asp:Label>
                </li>
                <li>
                    <asp:ImageButton ID="ibtnInfo" runat="server" ImageUrl="../images/Info.png" ImageAlign="Middle" Height="20" Width="20"  PostBackUrl="#" ToolTip="About this Corpus"  Enabled="false"/>
                </li>
                <li>
                    <asp:ImageButton ID="ibtnDownload" runat="server" ImageUrl="../images/Download.png" ImageAlign="Middle" Height="20" Width="20"  PostBackUrl="#" ToolTip="DownLoad This Corpus" Enabled="false" />
                </li>
                <li>
                    <asp:ImageButton ID="ibtnUpload" runat="server" ImageUrl="../images/Upload.png" PostBackUrl="admin.aspx" ImageAlign="Middle" Height="20" Width="20" ToolTip="Upload a new Corpora" />

                </li>
                <li>
                    <asp:ImageButton ID="ibtnShare" runat="server" ImageUrl="../images/Share.png" PostBackUrl="#" ImageAlign="Middle" Height="20" Width="20" ToolTip="Share this Corpus" Enabled="false" />
                </li>
                <li>
                    <asp:ImageButton ID="ibtnGuide" runat="server" ImageUrl="../images/Guide.png" PostBackUrl="#" ImageAlign="Middle" Height="20" Width="20" ToolTip="Get Help" Enabled="false" />
                </li>
            </ul>
        </div>
        <%-- 导航菜单 --%>
        <div id="divMenu" class="divBlock">
            <asp:Menu ID="muNeulc" runat="server" Orientation="Horizontal" DynamicHorizontalOffset="2" StaticDisplayLevels="1" CssClass="mnuTopMenu" Width="801px" StaticSubMenuIndent="10px" DisappearAfter="600">
                <Items>
                    <asp:MenuItem Text="Corpus" Value="Corpus"></asp:MenuItem>
                    <asp:MenuItem Text="Concordance" Value="Concordance"></asp:MenuItem>
                    <asp:MenuItem Text="Collocate" Value="Collocate"></asp:MenuItem>
                    <asp:MenuItem Text="WordList" Value="WordList"></asp:MenuItem>
                    <asp:MenuItem Text="Cluster" Value="Cluster"></asp:MenuItem>
                    <asp:MenuItem Text="Compare" Value="Compare"></asp:MenuItem>
                </Items>
                <StaticSelectedStyle Font-Bold="true" HorizontalPadding="10" VerticalPadding="4"  ForeColor="red" />
                <StaticMenuItemStyle HorizontalPadding="10" VerticalPadding="4" />
                <StaticHoverStyle ForeColor="red" />
                <DynamicMenuStyle HorizontalPadding="10" VerticalPadding="4" />
                <DynamicSelectedStyle BackColor="#808080" Font-Bold="true" ForeColor="red" />
                <DynamicMenuItemStyle BackColor="#ecf6ff" HorizontalPadding="18" VerticalPadding="4" />
                <DynamicHoverStyle ForeColor="red" />
            </asp:Menu>
        </div>
        <%-- 导航内容 --%>
        <div id="divViews" class="divBlock">
            <asp:MultiView ID="mvNeulc" runat="server">
                <%-- 检索 --%>
                <asp:View ID="vwCorpus" runat="server">
                    <%-- 语料库检索div --%>
                    <div id="divNEULC" runat="server">
                        <div class="qfld" style="font-size: 14px;">
                            Select Corpus you need according to:
                        </div>
                        <div class="qfld">

                            <fieldset id="fdsLevel">
                                <legend>
                                    <span> Level </span>
                                    <input id="cbforLevel" type="checkbox" onchange='checkOrNot("<%=cblLevel.ClientID%>", this)' />
                                    <label for="cbforLevel" style="font-weight:normal">Select All Levels</label>
                                </legend>
                                <asp:CheckBoxList ID="cblLevel" runat="server" RepeatDirection="Horizontal"
                                    RepeatLayout="Flow" CssClass="cb" RepeatColumns="6">
                                    <asp:ListItem Value="1">L1(freshmen)</asp:ListItem>
                                    <asp:ListItem Value="2">L2(sophomores)</asp:ListItem>
                                    <asp:ListItem Value="3">L3(juniors)</asp:ListItem>
                                    <asp:ListItem Value="4">L4(seniors)</asp:ListItem>
                                </asp:CheckBoxList>
                            </fieldset>
                            <fieldset>
                                <legend>
                                    <span> Genre </span>
                                    <input id="cbforGenre" type="checkbox" onchange='checkOrNot("<%=cblGenre.ClientID%>", this)' />
                                    <label for="cbforGenre" style="font-weight:normal">Select All Genres</label>
                                </legend>
                                <asp:CheckBoxList ID="cblGenre" runat="server" RepeatDirection="Horizontal"
                                    RepeatLayout="Flow" CssClass="cb" RepeatColumns="6">
                                    <asp:ListItem Value="1">Argumentation</asp:ListItem>
                                    <asp:ListItem Value="2">Narration</asp:ListItem>
                                    <asp:ListItem Value="3">Exposition</asp:ListItem>
                                    <asp:ListItem Value="4">Applied Writing</asp:ListItem>
                                </asp:CheckBoxList>
                            </fieldset>
                            <fieldset>
                                <legend>
                                    <span> Topic </span>
                                    <input id="cbforTopic" type="checkbox" onchange='checkOrNot("<%=cblTopic.ClientID%>", this)' />
                                    <label for="cbforTopic" style="font-weight:normal">Select All Topics</label>
                                </legend>
                                <asp:CheckBoxList ID="cblTopic" runat="server" RepeatDirection="Horizontal"
                                    RepeatLayout="Flow" RepeatColumns="6" CssClass="cb">
                                    <asp:ListItem Value="1">Culture</asp:ListItem>
                                    <asp:ListItem Value="2">Science & Technology</asp:ListItem>
                                    <asp:ListItem Value="3">Humanity</asp:ListItem>
                                    <asp:ListItem Value="4">Society</asp:ListItem>
                                </asp:CheckBoxList>
                            </fieldset>
                        </div>

                        <div class="qfld" style="text-align: center; margin: 10px;">
                            <asp:Button ID="btnSubmitforCorpus" runat="server" Text="Submit" CssClass="outbtndiv-button" />
                        </div>
                    </div>

                    <div id="divNEUAC" class="qfld" runat="server" visible="false">
                            <fieldset>
                                <legend>
                                    <span> Year </span>
                                    <input id="cbforYear" type="checkbox" onchange='checkOrNot("<%=cblYear.ClientID%>", this)' />
                                    <label for="cbforYear" style="font-weight:normal">Select All Years</label>
                                </legend>
                                <asp:CheckBoxList ID="cblYear" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow" RepeatColumns="6" CssClass="cb">
                                    <asp:ListItem Value="1">2013</asp:ListItem>
                                    <asp:ListItem Value="2">2014</asp:ListItem>
                                </asp:CheckBoxList>
                            </fieldset>
                            <fieldset>
                                <legend>
                                    <span> Major </span>
                                    <input id="cbforMajor" type="checkbox" onchange='checkOrNot("<%=cblMajor.ClientID%>", this)' />
                                    <label for="cbforMajor" style="font-weight:normal">Select All Majors</label>
                                </legend>
                                <asp:CheckBoxList ID="cblMajor" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow" RepeatColumns="6" CssClass="cb">
                                    <asp:ListItem Value="1">Computer</asp:ListItem>
                                    <asp:ListItem Value="2">Material</asp:ListItem>
                                </asp:CheckBoxList>
                            </fieldset>
                            <fieldset id="fldJournals" runat="server">
                                <legend>
                                    <span> Journal </span>
                                    <input id="cbforJournal" type="checkbox" onchange='checkOrNot("<%=cblJournal.ClientID%>", this)'/>
                                    <label for="cbforJournal" style="font-weight:normal">Select All Journals</label>
                                </legend>
                                <asp:CheckBoxList ID="cblJournal" runat="server" >
                                    <asp:ListItem Value="1">JOURNAL OF ALLOYS AND COMPOUNDS</asp:ListItem>
                                    <asp:ListItem Value="2">MATERIALS SCIENCE AND ENGINEERING A-STRUCTURAL MATERIALS PROPERTIES MICROSTRUCTURE AND PROCESSING
</asp:ListItem>
                                </asp:CheckBoxList>
                            </fieldset>
                        <div class="qfld" style="text-align: center; margin: 10px;">
                            <asp:Button ID="btnSubmitforNEUAC" runat="server" Text="Submit" CssClass="outbtndiv-button" Enabled="false" />
                            <asp:Label ID="Label1" runat="server" Text="NEUAC暂不提供汇总计算..." ForeColor="red"></asp:Label>
                        </div>
                        </div>

                    <%-- 检索过后的语料库数据统计与分析 --%>
                    <div id="divforCorpusResult" runat="server" visible="false">
                        <div id="tab">
                            <h3 class="up" id="two1" onclick="setContentTab('two',1,3)">
                                <asp:Label ID="lb1Summary" runat="server" Text="Summary by Level"></asp:Label>
                            </h3>
                            <div class="block" id="con_two_1">
                                <asp:Table ID="tbforLevel" runat="server"></asp:Table>
                            </div>
                            <h3 id="two2" onclick="setContentTab('two',2,3)">
                                <asp:Label ID="lb2Summary" runat="server" Text="Summary by Topic"></asp:Label>
                            </h3>
                            <div id="con_two_2">
                                <asp:Table ID="tbforTopic" runat="server"></asp:Table>
                            </div>
                            <h3 id="two3" onclick="setContentTab('two',3,3)">
                                <asp:Label ID="lb3Summary" runat="server" Text="Summary by Genre"></asp:Label>
                            </h3>
                            <div id="con_two_3">
                                <asp:Table ID="tbforGenre" runat="server"></asp:Table>
                            </div>
                            <asp:Button ID="btnBacktoQuery" runat="server" Text="Back to Query" OnClientClick="Uncheck_All()" Width="140px" class="outbtndiv-button"/>
                        </div>

                        <script type="text/javascript">
                            function setContentTab(name, curr, n) {
                                for (i = 1; i <= n; i++) {
                                    var menu = document.getElementById(name + i);
                                    var cont = document.getElementById("con_" + name + "_" + i);
                                    menu.className = i == curr ? "up" : "";
                                    if (i == curr) {
                                        cont.style.display = "block";
                                        cont.className = "block";
                                    } else {
                                        cont.style.display = "none";
                                    }
                                }
                            }
                        </script>
                    </div>
                </asp:View>
                <%-- Concordance --%>
                <asp:View ID="vwConcordance" runat="server">
                    <div id="divConcordanceQuery" runat="server" class="qfld">
                        <fieldset>
                            <legend>Select Corpus you need according to:</legend>
                            <div class="flexbox_div">
                                Search for&nbsp;
                            <input type="text" id="txtKeyConcordance" size="60" placeholder="Type a query keyWord here" runat="server" class="input-text" />
                            </div>

                            <div class="flexbox_div">
                                Search for matches&nbsp;
                                <asp:DropDownList ID="ddlSentencePos" runat="server">
                                    <asp:ListItem Enabled="False" Value="0">at the start of</asp:ListItem>
                                    <asp:ListItem Selected="True" Value="1">anywhere in</asp:ListItem>
                                    <asp:ListItem Enabled="False" Value="2">at the end of</asp:ListItem>
                                </asp:DropDownList>&nbsp;
                            a sentence
                            </div>
                            <div class="flexbox_div">
                                Display each match at&nbsp;
                                <asp:DropDownList ID="ddlMatchPos" runat="server">
                                    <asp:ListItem Value="0">start</asp:ListItem>
                                    <asp:ListItem Selected="True" Value="1">middle</asp:ListItem>
                                    <asp:ListItem Value="2">end</asp:ListItem>
                                </asp:DropDownList>&nbsp;
                                of a context of&nbsp;
                                <a href="#" onclick='sub("<%=txtCDChars.ClientID%>",1,5)' class="flexbox_asub">-</a>
                                <input type="text" id="txtCDChars" name="contextsize" size="4" value="10" runat="server" class="flexbox_text" />
                                <a href="#" onclick='add("<%=txtCDChars.ClientID%>",1,50)' class="flexbox_a">+</a>&nbsp;
                                <asp:DropDownList ID="ddlCDCharacters" runat="server">
                                    <asp:ListItem Enabled="False" Value="0">characters</asp:ListItem>
                                    <asp:ListItem Selected="True" Value="1">words</asp:ListItem>
                                    <asp:ListItem Enabled="False" Value="2">sentences</asp:ListItem>
                                </asp:DropDownList>
                            </div>
                            <div class="flexbox_div">
                                Display up to&nbsp;
                            <a href="#" onclick='sub("<%=txtLimit.ClientID%>",10,0)' class="flexbox_asub">-</a>
                                <input type="text" id="txtLimit" name="terminate" size="4" value="0" class="flexbox_text" runat="server" title="0 Means Display All Result" />
                                <a href="#" onclick='add("<%=txtLimit.ClientID%>",10,100)' class="flexbox_a">+</a>
                                &nbsp;matches with&nbsp;
                            <a href="#" onclick='sub("<%=txtRpp.ClientID%>",5,10)' class="flexbox_asub">-</a>
                                <input type="text" id="txtRpp" size="4" value="20" title="Type a new value or click the - + buttons to change the value." class="flexbox_text" runat="server" />
                                <a href="#" onclick='add("<%=txtRpp.ClientID%>",5,50)' class="flexbox_a">+</a>&nbsp;
                            per page
                            </div>
                            <div class="flexbox_div">
                                <asp:CheckBox ID="isShowTotalCount" runat="server" Text="Show total number of matches" />
                            </div>
                            <div class="flexbox_div">
                                <asp:Button ID="btnSubmitConcordance" runat="server" Text="Search" CssClass="outbtndiv-button" />
                            </div>
                        </fieldset>
                    </div>
                    <div id="divConcordanceResult" runat="server">
                        <div style="width:98%;text-align:right;padding-right:10px;">
                            <asp:Label ID="lbConcCount" runat="server" Text="" ForeColor="red"></asp:Label>
                            <asp:Button ID="btnReConc" runat="server" Text="<<< Go back to Search" Visible="false"/>
                        </div>
                        <asp:GridView ID="gvConcordance" runat="server" AutoGenerateColumns="False" CellPadding="2" ForeColor="#333333" GridLines="None" DataKeyNames="CorpusID" Width="100%" AllowPaging="True" PageSize="10" PagerSettings-Mode="NumericFirstLast" >
                                <AlternatingRowStyle BackColor="White" />
                                <Columns>
                                    <asp:TemplateField HeaderText="Title">
                                        <ItemTemplate>
                                            <div style="padding: 2px">
                                                <asp:HiddenField ID="hdfCorpusID" runat="server" value='<%# Bind("CorpusID")%>'/>
                                                <asp:LinkButton ID="lnkBtn" runat="server" Text='<%# Bind("Title")%>' CommandArgument='<%# Bind("CorpusID")%>' ToolTip="View this corpora"></asp:LinkButton>
                                            </div>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Left">
                                        <ItemTemplate>
                                            <div style="padding: 2px">
                                                <asp:Label ID="lbLeft" runat="server" Text='<%# Bind("left")%>'></asp:Label>
                                            </div>
                                        </ItemTemplate>
                                        <ItemStyle HorizontalAlign="right" />
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Match">
                                        <ItemTemplate>
                                            <div style="padding: 2px">
                                                <asp:Label ID="lbMatch" runat="server" Text='<%# Bind("match")%>'></asp:Label>
                                            </div>
                                        </ItemTemplate>
                                        <ItemStyle HorizontalAlign="center" />
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Right">
                                        <ItemTemplate>
                                            <div style="padding: 2px">
                                                <asp:Label ID="lbRight" runat="server" Text='<%# Bind("right")%>'></asp:Label>
                                            </div>
                                        </ItemTemplate>
                                        <ItemStyle HorizontalAlign="left" />
                                    </asp:TemplateField>
                                </Columns>
                                <EditRowStyle BackColor="#2461BF" />
                                <FooterStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
                                <HeaderStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" HorizontalAlign="center" />
                                <PagerStyle BackColor="#2461BF" ForeColor="White" HorizontalAlign="Center" />
                                <RowStyle BackColor="#EFF3FB" />
                                <SelectedRowStyle BackColor="#D1DDF1" Font-Bold="True" ForeColor="#333333" />
                                <SortedAscendingCellStyle BackColor="#F5F7FB" />
                                <SortedAscendingHeaderStyle BackColor="#6D95E1" />
                                <SortedDescendingCellStyle BackColor="#E9EBEF" />
                                <SortedDescendingHeaderStyle BackColor="#4870BE" />
                        </asp:GridView>
                    </div>
                </asp:View>

                <%-- Collocate --%>
                <asp:View ID="vwCollocate" runat="server">
                    <div id="divCollocateQuery" runat="server" class="qfld">
                        <fieldset>
                            <legend>Search Corpus you need according to:</legend>
                            <div class="flexbox_div">
                                Search for collocates of&nbsp;
                            <input type="text" id="txtKeyCollocate" size="60" placeholder="Type a query KeyWord here" runat="server" class="input-text"/>
                            </div>
                            <div class="flexbox_div">
                                Display collocates found up to &nbsp;
                            <a href="#" onclick='sub("<%=txtcfLeft.ClientID%>",1,0)' class="flexbox_asub">-</a>
                                <input type="text" id="txtcfLeft" size="3" value="0" class="flexbox_text" runat="server" />
                                <a href="#" onclick='add("<%=txtcfLeft.ClientID%>",1,3)' class="flexbox_a">+</a>
                                &nbsp;word(s) to the left and&nbsp;
                            <a href="#" onclick='sub("<%=txtcfRight.ClientID%>",1,0)' class="flexbox_asub">-</a>
                                <input type="text" id="txtcfRight" size="3" value="1" class="flexbox_text" runat="server" />
                                <a href="#" onclick='add("<%=txtcfRight.ClientID%>",1,3)' class="flexbox_a">+</a>
                                &nbsp;word(s) to the right&nbsp;
                            </div>
                            <div class="flexbox_div">
                                Display each collocates at&nbsp;
                                <asp:DropDownList ID="ddlCollocatesPos" runat="server">
                                    <asp:ListItem Value="0">start</asp:ListItem>
                                    <asp:ListItem Selected="True" Value="1">middle</asp:ListItem>
                                    <asp:ListItem Value="2">end</asp:ListItem>
                                </asp:DropDownList>&nbsp;
                                of a context of&nbsp;
                                <a href="#" onclick='sub("<%=txtCCChars.ClientID%>",1,5)' class="flexbox_asub">-</a>
                                <input type="text" id="txtCCChars" name="contextsize" size="4" value="10" runat="server" class="flexbox_text" />
                                <a href="#" onclick='add("<%=txtCCChars.ClientID%>",1,50)' class="flexbox_a">+</a>&nbsp;
                                <asp:DropDownList ID="ddlCCCharacters" runat="server">
                                    <asp:ListItem Enabled="False" Value="0">characters</asp:ListItem>
                                    <asp:ListItem Selected="True" Value="1">words</asp:ListItem>
                                    <asp:ListItem Enabled="False" Value="2">sentences</asp:ListItem>
                                </asp:DropDownList>
                            </div>
                            <div class="flexbox_div" style="display: none">
                                Collocate Part of Speech:&nbsp;
                            <input type="button" id="PosButton" class="shortbutton it-peb" value="PoS Editor" />
                            </div>
                            <div class="flexbox_div">
                                <asp:Button ID="btnSubmitCollocate" runat="server" Text="Search" CssClass="outbtndiv-button" />
                            </div>
                        </fieldset>
                    </div>
                    <div id="divCollocateResult" runat="server">
                        <div style="width:98%;text-align:right;padding-right:10px;">
                            <asp:Label ID="lbCoLLComputedCount" runat="server" Text="" ForeColor="red"></asp:Label>
                            <asp:Button ID="btnReColl" runat="server" Text="Go back to Search" Visible="false"/>
                            </div>
                        <asp:GridView ID="gvCollComputed" runat="server" AutoGenerateColumns="False" CellPadding="2" ForeColor="#333333" GridLines="None" DataKeyNames="match" Width="100%" AllowPaging="True" PageSize="10" PagerSettings-Mode="NumericFirstLast">
                            <Columns>
                                <asp:TemplateField HeaderText="Match">
                                        <ItemTemplate>
                                            <div style="padding: 2px">
                                                <asp:LinkButton ID="lnkBtn" runat="server" Text='<%# Bind("match")%>' CommandArgument='<%# Bind("match")%>' ToolTip="View all the corpus contain this match "></asp:LinkButton>
                                            </div>
                                        </ItemTemplate>
                                    <ItemStyle HorizontalAlign="left" />
                                    </asp:TemplateField>
                                <asp:TemplateField HeaderText="Total match count in Corpus">
                                        <ItemTemplate>
                                            <div style="padding: 2px">
                                                <asp:Label ID="lbTotal" runat="server" Text='<%# Bind("totalTimes")%>'></asp:Label>
                                            </div>
                                        </ItemTemplate>
                                     <ItemStyle HorizontalAlign="center" />
                                    </asp:TemplateField>

                                <asp:TemplateField HeaderText="Total corpora count that contain the match">
                                        <ItemTemplate>
                                            <div style="padding: 2px">
                                                <asp:Label ID="lbCorpora" runat="server" Text='<%# Bind("phraseTimes")%>'></asp:Label>
                                            </div>
                                        </ItemTemplate>
                                    <ItemStyle HorizontalAlign="center" />
                                    </asp:TemplateField>
                            </Columns>
                                <AlternatingRowStyle BackColor="White" />
                                <FooterStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
                                <HeaderStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" HorizontalAlign="center" />
                                <PagerStyle BackColor="#2461BF" ForeColor="White" HorizontalAlign="Center" />
                                <RowStyle BackColor="#EFF3FB" />
                                <SelectedRowStyle BackColor="#D1DDF1" Font-Bold="True" ForeColor="#333333" />
                                <SortedAscendingCellStyle BackColor="#F5F7FB" />
                                <SortedAscendingHeaderStyle BackColor="#6D95E1" />
                                <SortedDescendingCellStyle BackColor="#E9EBEF" />
                                <SortedDescendingHeaderStyle BackColor="#4870BE" />
                        </asp:GridView>
                        <div style="width:98%;text-align:right;padding-right:10px;">
                            <asp:Label ID="lbCollCount" runat="server" Text="" ForeColor="red"></asp:Label>
                            <asp:Button ID="btnCloseColl" runat="server" Text="Go back to Search Result" Visible="false"/>
                            </div>
                        <asp:GridView ID="gvCollocate" runat="server"  AutoGenerateColumns="False" CellPadding="2" ForeColor="#333333" GridLines="None" DataKeyNames="CorpusID" Width="100%" AllowPaging="True" PageSize="10" PagerSettings-Mode="NumericFirstLast" Visible="false">
                                <AlternatingRowStyle BackColor="White" />
                                <Columns>
                                    <asp:TemplateField HeaderText="Title">
                                        <ItemTemplate>
                                            <div style="padding: 2px">
                                                <asp:HiddenField ID="hdfCorpusID" runat="server" value='<%# Bind("CorpusID")%>'/>
                                                <asp:LinkButton ID="lnkBtn" runat="server" Text='<%# Bind("Title")%>' CommandArgument='<%# Bind("CorpusID")%>' ToolTip="View this corpora"></asp:LinkButton>
                                            </div>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Left">
                                        <ItemTemplate>
                                            <div style="padding: 2px">
                                                <asp:Label ID="lbLeft" runat="server" Text='<%# Bind("left")%>'></asp:Label>
                                            </div>
                                        </ItemTemplate>
                                        <ItemStyle HorizontalAlign="right" />
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Match">
                                        <ItemTemplate>
                                            <div style="padding: 2px">
                                                <asp:Label ID="lbMatch" runat="server" Text='<%# Bind("match")%>'></asp:Label>
                                            </div>
                                        </ItemTemplate>
                                        <ItemStyle HorizontalAlign="center" />
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Right">
                                        <ItemTemplate>
                                            <div style="padding: 2px">
                                                <asp:Label ID="lbRight" runat="server" Text='<%# Bind("right")%>'></asp:Label>
                                            </div>
                                        </ItemTemplate>
                                        <ItemStyle HorizontalAlign="left" />
                                    </asp:TemplateField>
                                </Columns>
                                <EditRowStyle BackColor="#2461BF" />
                                <FooterStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
                                <HeaderStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" HorizontalAlign="center" />
                                <PagerStyle BackColor="#2461BF" ForeColor="White" HorizontalAlign="Center" />
                                <RowStyle BackColor="#EFF3FB" />
                                <SelectedRowStyle BackColor="#D1DDF1" Font-Bold="True" ForeColor="#333333" />
                                <SortedAscendingCellStyle BackColor="#F5F7FB" />
                                <SortedAscendingHeaderStyle BackColor="#6D95E1" />
                                <SortedDescendingCellStyle BackColor="#E9EBEF" />
                                <SortedDescendingHeaderStyle BackColor="#4870BE" />

                        </asp:GridView>
                    </div>
                </asp:View>

                <%-- WordList --%>
                <asp:View ID="vwWordList" runat="server">

                    <%-- 文本来源选择 --%>
                    <div id="divtxtFrom">
                        <asp:RadioButtonList ID="rbltxtFrom" runat="server" RepeatDirection="Horizontal" RepeatLayout="Table" AutoPostBack="true" CellPadding="10" CellSpacing="5" CssClass="cblList" Visible="false">
                            <asp:ListItem Value="0" Selected="true">Fill Text by Yourself </asp:ListItem>
                            <asp:ListItem Value="1" Enabled="false">Get Text From Corpus </asp:ListItem>
                        </asp:RadioButtonList>
                        <asp:HiddenField ID="hdftxtFrom" Value="0" runat="server" />
                    </div>

                    <%-- 文本来源于个人输入 --%>
                    <div id="divfromshuru" class="wbdiv" runat="server">
                        <table class="wbtable">
                            <tr>
                                <th>Title：</th>
                                <td>
                                    <input type="text" value="" class="input-text" id="txt_Title" placeholder="Type the title" runat="server" />
                                </td>
                                <th>Author：
                                </th>
                                <td>
                                    <input type="text" value="" class="input-text" id="txt_Author" placeholder="Type the Author's Name" runat="server" />
                                </td>
                                <th>Topic：
                                </th>
                                <td>
                                    <asp:DropDownList ID="ddlTopics" runat="server"></asp:DropDownList>
                                </td>
                            </tr>
                            <tr>
                                <th>Text File：</th>
                                <td>
                                    <input type="file" onchange="upload(this)" />
                                </td>
                                <td colspan="4">你可以使用左边的文件控件从文本文件的导入内容，或者你可以在下方文本框中输入或者粘贴要处理的文本!
                                </td>
                            </tr>
                        </table>
                    </div>

                    <%-- 文本来源于语料库 --%>
                    <div id="divFromCorpus" runat="server" class="wbdiv" visible="false">
                        <input type="text" class="input-text" value="" id="txtKeyWordsforWordlist" style="width: 300px; height: 25px; border: 1px solid #808080; border-radius: 5px 5px 5px 5px;" placeholder="Type the KeyWords" runat="server" title="Type the KeyWords" />
                        <asp:Button ID="btnQueryforWordlist" runat="server" Text=" 检 索 " CssClass="outbtndiv-button" />
                        <div id="divCorpusforWordList" runat="server" visible="false">
                            <hr />
                            <script type="text/javascript">
                                function fillTextfromRow(txt) {
                                    var txtCtrl = document.getElementById('<%=txtcontent.ClientID%>');
                                    txtCtrl.value = txt;
                                }
                            </script>
                            点击下面数据行，可以处理该行语料文本，点击后面的处理所有按钮，将处理筛选出的所有语料&nbsp;&nbsp;
                                                <asp:Button ID="btnLemmaAll" runat="server" Text="处理所有" />
                            <br />
                            <asp:GridView ID="gvCorpusforWordList" runat="server" AutoGenerateColumns="False" CellPadding="2" ForeColor="#333333" GridLines="None" DataKeyNames="CorpusID" Width="100%" AllowPaging="True" PageSize="10" PagerSettings-Mode="NumericFirstLast">
                                <AlternatingRowStyle BackColor="White" />
                                <Columns>
                                    <asp:TemplateField HeaderText="Title">
                                        <ItemTemplate>
                                            <div style="padding: 2px">
                                                <asp:Label ID="lbTitle" runat="server" Text='<%# Bind("Title")%>'></asp:Label>
                                            </div>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Text">
                                        <ItemTemplate>
                                            <div style="padding: 2px">
                                                <asp:Label ID="lbText" runat="server" Text='<%# Bind("OriginalText")%>'></asp:Label>
                                            </div>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                </Columns>
                                <EditRowStyle BackColor="#2461BF" />
                                <FooterStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
                                <HeaderStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
                                <PagerStyle BackColor="#2461BF" ForeColor="White" HorizontalAlign="Center" />
                                <RowStyle BackColor="#EFF3FB" />
                                <SelectedRowStyle BackColor="#D1DDF1" Font-Bold="True" ForeColor="#333333" />
                                <SortedAscendingCellStyle BackColor="#F5F7FB" />
                                <SortedAscendingHeaderStyle BackColor="#6D95E1" />
                                <SortedDescendingCellStyle BackColor="#E9EBEF" />
                                <SortedDescendingHeaderStyle BackColor="#4870BE" />
                            </asp:GridView>
                        </div>
                    </div>

                    <%-- 文本框与词汇表选择，及Lemma按钮 --%>
                    <div id="divTexts" class="wbdiv" runat="server" visible="false">
                        <div>
                            <textarea cols="100" id="txtcontent" v-model="body" onkeyup="wordStatic(this);" onchange="wordStatic(this);" onblur="wordStatic(this);" runat="server" rows="100" class="ta" placeholder="Type the English text you want to process here !"></textarea>
                        </div>
                        <div id="txtInfo">
                            <span style="color: blue">Limit: 100,000 Words&nbsp;&nbsp;</span>
                            <span id="mywords" style="display: none;">(<span id="wcount" style="color: red">Entered：0 Words;</span>
                                <span id="lcount" style="color: green">Remaining: 30,000 Words</span>)
                            </span>
                        </div>
                        <div class="qlfd">
                                            <asp:Button ID="clearBtn" runat="server" Text="Clear" CssClass="outbtndiv-button" ToolTip="Clear the Texts" />
                                            <asp:Button ID="btnSubmitForLemma" runat="server" Text="Submit" CssClass="outbtndiv-button" ToolTip="Submit to Process" />
                                        </div>

                    </div>


                </asp:View>

                <%-- Cluster --%>
                <asp:View ID="vwCluster" runat="server">
                    <h2>Cluster</h2>
                </asp:View>

                <%-- Compare Frequencies --%>
                <asp:View ID="vwCompare" runat="server">
                    <div id="divQueryforCompare" runat="server" class="qfld">
                        <table>
                        <tr>
                            <th>Compared in: </th>
                            <td>
                                <asp:RadioButtonList ID="rblforCompare" runat="server" RepeatDirection="Horizontal" CssClass="cb">
                                    <asp:ListItem Value="All" Selected="true">All corpora</asp:ListItem>
                                    <asp:ListItem Value="Some">Only selected corpra</asp:ListItem>
                                </asp:RadioButtonList>
                            </td>
                        </tr>
                        <tr>
                            <th>1st KeyWord: </th>
                            <td><input type="text" id="txtfreqField1"  size="80" placeholder="Type the 1st KeyWord for comparison here. This is a required field" runat="server" class="input-text"/><span style="color:red">*</span></td>
                        </tr>
                        <tr>
                            <th>2nd KeyWord: </th>
                            <td><input type="text" id="txtfreqField2"  size="80" placeholder="Type the 2nd KeyWord for comparison here. This is a required field" runat="server" class="input-text"/><span style="color:red">*</span></td>
                        </tr>
                        <tr>
                            <th>3rd KeyWord: </th>
                            <td><input type="text" id="txtfreqField3"  size="80" placeholder="Type the 3rd KeyWord for comparison here,This is not required" runat="server" class="input-text"/></td>
                        </tr>
                        <tr>
                            <td colspan="2">
                                <asp:Button ID="btnCompared" runat="server" Text="Sumbit" />
                            </td>
                        </tr>
                            </table>
                   </div>
                    <div id="divforCompareResult" runat="server" visible="false" class="qfld">
                        <asp:Button ID="btnBackToCompare" runat="server" Text="Back" />
                        <asp:Chart ID="chartForCompare" runat="server">
                            <Series>
                                <asp:Series Name="Series1"></asp:Series>
                            </Series>
                            <ChartAreas>
                                <asp:ChartArea Name="ChartArea1"></asp:ChartArea>
                            </ChartAreas>
                        </asp:Chart>
                    </div>
                </asp:View>

                <asp:View ID="vwLemma" runat="server">
                     <%-- 输出处理结果 --%>
                    <div id="divLemma" runat="server" class="qlfd">
                        <table>
                            <tr>
                                <th>Select WordList：
                                </th>
                                <td>
                                    <asp:RadioButtonList ID="rbVBS" runat="server" RepeatDirection="Horizontal" CssClass="cblList" AutoPostBack="true">
                                        <asp:ListItem Value="CECR" Selected="true">CECR</asp:ListItem>
                                        <asp:ListItem Value="GSL" Enabled="false">GSL</asp:ListItem>
                                        <asp:ListItem Value="AWL" Enabled="false">AWL</asp:ListItem>
                                        <asp:ListItem Value="EVP" Enabled="false">EVP</asp:ListItem>
                                    </asp:RadioButtonList>
                                </td>
                            </tr>
                        </table>
                        <div class="divmain" class="qlfd">
                            <div class="it-ec-textdiv" id="divContext" runat="server">
                                <%-- 这是输出的彩色文本以及各个级别单词占比 --%>
                            </div>
                            <div class="it-ec-statsdiv" id="divstats">
                                <%-- 单词颜色标记说明与占比 --%>

                                <div class="outbtndiv">
                                    <input id="btnLight" type="button" value="HighLight" data-i="1" onclick="HighLightAll(this)" class="outbtndiv-button" />
                                    <asp:Button ID="btnBackLemma" runat="server" Text="Back" CssClass="outbtndiv-button" ToolTip="关闭页面" />
                                </div>
                                <dl class="it-chart-dl" id="dlChart" runat="server">
                                    <dt class="it-chart-dt" data-level="UN" onclick="HighLightthis(this)">忽略处理</dt>
                                    <dd class="it-chart-dd" data-level="UN" onclick="HighLightthis(this)">
                                        <div class="it-chart-bar" style="background-color: grey; width: 11.1588%;"></div>
                                        <div class="it-chart-label">26(11.16%)</div>
                                    </dd>
                                    <dt class="it-chart-dt" data-level="C1" onclick="HighLightthis(this)">未处理</dt>
                                    <dd class="it-chart-dd" data-level="C1" onclick="HighLightthis(this)">
                                        <div class="it-chart-bar" style="background-color: orange; width: 2.5751%;"></div>
                                        <div class="it-chart-label">6(2.58%)</div>
                                    </dd>
                                    <dt class="it-chart-dt" data-level="C2" onclick="HighLightthis(this)">超纲词汇</dt>
                                    <dd class="it-chart-dd" data-level="C2" onclick="HighLightthis(this)">
                                        <div class="it-chart-bar" style="background-color: red; width: 1.7167%;"></div>
                                        <div class="it-chart-label">4(1.72%)</div>
                                    </dd>
                                    <dt class="it-chart-dt" data-level="A1" onclick="HighLightthis(this)">高中大纲</dt>
                                    <dd class="it-chart-dd" data-level="A1" onclick="HighLightthis(this)">
                                        <div class="it-chart-bar" style="background-color: indigo; width: 99.8283%; max-width: unset"></div>
                                        <div class="it-chart-label">186(88.83%)</div>
                                    </dd>
                                    <dt class="it-chart-dt" data-level="A2" onclick="HighLightthis(this)">基本要求</dt>
                                    <dd class="it-chart-dd" data-level="A2" onclick="HighLightthis(this)">
                                        <div class="it-chart-bar" style="background-color: blue; width: 3.4335%;"></div>
                                        <div class="it-chart-label">8(3.43%)</div>
                                    </dd>
                                    <dt class="it-chart-dt" data-level="B1" onclick="HighLightthis(this)">较高要求</dt>
                                    <dd class="it-chart-dd" data-level="B1" onclick="HighLightthis(this)">
                                        <div class="it-chart-bar" style="background-color: green; width: 1.2876%;"></div>
                                        <div class="it-chart-label">3(1.29%)</div>
                                    </dd>
                                    <dt class="it-chart-dt" data-level="B2" onclick="HighLightthis(this)">更高要求</dt>
                                    <dd class="it-chart-dd" data-level="B2" onclick="HighLightthis(this)">
                                        <div class="it-chart-bar" style="background-color: yellow; width: 0%;"></div>
                                        <div class="it-chart-label">0(0.00%)</div>
                                    </dd>
                                </dl>
                            </div>
                        </div>
                        <div class="divbr"></div>
                    </div>
                </asp:View>
            </asp:MultiView>
        </div>
    </div>
    <asp:HiddenField ID="hdfvwIndex" runat="server" Value="0"/>
	<asp:Label ID="lbErr" runat="server" Text="" ForeColor="red"></asp:Label>
</asp:Content>

<asp:Content ID="PageTitle" ContentPlaceHolderID="PlaceHolderPageTitle" runat="server">
NEULC
</asp:Content>

<asp:Content ID="PageTitleInTitleArea" ContentPlaceHolderID="PlaceHolderPageTitleInTitleArea" runat="server" >
NEU English Corpus > NEULC <asp:Label runat="server" ID="Titlelb"></asp:Label>
</asp:Content>
