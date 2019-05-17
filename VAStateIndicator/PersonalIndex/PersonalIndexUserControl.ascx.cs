using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;

namespace VAStateIndicator.PersonalIndex
{
    public partial class PersonalIndexUserControl : UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Graphics objGraphics;//建立画板对象
            Bitmap objBitMap = new Bitmap(600, 300);//建立位图对象
            objGraphics = Graphics.FromImage(objBitMap);//根据位图对象建立画板对象
            objGraphics.Clear(Color.White);//设置画板对象的背景色
            int[] arrValues = { 0, 0, 0, 0, 0, 0 };//数据数组
            arrValues[0] = 50;
            arrValues[1] = 70;
            arrValues[2] = 90;
            arrValues[3] = 100;
            arrValues[4] = 140;
            arrValues[5] = 220;
            string[] arrValueNames = {"0","0","0","0","0","0"};//月份

            arrValueNames[0] = "一月";
            arrValueNames[1] ="二月";
            arrValueNames[2] = "三月";
            arrValueNames[3] ="四月";
            arrValueNames[4] ="五月";
            arrValueNames[5] ="六月";

            objGraphics.DrawString("上半年销售情况统计", new Font("宋体", 16), Brushes.Black, new PointF(0, 0));

            //创建图例文字
            PointF symbolLeg = new PointF(335, 20);
            PointF descLeg = new PointF(360, 16);

            //画出图例。利用objGraphics图形对象的三个方法画出图例：
            //FillRectangle()方法画出填充矩形，DrawRectangle()方法画出矩形的边框，
            //DrawString()方法画出说明文字。这三个图形对象的方法在 .NET 框架类库类库中均已重载，
            //可以很方便根据不同的参数来画出图形。

            for (int i = 0; i< arrValueNames.Length;i++)
            {
                objGraphics.FillRectangle(new SolidBrush(GetColor(i)), symbolLeg.X, symbolLeg.Y, 20, 10);

                objGraphics.DrawRectangle(Pens.Black, symbolLeg.X, symbolLeg.Y, 20, 10);

                objGraphics.DrawString(arrValueNames[i].ToString(), new Font("宋体", 10), Brushes.Black, descLeg);

                symbolLeg.Y += 15;
                descLeg.Y += 15;
            }

            for (int j = 0; j< arrValues.Length;j++)
            {
                objGraphics.FillRectangle(new SolidBrush(GetColor(j)), (j * 35) + 15, 200 - arrValues[j], 20, arrValues[j] + 5);
                objGraphics.DrawRectangle(Pens.Black, (j * 35) + 15, 200 - arrValues[j], 20, arrValues[j] + 5);
            }

            float sglCurrentAngle;
            float sglTotalAngle = 0;

            for (int a = 0; a < arrValues.Length; a++)
            {
                sglTotalAngle += arrValues[a];//取得数据总量 
            }

            float startAngle = 0;
            for (int b = 0; b < arrValues.Length; b++)
            {
                sglCurrentAngle = arrValues[b] / sglTotalAngle * 360;//求出该数据所占总数据的百分比 
                objGraphics.FillPie(new SolidBrush(GetColor(b)), 220, 95, 100, 100, startAngle, sglCurrentAngle);//画出椭圆 
                startAngle += sglCurrentAngle;
            }



            objBitMap.Save(Response.OutputStream, ImageFormat.Gif);//该位图对象以"GIF"格式输出

        }

        private Color GetColor(int itemIndex)
        {
            Color objColor = new Color();
            switch (itemIndex)
            {
                case 0:
                    objColor = Color.Blue;
                    break;
                case 1:
                    objColor = Color.Yellow;
                    break;
                case 2:
                    objColor = Color.Red;
                    break;
                case 3:
                    objColor = Color.Orange;
                    break;
                case 4:
                    objColor = Color.Purple;
                    break;
                case 5:
                    objColor = Color.Brown;
                    break;
                case 6:

                default:
                    objColor = Color.Blue;
                    break;
            }

            return objColor;
        }
    }
}
