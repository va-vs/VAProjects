<%@ Assembly Name="FSCAppPages, Version=1.0.0.0, Culture=neutral, PublicKeyToken=3e860473bf6d1bc5" %>
<%@ Import Namespace="Microsoft.SharePoint.ApplicationPages" %>
<%@ Register TagPrefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>

<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="admin.aspx.cs" Inherits="FSCAppPages.Layouts.FSCAppPages.Corpus.admin" DynamicMasterPageFile="~masterurl/default.master" %>

<asp:Content ID="PageHead" ContentPlaceHolderID="PlaceHolderAdditionalPageHead" runat="server">
    <link rel="stylesheet" href="http://202.118.11.33/NEU_EC/SiteAssets//css/stylelc.css" type="text/css" charset="utf-8" />

    <script type="text/javascript">
        var currentRowId = 0;
        var styleName = "";

        function SelectRow(ev, strGvName) {
            var e = window.event || ev;
            var keyCode = -1;
            if (e.which == null)
                keyCode = e.keyCode; // IE
            else
                if (e.which > 0)
                    keyCode = e.which; // All others
            if (keyCode == 40)
                MarkRow(currentRowId + 1, strGvName);
            if (keyCode == 38) {
                MarkRow(currentRowId - 1, strGvName);
            }

            document.getElementById("NUM").value = currentRowId;
        }

        function MarkRow(rowId, strGvName) {
            var Grid = document.getElementById(strGvName);
            var rowCount = Grid.rows.length;
            if (document.getElementById(strGvName + rowId) == null)
                return;
            if (rowId == rowCount) {
                return;
            }
            if (document.getElementById(strGvName + currentRowId) != null)
                document.getElementById(strGvName + currentRowId).style.backgroundColor = styleName;
            currentRowId = rowId;
            styleName = document.getElementById(strGvName + rowId).style.backgroundColor;
            document.getElementById(strGvName + rowId).style.backgroundColor = 'red';
            var obj = document.getElementById(strGvName);
            obj.rows[rowId].cells[0].focus();
            document.getElementById("NUM").value = currentRowId;

        }
    </script>
    <script type="text/javascript">
        var prevselitem = null;

        function selectx(row) {
            if (prevselitem != null) {
                prevselitem.style.backgroundColor = '#ffffff';
            }
            row.style.backgroundColor = '#CDE6F7';
            prevselitem = row;

        }
    </script>
</asp:Content>

