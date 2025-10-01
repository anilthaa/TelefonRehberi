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

        private void GruplarYukle()
        {
            using (var db=new TelefonRehberiDBEntities1())
            {
                var gruplar=db.Gruplar.ToList();
                grdGruplar.DataSource= gruplar;
            }
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
            }
        }

        private void btnSil_Click(object sender, EventArgs e)
        {
            if (grdGruplar.SelectedRows.Count>0)
            {
                int grupId = Convert.ToInt32(grdGruplar.SelectedRows[0].Cells["Grup_Id"].Value);

                using (var db=new TelefonRehberiDBEntities1())
                {
                    var grup=db.Gruplar.FirstOrDefault(g=>g.Grup_Id==grupId);
                    if (grup!=null)
                    {
                        db.Gruplar.Remove(grup);
                        db.SaveChanges() ;

                        MessageBox.Show("Grup silindi.");
                        GruplarYukle() ;
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
            if (grdGruplar.SelectedRows.Count > 0)
            {
                int grupId = Convert.ToInt32(grdGruplar.SelectedRows[0].Cells["Grup_Id"].Value);

                using (var context = new TelefonRehberiDBEntities1())
                {
                    var grup = context.Gruplar.FirstOrDefault(g => g.Grup_Id == grupId);
                    if (grup != null)
                    {
                        grup.Grup_Adi=txtGrupAdi.Text;
                        grup.Aciklama=txtGrupAciklama.Text;

                        context.SaveChanges();

                        MessageBox.Show("Grup güncellendi.");
                        GruplarYukle();
                    }
                }
            }
        }

        private void grdGruplar_SelectionChanged(object sender, EventArgs e)
        {
            if (grdGruplar.SelectedRows.Count>0)
            {
                var selectedRow = grdGruplar.SelectedRows[0];
                txtGrupAdi.Text = selectedRow.Cells["Grup_Adi"].Value.ToString();
                txtGrupAciklama.Text = selectedRow.Cells["Aciklama"].Value.ToString();
            }
        }
    }
}
