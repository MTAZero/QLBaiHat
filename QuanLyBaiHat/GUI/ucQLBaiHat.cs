using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using QuanLyBaiHat.Data;

namespace QuanLyBaiHat.GUI
{
    public partial class QLBAIHat : UserControl
    {
        private QLBAIHatContext db = new QLBAIHatContext();
        private int index = 0, index1 = 0;

        #region Hàm khởi tạo
        public QLBAIHat()
        {
            InitializeComponent();
        }
        #endregion

        #region LoadForm

        private void LoadInitControl()
        {
            cbxCaSi.DataSource = db.CASIs.ToList();
            cbxCaSi.ValueMember = "ID";
            cbxCaSi.DisplayMember = "TEN";
        }

        private void LoadDgvBaiHat()
        {
            int i = 0;
            dgvBAIHAT.DataSource = db.BAIHATs.ToList()
                                   .Where(p=>p.TEN.ToUpper().Contains(txtTimKiem.Text.ToUpper()))
                                   .Select(p => new
                                   {
                                       ID = p.ID,
                                       STT = ++i,
                                       TenBH = p.TEN,
                                       CaSi = db.CASIs.Where(z=>z.ID == p.CASIID).First().TEN,
                                       NamPhatHanh = p.NAMPHATHANH
                                   })
                                   .ToList();
        }
        private void ucQLBaiHat_Load(object sender, EventArgs e)
        {
            LoadInitControl();
            LoadDgvBaiHat();
            LockControl();
        }
        #endregion

        #region Hàm chức năng
        private BAIHAT getBAIHATByID()
        {
            try
            {
                int id = (int)dgvBAIHAT.SelectedRows[0].Cells["ID"].Value;
                BAIHAT ans = db.BAIHATs.Where(p => p.ID == id).FirstOrDefault();
                if (ans == null) return new BAIHAT();
                return ans;
            }
            catch
            {
                return new BAIHAT();
            }
        }

        private BAIHAT getBAIHATByForm()
        {
            BAIHAT ans = new BAIHAT();

            try
            {
                ans.TEN = txtTenBai.Text;
                ans.CASIID = (int) cbxCaSi.SelectedValue;
                ans.NAMPHATHANH = Int32.Parse(txtNamPhatHanh.Text);
            }
            catch { }

            return ans;
        }

        private void ClearControl()
        {
            txtTenBai.Text = "";
            cbxCaSi.SelectedIndex = 0;
            txtNamPhatHanh.Text = "";
        }

        private void UpdateDetail()
        {
            try
            {
                BAIHAT tg = getBAIHATByID();

                if (tg.ID == 0) return;

                txtTenBai.Text = tg.TEN;
                cbxCaSi.SelectedValue = tg.CASIID;
                txtNamPhatHanh.Text = tg.NAMPHATHANH.ToString();
            }
            catch
            {

            }
        }

        private void LockControl()
        {
            txtTenBai.Enabled = false;
            cbxCaSi.Enabled = false;
            txtNamPhatHanh.Enabled = false;

            dgvBAIHAT.Enabled = true;
            txtTimKiem.Enabled = true;

            btnThem.Enabled = true;
            btnSua.Enabled = true;
            btnXoa.Enabled = true;
        }

        private void UnlockControl()
        {
            txtTenBai.Enabled = true;
            cbxCaSi.Enabled = true;
            txtNamPhatHanh.Enabled = true;

            dgvBAIHAT.Enabled = false;
            txtTimKiem.Enabled = false;
        }

