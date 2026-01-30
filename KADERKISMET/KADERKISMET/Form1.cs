using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KADERKISMET
{
    public partial class Form1 : Form
    {

        [DllImport("user32.dll")]
        private static extern bool ReleaseCapture();

        [DllImport("user32.dll")]
        private static extern IntPtr SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

        private const int WM_NCLBUTTONDOWN = 0xA1;
        private const int HTCAPTION = 0x2;
        public Form1()
        {
            InitializeComponent();
        }

        private void label1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(this.Handle, WM_NCLBUTTONDOWN, HTCAPTION, 0);
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void button1_Click(object sender, EventArgs e)
        {
          
          frmbaslangic frm=new frmbaslangic();
            frm.Show();
            OleDbConnection conn = new OleDbConnection(con.baglan);
            conn.Open();
            OleDbCommand komutvaryokguncelle = new OleDbCommand("update TBLOGRENCILER SET OGRVARYOK=True", conn);
            komutvaryokguncelle.ExecuteNonQuery();

            conn.Close();
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (button2.Visible == true)
            {
                button2.Visible = false;
                this.WindowState = FormWindowState.Maximized;
                button5.Visible = true;
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (button5.Visible == true)
            {
                button5.Visible = false;
                this.WindowState = FormWindowState.Normal;
                button2.Visible = true;
            }
        }
        baglantisinif con=new baglantisinif();
        //---> frmbaslangic TAN GELEN DEĞİŞKENLER
        public string excelcon;
        public int sinif, sure1, sure2;
        public string ders, konu, test;

        private void button3_Click(object sender, EventArgs e)
        {
            
            kirmizisecim();
            mavisecim();
        }

        //Form1 DEĞİŞKENLERİ
        
        void kirmizisecim()
        {

            int secilenID = -1;
            string ogrAdSoyad = "";
            string ogrNumara = "";
            string ogrFotoYol = "";

            using (OleDbConnection conn = new OleDbConnection(con.baglan))
            {
                conn.Open();

                // 1️⃣ Şarta uyan TÜM öğrencileri çek
                string selectSql = @"
        SELECT 
               [ID],
               [OGRADSOYAD],
               [OGRNUMARA],
               [OGRFOTOYOL]
        FROM [TBLOGRENCILER]
        WHERE [OGRGRUP] = True
        AND [OGRYOKLAMA] = True
        AND [OGRVARYOK] = True
        AND [OGRSINIF] = ?";

                OleDbCommand cmd = new OleDbCommand(selectSql, conn);
                cmd.Parameters.AddWithValue("?", sinif);

                DataTable dt = new DataTable();
                OleDbDataAdapter da = new OleDbDataAdapter(cmd);
                da.Fill(dt);

                // 2️⃣ Rastgele seç
                if (dt.Rows.Count > 0)
                {
                    Random rnd = new Random();
                    int index = rnd.Next(dt.Rows.Count);

                    DataRow row = dt.Rows[index];

                    secilenID = Convert.ToInt32(row["ID"]);
                    ogrAdSoyad = row["OGRADSOYAD"].ToString();
                    ogrNumara = row["OGRNUMARA"].ToString();
                    ogrFotoYol = row["OGRFOTOYOL"].ToString();

                    // 3️⃣ Seçilen kişiyi havuzdan çıkar
                    OleDbCommand updateCmd = new OleDbCommand(
                        "UPDATE [TBLOGRENCILER] SET [OGRVARYOK] = False WHERE [ID] = ?", conn);
                    updateCmd.Parameters.AddWithValue("?", secilenID);
                    updateCmd.ExecuteNonQuery();
                }
                else
                {
                    pcksologr.Image = null;
                    lbladsoyadkirmizi.Text = "ÖĞRENCİ BİTTİ.";
                    lblnumarakirmizi.Text = null;
                    return;
                }
            }

            // 4️⃣ UI güncelle
            pcksologr.ImageLocation = ogrFotoYol;
            lbladsoyadkirmizi.Text = ogrAdSoyad;
            lblnumarakirmizi.Text = ogrNumara;
        }


        void mavisecim()
        {
            int secilenID = -1;
            string ogrAdSoyad = "";
            string ogrNumara = "";
            string ogrFotoYol = "";

            using (OleDbConnection conn = new OleDbConnection(con.baglan))
            {
                conn.Open();

                // 1️⃣ Şarta uyan TÜM öğrencileri çek
                string selectSql = @"
        SELECT 
               [ID],
               [OGRADSOYAD],
               [OGRNUMARA],
               [OGRFOTOYOL]
        FROM [TBLOGRENCILER]
        WHERE [OGRGRUP] = False
        AND [OGRYOKLAMA] = True
        AND [OGRVARYOK] = True
        AND [OGRSINIF] = ?";

                OleDbCommand cmd = new OleDbCommand(selectSql, conn);
                cmd.Parameters.AddWithValue("?", sinif);

                DataTable dt = new DataTable();
                OleDbDataAdapter da = new OleDbDataAdapter(cmd);
                da.Fill(dt);

                // 2️⃣ Rastgele seç
                if (dt.Rows.Count > 0)
                {
                    Random rnd = new Random();
                    int index = rnd.Next(dt.Rows.Count);

                    DataRow row = dt.Rows[index];

                    secilenID = Convert.ToInt32(row["ID"]);
                    ogrAdSoyad = row["OGRADSOYAD"].ToString();
                    ogrNumara = row["OGRNUMARA"].ToString();
                    ogrFotoYol = row["OGRFOTOYOL"].ToString();

                    // 3️⃣ Seçilen kişiyi havuzdan çıkar
                    OleDbCommand updateCmd = new OleDbCommand(
                        "UPDATE [TBLOGRENCILER] SET [OGRVARYOK] = False WHERE [ID] = ?", conn);
                    updateCmd.Parameters.AddWithValue("?", secilenID);
                    updateCmd.ExecuteNonQuery();
                }
                else
                {
                    pcksagogr.Image = null;
                    return;
                }
            }

            // 4️⃣ UI güncelle
            pcksagogr.ImageLocation = ogrFotoYol;
            //lbladsoyadkirmizi.Text = ogrAdSoyad;
            //lblnumarakirmizi.Text = ogrNumara;

        }


        
        private void Form1_Load(object sender, EventArgs e)
        {
           
            

            pictureBox1.Parent = pcksologr;
            pictureBox2.Parent = pcksagogr;
            
        }

        

    }
}
