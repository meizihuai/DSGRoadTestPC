using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSG东莞路测客户端
{
    public class FreqBscanInfo
    {
        //设备ID
        public string DeviceID { get; set; }
        public string DateTime { get; set; }

        //增益
        public int DbValue { get; set; }
        public string DeviceDHKind { get; set; }
        //起始频率 MHz
        public double FreqStart { get; set; }
        //结束频率 MHz
        public double FreqStop { get; set; }
        //频率步进 KHz
        public double FreqStep { get; set; }
        //横坐标，频谱点
        public double[] Freqs { get; set; }
        //计算后的真实场强值
        public double[] FreqValues { get; set; }
        //频谱点数
        public int FreqDataCount { get; set; }
        //频谱数据，Base64格式的GZIP
        public string FreqData { get; set; }
        // public short[] Value { get; set; }
        //频谱地图经纬度信息
        public GPSInfo GpsInfo { get; set; }
        //频谱类型，0为实时普通频谱，1为最大值保持，2为差分谱
        public int FreqType { get; set; }
       

        //将接收到服务器的base64压缩的频谱，还原
        public void DeValue()
        {
            short[] value = this.DeCompressFreqData(this.FreqData);
            this.FreqData = "";
            double[] freqValue = new double[value.Length];
            double[] xx = new double[freqValue.Length];
            for (int i = 0; i < value.Length; i++)
            {
                double f = value[i];
                freqValue[i] = f * 0.1;
                xx[i] = (double)FreqStart + (double)FreqStep * i;
            }
            xx[xx.Length - 1] = (double)FreqStop;
            this.Freqs = xx;
            this.FreqValues = freqValue;
        }

        public short[] DeCompressFreqData(string base64)
        {
            if (base64 == "") return null;
            try
            {
                byte[] zipBuffer = Convert.FromBase64String(base64);
                byte[] unzipBuffer = GZIPHelper.Decompress(zipBuffer);
                short[] value = new short[unzipBuffer.Length / 2];
                int index = 0;
                for (int i = 0; i < unzipBuffer.Length; i += 2)
                {
                    value[index] = BitConverter.ToInt16(unzipBuffer, i);
                    index++;
                }
                return value;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public string CompressFreqData(short[] value)
        {
            if (value == null) return "";
            byte[] by = new byte[value.Length * 2];
            for (int i = 0; i < value.Length; i++)
            {
                short val = value[i];
                byte[] tmp = BitConverter.GetBytes(val);
                by[i * 2] = tmp[0];
                by[i * 2 + 1] = tmp[1];
            }
            byte[] zipbuffer = GZIPHelper.Compress(by);
            string base64 = Convert.ToBase64String(zipbuffer);
            return base64;
        }
        public void DoReduce()
        {
            int maxCount = 5001;
            if (this.FreqDataCount <= maxCount) return;

            List<double> xlist = new List<double>();
            List<double> ylist = new List<double>();
            int step = (int)Math.Ceiling((double)FreqDataCount / (double)maxCount);
            for (int i = 0; i < FreqDataCount; i += step)
            {
                xlist.Add((double)(FreqStart + i * FreqStep));
                ylist.Add(FreqValues[i]);
            }
            if (xlist[xlist.Count - 1] != (double)FreqStop)
            {
                xlist.Add((double)FreqStop);
                ylist.Add(FreqValues[FreqValues.Length - 1]);
            }
            this.Freqs = xlist.ToArray();
            this.FreqValues = ylist.ToArray();
            this.FreqDataCount = xlist.Count;

            this.FreqStep = (FreqStop - FreqStart) / (FreqDataCount - 1);

        }

        public FreqBscanInfo Copy()
        {
            if (this == null) return null;
            FreqBscanInfo cp = (FreqBscanInfo)this.MemberwiseClone();
            if (GpsInfo != null) cp.GpsInfo = this.GpsInfo.Copy();
            return cp;
        }
        //减法，差分用
        public static FreqBscanInfo operator -(FreqBscanInfo newFreq, FreqBscanInfo moduleFreq)
        {
            if (newFreq.FreqStart != moduleFreq.FreqStart || newFreq.FreqStop != moduleFreq.FreqStop || newFreq.FreqStep != moduleFreq.FreqStep) return null;
            if (newFreq.FreqValues == null || newFreq.FreqValues == null) return null;
            FreqBscanInfo info = new FreqBscanInfo();
            info.FreqStart = newFreq.FreqStart;
            info.FreqStop = newFreq.FreqStop;
            info.FreqStep = newFreq.FreqStep;
            info.FreqValues = newFreq.FreqValues.ToArray();
            info.Freqs = newFreq.Freqs.ToArray();
            for (int i = 0; i < info.FreqValues.Length; i++)
            {
                info.FreqValues[i] = info.FreqValues[i] - moduleFreq.FreqValues[i];
            }
            return info;
        }
        public double GetFreqPointValue(double freq, double selfValue)
        {
            if (this.Freqs == null || this.FreqValues == null || this.FreqStart > (double)freq || this.FreqStop < (double)freq) return selfValue;
            double abs = double.MaxValue;
            double resultValue = selfValue;
            for (int i = 0; i < Freqs.Length; i++)
            {
                double pointFreq = Freqs[i];
                double newAbs = Math.Abs(pointFreq - freq);
                if (newAbs == 0)
                {
                    return FreqValues[i];
                }
                if (abs >= newAbs)
                {
                    abs = newAbs;
                    resultValue = FreqValues[i];
                }
                else
                {
                    return resultValue;
                }
            }
            return resultValue;
        }


        /// <summary>
        /// 最大值保持频谱更新最大值谱
        /// </summary>
        /// <param name="info"></param>
        public void GetMaxer(FreqBscanInfo info)
        {
            if (info.FreqValues == null || info.Freqs == null) return;
            if (this.FreqValues == null || this.Freqs == null) return;
            if (info.FreqStart != this.FreqStart || info.FreqStop != this.FreqStop || info.FreqStep != this.FreqStep) return;
            if (info.FreqValues.Length != this.FreqValues.Length || info.Freqs.Length != this.Freqs.Length) return;
            for (int i = 0; i < info.FreqValues.Length; i++)
            {
                double oldValue = this.FreqValues[i];
                double newValue = info.FreqValues[i];
                if (newValue > oldValue)
                    this.FreqValues[i] = newValue;
            }

        }
        /// <summary>
        /// 最小值保持谱更新最小值谱
        /// </summary>
        /// <param name="info"></param>
        public void GetMiner(FreqBscanInfo info)
        {
            if (info.FreqValues == null || info.Freqs == null) return;
            if (this.FreqValues == null || this.Freqs == null) return;
            if (info.FreqStart != this.FreqStart || info.FreqStop != this.FreqStop || info.FreqStep != this.FreqStep) return;
            if (info.FreqValues.Length != this.FreqValues.Length || info.Freqs.Length != this.Freqs.Length) return;
            for (int i = 0; i < info.FreqValues.Length; i++)
            {
                double oldValue = this.FreqValues[i];
                double newValue = info.FreqValues[i];
                if (newValue < oldValue)
                    this.FreqValues[i] = newValue;
            }
        }
    }
}
