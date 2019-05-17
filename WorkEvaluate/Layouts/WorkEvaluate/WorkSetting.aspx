<%@ Assembly Name="WorkEvaluate, Version=1.0.0.0, Culture=neutral, PublicKeyToken=de9e609710912b67" %>
<%@ Import Namespace="Microsoft.SharePoint.ApplicationPages" %>
<%@ Register TagPrefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls"
    Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>

<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="WorkSetting.aspx.cs" Inherits="WorkEvaluate.Layouts.WorkEvaluate.WorkSetting"
    DynamicMasterPageFile="~masterurl/default.master" %>

<asp:Content ID="PageHead" ContentPlaceHolderID="PlaceHolderAdditionalPageHead" runat="server">
    <link rel="stylesheet" type="text/css" href="css/base.css" />
    <link rel="stylesheet" type="text/css" href="css/page.css" />
    <style type="text/css">
        .WorksMain {
background-color:#F0F0F0;
            margin-left:30px;
border: 1px solid #CCCCCC;
-moz-border-radius: 10px;
-webkit-border-radius: 10px;
width:700px;

        }

            .WorksMain p {
                margin: 10px 0;
            }

            .WorksMain i {
                margin: 0 5px;
            }

            .WorksMain label {
                margin-right: 10px;
            }

        .WorksVerification {
            font-family: "微软雅黑";
            color: #666666;
            font-size: 14px;
        }

        .mr5 {
            margin-right: 5px;
        }

        .wbtn {
            margin: 15px 0 0 85px;
            background-color: #6bab5f;
            color: White;
            font-family: "微软雅黑";
            font-size: 16px;
            border: 0;
        }

        .txtdoenlist {
            width: 90px;
            height: 25px;
            line-height: 25px;
            vertical-align: middle;
            padding: 3px;
            border: 1px #bebee1 solid;
            color: #494b4c;
        }

        .txtdoenlista {
            width: 210px;
            height: 25px;
            line-height: 25px;
            vertical-align: middle;
            padding: 3px;
            border: 1px #bebee1 solid;
            color: #494b4c;
        }

        .cssDiv {
            width: 800px;
            font-size: 13px;
            padding: 5px;
            color: #414141;
            border: 1px solid #c7c7c7;
        }

        .wt {
            padding-right: 5px;
            color: #4a4a4a;
            font-size: 15px;
            width: 120px;
            float: left;
            text-align: right;
        }

        .divm {
            margin: 8px 0 0 50px;
            padding: 0;
            width: 800px;
        }

        .warn {
            margin: 0;
            text-align: left;
            color: red;
            font-size: 14px;
            display: none;
            width: 179px;
            height: 27px;
        }

        .warna {
            margin: 0;
            color: red;
            font-size: 14px;
            display: inline;
            padding: 3px;
            width: 179px;
            height: 27px;
            text-align: center;
            border: 1px solid red;
        }

        .Stepsheet {
            width: 800px;
            height: 34px;
            background: url(images/bar1.jpg) no-repeat center center;
        }

        .Stepsheet2 {
            width: 800px;
            height: 34px;
            background: url(images/bar2.jpg) no-repeat center center;
        }

        .Stepsheet3 {
            width: 800px;
            height: 34px;
            background: url(images/bar3.jpg) no-repeat center center;
        }

        .Stepsheetword {
            list-style: none;
            height: 34px;
            line-height: 34px;
            margin-top: 0;
            margin-bottom: 0;
            vertical-align: middle;
        }

            .Stepsheetword li {
                float: left;
                padding: 0 30px;
                font-size: 18px;
                font-family: 微软雅黑;
                color: #fff;
            }

        .txtcss {
            width: 200px;
            border: 1px #bebee1 solid;
            height: 25px;
            vertical-align: middle;
            line-height: 25px;
            padding: 0 5px;
            color: Black;
        }

        .wd350 {
            width: 350px;
        }

        .GetNameDiv {
            padding: 0;
            margin-left: 180px;
            margin-top: 12px;
            border: 1px solid #cccc66;
            width: 363px;
            height: 53px;
            background-color: #f4f1c6;
            position: relative;
            text-align: left;
        }

        .TriangleDiv {
            width: 14px;
            height: 12px;
            background: url(images/Triangle.gif) no-repeat;
            position: absolute;
            left: 24px;
            top: -12px;
            z-index: 99;
        }

        .wtSpan {
            padding: 5px;
            color: #4a4a4a;
            font-size: 14px;
            width: 150px;
            float: left;
            text-align: left;
        }

        .GetWarnDiv {
            padding: 0;
            margin-left: 150px;
            margin-top: 12px;
            width: 500px;
            height: 53px;
            position: relative;
            text-align: left;
            font-size: 12px;
            color: #666;
        }

        .hg150 {
            height: 150px;
        }

        .hg100 {
            height: 100px;
        }


        .hidden {
            display: none;
        }


        .GridViewStyle {
            border-right: 2px solid #A7A6AA;
            border-bottom: 2px solid #A7A6AA;
            border-left: 2px solid white;
            border-top: 2px solid white;
            padding: 4px;
        }

            .GridViewStyle a {
                color: #FFFFFF;
            }

        .GridViewHeaderStyle th {
            border-left: 1px solid #EBE9ED;
            border-right: 1px solid #EBE9ED;
        }

        .GridViewHeaderStyle {
            background-color: #5D7B9D;
            font-weight: bold;
            color: White;
        }

        .GridViewFooterStyle {
            background-color: #5D7B9D;
            font-weight: bold;
            color: White;
        }

        .GridViewRowStyle {
            background-color: #F7F6F3;
            color: #333333;
        }

        .GridViewAlternatingRowStyle {
            background-color: #FFFFFF;
            color: #284775;
        }

            .GridViewRowStyle td, .GridViewAlternatingRowStyle td {
                border: 1px solid #EBE9ED;
            }

        .GridViewSelectedRowStyle {
            background-color: #E2DED6;
            font-weight: bold;
            color: #333333;
        }

        .GridViewPagerStyle {
            background-color: #5D7B9D;
            color: #EBE9ED;
        }

            .GridViewPagerStyle a {
                color: #FFFFFF;
            }

            .GridViewPagerStyle table /* to center the paging links*/ {
                margin: 0 auto 0 auto;
            }

        .fullh {
            float: left;
        }

        .ms-long {
            width: 90% !important;
        }

        .ms-longer {
            width: 90% !important;
        }

        .ms-rtelong {
            width: 90% !important;
        }
