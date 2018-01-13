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
    public partial class ucQLCaSi : UserControl
    {
        private QLBAIHatContext db = new QLBAIHatContext();
        private int index = 0, index1 = 0;

        #region Hàm khởi tạo
        public ucQLCaSi()
        {
            InitializeComponent();
        }
        #endregion

        #region Hàm chức năng
        private CASI getCASIByID()
        {
            try
            {
                int id = (int)dgvCASI.SelectedRows[0].Cells["ID"].Value;
                CASI ans = db.CASIs.Where(p => p.ID == id).FirstOrDefault();
                if (ans == null) return new CASI();
                return ans;
            }
            catch
            {
                return new CASI();
            }
        }

        private CASI getCASIByForm()
        {
            CASI ans = new CASI();

            try
            {
                ans.TEN = txtHoTen.Text;
                ans.GIOITINH = cbxGioiTinh.SelectedIndex;
            }
            catch { }

            return ans;
        }

        private void ClearControl()
        {
            txtHoTen.Text = "";
            cbxGioiTinh.SelectedIndex = 0;
        }

        private void UpdateDetail()
        {
            try
            {
                CASI tg = getCASIByID();

                if (tg.ID == 0) return;

                txtHoTen.Text = tg.TEN;
                cbxGioiTinh.SelectedIndex = (int) tg.GIOITINH;
            }
            catch
            {

            }
        }

        private void LockControl()
        {
            txtHoTen.Enabled = false;
            cbxGioiTinh.Enabled = false;

            dgvCASI.Enabled = true;
            txtTimKiem.Enabled = true;

            btnThem.Enabled = true;
            btnSua.Enabled = true;
            btnXoa.Enabled = true;
        }

        private void UnlockControl()
        {
            txtHoTen.Enabled = true;
            cbxGioiTinh.Enabled = true;

            dgvCASI.Enabled = false;
            txtTimKiem.Enabled = false;
        }

        private bool Check()
        {
            if (txtHoTen.Text == "")
            {
                MessageBox.Show("Tên ca sĩ không được để trống", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            return true;
        }

        private bool CheckLuaChon()
        {
            CASI tg = getCASIByID();
            if (tg.ID == 0)
            {
                MessageBox.Show("Chưa có ca sĩ nào được chọn",
                                "Thông báo",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
                return false;
            }
            return true;
        }

        private void CapNhat(ref CASI cu, CASI moi)
        {
            cu.TEN = moi.TEN;
            cu.GIOITINH = moi.GIOITINH;
        }
        #endregion

        #region LoadForm

        private void LoadDgvCASI()
        {
            int stt = 0;
            dgvCASI.DataSource = db.CASIs.ToList()
                                 .Where(p=>p.TEN.ToUpper().Contains(txtTimKiem.Text.ToUpper()))
                                 .Select(p => new
                                 {
                                     ID = p.ID,
                                     STT = ++stt,
                                     HoTen = p.TEN,
                                     GioiTinh = (p.GIOITINH == 0) ? "Nữ" : "Nam"
                                 })
                                 .ToList();

            /// Load Index
            try
            {
                index = index1;
                dgvCASI.Rows[index].Cells["STT"].Selected = true;
                dgvCASI.Select();
            }
            catch
            {

            }

        }
        private void ucQLCaSi_Load(object sender, EventArgs e)
        {
            LoadDgvCASI();
            LockControl();
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

                    CASI moi = getCASIByForm();

                    db.CASIs.Add(moi);

                    try
                    {
                        db.SaveChanges();
                        MessageBox.Show("Thêm thông tin ca sĩ thành công",
                                        "Thông báo",
                                        MessageBoxButtons.OK,
                                        MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Thêm thông tin ca sĩ thất bại\n" + ex.Message,
                                        "Thông báo",
                                        MessageBoxButtons.OK,
                                        MessageBoxIcon.Error);
                    }
                    LoadDgvCASI();
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

                    CASI cu = getCASIByID();
                    CASI moi = getCASIByForm();
                    CapNhat(ref cu, moi);

                    try
                    {
                        db.SaveChanges();
                        MessageBox.Show("Sưa thông tin ca sĩ thành công",
                                        "Thông báo",
                                        MessageBoxButtons.OK,
                                        MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Sửa thông tin ca sĩ thất bại\n" + ex.Message,
                                        "Thông báo",
                                        MessageBoxButtons.OK,
                                        MessageBoxIcon.Error);
                    }
                    LoadDgvCASI();
                }

                return;
            }
        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            if (btnXoa.Text == "Xóa")
            {
                if (!CheckLuaChon()) return;

                CASI cu = getCASIByID();
                DialogResult rs = MessageBox.Show("Bạn có chắc chắn xóa ca sĩ " + cu.TEN + "?",
                                                  "Thông báo",
                                                  MessageBoxButtons.OKCancel,
                                                  MessageBoxIcon.Warning);

                if (rs == DialogResult.Cancel) return;

                try
                {
                    db.CASIs.Remove(cu);
                    db.SaveChanges();
                    MessageBox.Show("Xóa thông tin ca sĩ thành công",
                                    "Thông báo",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Xóa thông tin ca sĩ thất bại\n" + ex.Message,
                                    "Thông báo",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Error);
                }
                LoadDgvCASI();

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
            LoadDgvCASI();
            txtTimKiem.Focus();
        }
        #endregion

        #region  Sự kiện ngầm
        private void dgvCASI_SelectionChanged(object sender, EventArgs e)
        {
            UpdateDetail();

            try
            {
                index1 = index;
                index = dgvCASI.SelectedRows[0].Index;
            }
            catch { }
        }
        #endregion
    }
}
