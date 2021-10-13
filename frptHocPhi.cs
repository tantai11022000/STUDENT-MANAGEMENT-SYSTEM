using DevExpress.XtraEditors;
using DevExpress.XtraReports.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QLNV1
{
    public partial class frptHocPhi : DevExpress.XtraEditors.XtraForm
    {
        public frptHocPhi()
        {
            InitializeComponent();
        }

        private void lOPBindingNavigatorSaveItem_Click(object sender, EventArgs e)
        {
            this.Validate();
            this.bdsLop.EndEdit();
            this.tableAdapterManager.UpdateAll(this.DS);

        }
        public static string NumberToText(double inputNumber, bool suffix = true)
        {
            string[] unitNumbers = new string[] { "không", "một", "hai", "ba", "bốn", "năm", "sáu", "bảy", "tám", "chín" };
            string[] placeValues = new string[] { "", "nghìn", "triệu", "tỷ" };
            bool isNegative = false;

            // -12345678.3445435 => "-12345678"
            string sNumber = inputNumber.ToString("#");
            double number = Convert.ToDouble(sNumber);
            if (number < 0)
            {
                number = -number;
                sNumber = number.ToString();
                isNegative = true;
            }


            int ones, tens, hundreds;

            int positionDigit = sNumber.Length;   // last -> first

            string result = " ";


            if (positionDigit == 0)
                result = unitNumbers[0] + result;
            else
            {
                // 0:       ###
                // 1: nghìn ###,###
                // 2: triệu ###,###,###
                // 3: tỷ    ###,###,###,###
                int placeValue = 0;

                while (positionDigit > 0)
                {
                    // Check last 3 digits remain ### (hundreds tens ones)
                    tens = hundreds = -1;
                    ones = Convert.ToInt32(sNumber.Substring(positionDigit - 1, 1));
                    positionDigit--;
                    if (positionDigit > 0)
                    {
                        tens = Convert.ToInt32(sNumber.Substring(positionDigit - 1, 1));
                        positionDigit--;
                        if (positionDigit > 0)
                        {
                            hundreds = Convert.ToInt32(sNumber.Substring(positionDigit - 1, 1));
                            positionDigit--;
                        }
                    }

                    if ((ones > 0) || (tens > 0) || (hundreds > 0) || (placeValue == 3))
                        result = placeValues[placeValue] + result;

                    placeValue++;
                    if (placeValue > 3) placeValue = 1;

                    if ((ones == 1) && (tens > 1))
                        result = "một " + result;
                    else
                    {
                        if ((ones == 5) && (tens > 0))
                            result = "lăm " + result;
                        else if (ones > 0)
                            result = unitNumbers[ones] + " " + result;
                    }
                    if (tens < 0)
                        break;
                    else
                    {
                        if ((tens == 0) && (ones > 0)) result = "lẻ " + result;
                        if (tens == 1) result = "mười " + result;
                        if (tens > 1) result = unitNumbers[tens] + " mươi " + result;
                    }
                    if (hundreds < 0) break;
                    else
                    {
                        if ((hundreds > 0) || (tens > 0) || (ones > 0))
                            result = unitNumbers[hundreds] + " trăm " + result;
                    }
                    result = " " + result;
                }
            }
            result = result.Trim();
            if (isNegative) result = "Âm " + result;
            return "("+ result + (suffix ? " đồng chẵn)" : ")");
        }
    

private void frptHocPhi_Load(object sender, EventArgs e)
        {
            if (Program.mGroup.Equals("PGV"))
            {
                Program.severname = "LAPTOP-K21D5PFV\\MSSQL3";
                Program.mlogin = Program.remotelogin;
                Program.password = Program.remotepassword;
                if (Program.KetNoi() == 0)
                {
                    MessageBox.Show("Lỗi kết nối về chi nhánh mới", "", MessageBoxButtons.OK);
                }
            }
            this.LOPTableAdapter.Connection.ConnectionString = Program.connstr;
            this.LOPTableAdapter.Fill(this.DS.LOP);

            cbLop.DataSource = bdsLop;
            cbLop.DisplayMember = "MALOP";
            cbLop.ValueMember = "TENLOP";
        }

        private void btnIn_Click(object sender, EventArgs e)
        {
            
            if (txbNienKhoa.Text.Trim() == "")
            {
                MessageBox.Show("Niên khóa không được để trống", "", MessageBoxButtons.OK);
                txbNienKhoa.Focus();
                return;
            }
            if (nmHocKy.Value == 0)
            {
                MessageBox.Show("Học Kỳ không được để trống", "", MessageBoxButtons.OK);
                nmHocKy.Focus();
                return;
            }
            string nienkhoa = txbNienKhoa.Text;
            int hocky = (int)nmHocKy.Value;
            string malop = cbLop.Text;
            string tongtien = "";
            string cmd = "SELECT TENKHOA FROM dbo.LOP,dbo.KHOA WHERE MALOP = '" + malop + "' AND KHOA.MAKHOA = LOP.MAKHOA";
            SqlDataReader reader = Program.ExecSqlDataReader(cmd);
            reader.Read();
            string tenkhoa = reader.GetString(0);
            reader.Close();

                string cmd1 = "EXEC [dbo].[SP_TongTienHocPhi] '" + malop + "', '" + nienkhoa + "', " + hocky;
                SqlDataReader reader1 = Program.ExecSqlDataReader(cmd1);
                reader1.Read();
                tongtien = reader1.GetInt32(0).ToString();     
                reader1.Close();

          
           
            if(tongtien != "0")
            {
                tongtien = NumberToText(double.Parse(tongtien));
            }
           

            XrptInHocPhi rpt = new XrptInHocPhi(malop, nienkhoa, hocky);
            rpt.lbMaLop.Text = malop;
            rpt.lbKhoa.Text = tenkhoa;
            rpt.lbTienChu.Text = tongtien;
    
          

            ReportPrintTool print = new ReportPrintTool(rpt);
            print.ShowPreviewDialog();
        }

        private void btnThoat_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void cbLop_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}