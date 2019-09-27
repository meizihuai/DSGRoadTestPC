Imports System
Imports System.IO
Imports System.Runtime.InteropServices
Imports Common.Models

Public Class TekApi
    Public Shared Function DeviceSearch() As DeviceSearchInfo
        Dim numDevicesFound As IntPtr
        Dim deviceIDs(10) As IntPtr
        Dim deviceSerial(10) As IntPtr
        Dim deviceType(10) As IntPtr
        Dim result As IntPtr = TekDriver.DEVICE_SearchInt(numDevicesFound, deviceIDs, deviceSerial, deviceType)
        If result.ToInt32() <> 0 Then Return Nothing
        Dim info As New DeviceSearchInfo()
        info.Num = numDevicesFound.ToInt32()
        info.DeviceId = deviceIDs(0).ToInt32()
        info.Serials = Marshal.PtrToStringAnsi(deviceSerial(0))
        info.Type = Marshal.PtrToStringAnsi(deviceType(0))
        Return info
    End Function
End Class
