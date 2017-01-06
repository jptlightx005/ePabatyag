Imports GsmComm.GsmCommunication
Imports GsmComm.PduConverter
Imports System.IO
Imports System.Threading
Imports System.IO.Ports
Module SMSFunctions
    Public smsDevicePort As String
    Public smsComm As GsmCommMain

    Public Sub InitGSM()
        If Not smsDevicePort = String.Empty Then
            smsComm = New GsmCommMain(smsDevicePort)
            Try
                smsComm.Open()
                While Not smsComm.IsConnected
                    Dim a = MsgBox("No device is connected on the current port.", vbRetryCancel + vbExclamation, "Connection Setup")
                    If a = vbCancel Then
                        smsDevicePort = ""
                        My.Settings.smsDevicePort = smsDevicePort
                        My.Settings.Save()
                        smsComm.Close()
                        Exit Sub
                    End If
                End While
            Catch ex As Exception
                Dim errMessage = String.Format("Connection error: {0}", ex.Message)
                MsgBox(errMessage, vbOKOnly + vbExclamation, "Connection Failed")
                smsComm.Close()
            End Try
        End If
    End Sub
    Public Function GetPorts() As List(Of String)
        ' Get a list of serial port names.
        Dim ports As List(Of String) = SerialPort.GetPortNames().ToList

        Debug.Print("The following serial ports were found:")

        ' Display each port name to the console.
        Dim port As String

        For Each port In ports
            Debug.Print(port)
        Next port

        Return ports
    End Function

    Public Function GetAllMessages() As List(Of DecodedShortMessage)
        Dim allMessages As New List(Of DecodedShortMessage)
        Try
            allMessages = smsComm.ReadMessages(PhoneMessageStatus.All, PhoneStorageType.Sim).ToList
            For Each message In allMessages
                Dim data As SmsDeliverPdu = TryCast(message.Data, SmsDeliverPdu)

                Debug.Print("RECEIVED MESSAGE")
                Debug.Print("Sender: {0}", data.OriginatingAddress)
                Debug.Print("Sent: {0}", data.SCTimestamp.ToString)
                Debug.Print("Message: {0}", data.UserDataText)
                Debug.Print("-------------------------------------------------------------------")

            Next
        Catch ex As Exception

        End Try
        Return allMessages
    End Function

    Public Function GetAllUnreadMessages() As List(Of DecodedShortMessage)
        Dim unreadMessages As New List(Of DecodedShortMessage)
        Try
            unreadMessages = smsComm.ReadMessages(PhoneMessageStatus.ReceivedUnread, PhoneStorageType.Sim).ToList
            For Each message In unreadMessages
                Dim data As SmsDeliverPdu = TryCast(message.Data, SmsDeliverPdu)

                Debug.Print("RECEIVED MESSAGE")
                Debug.Print("Sender: {0}", data.OriginatingAddress)
                Debug.Print("Sent: {0}", data.SCTimestamp.ToString)
                Debug.Print("Message: {0}", data.UserDataText)
                Debug.Print("-------------------------------------------------------------------")

            Next
        Catch ex As Exception

        End Try
        Return unreadMessages
    End Function

    Public Function ConvertToCountryCodedNumber(mobileNumber As String) As String
        Dim newMobileNumber As String = mobileNumber
        If newMobileNumber.StartsWith("09") And newMobileNumber.Length = 11 Then
            newMobileNumber.Remove(0, 2)
            newMobileNumber.Insert(0, "+639")
        ElseIf newMobileNumber.StartsWith("639") Then
            newMobileNumber.Remove(0, 3)
            newMobileNumber.Insert(0, "+639")
        End If
        Return newMobileNumber
    End Function
End Module

