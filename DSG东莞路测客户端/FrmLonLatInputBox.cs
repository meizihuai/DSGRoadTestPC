using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DSG东莞路测客户端
{
    public partial class FrmLonLatInputBox : Form
    {
        private Action<double, double> OnPoint;
        public FrmLonLatInputBox(Action<double, double> onPoint)
        {
            InitializeComponent();
            this.OnPoint = onPoint;
        }

        private void FrmLonLatInputBox_Load(object sender, EventArgs e)
        {
            this.TopMost = true;
            button1.Click += Button1_Click;
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            double.TryParse(txtLon.Text, out double lon);
            double.TryParse(txtLat.Text, out double lat);
            OnPoint?.Invoke(lon, lat);
            this.Close();
        }
    }
}
