using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KADERKISMET
{
    public partial class frmbaslangic : Form
    {
        public frmbaslangic()
        {
            InitializeComponent();
        }

        private void button4_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {

        }

        private void button5_Click(object sender, EventArgs e)
        {

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
            colUcuncu.HeaderText = "SINIFTA MI";
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

            dataGridView1.DataSource = dt1;
            dataGridView1.Columns["colEvet"].DisplayIndex = dataGridView1.Columns.Count - 2;
            dataGridView1.Columns["colHayir"].DisplayIndex = dataGridView1.Columns.Count - 1;
            dataGridView1.Columns["colUcuncu"].DisplayIndex = dataGridView1.Columns.Count - 1;
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
                // 🔕 PASİF GÖRÜNÜM
                row.DefaultCellStyle.BackColor = Color.Gainsboro;
                row.DefaultCellStyle.ForeColor = Color.Gray;
                row.DefaultCellStyle.SelectionBackColor = Color.Silver;
                row.DefaultCellStyle.SelectionForeColor = Color.Gray;
            }
            else
            {
                // 🔔 AKTİF GÖRÜNÜM
                row.DefaultCellStyle.BackColor = Color.White;
                row.DefaultCellStyle.ForeColor = Color.Black;
                row.DefaultCellStyle.SelectionBackColor = dataGridView1.DefaultCellStyle.SelectionBackColor;
                row.DefaultCellStyle.SelectionForeColor = dataGridView1.DefaultCellStyle.SelectionForeColor;
            }
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            //if (e.RowIndex < 0) return;

            //// Checkbox sütunu değilse çık
            //if (!(dataGridView1.Columns[e.ColumnIndex] is DataGridViewCheckBoxColumn))
            //    return;

            //var cell = (DataGridViewCheckBoxCell)dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex];

            //bool yeniDeger = !(cell.Value is bool b && b);
            //cell.Value = yeniDeger;

            //// Aynı satırdaki diğer checkboxları kapat
            //foreach (DataGridViewColumn col in dataGridView1.Columns)
            //{
            //    if (col is DataGridViewCheckBoxColumn && col.Index != e.ColumnIndex)
            //    {
            //        dataGridView1.Rows[e.RowIndex].Cells[col.Index].Value = false;
            //    }
            //}
            //if (e.RowIndex < 0) return;

            //if (!(dataGridView1.Columns[e.ColumnIndex] is DataGridViewCheckBoxColumn))
            //    return;

            //DataGridViewRow row = dataGridView1.Rows[e.RowIndex];

            //bool evet = Convert.ToBoolean(row.Cells["colEvet"].Value ?? false);
            //bool hayir = Convert.ToBoolean(row.Cells["colHayir"].Value ?? false);

            //string tiklanan = dataGridView1.Columns[e.ColumnIndex].Name;

            //// 🔒 İKİSİ BİRDEN BOŞ KALAMAZ
            //if (tiklanan == "colEvet" && evet && !hayir)
            //    return;

            //if (tiklanan == "colHayir" && hayir && !evet)
            //    return;

            //// Değeri değiştir
            // yeniDeger = !(bool)(row.Cells[e.ColumnIndex].Value ?? false);
            //row.Cells[e.ColumnIndex].Value = yeniDeger;

            //// Diğer checkbox kapansın (radio mantığı)
            //if (tiklanan == "colEvet")
            //    row.Cells["colHayir"].Value = false;
            //else
            //    row.Cells["colEvet"].Value = false;
            if (e.RowIndex < 0) return;

            if (!(dataGridView1.Columns[e.ColumnIndex] is DataGridViewCheckBoxColumn))
                return;

            string kolon = dataGridView1.Columns[e.ColumnIndex].Name;
            DataGridViewRow row = dataGridView1.Rows[e.RowIndex];

            // 🔓 BAĞIMSIZ CHECKBOX
            if (kolon == "colUcuncu")
            {
                bool deger = Convert.ToBoolean(row.Cells[kolon].Value ?? false);
                row.Cells[kolon].Value = !deger;
                return;
            }

            // 🔒 SADECE colEvet - colHayir ARASI KURAL
            bool evet = Convert.ToBoolean(row.Cells["colEvet"].Value ?? false);
            bool hayir = Convert.ToBoolean(row.Cells["colHayir"].Value ?? false);

            // İkisi birden boş kalamaz
            if (kolon == "colEvet" && evet && !hayir) return;
            if (kolon == "colHayir" && hayir && !evet) return;

            // Değeri değiştir
            bool yeniDeger = !(bool)(row.Cells[kolon].Value ?? false);
            row.Cells[kolon].Value = yeniDeger;

            // Diğerini kapat
            if (kolon == "colEvet")
                row.Cells["colHayir"].Value = false;
            else
                row.Cells["colEvet"].Value = false;

            //// ⬇️ OTOMATİK SCROLL
            //if (e.RowIndex < dataGridView1.RowCount - 1)
            //{
            //    dataGridView1.FirstDisplayedScrollingRowIndex = e.RowIndex + 1;
            //    dataGridView1.CurrentCell = dataGridView1.Rows[e.RowIndex + 1].Cells[0];
            //}
            //else
            //{
            //    // en sondaysa başa dön
            //    dataGridView1.FirstDisplayedScrollingRowIndex = 0;
            //    dataGridView1.CurrentCell = dataGridView1.Rows[0].Cells[0];
            //}

            dataGridView1.Invalidate();
        }

        private void dataGridView1_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.RowIndex < 0) return;

            // Sadece CheckBox sütunları
            if (!(dataGridView1.Columns[e.ColumnIndex] is DataGridViewCheckBoxColumn))
                return;

            bool secili = e.Value != null && (bool)e.Value;
            if (!secili) return;

            e.Handled = true;

            // Arka planı temizle
            e.PaintBackground(e.CellBounds, true);

            // Gradient çiz
            using (var brush = new System.Drawing.Drawing2D.LinearGradientBrush(
                e.CellBounds,
                Color.LightSkyBlue,
                Color.DodgerBlue,
                System.Drawing.Drawing2D.LinearGradientMode.Vertical))
            {
                e.Graphics.FillRectangle(brush, e.CellBounds);
            }

            // Border + checkbox çizimi
            e.PaintContent(e.CellBounds);
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
            }

            //2007 .xlsx ise
            else if (uzanti == ".xlsx")
            {
                connStr = @"Provider=Microsoft.ACE.OLEDB.12.0;" +
                          "Data Source=" + excelYol + ";" +
                          "Extended Properties='Excel 12.0 Xml;HDR=YES;'";
            }
        }

        private void cmbsinif_SelectedValueChanged(object sender, EventArgs e)
        {
            
            ogrencilistele();
        }
    }
}
