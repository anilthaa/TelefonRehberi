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
                var kisiler = db.Kisiler.AsEnumerable().Select((k,index) => new
                {
                    Sira=index+1,
                    k.Kisi_Id,
                    k.Ad,
                    k.Soyad,
                    k.Tel_No1,
                    k.Tel_No2,
                    k.Mail,
                    k.Unvan,
                    k.Grup_ID,
                    Grup = (k.Gruplar!=null ? k.Gruplar.Grup_Adi:"Grupsuz")
                }).ToList();

                grdKisiler.DataSource = kisiler;
                grdKisiler.Columns["Kisi_Id"].Visible = false;
                
            }

            grdKisiler.AutoSizeColumnsMode=DataGridViewAutoSizeColumnsMode.Fill;
            grdKisiler.SelectionMode=DataGridViewSelectionMode.FullRowSelect;
            grdKisiler.MultiSelect=false;
        }

        private bool formHazir=false;
        private void KisiBilgileri_Load(object sender, EventArgs e)
        {
            formHazir = false;
            KisilerGridSutunlariOlustur();
            GruplarYukleCombo();
            GruplarYukleAraCombo();
            KisilerYukle();

            formHazir=true;
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
            if (e.RowIndex>=0)
            {
                var row = grdKisiler.Rows[e.RowIndex];
                
                txtAd.Text = row.Cells["Ad"].Value.ToString();
                txtSoyad.Text = row.Cells["Soyad"].Value.ToString();
                txtTel1.Text = row.Cells["Tel_No1"].Value.ToString();
                txtTel2.Text = row.Cells["Tel_No2"].Value.ToString();
                txtMail.Text = row.Cells["Mail"].Value.ToString();
                txtUnvan.Text = row.Cells["Unvan"].Value.ToString();

                var grupID = row.Cells["Grup_ID"].Value;
                if(grupID!=null && int.TryParse(grupID.ToString(), out int grupId))
                {
                    cmbGrup.SelectedValue = grupId;
                }
                else
                {
                    cmbGrup.SelectedIndex = -1;
                }
            }
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
            using (var frm=new GrupIslemleri())
            {
                frm.ShowDialog();
                GruplarYukleCombo();
                GruplarYukleAraCombo();
                KisilerYukle();
            }



        }

        private void grdKisiler_Click(object sender, EventArgs e)
        {

        }
        private void KisilerGridSutunlariOlustur()
        {
            grdKisiler.AutoGenerateColumns = false;
            grdKisiler.Columns.Clear();

            grdKisiler.Columns.Add(new DataGridViewTextBoxColumn()
            {
                DataPropertyName = "Sira",
                HeaderText = "Sıra",
                Name = "Sira",
                Width = 50
            });
            grdKisiler.Columns.Add(new DataGridViewTextBoxColumn()
            {
                DataPropertyName = "Kisi_Id",
                HeaderText = "ID",
                Name = "Kisi_Id",
                Visible = false
            });
            grdKisiler.Columns.Add(new DataGridViewTextBoxColumn()
            {
                DataPropertyName = "Ad",
                HeaderText = "Ad",
                Name = "Ad",
            });
            grdKisiler.Columns.Add(new DataGridViewTextBoxColumn()
            {
                DataPropertyName = "Soyad",
                HeaderText = "Soyad",
                Name = "Soyad"
            });
            grdKisiler.Columns.Add(new DataGridViewTextBoxColumn()
            {
                DataPropertyName = "Tel_No1",
                HeaderText = "Telefon 1",
                Name = "Tel_No1",
            });
            grdKisiler.Columns.Add(new DataGridViewTextBoxColumn()
            {
                DataPropertyName = "Tel_No2",
                HeaderText = "Telefon 2",
                Name = "Tel_No2"
            });
            grdKisiler.Columns.Add(new DataGridViewTextBoxColumn()
            {
                DataPropertyName = "Mail",
                HeaderText = "Mail",
                Name = "Mail"
            });
            grdKisiler.Columns.Add(new DataGridViewTextBoxColumn()
            {
                DataPropertyName = "Unvan",
                HeaderText = "Unvan",
                Name = "Unvan"
            });
            grdKisiler.Columns.Add(new DataGridViewTextBoxColumn()
            {
                DataPropertyName = "Grup_Id",
                HeaderText = "Grup ID",
                Name = "Grup_Id",
                Visible = false
            });
            grdKisiler.Columns.Add(new DataGridViewTextBoxColumn()
            {
                DataPropertyName = "Grup",
                HeaderText = "Grup",
                Name = "Grup"
            });

            grdKisiler.AutoSizeColumnsMode=DataGridViewAutoSizeColumnsMode.Fill;
            grdKisiler.SelectionMode=DataGridViewSelectionMode.FullRowSelect;
            grdKisiler.MultiSelect = false;
            grdKisiler.ReadOnly = true;
            grdKisiler.AllowUserToAddRows = false;
            grdKisiler.AllowUserToDeleteRows = false;
        }

        private void btnGruptanCikar_Click(object sender, EventArgs e)
        {
            if (grdKisiler.CurrentRow==null)
            {
                MessageBox.Show("Lütfen önce bir kişi seçiniz.","Uyarı", MessageBoxButtons.OK,MessageBoxIcon.Warning);
                return;
            }

            int kisiId = Convert.ToInt32(grdKisiler.CurrentRow.Cells["Kisi_Id"].Value);

            

            using (var db= new TelefonRehberiDBEntities1())
            {
                var kisi=db.Kisiler.FirstOrDefault(k=>k.Kisi_Id==kisiId);
                if (kisi==null)
                {
                    MessageBox.Show("Kişi bulunamadı","Hata",MessageBoxButtons.OK,MessageBoxIcon.Error);
                    return;
                }

                if (kisi.Grup_ID==null)
                {
                    MessageBox.Show("Bu kişi zaten grupta değil.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                DialogResult cevap=MessageBox.Show("Bu kişiyi gruptan çıkarmak istediğinize emin misiniz?","Onay",MessageBoxButtons.YesNo,MessageBoxIcon.Question);

                if (cevap==DialogResult.No)
                {
                    return;
                }

                kisi.Grup_ID = null;
                db.SaveChanges();
            }

            KisilerYukle();

            MessageBox.Show("Kişi gruptan çıkarıldı.","Bilgi",MessageBoxButtons.OK,MessageBoxIcon.Information);
        } 
    }
}
