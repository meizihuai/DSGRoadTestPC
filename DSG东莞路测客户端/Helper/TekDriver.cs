using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualBasic;
using System.Runtime.InteropServices;
using System.Threading;
static class TekDriver
{
   

    [DllImport(@"dll\RSA_API.dll", EntryPoint = "DEVICE_Run", CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr DEVICE_Run();


    [DllImport(@"dll\RSA_API.dll", EntryPoint = "DEVICE_Stop", CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr DEVICE_Stop();

    [DllImport(@"dll\RSA_API.dll", EntryPoint = "DEVICE_Reset", CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr DEVICE_Reset(int deviceID);

    [DllImport(@"dll\RSA_API.dll", EntryPoint = "AUDIO_SetFrequencyOffset", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr AUDIO_SetFrequencyOffset(double freqOffsetHz);

    [DllImport(@"dll\RSA_API.dll", EntryPoint = "AUDIO_SetMute", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr AUDIO_SetMute(bool mute);

    [DllImport(@"dll\RSA_API.dll", EntryPoint = "AUDIO_SetMode", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr AUDIO_SetMode(AudioDemodMode mode);

    [DllImport(@"dll\RSA_API.dll", EntryPoint = "AUDIO_SetVolume", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr AUDIO_SetVolume(float f);

    [DllImport(@"dll\RSA_API.dll", EntryPoint = "AUDIO_GetEnable", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr AUDIO_GetEnable(ref bool enable);

    [DllImport(@"dll\RSA_API.dll", EntryPoint = "AUDIO_Start", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr AUDIO_Start();

    [DllImport(@"dll\RSA_API.dll", EntryPoint = "AUDIO_Stop", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr AUDIO_Stop();

    [DllImport(@"dll\RSA_API.dll", EntryPoint = "AUDIO_GetData", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr AUDIO_GetData(IntPtr data, UInt16 inSize, ref UInt16 outSize);

   

    [DllImport(@"dll\RSA_API.dll", EntryPoint = "DEVICE_Connect", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr DEVICE_Connect(Int32 deviceID);

    [DllImport(@"dll\RSA_API.dll", EntryPoint = "DEVICE_Disconnect", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr DEVICE_Disconnect(Int32 deviceID);

    [DllImport(@"dll\RSA_API.dll", EntryPoint = "DEVICE_GetEnable", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr DEVICE_GetEnable(ref bool enable);

    [DllImport(@"dll\RSA_API.dll", EntryPoint = "CONFIG_SetCenterFreq", CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr CONFIG_SetCenterFreq(double refLevel);

    [DllImport(@"dll\RSA_API.dll", EntryPoint = "SPECTRUM_SetSettings", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr SPECTRUM_SetSettings(Spectrum_Settings setting);

    [DllImport(@"dll\RSA_API.dll", EntryPoint = "SPECTRUM_GetSettings", CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr SPECTRUM_GetSettings(IntPtr Setting);

    [DllImport(@"dll\RSA_API.dll", EntryPoint = "SPECTRUM_GetEnable", CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr SPECTRUM_GetEnable(ref bool enable);

    [DllImport(@"dll\RSA_API.dll", EntryPoint = "SPECTRUM_SetDefault", CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr SPECTRUM_SetDefault();

    [DllImport(@"dll\RSA_API.dll", EntryPoint = "SPECTRUM_SetEnable", CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr SPECTRUM_SetEnable(bool enable);

    [DllImport(@"dll\RSA_API.dll", EntryPoint = "SPECTRUM_AcquireTrace", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr SPECTRUM_AcquireTrace();

    [DllImport(@"dll\RSA_API.dll", EntryPoint = "SPECTRUM_WaitForTraceReady", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr SPECTRUM_WaitForTraceReady(int timeoutMsec, ref bool ready);

    [DllImport(@"dll\RSA_API.dll", EntryPoint = "SPECTRUM_GetTrace", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr SPECTRUM_GetTrace(int trace, int maxTracePoints, IntPtr traceData, ref int outTracePoints);

    [DllImport(@"dll\RSA_API.dll", EntryPoint = "DEVICE_PrepareForRun", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr DEVICE_PrepareForRun();

    // '获取错误string
    [DllImport(@"dll\RSA_API.dll", EntryPoint = "DEVICE_GetErrorString", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr GetErrorString(IntPtr status);

  

    [DllImport(@"dll\RSA_API.dll", EntryPoint = "DEVICE_Search", CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr DEVICE_Search(ref int numDevicesFound, int[] deviceIDs, StringBuilder deviceSerial, StringBuilder deviceType);


    [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
    public struct Spectrum_Settings
    {

        /// double
        public double span;

        /// double
        public double rbw;

        /// boolean
        [System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.I1)]
        public bool enableVBW;

        /// double
        public double vbw;

        /// int
        public int traceLength;

        /// SpectrumWindows->Anonymous_b441ab41_6b19_41df_b2bd_7f91ce85dd19
        public SpectrumWindows window;

        /// SpectrumVerticalUnits->Anonymous_eff7c62c_dfab_4465_897f_5899d935d934
        public SpectrumVerticalUnits verticalUnit;

        /// double
        public double actualStartFreq;

        /// double
        public double actualStopFreq;

        /// double
        public double actualFreqStepSize;

        /// double
        public double actualRBW;

        /// double
        public double actualVBW;

        /// int
        public int actualNumIQSamples;
    }

    public enum SpectrumWindows
    {

        /// SpectrumWindow_Kaiser -> 0
        SpectrumWindow_Kaiser = 0,

        /// SpectrumWindow_Mil6dB -> 1
        SpectrumWindow_Mil6dB = 1,

        /// SpectrumWindow_BlackmanHarris -> 2
        SpectrumWindow_BlackmanHarris = 2,

        /// SpectrumWindow_Rectangle -> 3
        SpectrumWindow_Rectangle = 3,

        /// SpectrumWindow_FlatTop -> 4
        SpectrumWindow_FlatTop = 4,

        /// SpectrumWindow_Hann -> 5
        SpectrumWindow_Hann = 5
    }

    public enum SpectrumVerticalUnits
    {

        /// SpectrumVerticalUnit_dBm -> 0
        SpectrumVerticalUnit_dBm = 0,

        /// SpectrumVerticalUnit_Watt -> 1
        SpectrumVerticalUnit_Watt = 1,

        /// SpectrumVerticalUnit_Volt -> 2
        SpectrumVerticalUnit_Volt = 2,

        /// SpectrumVerticalUnit_Amp -> 3
        SpectrumVerticalUnit_Amp = 3,

        /// SpectrumVerticalUnit_dBmV -> 4
        SpectrumVerticalUnit_dBmV = 4
    }
    public enum AudioDemodMode
    {

        /// ADM_FM_8KHZ -> 0
        ADM_FM_8KHZ = 0,

        /// ADM_FM_13KHZ -> 1
        ADM_FM_13KHZ = 1,

        /// ADM_FM_75KHZ -> 2
        ADM_FM_75KHZ = 2,

        /// ADM_FM_200KHZ -> 3
        ADM_FM_200KHZ = 3,

        /// ADM_AM_8KHZ -> 4
        ADM_AM_8KHZ = 4,
        ADM_NONE
    }

}
