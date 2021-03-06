﻿<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Import Namespace="Microsoft.SharePoint.ApplicationPages" %>
<%@ Register TagPrefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Register Assembly="System.Web.DataVisualization, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" Namespace="System.Web.UI.DataVisualization.Charting" TagPrefix="asp" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>

<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="neulc.aspx.cs" Inherits="FSCAppPages.Layouts.FSCAppPages.Corpus.neulc" DynamicMasterPageFile="~masterurl/default.master" EnableEventValidation="false" %>



<asp:Content ID="PageHead" ContentPlaceHolderID="PlaceHolderAdditionalPageHead" runat="server">
    <link rel="stylesheet" href="http://202.118.11.33/NEU_EC/SiteAssets/css/stylelc.css" type="text/css" />
    <link rel="Stylesheet" href="../css/pageStyle.css" type="text/css" />
    <link rel="stylesheet" href="http://202.118.11.33/NEU_EC/SiteAssets/css/highlight.css" type="text/css" />
    <link rel="stylesheet" href="../css/tab.css" type="text/css" />
    <script type="text/javascript">


        function HighLightthis(val) {
            var setValue = val.getAttribute("data-Level");
            var obj = js("<%=divContextHighLight.ClientID%>");
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
            var obj = js("<%=divContextHighLight.ClientID%>");
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
            }
            else {
                //alert('error');
                txt.value = "文件导入失败，请确认你是导入的文本文件！";
            }
        }



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

		function checkOrNot(cblId, val) {
		    for (var i = 0; i < document.getElementById(cblId).getElementsByTagName("input").length; i++) {
		        document.getElementById(cblId + "_" + i).checked = val.checked;
		    }
		}

    </script>

     <script type="text/javascript">
       function shield() {
            var s = document.getElementById("divLoadingBG");
            s.style.display = "block";

            var l = document.getElementById("divLoading");
            l.style.display = "block";
            document.getElementById('s4-workspace').style.overflow = "hidden";
        }

        function cancel_shield() {
            var s = document.getElementById("divLoadingBG");
            s.style.display = "none";

            var l = document.getElementById("divLoading");
            l.style.display = "none";

            document.getElementById('s4-workspace').style.overflow = "auto";
        }
   </script>
    <style type="text/css">
        /*遮罩层*/

        #divLoadingBG {
            width: 100%;
            height: 100%;
            background-color: #000;
            position: fixed;
            top: 0;
            left: 0;
            z-index:999;
            opacity: 0.2;
            /*兼容IE8及以下版本浏览器*/
            filter: alpha(opacity=20);
        }

        #divLoading {
            width: 200px;
            height: 200px;
            text-align: center;
            margin: auto;
            position: fixed;
            z-index:10000;
            top: 0;
            bottom: 0;
            left: 0;
            right: 0;
        }


        /*语料彩色可视化*/
        .divmain {
            width: 100%;
            margin-bottom: 10px;
        }

        .it-ec-textdiv {
            float: left;
            background-color: #dcdcdc;
            border: 1px solid #808080;
            line-height: 25px;
            font-size: 13px;
            padding: 5px;
            height: 400px;
            overflow-y: auto;
        }

        .it-ec-statsdiv {
            float: left;
        }

        .it-ec-paragraph {
            text-align: justify;
            border: 1px solid;
            margin: 0px;
            padding: 5px;
            border-color: #663;
            background-color: #dcdcdc;
        }



        .it-chart-dl {
            margin-left: 10px;
            width: 400px;
        }

        .it-chart-dt {
            cursor: pointer;
            float: left;
            width: 60px;
            text-align: right;
            padding: 5px 0px 5px 0px;
            clear: left;
        }

        .it-chart-dd {
            cursor: pointer;
            float: left;
            width: 320px;
            margin-left: 5px;
            padding: 5px 0px 5px 0px;
        }

        .it-chart-bar {
            height: 20px;
            float: left;
            -moz-border-radius: 2px;
            -webkit-border-radius: 2px;
            border-radius: 2px;
        }

        .it-chart-label {
            width: 20px;
            height: 100%;
            float: left;
            text-align: right;
        }

        /*文本高亮*/

        .KW.highlight {
            background-color: pink;
        }

        .C2.highlight {
            background-color: red;
            color: white;
        }

        .C1.highlight {
            background-color: orange;
            color: white;
        }

        .B2.highlight {
            background-color: yellow;
            color: black;
        }

        .B1.highlight {
            background-color: green;
            color: white;
        }

        .A2.highlight {
            background-color: blue;
            color: white;
        }

        .A1.highlight {
            background-color: indigo;
            color: white;
        }

        .UN.highlight {
            background-color: grey;
            color: white;
        }

        span.MW {
            font-weight: bold;
        }

        .div-inline {
            display: inline;
        }

        .infoTable {
            border: none;
            border-collapse: collapse;
            line-height:15px;
            font-size:13px;
        }

            .infoTable tr {
                border:none;
            }

            .infoTable th {
                color: #333;
                font-weight:500;
                padding:2px 5px;
                text-align: right;
                min-width: 200px;
            }

            .infoTable td {
                padding:2px 5px;
                text-align: left;
                color:#555;
            }

            .infoTable tr:nth-of-type(odd) {
                background: #F5FAFA;
            }

            .infoTable tr:hover {
                background: #D2DCF5;
            }
    </style>

    <style type="text/css">
        div.menu {
            padding: 0px 0px 0px 0px;
        }

            div.menu ul {
                list-style: none;
                margin: 0px;
                padding: 0px;
                width: auto;
            }

                div.menu ul li ul li {
                    display: block;
                    width: 250px;
                    padding: 5px;
                }

                div.menu ul li a, div.menu ul li a:visited {
                    background-color: lightgrey;
                    color: #333333;
                    display: block;
                    line-height: 35px;
                    text-decoration: none;
                    white-space: nowrap;
                    height: 35px;
                    font-size: 12px;
                }

                div.menu ul li {
                    margin: 0 0 0px;
                }

                    div.menu ul li a:hover {
                        background-color: #bfcbd6;
                        color: red;
                        text-decoration: none;
                    }

                    div.menu ul li a:active {
                        text-decoration: none;
                    }

        .mynav {
            border-top: 1px solid #808080;
            border-left: 1px solid #808080;
            border-right: 1px solid #808080;
        }

        .myblock {
            background-color: lightgrey;
            border-top: 1px solid #808080;
            border-left: 1px solid #808080;
            border-right: 1px solid #808080;
        }

        .mycontent {
            border-bottom: 1px solid #808080;
            border-left: 1px solid #808080;
            border-right: 1px solid #808080;
            padding: 5px;
            line-height: 25px;
        }



        .elemfield {
            margin-bottom: 10px;
            padding: 5px;
            border-width: 1px;
            border-style: solid;
            border-color: #e6e6e6;
        }

            .elemfield legend {
                margin-left: 10px;
                padding: 0 10px;
                font-size: 15px;
                font-weight: 600;
                border-bottom: none;
                width: auto;
            }

                .elemfield legend label {
                    font-size: 12px;
                    font-weight: 300;
                    color: #663;
                }

        .fieldbox {
            padding: 10px 15px;
            line-height: 25px;
        }

        .cbl {
            border-collapse: collapse;
            line-height:25px;
        }

            .cbl input[type="checkbox"] {
                display: none;
            }

                .cbl input[type="checkbox"]:checked + label {
                    background-color: #F5F5F5;
                    color: #EA5C64;
                    padding: 2px 5px;
                    border-radius: 5px;
                    border: 1px solid #EA5C64;
                }

            .cbl input[type="radio"] {
                display: none;
            }

                .cbl input[type="radio"]:checked + label {
                    background-color: #F5F5F5;
                    color: #EA5C64;
                    padding: 2px 5px;
                    border-radius: 5px;
                    border: 1px solid #EA5C64;
                }

            .cbl label {
                margin-left: 10px;
                padding: 2px 5px;
                border: 1px solid #CCC;
                color: #777;
                border-radius: 5px;
            }

                .cbl label:hover {
                    color: #EA5C64;
                    cursor: pointer;
                }

        .rbl {
            border-collapse: collapse;
            line-height: 25px;
        }
         .rbl input[type="radio"] {
                display: none;
            }

                .rbl input[type="radio"]:checked + label {
                    background-color: #F5F5F5;
                    color: #EA5C64;
                    padding: 2px 5px;
                    border-radius: 5px;
                    border: 1px solid #EA5C64;
                    border-bottom:none;
                    font-size:16px;
                    font-weight:bold;
                }

            .rbl label {
                margin-left: 10px;
                padding: 2px 5px;
                border: 1px solid #CCC;
                color: #777;
                border-radius: 5px;
                font-size:14px;
                font-weight:400;
            }

                .rbl label:hover {
                    color: #EA5C64;
                    cursor: pointer;
                }
        .lglabel {
            margin-left: 10px;
            font-size: 12px;
            font-weight: 300;
            padding: 2px 5px;
            border: 1px solid #CCC;
            color: #777;
            border-radius: 5px;
        }

            .lglabel:hover {
                cursor: pointer;
                color: #EA5C64;
            }

        input[type="checkbox"]:checked + .lglabel {
            background-color: #F5F5F5;
            color: #EA5C64;
            padding: 0px 5px;
            border-radius: 5px;
            border: 1px solid #EA5C64;
        }

        .input-text {
            border: 1px solid #808080;
            border-radius: 5px 5px 5px 5px;
            font-size: 13px;
            height: 25px;
            line-height: 25px;
            margin-right: 10px;
            padding-left: 5px;
        }

        .input-ddl {
            border: 1px solid #808080;
            border-radius: 5px 5px 5px 5px;
            font-size: 13px;
            height: 25px;
            line-height: 25px;
            margin-right: 10px;
            padding-left: 5px;
        }

        .input-file {
            border: 1px solid #808080;
            border-radius: 5px 5px 5px 5px;
            font-size: 13px;
            height: 25px;
            line-height: 25px;
            margin-right: 10px;
            padding-left: 5px;
        }

