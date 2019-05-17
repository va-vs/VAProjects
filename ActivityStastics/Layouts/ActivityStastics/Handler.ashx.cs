using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using System.Web;


namespace ActivityStastics.Layouts.ActivityStastics
{
    /// <summary>
    /// Handler 的摘要说明
    /// </summary>
    public class Handler : IHttpHandler
    {
        public bool IsReusable
        {
            get { return true; }
        }

        public void ProcessRequest(HttpContext context)
        {
            System.Web.Script.Serialization.JavaScriptSerializer jsonSerializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            context.Response.ContentType = "application/json";
            var jsonResult = jsonSerializer.Serialize(new JosnResult() { Name = "Abraham Cheng", Age = 29 });// return what you want
            context.Response.Write(jsonResult);
        }
    }
    class JosnResult
    {
        public string Name { get; set; }
        public int Age { get; set; }
    }
}