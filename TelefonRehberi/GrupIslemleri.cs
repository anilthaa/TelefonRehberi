using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics.Eventing.Reader;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TelefonRehberi
{
    public partial class GrupIslemleri : Form
    {
        public GrupIslemleri()
        {
            InitializeComponent();
        }
        private int seciliGrupId = -1;
        private void GruplarYukle()
        {
            using (var db=new TelefonRehberiDBEntities1())
            {
                var gruplar=db.Gruplar.AsEnumerable().Select((g, index)=>new
                {
                    Sira=index+1,
                    g.Grup_Id,
                    g.Grup_Adi,
                    g.Aciklama,
                }).ToList();

                grdGruplar.AutoGenerateColumns = false;
                grdGruplar.DataSource = gruplar;

                if (grdGruplar.Columns.Count==0)
                {
                    grdGruplar.Columns.Add(new DataGridViewTextBoxColumn()
                    {
                        DataPropertyName = "Sira",
                        HeaderText = "Sıra",
                        Name = "Sira",
                        Width = 50,
                    });
                    grdGruplar.Columns.Add(new DataGridViewTextBoxColumn()
                    {
                        DataPropertyName = "Grup_Id",
                        HeaderText = "ID",
                        Name = "Grup_Id",
                        Visible = false
                    });
                    grdGruplar.Columns.Add(new DataGridViewTextBoxColumn()
                    {
                        DataPropertyName = "Grup_Adi",
                        HeaderText = "Grup Adı",
                        Name = "Grup_Adi"
                        
                    });
                    grdGruplar.Columns.Add(new DataGridViewTextBoxColumn()
                    {
                        DataPropertyName = "Aciklama",
                        HeaderText = "Açıklama",
                        Name = "Aciklama"
                        
                    });
                }
            }
            grdGruplar.ClearSelection();

            grdGruplar.AutoSizeColumnsMode=DataGridViewAutoSizeColumnsMode.Fill;
            grdGruplar.AutoSizeRowsMode=DataGridViewAutoSizeRowsMode.AllCells;
            grdGruplar.SelectionMode=DataGridViewSelectionMode.FullRowSelect;
            grdGruplar.MultiSelect=false;
        }

        private void GrupIslemleri_Load(object sender, EventArgs e)
        {
            GruplarYukle();
        }

        private void btnEkle_Click(object sender, EventArgs e)
        {
            using (var context = new TelefonRehberiDBEntities1())
            {
                var yeniGrup = new Gruplar
                {
                    Grup_Adi = txtGrupAdi.Text,
                    Aciklama = txtGrupAciklama.Text
                };

                context.Gruplar.Add(yeniGrup);
                context.SaveChanges();

                MessageBox.Show("Yeni grup eklendi.");
                GruplarYukle();
                Temizle();
            }
        }

        private void btnSil_Click(object sender, EventArgs e)
        {
            if (grdGruplar.CurrentRow!=null)
            {
                int grupId = Convert.ToInt32(grdGruplar.CurrentRow.Cells["Grup_Id"].Value);

                using (var db=new TelefonRehberiDBEntities1())
                {
                    var grup=db.Gruplar.FirstOrDefault(g=>g.Grup_Id==grupId);
                    if (grup!=null)
                    {
                        var kisiler=db.Kisiler.Where(k=>k.Grup_ID==grupId).ToList();

                        foreach (var kisi in kisiler)
                        {
                            kisi.Grup_ID = null;
                        }
                        db.Gruplar.Remove(grup);
                        db.SaveChanges() ;

                        MessageBox.Show("Grup silindi.");
                        GruplarYukle() ;
                        Temizle() ;
                    }
                }
            }
            else
            {
                MessageBox.Show("Silmek için bir grup seçiniz.");
            }
        }

        private void btnGuncelle_Click(object sender, EventArgs e)
        {
            if (seciliGrupId==-1)
            {
                MessageBox.Show("Lütfen güncellenecek bir grup seçiniz.");
                return;
            }

            using (var db = new TelefonRehberiDBEntities1())
            {
                var grup = db.Gruplar.FirstOrDefault(g => g.Grup_Id == seciliGrupId);
                if (grup != null)
                {
                    grup.Grup_Adi=txtGrupAdi.Text;
                    grup.Aciklama=txtGrupAciklama.Text;

                    db.SaveChanges();

                    MessageBox.Show("Grup Güncellendi");
                    GruplarYukle();

                    Temizle();
                    seciliGrupId = -1;
                    grdGruplar.ClearSelection();
                }
            }
        }

        private void grdGruplar_SelectionChanged(object sender, EventArgs e)
        {

        }

        private void grdGruplar_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex>=0)
            {
                var row = grdGruplar.Rows[e.RowIndex];
                seciliGrupId = Convert.ToInt32(row.Cells["Grup_Id"].Value);

                txtGrupAdi.Text = row.Cells["Grup_Adi"].Value.ToString();
                txtGrupAciklama.Text = row.Cells["Aciklama"].Value.ToString();
            }
        }

        private void Temizle()
        {
            txtGrupAdi.Clear();
            txtGrupAciklama.Clear();
        }
    }
}
