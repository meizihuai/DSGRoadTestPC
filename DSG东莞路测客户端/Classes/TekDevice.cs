
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace DSG东莞路测客户端
{
  public  class TekDevice
    {
        public int DeviceId { get; set; }
        public string DeviceSerial { get; set; }
        public string DeviceType { get; set; }
        public FreqBscanOrderInfo FBOrderInfo { get; set; }

        public event Action<FreqBscanInfo> OnNewFreq;

        private string freqOrderGuid = "";
        private object freqOrderGuidlock = new object();

        private MTask mTask;
       
        public static Task<TekDevice> SearchDevice()
        {
            return Task.Run(()=>
            {
                while (true)
                {
                    NormalResponse np= TekApi.DeviceSearch();
                    if (!np.result) continue;
                    TekDevice tek = np.Parse<TekDevice>();
                    if (tek == null) continue;
                    return tek;
                }                
            });         
        }
        public bool Connect()
        {
            return TekDriver.DEVICE_Connect(DeviceId) == IntPtr.Zero;
        }

        public NormalResponse DoFreqBscan(double freqStart, double freqStop, double freqStep)
        {
            NormalResponse np = TekApi.DoFreqBscan(freqStart, freqStop, freqStep);
            if (np.result)
            {
                this.FBOrderInfo = np.Parse<FreqBscanOrderInfo>();
                StartReciveFreq();
            }
            return np;
        }
        public void StartReciveFreq()
        {
            mTask = new MTask();
            mTask.SetAction(()=>
            {
                if (FBOrderInfo == null) return;
                if (FBOrderInfo.XX == null) return;
                if (FBOrderInfo.MaxTracePoints <= 0) return;
                double[] xx = FBOrderInfo.XX;
                int maxTracePoint = FBOrderInfo.MaxTracePoints;
                FreqBscanInfo bscanInfo = new FreqBscanInfo();
                bscanInfo.FreqStart = FBOrderInfo.FreqStart;
                bscanInfo.FreqStop = FBOrderInfo.FreqStop;
                bscanInfo.FreqStep = FBOrderInfo.FreqStep;
                bscanInfo.FreqStep = bscanInfo.FreqStep / 1000;
                bscanInfo.FreqDataCount = xx.Length;
                bscanInfo.Freqs = xx;
                for(int i=0;i<bscanInfo.Freqs.Length; i++)
                {
                    bscanInfo.Freqs[i] = bscanInfo.Freqs[i] / 1000000;
                }
                //Stopwatch sp = new Stopwatch();
                //sp.Start();
                string guid = Guid.NewGuid().ToString();
                lock (freqOrderGuidlock)
                {
                    freqOrderGuid= guid;
                }                             
                while (!mTask.IsCancelled() && guid== freqOrderGuid)
                {
                    NormalResponse np = TekApi.GetFreqBscanInfo(maxTracePoint, xx.Length);
                    if (!np.result)
                    {
                        MessageBox.Show($"频谱获取失败，{np.msg}");
                        return;
                    }
                    if (np.msg == "wait" || np.data == null) continue;
                    double[] yy = np.Parse<double[]>();
                    bscanInfo.FreqValues = yy;
                    OnNewFreq?.Invoke(bscanInfo.Copy());
                }
            });
            mTask.Start();
        }
        public void StopWork()
        {
            mTask?.Cancel();
        }

    }
}
