using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using static TekDriver;

namespace DSG东莞路测客户端
{
  public static  class TekApi
    {
        private static NormalResponse ApiResultToNp(IntPtr i)
        {
            if (i.ToInt32() == 0) return new NormalResponse(true, "");
            IntPtr ptr = GetErrorString(i);
            string err = Marshal.PtrToStringAnsi(ptr);
            return new NormalResponse(false, err);
        }
        public static NormalResponse DeviceSearch()
        {
            int num = 0;
            int[] ids = new int[10];
            StringBuilder serial = new StringBuilder();
            StringBuilder type = new StringBuilder();
            IntPtr rt = DEVICE_Search(ref num,  ids,  serial,  type);
            NormalResponse np = ApiResultToNp(rt);
            if (!np.result) return np;
            if (num == 0) return new NormalResponse(false, "没有发现设备");
            TekDevice tek = new TekDevice();
            tek.DeviceId = ids[0];
            tek.DeviceSerial = serial.ToString();
            tek.DeviceType = type.ToString();
            return new NormalResponse(true, "", "", tek);
        }
        
        public static NormalResponse DeviceReset(int deviceId)
        {
            IntPtr ptr = DEVICE_Reset(deviceId);
            return ApiResultToNp(ptr);
        }
        public static NormalResponse DeviceConnect(int deviceId)
        {
            IntPtr ptr = DEVICE_Connect(deviceId);
            return ApiResultToNp(ptr);
        }
        /// <summary>
        /// 下发频谱命令到设备
        /// </summary>
        /// <param name="freqStart">起始频率 MHz</param>
        /// <param name="freqEnd">终止频率 MHz</param>
        /// <param name="freqStep">频率步进 KHz</param>
        /// <returns></returns>
        public static NormalResponse DoFreqBscan(double freqStart,double freqEnd,double freqStep)
        {
            freqStart = freqStart * 1000000;
            freqEnd = freqEnd * 1000000;
            freqStep = freqStep * 1000;
            double freqCenter = (freqStart + freqEnd) / 2;
            int traceLength = (int)((freqEnd - freqStart) / freqStep) + 1;
            if (!ApiResultToNp(SPECTRUM_SetEnable(true)).result)
                return new NormalResponse(false, "设备此时无法执行频谱命令，请检查设备连接状态");
            if (!ApiResultToNp(CONFIG_SetCenterFreq(freqCenter)).result)
                return new NormalResponse(false, "中心频点设置失败");
            Spectrum_Settings settings = new Spectrum_Settings();
            settings.span = freqEnd - freqStart;
            settings.rbw = 1000;
            settings.enableVBW = false;
            settings.vbw = 200000;
            settings.traceLength = GetTraceLength(freqStart, freqEnd, freqStep);
            settings.window = 0;
            settings.verticalUnit = 0;
            if(!ApiResultToNp(SPECTRUM_SetSettings(settings)).result)
                return new NormalResponse(false, "频谱命令参数设置失败");
            settings = RunApiGetNp<Spectrum_Settings, Spectrum_Settings>(SPECTRUM_GetSettings);
            if (!ApiResultToNp(DEVICE_Run()).result)
                return new NormalResponse(false, "设备无法运行命令");
            double[] xx = new double[settings.traceLength];
            for (int i = 0; i < xx.Length; i++)
                xx[i] = settings.actualStartFreq + i * settings.actualFreqStepSize;
            FreqBscanOrderInfo freqBscanOrderInfo = new FreqBscanOrderInfo(xx, xx.Length);
            freqBscanOrderInfo.FreqStart = settings.actualStartFreq / 1000000;
            freqBscanOrderInfo.FreqStop = settings.actualStopFreq / 1000000;
            freqBscanOrderInfo.FreqStep = settings.actualFreqStepSize / 1000;
            return new NormalResponse(true, "", "", freqBscanOrderInfo);
        }

       public static NormalResponse GetFreqBscanInfo(int maxTracePoint,int freqPointCount)
        {
         
            int size = maxTracePoint * Marshal.SizeOf(typeof(float));
            if (!ApiResultToNp(SPECTRUM_AcquireTrace()).result)
                return new NormalResponse(false, "无法获取频谱");
            bool ready = false;
            if (!ApiResultToNp(SPECTRUM_WaitForTraceReady(1000,ref ready)).result)
                return new NormalResponse(false, "等待数据频谱失败");
            if(!ready)
                return new NormalResponse(true, "wait", "", null);
            IntPtr traceData = Marshal.AllocHGlobal(size);
            int outTracePoint = 0;
            int trace = 0;
            if(!ApiResultToNp(SPECTRUM_GetTrace(trace,maxTracePoint,traceData,ref outTracePoint)).result)
                return new NormalResponse(false, "获取频谱数据失败");
            byte[] buffer = new byte[size];
            Marshal.Copy(traceData, buffer, 0, buffer.Length);
            int idx = 0;
            double[] freq = new double[freqPointCount];
            for(int i = 0; i < buffer.Length; i+=4)
            {
                float f = BitConverter.ToSingle(buffer, i);
                if (idx >= freq.Length) break;
                freq[idx] = f;
                idx++;
            }      
            return new NormalResponse(true, "","", freq);
        }

        public static V RunApiGetNp<T,V>(Func<IntPtr,IntPtr> func)
        {
            int size = Marshal.SizeOf(typeof(T));
            IntPtr ptr = Marshal.AllocHGlobal(size);
            IntPtr rt = func(ptr);
            NormalResponse np = ApiResultToNp(rt);
            if (!np.result) return default(V);
            return (V)Marshal.PtrToStructure(ptr, typeof(V));
        }

        private static int GetTraceLength(double freqStart, double freqEnd, double freqStep)
        {
            int realCount = (int)((freqEnd - freqStart) / freqStep);
            realCount = realCount + 1;
            // Return realCount
            if (realCount <= 801)
                return 801;
            if (realCount <= 2401)
                return 2401;
            if (realCount <= 4001)
                return 4001;
            if (realCount <= 8001)
                return 8001;
            if (realCount <= 10401)
                return 10401;
            if (realCount <= 16001)
                return 16001;
            if (realCount <= 32001)
                return 32001;
            return 64001;
        }

    }
}
