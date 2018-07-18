Imports iLibrary
Imports Rohm.Common.Logging
'Imports System.Xml.Serialization
Imports System.IO
Imports System.Collections
Imports System.Runtime.Serialization.Formatters.Binary
Imports System.Runtime.Serialization

Public Class Form1
    Dim c_ApcsProiLibrary As IApcsProService
    Dim c_logger As Rohm.Common.Logging.Logger
    Public PathXmlObj As String = My.Application.Info.DirectoryPath
    Public DIR_LOG As String = My.Application.Info.DirectoryPath & "\LOG"
    Dim c_CellConLotdata As LotData

    Enum MachineState
        Initial
        idle
        Setup
        Ready
        Execute
        Pause
        LotSetUp
        Maintenance
    End Enum


    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles Me.Load
        c_ApcsProiLibrary = New ApcsProService
        c_logger = New Logger("VerTest", Me.Text)
        If (Not System.IO.Directory.Exists(PathXmlObj)) Then
            System.IO.Directory.CreateDirectory(PathXmlObj)
        End If
        If Not (Directory.Exists(DIR_LOG)) Then
            Directory.CreateDirectory(DIR_LOG)
        End If
    End Sub

    Public Sub WriteToXmlCellcon(ByVal FileName As String, ByVal TarObj As LotData)               '170207 \783 Cellcon Element

        'Dim XmlFile As FileStream = New FileStream(FileName, FileMode.Create)
        'Dim serialize As XmlSerializer = New XmlSerializer(TarObj.GetType)
        'serialize.Serialize(XmlFile, TarObj)
        'XmlFile.Close()


        Dim fs As New FileStream(FileName, FileMode.Create)

        ' Construct a BinaryFormatter and use it to serialize the data to the stream.
        Dim formatter As New BinaryFormatter
        Try
            formatter.Serialize(fs, TarObj)
        Catch e As SerializationException
            Console.WriteLine("Failed to serialize. Reason: " & e.Message)
            Throw
        Finally
            fs.Close()
        End Try

    End Sub

    Public Function ReadFromXmlCellcon(ByVal FileName As String) As LotData                      '170207 \783 Cellcon Element
        'Dim XmlFile As FileStream = New FileStream(FileName, FileMode.Open)
        'Dim serialize As XmlSerializer = New XmlSerializer(GetType(LotData))
        'ReadFromXmlCellcon = CType(serialize.Deserialize(XmlFile), LotData)
        'XmlFile.Close()
        'Return ReadFromXmlCellcon

        Dim fs As New FileStream(FileName, FileMode.Open)
        Try
            Dim formatter As New BinaryFormatter

            ' Deserialize the hashtable from the file and 
            ' assign the reference to the local variable.
            Return DirectCast(formatter.Deserialize(fs), LotData)
        Catch e As SerializationException
            Console.WriteLine("Failed to deserialize. Reason: " & e.Message)
            Throw
        Finally
            fs.Close()
        End Try

    End Function
    Public Sub SaveCatchLog(ByVal message As String, ByVal fnName As String)

        Using sw As StreamWriter = New StreamWriter(Path.Combine(DIR_LOG, "Catch_" & Now.ToString("yyyyMMdd") & ".log"), True)
            sw.WriteLine(Now.ToString("yyyy/MM/dd HH:mm:ss.fff") & " " & fnName & ">" & message)
        End Using
    End Sub

    Private Sub btnLotSetUp_Click(sender As Object, e As EventArgs) Handles btnLotSetUp.Click
        Dim c_CellConLotdata As New LotData
        Try
            lbMessage.Text = ""
            c_CellConLotdata.Lotno = tbxLotNo.Text.Trim
            c_CellConLotdata.UserInfo_SetUp = c_ApcsProiLibrary.GetUserInfo(tbxOPNO.Text.Trim)
            c_CellConLotdata.MachineNo = Me.Text
            c_CellConLotdata.MachineInfo = c_ApcsProiLibrary.GetMachineInfo(Me.Text)
            c_CellConLotdata.LotInfo = c_ApcsProiLibrary.GetLotInfo(c_CellConLotdata.Lotno)

            Dim chkMachine As CheckMachineConditionResult = c_ApcsProiLibrary.CheckMachineCondition(c_CellConLotdata.MachineInfo)
            If chkMachine.IsPass Then
                lbMessage.Text += "CheckMachineResult :  True " & vbCrLf
                c_CellConLotdata.CheckMachineResult = "True : " & Format(Now, "yyyy/MM/dd HH:mm:ss ")
            Else
                MsgBox(chkMachine.ErrorMessage)
                lbMessage.Text += "CheckMachineResult :  False " & vbCrLf
                c_CellConLotdata.CheckMachineResult = "False : " & Format(Now, "yyyy/MM/dd HH:mm:ss ") & chkMachine.ErrorMessage & " : At step : " & chkMachine.ErrorAtStep
                GoTo WriteToXml
            End If

            tbxAppName.Text = "CellController"
            tbxFunctionName.Text = "DB-SetupLot"

            Dim chkUserP As CheckUserPermissionResult = c_ApcsProiLibrary.CheckUserPermission(c_CellConLotdata.UserInfo_SetUp, tbxAppName.Text.Trim, tbxFunctionName.Text.Trim)
            If chkUserP.IsPass Then
                c_CellConLotdata.CheckUserPermissionResult = "True : " & Format(Now, "yyyy/MM/dd HH:mm:ss ")
            Else
                lbMessage.Text += "CheckUserPermissionResult :  False " & vbCrLf
                c_CellConLotdata.CheckUserPermissionResult = "False : " & Format(Now, "yyyy/MM/dd HH:mm:ss ") & chkUserP.ErrorMessage
                MsgBox(chkUserP.ErrorMessage)
                GoTo WriteToXml
            End If

            If c_ApcsProiLibrary.Check_PermissionMachinesByLMS(c_CellConLotdata.UserInfo_SetUp.Id, c_CellConLotdata.MachineInfo.Name, c_logger) Then
                c_CellConLotdata.CheckUserLicenseResult = "True : " & Format(Now, "yyyy/MM/dd HH:mm:ss ")
            Else
                c_CellConLotdata.CheckUserLicenseResult = "False : " & Format(Now, "yyyy/MM/dd HH:mm:ss ")
                lbMessage.Text += "CheckUserLicenseResult :  False " & vbCrLf

                'GoTo WriteToXml
            End If

            c_CellConLotdata.CheckUserLicenseResult = "True : " & Format(Now, "yyyy/MM/dd HH:mm:ss ")
            Dim chkLot As CheckLotConditionResult = c_ApcsProiLibrary.CheckLotCondition(c_CellConLotdata.LotInfo)
            If chkLot.IsPass Then
                lbMessage.Text += "CheckLotConditionResult :  True " & vbCrLf
                c_CellConLotdata.CheckLotConditionResult = "True : " & Format(Now, "yyyy/MM/dd HH:mm:ss ")
            Else
                MsgBox(chkLot.ErrorMessage)
                lbMessage.Text += "CheckLotConditionResult :  False " & vbCrLf
                c_CellConLotdata.CheckLotConditionResult = "False : " & Format(Now, "yyyy/MM/dd HH:mm:ss ") & chkLot.ErrorMessage & " : At step : " & chkLot.ErrorAtStep
                GoTo WriteToXml
            End If
            Dim currentServerTime As DateTimeInfo = Nothing
            currentServerTime = c_ApcsProiLibrary.Get_DateTimeInfo(c_logger)
            c_CellConLotdata.LotUpdateInfoLotSetUp = c_ApcsProiLibrary.LotSetup(c_CellConLotdata.LotInfo.Id, c_CellConLotdata.MachineInfo.Id, c_CellConLotdata.UserInfo_SetUp.Id, 0, "", 1, currentServerTime.Datetime, c_logger)
            c_ApcsProiLibrary.Update_MachineState(c_CellConLotdata.MachineInfo.Id, MachineState.LotSetUp, c_CellConLotdata.UserInfo_SetUp.Id, c_logger)
            If c_CellConLotdata.LotUpdateInfoLotSetUp.IsOk Then
                lbMessage.Text += "LotSetup :  True " & vbCrLf
                c_CellConLotdata.RecipeMDM = c_CellConLotdata.LotUpdateInfoLotSetUp.Recipe.Name
            Else
                lbMessage.Text += "LotSetup :  False " & vbCrLf
                MsgBox(c_CellConLotdata.LotUpdateInfoLotSetUp.ErrorMessage)
            End If
        Catch ex As Exception
            SaveCatchLog(ex.ToString, "btnLotSetUp_Click")
        End Try

