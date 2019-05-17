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
	<style type="text/css">
		.div {
			width: 100%;
			max-width: 800px;
		}

			.div table {
				width: 100%;
				border: 1px #808080 solid;
			}

		.textul {
			list-style: none; /* 去掉ul前面的符号 */
			margin: 0px; /* 与外界元素的距离为0 */
			padding: 0px; /* 与内部元素的距离为0 */
			width: auto; /* 宽度根据元素内容调整 */
		}

			.textul li {
				float: left; /* 向左漂移，将竖排变为横排 */
				border-right: 1px #808080 solid;
			}
				/* 所有class为menu的div中的ul中的a样式(包括尚未点击的和点击过的样式) */
				.textul li a, .textul li a:visited {
					/*background-color: #bfcbd6;
							border: 1px #4e667d solid;
							color: #465c71;*/
					display: block; /* 此元素将显示为块级元素，此元素前后会带有换行符 */
					line-height: 1.35em;
					padding: 4px 20px;
					text-decoration: none;
					white-space: nowrap;
				}
					/* 所有class为menu的div中的ul中的a样式(鼠标移动到元素中的样式) */
					.textul li a:hover {
						background-color: #eeeeee;
						color: #ff6a00;
						text-decoration: none;
					}
					/* 所有class为menu的div中的ul中的a样式(鼠标点击元素时的样式) */
					.textul li a:active {
						background-color: #4694DE; /* 背景色 */
						color: #cfdbe6; /* 文字颜色 */
						text-decoration: none; /* 不显示超链接下划线 */
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
	</style>
	<style type="text/css">
		.mnuTopMenu {
			font-family: 宋体;
			font-size: 16px;
			color: #1E5494;
		}

		.DynamicMenuStyle /*动态菜单矩形区域样式*/ {
			padding:5px;
			text-align: left;
			font-family: 宋体;
			font-size: 14px;
		}

		.DynamicHoverStyle /*动态菜单项:鼠标悬停时的样式*/ {
			background-color: #ecf6ff; /*#7C6F57;*/
			color: #333333;
		}

		.DynamicSelectedStyle /*动态菜单项:选择时的样式*/ {
			color: red;
		}

		.DynamicMenuItemStyle /*动态菜单项样式*/ {
			padding:5px;
			color: #000000;
			background-color: #ecf6ff;
			font-family: 宋体;
			font-size: 14px;
		}


		.StaticSelectedStyle /*静态菜单项:选择时的样式*/ {
			color: red;
			font-weight:bold;
		}

		.StaticMenuItemStyle /*静态菜单项样式*/ {
			cursor: pointer;
			padding: 5px;
			color: #1E5494;
		}

		.StaticHoverStyle /*静态菜单项:鼠标悬停时的样式*/ {
			background-color: #FFCC66; /*#7C6F57;*/
			cursor: pointer;
			color: #1E5494;
		}
	</style>
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
	</script>
        <link rel="stylesheet" href="../css/stylelc.css" media="screen" type="text/css" />
        <!-- #region 全局样式表neulc -->
    <style type="text/css">/*全局样式表*/
         .yuanjiao{
             font-family: Arial;
             border: 1px solid #379082;
             border-radius:5px 5px 5px 5px;
             background-color: white;
             margin:10px;
         }
        .yuanjiaobtn{
            font-family: Arial;
            border: 1px solid #379082;
            border-radius:5px 5px 5px 5px;
            background-color:#999999;
            cursor: pointer;
            font-size: 16px;
            font-weight: bold;
            color: blue;
        }
        .hoverTable{
            width:900px;
            border-collapse:collapse;
        }
        /* rm css */
        .rm_mainContent{
            width:1000px;
            background-color:#e7e9e8;
            padding-left:5px;
        }
        .hoverTable td{
            padding:5px; border:none;
        }
        /* Define the default color for all the table rows */
        .hoverTable tr{
            background: #e9e7e7;
        }
        /* Define the hover highlight color for the table row */
        /*.hoverTable tr:hover {
              background-color: #ffff99;
        }*/
        .resultDiv {
            width: 800px;
            height: 200px;
            overflow-x: hidden;
            padding: 10px;
            -moz-border-radius: 15px;
            -webkit-border-radius: 15px;
            border-radius: 15px;
        }
          .ta{
    	    -moz-box-shadow:1px 1px 0 #E7E7E7;
            -moz-box-sizing:border-box;
            border-color:#CCCCCC #999999 #999999 #CCCCCC;
            border-style:solid;
            border-width:1px;
            font-family:arial,sans-serif;
            font-size:13px;
            height:280px;
            margin:10px auto;
            outline-color:-moz-use-text-color;
            outline-style:none;
            outline-width:medium;
            padding:2px;
            width:980px;
            border-radius:5px 5px 5px 5px;
              margin: 4px 4px 4px 4px;
          }
        .FileUpload
        {

        border: solid 1px #999999;
        background-color: #ffffff;
        background-image: none;
        margin-top:4px;
        width:250px;
        margin-right:16px;
        }
        .fakeContainer {
            float: left;
            margin: 20px;
            border: none;
            width: 640px;
            height: 320px;
	        background-color: #ffffff;
            overflow: hidden;
        }
    </style>
    <!-- #endregion -->
    <style type="text/css">
   /******输出结果三个tab样式表******/
        .content {padding:5px;}
        .tabs {
            width: 600px;
            float: none;
            list-style: none;
            position: relative;
            text-align: left;
        }
        .tabs li {
            float: left;
            display: block;
        }
        .tabs input[type="radio"] {
            position: absolute;
            top: -9999px;
            left: -9999px;
        }
        .tabs label {
            display: block;
            padding: 5px 20px;
            border-radius: 15px 15px 0 0;
            font-size: 20px;
            font-weight: bold;
            background:#424242;
            color: #ffffff;
            cursor: pointer;
            position: relative;
            top: 10px;
            -webkit-transition: all 0.2s ease-in-out;
            -moz-transition: all 0.2s ease-in-out;
            -o-transition: all 0.2s ease-in-out;
            transition: all 0.2s ease-in-out;
        }
        .tabs label:hover {
            background:#cccccc;
            color:royalblue;
        }
        .tabs .tab-content {
            z-index: 2;
            display: none;
            overflow: hidden;
            width: 1000px;
            height: 500px;
            font-size: 15px;
            line-height:20px;
            padding: 5px;
            position: absolute;
            top: 45px;
            left: 0;
            background: #cccccc;
        }
        .tabs [id^="tab"]:checked + label {
            top: 0;
            padding-top: 15px;
            background:#cccccc;
            color:blue ;
        }
        .tabs [id^="tab"]:checked ~ [id^="tab-content"] {
            display: block;
        }

        /**Title选择样式**/
        .demo{width:400px;margin:5px;float:left;}
        /*.demo div{height:30px;width:400px;}*/
    </style>
   
    <script type="text/javascript">
        //兼容火狐、IE8
        //显示遮罩层
        function showMask() {
            $("#mask").css("height", $(document).height());
            $("#mask").css("width", $(document).width());
            $("#mask").show();
        }
        //隐藏遮罩层
        function hideMask() {

            $("#mask").hide();
        }

</script>
    <style type="text/css">
        .mask {
            position: absolute;
            top: 0px;
            filter: alpha(opacity=60);
            background-color: #777;
            z-index: 1002;
            left: 0px;
            opacity:0.5; -moz-opacity:0.5;

        }
</style>
</asp:Content>

<asp:Content ID="Main" ContentPlaceHolderID="PlaceHolderMain" runat="server">
	<div class="div">
		<table border="0">
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
							<a href="#">
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
						<asp:Menu ID="menu01" runat="server" Orientation="Horizontal" DynamicHorizontalOffset="2" StaticDisplayLevels="1" CssClass="mnuTopMenu" Width="801px" StaticSubMenuIndent="10px" DisappearAfter="600" >
							<StaticSelectedStyle BackColor="#fff" Font-Bold="true"/>
							<StaticMenuItemStyle BorderStyle="Solid" BorderWidth="1px" BorderColor="#D9D9E6" />
							<StaticHoverStyle ForeColor="red" Font-Bold="true" />
							<DynamicMenuStyle BorderStyle="Solid" BorderWidth="1px" BorderColor="#D9D9E6" />
							<DynamicSelectedStyle BackColor="#fff" Font-Bold="true" />
							<DynamicMenuItemStyle BackColor="#ecf6ff" BorderStyle="Solid" BorderWidth="1px" BorderColor="#D9D9E6" />
							<DynamicHoverStyle ForeColor="red"  Font-Bold="true" />
							<Items>
								<asp:MenuItem Text="Corpus" Value="0"></asp:MenuItem>
								<asp:MenuItem Text="Concordance" Value="1"></asp:MenuItem>
								<asp:MenuItem Text="Collocate" Value="2"></asp:MenuItem>
								<asp:MenuItem Text="WordList" Value="3"></asp:MenuItem>
								<asp:MenuItem Text="Cluster" Value="4"></asp:MenuItem>
							</Items>
						</asp:Menu>
					</div>
				</td>
			</tr>
			<tr>
				<td colspan="2" style="">
					<div style="padding: 10px;">
						<asp:MultiView ID="mv1" runat="server">
							<asp:View ID="View0" runat="server">
								<table style="border: none; line-height: 25px;" border="0">
									<tr>
										<td colspan="2">Select files you need according to:
										</td>
									</tr>
									<tr>
										<td style="text-align: right">Grade:
										</td>
										<td>
											<asp:CheckBoxList ID="cblGrade" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow">
												<asp:ListItem Value="1">F1</asp:ListItem>
												<asp:ListItem Value="2">S2</asp:ListItem>
												<asp:ListItem Value="3">J3</asp:ListItem>
												<asp:ListItem Value="4">S4</asp:ListItem>
											</asp:CheckBoxList>
										</td>
									</tr>
									<tr>
										<td style="text-align: right">Genre:
										</td>
										<td>
											<asp:CheckBoxList ID="cblGenre" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow">
												<asp:ListItem Value="1">Argumentation</asp:ListItem>
												<asp:ListItem Value="2">Narration</asp:ListItem>
												<asp:ListItem Value="3">Exposition</asp:ListItem>
												<asp:ListItem Value="4">Applied Writing</asp:ListItem>
											</asp:CheckBoxList>
										</td>
									</tr>
									<tr>
										<td style="text-align: right">Topic:
										</td>
										<td>
											<asp:CheckBoxList ID="cblTopic" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow">
												<asp:ListItem Value="1">Culture</asp:ListItem>
												<asp:ListItem Value="2">Science & Technology</asp:ListItem>
												<asp:ListItem Value="3">Humanity</asp:ListItem>
												<asp:ListItem Value="4">Society</asp:ListItem>
											</asp:CheckBoxList>
										</td>
									</tr>
									<tr>
										<td colspan="2" style="text-align: center; padding-top: 10px;">
											<asp:Button ID="btnSubmit" runat="server" Text="Submit" />&nbsp;&nbsp;
								<asp:Button ID="btnReset" runat="server" Text="Reset" OnClientClick="Check_Uncheck_All()" />
										</td>
									</tr>
								</table>

							</asp:View>
							<asp:View ID="View1" runat="server">
								<h2>Concordance</h2>
								<asp:Button ID="btnChange" runat="server" Text="关灯" />
								<asp:Label ID="lbAA" runat="server" Text="现在是开灯状态"></asp:Label>
							</asp:View>
							<asp:View ID="View2" runat="server">
								<h2>Collocate</h2>
							</asp:View>
							<asp:View ID="View3" runat="server">
								<h2>WordList</h2>
								<hr />
								<%-- 遮罩层--%>
								<div id="mask" class="mask" style="display: none;">
									<div style="text-align: center; vertical-align: middle; height: 100%; width: 100%">
										<img src="./images/loading.gif" />
									</div>
								</div>

								<%-- 输入文档 --%>
								<div id="inputDiv" class="rm_mainContent" runat="server">
									<div class="demo">
										<div style="margin-left: 4px;">
											<table>
												<tr>
													<td>Title：</td>
													<td>
														<input type="text" value="" class="yuanjiao" style="width: 600px; height: 25px" id="homecity_name"
															name="homecity_name" mod="address|notice" mod_address_source="hotel" mod_address_suggest=""
															mod_address_reference="cityid" mod_notice_tip="Type the title or click to choose it"
															runat="server" title="Type the title or click to choose it" />
														<input id="cityid" name="cityid" type="hidden" value="{$cityid}" />
													</td>
													<td>User:</td>
													<td>
														<input id="username" type="text" class="yuanjiao" style="width: 240px; height: 25px;" title="Type Your Name" placeholder="Type Your Name" runat="server" />
														<%--<asp:TextBox ID="usertb" runat="server" Width="250px" Height="25px" CssClass="yuanjiao" ToolTip=""></asp:TextBox>--%>
													</td>
												</tr>
											</table>
										</div>
									</div>
									<div id="jsContainer" class="jsContainer" style="height: 0">
										<div id="tuna_alert" style="display: none; position: absolute; z-index: 999; overflow: hidden;"></div>
										<div id="tuna_jmpinfo" style="visibility: hidden; position: absolute; z-index: 120;"></div>
									</div>
									<script type="text/javascript" src="../js/fixdiv.js"></script>
									<script type="text/javascript" src="../js/address.js"></script>
									<asp:TextBox ID="titletb" runat="server" Width="400px" Height="25px" CssClass="yuanjiao" Visible="false"></asp:TextBox>
									<asp:RequiredFieldValidator ID="rftitle" runat="server" ErrorMessage="*(必填)" ControlToValidate="titletb" ForeColor="#FF3300" Visible="false"></asp:RequiredFieldValidator>
									<div>
										<table width="100%" style="margin-left: 10px;">
											<tr>
												<td style="width: 40%; text-align: left">
													<span style="font-weight: bold; font-size: 18px;">Texts:   &nbsp;&nbsp;</span>
													<asp:FileUpload ID="txtUpload" runat="server" Width="200px" CssClass="yuanjiaobtn" Style="width: 200px; border: 0; height: 30px; line-height: 30px;" />
													<asp:Button ID="txtImport" runat="server" Text="Import" OnClick="txtImport_Click"
														Width="68px" CssClass="yuanjiaobtn" />

												</td>
												<td style="width: 20%; text-align: right">Choose Vocabulary：

												</td>
												<td style="width: 40%; text-align: left">
													<asp:CheckBoxList ID="cblist" runat="server" ToolTip="Choose Vocabulary" RepeatDirection="Horizontal" Width="400px" OnSelectedIndexChanged="cblist_SelectedIndexChanged" AutoPostBack="true">
														<asp:ListItem Value="0">高中大纲</asp:ListItem>
														<asp:ListItem Value="1">基本要求</asp:ListItem>
														<asp:ListItem Value="2">较高要求</asp:ListItem>
														<asp:ListItem Value="3">更高要求</asp:ListItem>
													</asp:CheckBoxList>

												</td>
											</tr>
											<%--<tr style="text-align:right;"><asp:CheckBox ID="ckEurope" Text="是否排查欧框词汇" runat="server" /></tr>--%>
										</table>

										<%--<input type="file" id="btnFile" onpropertychange="txtFoo.value=this.value;" style="visibility:hidden;" />
			<input type="text" id="txtFoo" style="visibility:hidden" />
			<input type="button" onclick="btnFile.click()" value="选择文件" class="yuanjiaobtn"/>--%>

										<%-- <input type="button" name="button" title="粘贴到文本框" value="来自粘贴板" onclick="javascript: mypast();" class="yuanjiaobtn" />--%>

										<%--<asp:TextBox ID="txtsBox" runat="server" Height="280px" TextMode="MultiLine" Width="960px" CssClass="yuanjiao" ToolTip="Please Type or paste your text here and click the submit button below!"
				></asp:TextBox>   --%>
										<textarea id="txtcontent" onkeyup="wordStatic(this);" onchange="wordStatic(this);" onblur="wordStatic(this);" runat="server" rows="100" cols="80" class="ta" placeholder="Please Type or paste your text here and click the submit button below!"></textarea>
									</div>
									<table width="100%">
										<tr>
											<td style="text-align: left">
												<div>
													(<span style="color: blue">Limit: 100,000 Words</span>)<span id="mywords" style="display: none;">&nbsp;&nbsp;<span
														id="wcount" style="color: red">Entered：0 Words;</span>  <span id="lcount" style="color: green">Remaining: 30,000 Words</span></span>
												</div>
												<script type="text/javascript">
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
											</td>
											<td style="text-align: right; margin-right: 40px; padding-right: 20px;">
												<asp:Button ID="clearBtn" runat="server" Text="Clear" OnClick="clearBtn_OnClick" CssClass="yuanjiaobtn" ToolTip="Clear the Texts" />
												<asp:Button ID="lemmanew" runat="server" Text="Submit" OnClick="lemmanew_Click" CssClass="yuanjiaobtn" ToolTip="Submit to Process" />
											</td>
										</tr>
									</table>

								</div>

								<%-- 输出结果 --%>
								<div id="outputDiv" runat="server" visible="false" style="padding-left: 5px; width: 1000px;">
									<div style="float: right; margin-right: 20px;">
										<asp:Button ID="backBtn" runat="server" Text="返回" OnClick="backBtn_OnClickBtn_OnClick" CssClass="yuanjiaobtn" ToolTip="返回上次操作" />
										<asp:Button ID="closeBtn" runat="server" Text="关闭" OnClick="closeBtn_OnClick" CssClass="yuanjiaobtn" ToolTip="关闭页面" />
									</div>
									<div class="content">
										<ul class="tabs">

											<li>
												<%--词汇列表--%>
												<input type="radio" name="tabs" id="tab1" checked />
												<label for="tab1">WordList</label>
												<div id="tab-content1" class="tab-content">
													<div style="overflow-y: auto; overflow-x: hidden; height: 450px; width: 980px; padding: 5px 5px 5px 10px;">
														<asp:GridView ID="wordgv" runat="server" OnRowCreated="wordgv_OnRowCreated" OnRowDataBound="wordgv_OnRowDataBound"
															Width="100%" AllowSorting="True" OnSorting="wordgv_OnSorting">
															<AlternatingRowStyle BackColor="White" />
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
															<Columns>
																<asp:TemplateField HeaderText="NO." InsertVisible="False">
																	<ItemStyle HorizontalAlign="Center" />
																	<HeaderStyle HorizontalAlign="Center" Width="5%" />
																	<ItemTemplate>
																	</ItemTemplate>
																</asp:TemplateField>
															</Columns>
														</asp:GridView>
													</div>
													<div style="float: right; margin-right: 20px; padding: 10px 5px 5px">
														<asp:Label ID="totalW" runat="server" Text="Total: 0 Words" Visible="False"></asp:Label>
														<asp:Button ID="exExcel" runat="server" Text="导出EXCEL" CssClass="yuanjiaobtn" OnClick="exExcel_OnClick" />
													</div>
												</div>
											</li>
											<li><%--彩色文本--%>
												<input type="radio" name="tabs" id="tab2" />
												<label for="tab2">Profiled Text</label>
												<div id="tab-content2" class="tab-content">
													<%--<span style="color: blue;font-size:large;padding-left: 10px;">Title: </span> --%>
													<asp:Label ID="outlb" runat="server" Text="" ForeColor="#000" Font-Size="Large" Font-Italic="True"></asp:Label>
													<div style="float: right; padding: 5px 5px 5px 5px; background-color: #000000; margin-right: 18px;"
														id="tuliDiv" runat="server">
													</div>
													<div id="outDiv" runat="server" style="width: 990px; padding: 5px 5px 5px 5px; background-color: #000000; height: 460px; overflow-y: auto; white-space: normal; word-wrap: break-word; word-break: break-all;">
													</div>
												</div>
											</li>

											<li><%--数据统计饼状图--%>
												<input type="radio" name="tabs" id="tab3" />
												<label for="tab3">Word Profile</label>
												<div id="tab-content3" class="tab-content" style="text-align: center; vertical-align: middle">
                                                    <asp:Chart ID="Chart1" runat="server" Height="500px" Width="800px">
                                                        <Series>
                                                            <asp:Series Name="Series1"></asp:Series>
                                                        </Series>
                                                        <ChartAreas>
                                                            <asp:ChartArea Name="ChartArea1"></asp:ChartArea>
                                                        </ChartAreas>
                                                    </asp:Chart>
												</div>
											</li>
										</ul>
									</div>
								</div>

							</asp:View>
							<asp:View ID="View4" runat="server">
								<h2>Cluster</h2>
							</asp:View>
						</asp:MultiView>

					</div>
				</td>
			</tr>
		</table>
	</div>
	<asp:Label ID="lbErr" runat="server" Text="" ForeColor="red"></asp:Label>
</asp:Content>

<asp:Content ID="PageTitle" ContentPlaceHolderID="PlaceHolderPageTitle" runat="server">
NEULC
</asp:Content>

<asp:Content ID="PageTitleInTitleArea" ContentPlaceHolderID="PlaceHolderPageTitleInTitleArea" runat="server" >
NEULC-<asp:Label runat="server" ID="Titlelb">Input</asp:Label>
</asp:Content>
