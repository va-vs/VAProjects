<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %> 
<%@ Register TagPrefix="cc1" Namespace="AjaxControlToolkit" Assembly="AjaxControlToolkit, Version=18.1.1.0, Culture=neutral, PublicKeyToken=28f01b0e84b6d53e" %>
<%@ Register Tagprefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AddAttachmentsUserControl.ascx.cs" Inherits="AssistantResults.AddAttachments.AddAttachmentsUserControl" %>
  <script src="http://code.jquery.com/jquery-1.10.2.min.js" type="text/javascript"></script>
  <script type="text/javascript">
      function AddAttach()
      {
          //alert(document.getElementById("_wpSelected").value);
          var items = GetSelectedDocuments()
          //if (items.length == 0)
          //    alert("请先选择附件文件！");
          //else
          //    alert();
      }
      function GetSelectedDocuments() {
       var ctx = new SP.ClientContext.get_current();
       if (ctx != undefined &&  ctx != null) {
            var listId = SP.ListOperation.Selection.getSelectedList();  
            var selectedItems = SP.ListOperation.Selection.getSelectedItems(ctx);
            var ItemIDs = '';
            var i;
            for(i = 0; i < selectedItems.length; i++)  
                ItemIDs += selectedItems[i].id + ';';

            alert(ItemIDs);
         }
        }

        function GetQueryString(name)
        {
             var reg = new RegExp("(^|&)"+ name +"=([^&]*)(&|$)");
             var r = window.location.search.substr(1).match(reg);
             if(r!=null)return  unescape(r[2]); return null;
        }
        function getlistitem( resultID)
　　        {
 　                  var mycontext=new SP.ClientContext();
 　                  var mysite=mycontext.get_web();
                     var query = new SP.CamlQuery();
                     var listRel = "用户媒体";//关系表
                     var assID = GetQueryString("ID");
                     if (assID == null) assID = 0;//newform.aspx
　　                 query.set_viewXml("<View><Query>"+
                                            "<ViewFields>" +
                                                "<FieldRef Name='Title' />" +
                                                "<FieldRef Name='AssistantID'/>" +
                                                "<FieldRef Name='ResultID'/>" +
                                           "</ViewFields>" +
                                           "<Where><And>" +
                                                 "<Eq>" +
                                                    "<FieldRef Name='AssistantID'/>" +
                                                    "<Value Type='Number'>"+ resultID+"</Value>" +
                                                 "</Eq>" +
                                                 "<Eq>" +
                                                    "<FieldRef Name='ResultID'/>" +
                                                    "<Value Type='Number'>"+ s+"</Value>" +
                                                 "</Eq>" +
                                           "</And></Where></Query></View>");
  
  　                 var mylist=mysite.get_lists().getByTitle( listRel );
   　                myitem= mylist.getItems(query);
  
   mycontext.load(myitem);
           
   　                mycontext.executeQueryAsync(Function.createDelegate(this,this.getsuccessed),Function.createDelegate　　　　
  
　　                (this,this.getfailed));
　　         }
　　         function getsuccessed()
　　         { 
　                var str="";
　　             var listsE=myitem.getEnumerator();
　　             while(listsE.moveNext())
 　　            {
 　　             str+=listsE.get_current().get_item("Title")+"<br>";   
 　　            }
　　             alert (str);
  
 　　        }
 　　        function getfailed(sender,args)
　　         {
  　　            alert("failed~!");
 　　        }

            function createListItem() {
                var clientContext =SP.ClientContext.get_current();
                var oList = clientContext.get_web().get_lists().getByTitle('操作');
                var itemCreateInfo = new SP.ListItemCreationInformation();

                this.oListItem = oList.addItem(itemCreateInfo);

                oListItem.set_item('Title', 'Item from de Hrnode!');

                oListItem.update();

                clientContext.load(oListItem);

                clientContext.executeQueryAsync(Function.createDelegate(this, this.onQuerySucceeded), Function.createDelegate(this, this.onQueryFailed));
            }

            function onQuerySucceeded() {

                alert('Item created: ' + oListItem.get_id());
            }

            function onQueryFailed(sender, args) {

                alert('Request failed. ' + args.get_message() + '\n' + args.get_stackTrace());
            }
      //获取用户选的ID
            var list;
            ExecuteOrDelayUntilScriptLoaded(GetSelectedItemsID, 'SP.js');
            function GetSelectedItemsID() {
                var context = SP.ClientContext.get_current();
                var selectedItems = SP.ListOperation.Selection.getSelectedItems(context);
                 list = context.get_web().get_lists().getByTitle("活动媒体库");
                var item;
               
                context.load(list);
                context.executeQueryAsync(
        Function.createDelegate(this, this.onQuerySucceeded1), 
        Function.createDelegate(this, this.onQueryFailed1)
    );
            //     listInfo += 'Title: ' + oList.get_title() + 
            //' ID: ' + oList.get_id().toString();
                //alert(list.get_id().toString());
                //alert(SP.ListOperation.Selection.getSelectedList());
                 var ItemIDs = new Array();　//创建一个数组;
                for (item in selectedItems) {
                    ItemIDs.push(selectedItems[item].id);
                    //context.executeQueryAsync(Function.createDelegate(this, this.onQuerySucceeded), Function.createDelegate(this, this.onQueryFailed));
                }
                //if (selectedItems.length == 0)
                //{
                //    var value = SP.ListOperation.Selection.selectListItem("0,1,0", true);
                //    alert(value);
                   
                //}
                   
                return ItemIDs;
                    
            }

   function GetListID() {
       listId = SP.ListOperation.Selection.getSelectedList();
       
        alert(listId);
   }
  </script>
