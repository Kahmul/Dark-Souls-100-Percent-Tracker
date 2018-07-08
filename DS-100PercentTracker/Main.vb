Imports System.IO
Imports System.Reflection
Imports System.Runtime.InteropServices
Imports System.Threading

Public Class Main

    Shared Version As String

    Private WithEvents updateTimer As New System.Windows.Forms.Timer()
    Const updateTimer_Interval = 33

    Private Declare Function OpenProcess Lib "kernel32.dll" (ByVal dwDesiredAcess As UInt32, ByVal bInheritHandle As Boolean, ByVal dwProcessId As Int32) As IntPtr
    Private Declare Function ReadProcessMemory Lib "kernel32" (ByVal hProcess As IntPtr, ByVal lpBaseAddress As IntPtr, ByVal lpBuffer() As Byte, ByVal iSize As Integer, ByRef lpNumberOfBytesRead As Integer) As Boolean
    Private Declare Function WriteProcessMemory Lib "kernel32" (ByVal hProcess As IntPtr, ByVal lpBaseAddress As IntPtr, ByVal lpBuffer() As Byte, ByVal iSize As Integer, ByVal lpNumberOfBytesWritten As Integer) As Boolean
    Public Declare Function CloseHandle Lib "kernel32.dll" (ByVal hObject As IntPtr) As Boolean
    Private Declare Function VirtualAllocEx Lib "kernel32.dll" (ByVal hProcess As IntPtr, ByVal lpAddress As IntPtr, ByVal dwSize As IntPtr, ByVal flAllocationType As Integer, ByVal flProtect As Integer) As IntPtr
    Private Declare Function VirtualProtectEx Lib "kernel32.dll" (hProcess As IntPtr, lpAddress As IntPtr, ByVal lpSize As IntPtr, ByVal dwNewProtect As UInt32, ByRef dwOldProtect As UInt32) As Boolean
    Private Declare Function VirtualFreeEx Lib "kernel32.dll" (hProcess As IntPtr, lpAddress As IntPtr, ByVal dwSize As Integer, ByVal dwFreeType As Integer) As Boolean
    Public Declare Function CreateRemoteThread Lib "kernel32" (ByVal hProcess As Integer, ByVal lpThreadAttributes As Integer, ByVal dwStackSize As Integer, ByVal lpStartAddress As Integer, ByVal lpParameter As Integer, ByVal dwCreationFlags As Integer, ByRef lpThreadId As Integer) As Integer
    <DllImport("kernel32", SetLastError:=True)>
    Shared Function WaitForSingleObject(
        ByVal handle As IntPtr,
        ByVal milliseconds As UInt32) As UInt32
    End Function

    <DllImport("kernel32")>
    Shared Function OpenThread(
        ByVal dwDesiredAccess As Integer,
        ByVal bInheritHandle As Boolean,
        ByVal dwThreadId As UInt32) As IntPtr
    End Function

    <DllImport("kernel32")>
    Shared Function SuspendThread(
        ByVal hThread As IntPtr) As UInt32
    End Function

    <DllImport("kernel32")>
    Shared Function ResumeThread(
        ByVal hThread As IntPtr) As Int32
    End Function

    <DllImport("kernel32.dll")>
    Shared Function FlushInstructionCache(
        ByVal hProcess As IntPtr,
        ByVal lpBaseAddress As IntPtr,
        ByVal dwSize As UIntPtr) As Boolean
    End Function

    Public Const PROCESS_VM_READ = &H10
    Public Const TH32CS_SNAPPROCESS = &H2
    Public Const MEM_COMMIT = 4096
    Public Const MEM_RELEASE = &H8000
    Public Const PAGE_READWRITE = 4
    Public Const PAGE_EXECUTE_READWRITE = &H40
    Public Const PROCESS_CREATE_THREAD = (&H2)
    Public Const PROCESS_VM_OPERATION = (&H8)
    Public Const PROCESS_VM_WRITE = (&H20)
    Public Const PROCESS_ALL_ACCESS = &H1F0FFF

    Dim isHooked As Boolean = False
    Public Shared exeVER As String = ""

    Private _targetProcess As Process = Nothing 'to keep track of it. not used yet...or is it? :thinking:
    Public Shared _targetProcessHandle As IntPtr = IntPtr.Zero 'Used for ReadProcessMemory

    Public Function ScanForProcess(ByVal windowCaption As String, Optional automatic As Boolean = False) As Boolean
        Dim _allProcesses() As Process = Process.GetProcesses
        Dim selectedProcess As Process = Nothing
        For Each pp As Process In _allProcesses
            If pp.MainWindowTitle.ToLower.Equals(windowCaption.ToLower) Then
                selectedProcess = pp
            Else
                pp.Dispose()
            End If
        Next
        If selectedProcess IsNot Nothing Then
            Return TryAttachToProcess(selectedProcess, automatic)
        End If
        Return False
    End Function
    Public Function TryAttachToProcess(ByVal proc As Process, Optional automatic As Boolean = False) As Boolean
        DetachFromProcess()

        _targetProcess = proc
        _targetProcessHandle = OpenProcess(PROCESS_ALL_ACCESS, False, _targetProcess.Id)
        If _targetProcessHandle = 0 Then
            If Not automatic Then 'Showing 2 message boxes as soon as you start the program is too annoying.
                MessageBox.Show("Failed to attach to process. Please rerun the application with administrative privileges.")
            End If

            Return False
        Else
            'if we get here, all connected and ready to use ReadProcessMemory()
            Return True
            'MessageBox.Show("OpenProcess() OK")
        End If

    End Function
    Public Sub DetachFromProcess()
        If _targetProcessHandle <> IntPtr.Zero Then
            _targetProcess.Dispose()
            _targetProcess = Nothing
            Try
                CloseHandle(_targetProcessHandle)
                _targetProcessHandle = IntPtr.Zero
                'MessageBox.Show("MemReader::Detach() OK")
            Catch ex As Exception
                MessageBox.Show("Warning: MemoryManager::DetachFromProcess::CloseHandle error " & Environment.NewLine & ex.Message)
            End Try
        End If
    End Sub

    Private Sub checkDarkSoulsVersion()
        Dim versionFlag = RUInt32(&H400080)
        If (versionFlag = &HCE9634B4&) Then
            exeVER = "Debug"
        ElseIf (versionFlag = &HE91B11E2&) Then
            exeVER = "Beta"
        ElseIf (versionFlag = &HFC293654&) Then
            exeVER = "Release"
        Else
            exeVER = "Unknown"
        End If
    End Sub

    Public Shared Function RInt8(ByVal addr As IntPtr) As SByte
        Dim _rtnBytes(0) As Byte
        ReadProcessMemory(_targetProcessHandle, addr, _rtnBytes, 1, vbNull)
        Return _rtnBytes(0)
    End Function
    Public Shared Function RInt16(ByVal addr As IntPtr) As Int16
        Dim _rtnBytes(1) As Byte
        ReadProcessMemory(_targetProcessHandle, addr, _rtnBytes, 2, vbNull)
        Return BitConverter.ToInt16(_rtnBytes, 0)
    End Function
    Public Shared Function RInt32(ByVal addr As IntPtr) As Int32
        Dim _rtnBytes(3) As Byte
        ReadProcessMemory(_targetProcessHandle, addr, _rtnBytes, 4, vbNull)
        Return BitConverter.ToInt32(_rtnBytes, 0)
    End Function
    Public Shared Function RInt64(ByVal addr As IntPtr) As Int64
        Dim _rtnBytes(7) As Byte
        ReadProcessMemory(_targetProcessHandle, addr, _rtnBytes, 8, vbNull)
        Return BitConverter.ToInt64(_rtnBytes, 0)
    End Function
    Public Shared Function RUInt16(ByVal addr As IntPtr) As UInt16
        Dim _rtnBytes(1) As Byte
        ReadProcessMemory(_targetProcessHandle, addr, _rtnBytes, 2, vbNull)
        Return BitConverter.ToUInt16(_rtnBytes, 0)
    End Function
    Public Shared Function RUInt32(ByVal addr As IntPtr) As UInt32
        Dim _rtnBytes(3) As Byte
        ReadProcessMemory(_targetProcessHandle, addr, _rtnBytes, 4, vbNull)
        Return BitConverter.ToUInt32(_rtnBytes, 0)
    End Function
    Public Shared Function RUInt64(ByVal addr As IntPtr) As UInt64
        Dim _rtnBytes(7) As Byte
        ReadProcessMemory(_targetProcessHandle, addr, _rtnBytes, 8, vbNull)
        Return BitConverter.ToUInt64(_rtnBytes, 0)
    End Function
    Public Shared Function RSingle(ByVal addr As IntPtr) As Single
        Dim _rtnBytes(3) As Byte
        ReadProcessMemory(_targetProcessHandle, addr, _rtnBytes, 4, vbNull)
        Return BitConverter.ToSingle(_rtnBytes, 0)
    End Function
    Public Shared Function RDouble(ByVal addr As IntPtr) As Double
        Dim _rtnBytes(7) As Byte
        ReadProcessMemory(_targetProcessHandle, addr, _rtnBytes, 8, vbNull)
        Return BitConverter.ToDouble(_rtnBytes, 0)
    End Function
    Public Shared Function RIntPtr(ByVal addr As IntPtr) As IntPtr
        Dim _rtnBytes(IntPtr.Size - 1) As Byte
        ReadProcessMemory(_targetProcessHandle, addr, _rtnBytes, IntPtr.Size, Nothing)
        If IntPtr.Size = 4 Then
            Return New IntPtr(BitConverter.ToInt32(_rtnBytes, 0))
        Else
            Return New IntPtr(BitConverter.ToInt64(_rtnBytes, 0))
        End If
    End Function
    Public Shared Function RBytes(ByVal addr As IntPtr, ByVal size As Int32) As Byte()
        Dim _rtnBytes(size - 1) As Byte
        ReadProcessMemory(_targetProcessHandle, addr, _rtnBytes, size, vbNull)
        Return _rtnBytes
    End Function

    Public Shared Sub WInt32(ByVal addr As IntPtr, val As Int32)
        WriteProcessMemory(_targetProcessHandle, addr, BitConverter.GetBytes(val), 4, Nothing)
    End Sub
    Public Shared Sub WUInt32(ByVal addr As IntPtr, val As UInt32)
        WriteProcessMemory(_targetProcessHandle, addr, BitConverter.GetBytes(val), 4, Nothing)
    End Sub
    Public Shared Sub WSingle(ByVal addr As IntPtr, val As Single)
        WriteProcessMemory(_targetProcessHandle, addr, BitConverter.GetBytes(val), 4, Nothing)
    End Sub
    Public Shared Sub WBytes(ByVal addr As IntPtr, val As Byte())
        WriteProcessMemory(_targetProcessHandle, addr, val, val.Length, Nothing)
    End Sub

    Public Shared Function ReadFlag32(address As Integer, mask As UInteger) As Boolean
        Dim _rtnBytes(3) As Byte
        ReadProcessMemory(_targetProcessHandle, address, _rtnBytes, 4, vbNull)
        Dim flags = BitConverter.ToUInt32(_rtnBytes, 0)

        Return (flags And mask) <> 0
    End Function

    Private Sub SetDarkSoulsThreadSuspend(suspend As Boolean)

        If _targetProcess Is Nothing Then Return

        For Each pthread As ProcessThread In _targetProcess.Threads

            Dim pOpenThread = OpenThread(&H2, False, pthread.Id)

            If pOpenThread = 0 Then
                Continue For
            End If

            If suspend Then
                SuspendThread(pOpenThread)
            Else
                Dim suspendCount = 0
                Do
                    suspendCount = ResumeThread(pOpenThread)
                Loop While suspendCount > 0
            End If

            CloseHandle(pOpenThread)
        Next

    End Sub

    Private Sub SetHookButtonsEnabled(hookEnabled As Boolean, unhookEnabled As Boolean)
        Invoke(
            Sub()
                btnHook.Enabled = hookEnabled
                btnUnhook.Enabled = unhookEnabled
            End Sub)
    End Sub

    Private Sub btnHook_Click(sender As Object, e As EventArgs) Handles btnHook.Click

        Dim newThread = New Thread(AddressOf HookInOtherThread) With {.IsBackground = True}
        newThread.Start()

    End Sub

    Public Shared eventFlagPtr As Integer

    Private Sub HookInOtherThread()
        SetHookButtonsEnabled(False, False)

        If ScanForProcess("DARK SOULS", True) Then
            checkDarkSoulsVersion()
            If Not (exeVER = "Debug" Or exeVER = "Release") Then
                MsgBox("Invalid EXE type.")
                Return
            End If

            eventFlagPtr = If(exeVER = "Debug", RInt32(&H1381994), RInt32(&H137D7D4))
            eventFlagPtr = RInt32(eventFlagPtr + 0)

            Invoke(
                Sub()
                    updateTimer = New System.Windows.Forms.Timer
                    updateTimer.Interval = updateTimer_Interval
                    AddHandler updateTimer.Tick, AddressOf updateTimer_Tick
                    updateTimer.Start()
                End Sub)


        Else
            MsgBox("Couldn't find the Dark Souls process!")
            SetHookButtonsEnabled(True, False)
            Return
        End If

        SetHookButtonsEnabled(False, True)
    End Sub

    Private Sub updateTimer_Tick()

        Dim newThread As New Thread(AddressOf scanEventFlagsAndUpdateUI) With {.IsBackground = True}
        newThread.Start()

    End Sub

    Dim lastPercentage As Double

    Private Sub scanEventFlagsAndUpdateUI()
        ' Timer running at an interval of 500ms. Calls the Game class to update its event flags and then updates the UI.

        'Return if the player is not in his own world
        If Game.isPlayerInOwnWorld() = False Then
            Return
        End If

        'If the IGT doesn't change, then the player entered a loading screen or is in the main menu
        'Additional check to isPlayerLoaded() because it returns true in some cases where the player is not actually in-game
        Dim currentIGT = Game.GetIngameTimeInMilliseconds()
        Thread.Sleep(50)
        Dim nextIGT = Game.GetIngameTimeInMilliseconds()
        If nextIGT = currentIGT Then
            Return
        End If

        Invoke(
            Sub()
                updateTimer.Stop()
            End Sub)

        Game.updateAllEventFlags()

        'Before updating the UI, check if the player is in a loadscreen to make sure all flags have been accounted for
        If Game.IsPlayerLoaded() = False Then
            Invoke(
                Sub()
                    updateTimer.Start()
                End Sub)
            Return
        End If

        If Game.GetClearCount > 0 Then
            'Make sure entering NG+ doesn't override a previous run,
            'but still allow increases to happen during the ending cutscene before the credits
            If Game.GetTotalCompletionPercentage < lastPercentage Then
                Invoke(
                Sub()
                    updateTimer.Start()
                End Sub)
                Return
            End If
        End If


        Invoke(
            Sub()
                treasureLocationsValueLabel.Text = $"{Game.GetTreasureLocationsCleared} / {Game.GetTotalTreasureLocationsCount}"
                bossesKilledValueLabel.Text = $"{Game.GetBossesKilled} / {Game.GetTotalBossCount}"
                nonRespawningEnemiesValueLabel.Text = $"{Game.GetNonRespawningEnemiesKilled} / {Game.GetTotalNonRespawningEnemiesCount}"
                npcQuestlinesValueLabel.Text = $"{Game.GetNPCQuestlinesCompleted} / {Game.GetTotalNPCQuestlinesCount}"
                shortcutsValueLabel.Text = $"{Game.GetShortcutsAndLockedDoorsUnlocked} / {Game.GetTotalShortcutsAndLockedDoorsCount}"
                illusoryWallsValueLabel.Text = $"{Game.GetIllusoryWallsRevealed} / {Game.GetTotalIllusoryWallsCount}"
                foggatesValueLabel.Text = $"{Game.GetFoggatesDissolved} / {Game.GetTotalFoggatesCount}"
                bonfiresValueLabel.Text = $"{Game.GetBonfiresFullyKindled} / {Game.GetTotalBonfiresCount}"

                Dim totalPercentage = Game.GetTotalCompletionPercentage.ToString("0.0")
                percentageLabel.Text = $"{totalPercentage}%"
                lastPercentage = Game.GetTotalCompletionPercentage

                updateTimer.Start()
            End Sub)
    End Sub

    Private Sub isInMainMenu()
        Dim ptr = If(exeVER = "Debug", RInt32(&H137C890), RInt32(&H13786D0))
        If ptr = 0 Then Return
        'Dim int = CType(RBytes(ptr + &HB4E, 1)(0), Integer)
        Dim int = RInt32(ptr + &HFD0)
        Console.WriteLine($"{int}")
    End Sub


    Private Sub UI_Load(sender As Object, e As EventArgs) Handles MyBase.Load

    End Sub

    Private Sub unhook()

        isHooked = False

        updateTimer.Stop()
        Dim updateTimerTickEventHandler As New EventHandler(AddressOf updateTimer_Tick)
        RemoveHandler updateTimer.Tick, updateTimerTickEventHandler

        DetachFromProcess()
    End Sub

    Private Sub btnUnhook_Click(sender As Object, e As EventArgs) Handles btnUnhook.Click

        Dim newThread = New Thread(AddressOf DoUnhookInOtherThread) With {.IsBackground = True}
        newThread.Start()

    End Sub

    Private Sub DoUnhookInOtherThread()
        SetHookButtonsEnabled(False, False)
        unhook()
        SetHookButtonsEnabled(True, False)
    End Sub

    Private Sub RestartHook()
        DoUnhookInOtherThread()
        Dim newThread = New Thread(AddressOf HookInOtherThread) With {.IsBackground = True}
        newThread.Start()
    End Sub

    Private Sub UI_Exit(sender As Object, e As EventArgs) Handles MyBase.Closing
        Dim newThread = New Thread(AddressOf unhook) With {.IsBackground = True}
        newThread.Start()
    End Sub

    Private Sub Label2_Click(sender As Object, e As EventArgs)

    End Sub

    Private Sub Label1_Click(sender As Object, e As EventArgs) Handles treasureLocationsLabel.Click

    End Sub

    Private Sub Label3_Click(sender As Object, e As EventArgs) Handles percentageLabel.Click

    End Sub

    Private Sub Label5_Click(sender As Object, e As EventArgs) Handles Label5.Click

    End Sub

    Private Sub Label13_Click(sender As Object, e As EventArgs) Handles treasureLocationsValueLabel.Click

    End Sub

    Private Sub Label1_Click_1(sender As Object, e As EventArgs) Handles Label1.Click

    End Sub

    Private Sub Label8_Click(sender As Object, e As EventArgs) Handles Label8.Click

    End Sub

    Private Sub Label3_Click_1(sender As Object, e As EventArgs) Handles bonfiresValueLabel.Click

    End Sub

    Private Sub Label2_Click_1(sender As Object, e As EventArgs) Handles Label2.Click

    End Sub
End Class
