﻿'Hack Trainer | Private SDK
'Made by Destroyer | Discord : Destroyer#3527
'Creation date : 4/02/2017
'Last Update : 26/06/2019  - Minimal Update

Namespace DestroyerSDK

    Public Class Injector

#Region " Declare's "

        Declare Function OpenProcess Lib "kernel32" (ByVal dwDesiredAccess As UInt32, ByVal bInheritHandle As Int32, ByVal dwProcessId As UInt32) As IntPtr
        Declare Function CloseHandle Lib "kernel32" (ByVal hObject As IntPtr) As Int32
        Declare Function WriteProcessMemory Lib "kernel32" (ByVal hProcess As IntPtr, ByVal lpBaseAddress As IntPtr, ByVal buffer As Byte(), ByVal size As UInt32, ByRef lpNumberOfBytesWritten As IntPtr) As Boolean
        Declare Function GetProcAddress Lib "kernel32" (ByVal hModule As IntPtr, ByVal methodName As String) As IntPtr
        Declare Function GetModuleHandleA Lib "kernel32" (ByVal moduleName As String) As IntPtr
        Declare Function VirtualAllocEx Lib "kernel32" (ByVal hProcess As IntPtr, ByVal lpAddress As IntPtr, ByVal dwSize As IntPtr, ByVal flAllocationType As UInteger, ByVal flProtect As UInteger) As IntPtr
        Declare Function CreateRemoteThread Lib "kernel32" (ByVal hProcess As IntPtr, ByVal lpThreadAttribute As IntPtr, ByVal dwStackSize As IntPtr, ByVal lpStartAddress As IntPtr, ByVal lpParameter As IntPtr, ByVal dwCreationFlags As UInteger, ByVal lpThreadId As IntPtr) As IntPtr
        Declare Function GetPrivateProfileStringA Lib "kernel32" (ByVal lpAppName As String, ByVal lpKeyName As String, ByVal lpDefault As String, ByVal lpReturnedString As System.Text.StringBuilder, ByVal nSize As Integer, ByVal lpFileName As String) As Integer
        Declare Function WritePrivateProfileStringA Lib "kernel32" (ByVal lpAppName As String, ByVal lpKeyName As String, ByVal lpString As String, ByVal lpFileName As String) As Integer

#End Region

#Region " Method's "

        Private Shared Function CreateRemoteThread(ByVal procToBeInjected As Process, ByVal sDllPath As String) As Boolean
            Dim lpLLAddress As IntPtr = IntPtr.Zero
            Dim hndProc As IntPtr = OpenProcess(&H2 Or &H8 Or &H10 Or &H20 Or &H400, 1, CUInt(procToBeInjected.Id))
            If hndProc = IntPtr.Zero Then
                Return False
            End If
            lpLLAddress = GetProcAddress(GetModuleHandleA("kernel32.dll"), "LoadLibraryA")
            If lpLLAddress = CType(0, IntPtr) Then
                Return False
            End If
            Dim lpAddress As IntPtr = VirtualAllocEx(hndProc, CType(Nothing, IntPtr), CType(sDllPath.Length, IntPtr), CUInt(&H1000) Or CUInt(&H2000), CUInt(&H40))
            If lpAddress = CType(0, IntPtr) Then
                Return False
            End If
            Dim bytes As Byte() = System.Text.Encoding.ASCII.GetBytes(sDllPath)
            Dim ipTmp As IntPtr = IntPtr.Zero
            WriteProcessMemory(hndProc, lpAddress, bytes, CUInt(bytes.Length), ipTmp)
            If ipTmp = IntPtr.Zero Then
                Return False
            End If
            Dim ipThread As IntPtr = CreateRemoteThread(hndProc, CType(Nothing, IntPtr), IntPtr.Zero, lpLLAddress, lpAddress, 0, CType(Nothing, IntPtr))
            If ipThread = IntPtr.Zero Then
                Return False
            End If
            Return True
        End Function

        Public Shared Function InjectDLL(ByVal ProcessName As String, ByVal sDllPath As String) As Boolean
            System.Threading.Thread.Sleep(5000)
            Dim p As Process() = Process.GetProcessesByName(ProcessName)
            If p.Length <> 0 Then
                If Not CreateRemoteThread(p(0), sDllPath) Then
                    If p(0).MainWindowHandle <> CType(0, IntPtr) Then
                        CloseHandle(p(0).MainWindowHandle)
                    End If
                    Return False
                End If
                Return True
            End If
            Return False
        End Function

#End Region

    End Class

End Namespace