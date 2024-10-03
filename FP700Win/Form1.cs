using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using UDatecsProtocol;

namespace FP700Win
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DatecsProtocol fp = new DatecsProtocol("COM10", 115200, ProtocolType.Status_8bytes);
            fp.OpenConnection();
            
            var columns = fp.GetPrintColumns();
            var output = "";
            fp.ExecuteCommand(49, string.Format("{0}[\t]{1}[\t]{2}[\t]{3}[\t]{4}[\t]{5}[\t]{6}[\t]{7}[\t]", new object[]
                            {
                                "test1",
                                "test2",
                                "test3",
                                "test4",
                                "test5",
                                "test6",
                                "test7",
                                "test8"
                            }), ref output);

            fp.CloseConnection();
        }
    }
}
