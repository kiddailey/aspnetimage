using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ASPNetImage;
using System.Globalization;
using System.Data.SqlClient;
using System.Data;

namespace testWeb
{
    public partial class _Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            NetImage image = new NetImage();

            SqlConnection con = new SqlConnection("Server=(local);uid=turismo;pwd=$!tur1sm039;database=turismo2008");
            SqlDataAdapter da = new SqlDataAdapter("SELECT immagine FROM tab_immagini WHERE pk_immagine = 6469", con);

            SqlCommandBuilder MyCB = new SqlCommandBuilder(da);
            DataSet ds = new DataSet("MyImages");

            byte[] MyData = new byte[0];

            da.Fill(ds, "MyImages");
            DataRow myRow;
            myRow = ds.Tables["MyImages"].Rows[0];

            MyData = (byte[])myRow["immagine"];
            int ArraySize = new int();
            ArraySize = MyData.GetUpperBound(0);

            //image.LoadBlob(MyData, 1);
            image.LoadImage(Server.MapPath("prova.jpg"));
            image.FontSize = 100;
            image.FontName = "Arial";
            image.AntiAliasText = true;
            image.TextAngle = 0;
            image.TextOut("testo orizzontale", 1800, 1200, false);
            image.TextAngle = 45;
            image.TextOut("testo inclinato alto-basso", 1800, 1200, false);
            image.TextAngle = 90;
            image.TextOut("testo verticale alto-basso", 1800, 1200, false);
            image.TextAngle = 135;
            image.TextOut("testo rovesciato alto-basso", 1800, 1200, false);
            image.TextAngle = 180;
            image.TextOut("testo rovesciato orizzontale", 1800, 1200, false);
            image.TextAngle = 225;
            image.TextOut("testo rovesciato basso-alto", 1800, 1200, false);
            image.TextAngle = 270;
            image.TextOut("testo verticale basso-alto", 1800, 1200, false);
            image.TextAngle = 315;
            image.TextOut("testo inclinato basso-alto", 1800, 1200, false);

            //image.ResizeR(2000, 1000);
            //image.TextOut("chissa chi lo sa".ToString(), 200, 100, false);
            image.Filename = Server.MapPath("prova2.jpg");
            image.SaveImage();
            con.Close();
        }
    }
}
