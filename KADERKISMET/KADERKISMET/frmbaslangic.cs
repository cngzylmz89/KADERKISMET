using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;

namespace KADERKISMET
{
    public partial class frmbaslangic : Form
    {

        [DllImport("user32.dll")]
        private static extern bool ReleaseCapture();

        [DllImport("user32.dll")]
        private static extern IntPtr SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

        private const int WM_NCLBUTTONDOWN = 0xA1;
        private const int HTCAPTION = 0x2;
        public frmbaslangic()
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

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        private void button4_Click(object sender, EventArgs e)
        {
            this.WindowState= FormWindowState.Minimized;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (button2.Visible == true)
            {
                this.WindowState = FormWindowState.Maximized;
                button2.Visible = false;
                button5.Visible = true;
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (button5.Visible == true)
            {
                this.WindowState = FormWindowState.Normal;
                button2.Visible = true;
                button5.Visible = false;
            }
        }

        //VERİTABANINA CHECKBOX ı kaydetme
        //using (SqlConnection con = new SqlConnection(baglanti))
//{
//    con.Open();

//    foreach (DataGridViewRow row in dataGridView1.Rows)
//    {
//        if (row.IsNewRow) continue;

//        bool evet = Convert.ToBoolean(row.Cells["colEvet"].Value);
//        bool hayir = Convert.ToBoolean(row.Cells["colHayir"].Value);

//        int durum = evet ? 1 : 0;   // EVET=1, HAYIR=0

//        SqlCommand cmd = new SqlCommand(
//            "INSERT INTO TABLO_ADI (Durum) VALUES (@durum)", con);

//        cmd.Parameters.AddWithValue("@durum", durum);
//        cmd.ExecuteNonQuery();
//    }
//}

        void sinifflistele()
        {
            OleDbConnection conn = new OleDbConnection(con.baglan);
            conn.Open();
            OleDbDataAdapter da2 = new OleDbDataAdapter("select SINIFID, SINIFAD FROM TBLSINIF", conn);
            DataTable dt2 = new DataTable();
            da2.Fill(dt2);
            cmbsinif.DisplayMember = "SINIFAD";
            cmbsinif.ValueMember = "SINIFID";
            cmbsinif.DataSource = dt2;

            conn.Close();
        }

        void ogrencilistele()
        {
            dataGridView1.Columns.Clear();
            dataGridView1.DataSource = null;
            OleDbConnection conn = new OleDbConnection(con.baglan);
            dataGridView1.EditMode = DataGridViewEditMode.EditProgrammatically;
            DataGridViewCheckBoxColumn colEvet = new DataGridViewCheckBoxColumn();
            colEvet.Name = "colEvet";
            colEvet.HeaderText = "BİRİNCİ  GRUP";
            dataGridView1.Columns.Add(colEvet);

            DataGridViewCheckBoxColumn colHayir = new DataGridViewCheckBoxColumn();
            colHayir.Name = "colHayir";
            colHayir.HeaderText = "İKİNCİ GRUP";
            dataGridView1.Columns.Add(colHayir);

            DataGridViewCheckBoxColumn colUcuncu = new DataGridViewCheckBoxColumn();
            colUcuncu.Name = "colUcuncu";
            colUcuncu.HeaderText = "SINIFTA MI?";
            dataGridView1.Columns.Add(colUcuncu);


            //DATAGRİDDE FOTOYOL COLUMN OLUŞTUR
            DataGridViewImageColumn imgCol = new DataGridViewImageColumn();
            imgCol.Name = "FOTOYOL";
            imgCol.HeaderText = "FOTOĞRAF";
            imgCol.DataPropertyName = "OGRFOTOYOL";
            imgCol.ImageLayout = DataGridViewImageCellLayout.Zoom;
            dataGridView1.Columns.Add(imgCol);

            dataGridView1.RowTemplate.Height = 150;
            dataGridView1.Columns["FOTOYOL"].Width = 300;

            conn.Open();
            OleDbDataAdapter da1 = new OleDbDataAdapter("select ID AS 'SIRA NO', OGRADSOYAD AS 'ADI SOYADI', OGRNUMARA AS 'NUMARASI',OGRSINIF, SINIFAD  AS 'SINIFI', OGRGRUP, OGRFOTOYOL, OGRYOKLAMA FROM TBLOGRENCILER INNER JOIN TBLSINIF ON TBLSINIF.SINIFID=TBLOGRENCILER.OGRSINIF WHERE OGRSINIF LIKE'"+cmbsinif.SelectedValue+"%'", conn);
            DataTable dt1 = new DataTable();
            da1.Fill(dt1);
            //colunnları sırasını belli eder
            dataGridView1.DataSource = dt1;
            dataGridView1.Columns["colEvet"].DisplayIndex = dataGridView1.Columns.Count - 2;
            dataGridView1.Columns["colHayir"].DisplayIndex = dataGridView1.Columns.Count - 1;
            dataGridView1.Columns["colUcuncu"].DisplayIndex = dataGridView1.Columns.Count - 1;
            //columnların visible özelliğini belli eder
            dataGridView1.Columns["OGRGRUP"].Visible = false;
            dataGridView1.Columns["OGRYOKLAMA"].Visible = false;
            dataGridView1.Columns["OGRSINIF"].Visible = false;
            //dataGridView1.Columns["OGRFOTOYOL"].Visible=false;  

            // DataSource bağlıyken SATIR EKLEME YOK

            conn.Close();



            
            for (int i = 0; i < dt1.Rows.Count; i++)
            {
                bool durum = dt1.Rows[i]["OGRGRUP"] != DBNull.Value &&
                             Convert.ToBoolean(dt1.Rows[i]["OGRGRUP"]);

                dataGridView1.Rows[i].Cells["colEvet"].Value = durum;
                dataGridView1.Rows[i].Cells["colHayir"].Value = !durum;

                // ✅ ÜÇÜNCÜ CHECKBOX (OGRYOKLAMA)
                bool yoklamaDurum = dt1.Rows[i]["OGRYOKLAMA"] != DBNull.Value &&
                                    Convert.ToBoolean(dt1.Rows[i]["OGRYOKLAMA"]);

                dataGridView1.Rows[i].Cells["colUcuncu"].Value = yoklamaDurum;

            }

            








        }
        private void frmbaslangic_Load(object sender, EventArgs e)
        {
            sinifflistele();
           

        }
        baglantisinif con= new baglantisinif();
        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            // Checkbox değilse çık
            if (!(dataGridView1.Columns[e.ColumnIndex] is DataGridViewCheckBoxColumn))
                return;

            // Anında işaretlensin
            dataGridView1.CommitEdit(DataGridViewDataErrorContexts.Commit);

            // Aynı satırda diğer checkboxları kapat
            foreach (DataGridViewColumn col in dataGridView1.Columns)
            {
                if (col is DataGridViewCheckBoxColumn && col.Index != e.ColumnIndex)
                {
                    dataGridView1.Rows[e.RowIndex].Cells[col.Index].Value = false;
                }
            }

            // Checkbox sütunu değilse çık
            if (!(dataGridView1.Columns[e.ColumnIndex] is DataGridViewCheckBoxColumn))
                return;

            // Edit modunu hemen uygula
            dataGridView1.CommitEdit(DataGridViewDataErrorContexts.Commit);

            // Aynı satırdaki diğer checkboxları kapat
            foreach (DataGridViewColumn col in dataGridView1.Columns)
            {
                if (col is DataGridViewCheckBoxColumn && col.Index != e.ColumnIndex)
                {
                    dataGridView1.Rows[e.RowIndex].Cells[col.Index].Value = false;
                }
            }

            if (dataGridView1.Columns[e.ColumnIndex].Name == "colEvet")
            {
                dataGridView1.Rows[e.RowIndex].Cells["colHayir"].Value = false;
            }
            else if (dataGridView1.Columns[e.ColumnIndex].Name == "colHayir")
            {
                dataGridView1.Rows[e.RowIndex].Cells["colEvet"].Value = false;
            }

        }

