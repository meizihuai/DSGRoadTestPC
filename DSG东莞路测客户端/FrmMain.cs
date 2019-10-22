using DSG东莞路测客户端.Helper;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using 电磁信息云服务CSharp;
using static ConvertGPS;

namespace DSG东莞路测客户端
{
    public partial class FrmMain : Form
    {
        private readonly string title = "东莞路测客户端 V1.0.2.1";
        private MChart chart;
        private BMap map;
        private TekDevice tekDevice;
        private double mLon, mLat;
        private FreqGisFileWritter freqGisFileWritter;
        public bool flagHaveManualGps = false;
        public FrmMain()
        {
            InitializeComponent();
            Module.frmMain = this;
        }

        private void FrmMain_Load(object sender, EventArgs e)
        {
          
            this.Text = title;
            Control.CheckForIllegalCrossThreadCalls = false;
            linkLabel1.Click += (a, b) =>
            {
                new FrmLonLatInputBox((lon,lat)=>
                {
                    mLon = lon;
                    mLat = lat;
                    flagHaveManualGps = true;
                    lblGps.Text = $"{mLon},{mLat}  (手动)";

                    map.ClearAll();
                    map.SetCenter(lon, lat, 20);
                    map.AddPoint(lon, lat, "我的位置");

                }).Show();
            };
            Init();
        }
        private void Init()
        {
            txtIp.Text = "192.168.12.1";
            txtPort.Text = "3206";
            chart = new MChart();
            panel4.Controls.Add(chart);

            map = new BMap(this);
            panel3.Controls.Add(map);

            SearchDevice();

            btnStartFreq.Click += (a, b) => StartFreq();
            btnStopFreq.Click += (a, b) => StopFreq();
            btnResearch.Click += (a, b) => SearchDevice();
            lblGps.Text = "未开启获取Gps模块";
            btnGpsSettingConfrim.Click += (a, b) =>
            {
                string ip = txtIp.Text;
                int port = int.Parse(txtPort.Text);
                GateWayStatusHelper gateWayStatusHelper = new GateWayStatusHelper(ip, port, 4516);
               
                gateWayStatusHelper.OnGateWayStatusInfo += GateWayStatusHelper_OnGateWayStatusInfo;
                gateWayStatusHelper.StartWork();
                lblGps.Text = "已开启模块，等待GPS数据...";
            };
        }

        private void GateWayStatusHelper_OnGateWayStatusInfo(GateWayStatusHelper.GateWayStatusInfo gateWayStatusInfo)
        {
            if (gateWayStatusInfo == null) return;
            lblGps.Text = gateWayStatusInfo.lon + "," + gateWayStatusInfo.lat;
            PointLatLng point = new PointLatLng(gateWayStatusInfo.lat, gateWayStatusInfo.lon);
            point = ConvertGPS.Gps84_To_bd09(point);
            double lon = point.Lng;
            double lat = point.Lat;
            if (lon <=100 || lat <= 20) return;
            if (lon >= 200 || lat >= 40) return;
            mLon = lon;
            mLat = lat;
            map.SetCenter(lon, lat, 20);
            map.AddFreqGisPoint(lon, lat, "我的位置","Tek","01",true);          
        }

        public void Log(string str)
        {
            listBox1.Items.Add(str);
            listBox1.SelectedIndex = listBox1.Items.Count - 1;
            if (listBox1.Items.Count >= 1000) listBox1.Items.Clear();
        }
        private void SearchDevice()
        {
            Task.Run(async()=>
            {
                try
                {
                    lblDevice.Text = "等待设备连接";
                    Log("等待设备连接...");
                    TekDevice device = await TekDevice.SearchDevice();
                    if (device == null)
                    {
                        lblDevice.Text = "无设备连接";
                        Log("无设备连接!");
                        return;
                    }
                    string deviceInfo = $"{device.DeviceId} - {device.DeviceType}";
                    Log($"发现设备! {deviceInfo}");
                    lblDevice.Text = "正在连接:"+deviceInfo;
                    Log("重置设备...");
                    if (!TekApi.DeviceReset(device.DeviceId).result)
                    {
                        MessageBox.Show($"设备重置失败，请重新打开软件或拔插设备");
                        return;
                    }
                    Log("连接设备...");
                    if (!TekApi.DeviceConnect(device.DeviceId).result)
                    {
                        Log("连接失败");
                        lblDevice.Text = "连接失败";
                        MessageBox.Show($"设备连接失败，请重新打开软件或拔插设备");
                        return;
                    }                   
                    this.tekDevice = device;
                  
                    Log("设备已连接！");
                    lblDevice.Text = deviceInfo;
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.ToString());
                }                
            });
        }

      

        private void StartFreq()
        {
            if (this.tekDevice == null)
            {
                MessageBox.Show($"设备未连接");
                return;
            }
            double.TryParse(txtFreqStart.Text, out double freqStart);
            double.TryParse(txtFreqStop.Text, out double freqStop);
            double.TryParse(txtFreqStep.Text, out double freqStep);
            NormalResponse np= this.tekDevice.DoFreqBscan(freqStart, freqStop, freqStep);
            if (!np.result)
            {
                MessageBox.Show($"频谱命令下发失败,{np.msg}");
                return;
            }
            Log("频谱命令下发成功");
            tekDevice.OnNewFreq += TekDevice_OnNewFreq;
            lblOrder.Text = $"[{tekDevice.FBOrderInfo.FreqStart},{tekDevice.FBOrderInfo.FreqStop}] {tekDevice.FBOrderInfo.FreqStep} KHz";
        }
        private void StopFreq()
        {
            tekDevice?.StopWork();
        }

        private void TekDevice_OnNewFreq(FreqBscanInfo freq)
        {
            try
            {
                lblOrder.Text = $"[{freq.FreqStart},{freq.FreqStop}] {freq.FreqStep*1000} KHz";

                if (freqGisFileWritter == null)
                {
                    int.TryParse(txtId.Text, out int id);
                    freqGisFileWritter = new FreqGisFileWritter(id,freq.FreqStart, freq.FreqStop, freq.FreqStep);
                    txtFileName.Text = freqGisFileWritter.FileName;
                    txtPath.Text = freqGisFileWritter.WritePath;
                    freqGisFileWritter.OnNewFile += (fileName, filePath) =>
                    {
                        txtFileName.Text = fileName;
                        txtPath.Text = filePath;
                        FileInfo fileInfo = new FileInfo(Path.Combine(filePath, fileName));
                        if (fileInfo.Exists)
                            txtLength.Text = GetLength(fileInfo.Length);
                        else
                            txtLength.Text = "0";
                    };
                    freqGisFileWritter.OnFileLengthChange += (length) =>
                    {
                        Module.RunUi(this, () =>
                        {
                            txtLength.Text = GetLength(length);
                        });
                    };
                }
                freqGisFileWritter.WriteFreq(freq.Copy(), mLon, mLat);
                
                freq.DoReduce();
                Module.RunUi(this, () => chart.ShowFreq(freq));
            }
            catch (Exception e)
            {
                File.WriteAllText("error.txt", e.ToString());
            }
                   
        }
        private string GetLength(long len)
        {
            if (len < 1024) return len.ToString();
            if (len < 1024 * 1024) return ((double)len / 1024).ToString("0.00") + "KB";
            if (len < 1024 * 1024*1024) return ((double)len / (1024 * 1024)).ToString("0.00") + "MB";
            return (len / (1024 * 1024*1024)).ToString("0.0") + "GB";
        }

    }
}
