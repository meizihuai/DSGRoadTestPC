using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSG东莞路测客户端
{
    class WhiteRadioInfo
    {
        public int ID { get; set; }
        public string DateTime { get; set; }
        public double Freq { get; set; }//MHz
        public double Width { get; set; }//KHz
        public string RadioName { get; set; }//电台名称
        public string Mark { get; set; }//备注
        public string RecordUsr { get; set; }
        public string Province { get; set; }
        public string City { get; set; }
        public string District { get; set; }
        private static List<WhiteRadioInfo> mlist = new List<WhiteRadioInfo>();
        public static async void GetWhiteRadioListFromServer()
        {
            NormalResponse np = await API.GetWhiteRadio();
            if (!np.result)
            {
                return;
            }
            else
            {
                mlist = np.Parse<List<WhiteRadioInfo>>();
                if (mlist != null && mlist.Count > 0)
                {
                    foreach (var itm in mlist)
                    {
                        itm.Freq = Math.Round(itm.Freq, 1);
                    }
                }
            }
        }
        public static WhiteRadioInfo GetSignalInfo(double freq)
        {
            if (mlist == null || mlist.Count == 0) return null;
            freq = Math.Round(freq, 1);
            return mlist.Where(a => a.Freq == freq).FirstOrDefault();
        }
    }
}