<script  type="text/javascript">
    function SetOperatorSelect()
        {
           <%-- var objSelect=document.getElementById("<%=ddlOrderType.ClientID %>");
            var myOperatorKeyWord=document.getElementById('txtOperatorKeyWord').value;
            SetValueSelect(objSelect,myOperatorKeyWord);--%>
        }
        function SetValueSelect(objSelect,myKeyWord)
        {
            var myTemp=document.getElementById('ddlTemp');
            myTemp.options.length=0;
            if(myKeyWord!="")
            {
                for (var i = 0; i < objSelect.options.length; i++) { 
                    if (objSelect.options[i].text.indexOf(myKeyWord)> -1) {  
                        myTemp.options.add(new Option(objSelect.options[i].text,objSelect.options[i].value));
                    }  
                }  


                objSelect.options.length=0;
                for (var i = 0; i < myTemp.options.length; i++) {                 
                    objSelect.options.add(new Option(myTemp.options[i].text,myTemp.options[i].value));
                }
            }
            else
            {
                alert("请先输入关键字!");
            }
        }

</script>
<style type="text/css">
div[id^=MSOZoneCell_] {
    position:relative;
}
#ms-dnd-dropbox{
    left:0 !important;
    top: 0 !important;
    width: 100% !important;
    height: 100% !important;
}

</style>
<div id="ms-dnd-dropbox" style="display:none;">
    <span class="ms-attractMode ms-noWrap" id="ms-dnd-dropboxText" style="line-height: 324px;">Drop here...</span>
</div>
 
    <asp:DropDownList ID="DropDownList1" runat="server" AppendDataBoundItems="True"></asp:DropDownList>

  <input type="button" id="writelistitem" value="添加附件" onclick="AddAttach() " />
    <asp:Label ID="lblMsg" ForeColor="Red" runat="server" Text=""></asp:Label>

<div runat ="server" id="divRoll">
<%--    <asp:DropDownList ID="ddlOrderType" runat="server" >
    <asp:ListItem>Red</asp:ListItem>
    <asp:ListItem>Green</asp:ListItem>
    <asp:ListItem>Blue</asp:ListItem>
    <asp:ListItem>Yellow</asp:ListItem>
    <asp:ListItem>Magenta</asp:ListItem>
    <asp:ListItem>Violet</asp:ListItem>
    <asp:ListItem>Indigo</asp:ListItem>
    <asp:ListItem>Orange</asp:ListItem>
    <asp:ListItem>Oan</asp:ListItem>
            </asp:DropDownList>

            颜色：<input name="txtOperatorKeyWord" type="text" id="txtOperatorKeyWord" class="addinpu160" />
                                <input name="btnSearchOperator" onclick="SetOperatorSelect();" class="tbtn" type="button" value=" 搜索 " />

<select id="ddlTemp" style="display:none"></select>--%>
</div>