        private void dataGridView1_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (dataGridView1.Columns[e.ColumnIndex].Name == "FOTOYOL" && e.Value != null)
            {
                string yol = e.Value.ToString();

                if (File.Exists(yol))
                {
                    using (FileStream fs = new FileStream(yol, FileMode.Open, FileAccess.Read))
                    {
                        e.Value = Image.FromStream(fs);
                    }
                }
                else
                {
                    e.Value = null;
                }
            }

            if (e.RowIndex < 0) return;

            if (dataGridView1.Columns[e.ColumnIndex] is DataGridViewCheckBoxColumn)
            {
                bool secili = e.Value != null && (bool)e.Value;

                if (secili)
                {
                    e.CellStyle.BackColor = Color.LightSkyBlue;
                    e.CellStyle.SelectionBackColor = Color.DodgerBlue;
                }
                else
                {
                    e.CellStyle.BackColor = Color.White;
                    e.CellStyle.SelectionBackColor = dataGridView1.DefaultCellStyle.SelectionBackColor;
                }
            }

            if (dataGridView1.Columns[e.ColumnIndex].Name == "colUcuncu")
            {
                bool secili = e.Value != null && (bool)e.Value;

                if (secili)
                {
                    e.CellStyle.BackColor = Color.LightGreen;
                    e.CellStyle.SelectionBackColor = Color.SeaGreen;
                }
                else
                {
                    e.CellStyle.BackColor = Color.LightGoldenrodYellow;
                    e.CellStyle.SelectionBackColor = Color.Goldenrod;
                }
            }

          
            if (e.RowIndex < 0) return;

