using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSG东莞路测客户端
{
    class DiscreteScanInfo
    {
        //设备ID
        public string DeviceID { get; set; }
        public double FreqStart { get; set; }
        public double FreqStop { get; set; }
        //频点数
        public int PointCount { get; set; }
        //频点列表
        public double[] FreqPoints { get; set; }
        //场强列表  与频点列表一 一对应
        public double[] Values { get; set; }
    }
}
