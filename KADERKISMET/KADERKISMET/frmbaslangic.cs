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
private void frmbaslangic_Load(object sender, EventArgs e)
        {
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

            //DATAGRİDDE FOTOYOL COLUMN OLUŞTUR
            DataGridViewImageColumn imgCol = new DataGridViewImageColumn();
            imgCol.Name = "FOTOYOL";
            imgCol.HeaderText = "FOTOĞRAF";
            imgCol.DataPropertyName = "OGRFOTOYOL";
            imgCol.ImageLayout = DataGridViewImageCellLayout.Zoom;
            dataGridView1.Columns.Add(imgCol);

            dataGridView1.RowTemplate.Height =150;
            dataGridView1.Columns["FOTOYOL"].Width = 300;

            conn.Open();
            OleDbDataAdapter da1 = new OleDbDataAdapter("select ID AS 'SIRA NO', OGRADSOYAD AS 'ADI SOYADI', OGRNUMARA AS 'NUMARASI',OGRSINIF AS 'SINIFI', OGRGRUP, OGRFOTOYOL FROM TBLOGRENCILER", conn);
            DataTable dt1 = new DataTable();
            da1.Fill(dt1);

            dataGridView1.DataSource = dt1;
            dataGridView1.Columns["OGRGRUP"].Visible = false;
            //dataGridView1.Columns["OGRFOTOYOL"].Visible=false;  

            // DataSource bağlıyken SATIR EKLEME YOK
           
            conn.Close();
            for (int i = 0; i < dt1.Rows.Count; i++)
            {
                bool durum = dt1.Rows[i]["OGRGRUP"] != DBNull.Value &&
                             Convert.ToBoolean(dt1.Rows[i]["OGRGRUP"]);

                dataGridView1.Rows[i].Cells["colEvet"].Value = durum;
                dataGridView1.Rows[i].Cells["colHayir"].Value = !durum;
            }



            



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
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            // Checkbox sütunu değilse çık
            if (!(dataGridView1.Columns[e.ColumnIndex] is DataGridViewCheckBoxColumn))
                return;

            var cell = (DataGridViewCheckBoxCell)dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex];

            bool yeniDeger = !(cell.Value is bool b && b);
            cell.Value = yeniDeger;

            // Aynı satırdaki diğer checkboxları kapat
            foreach (DataGridViewColumn col in dataGridView1.Columns)
            {
                if (col is DataGridViewCheckBoxColumn && col.Index != e.ColumnIndex)
                {
                    dataGridView1.Rows[e.RowIndex].Cells[col.Index].Value = false;
                }
            }

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
    }
}