            DataGridViewRow row = dataGridView1.Rows[e.RowIndex];
            bool yoklamaVar = Convert.ToBoolean(row.Cells["colUcuncu"].Value ?? false);

            if (!yoklamaVar)
            {
                // 🔕 Satır pasif
                row.DefaultCellStyle.BackColor = Color.Gainsboro;
                row.DefaultCellStyle.ForeColor = Color.Gray;
                row.DefaultCellStyle.SelectionBackColor = Color.Silver;
                row.DefaultCellStyle.SelectionForeColor = Color.Gray;

                // colUcuncu biraz daha belirgin kalsın
                row.Cells["colUcuncu"].Style.BackColor = Color.LightGoldenrodYellow;
            }
            else
            {
                // 🔔 Aktif
                row.DefaultCellStyle.BackColor = Color.White;
                row.DefaultCellStyle.ForeColor = Color.Black;
                row.DefaultCellStyle.SelectionBackColor = dataGridView1.DefaultCellStyle.SelectionBackColor;
                row.DefaultCellStyle.SelectionForeColor = dataGridView1.DefaultCellStyle.SelectionForeColor;
            }

        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0) return;

            if (!(dataGridView1.Columns[e.ColumnIndex] is DataGridViewCheckBoxColumn))
                return;

            DataGridViewRow row = dataGridView1.Rows[e.RowIndex];
            string kolon = dataGridView1.Columns[e.ColumnIndex].Name;

            bool yoklamaVar = Convert.ToBoolean(row.Cells["colUcuncu"].Value ?? false);

            // 🔓 1️⃣ colUcuncu TAMAMEN BAĞIMSIZ
            if (kolon == "colUcuncu")
            {
                bool eski = Convert.ToBoolean(row.Cells["colUcuncu"].Value ?? false);
                row.Cells["colUcuncu"].Value = !eski;

                if (!eski)
                    AktiflestirmeAnimasyonu(e.RowIndex);

                dataGridView1.Invalidate();
                return;
            }