        private bool Check()
        {
            if (txtTenBai.Text == "")
            {
                MessageBox.Show("Tên bài hát không được để trống", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            try
            {
                int giasp = Int32.Parse(txtNamPhatHanh.Text);
            }
            catch
            {
                MessageBox.Show("Năm phát hành bài hát phải là số nguyên",
                                "Thông báo",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
                return false;
            }


            return true;
        }

        private bool CheckLuaChon()
        {
            BAIHAT tg = getBAIHATByID();
            if (tg.ID == 0)
            {
                MessageBox.Show("Chưa có bài hát nào được chọn",
                                "Thông báo",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
                return false;
            }
            return true;
        }

        private void CapNhat(ref BAIHAT cu, BAIHAT moi)
        {
            cu.TEN = moi.TEN;
            cu.CASIID = moi.CASIID;
            cu.NAMPHATHANH = moi.NAMPHATHANH;
         }
        #endregion


        #region Sự kiện
        private void btnThem_Click(object sender, EventArgs e)
        {
            if (btnThem.Text == "Thêm")
            {
                btnSua.Enabled = false;
                btnThem.Text = "Lưu";
                btnXoa.Text = "Hủy";

                ClearControl();
                UnlockControl();


                return;
            }

            if (btnThem.Text == "Lưu")
            {
                if (Check())
                {
                    btnThem.Text = "Thêm";
                    btnXoa.Text = "Xóa";
                    LockControl();

                    BAIHAT moi = getBAIHATByForm();
                   
                    db.BAIHATs.Add(moi);

                    try
                    {
                        db.SaveChanges();
                        MessageBox.Show("Thêm thông tin Bài hát thành công",
                                        "Thông báo",
                                        MessageBoxButtons.OK,
                                        MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Thêm thông tin Bài hát thất bại\n" + ex.Message,
                                        "Thông báo",
                                        MessageBoxButtons.OK,
                                        MessageBoxIcon.Error);
                    }
                    LoadDgvBaiHat();
                }
                return;
            }
        }

        private void btnSua_Click(object sender, EventArgs e)
        {
            if (!CheckLuaChon()) return;

            if (btnSua.Text == "Sửa")
            {
                btnSua.Text = "Lưu";
                btnXoa.Text = "Hủy";
                btnThem.Enabled = false;

                UnlockControl();

                return;
            }

            if (btnSua.Text == "Lưu")
            {
                if (Check())
                {
                    btnSua.Text = "Sửa";
                    btnXoa.Text = "Xóa";

                    LockControl();

                    BAIHAT cu = getBAIHATByID();
                    BAIHAT moi = getBAIHATByForm();
                    CapNhat(ref cu, moi);

                    try
                    {
                        db.SaveChanges();
                        MessageBox.Show("Sưa thông tin Bài hát thành công",
                                        "Thông báo",
                                        MessageBoxButtons.OK,
                                        MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Sửa thông tin Bài hát thất bại\n" + ex.Message,
                                        "Thông báo",
                                        MessageBoxButtons.OK,
                                        MessageBoxIcon.Error);
                    }
                    LoadDgvBaiHat();
                }

                return;
            }
        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            if (btnXoa.Text == "Xóa")
            {
                if (!CheckLuaChon()) return;

                BAIHAT cu = getBAIHATByID();
                DialogResult rs = MessageBox.Show("Bạn có chắc chắn xóa Bài hát " + cu.TEN + "?",
                                                  "Thông báo",
                                                  MessageBoxButtons.OKCancel,
                                                  MessageBoxIcon.Warning);

                if (rs == DialogResult.Cancel) return;

                try
                {
                    db.BAIHATs.Remove(cu);
                    db.SaveChanges();
                    MessageBox.Show("Xóa thông tin Bài hát thành công",
                                    "Thông báo",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Xóa thông tin Bài hát thất bại\n" + ex.Message,
                                    "Thông báo",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Error);
                }
                LoadDgvBaiHat();

                return;
            }
            if (btnXoa.Text == "Hủy")
            {
                btnSua.Text = "Sửa";
                btnThem.Text = "Thêm";
                btnXoa.Text = "Xóa";

                LockControl();
                UpdateDetail();
                return;
            }
        }

        private void btnTimKiem_Click(object sender, EventArgs e)
        {
            LoadDgvBaiHat();
            txtTimKiem.Focus();
        }
        #endregion


        #region Sự kiện ngầm
        private void dgvBAIHAT_SelectionChanged(object sender, EventArgs e)
        {
            UpdateDetail();

            try
            {
                index1 = index;
                index = dgvBAIHAT.SelectedRows[0].Index;
            }
            catch { }
        }
        #endregion
    }
}
