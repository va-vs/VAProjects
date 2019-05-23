﻿<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Import Namespace="Microsoft.SharePoint.ApplicationPages" %>
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="neulc.aspx.cs" Inherits="FSCAppPages.Layouts.FSCAppPages.Corpus.neulc" DynamicMasterPageFile="~masterurl/default.master" EnableEventValidation="false" %>

<asp:Content ID="PageHead" ContentPlaceHolderID="PlaceHolderAdditionalPageHead" runat="server">
	<link rel="stylesheet" href="../css/stylelc.css" type="text/css" charset="utf-8"  />
	<link rel="stylesheet" href="../css/tab.css"  type="text/css" charset="utf-8" />
	<script type="text/javascript">
		function Check_Uncheck_All() {
			var cbl0 = document.getElementById("<%=cblGenre.ClientID%>");
		   var input = cbl0.getElementsByTagName("input");
		   for (var i = 0; i < input.length; i++) {
			   input[i].checked = false;
		   }
		   var cbl1 = document.getElementById("<%=cblGrade.ClientID%>");
		   var input = cbl1.getElementsByTagName("input");
		   for (var i = 0; i < input.length; i++) {
			   input[i].checked = false;
		   }
		   var cbl2 = document.getElementById("<%=cblTopic.ClientID%>");
			var input = cbl2.getElementsByTagName("input");
			for (var i = 0; i < input.length; i++) {
				input[i].checked = false;
			}
			//if (cb.checked) {
			//    for (var i = 0; i < input.length; i++) {
			//        input[i].checked = true;
			//    }
			//}
			//else {
			//    for (var i = 0; i < input.length; i++) {
			//        input[i].checked = false;
			//    }
			//}
		}

		function HighLightthis(val) {
			var setValue = val.getAttribute("data-grade");
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
	<div class="divneulc">
		<table border="0" class="neulctable">
			<tr>
				<td>
					<ul class="imgul">
						<li>
							<a><span style="font-size: 18px; font-weight: bold;">NEULC</span></a>
						</li>
						<li>
							<a href="#">
								<img alt="About NEULC" src="../images/Info.png" width="20" height="20" /></a>
						</li>
						<li>
							<a href="#">
								<img alt="Download" src="../images/Download.png" height="20" /></a>
						</li>
						<li>
							<a href="admin.aspx">
								<img alt="Upload" src="../images/Upload.png" height="20" /></a>
						</li>
						<li>
							<a href="#">
								<img alt="Share" src="../images/Share.png" height="20" /></a>
						</li>
						<li>
							<a href="#">
								<img alt="Guide" src="../images/Guide.png" height="20" /></a>
						</li>
					</ul>
				</td>
				<td style="text-align: right;">
					<ul class="imgul">
						<li><a>Login</a></li>
						<li><a>Contact</a></li>
					</ul>
				</td>
			</tr>
			<tr>
				<td colspan="2" style="border-top: 1px #808080 solid; border-bottom: 1px #808080 solid;">
					<div>
						<asp:Menu ID="muNeulc" runat="server" Orientation="Horizontal" DynamicHorizontalOffset="2" StaticDisplayLevels="1" CssClass="mnuTopMenu" Width="801px" StaticSubMenuIndent="10px" DisappearAfter="600">
							<Items>
								<asp:MenuItem Text="Corpus" Value="0"></asp:MenuItem>
								<asp:MenuItem Text="Concordance" Value="1"></asp:MenuItem>
								<asp:MenuItem Text="Collocate" Value="2"></asp:MenuItem>
								<asp:MenuItem Text="WordList" Value="3"></asp:MenuItem>
								<asp:MenuItem Text="Cluster" Value="4"></asp:MenuItem>
							</Items>
							<StaticSelectedStyle Font-Bold="true" HorizontalPadding="18" VerticalPadding="4" BorderStyle="Solid" BorderWidth="1px" BorderColor="#808080" />
							<StaticMenuItemStyle HorizontalPadding="18" VerticalPadding="4" />
							<StaticHoverStyle ForeColor="red" />
							<DynamicMenuStyle HorizontalPadding="18" VerticalPadding="4" />
							<DynamicSelectedStyle BackColor="#808080" Font-Bold="true" />
							<DynamicMenuItemStyle BackColor="#ecf6ff" HorizontalPadding="18" VerticalPadding="4" />
							<DynamicHoverStyle ForeColor="red" />
						</asp:Menu>
					</div>
				</td>
			</tr>
			<tr>
				<td colspan="2">
					<div style="padding: 10px;">
						<asp:MultiView ID="mvNeulc" runat="server">
							<%-- 检索 --%>
							<asp:View ID="vwQuery" runat="server">
								<table style="border: none; line-height: 25px;" border="0">
									<tr>
										<td colspan="2">Select files you need according to:</td>
									</tr>
									<tr>
										<td style="text-align: right">Grade:</td>
										<td>
											<asp:CheckBoxList ID="cblGrade" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow" CssClass="cblList" RepeatColumns="6">
												<asp:ListItem Value="1">F1(freshmen)</asp:ListItem>
												<asp:ListItem Value="2">S2(sophomores)</asp:ListItem>
												<asp:ListItem Value="3">J3(juniors)</asp:ListItem>
												<asp:ListItem Value="4">S4(seniors)</asp:ListItem>
											</asp:CheckBoxList>
										</td>
									</tr>
									<tr>
										<td style="text-align: right">Genre:</td>
										<td>
											<asp:CheckBoxList ID="cblGenre" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow" CssClass="cblList">
												<asp:ListItem Value="1">Argumentation</asp:ListItem>
												<asp:ListItem Value="2">Narration</asp:ListItem>
												<asp:ListItem Value="3">Exposition</asp:ListItem>
												<asp:ListItem Value="4">Applied Writing</asp:ListItem>
											</asp:CheckBoxList>
										</td>
									</tr>
									<tr>
										<td style="text-align: right">Topic:</td>
										<td>
											<asp:CheckBoxList ID="cblTopic" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow" CssClass="cblList">
												<asp:ListItem Value="1">Culture</asp:ListItem>
												<asp:ListItem Value="2">Science & Technology</asp:ListItem>
												<asp:ListItem Value="3">Humanity</asp:ListItem>
												<asp:ListItem Value="4">Society</asp:ListItem>
											</asp:CheckBoxList>
										</td>
									</tr>
									<tr>
										<td colspan="2" style="text-align: center; padding-top: 10px;">
											<asp:Button ID="btnSubmitforCorpus" runat="server" Text="Submit" CssClass="outbtndiv-button" />&nbsp;&nbsp;
								<asp:Button ID="btnResetforCorpus" runat="server" Text="Reset" OnClientClick="Check_Uncheck_All()" CssClass="outbtndiv-button" />
										</td>
									</tr>
								</table>
								<div id="divforCorpusResult" runat="server" visible="false">

									<div id="tab">
										<h3 class="up" id="two1" onmouseover="setContentTab('two',1,3)">Summary by Grade</h3>
										<div class="block" id="con_two_1">
											<asp:Table ID="tbforGrade" runat="server" CssClass="mytable"></asp:Table>
										</div>
										<h3 id="two2" onmouseover="setContentTab('two',2,3)">Summary by Topic</h3>
										<div id="con_two_2">
											<asp:Table ID="tbforTopic" runat="server" CssClass="mytable"></asp:Table>
										</div>
										<h3 id="two3" onmouseover="setContentTab('two',3,3)">Summary by Genre</h3>
										<div id="con_two_3">
											<asp:Table ID="tbforGenre" runat="server" CssClass="mytable"></asp:Table>
										</div>
									</div>

									<script type="text/javascript">
										function setContentTab(name, curr, n) {
											for (i = 1; i <= n; i++) {
												var menu = document.getElementById(name + i);
												var cont = document.getElementById("con_" + name + "_" + i);
												menu.className = i == curr ? "up" : "";
												if (i == curr) {
													cont.style.display = "block";
													cont.className="block";
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
								<h2>Concordance</h2>
							</asp:View>

							<%-- Collocate --%>
							<asp:View ID="vwCollocate" runat="server">
								<h2>Collocate</h2>
							</asp:View>

							<%-- WordList --%>
							<asp:View ID="vwWordList" runat="server">

								<%-- 输入文档 --%>
								<div id="inputDiv" runat="server">
									<div id="divtxtFrom">
										<asp:RadioButtonList ID="rbltxtFrom" runat="server" RepeatDirection="Horizontal" RepeatLayout="Table" AutoPostBack="true" CellPadding="10" CellSpacing="5" CssClass="cblList">
											<asp:ListItem Value="0" Enabled="false">Get Text From Corpus </asp:ListItem>
											<asp:ListItem Value="1" Selected="true">Fill Text by Yourself </asp:ListItem>
										</asp:RadioButtonList>
									</div>
									<div id="divFromCorpus" runat="server" class="wbdiv">
										<asp:GridView ID="gvCorpusforWordList" runat="server"></asp:GridView>
										<input type="text" class="input-text" value="" id="txtKeyWordsforWordlist" style="width: 300px; height: 25px; border: 1px solid #808080; border-radius: 5px 5px 5px 5px;" placeholder="Type the KeyWords" runat="server" title="Type the KeyWords" />
										<asp:Button ID="btnQueryforWordlist" runat="server" Text=" Query " CssClass="outbtndiv-button" Enabled="false" />
									</div>
									<div class="wbdiv" id="divfromshuru" runat="server" visible="false">
										<table class="wbtable">
											<tr>
												<th>Text Title：</th>
												<td>
													<input type="text" value="" class="input-text" style="width: 300px; height: 25px; border: 1px solid #808080; border-radius: 5px 5px 5px 5px;" id="txt_Title" placeholder="Type the title" runat="server" />
												</td>
												<th>Your Name：</th>
												<td>
													<input id="username" type="text" class="input-text" title="Type Your Name" placeholder="Type Your Name" runat="server" style="height: 25px; border: 1px solid #808080; border-radius: 5px 5px 5px 5px;" />
												</td>
											</tr>

											<tr>
												<th>Select Text File：
												</th>
												<td>
													<input type="file" onchange="upload(this)" />
												</td>
												<th>Select WordList：
												</th>
												<td>
													<asp:RadioButtonList ID="rbVBS" runat="server" RepeatDirection="Horizontal" CssClass="cblList">
														<asp:ListItem Value="0" Selected="true">教学大纲</asp:ListItem>
														<asp:ListItem Value="1" Enabled="false">GSL</asp:ListItem>
														<asp:ListItem Value="2" Enabled="false">AWL</asp:ListItem>
														<asp:ListItem Value="3" Enabled="false">EVP</asp:ListItem>
													</asp:RadioButtonList>
												</td>
											</tr>
										</table>
									</div>
									<div id="divTexts" class="wbdiv" runat="server" visible="false">
										<textarea cols="100" id="txtcontent" v-model="body" onkeyup="wordStatic(this);" onchange="wordStatic(this);" onblur="wordStatic(this);" runat="server" rows="100" class="ta" placeholder="Please Type or paste your text here and click the submit button below!"></textarea>
										<table style="width: 98%">
											<tr>
												<td style="text-align: left; padding: 5px;">
													<div>
														(<span style="color: blue">Limit: 100,000 Words</span>)
														&nbsp;&nbsp;
														<span id="mywords" style="display: none;">
															<span id="wcount" style="color: red">Entered：0 Words;</span>
															<span id="lcount" style="color: green">Remaining: 30,000 Words</span>
														</span>
													</div>
												</td>
												<td style="text-align: right; padding-right: 10px;">
													<asp:Button ID="clearBtn" runat="server" Text="Clear" CssClass="outbtndiv-button" ToolTip="Clear the Texts" />
													<asp:Button ID="lemmanew" runat="server" Text="Submit" CssClass="outbtndiv-button" ToolTip="Submit to Process" />
												</td>
											</tr>
										</table>
									</div>
								</div>

								<%-- 输出结果 --%>
								<div id="outputDiv" runat="server" visible="false" class="wbdiv">

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
												<dt class="it-chart-dt" data-grade="UN" onclick="HighLightthis(this)">忽略处理</dt>
												<dd class="it-chart-dd" data-grade="UN" onclick="HighLightthis(this)">
													<div class="it-chart-bar" style="background-color: grey; width: 11.1588%;"></div>
													<div class="it-chart-label">26(11.16%)</div>
												</dd>
												<dt class="it-chart-dt" data-grade="C1" onclick="HighLightthis(this)">未处理</dt>
												<dd class="it-chart-dd" data-grade="C1" onclick="HighLightthis(this)">
													<div class="it-chart-bar" style="background-color: orange; width: 2.5751%;"></div>
													<div class="it-chart-label">6(2.58%)</div>
												</dd>
												<dt class="it-chart-dt" data-grade="C2" onclick="HighLightthis(this)">超纲词汇</dt>
												<dd class="it-chart-dd" data-grade="C2" onclick="HighLightthis(this)">
													<div class="it-chart-bar" style="background-color: red; width: 1.7167%;"></div>
													<div class="it-chart-label">4(1.72%)</div>
												</dd>
												<dt class="it-chart-dt" data-grade="A1" onclick="HighLightthis(this)">高中大纲</dt>
												<dd class="it-chart-dd" data-grade="A1" onclick="HighLightthis(this)">
													<div class="it-chart-bar" style="background-color: indigo; width: 99.8283%; max-width: unset"></div>
													<div class="it-chart-label">186(88.83%)</div>
												</dd>
												<dt class="it-chart-dt" data-grade="A2" onclick="HighLightthis(this)">基本要求</dt>
												<dd class="it-chart-dd" data-grade="A2" onclick="HighLightthis(this)">
													<div class="it-chart-bar" style="background-color: blue; width: 3.4335%;"></div>
													<div class="it-chart-label">8(3.43%)</div>
												</dd>
												<dt class="it-chart-dt" data-grade="B1" onclick="HighLightthis(this)">较高要求</dt>
												<dd class="it-chart-dd" data-grade="B1" onclick="HighLightthis(this)">
													<div class="it-chart-bar" style="background-color: green; width: 1.2876%;"></div>
													<div class="it-chart-label">3(1.29%)</div>
												</dd>
												<dt class="it-chart-dt" data-grade="B2" onclick="HighLightthis(this)">更高要求</dt>
												<dd class="it-chart-dd" data-grade="B2" onclick="HighLightthis(this)">
													<div class="it-chart-bar" style="background-color: yellow; width: 0%;"></div>
													<div class="it-chart-label">0(0.00%)</div>
												</dd>
											</dl>
										</div>
									</div>
									<div class="divbr"></div>
								</div>
							</asp:View>

							<%-- Cluster --%>
							<asp:View ID="vwCluster" runat="server">
								<h2>Cluster</h2>
							</asp:View>
						</asp:MultiView>

					</div>
				</td>
			</tr>
		</table>
	</div>

	<asp:Label ID="lbErr" runat="server" Text="" ForeColor="red"></asp:Label>
	 <hr style="margin-top:100px;"/>
		  <ul class="ext_link">
			<li><a href="https://www.english-corpora.org/bnc/" target="_blank">BNC</a></li>
			<li><a href="https://www.english-corpora.org/coca/" target="_blank">COCA</a></li>
			<li><a href="http://corpus.leeds.ac.uk/itweb/htdocs/Query.html#" target="_blank">IntelliText</a></li>
			<li><a href="https://www.lextutor.ca/vp/eng/" target="_blank">Lextutor</a></li>
			<li><a href="http://www.natcorp.ox.ac.uk/corpus/creating.xml" target="_blank">Natcorp</a></li>
		  </ul>
</asp:Content>

<asp:Content ID="PageTitle" ContentPlaceHolderID="PlaceHolderPageTitle" runat="server">
NEULC
</asp:Content>

<asp:Content ID="PageTitleInTitleArea" ContentPlaceHolderID="PlaceHolderPageTitleInTitleArea" runat="server" >
NEU English Corpus > NEULC <asp:Label runat="server" ID="Titlelb"></asp:Label>
</asp:Content>