WriteToXml:
        WriteToXmlCellcon(PathXmlObj & "\" & c_CellConLotdata.Lotno & ".xml", c_CellConLotdata)
    End Sub

    Private Sub btnReload_Click(sender As Object, e As EventArgs) Handles btnReload.Click
        Dim fileName As String = InputBox("Please input filename", "Reload")
        If fileName = "" Then
            Exit Sub
        End If
        c_CellConLotdata = ReadFromXmlCellcon(PathXmlObj & "\" & fileName & ".xml")
        tbxLotNo.Text = c_CellConLotdata.Lotno
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        c_ApcsProiLibrary.CheckPackageEnable("SSOP-B28W", c_logger)
    End Sub
    '0:Initial  1:idle 2:Setup 3:Ready 4:Execute 5:Pause 6:LotSetUp 7:Maintenance
    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click

        c_CellConLotdata.LotUpdateInfoLotStart = c_ApcsProiLibrary.LotStart(c_CellConLotdata.LotInfo.Id, c_CellConLotdata.MachineInfo.Id, c_CellConLotdata.UserInfo_SetUp.Id, 0, "", 4, c_CellConLotdata.RecipeMDM, c_logger)
        c_ApcsProiLibrary.Update_MachineState(c_CellConLotdata.MachineInfo.Id, MachineState.Execute, c_CellConLotdata.UserInfo_SetUp.Id, c_logger)

        If c_CellConLotdata.LotUpdateInfoLotStart.IsOk Then
            lbMessage.Text += "LotStart :  True " & vbCrLf
        Else
            lbMessage.Text += "LotStart :  False " & vbCrLf
            MsgBox(c_CellConLotdata.LotUpdateInfoLotStart, MsgBoxStyle.OkOnly, "Error No : " & c_CellConLotdata.LotUpdateInfoLotStart.ErrorNo)
        End If




    End Sub

    Private Sub btnLotEnd_Click(sender As Object, e As EventArgs) Handles btnLotEnd.Click
        Dim currentServerTime As DateTimeInfo = Nothing
        currentServerTime = c_ApcsProiLibrary.Get_DateTimeInfo(c_logger)
        Dim lotUpdate As New LotUpdateInfo
        lotUpdate = c_ApcsProiLibrary.Update_Finalinspection(c_CellConLotdata.LotInfo.Id, 1, c_CellConLotdata.UserInfo_SetUp.Id, 0, "", 1, currentServerTime.Datetime, c_logger)
        c_CellConLotdata.LotUpdateInfoLotEnd = c_ApcsProiLibrary.LotEnd(c_CellConLotdata.LotInfo.Id, c_CellConLotdata.MachineInfo.Id, c_CellConLotdata.UserInfo_SetUp.Id, 0, 100, 10, 0, "", 1, currentServerTime.Datetime, c_logger)
        c_ApcsProiLibrary.Update_MachineState(c_CellConLotdata.MachineInfo.Id, MachineState.idle, c_CellConLotdata.UserInfo_SetUp.Id, c_logger)



    End Sub

    Private Sub btnFirstIns_Click(sender As Object, e As EventArgs) Handles btnFirstIns.Click
        Dim currentServerTime As DateTimeInfo = Nothing
        currentServerTime = c_ApcsProiLibrary.Get_DateTimeInfo(c_logger)
        Dim lotUpdate As New LotUpdateInfo
        lotUpdate = c_ApcsProiLibrary.Update_Firstinspection(c_CellConLotdata.LotInfo.Id, 1, c_CellConLotdata.UserInfo_SetUp.Id, 0, "", 1, currentServerTime.Datetime, c_logger)

        If lotUpdate.IsOk Then
            lbMessage.Text += "Firstinspection :  True " & vbCrLf
            c_CellConLotdata.FirstIns = "True : " & Format(Now, "yyyy/MM/dd HH:mm:ss ")
        Else
            lbMessage.Text += "Firstinspection :  False " & vbCrLf
            c_CellConLotdata.FirstIns = "False : " & Format(Now, "yyyy/MM/dd HH:mm:ss ") & lotUpdate.ErrorMessage
            MsgBox(lotUpdate.ErrorMessage, MsgBoxStyle.OkOnly, "Error No : " & lotUpdate.ErrorNo)
        End If


    End Sub

    Private Sub btnLotCancel_Click(sender As Object, e As EventArgs) Handles btnLotCancel.Click
        c_ApcsProiLibrary.LotCancel(c_CellConLotdata.LotInfo.Id, c_CellConLotdata.MachineInfo.Id, c_CellConLotdata.UserInfo_SetUp.Id, 1, c_logger)
    End Sub
End Class
