using DSG东莞路测客户端;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace 电磁信息云服务CSharp
{
    class MChart:Chart
    {
        private double SignalTimlyFreq { get; set; }
        private int timlyFreqMaxPoint = 2000;
        private int timlyCurrentPointCount = 0;
        public MChart()
        {
            this.Dock = DockStyle.Fill;
            Init();
        }
        private void Init()
        {
            ChartArea area = new ChartArea();
            area.InnerPlotPosition = new ElementPosition(4, 4, 96, 86);
            area.Position = new ElementPosition(0, 0, 98, 100);
            area.AxisY.Maximum = -20;
            area.AxisY.Minimum = -120;       

            area.AxisX.Interval =20;
            area.AxisX.LabelStyle.IsStaggered = false;

            area.AxisX.LineColor = Color.Gray;
            area.AxisY.LineColor = Color.Gray;

            area.AxisX.InterlacedColor= Color.Gray;
            area.AxisY.InterlacedColor = Color.Gray;

            area.AxisX.MajorGrid.LineColor= Color.Gray;
            area.AxisY.MajorGrid.LineColor = Color.Gray;

            area.BorderColor= Color.Gray;
            this.ChartAreas.Add(area);
            InitSeries();
        }
        private void InitSeries()
        {
            Series.Clear();
            //0
            Series seriesFreq = new Series("频谱");
            seriesFreq.XValueType = ChartValueType.Auto;
            seriesFreq.ChartType = SeriesChartType.FastLine;
            seriesFreq.IsVisibleInLegend = false;
            seriesFreq.Color = Color.Blue;
            seriesFreq.Name = "频谱";
            seriesFreq.LabelToolTip = "频率：#VALX 场强：#VAL";
            seriesFreq.ToolTip = "频率：#VALX 场强：#VAL";
            Series.Add(seriesFreq);

            //1
            Series seriesPoint = new Series("离散");
            seriesPoint.XValueType = ChartValueType.Auto;
            seriesPoint.ChartType = SeriesChartType.Column;
            seriesPoint.IsVisibleInLegend = false;
            seriesPoint.Color = Color.Blue;
            seriesPoint.Name = "离散";
            seriesPoint.LabelToolTip = "频率：#VALX 场强：#VAL";
            seriesPoint.ToolTip = "频率：#VALX 场强：#VAL";
            Series.Add(seriesPoint);

            //2
            Series seriesWhite = new Series("离散白名单");
            seriesWhite.XValueType = ChartValueType.Auto;
            seriesWhite.ChartType = SeriesChartType.Column;
            seriesWhite.IsVisibleInLegend = false;
            seriesWhite.Color = Color.Green;
            seriesWhite.Name = "离散白名单";
            seriesWhite.LabelToolTip = "频率：#VALX 场强：#VAL";
            seriesWhite.ToolTip = "频率：#VALX 场强：#VAL";
            Series.Add(seriesWhite);

            //3
            Series seriesMaxFreq = new Series("最大值频谱");
            seriesMaxFreq.XValueType = ChartValueType.Auto;
            seriesMaxFreq.ChartType = SeriesChartType.FastLine;
            seriesMaxFreq.IsVisibleInLegend = false;
            seriesMaxFreq.Color = Color.FromArgb(236, 170, 0);
            seriesMaxFreq.Name = "最大值频谱";
            seriesMaxFreq.LabelToolTip = "频率：#VALX 场强：#VAL";
            seriesMaxFreq.ToolTip = "频率：#VALX 场强：#VAL";
            Series.Add(seriesMaxFreq);

            //4
            Series seriesDifferUpFreq = new Series("差分频谱(上 超出)");
            seriesDifferUpFreq.XValueType = ChartValueType.Auto;
            seriesDifferUpFreq.ChartType = SeriesChartType.FastLine;
            seriesDifferUpFreq.IsVisibleInLegend = false;
            seriesDifferUpFreq.Color = Color.Black;
            seriesDifferUpFreq.Name = "差分频谱(上 超出)";
            seriesDifferUpFreq.LabelToolTip = "频率：#VALX 场强：#VAL";
            seriesDifferUpFreq.ToolTip = "频率：#VALX 场强：#VAL";
            Series.Add(seriesDifferUpFreq);

            //5
            Series seriesDifferDownFreq = new Series("差分频谱(下 减少)");
            seriesDifferDownFreq.XValueType = ChartValueType.Auto;
            seriesDifferDownFreq.ChartType = SeriesChartType.FastLine;
            seriesDifferDownFreq.IsVisibleInLegend = false;
            seriesDifferDownFreq.Color = Color.YellowGreen;
            seriesDifferDownFreq.Name = "差分频谱(下 减少)";
            seriesDifferDownFreq.LabelToolTip = "频率：#VALX 场强：#VAL";
            seriesDifferDownFreq.ToolTip = "频率：#VALX 场强：#VAL";
            Series.Add(seriesDifferDownFreq);

            //6
            Series seriesUnknow = new Series("离散未知信号");
            seriesUnknow.XValueType = ChartValueType.Auto;
            seriesUnknow.ChartType = SeriesChartType.Column;
            seriesUnknow.IsVisibleInLegend = false;
            seriesUnknow.Color = Color.Green;
            seriesUnknow.Name = "离散未知信号";
            seriesUnknow.LabelToolTip = "频率：#VALX 场强：#VAL";
            seriesUnknow.ToolTip = "频率：#VALX 场强：#VAL";
            Series.Add(seriesUnknow);

            //7
            Series seriesSignalTiming = new Series("信号强度时序图");
            seriesSignalTiming.XValueType = ChartValueType.Auto;
            seriesSignalTiming.ChartType = SeriesChartType.FastLine;
            seriesSignalTiming.IsVisibleInLegend = false;
            seriesSignalTiming.Color = Color.Blue;
            seriesSignalTiming.Name = "信号强度时序图";
            seriesSignalTiming.LabelToolTip = "频率：#VALX 场强：#VAL";
            seriesSignalTiming.ToolTip = "频率：#VALX 场强：#VAL";
            Series.Add(seriesSignalTiming);

            //8
            Series seriesSignalMaxThreshold = new Series("信号强度最大门限值");
            seriesSignalMaxThreshold.XValueType = ChartValueType.Auto;
            seriesSignalMaxThreshold.ChartType = SeriesChartType.FastLine;
            seriesSignalMaxThreshold.IsVisibleInLegend = false;
            seriesSignalMaxThreshold.Color = Color.Red;
            seriesSignalMaxThreshold.Name = "信号强度最大门限值";
            seriesSignalMaxThreshold.LabelToolTip = "频率：#VALX 场强：#VAL";
            seriesSignalMaxThreshold.ToolTip = "频率：#VALX 场强：#VAL";
            Series.Add(seriesSignalMaxThreshold);

            //9
            Series seriesSignalMinThreshold = new Series("信号强度最小门限值");
            seriesSignalMinThreshold.XValueType = ChartValueType.Auto;
            seriesSignalMinThreshold.ChartType = SeriesChartType.FastLine;
            seriesSignalMinThreshold.IsVisibleInLegend = false;
            seriesSignalMinThreshold.Color = Color.YellowGreen;
            seriesSignalMinThreshold.Name = "信号强度最小门限值";
            seriesSignalMinThreshold.LabelToolTip = "频率：#VALX 场强：#VAL";
            seriesSignalMinThreshold.ToolTip = "频率：#VALX 场强：#VAL";
            Series.Add(seriesSignalMinThreshold);

        }
        public void ClearPointsBySeriesIndex(int index)
        {
            if (index >= this.Series.Count) return;
            this.Series[index].Points.Clear();
        }
        public void ShowFreq(FreqBscanInfo freq)
        {
            double[] freqValue = freq.FreqValues;
            double[] xx = freq.Freqs;
            Series seriesFreq = this.Series[0];
            seriesFreq.Points.Clear();
            for (int i = 0; i < freqValue.Length; i++)
            {
                seriesFreq.Points.AddXY(xx[i], freqValue[i]);
            }
            this.ChartAreas[0].AxisX.Minimum = xx[0];
            this.ChartAreas[0].AxisX.Maximum = xx[xx.Length - 1];
            this.ChartAreas[0].AxisX.Interval = (freq.FreqStop - freq.FreqStart) / 5;
        }
        public void ShowMaxFreq(FreqBscanInfo freq)
        {

            double[] freqValue = freq.FreqValues;
            double[] xx = freq.Freqs;
            Series seriesFreq = this.Series[3];
            seriesFreq.Points.Clear();
            for (int i = 0; i < freqValue.Length; i++)
            {              
                seriesFreq.Points.AddXY(xx[i], freqValue[i]);
            }
            this.ChartAreas[0].AxisX.Minimum = xx[0];
            this.ChartAreas[0].AxisX.Maximum = xx[xx.Length - 1];
            this.ChartAreas[0].AxisX.Interval = (freq.FreqStop - freq.FreqStart) / 5;
        }
        public void ShowDifferFreq(FreqBscanInfo freq)
        {
            this.ChartAreas[0].AxisY.Minimum = -120;
            this.ChartAreas[0].AxisY.Maximum = 40;
            this.ChartAreas[0].AxisY.Interval = 20;
            double[] freqValue = freq.FreqValues;
            double[] xx = freq.Freqs;
            Series sup = this.Series[4];
            Series sdown = this.Series[5];
            sup.Points.Clear();
            sdown.Points.Clear();
            for (int i = 0; i < freqValue.Length; i++)
            {
                double x = xx[i];
                double y = freqValue[i];
                sup.Points.AddXY(x, 0);
                sdown.Points.AddXY(x, 0);
                if (y > 0)
                {
                    sup.Points.AddXY(x, y);
                }
                else
                {
                    sdown.Points.AddXY(x, y);
                }
            }

            this.ChartAreas[0].AxisX.Minimum = xx[0];
            this.ChartAreas[0].AxisX.Maximum = xx[xx.Length - 1];
            this.ChartAreas[0].AxisX.Interval = (freq.FreqStop - freq.FreqStart) / 5;
        }
        public void ShowPoint(DiscreteScanInfo info)
        {
            this.ChartAreas[0].AxisX.Minimum = Math.Ceiling(info.FreqStart -3);
            this.ChartAreas[0].AxisX.Maximum = Math.Ceiling(info.FreqStop + 3);
            double yu = (this.ChartAreas[0].AxisX.Maximum - this.ChartAreas[0].AxisX.Minimum) % 5;
            this.ChartAreas[0].AxisX.Maximum+= yu;
            this.ChartAreas[0].AxisX.Interval = (this.ChartAreas[0].AxisX.Maximum - this.ChartAreas[0].AxisX.Minimum) / 5;
            this.Series[1].Points.Clear();
            
            Series seriesFreq = this.Series[1];
            seriesFreq.Color = Color.Blue;
            double step = 0.1;
            double minY = this.ChartAreas[0].AxisY.Minimum;
            for (int i = 0; i < info.PointCount; i++)
            {
                seriesFreq.Points.AddXY(info.FreqPoints[i] - step, minY);
                seriesFreq.Points.AddXY(info.FreqPoints[i], info.Values[i]);
                seriesFreq.Points.AddXY(info.FreqPoints[i] + step, minY);
            }
            if (this.Series[0].Points.Count > 0) return;
            this.Series[0].Points.Add(info.FreqPoints[0], info.Values[0]);
        }
        public void ShowPoint(List<SignalDbSaveInfo> list)
        {
            if (list == null || list.Count == 0) return;
            double freqStart = list[0].Freq;
            double freqEnd = list[0].Freq;
            foreach(var itm in list)
            {
                if (freqStart > itm.Freq)
                {
                    freqStart = itm.Freq;
                }
                if (freqEnd < itm.Freq)
                {
                    freqEnd = itm.Freq;
                }
            }

            this.ChartAreas[0].AxisX.Minimum = Math.Ceiling(freqStart - 3);
            this.ChartAreas[0].AxisX.Maximum = Math.Ceiling(freqEnd + 3);
            double yu = (this.ChartAreas[0].AxisX.Maximum - this.ChartAreas[0].AxisX.Minimum) % 5;
            this.ChartAreas[0].AxisX.Maximum += yu;
            this.ChartAreas[0].AxisX.Interval = (this.ChartAreas[0].AxisX.Maximum - this.ChartAreas[0].AxisX.Minimum) / 5;
            this.Series[1].Points.Clear();

            Series seriesFreq = this.Series[1];
            seriesFreq.Color = Color.Blue;
            double step = 0.1;
            double minY = this.ChartAreas[0].AxisY.Minimum;
            for (int i = 0; i < list.Count; i++)
            {
                seriesFreq.Points.AddXY(list[i].Freq - step, minY);
                seriesFreq.Points.AddXY(list[i].Freq, list[i].Value);
                seriesFreq.Points.AddXY(list[i].Freq + step, minY);
            }
            if (this.Series[0].Points.Count > 0) return;
            this.Series[0].Points.Add(list[0].Freq, list[0].Value);
        }
        public void ShowSigNals(List<SignalInfo> list)
        {
            Series seriesSignal = this.Series[1];
            seriesSignal.Points.Clear();
            seriesSignal.Color = Color.Red;
            Series seriesWhite = this.Series[2];
            seriesWhite.Points.Clear();
            seriesWhite.Color = Color.Blue;
            Series seriesUnKnow = this.Series[6];
            seriesUnKnow.Points.Clear();
            seriesUnKnow.Color = Color.Green;
            list.ForEach(a =>
            {
               for(int i = 0; i < a.SigNalChartPointX.Length; i++)
                {
                    double x = a.SigNalChartPointX[i];
                    double y = a.SigNalChartPointY[i];
                    if (a.IsWhite)
                    {
                        seriesWhite.Points.AddXY(x, y);
                    }
                    else
                    {
                        if (a.SignalLevel == 1)
                        {
                            seriesWhite.Points.AddXY(x, y);
                        }
                        if (a.SignalLevel == 0) 
                        {
                            seriesUnKnow.Points.AddXY(x, y);
                        }
                        if (a.SignalLevel == 2)
                        {
                            seriesSignal.Points.AddXY(x, y);
                        }
                    }                    
                }
            });
        }
        public void AddPOAPoint(string deviceNickName)
        {
            double defaultValue = this.ChartAreas[0].AxisY.Minimum;
            Series[1].Points.AddXY(deviceNickName, defaultValue);
            Series[0].Points.Clear();
            Series[0].Points.Add(defaultValue);
        }
        public void SetPOACValue(int index,double value,int maxestIndex)
        {
            if (Series[1].Points.Count <= index) return;
            Series[1].Points[index].YValues[0] = value;
            Series[1].Points[maxestIndex].Color = Color.Red;
            for (int i = 0; i < Series[1].Points.Count; i++)
            {
                if (i != maxestIndex)
                {
                    Series[1].Points[i].Color = Color.Blue;
                }
            }
        }
        public void CleanAll()
        {
            foreach(Series s in this.Series)
            {
                s.Points.Clear();
            }
        }
       
        public void InitSignalTimly(double freq)
        {
            SignalTimlyFreq = freq;
            this.Series[7].Points.Clear();
            double min = this.ChartAreas[0].AxisY.Minimum;
            timlyCurrentPointCount = 0;
            for (int i = 0; i < timlyFreqMaxPoint; i++)
            {
                this.Series[7].Points.Add(min);
            }
        }
        public void AddSignalTimlyPoint(double value)
        {          
            Series series = this.Series[7];
            if(timlyCurrentPointCount < this.timlyFreqMaxPoint)
            {
                series.Points[timlyCurrentPointCount].SetValueY(value);
                double lastValue = series.Points[series.Points.Count - 1].YValues[0];
                series.Points.RemoveAt(series.Points.Count - 1);
                series.Points.Add(lastValue);            
            }
            else
            {
                if (series.Points.Count >= this.timlyFreqMaxPoint)
                {
                    series.Points.RemoveAt(0);
                }
                series.Points.Add(value);
               // Console.WriteLine($"series.Points.Count={series.Points.Count}  timlyFreqMaxPoint={timlyFreqMaxPoint}");
               
            }
            timlyCurrentPointCount++;
            //Console.WriteLine($"timlyCurrentPointCount={timlyCurrentPointCount}");
        }
        public void SetSignalThreshold(double maxValue,double minValue)
        {
            this.Series[8].Points.Clear();
            this.Series[9].Points.Clear();
            for(int i = 0; i < this.timlyFreqMaxPoint; i++)
            {
                this.Series[8].Points.Add(maxValue);
                this.Series[9].Points.Add(minValue);
            }
        }
    }
}
