using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.IO;

namespace DSG东莞路测客户端.Helper
{
    public class GateWayStatusHelper
    {
        public int localPort, remotePort;
        public string remoteIp;
        private IPEndPoint remoteIpEndPoint, localIpEndPoint;
        private Socket socket;
        private Thread udpServerThread;
        private Thread udpClientThread;
        private int sleepTimes;
        private UdpClient udpClient;
        private readonly TssMsgHelper tmg = new TssMsgHelper();
        public delegate void DlgOnGateWayStatusInfo(GateWayStatusInfo gateWayStatusInfo);
        public event DlgOnGateWayStatusInfo OnGateWayStatusInfo;
        public class GateWayStatusInfo
        {
            public string net;
            public string power;
            public double lon;
            public double lat;
            public double voltage;
        }
        public GateWayStatusHelper(string remoteIp, int remotePort, int localPort = 4516)
        {
            this.localPort = localPort;
            this.remoteIp = remoteIp;
            this.remotePort = remotePort;
            remoteIpEndPoint = new IPEndPoint(IPAddress.Parse(remoteIp), remotePort);
            localIpEndPoint = new IPEndPoint(IPAddress.Any, localPort);
        }      

        public void StartWork(int sleepSecond = 5)
        {
            StopWork();
            sleepTimes = sleepSecond * 1000;
            udpClient = new UdpClient(localPort);
            udpServerThread = new Thread(StartUdpServer);
            udpServerThread.IsBackground = true;
            udpServerThread.Start();
            udpClientThread = new Thread(StartUdpRecive);
            udpClientThread.IsBackground = true;
            udpClientThread.Start();

            //Task.Run(async()=>
            //{
            //    double lon = 113.456789;
            //    double lat = 23.456789;
            //    while (true)
            //    {
            //        GateWayStatusInfo info = new GateWayStatusInfo();
            //        lon += 0.0001;
            //        lat += 0.0001;
            //        info.lon = lon;
            //        info.lat = lat;
            //        OnGateWayStatusInfo(info);
            //        await Task.Delay(1000);
            //    }
            //});          
        }
        public void StopWork()
        {
            if (udpClient != null) udpClient.Close();
            try
            {

                if (udpClientThread != null)
                {
                    udpClientThread.Abort();
                }
            }
            catch (Exception e)
            {

            }
            try
            {
                if (udpServerThread != null)
                {
                    udpServerThread.Abort();
                }
            }
            catch (Exception e)
            {

            }
        }
        private void StartUdpServer()
        {
            while (true)
            {
                try
                {
                    if (udpClient == null)
                    {
                        Thread.Sleep(sleepTimes);
                        udpClient = new UdpClient(localPort);
                        continue;
                    }
                    TssMsg tm = tmg.Msg2TssMsg(0, "task", "getheartbeat", "", null);
                    byte[] buffer = tmg.Tssmsg2byte(tm);
                    if (buffer == null)
                    {
                       //  Module.Log("<Send> buffer is null");
                    }
                   
                    udpClient.Send(buffer, buffer.Length, remoteIpEndPoint);
                   // File.WriteAllBytes("test.bin", buffer);
                   // Log("<Send>" + buffer.Length + " bytes");
                }
                catch (Exception e)
                {
                    //  Log("<Send Err>" + e.ToString());
                }
                Thread.Sleep(sleepTimes);
            }
        }
        private void StartUdpRecive()
        {
            while (true)
            {
                try
                {
                    if (udpClient == null)
                    {
                        Thread.Sleep(sleepTimes);
                        continue;
                    }
                    IPEndPoint rp = new IPEndPoint(IPAddress.Any, 0);
                    byte[] buffer = null;
                    buffer = udpClient.Receive(ref rp);
                   // Module.Log($"<Rev> buffer.length={buffer.Length}");
                    string thisRemoteIp = rp.Address.ToString();
                    if (thisRemoteIp == remoteIp)
                    {
                        TssMsg tm = tmg.Byte2tssmsg(buffer);
                        //Module.Log(tm.datatype + "," + tm.functype + "," + tm.canshuqu);
                        if (tm.datatype == "Data" && tm.functype == "heartbeat")
                        {
                            string msg = tm.canshuqu;
                            string str = msg.Replace("<", "").Replace(">", "");
                            string headFunc = str.Split(':')[0];
                         
                            if (headFunc == "heartbeat")
                            {
                                //  OnUdpHeartBeat(tm.canshuqu);
                                string[] paras = str.Split(';');
                                GateWayStatusInfo gateWayStatusInfo = new GateWayStatusInfo();
                                foreach (string kv in paras)
                                {
                                    if (kv.Contains("="))
                                    {
                                        string key = kv.Split('=')[0];
                                        string value = kv.Split('=')[1];
                                        if (key == "swnet") gateWayStatusInfo.net = value;
                                        if (key == "swpow") gateWayStatusInfo.power = value;
                                        if (key == "longitude") gateWayStatusInfo.lon = double.Parse(value);
                                        if (key == "latitude") gateWayStatusInfo.lat = double.Parse(value);
                                        if (key == "voltage")
                                        {
                                            // OnUdpVoltageInfo(value);
                                            gateWayStatusInfo.voltage = double.Parse(value);
                                        }
                                    }
                                }
                                OnGateWayStatusInfo(gateWayStatusInfo);
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                  
                }
            }

        }



    }
}
