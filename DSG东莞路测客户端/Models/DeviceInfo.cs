using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSG东莞路测客户端
{
    class DeviceInfo
    {
        public string DeviceID { get; set; }
        public string Kind { get; set; }
        public string DeviceNickName { get; set; }
        public string OnlineTime { get; set; }
        public string Status { get; set; }
        public bool IsTasking { get; set; }
        public bool IsReadyForWork { get; set; }
        public string RunMode { get; set; }
        public double Lng { get; set; }
        public double Lat { get; set; }
        public string IP { get; set; }
        public int Port { get; set; }
        public string RunKind { get; set; }
        public string Province { get; set; }
        public string City { get; set; }
        public string District { get; set; }
        public string Address { get; set; }
        public string DSGWGDeviceId { get; set; }
        public double TodayFlow { get; set; }
        public double MonthFlow { get; set; }
        public double YearFlow { get; set; }
        public bool IsOnline { get; set; }
    }
}
