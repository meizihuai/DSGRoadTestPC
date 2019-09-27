using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSG东莞路测客户端
{
    public class GPSInfo
    {
        public string Time { get; set; }
        public double Lng { get; set; }
        public double Lat { get; set; }
        public int GridID { get; set; }
        public LocationInfo Location { get; set; }
        public GPSInfo()
        {

        }
        public GPSInfo(double lng, double lat)
        {
            this.Lng = lng;
            this.Lat = lat;
            this.Time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        }
        public GPSInfo Copy()
        {
            if (this == null) return null;
            return (GPSInfo)this.MemberwiseClone();
        }
    }
    public class GridInfo
    {
        public int ID { get; set; }
        public double StartLon { get; set; }
        public double StopLon { get; set; }
        public double StartLat { get; set; }
        public double StopLat { get; set; }
        public int? CM { get; set; }
        public int? CU { get; set; }
        public int? CT { get; set; }

    }
    public class LocationInfo
    {
        public string Province { get; set; }
        public string City { get; set; }
        public string District { get; set; }
        public string DetailAddress { get; set; }
    }
}
