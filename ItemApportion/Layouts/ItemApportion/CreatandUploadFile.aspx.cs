using System;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using System.Collections.Generic;
using Microsoft.SharePoint.Client;
using System.IO;

namespace ItemApportion.Layouts.ItemApportion
{
    public partial class CreatandUploadFile : LayoutsPageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            btnCreat.Click += BtnCreat_Click;
        }

        private void BtnCreat_Click(object sender, EventArgs e)
        {
            string filename=tbfilename.Text;
            //using (SPWeb web = SPContext.Current.Web)
            //{
            //    var list = web.Lists.GetByTitle(listTitle);
            //    var itemProperties = new Dictionary<string, object>();
            //    itemProperties["Title"] = "SharePoint User Guide";
            //    CreateAndUploadFile(list, "./SharePoint User Guide.docx", itemProperties);

            //}

            CreateDocument(filename, "文档","文档");

        }

        // Creates document in given list (root folder). 
        // Returns true if the file was created, false if it already exists
        // or throws exception for other failure
        protected bool CreateDocument(string sFilename, string sContentType, string sList)
        {
            try
            {
                SPSite site = SPContext.Current.Site;

                using (SPWeb web = site.OpenWeb())
                {
                    SPList list = web.Lists[sList];
                    // this always uses root folder
                    SPFolder folder = web.Folders[sList];
                    SPFileCollection fcol = folder.Files;

                    // find the template url and open
                    string sTemplate =list.ContentTypes[sContentType].DocumentTemplateUrl;
                    SPFile spf = web.GetFile(sTemplate);
                    byte[] binFile = spf.OpenBinary();
                    // Url for file to be created
                    string destFile = fcol.Folder.Url + "/" + sFilename;

                    // create the document and get SPFile/SPItem for 
                    // new document
                    SPFile addedFile = fcol.Add(destFile, binFile, false);
                    SPItem newItem = addedFile.Item;
                    newItem["ContentType"] = sContentType;
                    newItem.Update();
                    addedFile.Update();
                    return true;
                }
            }
            catch (SPException spEx)
            {
                // file already exists?
                if (spEx.ErrorCode == -2130575257)
                    return false;
                else
                    throw spEx;
            }
        }


        public static void UploadFile(List list, string filePath, IDictionary<string, object> itemProperties)
        {
            var ctx = list.Context;
            var fileInfo = new FileCreationInformation();
            fileInfo.Url = Path.GetFileName(filePath);
            fileInfo.Overwrite = true;
            fileInfo.Content = System.IO.File.ReadAllBytes(filePath);
            var file = list.RootFolder.Files.Add(fileInfo);
            var listItem = file.ListItemAllFields;
            foreach (var p in itemProperties)
            {
                listItem[p.Key] = p.Value;
            }
            listItem.Update();
            ctx.ExecuteQuery();
        }
        //public static void CreateAndUploadFile(List list, string filePath, IDictionary<string, object> itemProperties)
        //{
        //    using (var document = WordprocessingDocument.Create(filePath, WordprocessingDocumentType.Document))
        //    {
        //        var mainPart = document.AddMainDocumentPart();
        //        mainPart.Document = new Document(new Body());
        //    }
        //    UploadFile(list, filePath, itemProperties);
        //}

        
    }
}
