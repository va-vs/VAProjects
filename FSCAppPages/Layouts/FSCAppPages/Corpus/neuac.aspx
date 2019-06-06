<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Import Namespace="Microsoft.SharePoint.ApplicationPages" %>
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="neuac.aspx.cs" Inherits="FSCAppPages.Layouts.FSCAppPages.Corpus.neuac" DynamicMasterPageFile="~masterurl/default.master" %>

<asp:Content ID="PageHead" ContentPlaceHolderID="PlaceHolderAdditionalPageHead" runat="server">
    <link rel="stylesheet" href="http://202.118.11.33/NEU_EC/SiteAssets/css/stylelc.css" type="text/css" charset="utf-8"  />
	<link rel="stylesheet" href="../css/tab.css"  type="text/css" charset="utf-8" />

	<style type="text/css">
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
			margin-left:5em;
		}
        .qfld fieldset legend label{
			font-weight:normal;
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
</asp:Content>

<asp:Content ID="Main" ContentPlaceHolderID="PlaceHolderMain" runat="server">
    <div id="divOutput" runat="server" class="wbdiv">
        <div id="divLemma">
            <table>
                <tr>
                    <th>Select WordList：
                    </th>
                    <td colspan="3">
                        <asp:RadioButtonList ID="rbVBS" runat="server" RepeatDirection="Horizontal" CssClass="cblList">
                            <asp:ListItem Value="0" Selected="true">CECR</asp:ListItem>
                            <asp:ListItem Value="1" Enabled="false">GSL</asp:ListItem>
                            <asp:ListItem Value="2" Enabled="false">AWL</asp:ListItem>
                            <asp:ListItem Value="3" Enabled="false">EVP</asp:ListItem>
                        </asp:RadioButtonList>
                    </td>
                </tr>
                <tr>
                    <td>
                        <div class="divmain">
                            <div class="it-ec-textdiv" id="divContext" runat="server">
                                <%-- 这是输出的彩色文本以及各个级别单词占比 --%>
                            </div>
                            <div class="it-ec-statsdiv" id="divstats">
                                <%-- 单词颜色标记说明与占比 --%>

                                <div class="outbtndiv">
                                    <input id="btnLight" type="button" value="HighLight" data-i="1" onclick="HighLightAll(this)" class="outbtndiv-button" />
                                    <asp:Button ID="btnBackLemma" runat="server" Text="Back" CssClass="outbtndiv-button" ToolTip="关闭页面" />
                                    <asp:Button ID="btnCloseLemma" runat="server" Text="Close" CssClass="outbtndiv-button" ToolTip="关闭页面" />
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
                    </td>
                </tr>
            </table>
        </div>

    </div>
    <asp:Label ID="lbErr" runat="server" Text="" ForeColor="red"></asp:Label>
</asp:Content>

<asp:Content ID="PageTitle" ContentPlaceHolderID="PlaceHolderPageTitle" runat="server">
NEUAC
</asp:Content>

<asp:Content ID="PageTitleInTitleArea" ContentPlaceHolderID="PlaceHolderPageTitleInTitleArea" runat="server" >
NEUAC
</asp:Content>