.rounddiv {
background-color: #000;
border: 1px solid #000;
-moz-border-radius: 10px;
-webkit-border-radius: 10px;
color:#fff;
}

        /*.ms-rtelonger {
            width: 90% !important;
        }*/
    </style>
    <link rel="stylesheet" type="text/css" href="css/base.css" />
    <link rel="stylesheet" type="text/css" href="css/page.css" />
    <script type="text/javascript">
        function IsValidText() {
            var text = RTE_GetRichEditTextOnly("<%=txtRequire.ClientID%>");
            if (text != "") {
                return true;
            }
            else {
                alert('作品要求必填');
                RTE_GiveEditorFocus("<%= txtRequire.ClientID%>");
                return false;
            }
        }
    </script>

</asp:Content>

<asp:Content ID="Main" ContentPlaceHolderID="PlaceHolderMain" runat="server">
    <div>
        <asp:Label ID="error" CssClass="redStar" runat="server" Text=""></asp:Label>
    </div>
    <!--<div style="height: 25px; margin-left: 30px">
        <span style="font-family:微软雅黑;font-size:16px;font-weight:border;">&nbsp;&nbsp;课程作品期次列表</span>
    </div>
-->

    <div style="margin-left: 30px" class="WorksMain">
        <span style="font-family: 微软雅黑; font-size: 16px; font-weight: border; color: black;">
            课程作品期次列表</span>
        <asp:GridView ID="gvPeriod" runat="server" AllowPaging="true" PageSize="5" AutoGenerateColumns="False"
            CellPadding="0" GridLines="None" Height="30px" Width="700px" PagerSettings-Mode="NumericFirstLast"
            PagerSettings-FirstPageText="首页" PagerSettings-NextPageText="下一页" PagerSettings-PreviousPageText="上一页"
            PagerSettings-LastPageText="尾页">
            <FooterStyle CssClass="GridViewFooterStyle" />
            <RowStyle CssClass="GridViewRowStyle" />
            <SelectedRowStyle CssClass="GridViewSelectedRowStyle" />
            <PagerStyle CssClass="GridViewPagerStyle" HorizontalAlign="Center" />
            <AlternatingRowStyle CssClass="GridViewAlternatingRowStyle" />
            <HeaderStyle CssClass="GridViewHeaderStyle" HorizontalAlign="Left" />


            <EmptyDataTemplate>
                <table>
                    <tr>
                        <td>作品名称
                        </td>
                    </tr>
                    <tr>
                        <td colspan="5">当前没有数据
                        </td>
                    </tr>
                </table>
            </EmptyDataTemplate>

            <Columns>
                <asp:BoundField DataField="PeriodID" HeaderText="期次ID">
                    <FooterStyle CssClass="hidden" />
                    <HeaderStyle CssClass="hidden" />
                    <ItemStyle Width="60px" CssClass="hidden"></ItemStyle>
                </asp:BoundField>
                <asp:BoundField DataField="PeriodTitle" HeaderText="课程期次" />
                <asp:TemplateField HeaderText="删除" ItemStyle-Width="120px">
                    <ItemTemplate>
                        <asp:Button ID="btnDel" CausesValidation="false" CommandName="Del" Text="删除" runat="server"
                            BorderStyle="None" />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="编辑" ItemStyle-Width="120px">
                    <ItemTemplate>
                        <asp:Button ID="btnEdit" CausesValidation="false" CommandName="Down" Text="编辑" runat="server"
                            BorderStyle="None" />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="上传样例" ItemStyle-Width="120px">
                    <ItemTemplate>
                        <%-- <asp:Button ID="btnUpload" CausesValidation="false" CommandName="Upload" Text="上传" runat="server" BorderStyle="None" />--%>
                        <asp:HyperLink Target="_blank" runat="server" ID="lnkUpload">上传</asp:HyperLink>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="评价指标" ItemStyle-Width="120px">
                    <ItemTemplate>
                        <%--<asp:Button ID="btnStandard" CausesValidation="false" CommandName="Standard" Text="添加" runat="server" BorderStyle="None" />--%>
                        <asp:HyperLink Target="_blank" runat="server" ID="lnkStandard">添加</asp:HyperLink>
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>

        </asp:GridView>
        <br>
    </div>
    <p>&nbsp;</p>
    <div class="WorksMain">

        <span style="font-family: 微软雅黑; font-size: 16px; font-weight: border; margin-top: 10px;
            color: black;">&nbsp;&nbsp;新增或修改作品期次</span>

        <hr style="height: 1px; width: 700px; margin-top: 4px; filter: alpha(opacity=5,finishopacity=100,style=1);" />
        <table cellpadding="0" cellspacing="0">
            <tr>
                <td>
                    <p>
                        <label class="WorksVerification"><span class="redStar"><i>*</i></span>课程期次:</label>
                        <asp:TextBox ID="txtName" runat="server" Width="450px" Height="30px" BorderColor="#CCCCCC"
                            BorderStyle="Solid" BorderWidth="1px"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="RequiredFieldValidator1" CssClass="redStar" runat="server"
                            ErrorMessage="必填" ControlToValidate="txtName"></asp:RequiredFieldValidator>
                    </p>
                </td>
            </tr>
            <tr>
                <td>
                    <p>
                        <label class="WorksVerification"><span class="redStar"><i>*</i></span>作品类别:</label>
                        <asp:DropDownList ID="ddlOneWorksType" runat="server" CssClass="txtdoenlista" Height="25px"
                            AutoPostBack="True">
                        </asp:DropDownList>
                        <asp:DropDownList ID="ddlTwoWorksType" runat="server" CssClass="txtdoenlista" Width="220px"
                            Height="25px">
                        </asp:DropDownList>
                        <asp:RequiredFieldValidator ID="RequiredFieldValidator2" CssClass="redStar" runat="server"
                            ErrorMessage="必填" ControlToValidate="ddlTwoWorksType"></asp:RequiredFieldValidator>
                    </p>
                </td>
            </tr>
            <tr>
                <td>
                    <p>
                        <label class="WorksVerification"><span class="redStar"><i>*</i></span>最多参与人数:</label>
                        <asp:TextBox ID="txtNum" runat="server" Width="20px" Height="20px" BorderColor="#CCCCCC"
                            BorderStyle="Solid" BorderWidth="1px"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="RequiredFieldValidator4" CssClass="redStar" runat="server"
                            ErrorMessage="必填" ControlToValidate="txtNum"></asp:RequiredFieldValidator>
                        <asp:RangeValidator ID="RangeValidator1" CssClass="redStar" runat="server" ErrorMessage="只能输入1-10的数字"
                            Type="Integer" ControlToValidate="txtNum" MinimumValue="1" MaximumValue="10"></asp:RangeValidator>
                    </p>
                </td>
            </tr>

            <tr>
                <td>
                    <p>
                        <label class="WorksVerification" style="float: left"><span class="redStar"><i>*</i></span>作品要求:</label>
                        <span class="fullh" style="width: 500px;">
                            <SharePoint:InputFormTextBox ID="txtRequire" runat="server" RichText="true" Rows="10"
                                TextMode="MultiLine" RichTextMode="Compatible" Style="width: 500px; height: 200px;"></SharePoint:InputFormTextBox>
                        </span>
                    </p>
                </td>
            </tr>

        </table>

        <table cellpadding="0" cellspacing="0">
            <tr>
                <td colspan="4">
                    <p></p>
                    <hr style="height: 1px; border: none; border-top: 1px dotted #CCCCCC; width: 700px;
                        margin-top: 4px;" />
                </td>
            </tr>
            <tr>
                <td colspan="4">
                    <h3>作品提交时间</h3>

                </td>
            </tr>
            <tr>
                <td width="150px" align="right">
                    <label class="WorksVerification"><i style="color: red">*</i>开放时间:</label></td>
                <td width="100px" align="left">
                    <SharePoint:DateTimeControl ID="dateTimeStartSubmit" AutoPostBack="true" runat="server"
                        DateOnly="true" />
                </td>
                <td width="150px" align="right">
                    <label class="WorksVerification"><i style="color: red">*</i>截止时间:</label></td>
                <td align="left">
                    <SharePoint:DateTimeControl ID="dateTimeEndSubmit" runat="server" DateOnly="true" />
                </td>
            </tr>
            <tr>
                <td colspan="4">
                    <p></p>
                    <hr style="height: 1px; border: none; border-top: 1px dotted #CCCCCC; width: 700px;
                        margin-top: 4px;" />
                </td>
            </tr>
            <tr>
                <td colspan="4">
                    <h3>作品评分时间</h3>

                </td>
            </tr>
            <tr>
                <td width="150px" align="right">
                    <label class="WorksVerification"><i style="color: red">*</i>开放时间:</label></td>
                <td width="100px" align="left">
                    <SharePoint:DateTimeControl ID="dateTimeStartScore" runat="server" DateOnly="true" />
                </td>
                <td width="150px" align="right">
                    <label class="WorksVerification"><i style="color: red">*</i>截止时间:</label></td>
                <td align="left">
                    <SharePoint:DateTimeControl ID="dateTimeEndScore" runat="server" DateOnly="true" />
                </td>
            </tr>
            <tr>
                <td colspan="4">
                    <p></p>
                    <hr style="height: 1px; border: none; border-top: 1px dotted #CCCCCC; width: 700px;
                        margin-top: 4px;" />
                </td>
            </tr>
            <tr>
                <td colspan="4">
                    <h3>作品公示时间</h3>

                </td>
            </tr>
            <tr>
                <td width="150px" align="right">
                    <label class="WorksVerification"><i style="color: red">*</i>开放时间:</label></td>
                <td width="100px" align="left">
                    <SharePoint:DateTimeControl ID="dateTimeStartPublic" runat="server" DateOnly="true" />
                </td>
                <td width="150px" align="right">
                    <label class="WorksVerification"><i style="color: red">*</i>截止时间:</label></td>
                <td align="left">
                    <SharePoint:DateTimeControl ID="dateTimeEndPublic" runat="server" DateOnly="true" />
                </td>
            </tr>
            <tr>
                <td colspan="4">
                    <p></p>
                    <hr style="height: 1px; border: none; border-top: 1px dotted #CCCCCC; width: 700px;
                        margin-top: 4px;" />
                </td>
            </tr>
        </table>
        <div align="right">
            <p style="clear: both; margin-right: 160px;">
                <asp:HiddenField ID="hfID" runat="server" />
                <asp:Button ID="btnAdd" runat="server" Text="发     布" Style="width: 133px; height: 35px;
                    background: url(images/ButtonBg.gif); border: 0; color: #fff; font-weight: bolder;
                    font-size: 15px; margin: 0; cursor: pointer" OnClientClick="return IsValidText()" />
                <asp:Button ID="btnSave" runat="server" Text="保     存" Style="width: 133px; height: 35px;
                    background: url(images/ButtonBg.gif); border: 0; color: #fff; font-weight: bolder;
                    font-size: 15px; margin: 0; cursor: pointer" OnClientClick="return IsValidText()" />
            </p>
        </div>
    </div>

</asp:Content>

<asp:Content ID="PageTitle" ContentPlaceHolderID="PlaceHolderPageTitle" runat="server">
    展评作品的需求
</asp:Content>

<asp:Content ID="PageTitleInTitleArea" ContentPlaceHolderID="PlaceHolderPageTitleInTitleArea"
    runat="server">
    展评作品的需求
</asp:Content>
