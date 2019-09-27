using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSG东莞路测客户端
{
  public class FreqBscanOrderInfo
    {
        public double FreqStart { get; set; } //MHz
        public double FreqStop { get; set; } //MHz
        public double FreqStep { get; set; } //KHz
        public double[] XX { get; set; }
        public int MaxTracePoints { get; set; }
        public FreqBscanOrderInfo(double[]xx,int maxTracePoints)
        {
            this.XX = xx;
            this.MaxTracePoints = maxTracePoints;
        }
    }
}
