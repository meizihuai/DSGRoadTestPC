using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSG东莞路测客户端
{
   public class FreqGisFileWritter
    {
        public string FileName { get; set; }
        public string WritePath { get; set; }
        private readonly string rootPath = "路测文件夹";
        private string filePath;
        private readonly long maxFileLength=198*1024*1024;  //最大文件字节数  文档要求每个文件不大于200M
        private double mFreqStart, mFreqEnd, mFreqStep;
        public event Action<string, string> OnNewFile;
        public event Action<long> OnFileLengthChange;
        public int DeviceID { get; set; }

        public FreqGisFileWritter(int deviceId,double freqStart,double freqEnd,double freqStep)
        {
            this.DeviceID = deviceId;
            Init(freqStart, freqEnd, freqStep);
        }
        private void Init(double freqStart, double freqEnd, double freqStep)
        {
            mFreqStart = freqStart;
            mFreqEnd = freqEnd;
            mFreqStep = freqStep;

            DateTime now = DateTime.Now;
            FileName = $"44190000_{DeviceID.ToString("0000")}_{now.ToString("yyyyMMdd_HHmmss")}_{freqStart}MHz_{freqEnd}MHz_{(freqStep*1000).ToString("0.000")}kHz_V_M.bin";
            if (!Directory.Exists(rootPath)) Directory.CreateDirectory(rootPath);
            filePath = Path.Combine(rootPath, now.ToString("yyyy-MM-dd"));
            if (!Directory.Exists(filePath)) Directory.CreateDirectory(filePath);
            WritePath = filePath;
            OnNewFile?.Invoke(FileName, WritePath);
        }
        public void WriteFreq(FreqBscanInfo freq,double lon,double lat)
        {
            if (lon <= 100 || lat <= 20) return;
            if (lon >= 200 || lat >=40) return;
            if (Math.Abs(freq.FreqStart- mFreqStart)>=0.001 || Math.Abs(freq.FreqStop- mFreqEnd) >= 0.001 || Math.Abs(freq.FreqStep - mFreqStep) >= 0.001)
            {
                
                Init(freq.FreqStart, freq.FreqStop, freq.FreqStep);
            }
            string path = Path.Combine(filePath, FileName);
            long length = 0;
            if (File.Exists(path))
            {
                FileInfo fileInfo = new FileInfo(path);
                length = fileInfo.Length;
                if (fileInfo.Length >= maxFileLength)
                {
                    Init(freq.FreqStart, freq.FreqStop, freq.FreqStep);
                }
            }
            path = Path.Combine(filePath, FileName);
           
            if (File.Exists(path))
            {
                length = new FileInfo(path).Length;
            }
               


            byte[] head = new byte[] { Hex2Byte("CD"), Hex2Byte("AB"), Hex2Byte("CD"), Hex2Byte("AB") };
            byte[] bufferHead = new byte[52];
            Array.Copy(head, 0, bufferHead, 0, head.Length);
            short deviceId =(short)this.DeviceID;
            byte[] tmp = BitConverter.GetBytes(deviceId);
            //tmp = tmp.Reverse().ToArray();
            Array.Copy(tmp, 0, bufferHead, 4, tmp.Length);

            bufferHead[6] = 1;
            DateTime time = DateTime.Now;
            short year = (short)time.Year;
            tmp = BitConverter.GetBytes(year);
            //tmp = tmp.Reverse().ToArray();
            Array.Copy(tmp, 0, bufferHead, 7, tmp.Length);
            bufferHead[9] = (byte)time.Month;
            bufferHead[10] = (byte)time.Day;
            bufferHead[11] = (byte)time.Hour;
            bufferHead[12] = (byte)time.Minute;
            bufferHead[13] = (byte)time.Second;
            bufferHead[14] = 0;
            bufferHead[15] = 0;

            tmp = BitConverter.GetBytes(lon);
            Array.Copy(tmp, 0, bufferHead, 16, tmp.Length);
            tmp = BitConverter.GetBytes(lat);
            Array.Copy(tmp, 0, bufferHead, 24, tmp.Length);

            float height = 0;
            tmp = BitConverter.GetBytes(height);
            //tmp = tmp.Reverse().ToArray();
            Array.Copy(tmp, 0, bufferHead, 32, tmp.Length);

            double freqStart = (double)freq.FreqStart;
            freqStart = freqStart * 1000 * 1000;
            tmp = BitConverter.GetBytes(freqStart);
            //tmp = tmp.Reverse().ToArray();
            Array.Copy(tmp, 0, bufferHead, 36, tmp.Length);

            float freqStep = (float)freq.FreqStep;
            freqStep = freqStep * 1000*1000;
            tmp = BitConverter.GetBytes(freqStep);
            //tmp = tmp.Reverse().ToArray();
            Array.Copy(tmp, 0, bufferHead, 44, tmp.Length);

            int count = (int)freq.FreqDataCount;
            tmp = BitConverter.GetBytes(count);
            //tmp = tmp.Reverse().ToArray();
            Array.Copy(tmp, 0, bufferHead, 48, tmp.Length);

            byte[] bodyBuffer = new byte[count * 2];
            for (int i = 0; i < count; i++)
            {
                double value = freq.FreqValues[i];
                short s = Convert.ToInt16(value);
                s += 107; //dBm换算为dbVm 
                tmp = BitConverter.GetBytes(s);
                bodyBuffer[i * 2] = tmp[0];
                bodyBuffer[i * 2 + 1] = tmp[1];
            }
            byte[] totalBuffer = bufferHead.Concat(bodyBuffer).ToArray();
            length += totalBuffer.Length;
          
            if (length >= maxFileLength)
            {
                Init(freq.FreqStart, freq.FreqStop, freq.FreqStep);
                path = Path.Combine(filePath, FileName);
                length = 0;
            }
            else
            {
                OnFileLengthChange?.Invoke(length);
            }
          
            using (var stream=new FileStream(path, FileMode.Append))
            {
                using(var bw =new BinaryWriter(stream))
                {                 
                    bw.Write(totalBuffer);
                  
                }
            }
        }


        private byte Hex2Byte(string hx)
        {
            hx = hx.Replace("0x", "").Replace("0X", "");
            int i = Convert.ToInt32(hx, 16);
            return (byte)i;
        }
    }
}