.wbdiv {
	width: 100%;
	line-height: 25px;
	margin: 5px;
}
    </style>
</asp:Content>

<asp:Content ID="Main" ContentPlaceHolderID="PlaceHolderMain" runat="server">
    <div class="divneulc">
        <%-- 快捷菜单 --%>
        <div id="divNav" class="mynav">
            <table>
                <tr>
                    <td>
                        <asp:Label ID="lbCorpus" runat="server" Text="NEULC" Font-Size="18" Font-Bold="true"></asp:Label>
                    </td>
                    <td>
                        <ul class="imgul">
                            <li>
                                <asp:ImageButton ID="ibtnInfo" runat="server" ImageUrl="../images/Info.png" ImageAlign="Middle" Height="20" Width="20" PostBackUrl="#" ToolTip="About this Corpus" Enabled="false" />
                            </li>
                            <li>
                                <asp:ImageButton ID="ibtnDownload" runat="server" ImageUrl="../images/Download.png" ImageAlign="Middle" Height="20" Width="20" PostBackUrl="#" ToolTip="DownLoad this text" Enabled="false" />
                            </li>
                            <li>
                                <asp:ImageButton ID="ibtnUpload" runat="server" ImageUrl="../images/Upload.png" PostBackUrl="admin.aspx" ImageAlign="Middle" Height="20" Width="20" ToolTip="Upload a new text" />

                            </li>
                            <li>
                                <asp:ImageButton ID="ibtnShare" runat="server" ImageUrl="../images/Share.png" PostBackUrl="#" ImageAlign="Middle" Height="20" Width="20" ToolTip="Share this text" Enabled="false" />
                            </li>
                            <li>
                                <asp:ImageButton ID="ibtnGuide" runat="server" ImageUrl="../images/Guide.png" PostBackUrl="#" ImageAlign="Middle" Height="20" Width="20" ToolTip="Get Help" Enabled="false" />
                            </li>
                        </ul>
                    </td>
                    <td>
                        <div id="divCPTips" runat="server" visible="false">
                            <span class="sptips">You haven't chosen corpus yet,so you can use all corpus for other operations or click the <strong>"Corpus"</strong> Menu to  select the corpus first.
                            </span>
                        </div>

                    </td>
                </tr>
            </table>
        </div>
        <%-- 导航菜单 --%>
        <div id="divMenu" class="myblock">
            <asp:Menu ID="muNeulc" runat="server" Orientation="Horizontal" DynamicHorizontalOffset="2" StaticDisplayLevels="1" CssClass="menu" StaticSubMenuIndent="10px" DisappearAfter="600">
                <Items>
                    <asp:MenuItem Text="Corpus" Value="Corpus"></asp:MenuItem>
                    <asp:MenuItem Text="Concordance" Value="Concordance"></asp:MenuItem>
                    <asp:MenuItem Text="Collocation" Value="Collocate"></asp:MenuItem>
                    <asp:MenuItem Text="Word Chunk" Value="Cluster"></asp:MenuItem>
                    <asp:MenuItem Text="Compare" Value="Compare"></asp:MenuItem>
                    <asp:MenuItem Text="VocabProfile" Value="Grading"></asp:MenuItem>
                </Items>
                <StaticSelectedStyle Font-Bold="true" HorizontalPadding="10" ForeColor="#333333" BackColor="white" />
                <StaticMenuItemStyle HorizontalPadding="10" Font-Size="Medium" />
                <StaticHoverStyle ForeColor="red" />
                <DynamicMenuStyle HorizontalPadding="10" />
                <DynamicSelectedStyle BackColor="white" Font-Bold="true" ForeColor="#333333" />
                <DynamicMenuItemStyle BackColor="#cccccc" HorizontalPadding="18" Font-Size="Medium" />
                <DynamicHoverStyle ForeColor="red" />
            </asp:Menu>
        </div>
        <%-- 导航内容 --%>
        <div id="divViews" class="mycontent">
            <asp:MultiView ID="mvNeulc" runat="server">
                <%-- 检索 --%>
                <asp:View ID="vwCorpus" runat="server">
                    <%-- 语料库检索div --%>
                    <div id="divNEULC" runat="server">
                        <div style="font-size: 14px;">
                            Select Corpus you need according to:
                        </div>
                        <div>

                            <fieldset class="elemfield">
                                <legend>
                                    <span>Level </span>
                                    <input id="cbforLevel" type="checkbox" onchange='checkOrNot("<%=cblLevel.ClientID%>", this)' hidden />
                                    <label for="cbforLevel" class="lglabel">Select All Levels</label>
                                </legend>
                                <div class="fieldbox">
                                    <asp:CheckBoxList ID="cblLevel" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow" CssClass="cbl">
                                        <asp:ListItem Value="1">L1(freshmen)</asp:ListItem>
                                        <asp:ListItem Value="2">L2(sophomores)</asp:ListItem>
                                        <asp:ListItem Value="3">L3(juniors)</asp:ListItem>
                                        <asp:ListItem Value="4">L4(seniors)</asp:ListItem>
                                    </asp:CheckBoxList>
                                </div>
                            </fieldset>
                            <fieldset class="elemfield">
                                <legend>
                                    <span>Genre </span>
                                    <input id="cbforGenre" type="checkbox" onchange='checkOrNot("<%=cblGenre.ClientID%>", this)' hidden />
                                    <label for="cbforGenre" class="lglabel">Select All Genres</label>
                                </legend>
                                <div class="fieldbox">
                                    <asp:CheckBoxList ID="cblGenre" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow" CssClass="cbl">
                                        <asp:ListItem Value="1">Argumentation</asp:ListItem>
                                        <asp:ListItem Value="2">Narration</asp:ListItem>
                                        <asp:ListItem Value="3">Exposition</asp:ListItem>
                                        <asp:ListItem Value="4">Applied Writing</asp:ListItem>
                                    </asp:CheckBoxList>
                                </div>
                            </fieldset>
                            <fieldset class="elemfield">
                                <legend>
                                    <span>Topic </span>
                                    <input id="cbforTopic" type="checkbox" onchange='checkOrNot("<%=cblTopic.ClientID%>", this)' hidden />
                                    <label for="cbforTopic" class="lglabel">Select All Topics</label>
                                </legend>
                                <div class="fieldbox">
                                    <asp:CheckBoxList ID="cblTopic" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow" CssClass="cbl">
                                        <asp:ListItem Value="1">Culture</asp:ListItem>
                                        <asp:ListItem Value="2">Science & Technology</asp:ListItem>
                                        <asp:ListItem Value="3">Humanity</asp:ListItem>
                                        <asp:ListItem Value="4">Society</asp:ListItem>
                                    </asp:CheckBoxList>
                                </div>
                            </fieldset>
                        </div>

                        <div style="text-align: center; margin: 10px;">
                            <asp:Button ID="btnSubmitforCorpus" runat="server" Text="Submit" CssClass="outbtndiv-button" />
                        </div>
                    </div>

                    <div id="divNEUAC" runat="server" visible="false">
                        <fieldset class="elemfield">
                            <legend>
                                <span>Year </span>
                                <input id="cbforYear" type="checkbox" onchange='checkOrNot("<%=cblYear.ClientID%>", this)' hidden />
                                <label for="cbforYear" class="lglabel">Select All Years</label>
                            </legend>
                            <div class="fieldbox">
                                <asp:CheckBoxList ID="cblYear" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow" CssClass="cbl">
                                    <asp:ListItem Value="1">2013</asp:ListItem>
                                    <asp:ListItem Value="2">2014</asp:ListItem>
                                </asp:CheckBoxList>
                            </div>
                        </fieldset>
                        <fieldset class="elemfield">
                            <legend>
                                <span>Major </span>
                                <input id="cbforMajor" type="checkbox" onchange='checkOrNot("<%=cblMajor.ClientID%>", this)' hidden />
                                <label for="cbforMajor" class="lglabel">Select All Majors</label>
                            </legend>
                            <div class="fieldbox">
                                <asp:CheckBoxList ID="cblMajor" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow" CssClass="cbl">
                                    <asp:ListItem Value="1">Computer</asp:ListItem>
                                    <asp:ListItem Value="2">Material</asp:ListItem>
                                </asp:CheckBoxList>
                            </div>
                        </fieldset>
                        <fieldset id="fldJournals" runat="server" class="elemfield">
                            <legend>
                                <span>Journal </span>
                                <input id="cbforJournal" type="checkbox" onchange='checkOrNot("<%=cblJournal.ClientID%>", this)' hidden />
                                <label for="cbforJournal" class="lglabel">Select All Journals</label>
                            </legend>
                            <div class="fieldbox">
                                <asp:CheckBoxList ID="cblJournal" runat="server" RepeatLayout="Flow" CssClass="cbl">
                                    <asp:ListItem Value="1">JOURNAL OF ALLOYS AND COMPOUNDS</asp:ListItem>
                                    <asp:ListItem Value="2">MATERIALS SCIENCE AND ENGINEERING A-STRUCTURAL MATERIALS PROPERTIES MICROSTRUCTURE AND PROCESSING
                                    </asp:ListItem>
                                </asp:CheckBoxList>
                            </div>
                        </fieldset>
                        <div style="text-align: center; margin: 10px;">
                            <asp:Button ID="btnSubmitforNEUAC" runat="server" Text="Submit" CssClass="outbtndiv-button" Enabled="false" />
                            <asp:Label ID="Label1" runat="server" Text=" NEUAC 暂不提供筛选和汇总计算..." ForeColor="red"></asp:Label>
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
                            <asp:Button ID="btnBacktoQuery" runat="server" Text=" Back " OnClientClick="Uncheck_All()" class="outbtndiv-button" />
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
                    <div id="divConcordanceQuery" runat="server">
                        <fieldset class="elemfield">
                            <legend>Select Corpus you need according to:</legend>
                            <div class="fieldbox">
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
                                <div class="flexbox_div" style="padding-left: 50px;">
                                    <asp:Button ID="btnSubmitConcordance" runat="server" Text="Search" CssClass="outbtndiv-button" />
                                </div>
                            </div>
                        </fieldset>
                    </div>
                    <div id="divConcordanceResult" runat="server" visible="false">
                        <div style="width: 98%; padding: 10px;">
                            <span id="spConcCount" class="sptips" runat="server"></span>
                            <asp:Button ID="btnReConc" runat="server" Text=" Back " CssClass="outbtndiv-button" />
                        </div>
                        <asp:GridView ID="gvConcordance" runat="server" AutoGenerateColumns="False" CellPadding="2" ForeColor="#333333" GridLines="None" DataKeyNames="CorpusID" AllowPaging="True" PageSize="10" PagerSettings-Mode="NumericFirstLast">
                            <AlternatingRowStyle BackColor="White" />
                            <Columns>
                                <asp:TemplateField HeaderText="Title">
                                    <ItemTemplate>
                                        <div style="padding: 5px">
                                            <asp:HiddenField ID="hdfCorpusID" runat="server" Value='<%# Bind("CorpusID")%>' />
                                            <asp:LinkButton ID="lnkBtn" runat="server" Text='<%# Bind("Title")%>' CommandArgument='<%# Bind("CorpusID")%>' ToolTip="View this text"></asp:LinkButton>
                                        </div>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Left">
                                    <ItemTemplate>
                                        <div style="padding: 5px">
                                            <asp:Label ID="lbLeft" runat="server" Text='<%# Bind("left")%>'></asp:Label>
                                        </div>
                                    </ItemTemplate>
                                    <ItemStyle HorizontalAlign="right" />
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Match">
                                    <ItemTemplate>
                                        <div style="padding: 5px">
                                            <asp:Label ID="lbMatch" runat="server" Font-Bold="true" ForeColor="red" Text='<%# Bind("match")%>'></asp:Label>
                                        </div>
                                    </ItemTemplate>
                                    <ItemStyle HorizontalAlign="center" />
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Right">
                                    <ItemTemplate>
                                        <div style="padding: 5px">
                                            <asp:Label ID="lbRight" runat="server" Text='<%# Bind("right")%>'></asp:Label>
                                        </div>
                                    </ItemTemplate>
                                    <ItemStyle HorizontalAlign="left" />
                                </asp:TemplateField>
                            </Columns>
                            <EditRowStyle BackColor="#036cb4" />
                            <FooterStyle Font-Bold="True" />
                            <HeaderStyle BackColor="#036cb4" Font-Bold="True" ForeColor="White" HorizontalAlign="center" />
                            <RowStyle BackColor="#EFF3FB" />
                            <SelectedRowStyle BackColor="#D1DDF1" Font-Bold="True" ForeColor="#333333" />
                            <SortedAscendingCellStyle BackColor="#F5F7FB" />
                            <SortedAscendingHeaderStyle BackColor="#6D95E1" />
                            <SortedDescendingCellStyle BackColor="#E9EBEF" />
                            <SortedDescendingHeaderStyle BackColor="#036cb4" />
                            <PagerStyle HorizontalAlign="Center" VerticalAlign="Middle" CssClass="pagination" />
                        </asp:GridView>
                        <div id="divConcTips" runat="server" style="width: 98%; padding: 10px;" visible="false">
                            <span id="spConcTips" class="sptips" runat="server">Click on the <strong>"Title"</strong> in each row of the list to view the text</span>
                            <asp:Button ID="btnViewConc" runat="server" Text="View All" CssClass="outbtndiv-button" />
                        </div>
                    </div>
                </asp:View>

                <%-- Collocate --%>
                <asp:View ID="vwCollocate" runat="server">
                    <div id="divCollocateQuery" runat="server">
                        <fieldset class="elemfield">
                            <legend>Search Corpus you need according to:</legend>
                            <div class="fieldbox">
                                <div class="flexbox_div">
                                    Search for collocates of&nbsp;
							<input type="text" id="txtKeyCollocate" size="60" placeholder="Type a query KeyWord here" runat="server" class="input-text" />
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
                                <div class="flexbox_div" style="padding-left: 50px;">
                                    <asp:Button ID="btnSubmitCollocate" runat="server" Text="Search" CssClass="outbtndiv-button" />
                                </div>
                            </div>
                        </fieldset>
                    </div>
                    <div id="divCollocateResult" runat="server">
                        <div id="divCollComputed" runat="server" visible="false">
                            <div style="width: 98%; padding: 10px;">
                                <span id="spCoLLComputedCount" class="sptips" runat="server"></span>
                                <asp:Button ID="btnReColl" runat="server" Text=" Back " CssClass="outbtndiv-button" />
                            </div>
                            <asp:GridView ID="gvCollComputed" runat="server" AutoGenerateColumns="False" CellPadding="2" ForeColor="#333333" GridLines="None" DataKeyNames="match" AllowPaging="True" PageSize="10" PagerSettings-Mode="NumericFirstLast">
                                <Columns>
                                    <asp:TemplateField HeaderText="Match">
                                        <ItemTemplate>
                                            <div style="min-width: 100px; padding: 5px">
                                                <asp:LinkButton ID="lnkBtn" runat="server" Text='<%# Bind("match")%>' Font-Bold="true" ForeColor="red" CommandArgument='<%# Bind("match")%>' ToolTip="View all the corpus contain this match "></asp:LinkButton>
                                            </div>
                                        </ItemTemplate>
                                        <ItemStyle HorizontalAlign="left" />
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Frequence">
                                        <ItemTemplate>
                                            <div style="padding: 5px; min-width: 100px;">
                                                <asp:Label ID="lbTotal" runat="server" Text='<%# Bind("totalTimes")%>'></asp:Label>
                                            </div>
                                        </ItemTemplate>
                                        <ItemStyle HorizontalAlign="center" />
                                    </asp:TemplateField>

                                    <asp:TemplateField HeaderText="Range">
                                        <ItemTemplate>
                                            <div style="padding: 5px; min-width: 100px;">
                                                <asp:Label ID="lbCorpora" runat="server" Text='<%# Bind("phraseTimes")%>'></asp:Label>
                                            </div>
                                        </ItemTemplate>
                                        <ItemStyle HorizontalAlign="center" />
                                    </asp:TemplateField>
                                </Columns>
                                <EditRowStyle BackColor="#036cb4" />
                                <FooterStyle Font-Bold="True" Height="25px" />
                                <HeaderStyle BackColor="#036cb4" Font-Bold="True" ForeColor="White" HorizontalAlign="center" Height="25px" />
                                <RowStyle BackColor="#EFF3FB" />
                                <SelectedRowStyle BackColor="#D1DDF1" Font-Bold="True" ForeColor="#333333" />
                                <SortedAscendingCellStyle BackColor="#F5F7FB" />
                                <SortedAscendingHeaderStyle BackColor="#6D95E1" />
                                <SortedDescendingCellStyle BackColor="#E9EBEF" />
                                <SortedDescendingHeaderStyle BackColor="#036cb4" />
                                <PagerStyle HorizontalAlign="Center" VerticalAlign="Middle" CssClass="pagination" />
                            </asp:GridView>
                            <div id="divCollComputedTips" runat="server" style="width: 98%; padding: 10px;">
                                <span class="sptips">Click on the <strong>"Match"</strong> in each row of the list to view the text with the match</span>
                            </div>
                        </div>
                        <div id="divCollView" runat="server" visible="false">
                            <div style="width: 98%; padding: 10px;">
                                <span id="spCoLLCount" class="sptips" runat="server"></span>
                                <asp:Button ID="btnCloseColl" runat="server" Text=" Back " CssClass="outbtndiv-button" />
                            </div>
                            <asp:GridView ID="gvCollocate" runat="server" AutoGenerateColumns="False" CellPadding="2" ForeColor="#333333" GridLines="None" DataKeyNames="CorpusID" AllowPaging="True" PageSize="10" PagerSettings-Mode="NumericFirstLast">
                                <AlternatingRowStyle BackColor="White" />
                                <Columns>
                                    <asp:TemplateField HeaderText="Title">
                                        <ItemTemplate>
                                            <div style="padding: 5px">
                                                <asp:HiddenField ID="hdfCorpusID" runat="server" Value='<%# Bind("CorpusID")%>' />
                                                <asp:LinkButton ID="lnkBtn" runat="server" Text='<%# Bind("Title")%>' CommandArgument='<%# Bind("CorpusID")%>' ToolTip="View this text"></asp:LinkButton>
                                            </div>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Left">
                                        <ItemTemplate>
                                            <div style="padding: 5px">
                                                <asp:Label ID="lbLeft" runat="server" Text='<%# Bind("left")%>'></asp:Label>
                                            </div>
                                        </ItemTemplate>
                                        <ItemStyle HorizontalAlign="right" />
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Match">
                                        <ItemTemplate>
                                            <div style="padding: 5px">
                                                <asp:Label ID="lbMatch" runat="server" Font-Bold="true" ForeColor="red" Text='<%# Bind("match")%>'></asp:Label>
                                            </div>
                                        </ItemTemplate>
                                        <ItemStyle HorizontalAlign="center" />
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Right">
                                        <ItemTemplate>
                                            <div style="padding: 5px">
                                                <asp:Label ID="lbRight" runat="server" Text='<%# Bind("right")%>'></asp:Label>
                                            </div>
                                        </ItemTemplate>
                                        <ItemStyle HorizontalAlign="left" />
                                    </asp:TemplateField>
                                </Columns>
                                <EditRowStyle BackColor="#036cb4" />
                                <FooterStyle Font-Bold="True" Height="25px" />
                                <HeaderStyle BackColor="#036cb4" Font-Bold="True" ForeColor="White" HorizontalAlign="center" Height="25px" />
                                <RowStyle BackColor="#EFF3FB" />
                                <SelectedRowStyle BackColor="#D1DDF1" Font-Bold="True" ForeColor="#333333" />
                                <SortedAscendingCellStyle BackColor="#F5F7FB" />
                                <SortedAscendingHeaderStyle BackColor="#6D95E1" />
                                <SortedDescendingCellStyle BackColor="#E9EBEF" />
                                <SortedDescendingHeaderStyle BackColor="#036cb4" />
                                <PagerStyle HorizontalAlign="Center" VerticalAlign="Middle" CssClass="pagination" />

                            </asp:GridView>
                            <div id="divCollTips" style="width: 98%; padding: 10px;" runat="server">
                                <span id="spCollTips" runat="server" class="sptips">Click on the <strong>"Title"</strong> in each row of the list to view the text</span>
                                <asp:Button ID="btnViewColl" runat="server" Text="View All" CssClass="outbtndiv-button" />
                            </div>
                        </div>
                    </div>
                </asp:View>

                <%-- Cluster --%>
                <asp:View ID="vwCluster" runat="server">
                    <div id="divSetCluster" runat="server">
                        <div class="flexbox_div">
                            Input or Set the word count for Word Chunk（2~5）:
                        <a href="#" onclick='sub("<%=txtClusterChars.ClientID%>",1,2)' class="flexbox_asub">-</a>
                            <input type="text" id="txtClusterChars" name="contextsize" size="5" value="2" runat="server" class="flexbox_text" title="Input the word count for Word Chunk" placeholder="Type a integer number between 2 to 5" />
                            <a href="#" onclick='add("<%=txtClusterChars.ClientID%>",1,5)' class="flexbox_a">+</a>&nbsp;
                            <asp:Button ID="btnSetCluster" runat="server" Text="Submit" CssClass="outbtndiv-button" />
                            <span id="spanMsg" runat="server" style="padding-left: 200px;"></span>
                        </div>
                        <div id="divAllCluster" runat="server" visible="false">
                            <table>
                                <tr>
                                    <td style="vertical-align: top;">
                                        <fieldset class="elemfield">
                                            <legend>Word Chunk</legend>
                                            <div style="height: 460px; width: 100%; line-height: 25px; overflow-y: auto;">
                                                <asp:GridView ID="gvClusterAll" runat="server" AutoGenerateColumns="False" CellPadding="2" ForeColor="#333333" GridLines="None" DataKeyNames="Cluster">
                                                    <Columns>
                                                        <asp:TemplateField HeaderText="Word Chunk">
                                                            <ItemTemplate>
                                                                <div style="min-width: 100px; padding: 5px">
                                                                    <asp:LinkButton ID="lnkBtn" runat="server" Text='<%# Bind("Cluster")%>' CommandArgument='<%# Bind("Cluster")%>' ToolTip="Click here to view all the texts that contains this word chunk in the right list"></asp:LinkButton>
                                                                </div>
                                                            </ItemTemplate>
                                                            <ItemStyle HorizontalAlign="left" />
                                                        </asp:TemplateField>
                                                        <asp:TemplateField HeaderText="Count">
                                                            <ItemTemplate>
                                                                <div style="padding: 5px; min-width: 100px;">
                                                                    <asp:Label ID="lbTotal" runat="server" Text='<%# Bind("Count")%>'></asp:Label>
                                                                </div>
                                                            </ItemTemplate>
                                                            <ItemStyle HorizontalAlign="right" />
                                                        </asp:TemplateField>
                                                    </Columns>
                                                    <EditRowStyle BackColor="#036cb4" />
                                                    <FooterStyle Font-Bold="True" Height="25px" />
                                                    <HeaderStyle BackColor="#036cb4" Font-Bold="True" ForeColor="White" HorizontalAlign="center" Height="25px" />
                                                    <RowStyle BackColor="#EFF3FB" />
                                                    <SelectedRowStyle BackColor="#D1DDF1" Font-Bold="True" ForeColor="#333333" />
                                                    <SortedAscendingCellStyle BackColor="#F5F7FB" />
                                                    <SortedAscendingHeaderStyle BackColor="#6D95E1" />
                                                    <SortedDescendingCellStyle BackColor="#E9EBEF" />
                                                    <SortedDescendingHeaderStyle BackColor="#036cb4" />
                                                    <PagerStyle HorizontalAlign="Center" VerticalAlign="Middle" CssClass="pagination" />
                                                </asp:GridView>
                                            </div>

                                        </fieldset>
                                    </td>
                                    <td style="vertical-align: top;">
                                        <fieldset class="elemfield">
                                            <legend>Texts<span id="spCClusterTips" runat="server" class="sptips"></span></legend>
                                            <div id="divCluterList" runat="server" style="width: 100%; line-height: 25px; overflow-y: auto;">
                                                <asp:GridView ID="gvCorpusforCluster" runat="server" AutoGenerateColumns="False" CellPadding="2" ForeColor="#333333" GridLines="None" DataKeyNames="CorpusID" Width="100%" AllowPaging="True" PageSize="10" PagerSettings-Mode="NumericFirstLast">
                                                    <AlternatingRowStyle BackColor="White" />
                                                    <Columns>
                                                        <asp:TemplateField HeaderText="Title">
                                                            <ItemTemplate>
                                                                <div style="padding: 5px">
                                                                    <asp:HiddenField ID="hdfCorpusID" runat="server" Value='<%# Bind("CorpusID")%>' />
                                                                    <asp:LinkButton ID="lnkBtn" runat="server" Text='<%# Bind("Title")%>' CommandArgument='<%# Bind("CorpusID")%>' ToolTip="View this text"></asp:LinkButton>
                                                                </div>
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                        <asp:TemplateField HeaderText="Text">
                                                            <ItemTemplate>
                                                                <div style="padding: 5px">
                                                                    <asp:Label ID="lbText" runat="server" Text='<%# Bind("OriginalText")%>'></asp:Label>
                                                                </div>
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                    </Columns>
                                                    <EditRowStyle BackColor="#036cb4" />
                                                    <FooterStyle Font-Bold="True" />
                                                    <HeaderStyle BackColor="#036cb4" Font-Bold="True" ForeColor="White" HorizontalAlign="center" />
                                                    <RowStyle BackColor="#EFF3FB" />
                                                    <SelectedRowStyle BackColor="#D1DDF1" Font-Bold="True" ForeColor="#333333" />
                                                    <SortedAscendingCellStyle BackColor="#F5F7FB" />
                                                    <SortedAscendingHeaderStyle BackColor="#6D95E1" />
                                                    <SortedDescendingCellStyle BackColor="#E9EBEF" />
                                                    <SortedDescendingHeaderStyle BackColor="#036cb4" />
                                                    <PagerStyle HorizontalAlign="Center" VerticalAlign="Middle" CssClass="pagination" />
                                                </asp:GridView>
                                            </div>
                                        </fieldset>
                                    </td>
                                </tr>
                            </table>
                        </div>

                    </div>
                    <div id="divShowCluster" runat="server" visible="false">
                        <div id="divClusterTips" style="width: 98%; padding: 10px;" runat="server">
                            <span id="spClusterTips" runat="server" class="sptips"></span>
                            <asp:Button ID="btnResetCluster" runat="server" Text=" Back " CssClass="outbtndiv-button" />
                        </div>
                        <div>
                            <table>
                                <tr>
                                    <td style="vertical-align: top; width: 60%">
                                        <fieldset class="elemfield">
                                            <legend>Text</legend>
                                            <div id="divCluterContext" runat="server" style="height: 400px; min-width: 300px; width: 100%; background-color: #dcdcdc; line-height: 25px; font-size: 13px; overflow-y: auto;"></div>
                                        </fieldset>
                                    </td>
                                    <td style="vertical-align: top; width: 40%">
                                        <fieldset class="elemfield">
                                            <legend>Word Chunk</legend>
                                            <div style="height: 400px; min-width: 300px; width: 100%; overflow-y: auto;">
                                                <asp:GridView ID="gvCluster" runat="server" AutoGenerateColumns="False" CellPadding="2" ForeColor="#333333" GridLines="None" DataKeyNames="Cluster">
                                                    <Columns>
                                                        <asp:TemplateField HeaderText="Cluster">
                                                            <ItemTemplate>
                                                                <div style="min-width: 100px; padding: 5px">
                                                                    <asp:Label ID="lbCluster" runat="server" Text='<%# Bind("Cluster")%>'></asp:Label>
                                                                </div>
                                                            </ItemTemplate>
                                                            <ItemStyle HorizontalAlign="left" />
                                                        </asp:TemplateField>
                                                        <asp:TemplateField HeaderText="Count">
                                                            <ItemTemplate>
                                                                <div style="padding: 5px; min-width: 100px;">
                                                                    <asp:Label ID="lbTotal" runat="server" Text='<%# Bind("Count")%>'></asp:Label>
                                                                </div>
                                                            </ItemTemplate>
                                                            <ItemStyle HorizontalAlign="right" />
                                                        </asp:TemplateField>
                                                    </Columns>
                                                    <EditRowStyle BackColor="#036cb4" />
                                                    <FooterStyle Font-Bold="True" Height="25px" />
                                                    <HeaderStyle BackColor="#036cb4" Font-Bold="True" ForeColor="White" HorizontalAlign="center" Height="25px" />
                                                    <RowStyle BackColor="#EFF3FB" />
                                                    <SelectedRowStyle BackColor="#D1DDF1" Font-Bold="True" ForeColor="#333333" />
                                                    <SortedAscendingCellStyle BackColor="#F5F7FB" />
                                                    <SortedAscendingHeaderStyle BackColor="#6D95E1" />
                                                    <SortedDescendingCellStyle BackColor="#E9EBEF" />
                                                    <SortedDescendingHeaderStyle BackColor="#036cb4" />
                                                    <PagerStyle HorizontalAlign="Center" VerticalAlign="Middle" CssClass="pagination" />
                                                </asp:GridView>
                                            </div>

                                        </fieldset>
                                    </td>
                                </tr>
                            </table>
                        </div>
                    </div>
                </asp:View>

                <%-- Compare Frequencies --%>
                <asp:View ID="vwCompare" runat="server">
                    <div id="divQueryforCompare" runat="server" class="divmain">
                        <table>
                            <tr>
                                <th>Compared in: </th>
                                <td style="text-align: center;">
                                    <asp:RadioButtonList ID="rblforCompare" runat="server" RepeatDirection="Horizontal" CssClass="cbl">
                                        <asp:ListItem Value="All" Selected="true">All texts in the corpus</asp:ListItem>
                                        <asp:ListItem Value="Some" Enabled="false">Only selected texts</asp:ListItem>
                                    </asp:RadioButtonList>
                                </td>
                            </tr>
                            <tr>
                                <th>1st KeyWord: </th>
                                <td>
                                    <input type="text" id="txtfreqField1" size="80" placeholder="Type the 1st KeyWord for comparison here. This is a required field" runat="server" class="input-text" /><span style="color: red">*</span></td>
                            </tr>
                            <tr>
                                <th>2nd KeyWord: </th>
                                <td>
                                    <input type="text" id="txtfreqField2" size="80" placeholder="Type the 2nd KeyWord for comparison here. This is a required field" runat="server" class="input-text" /><span style="color: red">*</span></td>
                            </tr>
                            <tr>
                                <th>3rd KeyWord: </th>
                                <td>
                                    <input type="text" id="txtfreqField3" size="80" placeholder="Type the 3rd KeyWord for comparison here,This is not required" runat="server" class="input-text" /></td>
                            </tr>
                            <tr>
                                <td colspan="2" style="text-align: center;">
                                    <asp:Button ID="btnCompared" runat="server" Text="Sumbit" CssClass="outbtndiv-button" />
                                </td>
                            </tr>
                        </table>
                    </div>
                    <div id="divforCompareResult" runat="server" visible="false">
                        <div style="width: 100%; padding: 5px;">
                            <asp:Button ID="btnBackToCompare" runat="server" Text=" Close " CssClass="outbtndiv-button" />
                        </div>
                        <div style="width: 100%; padding: 5px;">
                            <asp:Chart ID="ctForCompare" runat="server" Width="960px" BorderlineDashStyle="Solid" BorderlineColor="Gray" BackGradientStyle="DiagonalLeft" BackSecondaryColor="AliceBlue" BackColor="WhiteSmoke">
                                <Series>
                                    <asp:Series Name="Series1"></asp:Series>
                                </Series>
                                <ChartAreas>
                                    <asp:ChartArea Name="ChartArea1">
                                        <AxisY>
                                            <MajorGrid LineColor="LightSlateGray" LineDashStyle="Dash" />
                                        </AxisY>
                                        <AxisX>
                                            <MajorGrid Enabled="False" />
                                            <LabelStyle Font="Microsoft Sans Serif, 8pt" />
                                        </AxisX>
                                    </asp:ChartArea>
                                </ChartAreas>
                            </asp:Chart>
                        </div>
                    </div>
                </asp:View>

                <%-- WordList --%>
                <asp:View ID="vwWordList" runat="server">

                    <%-- 文本来源选择 --%>
                    <div id="divtxtFrom">
                        <asp:RadioButtonList ID="rbltxtFrom" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow" AutoPostBack="true" CellPadding="10" CellSpacing="5" CssClass="rbl">
                            <asp:ListItem Value="0" Selected="true">Fill Text by Yourself </asp:ListItem>
                            <asp:ListItem Value="1" Enabled="true">Get Text From Corpus </asp:ListItem>
                        </asp:RadioButtonList>
                        <asp:HiddenField ID="hdftxtFrom" Value="0" runat="server" />


                    <%-- 文本来源于个人输入 --%>
                    <div id="divGradedfromshuru" runat="server">
                        <table class="wbtable">
                            <tr>
                                <th>Title：</th>
                                <td>
                                    <input type="text" value="" size="80" style="width: 200px" class="input-text" id="txt_Title" placeholder="Type the title" runat="server" />
                                </td>
                                <th>Author：</th>
                                <td>
                                    <input type="text" value="" size="80" style="width: 200px" class="input-text" id="txt_Author" placeholder="Type the Author's Name" runat="server" />
                                </td>
                                <th>Topic：</th>
                                <td>
                                    <asp:DropDownList ID="ddlTopics" runat="server" Width="200px" CssClass="input-ddl"></asp:DropDownList>
                                </td>
                            </tr>
                            <tr>
                                <th>Text File：</th>
                                <td>
                                    <input type="file" onchange="upload(this)" style="width: 200px;" class="input-file" />
                                </td>
                                <td colspan="4">你可以使用左边的文件控件从文本文件的导入内容，或者你可以在下方文本框中输入或者粘贴要处理的文本!
                                </td>
                            </tr>
                        </table>
                        <%-- 文本框与词汇表选择，及Lemma按钮 --%>
                        <div id="divTexts" runat="server">
                            <div>
                                <textarea cols="100" id="txtcontent" v-model="body" onkeyup="wordStatic(this);" onchange="wordStatic(this);" onblur="wordStatic(this);" runat="server" rows="100" class="ta" placeholder="Type the English text you want to process here !"></textarea>
                            </div>
                            <div id="txtInfo">
                                <table style="width: 100%;">
                                    <tr>
                                        <td style="text-align: left; width: 70%; padding: 5px;">
                                            <span style="color: blue">Limit: 100,000 Words&nbsp;&nbsp;</span>
                                            <span id="mywords" style="display: none;">(<span id="wcount" style="color: red">Entered：0 Words;</span>
                                                <span id="lcount" style="color: green">Remaining: 30,000 Words</span>)
                                            </span>
                                        </td>
                                        <td style="text-align: right; width: 30%; min-width: 200px; padding: 5px;">
                                            <asp:Button ID="clearBtn" runat="server" Text="Clear" CssClass="outbtndiv-button" ToolTip="Clear the Texts" />
                                            <asp:Button ID="btnSubmitForLemma" runat="server" Text="Submit" CssClass="outbtndiv-button" ToolTip="Submit to Process" />
                                        </td>
                                    </tr>
                                </table>
                            </div>
                        </div>
                    </div>

                    <%-- 文本来源于语料库 --%>
                    <div id="divGradedFromCorpus" runat="server" visible="false">
                        <%-- 语料库检索div --%>
                        <div id="divQueryGraded" runat="server">
                            <div style="font-size: 14px;">
                                Select Corpus you need according to:
                            </div>
                            <div>
                                <fieldset class="elemfield">
                                    <legend>
                                        <asp:Label ID="lbgdLevel" runat="server" Text="Level"></asp:Label>
                                        <input id="cbforLevel1" type="checkbox" onchange='checkOrNot("<%=cblgdLevel.ClientID%>", this)' hidden />
                                        <label for="cbforLevel1" class="lglabel">Select All</label>
                                    </legend>
                                    <div class="fieldbox">
                                        <asp:CheckBoxList ID="cblgdLevel" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow" CssClass="cbl">
                                            <asp:ListItem Value="1">L1(freshmen)</asp:ListItem>
                                            <asp:ListItem Value="2">L2(sophomores)</asp:ListItem>
                                            <asp:ListItem Value="3">L3(juniors)</asp:ListItem>
                                            <asp:ListItem Value="4">L4(seniors)</asp:ListItem>
                                        </asp:CheckBoxList>
                                    </div>
                                </fieldset>

                                <fieldset class="elemfield">
                                    <legend>
                                        <asp:Label ID="lbgdTopic" runat="server" Text=" Topic "></asp:Label>
                                        <input id="cbgdforTopic" type="checkbox" onchange='checkOrNot("<%=cblgdTopic.ClientID%>", this)' hidden />
                                        <label for="cbgdforTopic" class="lglabel">Select All</label>
                                    </legend>
                                    <div class="fieldbox">
                                        <asp:CheckBoxList ID="cblgdTopic" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow" CssClass="cbl">
                                            <asp:ListItem Value="1">Culture</asp:ListItem>
                                            <asp:ListItem Value="2">Science & Technology</asp:ListItem>
                                            <asp:ListItem Value="3">Humanity</asp:ListItem>
                                            <asp:ListItem Value="4">Society</asp:ListItem>
                                        </asp:CheckBoxList>
                                    </div>
                                </fieldset>

                                <div id="divgdFLd3" runat="server">
                                    <fieldset class="elemfield">
                                        <legend>
                                            <span>Genre </span>
                                            <input id="cbgdforGenre" type="checkbox" onchange='checkOrNot("<%=cblgdGenre.ClientID%>", this)' hidden />
                                            <label for="cbgdforGenre" class="lglabel">Select All</label>
                                        </legend>
                                        <div class="fieldbox">
                                            <asp:CheckBoxList ID="cblgdGenre" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow" CssClass="cbl">
                                                <asp:ListItem Value="1">Argumentation</asp:ListItem>
                                                <asp:ListItem Value="2">Narration</asp:ListItem>
                                                <asp:ListItem Value="3">Exposition</asp:ListItem>
                                                <asp:ListItem Value="4">Applied Writing</asp:ListItem>
                                            </asp:CheckBoxList>
                                        </div>
                                    </fieldset>
                                </div>
                            </div>

                            <div style="text-align: center; margin: 10px;">
                                <asp:Button ID="btnSubmitforGraded" runat="server" Text="Submit" CssClass="outbtndiv-button" />
                            </div>
                        </div>

                        <div id="divQueryGradedResult" runat="server">
                            <div style="width: 98%; padding: 10px;">
                                <span id="spGradedCount" class="sptips" runat="server"></span>
                            </div>
                            <div style="width: 98%; padding: 10px;">
                                <asp:GridView ID="gvCorpusforGraded" runat="server" AutoGenerateColumns="False" CellPadding="2" ForeColor="#333333" GridLines="None" DataKeyNames="CorpusID" Width="100%" AllowPaging="True" PageSize="10" PagerSettings-Mode="NumericFirstLast">
                                    <AlternatingRowStyle BackColor="White" />
                                    <Columns>
                                        <asp:TemplateField HeaderText="Title">
                                            <ItemTemplate>
                                                <div style="padding: 5px">
                                                    <asp:HiddenField ID="hdfCorpusID" runat="server" Value='<%# Bind("CorpusID")%>' />
                                                    <asp:LinkButton ID="lnkBtn" runat="server" Text='<%# Bind("Title")%>' CommandArgument='<%# Bind("CorpusID")%>' ToolTip="View this text"></asp:LinkButton>
                                                </div>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Text">
                                            <ItemTemplate>
                                                <div style="padding: 5px">
                                                    <asp:Label ID="lbText" runat="server" Text='<%# Bind("OriginalText")%>'></asp:Label>
                                                </div>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                    </Columns>
                                    <EditRowStyle BackColor="#036cb4" />
                                    <FooterStyle Font-Bold="True" />
                                    <HeaderStyle BackColor="#036cb4" Font-Bold="True" ForeColor="White" HorizontalAlign="center" />
                                    <RowStyle BackColor="#EFF3FB" />
                                    <SelectedRowStyle BackColor="#D1DDF1" Font-Bold="True" ForeColor="#333333" />
                                    <SortedAscendingCellStyle BackColor="#F5F7FB" />
                                    <SortedAscendingHeaderStyle BackColor="#6D95E1" />
                                    <SortedDescendingCellStyle BackColor="#E9EBEF" />
                                    <SortedDescendingHeaderStyle BackColor="#036cb4" />
                                    <PagerStyle HorizontalAlign="Center" VerticalAlign="Middle" CssClass="pagination" />
                                </asp:GridView>
                                <asp:Button ID="btnLemmaAll" runat="server" Text="View All" CssClass="outbtndiv-button" Visible="false" />
                            </div>
                        </div>
                    </div>
                    </div>
                </asp:View>

                <%-- 公用的Grading结果界面 --%>
                <asp:View ID="vwLemma" runat="server">
                    <%-- 输出处理结果 --%>
                    <div id="divLemma" runat="server">
                        <div id="divContextInfo" runat="server">
                            <%-- 这是文本的基本信息 --%>
                        </div>
                        <fieldset class="elemfield">
                            <legend>VocabProfile & Wordlist</legend>
                            <div class="divmain">
                                <table>
                                    <tr>
                                        <th>Select Vocabulary：
                                        </th>
                                        <td>
                                            <asp:RadioButtonList ID="rbVBS" runat="server" RepeatDirection="Horizontal" CssClass="cblList" AutoPostBack="true">
                                                <asp:ListItem Value="CECR" Selected="true">CECR</asp:ListItem>
                                                <asp:ListItem Value="GSL" Enabled="false">GSL</asp:ListItem>
                                                <asp:ListItem Value="AWL" Enabled="false">AWL</asp:ListItem>
                                                <asp:ListItem Value="EVP" Enabled="false">EVP</asp:ListItem>
                                            </asp:RadioButtonList>
                                        </td>
                                        <td>
                                            <input id="btnLight" type="button" value="HighLight" data-i="1" onclick="HighLightAll(this)" class="outbtndiv-button" />

                                        </td>
                                        <td>
                                            <asp:Button ID="btnBackLemma" runat="server" Text="Back" CssClass="outbtndiv-button" ToolTip="关闭页面" />
                                        </td>
                                    </tr>
                                </table>
                            </div>
                            <div class="divmain">
                                <table>
                                    <tr>
                                        <td style="vertical-align: top">
                                            <fieldset class="elemfield">
                                                <legend>VocabProfile</legend>
                                                <table>
                                                    <tr>
                                                        <td style="vertical-align: top">
                                                            <div id="divContextHighLight" class="it-ec-textdiv" runat="server">
                                                                <%-- 这是输出的彩色文本以及各个级别单词占比 --%>
                                                            </div>
                                                        </td>
                                                        <td style="vertical-align: top">
                                                            <div class="it-ec-statsdiv" id="divstats">
                                                                <%-- 单词颜色标记说明与占比 --%>
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
                                                        </td>

                                                    </tr>
                                                </table>
                                            </fieldset>
                                        </td>
                                        <td style="vertical-align: top">
                                            <fieldset class="elemfield">
                                                <legend>WordList
                                                </legend>
                                                <div style="height: 400px; min-width: 300px; width: 100%; overflow-y: auto;">
                                                    <asp:GridView ID="gvWordList" runat="server" AutoGenerateColumns="False" CellPadding="2" ForeColor="#333333" GridLines="None" DataKeyNames="Cluster">
                                                        <Columns>
                                                            <asp:TemplateField HeaderText="Word">
                                                                <ItemTemplate>
                                                                    <div style="padding: 0 5px; min-width: 125px">
                                                                        <asp:Label ID="lbCluster" runat="server" Text='<%# Bind("Cluster")%>'></asp:Label>
                                                                    </div>
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                            <asp:TemplateField HeaderText="Frequence">
                                                                <ItemTemplate>
                                                                    <div style="padding: 0 5px; min-width: 125px">
                                                                        <asp:Label ID="lbTotal" runat="server" Text='<%# Bind("Count")%>'></asp:Label>
                                                                    </div>
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                        </Columns>
                                                        <EditRowStyle BackColor="#036cb4" />
                                                        <FooterStyle Font-Bold="True" Height="25px" />
                                                        <HeaderStyle BackColor="#036cb4" Font-Bold="True" ForeColor="White" HorizontalAlign="center" Height="25px" />
                                                        <RowStyle BackColor="#EFF3FB" />
                                                        <SelectedRowStyle BackColor="#D1DDF1" Font-Bold="True" ForeColor="#333333" />
                                                        <SortedAscendingCellStyle BackColor="#F5F7FB" />
                                                        <SortedAscendingHeaderStyle BackColor="#6D95E1" />
                                                        <SortedDescendingCellStyle BackColor="#E9EBEF" />
                                                        <SortedDescendingHeaderStyle BackColor="#036cb4" />
                                                        <PagerStyle HorizontalAlign="Center" VerticalAlign="Middle" CssClass="pagination" />

                                                    </asp:GridView>
                                                </div>
                                            </fieldset>
                                        </td>
                                    </tr>
                                </table>


                            </div>
                            <div class="divbr"></div>
                        </fieldset>
                    </div>
                </asp:View>
            </asp:MultiView>
        </div>
    </div>
    <asp:HiddenField ID="hdfvwIndex" runat="server" Value="0" />
    <asp:Label ID="lbErr" runat="server" Text="" ForeColor="red"></asp:Label>

    <div id="divLoadingBG" style="display: none;"></div>
    <div id="divLoading" style="display: none;">
        <img src="../images/loading.gif" />
    </div>
</asp:Content>

<asp:Content ID="PageTitle" ContentPlaceHolderID="PlaceHolderPageTitle" runat="server">
    NEU English Corpus
</asp:Content>

<asp:Content ID="PageTitleInTitleArea" ContentPlaceHolderID="PlaceHolderPageTitleInTitleArea" runat="server">
    <a href="/NEU_EC/">NEU English Corpus</a> >
	<asp:HyperLink ID="hlinkPageTitle1" runat="server" NavigateUrl="neulc.aspx?cp=neulc">NEULC</asp:HyperLink>
    <asp:Label runat="server" ID="Titlelb"></asp:Label>
</asp:Content>
