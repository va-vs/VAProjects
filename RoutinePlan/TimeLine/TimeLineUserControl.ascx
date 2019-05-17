<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Register Tagprefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="TimeLineUserControl.ascx.cs" Inherits="RoutinePlan.TimeLine.TimeLineUserControl" %>
<link href="../../../../_layouts/15/RoutinePlan/css/history.css" rel="stylesheet" />
		<script src="../../../../_layouts/15/RoutinePlan/js/jquery.js"></script>
		<script src="../../../../_layouts/15/RoutinePlan/js/main.js"></script>
	<div class="head-warp">
			<div class="head">
				<div class="nav-box">
					<ul>
						<li class="cur" style="text-align:center; font-size:32px; font-family:'微软雅黑';">个人活动时间线</li>
					</ul>
				</div>
			</div>
		</div>

		<div class="main">
			<div class="history">
				<div class="history-date">
					<ul>
						<h2 class="first"><a href="#nogo">9月18日</a></h2>

						<li class="green">
							<h3>10:08<span>9/18</span></h3>
							<dl>
								<dt>工作
	<span>安装设备</span>
	</dt>
							</dl>
						</li>

					</ul>
				</div>
				<div class="history-date">
					<ul>
						<h2 class="date02"><a href="#nogo">9月17日</a></h2>
						<li>
							<h3>12:12<span>09/17</span></h3>
							<dl>
								<dt>例行工作
	<span>早会</span>
	</dt>
							</dl>
						</li>

					</ul>
				</div>
				<div class="history-date">
					<ul>
						<h2 class="date02"><a href="#nogo">9月16日</a></h2>
						<li>
							<h3>12:13<span>09/16</span></h3>
							<dl>
								<dt>生活<span>午餐</span></dt>
							</dl>
						</li>
					</ul>
				</div>
			</div>
		</div>
