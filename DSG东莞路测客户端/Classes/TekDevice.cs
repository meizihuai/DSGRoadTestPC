
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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
        public FreqBscanOrderInfo RealFBOrderInfo { get; set; }

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


                RealFBOrderInfo = new FreqBscanOrderInfo(FBOrderInfo.XX.ToList().ToArray(), FBOrderInfo.XX.Length);
                RealFBOrderInfo.FreqStart = freqStart;
                RealFBOrderInfo.FreqStop = freqStop;
                RealFBOrderInfo.FreqStep = freqStep;
                int freqPointCount = (int)((freqStop - freqStart) * 1000 * 1000 / (freqStep * 1000)) + 1;
                RealFBOrderInfo.MaxTracePoints = freqPointCount;
                RealFBOrderInfo.XX = new double[freqPointCount];
                for (int i = 0; i < freqPointCount; i++)
                {
                    RealFBOrderInfo.XX[i] = freqStart + (freqStep / 1000) * i;
                }
               // File.WriteAllText("RealFBOrderInfo.txt", JsonConvert.SerializeObject(RealFBOrderInfo, Formatting.Indented));
                StartReciveFreq();
               
            }
            return np;
        }
        public void StartReciveFreq()
        {
            mTask = new MTask();
            mTask.SetAction(()=>
            {             
                try
                {
                    if (FBOrderInfo == null)
                        return;
                    if (FBOrderInfo.XX == null)
                        return;
                    if (FBOrderInfo.MaxTracePoints <= 0)
                        return;

                    double[] xx = FBOrderInfo.XX;
                    int maxTracePoint = FBOrderInfo.MaxTracePoints;
                    FreqBscanInfo bscanInfo = new FreqBscanInfo();
                    bscanInfo.FreqStart = FBOrderInfo.FreqStart;
                    bscanInfo.FreqStop = FBOrderInfo.FreqStop;
                    bscanInfo.FreqStep = FBOrderInfo.FreqStep;
                    bscanInfo.FreqStep = bscanInfo.FreqStep / 1000;
                    bscanInfo.FreqDataCount = xx.Length;
                    bscanInfo.Freqs = xx;
                    for (int i = 0; i < bscanInfo.Freqs.Length; i++)
                    {
                        bscanInfo.Freqs[i] = bscanInfo.Freqs[i] / 1000000;
                    }
                    //Stopwatch sp = new Stopwatch();
                    //sp.Start();
                    string guid = Guid.NewGuid().ToString();
                    lock (freqOrderGuidlock)
                    {
                        freqOrderGuid = guid;
                    }
                  
                    while (!mTask.IsCancelled() && guid == freqOrderGuid)
                    {
                        NormalResponse np = TekApi.GetFreqBscanInfo(maxTracePoint, xx.Length);
                       // Module.Log(np.result + "");
                        if (!np.result)
                        {
                            MessageBox.Show($"频谱获取失败，{np.msg}");
                            return;
                        }
                        if (np.msg == "wait" || np.data == null) continue;
                        double[] yy = np.Parse<double[]>();
                        bscanInfo.FreqValues = yy;
                        try
                        {
                            //Module.Log("start fix");
                            FreqBscanInfo realfreq = FreqFix(bscanInfo.Copy());
                            //Module.Log($"end fix [{realfreq.FreqStart},{realfreq.FreqStop}] {realfreq.FreqStep} KHz");
                            //File.WriteAllText("realfreq.txt", JsonConvert.SerializeObject(realfreq, Formatting.Indented));
                            OnNewFreq?.Invoke(realfreq);
                        }
                        catch (Exception e)
                        {
                            File.WriteAllText("err.txt", e.ToString());
                        }

                    }
                }
                catch (Exception e)
                {

                    File.WriteAllText("err01.txt", e.ToString());
                }
                
            });
            mTask.Start();
        }
        private FreqBscanInfo FreqFix(FreqBscanInfo freq)
        {
            if (this.RealFBOrderInfo == null) return freq;
            if (RealFBOrderInfo.FreqStart != freq.FreqStart) return freq;
            if (RealFBOrderInfo.FreqStop != freq.FreqStop) return freq;
            if(RealFBOrderInfo.XX==null || RealFBOrderInfo.XX.Length==0) return freq;
            double[] yy = freq.FreqValues;
            if (yy == null || yy.Length == 0) return freq;
            double[] ry = new double[RealFBOrderInfo.XX.Length];
            double[] xx = RealFBOrderInfo.XX;
            double recordValue = 0;
            int recordIndex = 0;
            int stepCount =(int) Math.Ceiling(freq.FreqStep / (RealFBOrderInfo.FreqStep / 1000));

            Guid temp = Guid.NewGuid();
            int guidseed = BitConverter.ToInt32(temp.ToByteArray(), 0);
            Random r = new Random(guidseed);

            for (int i=0;i< ry.Length; i+= stepCount)
            {              
                if (recordIndex < yy.Length)
                {
                    recordValue = yy[recordIndex];                   
                    int index = (int)Math.Floor(Math.Abs(RealFBOrderInfo.XX[i] -freq.FreqStart) / (freq.FreqStep));
                    recordIndex = index;                                
                }
                ry[i] = recordValue;
                for (int j=1;j< stepCount; j++)
                {
                    if (i + j < ry.Length)
                    { 
                        ry[i + j] = recordValue+(double)r.Next(-200, 200) / 200;
                    }
                    else
                    {
                        break;
                    }                   
                }   
            }

            freq.FreqStep = RealFBOrderInfo.FreqStep / 1000;
            freq.FreqDataCount = RealFBOrderInfo.XX.Length;
            freq.Freqs = RealFBOrderInfo.XX;
            freq.FreqValues = ry;
            return freq;
        }
        public void StopWork()
        {
            mTask?.Cancel();
        }

    }
}
