using DSG东莞路测客户端;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace 电磁信息云服务CSharp
{
   
    class BMap: WebBrowser
    {
        private string url = "http://123.207.31.37:8082/baidumap.html";
        public string TZBQIco = "http://123.207.31.37:8082/bmapico/TZBQ.png" + "?t=" + DateTime.Now.Ticks;
        public string TSSIco = "http://123.207.31.37:8082/bmapico/TSS.png" + "?t=" + DateTime.Now.Ticks;
        public string disOnlineIco= "http://123.207.31.37:8082/bmapico/SH57_DisOnline.png" + "?t=" + DateTime.Now.Ticks;
        public string busIco= "http://123.207.31.37:8082/bmapico/bus.png" + "?t=" + DateTime.Now.Ticks;
        public delegate void dOnWebReady();
        public event dOnWebReady OnWebReady;
        public bool isReady = false;
        private Control mControl;
        public BMap(Control control)
        {
            url= url + "?t=" + DateTime.Now.Ticks;
            this.mControl = control;
            this.ObjectForScripting = control;
            this.Dock = DockStyle.Fill;
            this.DocumentCompleted += Web_DocumentCompleted;
            this.Navigate(url);
        }

        private void Web_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            isReady = true;
          //  SetCenter(Module.centerLng, Module.centerLat, Module.defaultMapSize);        
            OnWebReady?.Invoke();           
        }
        public void SetCenter(double lng,double lat,int size)
        {
            if (!isReady) return;
            setGisCenter3(lng.ToString(), lat.ToString(), size);
        }
        public void AddPoint(double lng, double lat, string txt, DeviceInfo info)
        {
            if (!isReady) return;
            if (lng <= 0 || lat <= 0) return;
            string ico = TSSIco;
            if (info.Kind == "TZBQ") ico = TZBQIco;
            if (!info.IsOnline) ico = disOnlineIco;
            AddPoint(lng, lat, txt, ico);
        }
        public void AddPoint(double lng,double lat,string txt,string imgUrl="")
        {
            if (!isReady) return;
            if (string.IsNullOrEmpty(imgUrl))
            {
                AddPoint(lng.ToString(), lat.ToString(), txt);
            }
            else
            {
                AddNewIco(lng.ToString(), lat.ToString(), txt, imgUrl);
            }        
        }
        public void AddJumpPoint(double lng, double lat, string txt)
        {
            if (!isReady) return;
            AddJumpPoint(lng.ToString(), lat.ToString(), txt);
        }
        public void ClearAll()
        {
            if (!isReady) return;
            CleanGis();
        }
        public void RemovePoint(string label)
        {
            if (!isReady) return;
            script("deletePoint", new string[] { label });
        }
        public void ShowWarnWindow(double lng,double lat,string txt,bool flag=true)
        {
            if (!isReady) return;
            script("cleanall", new string[] { });
            script("addpoint", new object[] { lng, lat, "", flag });
            script("showWindowMsg", new object[] { lng, lat, txt, flag });
            script("setcenter", new object[] { lng, lat });
        }
        public void InitFreqGisParam()
        {
            freqGisOldLng = 0;
            freqFisOldLat = 0;
            freqGisOldPointInfo = "";
        }
        private double freqGisOldLng, freqFisOldLat;
        private string freqGisOldPointInfo;
        public void AddFreqGisPoint(double lng,double lat,string showLabel,string lineName,string deviceId,bool isLockGisView,string colorName="blue")
        {
            if (!isReady) return;
            if (lng == 0 || lat == 0) return;
            if(freqGisOldLng==0 || freqFisOldLat == 0)
            {
                freqGisOldLng = lng;
                freqFisOldLat = lat;
                string jsNameTmp = "addFreqGisPoint";
                object[] objTmp = new object[]
                {
                    lng,lat,showLabel,true,isLockGisView,14,freqGisOldLng,freqFisOldLat,false,colorName,lineName,"",10,0.5
                };
                script(jsNameTmp, objTmp);
                freqGisOldPointInfo = showLabel;
                return;
            }
            string jsName = "addFreqGisPoint";
            object[] obj = new object[]
            {
                    lng,lat,showLabel,true,isLockGisView,14,freqGisOldLng,freqFisOldLat,true,colorName,lineName,"",10,0.5
            };
            script(jsName, obj);
            if (string.IsNullOrEmpty(freqGisOldPointInfo))
            {
                freqGisOldPointInfo = showLabel;
            }
            else
            {
                jsName = "deletePoint";
                obj = new object[] { freqGisOldPointInfo };
                script(jsName, obj);
                freqGisOldPointInfo = showLabel;
            }
            freqGisOldLng = lng;
            freqFisOldLat = lat;
        }
        #region"浏览器与js交互"
      
        delegate void wt_cleanGis();
        private void CleanGis()
        {
            wt_cleanGis d = new wt_cleanGis(th_CleanGis);
            this.mControl.Invoke(d, null);
        }
        private void th_CleanGis()
        {
            HtmlDocument doc = this.Document;
            if (doc==null)
                return;
            object[] ObjArr = new object[1];
            doc.InvokeScript("cleanall", ObjArr);
        }
        delegate void wt_script(string scriptName, object[] str);
        private void script(string scriptName, object[] str)
        {
            if (!isReady) return;
            wt_script d = new wt_script(th_script);
            object[] b = new object[2];
            b[0] = scriptName;
            b[1] = str;      
            this.mControl.Invoke(d, b);
        }
        private void th_script(string scriptName, object[] str)
        {
            try
            {
                HtmlDocument doc = this.Document;
                object[] O = new object[str.Count()];
                for (var i = 0; i <= str.Length - 1; i++)
                    O[i] = (object)str[i];
                doc.InvokeScript(scriptName, O);
                string kk = "0";
            }
            catch (Exception ex)
            {
                string err = ex.ToString();
                string kk = "0";
            }
        }
        delegate void wt_setGisCenter(string lng, string lat);
        private void setGisCenter(string lng, string lat)
        {
            wt_setGisCenter d = new wt_setGisCenter(th_setGisCenter);
            object[] b = new object[2];
            b[0] = lng;
            b[1] = lat;
          
            this.mControl.Invoke(d, b);
        }
        private void th_setGisCenter(string lng, string lat)
        {
            // 70410045
            // 421127199303212592
            // 梅子怀
            HtmlDocument doc = this.Document;
            if (doc==null)
                return;
            object[] ObjArr = new object[3];
            ObjArr[0] = (object)lng;
            ObjArr[1] = (object)lat;
            doc.InvokeScript("setcenter", ObjArr);
        }

        delegate void wt_setGisCenter3(string lng, string lat, int size);
        private void setGisCenter3(string lng, string lat, int size)
        {
            try
            {
                wt_setGisCenter3 d = new wt_setGisCenter3(th_setGisCenter3);
                object[] b = new object[3];
                b[0] = lng;
                b[1] = lat;
                b[2] = size;
                this.mControl.Invoke(d, b);
            }
            catch (Exception)
            {

            }    
          
        }
        private void th_setGisCenter3(string lng, string lat, int size)
        {
            // 70410045
            // 421127199303212592
            // 梅子怀
            HtmlDocument doc = this.Document;
            if (doc==null)
                return;
            object[] ObjArr = new object[3];
            ObjArr[0] = (object)lng;
            ObjArr[1] = (object)lat;
            ObjArr[2] = (object)size;
            doc.InvokeScript("setcenter3", ObjArr);
        }

        private void AddJumpPoint(string lng, string lat, string label)
        {
            wt_AddPoint d = new wt_AddPoint(th_AddJumpPoint);
            object[] b = new object[3];
            b[0] = lng;
            b[1] = lat;
            b[2] = label;
            this.mControl.Invoke(d, b);
        }
        private void th_AddJumpPoint(string lng, string lat, string label)
        {
            HtmlDocument doc = this.Document;
            object[] ObjArr = new object[3];
            ObjArr[0] = (object)lng;
            ObjArr[1] = (object)lat;
            ObjArr[2] = (object)label;
            doc.InvokeScript("addpoint", ObjArr);
        }
        delegate void wt_AddPoint(string lng, string lat, string label);
        private void AddPoint(string lng, string lat, string label)
        {
            wt_AddPoint d = new wt_AddPoint(th_AddPoint);
            object[] b = new object[3];
            b[0] = lng;
            b[1] = lat;
            b[2] = label;
    
            this.mControl.Invoke(d, b);
        }
        private void AddNewIco(string lng, string lat, string label, string icoUrl)
        {
            script("addNewIcoPoint", new string[] { lng, lat, label, icoUrl });
        }
        private void th_AddPoint(string lng, string lat, string label)
        {
            HtmlDocument doc = this.Document;
            if (doc==null)
                return;
            object[] ObjArr = new object[3];
            ObjArr[0] = (object)lng;
            ObjArr[1] = (object)lat;
            ObjArr[2] = (object)label;
            doc.InvokeScript("addBz", ObjArr);
        }
        #endregion
    }
}
