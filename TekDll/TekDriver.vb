Imports System
Imports System.Runtime.InteropServices
Imports System.Text
Imports System.Threading
Imports System.Threading.Thread
Imports System.IO
Public Class TekDriver
    Public Shared Function Code2Msg(ByVal code As IntPtr) As String
        Dim int As IntPtr = GetErrorString(code)
        Dim result As String = Marshal.PtrToStringAnsi(int)
        Return result
    End Function
    <DllImport("dll\RSA_API.dll", EntryPoint:="DEVICE_Run", CharSet:=CharSet.Ansi, ExactSpelling:=False, CallingConvention:=CallingConvention.Cdecl)>
    Public Shared Function DEVICE_Run() As IntPtr

    End Function
    <DllImport("dll\RSA_API.dll", EntryPoint:="DEVICE_Stop", CharSet:=CharSet.Ansi, ExactSpelling:=False, CallingConvention:=CallingConvention.Cdecl)>
    Public Shared Function DEVICE_Stop() As IntPtr

    End Function
    <DllImport("dll\RSA_API.dll", EntryPoint:="DEVICE_Reset", CharSet:=CharSet.Ansi, ExactSpelling:=False, CallingConvention:=CallingConvention.Cdecl)>
    Public Shared Function DEVICE_Reset(ByVal deviceID As Int32) As IntPtr

    End Function
    <DllImport("dll\RSA_API.dll", EntryPoint:="AUDIO_SetFrequencyOffset", CharSet:=CharSet.Ansi, CallingConvention:=CallingConvention.Cdecl)>
    Public Shared Function AUDIO_SetFrequencyOffset(ByVal freqOffsetHz As Double) As IntPtr

    End Function
    <DllImport("dll\RSA_API.dll", EntryPoint:="AUDIO_SetMute", CharSet:=CharSet.Ansi, CallingConvention:=CallingConvention.Cdecl)>
    Public Shared Function AUDIO_SetMute(ByVal mute As Boolean) As IntPtr

    End Function
    <DllImport("dll\RSA_API.dll", EntryPoint:="AUDIO_SetMode", CharSet:=CharSet.Ansi, CallingConvention:=CallingConvention.Cdecl)>
    Public Shared Function AUDIO_SetMode(ByVal mode As AudioDemodMode) As IntPtr

    End Function
    <DllImport("dll\RSA_API.dll", EntryPoint:="AUDIO_SetVolume", CharSet:=CharSet.Ansi, CallingConvention:=CallingConvention.Cdecl)>
    Public Shared Function AUDIO_SetVolume(ByVal f As Single) As IntPtr

    End Function
    <DllImport("dll\RSA_API.dll", EntryPoint:="AUDIO_GetEnable", CharSet:=CharSet.Ansi, CallingConvention:=CallingConvention.Cdecl)>
    Public Shared Function AUDIO_GetEnable(ByRef enable As Boolean) As IntPtr

    End Function
    <DllImport("dll\RSA_API.dll", EntryPoint:="AUDIO_Start", CharSet:=CharSet.Ansi, CallingConvention:=CallingConvention.Cdecl)>
    Public Shared Function AUDIO_Start() As IntPtr

    End Function
    <DllImport("dll\RSA_API.dll", EntryPoint:="AUDIO_Stop", CharSet:=CharSet.Ansi, CallingConvention:=CallingConvention.Cdecl)>
    Public Shared Function AUDIO_Stop() As IntPtr

    End Function
    <DllImport("dll\RSA_API.dll", EntryPoint:="AUDIO_GetData", CharSet:=CharSet.Ansi, CallingConvention:=CallingConvention.Cdecl)>
    Public Shared Function AUDIO_GetData(ByVal data As IntPtr, ByVal inSize As UInt16, ByRef outSize As UInt16) As IntPtr

    End Function
    Public Enum AudioDemodMode

        '''ADM_FM_8KHZ -> 0
        ADM_FM_8KHZ = 0

        '''ADM_FM_13KHZ -> 1
        ADM_FM_13KHZ = 1

        '''ADM_FM_75KHZ -> 2
        ADM_FM_75KHZ = 2

        '''ADM_FM_200KHZ -> 3
        ADM_FM_200KHZ = 3

        '''ADM_AM_8KHZ -> 4
        ADM_AM_8KHZ = 4

        ADM_NONE
    End Enum

    <DllImport("dll\RSA_API.dll", EntryPoint:="DEVICE_Connect", CharSet:=CharSet.Ansi, CallingConvention:=CallingConvention.Cdecl)>
    Public Shared Function DEVICE_Connect(ByVal deviceID As Int32) As IntPtr

    End Function
    <DllImport("dll\RSA_API.dll", EntryPoint:="DEVICE_Disconnect", CharSet:=CharSet.Ansi, CallingConvention:=CallingConvention.Cdecl)>
    Public Shared Function DEVICE_Disconnect(ByVal deviceID As Int32) As IntPtr

    End Function
    <DllImport("dll\RSA_API.dll", EntryPoint:="DEVICE_GetEnable", CharSet:=CharSet.Ansi, CallingConvention:=CallingConvention.Cdecl)>
    Public Shared Function DEVICE_GetEnable(ByRef enable As Boolean) As IntPtr

    End Function
    <DllImport("dll\RSA_API.dll", EntryPoint:="CONFIG_SetCenterFreq", CharSet:=CharSet.Ansi, ExactSpelling:=False, CallingConvention:=CallingConvention.Cdecl)>
    Public Shared Function CONFIG_SetCenterFreq(ByVal refLevel As Double) As IntPtr

    End Function
    <DllImport("dll\RSA_API.dll", EntryPoint:="SPECTRUM_SetSettings", CharSet:=CharSet.Ansi, CallingConvention:=CallingConvention.Cdecl)>
    Public Shared Function SPECTRUM_SetSettings(ByVal setting As Spectrum_Settings) As IntPtr

    End Function
    <DllImport("dll\RSA_API.dll", EntryPoint:="SPECTRUM_GetSettings", CharSet:=CharSet.Ansi, ExactSpelling:=False, CallingConvention:=CallingConvention.Cdecl)>
    Public Shared Function SPECTRUM_GetSettings(ByVal Setting As IntPtr) As IntPtr

    End Function
    <DllImport("dll\RSA_API.dll", EntryPoint:="SPECTRUM_GetEnable", CharSet:=CharSet.Ansi, ExactSpelling:=False, CallingConvention:=CallingConvention.Cdecl)>
    Public Shared Function SPECTRUM_GetEnable(ByRef enable As Boolean) As IntPtr

    End Function
    <DllImport("dll\RSA_API.dll", EntryPoint:="SPECTRUM_SetDefault", CharSet:=CharSet.Ansi, ExactSpelling:=False, CallingConvention:=CallingConvention.Cdecl)>
    Public Shared Function SPECTRUM_SetDefault() As IntPtr

    End Function
    <DllImport("dll\RSA_API.dll", EntryPoint:="SPECTRUM_SetEnable", CharSet:=CharSet.Ansi, ExactSpelling:=False, CallingConvention:=CallingConvention.Cdecl)>
    Public Shared Function SPECTRUM_SetEnable(ByVal enable As Boolean) As IntPtr

    End Function
    <DllImport("dll\RSA_API.dll", EntryPoint:="SPECTRUM_AcquireTrace", CharSet:=CharSet.Ansi, CallingConvention:=CallingConvention.Cdecl)>
    Public Shared Function SPECTRUM_AcquireTrace() As IntPtr

    End Function
    <DllImport("dll\RSA_API.dll", EntryPoint:="SPECTRUM_WaitForTraceReady", CharSet:=CharSet.Ansi, CallingConvention:=CallingConvention.Cdecl)>
    Public Shared Function SPECTRUM_WaitForTraceReady(ByVal timeoutMsec As Integer, ByRef ready As Boolean) As IntPtr

    End Function
    <DllImport("dll\RSA_API.dll", EntryPoint:="SPECTRUM_GetTrace", CharSet:=CharSet.Ansi, CallingConvention:=CallingConvention.Cdecl)>
    Public Shared Function SPECTRUM_GetTrace(ByVal trace As Int32, ByVal maxTracePoints As Integer, ByVal traceData As IntPtr, ByRef outTracePoints As Integer) As IntPtr

    End Function
    <DllImport("dll\RSA_API.dll", EntryPoint:="DEVICE_PrepareForRun", CharSet:=CharSet.Ansi, CallingConvention:=CallingConvention.Cdecl)>
    Public Shared Function DEVICE_PrepareForRun() As IntPtr

    End Function
    ''获取错误string
    <DllImport("dll\RSA_API.dll", EntryPoint:="GetErrorString", CharSet:=CharSet.Ansi, CallingConvention:=CallingConvention.Cdecl)>
    Public Shared Function GetErrorString(ByVal status As IntPtr) As IntPtr

    End Function
    <DllImport("dll\RSA_API.dll", EntryPoint:="DEVICE_SearchInt", CharSet:=CharSet.Ansi, ExactSpelling:=False, CallingConvention:=CallingConvention.Cdecl)>
    Public Shared Function DEVICE_SearchInt(ByRef numDevicesFound As IntPtr, ByRef deviceIDs() As IntPtr, ByRef deviceSerial() As IntPtr, ByRef deviceType() As IntPtr) As IntPtr

    End Function

    <System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)>
    Public Structure Spectrum_Settings

        '''double
        Public Shared span As Double

        '''double
        Public Shared rbw As Double

        '''boolean
        <System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.I1)>
        Public Shared enableVBW As Boolean

        '''double
        Public Shared vbw As Double

        '''int
        Public Shared traceLength As Integer

        '''SpectrumWindows->Anonymous_b441ab41_6b19_41df_b2bd_7f91ce85dd19
        Public Shared window As SpectrumWindows

        '''SpectrumVerticalUnits->Anonymous_eff7c62c_dfab_4465_897f_5899d935d934
        Public Shared verticalUnit As SpectrumVerticalUnits

        '''double
        Public Shared actualStartFreq As Double

        '''double
        Public Shared actualStopFreq As Double

        '''double
        Public Shared actualFreqStepSize As Double

        '''double
        Public Shared actualRBW As Double

        '''double
        Public Shared actualVBW As Double

        '''int
        Public Shared actualNumIQSamples As Integer
    End Structure

    Public Enum SpectrumWindows

        '''SpectrumWindow_Kaiser -> 0
        SpectrumWindow_Kaiser = 0

        '''SpectrumWindow_Mil6dB -> 1
        SpectrumWindow_Mil6dB = 1

        '''SpectrumWindow_BlackmanHarris -> 2
        SpectrumWindow_BlackmanHarris = 2

        '''SpectrumWindow_Rectangle -> 3
        SpectrumWindow_Rectangle = 3

        '''SpectrumWindow_FlatTop -> 4
        SpectrumWindow_FlatTop = 4

        '''SpectrumWindow_Hann -> 5
        SpectrumWindow_Hann = 5
    End Enum

    Public Enum SpectrumVerticalUnits

        '''SpectrumVerticalUnit_dBm -> 0
        SpectrumVerticalUnit_dBm = 0

        '''SpectrumVerticalUnit_Watt -> 1
        SpectrumVerticalUnit_Watt = 1

        '''SpectrumVerticalUnit_Volt -> 2
        SpectrumVerticalUnit_Volt = 2

        '''SpectrumVerticalUnit_Amp -> 3
        SpectrumVerticalUnit_Amp = 3

        '''SpectrumVerticalUnit_dBmV -> 4
        SpectrumVerticalUnit_dBmV = 4
    End Enum
End Class
