using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TelefonRehberi
{
    public partial class KisiBilgileri : Form
    {
        public KisiBilgileri()
        {
            InitializeComponent();
        }

        private void GruplarYukleAraCombo()
        {
            using(var db= new TelefonRehberiDBEntities1())
            {
                var gruplar = db.Gruplar.Select(g => new {g.Grup_Id, g.Grup_Adi}).ToList();

                gruplar.Insert(0, new { Grup_Id = 0, Grup_Adi = "Tümü" });

                cmbGrupAra.DataSource = gruplar;
                cmbGrupAra.DisplayMember = "Grup_Adi";
                cmbGrupAra.ValueMember = "Grup_Id";
                cmbGrupAra.SelectedIndex = -1;
            }
        }
        private void GruplarYukleCombo()
        {
            using(var db=new TelefonRehberiDBEntities1())
            {
                var gruplar = db.Gruplar.Select(g => new {g.Grup_Id, g.Grup_Adi}).ToList();

                cmbGrup.DataSource= gruplar;
                cmbGrup.DisplayMember = "Grup_Adi";
                cmbGrup.ValueMember = "Grup_Id";
                cmbGrup.SelectedIndex = -1;
            }
        }

        private void KisilerYukle()
        {
            using (var db = new TelefonRehberiDBEntities1())
            {
                var kisiler = db.Kisiler.Select(k => new
                {
                    k.Kisi_Id,
                    k.Ad,
                    k.Soyad,
                    k.Tel_No1,
                    k.Tel_No2,
                    k.Mail,
                    k.Unvan,
                    Grup = (k.Gruplar!=null ? k.Gruplar.Grup_Adi:"Grupsuz")
                }).ToList();

                grdKisiler.DataSource = kisiler;
                               
            }

            grdKisiler.AutoSizeColumnsMode=DataGridViewAutoSizeColumnsMode.Fill;
            grdKisiler.SelectionMode=DataGridViewSelectionMode.FullRowSelect;
            grdKisiler.MultiSelect=false;
        }

        private bool formHazir=false;
        private void KisiBilgileri_Load(object sender, EventArgs e)
        {
            KisilerYukle();
            GruplarYukleCombo();
            GruplarYukleAraCombo();

            formHazir = true;
        }

        private void btnEkle_Click(object sender, EventArgs e)
        {
            using (var db=new TelefonRehberiDBEntities1())
            {
                var yeniKisi = new Kisiler
                {
                    Ad = txtAd.Text,
                    Soyad = txtSoyad.Text,
                    Tel_No1 = txtTel1.Text,
                    Tel_No2 = txtTel2.Text,
                    Mail = txtMail.Text,
                    Unvan = txtUnvan.Text,
                    Grup_ID = (int)cmbGrup.SelectedValue
                };

                db.Kisiler.Add(yeniKisi);
                db.SaveChanges();
            }

            MessageBox.Show("Yeni kişi eklendi");
            KisilerYukle();
            Temizle();
        }

        private void btnSil_Click(object sender, EventArgs e)
        {
            if (grdKisiler.SelectedRows.Count > 0)
            {
                int kisiId = Convert.ToInt32(grdKisiler.SelectedRows[0].Cells["Kisi_Id"].Value);

                using (var db = new TelefonRehberiDBEntities1())
                {
                    var kisi=db.Kisiler.FirstOrDefault(x=>x.Kisi_Id == kisiId);
                    if (kisi!=null)
                    {
                        db.Kisiler.Remove(kisi);
                        db.SaveChanges() ;
                    }
                }
                MessageBox.Show("Kişi Silindi");
                KisilerYukle();
                Temizle();
            }
        }

        private void btnGuncelle_Click(object sender, EventArgs e)
        {
            if (grdKisiler.SelectedRows.Count>0)
            {
                int kisiId = Convert.ToInt32(grdKisiler.SelectedRows[0].Cells["Kisi_Id"].Value);

                using (var db = new TelefonRehberiDBEntities1())
                {
                    var kisi = db.Kisiler.FirstOrDefault(x => x.Kisi_Id == kisiId);
                    if (kisi!=null)
                    {
                        kisi.Ad = txtAd.Text;
                        kisi.Soyad = txtSoyad.Text;
                        kisi.Tel_No1=txtTel1.Text;
                        kisi.Tel_No2 = txtTel2.Text;
                        kisi.Mail=txtMail.Text;
                        kisi.Unvan=txtUnvan.Text;
                        kisi.Grup_ID = (int)cmbGrup.SelectedValue;
                        db.SaveChanges();
                    }
                }
                MessageBox.Show("Kişi Güncellendi");
                KisilerYukle();
                Temizle();
            }
        }

        private void grdKisiler_CellClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void Temizle()
        {
            txtAd.Clear();
            txtSoyad.Clear();
            txtTel1.Clear();
            txtTel2.Clear();
            txtMail.Clear();    
            txtUnvan.Clear();
            cmbGrup.SelectedIndex = -1;
        }

        private void KisilerAra()
        {
            using (var db = new TelefonRehberiDBEntities1())
            {
                var query = db.Kisiler.AsQueryable();

                // İsim - soyisim arama
                if (!string.IsNullOrEmpty(txtAdAra.Text))
                    query = query.Where(k => k.Ad.Contains(txtAdAra.Text));

                if (!string.IsNullOrEmpty(txtSoyadAra.Text))
                    query = query.Where(k => k.Soyad.Contains(txtSoyadAra.Text));

                // Grup filtresi (0 = Tümü demek)
                if (cmbGrupAra.SelectedValue != null && (int)cmbGrupAra.SelectedValue != 0)
                {
                    int grupId = (int)cmbGrupAra.SelectedValue;
                    query = query.Where(k => k.Grup_ID == grupId);
                }

                var kisiler = query
                                .Select(k => new
                                {
                                    k.Kisi_Id,
                                    k.Ad,
                                    k.Soyad,
                                    k.Tel_No1,
                                    k.Tel_No2,
                                    k.Mail,
                                    k.Unvan,
                                    Grup = (k.Gruplar != null ? k.Gruplar.Grup_Adi : "Grupsuz")
                                })
                                .ToList();

                grdKisiler.DataSource = kisiler;
            }
        }


        private void txtAdAra_TextChanged(object sender, EventArgs e)
        {
            KisilerAra();
        }

        private void txtSoyadAra_TextChanged(object sender, EventArgs e)
        {
            KisilerAra();
        }

        private void txtTelAra_TextChanged(object sender, EventArgs e)
        {
            KisilerAra();
        }

        private void cmbGrupAra_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!formHazir) return;
            KisilerAra();

            
        }

        private void btnYeniGrup_Click(object sender, EventArgs e)
        {
            GrupIslemleri frm=new GrupIslemleri();
            frm.ShowDialog();

            GruplarYukleCombo();
            GruplarYukleAraCombo();
        }

        private void grdKisiler_Click(object sender, EventArgs e)
        {

        }
    }
}
