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
using System.Media;

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

        SoundPlayer dogruSes;
        SoundPlayer yanlisSes;
        SoundPlayer kazananSes;
        SoundPlayer berabereSes;
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
        public int sinif, sure1, sure2,sure1basla, sure2basla;
        public string ders, konu, test;
        int kirmiziskor, maviskor;
        bool dogruyanlis;
        bool kirmiziogrencisecim, maviogrencisecim;

        bool kirmiziSecildi = false;
        bool maviSecildi = false;

        int zoomToplamSure = 600;   // Varsayılan (ms) — sonra sesi göre ayarlayacağız
        int zoomGecenSure = 0;

        Timer iconZoomTimer = new Timer();
        PictureBox zoomPic = null;
        float zoomSize = 1f;
        bool zoomBuyut = true;


        bool cevapVerildi = false;

        // 🔽 DOĞRU / YANLIŞ GÖSTERİMİ İÇİN (opsiyonel ama okunaklı)
        PictureBox AktifDogruPic;
        PictureBox AktifYanlisPic;

        int orijinalFontSize = 14;   // (Label'larının mevcut font size'ı neyse onu yaz)
        Timer animTimer = new Timer();
        int animAdim = 0;
        Label animLabel = null;
        float baslangicFontSize = 0;

        // === RICHTEXTBOX İÇİN KAZANAN ANİMASYONU ===
        Timer rchKazanAnimTimer = new Timer();
        float rchKazanFontSize;
        int rchKazanAnimAdim = 0;

        enum Takim
        {
            Yok = 0,
            Kirmizi = 1,
            Mavi = 2
        }

        Takim aktifTakim = Takim.Yok;
        bool soruCevaplandi = false;

        List<int> secilenKirmiziIDs = new List<int>();
        List<int> secilenMaviIDs = new List<int>();



        string asama;

        void KazananYazisiniRchsoruyaAnimasyonluYaz(string metin, Color renk)
        {
            // RichTextBox’u temizle ve ayarla
            rchsoru.Clear();
            rchsoru.SelectionAlignment = HorizontalAlignment.Center;
            rchsoru.ForeColor = renk;
            rchsoru.Font = new Font("Segoe UI", 20, FontStyle.Bold); // başlangıç boyutu
            rchsoru.Text = metin;

            rchKazanFontSize = rchsoru.Font.Size;
            rchKazanAnimAdim = 0;

            rchKazanAnimTimer.Interval = 30;
            rchKazanAnimTimer.Tick -= RchKazanAnimTimer_Tick;
            rchKazanAnimTimer.Tick += RchKazanAnimTimer_Tick;
            rchKazanAnimTimer.Start();
        }

        private void RchKazanAnimTimer_Tick(object sender, EventArgs e)
        {
            rchKazanAnimAdim++;

            if (rchKazanAnimAdim <= 16) // BÜYÜT
            {
                rchsoru.Font = new Font(
                    rchsoru.Font.FontFamily,
                    rchKazanFontSize + (rchKazanAnimAdim / 2f),
                    FontStyle.Bold);
            }
            else if (rchKazanAnimAdim <= 32) // KÜÇÜLT
            {
                float yeniSize =
                    rchKazanFontSize + (16 - (rchKazanAnimAdim - 16)) / 2f;

                rchsoru.Font = new Font(
                    rchsoru.Font.FontFamily,
                    Math.Max(rchKazanFontSize, yeniSize),
                    FontStyle.Regular);
            }
            else
            {
                rchsoru.Font = new Font(
                    rchsoru.Font.FontFamily,
                    rchKazanFontSize,
                    FontStyle.Regular);

                rchKazanAnimTimer.Stop();
            }
        }
        private void btnsorugetir_Click_1(object sender, EventArgs e)
        {
            sorugetir();   // Önce soru çek

            if (string.IsNullOrEmpty(aktifSoru))
            {
                // Sorular gerçekten bitti → artık sormadan oyunu bitiriyoruz
                btnsorugetir.Enabled = false;

                string sonucMesaji;
                Color sonucRengi;

                if (kirmiziskor > maviskor)
                {
                   
                    kazananSes.Play();
                    sonucMesaji = "🔴 KIRMIZI TAKIM KAZANDI!";
                    sonucRengi = Color.Red;
                }
                else if (maviskor > kirmiziskor)
                {
                  
                    kazananSes.Play();
                    sonucMesaji = "🔵 MAVİ TAKIM KAZANDI!";
                    sonucRengi = Color.Blue;
                }
                else
                {
                    
                    berabereSes.Play();
                    sonucMesaji = "⚖️ OYUN BERABERE!";
                    sonucRengi = Color.DarkGoldenrod;
                }

                // === KAZANANI ANİMASYONLU GÖSTER ===
                KazananYazisiniRchsoruyaAnimasyonluYaz(sonucMesaji, sonucRengi);

                // ======= SENİN İSTEDİĞİN EK TEMİZLİK (YENİ) =======

                // Öğrenci resimlerini temizle
                pcksologr.Image = null;
                pcksagogr.Image = null;

                // Kırmızı takım bilgilerini temizle
                lbladsoyadkirmizi.Text = "";
                lblnumarakirmizi.Text = "";

                // Mavi takım bilgilerini temizle
                lbladsoyadmavi.Text = "";
                lblnumaramavi.Text = "";

                // ======= GÜVENLİK: DİĞER BUTONLARI KAPAT =======
                btnogrencigetir.Enabled = false;
                btnkirmiziogrsec.Enabled = false;
                btnmaviogrsec.Enabled = false;
                btndogru.Enabled = false;
                btnyanlis.Enabled = false;

                return;
            }

            // Normal akış (soru varsa)
            rchsoru.Clear();
            harfIndex = 0;
            timer1.Start();
            btnogrencigetir.Enabled = false;

        }
        //private void btnsorugetir_Click(object sender, EventArgs e)
        //{
            
        //}

        private void btnogrencigetir_Click(object sender, EventArgs e)
        {
            //secilenKirmiziIDs.Clear();
            //secilenMaviIDs.Clear();

            iconZoomTimer.Stop();
            zoomPic = null;

            pictureBoxkirmizidogru.Visible = false;
            pictureBoxkirmiziyanlis.Visible = false;
            pictureBoxmavidogru.Visible = false;
            pictureBoxmaviyanlis.Visible = false;
            kirmiziSecildi = false;
            maviSecildi = false;
            cevapVerildi = false;
            aktifTakim = Takim.Yok;
            soruCevaplandi = false;
            lblmaviskor.Text = maviskor + " PUAN";
            lblkirmiziskor.Text = kirmiziskor + " PUAN";
            lblsure1.Text = "";
            lblsure2.Text = "";
            sure2basla = sure2;
            sure1basla = sure1;
            kirmizisecim();
            mavisecim();
            rchsoru.Text = "";
            pictureBox1.Visible=false;
            pictureBox2.Visible=false;
            btnsorugetir.Enabled = true;
            btnogrencigetir.Enabled = false;
            
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
            kirmiziSecildi = true;
            maviSecildi = false;
            aktifTakim = Takim.Kirmizi;
            soruCevaplandi = false;

            pictureBox1.Visible = true;
            pictureBox2.Visible = false;

            timer2.Stop();
            timer3.Start();

            AktifDogruPic = pictureBoxkirmizidogru;
            AktifYanlisPic = pictureBoxkirmiziyanlis;

            pictureBoxkirmizidogru.Visible = false;
            pictureBoxkirmiziyanlis.Visible = false;

            btndogru.Enabled = true;
            btnyanlis.Enabled = true;

            btnkirmiziogrsec.Enabled = false;
            btnmaviogrsec.Enabled = false;

        }

        private void btnmaviogrsec_Click(object sender, EventArgs e)
        {
            maviSecildi = true;
            kirmiziSecildi = false;
            aktifTakim = Takim.Mavi;
            soruCevaplandi = false;

            pictureBox2.Visible = true;
            pictureBox1.Visible = false;

            timer2.Stop();
            timer3.Start();

            AktifDogruPic = pictureBoxmavidogru;
            AktifYanlisPic = pictureBoxmaviyanlis;

            pictureBoxmavidogru.Visible = false;
            pictureBoxmaviyanlis.Visible = false;

            btndogru.Enabled = true;
            btnyanlis.Enabled = true;

            btnkirmiziogrsec.Enabled = false;
            btnmaviogrsec.Enabled = false;
        }
        void PuanVer(bool cevapDogru)
        {
            // Önce tüm işaretleri gizle
            if (AktifDogruPic != null) AktifDogruPic.Visible = false;
            if (AktifYanlisPic != null) AktifYanlisPic.Visible = false;
            if (aktifTakim == Takim.Yok) return;
            if (soruCevaplandi) return;

            bool cevapDogruMu = (cevapDogru == dogruyanlis);

            Takim kazananTakim;

            if (cevapDogruMu)
            {
                if (AktifDogruPic != null)
                {
                    AktifDogruPic.Visible = true;
                    ZoomAnimasyonBaslat(AktifDogruPic,
                        System.IO.Path.Combine(Application.StartupPath, "Sounds", "dogru.wav"));
                }
                kazananTakim = aktifTakim;
                dogruSes.Play();   // ✅ DOĞRU SESİ
            }
            else
            {
                if (AktifYanlisPic != null)
                {
                    AktifYanlisPic.Visible = true;
                    ZoomAnimasyonBaslat(AktifYanlisPic,
                        System.IO.Path.Combine(Application.StartupPath, "Sounds", "yanlis.wav"));
                }
            
            kazananTakim = (aktifTakim == Takim.Kirmizi)
                                ? Takim.Mavi
                                : Takim.Kirmizi;

                yanlisSes.Play();  // ❌ YANLIŞ SESİ
            }

            if (kazananTakim == Takim.Kirmizi)
            {
                kirmiziskor += 10;
                lblkirmiziskor.Text = kirmiziskor + " PUAN";
                PuanAnimasyonBaslat(lblkirmiziskor);   // 🔥 ANİMASYON
            }
            else
            {
                maviskor += 10;
                lblmaviskor.Text = maviskor + " PUAN";
                PuanAnimasyonBaslat(lblmaviskor);   // 🔥 ANİMASYON
            }

            soruCevaplandi = true;

        }
        private void timer2_Tick(object sender, EventArgs e)
        {
            sure1basla--;
            lblsure1.Text = sure1basla.ToString();

            if (sure1basla <= 0)
            {
                timer2.Stop();

                // 🔴 HİÇBİR TAKIM SEÇİLMEDİYSE → İKİ TAKIMA CEZA
                if (aktifTakim == Takim.Yok)
                {
                    //kirmiziskor -= 10;
                    //maviskor -= 10;

                    lblkirmiziskor.Text = kirmiziskor + " PUAN";
                    lblmaviskor.Text = maviskor + " PUAN";

                    // ❌ BUNU SİL:
                    // PuanAnimasyonBaslat(lblkirmiziskor);
                    // PuanAnimasyonBaslat(lblmaviskor);

                    // ✅ YENİSİNİ KOY:
                    //PuanAnimasyonIkisi(lblkirmiziskor, lblmaviskor);

                    yanlisSes.Play();   // ❌ CEZA SESİ
                }

                // Butonları doğru ayarla
                btnogrencigetir.Enabled = true;
                btnkirmiziogrsec.Enabled = false;
                btnmaviogrsec.Enabled = false;
                btndogru.Enabled = false;
                btnyanlis.Enabled = false;
                
            }
        }

        private void timer3_Tick(object sender, EventArgs e)
        {
            sure2basla--;
            lblsure2.Text = sure2basla.ToString();

            if (sure2basla <= 0)
            {
                timer3.Stop();

                // 🔴 Takım seçilmiş ama cevap verilmemişse CEZA
                if (!soruCevaplandi && aktifTakim != Takim.Yok)
                {
                    if (aktifTakim == Takim.Kirmizi)
                    {
                        kirmiziskor -= 10;
                        lblkirmiziskor.Text = kirmiziskor + " PUAN";

                        //// ✅ ANİMASYON
                        //PuanAnimasyonBaslat(lblkirmiziskor);
                    }
                    else if (aktifTakim == Takim.Mavi)
                    {
                        maviskor -= 10;
                        lblmaviskor.Text = maviskor + " PUAN";

                        //// ✅ ANİMASYON
                        //PuanAnimasyonBaslat(lblmaviskor);
                    }

                    yanlisSes.Play();   // ❌ CEZA SESİ
                }

                // Butonları düzenle
                btnogrencigetir.Enabled = true;
                btndogru.Enabled = false;
                btnyanlis.Enabled = false;
                
            }
        }

        private void btndogru_Click(object sender, EventArgs e)
        {
           
            cevapVerildi = true;
            timer3.Stop();

            PuanVer(true);   // "Doğru" butonuna basıldı

            btndogru.Enabled = false;
            btnyanlis.Enabled = false;
            btnogrencigetir.Enabled = true;
            
        }

        private void btnyanlis_Click(object sender, EventArgs e)
        {
           
            cevapVerildi = true;
            timer3.Stop();

            PuanVer(false);  // "Yanlış" butonuna basıldı

            btndogru.Enabled = false;
            btnyanlis.Enabled = false;
            btnogrencigetir.Enabled = true;
        }

        void sorugetir()
        {
            int secilenID = -1;
            string soru= "";
           
            

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
                    dogruyanlis = Convert.ToBoolean(row["DOGRUYANLIS"]);

                    // 3️⃣ Seçilen soruyu havuzdan çıkar
                    OleDbCommand updateCmd = new OleDbCommand(
                        "UPDATE [TBLSORULAR$] SET [GOSTERIM] = 1 WHERE [SORUID] = ?", conn);
                    updateCmd.Parameters.AddWithValue("?", secilenID);
                    updateCmd.ExecuteNonQuery();
                }
                else 
                {
                    aktifSoru = "";
                    rchsoru.Clear();
                    rchsoru.Text = "SORULAR BİTTİ ❗";

                    // Butonları da güvenli hale getirelim
                    btnsorugetir.Enabled = false;
                    btnogrencigetir.Enabled = true;
                    btnkirmiziogrsec.Enabled = false;
                    btnmaviogrsec.Enabled = false;
                    btndogru.Enabled = false;
                    btnyanlis.Enabled = false;

                    return;
                }
            }

            // 4️⃣ UI güncelle
          
        }


        //Form1 DEĞİŞKENLERİ
        void KirmiziOgrenciGoster(int id)
        {
            using (OleDbConnection conn = new OleDbConnection(con.baglan))
            {
                conn.Open();   // 🔥 BUNU EKLEDİK

                OleDbCommand cmd = new OleDbCommand(
                    "SELECT [OGRADSOYAD],[OGRNUMARA],[OGRFOTOYOL] FROM [TBLOGRENCILER] WHERE [ID] = ?", conn);

                cmd.Parameters.AddWithValue("?", id);

                OleDbDataReader dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    pcksologr.ImageLocation = dr["OGRFOTOYOL"].ToString();
                    lbladsoyadkirmizi.Text = dr["OGRADSOYAD"].ToString();
                    lblnumarakirmizi.Text = dr["OGRNUMARA"].ToString();
                }
                dr.Close();
            }
        }

        void MaviOgrenciGoster(int id)
        {
            using (OleDbConnection conn = new OleDbConnection(con.baglan))
            {
                conn.Open();   // 🔥 BUNU EKLEDİK

                OleDbCommand cmd = new OleDbCommand(
                    "SELECT [OGRADSOYAD],[OGRNUMARA],[OGRFOTOYOL] FROM [TBLOGRENCILER] WHERE [ID] = ?", conn);

                cmd.Parameters.AddWithValue("?", id);

                OleDbDataReader dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    pcksagogr.ImageLocation = dr["OGRFOTOYOL"].ToString();
                    lbladsoyadmavi.Text = dr["OGRADSOYAD"].ToString();
                    lblnumaramavi.Text = dr["OGRNUMARA"].ToString();
                }
                dr.Close();
            }
        }
        void kirmizisecim()
        {

            int secilenID = -1;
            string ogrAdSoyad = "";
            string ogrNumara = "";
            string ogrFotoYol = "";

            using (OleDbConnection conn = new OleDbConnection(con.baglan))
            {
                conn.Open();

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

                if (dt.Rows.Count > 0)
                {
                    Random rnd = new Random();
                    int index = rnd.Next(dt.Rows.Count);

                    DataRow row = dt.Rows[index];

                    secilenID = Convert.ToInt32(row["ID"]);
                    ogrAdSoyad = row["OGRADSOYAD"].ToString();
                    ogrNumara = row["OGRNUMARA"].ToString();
                    ogrFotoYol = row["OGRFOTOYOL"].ToString();

                    OleDbCommand updateCmd = new OleDbCommand(
                        "UPDATE [TBLOGRENCILER] SET [OGRVARYOK] = False WHERE [ID] = ?", conn);
                    updateCmd.Parameters.AddWithValue("?", secilenID);
                    updateCmd.ExecuteNonQuery();

                    secilenKirmiziIDs.Add(secilenID);
                }
                else
                {
                    // 🔥 HAVUZ BİTTİ → KENDİ TAKIMINDAN TEKRAR GETİR
                    if (secilenKirmiziIDs.Count > 0)
                    {
                        int tekrarID = secilenKirmiziIDs[0];

                        // Döngü yapalım (aynı kişi hep gelmesin)
                        secilenKirmiziIDs.RemoveAt(0);
                        secilenKirmiziIDs.Add(tekrarID);

                        KirmiziOgrenciGoster(tekrarID);
                        return;
                    }

                    // Gerçekten kimse yoksa
                    pcksologr.Image = null;
                    lbladsoyadkirmizi.Text = "ÖĞRENCİ BİTTİ.";
                    lblnumarakirmizi.Text = null;
                    return;
                }
            }

            // Normal yeni seçim varsa UI güncelle
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

                if (dt.Rows.Count > 0)
                {
                    Random rnd = new Random();
                    int index = rnd.Next(dt.Rows.Count);

                    DataRow row = dt.Rows[index];

                    secilenID = Convert.ToInt32(row["ID"]);
                    ogrAdSoyad = row["OGRADSOYAD"].ToString();
                    ogrNumara = row["OGRNUMARA"].ToString();
                    ogrFotoYol = row["OGRFOTOYOL"].ToString();

                    OleDbCommand updateCmd = new OleDbCommand(
                        "UPDATE [TBLOGRENCILER] SET [OGRVARYOK] = False WHERE [ID] = ?", conn);
                    updateCmd.Parameters.AddWithValue("?", secilenID);
                    updateCmd.ExecuteNonQuery();

                    secilenMaviIDs.Add(secilenID);
                }
                else
                {
                    // 🔥 HAVUZ BİTTİ → KENDİ TAKIMINDAN TEKRAR GETİR
                    if (secilenMaviIDs.Count > 0)
                    {
                        int tekrarID = secilenMaviIDs[0];

                        // Döngü yapalım
                        secilenMaviIDs.RemoveAt(0);
                        secilenMaviIDs.Add(tekrarID);

                        MaviOgrenciGoster(tekrarID);
                        return;
                    }

                    pcksagogr.Image = null;
                    lbladsoyadmavi.Text = "ÖĞRENCİ BİTTİ.";
                    lblnumaramavi.Text = null;
                    return;
                }
            }

            // Normal yeni seçim varsa UI güncelle
            pcksagogr.ImageLocation = ogrFotoYol;
            lbladsoyadmavi.Text = ogrAdSoyad;
            lblnumaramavi.Text = ogrNumara;

        }
        void PuanAnimasyonIkisi(Label lbl1, Label lbl2)
        {
            // Önce kırmızıyı çalıştır
            PuanAnimasyonBaslat(lbl1);

            // Maviyi çok kısa gecikmeyle başlat
            Timer gecikme = new Timer();
            gecikme.Interval = 200; // 0.2 saniye
            gecikme.Tick += (s, e) =>
            {
                gecikme.Stop();
                PuanAnimasyonBaslat(lbl2);
            };
            gecikme.Start();
        }
        void PuanAnimasyonBaslat(Label lbl)
        {
            animLabel = lbl;
            animAdim = 0;

            // ÖNEMLİ: Mevcut (doğru) font boyutunu kaydet
            baslangicFontSize = lbl.Font.Size;

            animTimer.Start();
        }

        private void AnimTimer_Tick(object sender, EventArgs e)
        {
            if (animLabel == null) return;

            animAdim++;

            if (animAdim <= 16) // BÜYÜT
            {
                animLabel.Font = new Font(
                    animLabel.Font.FontFamily,
                    baslangicFontSize + (animAdim / 2f),   // yumuşak büyütme
                    FontStyle.Bold);
            }
            else if (animAdim <= 32) // KÜÇÜLT (ESKİ HALİNE DÖN)
            {
                float yeniSize =
                    baslangicFontSize + (16 - (animAdim - 16)) / 2f;

                animLabel.Font = new Font(
                    animLabel.Font.FontFamily,
                    Math.Max(baslangicFontSize, yeniSize),
                    FontStyle.Regular);
            }
            else
            {
                // ✅ TAM OLARAK İLK HALİNE GETİR
                animLabel.Font = new Font(
                    animLabel.Font.FontFamily,
                    baslangicFontSize,
                    FontStyle.Regular);

                animTimer.Stop();
                animLabel = null;
            }
        }
        void ZoomAnimasyonBaslat(PictureBox pck, string sesYolu)
        {
            if (pck == null) return;

            zoomPic = pck;
            zoomGecenSure = 0;
            zoomToplamSure = 1000;   // 🔥 ARTIK SESLE BAĞLANTILI DEĞİL
            //zoomToplamSure = (int)SesSuresiMilisaniye(sesYolu);

            zoomSize = 1f;
            zoomBuyut = true;

            iconZoomTimer.Start();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            DialogResult result1=MessageBox.Show("Yeni oyun kurulması için başlangıç sayfasına gidilecek onaylıyor musunuz?","Bilgi", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result1 == DialogResult.Yes)
            {
                frmbaslangic frm = new frmbaslangic();
                frm.Show();
                OleDbConnection conn = new OleDbConnection(con.baglan);
                conn.Open();
                OleDbCommand komutvaryokguncelle = new OleDbCommand("update TBLOGRENCILER SET OGRVARYOK=True", conn);
                komutvaryokguncelle.ExecuteNonQuery();

                conn.Close();
                sorugosterımguncelle();
                this.Close();
            }
        }

        private void IconZoomTimer_Tick(object sender, EventArgs e)
        {
            if (zoomPic == null) return;

            zoomGecenSure += iconZoomTimer.Interval;

            float oran = (float)zoomGecenSure / zoomToplamSure; // 0 → 1 arası

            if (oran < 0.5f)   // İlk yarı: büyüt
            {
                zoomSize = 1f + (0.3f * (oran * 2)); // 1 → 1.3 arası
            }
            else               // İkinci yarı: küçült
            {
                zoomSize = 1.3f - (0.3f * ((oran - 0.5f) * 2));
            }

            int orijinalBoyut = zoomPic.Tag is int ? (int)zoomPic.Tag : zoomPic.Width;

            zoomPic.Width = (int)(orijinalBoyut * zoomSize);
            zoomPic.Height = (int)(orijinalBoyut * zoomSize);

            if (zoomGecenSure >= zoomToplamSure)
            {
                zoomPic.Width = orijinalBoyut;
                zoomPic.Height = orijinalBoyut;

                iconZoomTimer.Stop();
                zoomPic = null;
            }
        }
        double SesSuresiMilisaniye(string dosyaYolu)
        {
            using (var reader = new System.Media.SoundPlayer(dosyaYolu))
            {
                var wav = new System.IO.FileInfo(dosyaYolu);
                using (var fs = wav.OpenRead())
                {
                    // WAV süresini yaklaşık hesapla
                    return (fs.Length / 176.4); // 44.1kHz için yaklaşık ms
                }
            }
        }
        Timer kazanAnimTimer = new Timer();
        float kazanFontSize;
        int kazanAnimAdim = 0;

        void KazananLabelAnimasyonBaslat(Label lbl)
        {
            kazanAnimAdim = 0;
            kazanFontSize = lbl.Font.Size;

            kazanAnimTimer.Interval = 30;
            kazanAnimTimer.Tick -= KazanAnimTimer_Tick;
            kazanAnimTimer.Tick += KazanAnimTimer_Tick;

            kazanAnimTimer.Start();
        }

        private void KazanAnimTimer_Tick(object sender, EventArgs e)
        {
            kazanAnimAdim++;

            if (kazanAnimAdim <= 16) // BÜYÜT
            {
                lblkazanantakim.Font = new Font(
                    lblkazanantakim.Font.FontFamily,
                    kazanFontSize + (kazanAnimAdim / 2f),
                    FontStyle.Bold);
            }
            else if (kazanAnimAdim <= 32) // KÜÇÜLT
            {
                float yeniSize =
                    kazanFontSize + (16 - (kazanAnimAdim - 16)) / 2f;

                lblkazanantakim.Font = new Font(
                    lblkazanantakim.Font.FontFamily,
                    Math.Max(kazanFontSize, yeniSize),
                    FontStyle.Regular);
            }
            else
            {
                lblkazanantakim.Font = new Font(
                    lblkazanantakim.Font.FontFamily,
                    kazanFontSize,
                    FontStyle.Regular);

                kazanAnimTimer.Stop();
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

           



            animTimer.Interval = 30; // Animasyon hızı
            animTimer.Tick += AnimTimer_Tick;

            iconZoomTimer.Interval = 30;
            iconZoomTimer.Tick += IconZoomTimer_Tick;


            string sesKlasoru = System.IO.Path.Combine(Application.StartupPath, "Sounds");

            if (!System.IO.Directory.Exists(sesKlasoru))
            {
                MessageBox.Show("Sounds klasörü bulunamadı:\n" + sesKlasoru);
            }

            string dogruYol = System.IO.Path.Combine(sesKlasoru, "dogru.wav");
            string yanlisYol = System.IO.Path.Combine(sesKlasoru, "yanlis.wav");
            string kazananYol = System.IO.Path.Combine(sesKlasoru, "kazanan.wav");
            string berabereYol = System.IO.Path.Combine(sesKlasoru, "berabeser.wav");

           
            kazananSes = new SoundPlayer(kazananYol);
            berabereSes = new SoundPlayer(berabereYol);

            if (!System.IO.File.Exists(dogruYol))
                MessageBox.Show("dogru.wav bulunamadı:\n" + dogruYol);

            if (!System.IO.File.Exists(yanlisYol))
                MessageBox.Show("yanlis.wav bulunamadı:\n" + yanlisYol);

            dogruSes = new SoundPlayer(dogruYol);
            yanlisSes = new SoundPlayer(yanlisYol);

            //MessageBox.Show(System.IO.Path.Combine(Application.StartupPath, "Sounds", "dogru.wav"));

            kirmiziskor = 0;
            maviskor= 0;
            kirmiziogrencisecim = true;
            maviogrencisecim= true;
            timer3.Interval = 1000; // 1 saniye
            timer2.Interval = 1000; // 1 saniye
            timer1.Interval = 150; // 50 ms = hızlı yazım (istersen artır)
            timer1.Tick += timer1_Tick;
            pictureBox1.Visible= false;
            pictureBox2.Visible= false;
            pictureBoxmavidogru.Parent = pcksagogr;
            pictureBoxmaviyanlis.Parent = pcksagogr;
            pictureBoxkirmizidogru.Parent = pcksologr;
            pictureBoxkirmiziyanlis.Parent = pcksologr;

            pictureBoxkirmizidogru.Tag = pictureBoxkirmizidogru.Width;
            pictureBoxkirmiziyanlis.Tag = pictureBoxkirmiziyanlis.Width;
            pictureBoxmavidogru.Tag = pictureBoxmavidogru.Width;
            pictureBoxmaviyanlis.Tag = pictureBoxmaviyanlis.Width;

            pictureBox1.Parent = pcksologr;
            pictureBox2.Parent = pcksagogr;

            panelCerceve.Padding = new Padding(10);   // ← Border kalınlığı gibi davranır
            panelCerceve.BackColor = Color.FromArgb(160, 58, 19); ;    // Çerçeve rengi

            rchsoru.Parent = panelCerceve;
            rchsoru.BorderStyle = BorderStyle.None;
            rchsoru.Dock = DockStyle.Fill;

            // ==================  DOĞRU KATMAN KURGUSU (TEMİZ ÇÖZÜM) ==================

            

            // RichTextBox panelin arka planı gibi çalışacak
            rchsoru.Parent = panelCerceve;
            rchsoru.BorderStyle = BorderStyle.None;
            rchsoru.Dock = DockStyle.Fill;
            rchsoru.BringToFront();   // EN ALTTA AMA GÖRÜNÜR


           




        }



    }
}
