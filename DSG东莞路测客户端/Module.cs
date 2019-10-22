using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DSG东莞路测客户端
{
    class Module
    {
        public static FrmMain frmMain;
        delegate void dRunOnUiThread();
        public static void RunUi(Control control, Action action)
        {
            if (control != null)
            {
                control.Invoke(new dRunOnUiThread(action));
            }
            else
            {
                frmMain?.Invoke(new dRunOnUiThread(action));
            }
        }

        public static void Log(string txt)
        {
            frmMain?.Log(txt);
            //File.AppendAllText("log.txt", txt+Environment.NewLine);
        }
    }
}
