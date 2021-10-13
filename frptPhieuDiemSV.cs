using DevExpress.XtraEditors;
using DevExpress.XtraReports.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QLNV1
{
    public partial class frptPhieuDiemSV : DevExpress.XtraEditors.XtraForm
    {
        
        public frptPhieuDiemSV()
        {
            InitializeComponent();
        }

      

        private void frptPhieuDiemSV_Load(object sender, EventArgs e)
        {
           
           
            
        }
       
        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        

        private void btnThoat_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnIn_Click(object sender, EventArgs e)
        {
            if (txbMaSV.Text.Trim().Equals(""))
            {
                MessageBox.Show("Mã sinh viên không để trống", "", MessageBoxButtons.OK);
                txbMaSV.Focus();
                return;
            }
            string msv = txbMaSV.Text;
            int type = 0;
            if (Program.mGroup.Equals("KHOA"))
            {
                type = 1;
            }
            if (Program.mGroup.Equals("PGV"))
            {
                type = 0;
            }
            XrptPhieuDiemSV rpt = new XrptPhieuDiemSV(msv,type);
            rpt.lbMaSV.Text = msv;
            ReportPrintTool print = new ReportPrintTool(rpt);
            print.ShowPreviewDialog();
        }
    }
}