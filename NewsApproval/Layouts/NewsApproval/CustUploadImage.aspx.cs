using System;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using System.IO;

namespace NewsApproval.Layouts.NewsApproval
{
    public partial class CustUploadImage : LayoutsPageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }
        protected void btnSave_Click(object sender, EventArgs e)
        {

            SPSecurity.RunWithElevatedPrivileges(delegate()
            {

                byte[] imageData = null;

                if ((flUpload.PostedFile != null) && (flUpload.PostedFile.ContentLength > 0))
                {

                    Stream MyStream = flUpload.PostedFile.InputStream;

                    long iLength = MyStream.Length;

                    imageData = new byte[(int)MyStream.Length];

                    MyStream.Read(imageData, 0, (int)MyStream.Length);

                    MyStream.Close();

                    string filename = System.IO.Path.GetFileName(flUpload.PostedFile.FileName);
                    string imageurl;
                    using (SPWeb web = SPContext.Current.Web)
                    {
                        SPList pic = web.Lists.TryGetList("图像");
                        SPFile archivoSubir = pic.RootFolder.Files.Add(filename, imageData);

                        imageurl = archivoSubir.ServerRelativeUrl;
                    }
                    //web.Dispose();

                    ClientScript.RegisterStartupScript(ClientScript.GetType(), "myscript", "<script>ReturnPageValue(" + imageurl + "," + txtDes.Value + ");</script>");

                }

            });

        }
    }
}
