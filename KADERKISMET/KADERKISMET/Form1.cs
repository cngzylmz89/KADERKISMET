using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.Linq;
using System.Reflection;
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
            sorugosterımguncelle();
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
        public int sinif, sure1, sure2,sure1basla;
        public string ders, konu, test;

        string asama;
        private void btnsorugetir_Click_1(object sender, EventArgs e)
        {
            sorugetir();       // ⛔️ EKSİK OLAN BUYDU

            if (string.IsNullOrEmpty(aktifSoru))
                return;

            rchsoru.Clear();   // Önce temizle
            harfIndex = 0;     // Baştan başlat
            timer1.Start();
            btnogrencigetir.Enabled = false;
           
        }
        //private void btnsorugetir_Click(object sender, EventArgs e)
        //{
            
        //}

        private void btnogrencigetir_Click(object sender, EventArgs e)
        {
            lblsure1.Text = "";
            sure1basla = sure1;
            kirmizisecim();
            mavisecim();
            rchsoru.Text = "";
            pictureBox1.Visible=false;
            pictureBox2.Visible=false;
            btnsorugetir.Enabled = true;
            
        }
        string aktifSoru = "";   // Çekilen soru burada tutulacak
        int harfIndex = 0;       // Yazdırılacak harfin sırası
        private void timer1_Tick(object sender, EventArgs e)
        {
            if (harfIndex < aktifSoru.Length)
            {
                rchsoru.AppendText(aktifSoru[harfIndex].ToString());
                harfIndex++;
            }
            else
            {
                timer1.Stop(); // Yazım bitince durdur
                btnsorugetir.Enabled=false;
                btnkirmiziogrsec.Enabled = true;
                btnmaviogrsec.Enabled=true;
                
                timer2.Start();

            }
        }
        void sorugosterımguncelle()
        {
            OleDbConnection conn = new OleDbConnection(excelcon);
            conn.Open();
            OleDbCommand sorugosterimguncelle = new OleDbCommand("update [TBLSORULAR$] SET GOSTERIM=0", conn);
            sorugosterimguncelle.ExecuteNonQuery();
            conn.Close();
        }

        private void btnkirmiziogrsec_Click(object sender, EventArgs e)
        {
            if (pictureBox1.Visible == false)
            {
                pictureBox1.Visible= true;
            }
            if (pictureBox2.Visible == true)
            {
                pictureBox2.Visible = false;
            }
            timer2.Stop();
            if (sure2 >= 0)
            {

                timer3.Start();
            }
            btndogru.Enabled = true;
            btnyanlis.Enabled = true;
            btnmaviogrsec.Enabled = false;
            btnkirmiziogrsec.Enabled=false;
        }

        private void btnmaviogrsec_Click(object sender, EventArgs e)
        {
            if (pictureBox2.Visible == false)
            {
                pictureBox2 .Visible= true;
            }
            if(pictureBox1.Visible == true)
                { pictureBox1.Visible= false; }

            timer2.Stop();
            if(sure2> 0)
                {
                
                timer3.Start();
             }

            btndogru.Enabled = true;
            btnyanlis.Enabled = true;
            btnkirmiziogrsec.Enabled = false;
            btnmaviogrsec.Enabled=false;
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            sure1basla--;   // 1 saniye düş

            lblsure1.Text = sure1basla.ToString();

            if (sure1basla <= 0)
            {
                timer2.Stop();
            }
        }

        private void timer3_Tick(object sender, EventArgs e)
        {
            sure2--;   // 1 saniye düş

            lblsure2.Text = sure2.ToString();

            if (sure2<= 0)
            {
                timer3.Stop();
            }
        }

        private void btndogru_Click(object sender, EventArgs e)
        {
            if(btnkirmiziogrsec.Enabled==true||btnmaviogrsec.Enabled==true)
            {
                btnmaviogrsec.Enabled=false;
                btnkirmiziogrsec.Enabled = false;
            }
            btnyanlis.Enabled=false;
            btndogru.Enabled=false;
            btnogrencigetir.Enabled = true;
        }

        private void btnyanlis_Click(object sender, EventArgs e)
        {
            if (btnkirmiziogrsec.Enabled == true || btnmaviogrsec.Enabled == true)
            {
                btnmaviogrsec.Enabled = false;
                btnkirmiziogrsec.Enabled = false;
            }
            btnyanlis.Enabled= false;
            btndogru.Enabled = false;
            btnogrencigetir.Enabled = true;
        }

        void sorugetir()
        {
            int secilenID = -1;
            string soru= "";
            int dogruyanlis;
            

            using (OleDbConnection conn = new OleDbConnection(excelcon))
            {
                conn.Open();

                // 1️⃣ Şarta uyan TÜM öğrencileri çek
                string selectSql = @"
        SELECT 
               [SORUID],
               [SORU],
[DOGRUYANLIS]
            
        FROM [TBLSORULAR$]
        WHERE [DERS] = @P1
        AND [KONU] = @P2
        AND [TESTADI] = @P3
        AND [GOSTERIM] = 0";

                OleDbCommand cmd = new OleDbCommand(selectSql, conn);
                cmd.Parameters.AddWithValue("@P1", ders);
                cmd.Parameters.AddWithValue("@P2", konu);
                cmd.Parameters.AddWithValue("@P3", test);
                

                DataTable dt = new DataTable();
                OleDbDataAdapter da = new OleDbDataAdapter(cmd);
                da.Fill(dt);

                // 2️⃣ Rastgele seç
                if (dt.Rows.Count > 0)
                {
                    Random rnd = new Random();
                    int index = rnd.Next(dt.Rows.Count);

                    DataRow row = dt.Rows[index];

                    secilenID = Convert.ToInt32(row["SORUID"]);
                    aktifSoru = row["SORU"].ToString();
                    dogruyanlis = int.Parse(row["DOGRUYANLIS"].ToString());
                    
                    // 3️⃣ Seçilen soruyu havuzdan çıkar
                    OleDbCommand updateCmd = new OleDbCommand(
                        "UPDATE [TBLSORULAR$] SET [GOSTERIM] = 1 WHERE [SORUID] = ?", conn);
                    updateCmd.Parameters.AddWithValue("?", secilenID);
                    updateCmd.ExecuteNonQuery();
                }
                else 
                {
                    rchsoru.Text = "SORU KALMADI";
                    return;
                }
            }

            // 4️⃣ UI güncelle
          
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
                    lbladsoyadmavi.Text = "ÖĞRENCİ BİTTİ.";
                    lblnumaramavi.Text = null;

                    return;
                }
            }

            // 4️⃣ UI güncelle
            pcksagogr.ImageLocation = ogrFotoYol;
            lbladsoyadmavi.Text= ogrAdSoyad;
            lblnumaramavi.Text= ogrNumara;
         
        }


        
        private void Form1_Load(object sender, EventArgs e)
        {
            timer3.Interval = 1000; // 1 saniye
            timer2.Interval = 1000; // 1 saniye
            timer1.Interval = 150; // 50 ms = hızlı yazım (istersen artır)
            timer1.Tick += timer1_Tick;
            pictureBox1.Visible= false;
            pictureBox2.Visible= false;
            pictureBox1.Parent = pcksologr;
            pictureBox2.Parent = pcksagogr;

            panelCerceve.Padding = new Padding(10);   // ← Border kalınlığı gibi davranır
            panelCerceve.BackColor = Color.FromArgb(160, 58, 19); ;    // Çerçeve rengi

            rchsoru.Parent = panelCerceve;
            rchsoru.BorderStyle = BorderStyle.None;
            rchsoru.Dock = DockStyle.Fill;

           



        }



    }
}
