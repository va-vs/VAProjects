using System;
using System.ComponentModel;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;

namespace FSCWebParts.Corpus
{
    [ToolboxItemAttribute(false)]
    public class Corpus : WebPart
    {
        // Visual Studio might automatically update this path when you change the Visual Web Part project item.
        private const string _ascxPath = @"~/_CONTROLTEMPLATES/15/FSCWebParts/Corpus/CorpusUserControl.ascx";

        protected override void CreateChildControls()
        {
            CorpusUserControl control = Page.LoadControl(_ascxPath) as CorpusUserControl;
            if (control!=null)
            {
                control.webObj = this;
            }
            Controls.Add(control);
        }
    }
}