<asp:Content ID="Main" ContentPlaceHolderID="PlaceHolderMain" runat="server">
    <div id="divBody" runat="server">
        <asp:Label ID="lbPTitle" runat="server" Font-Size="16" Text="Manage Corpus In NEULC"></asp:Label>
        <hr />
        <table>
            <tr>
                <td>
                    <div id="divList" runat="server">
                        <div style="padding: 5px; text-align: right;">
                            <asp:Button ID="btnAdd" runat="server" Text="Add Corpora" ToolTip="Add a New Corpora" />
                        </div>
                        <div id="divQuery">
                            <table class="myform">
                                <tr>
                                    <th style="text-align: right;">KeyWord：
                                    </th>
                                    <td>
                                        <asp:TextBox ID="txtKeyWord" runat="server" Width="300"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <th style="text-align: right;">Level：
                                    </th>
                                    <td>
                                        <asp:CheckBoxList ID="cblQueryLevel" runat="server" RepeatColumns="5" RepeatDirection="Horizontal"></asp:CheckBoxList>
                                    </td>
                                </tr>
                                <tr>
                                    <th style="text-align: right;">Genre：
                                    </th>
                                    <td>
                                        <asp:CheckBoxList ID="cblQueryGenre" runat="server" RepeatColumns="5" RepeatDirection="Horizontal"></asp:CheckBoxList>
                                    </td>
                                </tr>
                                <tr>
                                    <th style="text-align: right;">Topic：
                                    </th>
                                    <td>
                                        <asp:CheckBoxList ID="cblQueryTopics" runat="server" RepeatColumns="5" RepeatDirection="Horizontal"></asp:CheckBoxList>
                                    </td>
                                </tr>
                                <tr>
                                    <td colspan="2" style="text-align: center; margin-right: 20px;">
                                        <asp:Button ID="btnQuery" runat="server" Text="Find matching Corpus" />&nbsp;&nbsp;

                                                                        <input type="button" value=" Close " class="addbtn" onclick="javascript: window.location.href = 'neulc.aspx'" />

                                    </td>
                                </tr>
                            </table>
                        </div>
                        <asp:GridView ID="gvCorpus" AllowPaging="true" runat="server" AutoGenerateColumns="False" CellPadding="2" CellSpacing="2" BorderColor="#CCCCCC" BorderStyle="None" GridLines="Horizontal" DataKeyNames="CorpusID" CssClass="myGrid" PageSize="50" PagerSettings-Mode="NumericFirstLast">
                            <AlternatingRowStyle BackColor="White" />
                            <Columns>
                                <asp:TemplateField HeaderText="&nbsp;Title&nbsp;">
                                    <ItemTemplate>
                                        <asp:Label runat="server" Text='<%#Eval("Title")%>' ID="lbTitle"></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="&nbsp;Level&nbsp;">
                                    <ItemTemplate>
                                        <asp:HiddenField ID="hdfLevelId" runat="server" Value='<%#Eval("LevelID")%>' />
                                        <asp:Label runat="server" Text="" ID="lbLevel"></asp:Label>
                                    </ItemTemplate>
                                    <ItemStyle HorizontalAlign="Center" />
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="&nbsp;Genre&nbsp;">
                                    <ItemTemplate>
                                        <asp:HiddenField ID="hdfGenreId" runat="server" Value='<%#Eval("GenreID")%>' />
                                        <asp:Label runat="server" Text="" ID="lbGenre"></asp:Label>
                                    </ItemTemplate>
                                    <ItemStyle HorizontalAlign="Center" />
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="&nbsp;Topic&nbsp;">
                                    <ItemTemplate>
                                        <asp:HiddenField ID="hdfTopicId" runat="server" Value='<%#Eval("TopicID")%>' />
                                        <asp:Label runat="server" Text="" ID="lbTopic"></asp:Label>
                                    </ItemTemplate>
                                    <ItemStyle HorizontalAlign="Center" />
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="&nbsp;Context&nbsp;">
                                    <ItemTemplate>
                                        <asp:Label runat="server" Text='<%#Eval("OriginalText")%>' ID="lbContext"></asp:Label>
                                    </ItemTemplate>
                                    <ItemStyle HorizontalAlign="Center" />
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="&nbsp;Manage&nbsp;" Visible="true">
                                    <ItemTemplate>
                                        <div style="padding: 5px;">
                                            <asp:LinkButton ID="lnkEdit" runat="server" Font-Underline="false" CommandArgument='<%#Eval("CorpusID") %>' CommandName="EditPlan">
                                                                                <img src="../../../../_layouts/15/VSProject/images/Edit.png" width="20" height="20" alt="" title="Edit this Corpora" />
                                            </asp:LinkButton>
                                            <asp:LinkButton ID="lnkDel" runat="server" Font-Underline="false" CommandArgument='<%#Eval("CorpusID") %>' CommandName="DelPlan" OnClientClick="return confirm('Are you sure to delete this Corpora？')" ToolTip="Delete this Corpora">
                                                                                <img src="../../../../_layouts/15/VSProject/images/Del.png" width="20" height="20" alt="" title="Delete this Corpora" />
                                            </asp:LinkButton>
                                        </div>
                                    </ItemTemplate>
                                    <ItemStyle HorizontalAlign="Center" />
                                </asp:TemplateField>
                            </Columns>
                            <EditRowStyle BackColor="#2461BF" />
                            <EmptyDataTemplate>
                                <asp:Label ID="lbEmptPlan" runat="server" Text="There is none corpora here!" ForeColor="red"></asp:Label>
                            </EmptyDataTemplate>

                            <FooterStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" HorizontalAlign="Center" />
                            <HeaderStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" HorizontalAlign="Center" Height="28" />
                            <PagerStyle BackColor="#2461BF" ForeColor="White" HorizontalAlign="Center" />
                            <RowStyle BackColor="#EFF3FB" />
                            <SelectedRowStyle BackColor="#D1DDF1" Font-Bold="True" ForeColor="#333333" />
                            <SortedAscendingCellStyle BackColor="#F5F7FB" />
                            <SortedAscendingHeaderStyle BackColor="#6D95E1" />
                            <SortedDescendingCellStyle BackColor="#E9EBEF" />
                            <SortedDescendingHeaderStyle BackColor="#4870BE" />
                        </asp:GridView>

                    </div>
                </td>
            </tr>
            <tr>
                <td>
                    <div id="divEditCorpora" runat="server">
                        <table class="myform" style="width: 100%">
                            <tr>
                                <th>Topic:
                                </th>
                                <td>
                                    <asp:CheckBoxList ID="cblTopics" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow" RepeatColumns="5" ToolTip="Choose any of the Topic that the Corpora belongs to"></asp:CheckBoxList>
                                </td>
                            </tr>
                            <tr>
                                <th>Genre:
                                </th>
                                <td>
                                    <asp:CheckBoxList ID="cblGenre" runat="server" RepeatColumns="5" RepeatLayout="Table" RepeatDirection="Horizontal" ToolTip="Choose any of the Genre Styles that the Corpora belongs to"></asp:CheckBoxList>
                                </td>
                            </tr>
                            <tr>
                                <th>Level:
                                </th>
                                <td>
                                    <asp:CheckBoxList ID="cblLevel" RepeatDirection="Horizontal" RepeatLayout="Flow" runat="server" RepeatColumns="5" ToolTip="Choose one Level that the Corpora Author belongs to"></asp:CheckBoxList>
                                </td>
                            </tr>
                            <tr>
                                <th>Title:
                                </th>
                                <td>
                                    <asp:TextBox ID="txtTitle" runat="server" Width="500px" ToolTip="Fill in this Blank with the title of the Corpora"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <th>Source:
                                </th>
                                <td>
                                    <asp:TextBox ID="txtSource" runat="server" Width="500px" ToolTip="Fill in this Blank with the source of the Corpora"></asp:TextBox>
                                </td>
                            </tr>

                            <tr>
                                <th style="vertical-align: top;">Context:
                                </th>
                                <td>
                                    <asp:TextBox ID="txtOriginalText" runat="server" TextMode="MultiLine" Rows="15" Width="500px" ToolTip="FIll in this blank with source context!"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td colspan="2" style="text-align: center;">
                                    <div style="padding: 10px;">
                                        &nbsp;&nbsp;
                                                                        <asp:Button ID="btnSubmit" runat="server" Text="Submit" />&nbsp;&nbsp;
                                                                        <asp:Button ID="btnCancel" runat="server" Text="Cancel" />&nbsp;&nbsp;
                                                                        <input type="button" value=" Close " class="addbtn" onclick="javascript: window.location.href = 'neulc.aspx'" />
                                    </div>
                                </td>
                            </tr>
                            <tr>
                                <td colspan="2">
                                    <div>
                                        <asp:HiddenField ID="hdfID" runat="server" Value="" />
                                    </div>
                                </td>
                            </tr>
                        </table>
                    </div>
                </td>
            </tr>
            <tr>
                <td>
                    <div id="divViewCorpora" runat="server" visible="false">
                        <div>
                            Source information:
                        </div>
                        <div>
                            Expanded context:
                        </div>
                        <div>
                            <input type="button" value=" Close " class="addbtn" onclick="javascript: window.location.href = 'neulc.aspx'" />
                        </div>
                    </div>
                </td>
            </tr>

        </table>

    </div>


    <asp:Label ID="lbErr" runat="server" Text="" ForeColor="red"></asp:Label>

</asp:Content>

<asp:Content ID="PageTitle" ContentPlaceHolderID="PlaceHolderPageTitle" runat="server">
    语料录入
</asp:Content>

<asp:Content ID="PageTitleInTitleArea" ContentPlaceHolderID="PlaceHolderPageTitleInTitleArea" runat="server">
    语料录入
</asp:Content>
