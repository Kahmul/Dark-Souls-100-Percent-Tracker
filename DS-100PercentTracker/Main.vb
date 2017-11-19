Imports System.IO
Imports System.Reflection
Imports System.Runtime.InteropServices
Imports System.Threading

Public Class Main

    Shared Version As String

    Private WithEvents refTimer As New System.Windows.Forms.Timer()
    Const refTimer_Interval = 500

    Private Declare Function OpenProcess Lib "kernel32.dll" (ByVal dwDesiredAcess As UInt32, ByVal bInheritHandle As Boolean, ByVal dwProcessId As Int32) As IntPtr
    Private Declare Function ReadProcessMemory Lib "kernel32" (ByVal hProcess As IntPtr, ByVal lpBaseAddress As IntPtr, ByVal lpBuffer() As Byte, ByVal iSize As Integer, ByRef lpNumberOfBytesRead As Integer) As Boolean
    Private Declare Function WriteProcessMemory Lib "kernel32" (ByVal hProcess As IntPtr, ByVal lpBaseAddress As IntPtr, ByVal lpBuffer() As Byte, ByVal iSize As Integer, ByVal lpNumberOfBytesWritten As Integer) As Boolean
    Private Declare Function CloseHandle Lib "kernel32.dll" (ByVal hObject As IntPtr) As Boolean
    Private Declare Function VirtualAllocEx Lib "kernel32.dll" (ByVal hProcess As IntPtr, ByVal lpAddress As IntPtr, ByVal dwSize As IntPtr, ByVal flAllocationType As Integer, ByVal flProtect As Integer) As IntPtr
    Private Declare Function VirtualProtectEx Lib "kernel32.dll" (hProcess As IntPtr, lpAddress As IntPtr, ByVal lpSize As IntPtr, ByVal dwNewProtect As UInt32, ByRef dwOldProtect As UInt32) As Boolean
    Private Declare Function VirtualFreeEx Lib "kernel32.dll" (hProcess As IntPtr, lpAddress As IntPtr, ByVal dwSize As Integer, ByVal dwFreeType As Integer) As Boolean
    Private Declare Function CreateRemoteThread Lib "kernel32" (ByVal hProcess As Integer, ByVal lpThreadAttributes As Integer, ByVal dwStackSize As Integer, ByVal lpStartAddress As Integer, ByVal lpParameter As Integer, ByVal dwCreationFlags As Integer, ByRef lpThreadId As Integer) As Integer
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
    Dim exeVER As String = ""

    Dim hook1mem As IntPtr
    Dim getflagfuncmem As IntPtr
    Dim setflagfuncmem As IntPtr

    Dim hooks As Hashtable
    Dim dbgHooks As Hashtable = New Hashtable
    Dim rlsHooks As Hashtable = New Hashtable

    Private _targetProcess As Process = Nothing 'to keep track of it. not used yet...or is it? :thinking:
    Private _targetProcessHandle As IntPtr = IntPtr.Zero 'Used for ReadProcessMemory

    Dim totalItemFlags As Array = {51000000, 51000010, 51000020, 51000030, 51000040, 51000050, 51000090, 51000100, 51000120, 51000140,
                                    51000170, 51000180, 51000190, 51000210, 51000240, 51000500, 51010000, 51010020, 51010040, 51010050,
                                    51010070, 51010080, 51010090, 51010100, 51010120, 51010140, 51010160, 51010210, 51010380, 51010400,
                                    51010130, 51010370, 51010430, 51010440, 51010470, 51010480, 51010490, 51010500, 51010510, 51010520,
                                    51010220, 51010260, 51010280, 51010300, 51010450, 51010460, 51020000, 51020010, 51020020, 51020030,
                                    51020040, 51020050, 51020060, 51020150, 51020160, 51020090, 51020170, 51020110, 51020120, 51020130,
                                    51020140, 51020210, 50001030, 51020070, 51020180, 51020190, 51020200, 51100010, 51100020, 51100030,
                                    51100040, 51100060, 51100070, 51100090, 51100100, 51100120, 51100130, 51100140, 51100150, 51100160,
                                    51100170, 51100190, 51100200, 51100210, 51100230, 51100240, 51100250, 51100260, 51100280, 51100290,
                                    51100300, 51100310, 51100320, 51100330, 51100340, 51100350, 51100370, 51100500, 51200030, 51200010,
                                    51200020, 51200040, 51200060, 51200070, 51200080, 51200180, 51200120, 51200150, 51200160, 51200170,
                                    51200190, 51200200, 51200210, 51200140, 51200500, 51200510, 51210010, 51210020, 51210030, 51210040,
                                    51210050, 51210060, 51210070, 51210080, 51210090, 51211000, 51210110, 51210160, 51210190, 51210210,
                                    51210220, 51210230, 51210240, 51210250, 51210260, 51210270, 51210280, 51210290, 51210300, 51210330,
                                    51210340, 51210350, 51210390, 51210400, 51210430, 51210440, 51210450, 51210460, 51210470, 51210500,
                                    51210510, 51210520, 51210540, 51210550, 51300000, 51300010, 51300020, 51300030, 51300070, 51300100,
                                    51300110, 51300140, 51300150, 51300190, 51300210, 51300220, 51300230, 51300240, 51300250, 51310000,
                                    51310010, 51310020, 51310030, 51310040, 51310050, 51310070, 51310080, 51310090, 51310100, 51310110,
                                    51310120, 51310140, 51310160, 51310180, 51310200, 51310220, 51310230, 51310240, 51310290, 51310300,
                                    51310500, 51320000, 51320020, 51320040, 51320050, 51320060, 51320070, 51320080, 51320090, 51320100,
                                    51320110, 51320120, 51320140, 51320150, 51320160, 51320170, 51320190, 51320180, 51400020, 51400040,
                                    51400050, 51400060, 51400080, 51400090, 51400100, 51400130, 51400140, 51400150, 51400160, 51400180,
                                    51400190, 51400210, 51400230, 51400250, 51400260, 51400270, 51400280, 51400290, 51400300, 51400310,
                                    51400320, 51400340, 51400350, 51400360, 51400370, 51400500, 51400510, 51400520, 51410000, 51410010,
                                    51410020, 51410030, 51410050, 51410060, 51410080, 51410090, 51410140, 51410160, 51410180, 51410230,
                                    51410250, 51410270, 51410310, 51410320, 51410330, 51410340, 51410360, 51410380, 51410390, 51410400,
                                    51410510, 51410530, 51410500, 51410100, 51410410, 51410520, 51500300, 51500310, 51500320, 51500330,
                                    51500350, 51500360, 51500400, 51500420, 51500070, 51500060, 51500010, 51500080, 51500150, 51500410,
                                    51500440, 51500000, 51500020, 51500040, 51500090, 51500100, 51510000, 51510030, 51510040, 51510050,
                                    51510060, 51510070, 51510080, 51510700, 51510510, 51510520, 51510530, 51510540, 51510570, 51510560,
                                    51510580, 51510590, 51510600, 51510610, 51510620, 51510660, 51510670, 51510680, 51510690, 51600000,
                                    51600020, 51600030, 51600040, 51600060, 51600070, 51600090, 51600100, 51600110, 51600120, 51600130,
                                    51600140, 51600150, 51600160, 51600170, 51600180, 51600190, 51600200, 51600210, 51600220, 51600250,
                                    51600260, 51600270, 51600280, 51600310, 51600330, 51600360, 51600370, 51600380, 51600520, 51600500,
                                    51600510, 51600290, 51700000, 51700010, 51700040, 51700060, 51700070, 51700080, 51700120, 51700150,
                                    51700160, 51700170, 51700180, 51700200, 51700210, 51700650, 51700510, 51700520, 51700530, 51700540,
                                    51700560, 51700580, 51700590, 51700600, 51700630, 51700640, 51700020, 51700050, 51800050, 51810000,
                                    51810060, 51810070, 51810080,
                                    51100980, 'Firesurge hollow item flag
                                    51700990  'Duke's Archives Tower Cell Key Serpent item flag
                                        }

    Dim totalBossFlags As Array = {2, 3, 4, 5, 6, 7, 9, 10, 11, 12, 13, 14,
                                    15, 16, 11210000, 11210001, 11210002, 11210004, 11410410,
                                    11410900, 11410901, 11510900, 11010901, 11010902, 11200900, 11810900}

    Dim totalNonRespawningEnemiesFlags As Array = {11010903, 11700815, 11700816, 11010861, 11300859, 11310820, 11810850, 11810851, 11200816, 11010862,
                                                    11000851, 11000852, 11700820, 11700821, 11000800, 11010860, 1062, 11410106, 11700810, 11010865,
                                                    11010863, 11200813, 11200814, 11200815, 11010900, 11200801, 11320100, 11400902, 11600850, 11600851,
                                                    11300858, 11500861, 11500862, 11500863, 11500864, 11510860, 11010864, 11100400, 11600810, 11500865,
                                                    11500867, 11515080, 11515081, 11410138, 11410139, 11410140, 11410141, 11410142, 11410143, 11410144,
                                                    11410109, 11410110, 11410111, 11410112, 11410113, 11410114, 11410115, 11410116, 11410117, 11410118,
                                                    11410119, 11410120, 11410121, 11410122, 11410123, 11410124, 11410125, 11410126, 11410127, 11410128,
                                                    11410129, 11410130, 11410131, 11410132, 11410133, 11410134, 11410135, 11410136, 11410137, 11200818,
                                                    11200819, 11200817, 11510863, 11510864, 11500860, 11700822, 11700823, 800, 11000850, 11010866,
                                                    11210680, 11210681, 11500900, 11510850, 11510851, 11510852, 11510853, 11700900, 11700901, 11300850,
                                                    11300851, 11300852, 11300853, 11300854, 11300855, 11400850, 11400851, 11400853, 11400854, 11400855,
                                                    11400856, 11400857, 11400858, 11200811, 11200810, 11010710, 11200812, 11210350, 11210351, 11210352,
                                                    11210353, 11210354, 11300856, 11300857, 11310821, 11410107, 11700811, 11700812, 11700813, 11700814,
                                                    11320301, 11320302, 11320303, 11320304, 11320305, 11320306, 11320307, 11320308, 11320309, 11320310,
                                                    11210310, 11210311, 11210312, 11210313, 11210314, 11210315, 11210355, 11410100, 11410101, 11410102,
                                                    11410103}

    Dim totalNPCQuestlineFlags As Array = {11020101, 1862, 1462, 1431, 1115, 1313, 1254, 1097, 1626,
                                            1177, 11200535, 11020606, 1003, 11210021}

    Dim totalShortcutsLockedDoorsFlags As Array = {11810105, 11600100, 11410340, 11210102, 11210122, 11210132, 11510251, 11510257, 11510220, 11510200,
                                                    11510210, 11020302, 11500100, 11500105, 11010101, 11010160, 11200110, 11600160, 11010100, 11700120,
                                                    11700110, 11300900, 11300901, 11300210, 11100135, 11100030, 11000100, 11010621, 'Shortcuts end here
                                                    11810103, 11810104, 11810106, 11810110, 11600120, 11700300, 11700301, 11700302, 11700303, 11700304,
                                                    11700305, 11700306, 11700140, 11000111, 11500112, 11500116, 11010171, 11600110, 11010140, 11010181,
                                                    11000120, 11010191, 11010192, 11010172, 11100120, 11210650 'Locked doors end here
                                                    }

    Dim totalIllusoryWallsFlags As Array = {11200120, 11300160, 11320200, 11320201, 11400210, 11510215, 11510401, 11410360, 11310100, 11210200,
                                            11210201, 11210346, 11210025}

    Dim totalFoggatesFlags As Array = {11510090, 11510091, 11810090, 11010090, 11300090, 11310090, 11400091, 11500090, 11500091, 11010091,
                                        11320090, 11000090, 11200090, 11700083, 11100091, 11600090}

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

    Public Function RInt8(ByVal addr As IntPtr) As SByte
        Dim _rtnBytes(0) As Byte
        ReadProcessMemory(_targetProcessHandle, addr, _rtnBytes, 1, vbNull)
        Return _rtnBytes(0)
    End Function
    Public Function RInt16(ByVal addr As IntPtr) As Int16
        Dim _rtnBytes(1) As Byte
        ReadProcessMemory(_targetProcessHandle, addr, _rtnBytes, 2, vbNull)
        Return BitConverter.ToInt16(_rtnBytes, 0)
    End Function
    Public Function RInt32(ByVal addr As IntPtr) As Int32
        Dim _rtnBytes(3) As Byte
        ReadProcessMemory(_targetProcessHandle, addr, _rtnBytes, 4, vbNull)
        Return BitConverter.ToInt32(_rtnBytes, 0)
    End Function
    Public Function RInt64(ByVal addr As IntPtr) As Int64
        Dim _rtnBytes(7) As Byte
        ReadProcessMemory(_targetProcessHandle, addr, _rtnBytes, 8, vbNull)
        Return BitConverter.ToInt64(_rtnBytes, 0)
    End Function
    Public Function RUInt16(ByVal addr As IntPtr) As UInt16
        Dim _rtnBytes(1) As Byte
        ReadProcessMemory(_targetProcessHandle, addr, _rtnBytes, 2, vbNull)
        Return BitConverter.ToUInt16(_rtnBytes, 0)
    End Function
    Public Function RUInt32(ByVal addr As IntPtr) As UInt32
        Dim _rtnBytes(3) As Byte
        ReadProcessMemory(_targetProcessHandle, addr, _rtnBytes, 4, vbNull)
        Return BitConverter.ToUInt32(_rtnBytes, 0)
    End Function
    Public Function RUInt64(ByVal addr As IntPtr) As UInt64
        Dim _rtnBytes(7) As Byte
        ReadProcessMemory(_targetProcessHandle, addr, _rtnBytes, 8, vbNull)
        Return BitConverter.ToUInt64(_rtnBytes, 0)
    End Function
    Public Function RSingle(ByVal addr As IntPtr) As Single
        Dim _rtnBytes(3) As Byte
        ReadProcessMemory(_targetProcessHandle, addr, _rtnBytes, 4, vbNull)
        Return BitConverter.ToSingle(_rtnBytes, 0)
    End Function
    Public Function RDouble(ByVal addr As IntPtr) As Double
        Dim _rtnBytes(7) As Byte
        ReadProcessMemory(_targetProcessHandle, addr, _rtnBytes, 8, vbNull)
        Return BitConverter.ToDouble(_rtnBytes, 0)
    End Function
    Public Function RIntPtr(ByVal addr As IntPtr) As IntPtr
        Dim _rtnBytes(IntPtr.Size - 1) As Byte
        ReadProcessMemory(_targetProcessHandle, addr, _rtnBytes, IntPtr.Size, Nothing)
        If IntPtr.Size = 4 Then
            Return New IntPtr(BitConverter.ToInt32(_rtnBytes, 0))
        Else
            Return New IntPtr(BitConverter.ToInt64(_rtnBytes, 0))
        End If
    End Function
    Public Function RBytes(ByVal addr As IntPtr, ByVal size As Int32) As Byte()
        Dim _rtnBytes(size - 1) As Byte
        ReadProcessMemory(_targetProcessHandle, addr, _rtnBytes, size, vbNull)
        Return _rtnBytes
    End Function

    Public Sub WInt32(ByVal addr As IntPtr, val As Int32)
        WriteProcessMemory(_targetProcessHandle, addr, BitConverter.GetBytes(val), 4, Nothing)
    End Sub
    Public Sub WUInt32(ByVal addr As IntPtr, val As UInt32)
        WriteProcessMemory(_targetProcessHandle, addr, BitConverter.GetBytes(val), 4, Nothing)
    End Sub
    Public Sub WSingle(ByVal addr As IntPtr, val As Single)
        WriteProcessMemory(_targetProcessHandle, addr, BitConverter.GetBytes(val), 4, Nothing)
    End Sub
    Public Sub WBytes(ByVal addr As IntPtr, val As Byte())
        WriteProcessMemory(_targetProcessHandle, addr, val, val.Length, Nothing)
    End Sub

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

    Private Sub HookInOtherThread()
        SetHookButtonsEnabled(False, False)

        If ScanForProcess("DARK SOULS", True) Then
            checkDarkSoulsVersion()
            If Not (exeVER = "Debug" Or exeVER = "Release") Then
                MsgBox("Invalid EXE type.")
                Return
            End If

            If exeVER = "Release" Then hooks = rlsHooks
            If exeVER = "Debug" Then hooks = dbgHooks

            SetDarkSoulsThreadSuspend(True)

            initFlagHook()
            initGetFlagFunc()

            SetDarkSoulsThreadSuspend(False)

            Invoke(
                Sub()
                    refTimer = New System.Windows.Forms.Timer
                    refTimer.Interval = refTimer_Interval
                    AddHandler refTimer.Tick, AddressOf refTimer_Tick
                    refTimer.Start()
                End Sub)


        Else
            MsgBox("Couldn't find the Dark Souls process!")
            SetHookButtonsEnabled(True, False)
            Return
        End If

        SetHookButtonsEnabled(False, True)
    End Sub

    Private Sub initFlagHook()
        hook1mem = VirtualAllocEx(_targetProcessHandle, 0, &H8000, MEM_COMMIT, PAGE_EXECUTE_READWRITE)
        Dim oldProtectionOut As UInteger
        VirtualProtectEx(_targetProcessHandle, hook1mem, &H8000, PAGE_EXECUTE_READWRITE, oldProtectionOut)
        isHooked = True

        Dim a As New asm

        a.AddVar("hook", hooks("hook1"))
        a.AddVar("newmem", hook1mem)
        a.AddVar("vardump", hook1mem + &H400)
        a.AddVar("hookreturn", hooks("hook1return"))
        a.AddVar("startloop", 0)
        a.AddVar("exitloop", 0)

        a.pos = hook1mem
        a.Asm("pushad")
        a.Asm("mov eax, vardump")

        a.Asm("startloop:")
        a.Asm("mov ecx, [eax]")
        a.Asm("cmp ecx, 0")
        a.Asm("je exitloop")

        a.Asm("add eax, 0x8")
        a.Asm("jmp startloop")

        a.Asm("exitloop:")
        a.Asm("mov [eax], edx")
        a.Asm("mov edx, [esp+0x24]")
        a.Asm("mov [eax+4], edx")
        a.Asm("popad")
        a.Asm("call " & hooks("hook1seteventflag").toint32())
        a.Asm("jmp hookreturn")

        WriteCodeAndFlushCache(_targetProcessHandle, hook1mem, a.bytes, a.bytes.Length, 0)

        a.Clear()
        a.AddVar("newmem", hook1mem)
        a.pos = hooks("hook1").toint32()
        a.Asm("jmp newmem")

        WriteCodeAndFlushCache(_targetProcessHandle, hooks("hook1"), a.bytes, a.bytes.Length, 0)
    End Sub

    Private Sub initGetFlagFunc()
        getflagfuncmem = VirtualAllocEx(_targetProcessHandle, 0, &H8000, MEM_COMMIT, PAGE_EXECUTE_READWRITE)
        Dim oldProtectionOut As UInteger
        VirtualProtectEx(_targetProcessHandle, getflagfuncmem, &H8000, PAGE_EXECUTE_READWRITE, oldProtectionOut)

        Dim a As New asm

        a.AddVar("newmem", getflagfuncmem)
        a.AddVar("vardump", getflagfuncmem + &H400)

        a.pos = getflagfuncmem
        a.Asm("pushad")
        a.Asm("mov eax, vardump")
        a.Asm("mov eax, [eax]")
        a.Asm("push eax")
        a.Asm("call " & hooks("geteventflagvalue").toint32)
        a.Asm("mov ecx, vardump")
        a.Asm("add ecx, 4")
        a.Asm("mov [ecx], eax")
        a.Asm("add ecx, 4")
        a.Asm("mov eax, 1")
        a.Asm("mov [ecx], eax")
        a.Asm("popad")
        a.Asm("ret")

        WriteCodeAndFlushCache(_targetProcessHandle, getflagfuncmem, a.bytes, a.bytes.Length, 0)
    End Sub

    Private Sub refTimer_Tick()

        Dim newThread As New Thread(AddressOf CheckAllEventFlags) With {.IsBackground = True}
        newThread.Start()

    End Sub

    'Used to restart the hook everytime the player enters a loadscreen or the main menu.
    'The program can't stay hooked for too long otherwise the game crashes. This eleviates this issue
    Dim reloadedHook = True

    Private Sub CheckAllEventFlags()
        ' Timer running at an interval of 500ms. Checks all the flags defined at the top

        If IsPlayerLoaded() = False Then
            'Everytime the player enters a loadscreen, the hook gets disconnected and reconnected
            'If IGT returns 0, the player is in the main menu. Disconnecting in the main menu may lead to the game freezing
            If reloadedHook = False And GetIngameTimeInMilliseconds() <> 0 Then
                rehook()
                reloadedHook = True
            End If
            Return
        End If

        reloadedHook = False

        'Return if the player is not in his own world
        If isPlayerInOwnWorld() = False Then
            Return
        End If

        'If the IGT doesn't change, then the player entered a loading screen or is in the main menu
        'Additional check to isPlayerLoaded() because it returns true in some cases where the player is not actually in-game
        Dim currentIGT = GetIngameTimeInMilliseconds()
        Thread.Sleep(50)
        Dim nextIGT = GetIngameTimeInMilliseconds()
        If nextIGT = currentIGT Then
            Return
        End If

        Invoke(
            Sub()
                refTimer.Stop()
            End Sub)

        Dim item As Integer
        Dim value As Boolean

        Dim bossesKilled As Integer
        Dim nonRespawningEnemiesKilled As Integer
        Dim npcQuestlinesCompleted As Integer
        Dim itemsPickedUp As Integer
        Dim shortcutsLockedDoorsUnlocked As Integer
        Dim illusoryWallsRevealed As Integer
        Dim foggatesDissolved As Integer

        Dim startingClass = GetPlayerStartingClass()

        'Check all treasure locations
        For Each item In totalItemFlags
            'If the treasure location has multiple items, check if the last item has been picked up instead to confirm all items have been picked up
            If Dictionaries.sharedTreasureLocationItems.ContainsKey(item) Then
                Dim values = Dictionaries.sharedTreasureLocationItems.Item(item)
                item = values(values.Length - 1)
            End If
            value = GetEventFlagState(item)

            If value = True Then
                itemsPickedUp += 1
            End If
        Next

        'Check which starting items the player had and whether he picked them up
        Dim startingItemFlags = Dictionaries.startingClassItems.Item(startingClass)

        For Each item In startingItemFlags
            value = GetEventFlagState(item)

            If value = True Then
                itemsPickedUp += 1
            End If
        Next

        'Check for killed NPCs. If one is killed, add their drops to the required item total and check if they have been picked up
        Dim additionalNPCItemsCount As Integer

        For Each pair As KeyValuePair(Of Integer, Array) In Dictionaries.npcDroppedItems
            'Check if NPC-dead flag is true
            value = GetEventFlagState(pair.Key)
            If value = True Then
                'If NPC is dead, add his treasure location to required total and check whether the last item has been picked up
                additionalNPCItemsCount += 1
                item = pair.Value(pair.Value.Length - 1)
                value = GetEventFlagState(item)
                If value = True Then
                    itemsPickedUp += 1
                End If
            End If
        Next

        'Add up the standard treasure location count with the starting items count and the NPC-dropped items count
        Dim totalItemsCount = totalItemFlags.Length + startingItemFlags.Length + additionalNPCItemsCount

        'Check bosses killed
        bossesKilled = getDefeatedBossesCount()

        'Check non-respawning enemies killed
        nonRespawningEnemiesKilled = getKilledNonRespawningEnemiesCount()

        Dim totalNonRespawningEnemiesCount = totalNonRespawningEnemiesFlags.Length

        For Each npc In Dictionaries.npcHostileDeadFlags
            If GetEventFlagState(npc.GetValue(0)) = True Then
                'If NPC is just hostile, add him to total non-respawning enemies required
                totalNonRespawningEnemiesCount += 1
            ElseIf GetEventFlagState(npc.GetValue(1)) = True Then
                'If NPC is dead, add him to total non-respawning enemies required and mark him as killed
                totalNonRespawningEnemiesCount += 1
                nonRespawningEnemiesKilled += 1
            End If
        Next

        'Check whether all NPC questlines have been finished
        npcQuestlinesCompleted = getCompletedQuestlinesCount()

        'Check whether all shortcuts and locked doors have been unlocked
        shortcutsLockedDoorsUnlocked = getUnlockedShortcutsAndLockedDoorsCount()

        'Check whether all illusory walls have been revealed
        illusoryWallsRevealed = getRevealedIllusoryWallsCount()

        'Check whether all foggates have been dissolved
        foggatesDissolved = getDissolvedFoggatesCount()


        Dim percentage As Double

        Dim itemPercentage As Double = itemsPickedUp * (0.25 / totalItemsCount)
        Dim bossPercentage As Double = bossesKilled * (0.25 / totalBossFlags.Length)
        Dim nonrespawningPercentage As Double = nonRespawningEnemiesKilled * (0.15 / totalNonRespawningEnemiesCount)
        Dim questlinesPercentage As Double = npcQuestlinesCompleted * (0.2 / totalNPCQuestlineFlags.Length)
        Dim shortcutsLockedDoorsPercentage As Double = shortcutsLockedDoorsUnlocked * (0.1 / totalShortcutsLockedDoorsFlags.Length)
        Dim illusoryWallsPercentage As Double = illusoryWallsRevealed * (0.025 / totalIllusoryWallsFlags.Length)
        Dim foggatesPercentage As Double = foggatesDissolved * (0.025 / totalFoggatesFlags.Length)

        percentage = itemPercentage + bossPercentage + nonrespawningPercentage + questlinesPercentage + shortcutsLockedDoorsPercentage + illusoryWallsPercentage + foggatesPercentage
        percentage = Math.Floor(percentage * 100)

        'Before updating the UI, check if the player is in a loadscreen to make sure all flags have been accounted for
        If IsPlayerLoaded() = False Then
            Invoke(
                Sub()
                    refTimer.Start()
                End Sub)
            Return
        End If

        Invoke(
            Sub()
                treasureLocationsValueLabel.Text = $"{itemsPickedUp} / {totalItemsCount}"
                bossesKilledValueLabel.Text = $"{bossesKilled} / {totalBossFlags.Length}"
                nonRespawningEnemiesValueLabel.Text = $"{nonRespawningEnemiesKilled} / {totalNonRespawningEnemiesCount}"
                npcQuestlinesValueLabel.Text = $"{npcQuestlinesCompleted} / {totalNPCQuestlineFlags.Length}"
                shortcutsValueLabel.Text = $"{shortcutsLockedDoorsUnlocked} / {totalShortcutsLockedDoorsFlags.Length}"
                illusoryWallsValueLabel.Text = $"{illusoryWallsRevealed} / {totalIllusoryWallsFlags.Length}"
                foggatesValueLabel.Text = $"{foggatesDissolved} / {totalFoggatesFlags.Length}"

                percentageLabel.Text = $"{percentage}%"

                refTimer.Start()
            End Sub)
    End Sub

    Private Function getDissolvedFoggatesCount() As Integer
        Dim value As Boolean
        Dim count As Integer

        For Each item In totalFoggatesFlags
            value = GetEventFlagState(item)

            If value = True Then
                count += 1
            End If
        Next

        Return count

    End Function

    Private Function getDefeatedBossesCount() As Integer
        Dim value As Boolean
        Dim count As Integer

        For Each item In totalBossFlags
            value = GetEventFlagState(item)

            If value = True Then
                count += 1
            End If
        Next

        Return count

    End Function

    Private Function getRevealedIllusoryWallsCount() As Integer
        Dim value As Boolean
        Dim count As Integer

        For Each item In totalIllusoryWallsFlags
            value = GetEventFlagState(item)

            If value = True Then
                count += 1
            End If
        Next

        Return count

    End Function

    Private Function getUnlockedShortcutsAndLockedDoorsCount() As Integer
        Dim value As Boolean
        Dim count As Integer

        For Each item In totalShortcutsLockedDoorsFlags
            value = GetEventFlagState(item)

            If value = True Then
                count += 1
            End If
        Next

        Return count

    End Function

    Private Function getCompletedQuestlinesCount() As Integer
        Dim value As Boolean
        Dim count As Integer

        For Each item In totalNPCQuestlineFlags
            value = GetEventFlagState(item)

            If value = True Then
                count += 1
            ElseIf item = 1003 Then 'Solaire has two outcomes: dead or rescued in Izalith
                value = GetEventFlagState(1011)
                If value = True Then
                    count += 1
                End If
            ElseIf item = 1862 Then 'Ciaran can be disabled after giving her the soul, which uses another flag
                value = GetEventFlagState(1865)
                If value = True Then
                    count += 1
                End If
            End If
        Next

        Return count

    End Function

    Private Function getKilledNonRespawningEnemiesCount() As Integer
        Dim value As Boolean
        Dim count As Integer

        For Each item In totalNonRespawningEnemiesFlags
            If item = 11515080 Or item = 11515081 Then
                value = GetEventFlagState(11510400) 'Check for AL Gargoyles if it's Dark AL
                If value = True Then
                    count += 1
                    Continue For
                End If
            End If
            value = GetEventFlagState(item)

            If value = True Then
                count += 1
            End If
        Next

        Return count

    End Function

    Private Function isPlayerInOwnWorld() As Boolean
        Dim chrType = GetPlayerCharacterType()
        Return chrType = PlayerCharacterType.Hollow Or chrType = PlayerCharacterType.Human
    End Function


    Private Sub rehook()
        Invoke(
            Sub()
                refTimer.Stop()

                Dim newThread = New Thread(AddressOf RestartHook) With {.IsBackground = True}
                newThread.Start()

                refTimer.Start()
            End Sub)
    End Sub



    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        initHooks()

    End Sub

    Private Sub initHooks()

        rlsHooks.Add("geteventflagvalue", New IntPtr(&HD60340))
        rlsHooks.Add("hook1", New IntPtr(&HBC1CEA))
        rlsHooks.Add("hook1return", New IntPtr(&HBC1CEF))
        rlsHooks.Add("hook1seteventflag", New IntPtr(&HD38CB0))


        dbgHooks.Add("geteventflagvalue", New IntPtr(&HD618D0))
        dbgHooks.Add("hook1", New IntPtr(&HBC23CA))
        dbgHooks.Add("hook1return", New IntPtr(&HBC23CF))
        dbgHooks.Add("hook1seteventflag", New IntPtr(&HD3A240))

    End Sub

    Private Function WriteCodeAndFlushCache(ByVal hProcess As IntPtr, ByVal lpBaseAddress As IntPtr, ByVal lpBuffer() As Byte, ByVal iSize As Integer, ByVal lpNumberOfBytesWritten As Integer) As Boolean

        Dim result = WriteProcessMemory(hProcess, lpBaseAddress, lpBuffer, iSize, lpNumberOfBytesWritten)
        If Not FlushInstructionCache(hProcess, lpBaseAddress, iSize) Then
            unhook()
            Throw New Exception("Flush Instruction Cache Failed")
        End If

        Return result
    End Function

    Private Sub unhook()

        SetDarkSoulsThreadSuspend(True)

        isHooked = False

        refTimer.Stop()
        Dim refTimerTickEventHandler As New EventHandler(AddressOf refTimer_Tick)
        RemoveHandler refTimer.Tick, refTimerTickEventHandler

        VirtualFreeEx(_targetProcessHandle, hook1mem, 0, MEM_RELEASE)
        VirtualFreeEx(_targetProcessHandle, getflagfuncmem, 0, MEM_RELEASE)

        Dim tmpbytes() As Byte = {}

        If exeVER = "Release" Then
            tmpbytes = {&HE8, &HC1, &H6F, &H17, 0}
            WriteCodeAndFlushCache(_targetProcessHandle, hooks("hook1"), tmpbytes, 5, 0)
        End If
        If exeVER = "Debug" Then
            tmpbytes = {&HE8, &H71, &H7E, &H17, 0}
            WriteCodeAndFlushCache(_targetProcessHandle, hooks("hook1"), tmpbytes, 5, 0)
        End If

        SetDarkSoulsThreadSuspend(False)

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
        Console.WriteLine("restart")
        Thread.Sleep(50)
        Dim newThread = New Thread(AddressOf HookInOtherThread) With {.IsBackground = True}
        newThread.Start()
    End Sub

    Private Sub Form1_exit(sender As Object, e As EventArgs) Handles MyBase.Closing
        Dim newThread = New Thread(AddressOf unhook) With {.IsBackground = True}
        newThread.Start()
    End Sub

    Private Function GetEventFlagState(eventID As Integer) As Boolean
        WInt32(getflagfuncmem + &H400, eventID)
        Dim newThreadHook = CreateRemoteThread(_targetProcessHandle, 0, 0, getflagfuncmem, 0, 0, 0)
        WaitForSingleObject(newThreadHook, &HFFFFFFFFUI)
        CloseHandle(newThreadHook)
        Return Math.Floor(RInt32(getflagfuncmem + &H404) / (2 ^ 7)) = 1
    End Function

    Private Function IsPlayerLoaded() As Boolean
        Dim ptr = If(exeVER = "Debug", RInt32(&H1381E30), RInt32(&H137DC70))
        If ptr = 0 Then Return False
        ptr = RInt32(ptr + 4)
        Return ptr <> 0
    End Function

    Private Function GetPlayerStartingClass() As PlayerStartingClass
        Dim ptr = If(exeVER = "Debug", RInt32(&H137C8C0), RInt32(&H1378700))
        If ptr = 0 Then Return PlayerStartingClass.None
        ptr = RInt32(ptr + 8)
        If ptr = 0 Then Return PlayerStartingClass.None
        Return CType(RBytes(ptr + &HC6, 1)(0), PlayerStartingClass)
    End Function

    Private Function GetPlayerCharacterType() As PlayerCharacterType
        Dim ptr = If(exeVER = "Debug", RInt32(&H13823C4), RInt32(&H137E204))
        If ptr = 0 Then Return -1
        Return CType(RInt32(ptr + &HA28), PlayerCharacterType)
    End Function

    Private Sub ReadBitArray()
        Dim ptr = GetPlayerCharData1Ptr()
        If ptr = 0 Then Return

        Dim size = 4
        Dim bytes = RBytes(ptr + &H1FC, size)
        Dim bitArray As New BitArray(bytes)

        'Bits are stored in array in reverse order
        Dim indexToAccess = 11
        Dim reversedIndex = size * 8 - 1 - indexToAccess
        Console.WriteLine($"{bitArray(reversedIndex)}")
        'bitArray.Set(reversedIndex, True)
        'Dim newBytes(size) As Byte
        'bitArray.CopyTo(newBytes, 0)
        'WBytes(ptr + &H3C4, newBytes)

        'Console.WriteLine($"Length: {bitArray.Length}")
        'Dim int As Integer
        'For Each bit In bitArray
        'Console.WriteLine($"{int}: {bit}")
        'Int += 1
        'Next

    End Sub

    Private Function GetPlayerCharData1Ptr() As IntPtr
        Dim ptr = If(exeVER = "Debug", RInt32(&H1381804), RInt32(&H137D644))
        If ptr = 0 Then Return -1
        Return RInt32(ptr + &H3C) 'pointer to chardata1
    End Function

    Private Function GetCurrentArea() As Map
        Dim ptr = If(exeVER = "Debug", RInt32(&H13823C4), RInt32(&H137E204))
        If ptr = 0 Then Return -1

        Dim world = CType(RBytes(ptr + &HA13, 1)(0), Integer)
        Dim area = CType(RBytes(ptr + &HA12, 1)(0), Integer)

        Return CType(world * 10 + area, Map)
    End Function

    Private Function GetPressedButton() As Double
        Dim ptr = If(exeVER = "Debug", RInt32(&H1381048), RInt32(&H137CE88))
        If ptr = 0 Then Return -1
        ptr = RInt32(ptr + 4)
        If ptr = 0 Then Return -1
        Return RDouble(ptr + &H18C)
    End Function

    Function GetIngameTimeInMilliseconds() As Integer
        If exeVER = "Debug" Then
            Return RInt32(RInt32(&H137C8C0) + &H68)
        Else
            Return RInt32(RInt32(&H1378700) + &H68)
        End If
    End Function

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
End Class