            // 🔒 2️⃣ Yoklama yoksa EVET / HAYIR → UYARI
            if (!yoklamaVar && (kolon == "colEvet" || kolon == "colHayir"))
            {
                MessageBox.Show(
                    "Öğrenci sınıfta değil!",
                    "Uyarı",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
                return;
            }

            // 🔁 3️⃣ Evet – Hayır kuralları
            bool evet = Convert.ToBoolean(row.Cells["colEvet"].Value ?? false);
            bool hayir = Convert.ToBoolean(row.Cells["colHayir"].Value ?? false);

            // İkisi birden boş kalamaz
            if (kolon == "colEvet" && evet && !hayir) return;
            if (kolon == "colHayir" && hayir && !evet) return;

            // Toggle
            bool yeniDeger = Convert.ToBoolean(row.Cells[kolon].Value ?? false);
            row.Cells[kolon].Value = !yeniDeger;

            // Diğerini kapat
            if (kolon == "colEvet")
                row.Cells["colHayir"].Value = false;
            else if (kolon == "colHayir")
                row.Cells["colEvet"].Value = false;

            dataGridView1.Invalidate();
        }
        private void AktiflestirmeAnimasyonu(int rowIndex)
        {
            DataGridViewRow row = dataGridView1.Rows[rowIndex];
            Color eskiRenk = row.DefaultCellStyle.BackColor;

            row.DefaultCellStyle.BackColor = Color.LightGreen;

            Timer t = new Timer();
            t.Interval = 300;
            t.Tick += (s, e) =>
            {
                row.DefaultCellStyle.BackColor = Color.White;
                t.Stop();
                t.Dispose();
            };
            t.Start();
        }
        private void dataGridView1_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0) return;

            if (!(dataGridView1.Columns[e.ColumnIndex] is DataGridViewCheckBoxColumn))
                return;

            bool secili = Convert.ToBoolean(e.Value ?? false);
            if (!secili) return;

            string kolon = dataGridView1.Columns[e.ColumnIndex].Name;
            DataGridViewRow row = dataGridView1.Rows[e.RowIndex];

            e.Handled = true;

            // Arka planı temizle
            e.PaintBackground(e.CellBounds, true);

            Color ustRenk;
            Color altRenk;

            // 🎨 colUcuncu TRUE → FARKLI GRADIENT
            if (kolon == "colUcuncu")
            {
                ustRenk = Color.LightGreen;
                altRenk = Color.SeaGreen;
            }
            else
            {
                // colEvet / colHayir
                ustRenk = Color.LightSkyBlue;
                altRenk = Color.DodgerBlue;
            }

            using (var brush = new System.Drawing.Drawing2D.LinearGradientBrush(
                e.CellBounds,
                ustRenk,
                altRenk,
                System.Drawing.Drawing2D.LinearGradientMode.Vertical))
            {
                e.Graphics.FillRectangle(brush, e.CellBounds);
            }

            // Checkbox + border
            e.PaintContent(e.CellBounds);
            //if (e.RowIndex < 0) return;

            //// Sadece CheckBox sütunları
            //if (!(dataGridView1.Columns[e.ColumnIndex] is DataGridViewCheckBoxColumn))
            //    return;

            //bool secili = e.Value != null && (bool)e.Value;
            //if (!secili) return;

            //e.Handled = true;

            //// Arka planı temizle
            //e.PaintBackground(e.CellBounds, true);

            //// Gradient çiz
            //using (var brush = new System.Drawing.Drawing2D.LinearGradientBrush(
            //    e.CellBounds,
            //    Color.LightSkyBlue,
            //    Color.DodgerBlue,
            //    System.Drawing.Drawing2D.LinearGradientMode.Vertical))
            //{
            //    e.Graphics.FillRectangle(brush, e.CellBounds);
            //}

            //// Border + checkbox çizimi
            //e.PaintContent(e.CellBounds);
        }
        public string connStr;
        private void button3_Click(object sender, EventArgs e)
        {

            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Excel Dosyaları (*.xls;*.xlsx)|*.xls;*.xlsx";
            ofd.Title = "Excel Dosyası Seç";

            if (ofd.ShowDialog() != DialogResult.OK)
                return;

            string excelYol = ofd.FileName;

            //dosya uzantısına göre connection string

            string uzanti = Path.GetExtension(excelYol);
            
            //2003 xls ise
            if (uzanti == ".xls")
            {
                connStr = @"Provider=Microsoft.Jet.OLEDB.4.0;" +
                          "Data Source=" + excelYol + ";" +
                          "Extended Properties='Excel 8.0;HDR=YES;'";
                if (connStr != "")
                {
                    try
                    {
                        OleDbConnection sorubaglanti = new OleDbConnection(connStr);
                        sorubaglanti.Open();
                        OleDbCommand dersgetir = new OleDbCommand("select DISTINCT(DERS) FROM  [TBLSORULAR$]", sorubaglanti);
                        OleDbDataAdapter dersgetircmb = new OleDbDataAdapter(dersgetir);
                        DataTable dtdersgetirexcel = new DataTable();
                        dersgetircmb.Fill(dtdersgetirexcel);

                        cmbders.DataSource = dtdersgetirexcel;
                        cmbders.DisplayMember = "DERS";
                        cmbders.ValueMember = "DERS";
                        sorubaglanti.Close();
                        cmbders.Enabled = true;
                    }
                    catch (Exception hata)
                    {

                        DialogResult result = MessageBox.Show("Seçmiş olduğunuz excel dosyası programın formatına uygun değildir.Formata uygun excel dosyasının ismi KADERKISMETSORULAR adındadır. Evete tıklayarak uygun excel dosyasını indirebilirsiniz.", "Bilgi", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (result == DialogResult.Yes)
                        {
                            try
                            {
                                string url = "https://docs.google.com/spreadsheets/d/1R_4g3TYMkqcQ8cfpQyKN9JJ_OuF7RtwJ/export?format=xlsx";

                                string kayitYolu = Path.Combine(
                                    Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                                    "KADERKISMETSORULAR.xlsx");

                                using (WebClient wc = new WebClient())
                                {
                                    wc.Headers.Add("User-Agent", "Mozilla/5.0");
                                    wc.DownloadFile(url, kayitYolu);
                                }

                                MessageBox.Show("Excel dosyası sorunsuz indirildi.");
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show("İndirme hatası: " + ex.Message);
                            }
                        }
                    }

                }

            }

            //2007 .xlsx ise
            else if (uzanti == ".xlsx")
            {
                connStr = @"Provider=Microsoft.ACE.OLEDB.12.0;" +
                          "Data Source=" + excelYol + ";" +
                          "Extended Properties='Excel 12.0 Xml;HDR=YES;'";
                if (connStr != "")
                {
                    try
                    {
                        OleDbConnection sorubaglanti = new OleDbConnection(connStr);
                        sorubaglanti.Open();
                        OleDbCommand dersgetir = new OleDbCommand("select DISTINCT(DERS) FROM  [TBLSORULAR$]", sorubaglanti);
                        OleDbDataAdapter dersgetircmb = new OleDbDataAdapter(dersgetir);
                        DataTable dtdersgetirexcel = new DataTable();
                        dersgetircmb.Fill(dtdersgetirexcel);

                        cmbders.DataSource = dtdersgetirexcel;
                        cmbders.DisplayMember = "DERS";
                        cmbders.ValueMember = "DERS";
                        sorubaglanti.Close();
                        cmbders.Enabled = true;
                    }
                    catch (Exception hata)
                    {

                        DialogResult result=MessageBox.Show("Seçmiş olduğunuz excel dosyası programın formatına uygun değildir.Formata uygun excel dosyasının ismi KADERKISMETSORULAR adındadır. Evete tıklayarak uygun excel dosyasını indirebilirsiniz.", "Bilgi", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (result == DialogResult.Yes)
                        {
                            try
                            {
                            
                                string url = "https://docs.google.com/spreadsheets/d/1PRY4vbObN8ZKMRLo0STzmSDpkye8Q5sE/export?format=xlsx";

                                string kayitYolu = Path.Combine(
                                    Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                                    "KADERKISMETSORULAR.xlsx");

                                using (WebClient wc = new WebClient())
                                {
                                    wc.Headers.Add("User-Agent", "Mozilla/5.0");
                                    wc.DownloadFile(url, kayitYolu);
                                }

                                MessageBox.Show("Excel dosyası sorunsuz indirildi.");
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show("İndirme hatası: " + ex.Message);
                            }
                        }
                    }
                    
                }
            }
        }

        private void cmbsinif_SelectedValueChanged(object sender, EventArgs e)
        {
            
            ogrencilistele();
            btnkaynaksec.Enabled = true;
        }

        private void dataGridView1_CellToolTipTextNeeded(object sender, DataGridViewCellToolTipTextNeededEventArgs e)
        {
            if (e.RowIndex < 0) return;

            DataGridViewRow row = dataGridView1.Rows[e.RowIndex];
            bool yoklamaVar = Convert.ToBoolean(row.Cells["colUcuncu"].Value ?? false);

            if (!yoklamaVar &&
                (dataGridView1.Columns[e.ColumnIndex].Name == "colEvet" ||
                 dataGridView1.Columns[e.ColumnIndex].Name == "colHayir"))
            {
                e.ToolTipText = "Öğrenci sınıfta değil";

            }

            if (e.RowIndex < 0 || e.ColumnIndex < 0) return;

            string kolon = dataGridView1.Columns[e.ColumnIndex].Name;

            if (kolon != "colEvet" && kolon != "colHayir")
                return;

            

            if (!yoklamaVar)
            {
                e.ToolTipText = "Öğrenci sınıfta değil";
            }
        }

        private void btnasagi_Click(object sender, EventArgs e)
        {
            // Hiç satır yoksa çık
            if (dataGridView1.RowCount == 0) return;

            int firstVisibleRow = dataGridView1.FirstDisplayedScrollingRowIndex;

            // Grid sonuna gelmediysek 1 satır aşağı kaydır
            if (firstVisibleRow < dataGridView1.RowCount - 1)
            {
                dataGridView1.FirstDisplayedScrollingRowIndex =
                    firstVisibleRow + 1;
            }
        }

        private void btnyukari_Click(object sender, EventArgs e)
        {
            if (dataGridView1.RowCount == 0) return;

            int firstVisibleRow = dataGridView1.FirstDisplayedScrollingRowIndex;

            // En üstte değilsek yukarı kay
            if (firstVisibleRow > 0)
            {
                dataGridView1.FirstDisplayedScrollingRowIndex =
                    firstVisibleRow - 1;
            }
        }

        private void cmbders_SelectedValueChanged(object sender, EventArgs e)
        {
            if (cmbders.SelectedValue == null)
                return;

            using (OleDbConnection sorubaglanti = new OleDbConnection(connStr))
            {
                sorubaglanti.Open();

                string sql = "SELECT DISTINCT(KONU) FROM [TBLSORULAR$] WHERE DERS = ?";

                OleDbCommand cmd = new OleDbCommand(sql, sorubaglanti);
                cmd.Parameters.AddWithValue("?", cmbders.SelectedValue.ToString());

                OleDbDataAdapter da = new OleDbDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                cmbkonu.DataSource = dt;
                cmbkonu.DisplayMember = "KONU";
                cmbkonu.ValueMember = "KONU";
                cmbkonu.Enabled = true;
            }
        }

        private void cmbkonu_SelectedValueChanged(object sender, EventArgs e)
        {
            if (cmbkonu.SelectedValue == null)
                return;

            using (OleDbConnection sorubaglanti = new OleDbConnection(connStr))
            {
                sorubaglanti.Open();

                string sql = "SELECT * FROM [TBLSORULAR$] WHERE KONU = ?";

                OleDbCommand cmd = new OleDbCommand(sql, sorubaglanti);
                cmd.Parameters.AddWithValue("@p1", cmbkonu.SelectedValue.ToString());

                OleDbDataAdapter da = new OleDbDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                cmbtest.DataSource = dt;
                cmbtest.DisplayMember = "TESTADI"; // örnek kolon
                cmbtest.ValueMember = "TESTADI";
                cmbtest.Enabled=true;
            }
        }
    }
}
