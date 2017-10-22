Imports GsmComm.GsmCommunication
Imports GsmComm.PduConverter
Imports System.IO
Imports System.Threading
Imports System.IO.Ports
Imports System.ComponentModel
Module SMSFunctions
    Public smsDeviceConnected As Boolean
    Public smsComm As GsmCommMain

    Public Function InitGSM(port As String) As Boolean
        smsComm = New GsmCommMain(port)
        Try
            smsComm.Open()
            While Not smsComm.IsConnected
                Debug.Print("Trying to connect...")
                smsComm.Close()
                Return False
            End While
            Debug.Print("Connected...")
            Return True
        Catch ex As Exception
            Dim errMessage = String.Format("Connection error: {0}", ex.Message)
            Debug.Print(errMessage)
            If smsComm.IsOpen Then
                smsComm.Close()
            End If
            Return False
        End Try
    End Function
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

    Public Function PortHasDevice(portName As String) As Boolean
        Dim port As New SerialPort(portName)
        'If port.IsOpen Then
        Dim comm As New GsmCommMain(portName)
        Dim info As IdentificationInfo
        Try
            comm.Open()
            info = comm.IdentifyDevice()
            comm.Close()
            Return InitGSM(portName)
        Catch ex As Exception
            Debug.Print("{0} has no device connected!", portName)
            If comm.IsOpen() Then
                comm.Close()
            End If
            Return False
        End Try
        'Else
        'Debug.Print("{0} is not available.", portName)
        'Return False
        'End If
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

    Public Function GetAllUnreadMessages() As List(Of SmsDeliverPdu)
        Dim rawUnreadMessages As New List(Of DecodedShortMessage)
        Dim unreadMessages As New List(Of SmsDeliverPdu)
        Try
            rawUnreadMessages = smsComm.ReadMessages(PhoneMessageStatus.ReceivedUnread, PhoneStorageType.Sim).ToList
            For Each message In rawUnreadMessages
                Dim data As SmsDeliverPdu = TryCast(message.Data, SmsDeliverPdu)

                Debug.Print("RECEIVED MESSAGE")
                Debug.Print("Sender: {0}", data.OriginatingAddress)
                Debug.Print("Sent: {0}", data.SCTimestamp.ToString)
                Debug.Print("Message: {0}", data.UserDataText)
                Debug.Print("Keyword: {0}", data.Keyword)
                Debug.Print("Actual Message: {0}", data.ActualMessage)
                Debug.Print("-------------------------------------------------------------------")

                unreadMessages.Add(data)
            Next

            If rawUnreadMessages.Count > 0 Then
                For i = rawUnreadMessages.Count - 1 To 0 Step -1
                    smsComm.DeleteMessage(i, PhoneStorageType.Sim)
                Next
                Debug.Print("{0} messages has been removed from the SIM", rawUnreadMessages.Count)
            End If
        Catch ex As Exception
            smsDeviceConnected = False
            Debug.Print("Device has been removed!")
        End Try
        Return unreadMessages
    End Function

    Public Function SendMessage(message As String, number As String)
        Dim newMessages As New List(Of SmsSubmitPdu)

        Dim newMessage As New SmsSubmitPdu(message, number)
        newMessages.Add(newMessage)

        Debug.Print("Attempting to save to outbox '{0}'", newMessage.UserDataText)
        Dim parameters As New Dictionary(Of String, String)
        parameters.Add("receiver", SQLInject(newMessage.DestinationAddress))
        parameters.Add("message", SQLInject(newMessage.UserDataText))
        parameters.Add("date_sent", SQLInject(Now.ToString))

        Dim sqlBuilder As New System.Text.StringBuilder
        sqlBuilder.AppendLine(String.Format("INSERT INTO tbl_outbox({0}) VALUES({1});",
            pList(parameters.Keys),
            pList(parameters.Values)))

        If ExecuteQuery(sqlBuilder.ToString) Then
            Debug.Print("Sending a message and stored to outbox")
        Else
            Debug.Print("Sending a message but failed to store to raw outbox")
        End If

        Try
            Debug.Print("Sending...")
            smsComm.SendMessages(newMessages.ToArray)

            Return True
        Catch ex As Exception
            Debug.Print("Failed to send!")
            Return False
        End Try
    End Function

    Public Function SendMessageToContacts(message As String, contacts As List(Of ContactInformation)) As Boolean
        Dim newMessages As New List(Of SmsSubmitPdu)
        For Each contact In contacts
            Dim newMessage As New SmsSubmitPdu(message, contact.contactNo)
            newMessages.Add(newMessage)

            Debug.Print("Attempting to save to outbox '{0}'", newMessage.UserDataText)
            Dim parameters As New Dictionary(Of String, String)
            parameters.Add("receiver", SQLInject(newMessage.DestinationAddress))
            parameters.Add("message", SQLInject(newMessage.UserDataText))
            parameters.Add("date_sent", SQLInject(Now.ToString))

            Dim sqlBuilder As New System.Text.StringBuilder
            sqlBuilder.AppendLine(String.Format("INSERT INTO tbl_outbox({0}) VALUES({1});",
                pList(parameters.Keys),
                pList(parameters.Values)))

           If ExecuteQuery(sqlBuilder.ToString) Then
                Debug.Print("Sending a message and stored to outbox")
            Else
                Debug.Print("Sending a message but failed to store to raw outbox")
            End If
        Next
        Try
            Debug.Print("Sending...")
            smsComm.SendMessages(newMessages.ToArray)
            Return True
        Catch ex As Exception
            Debug.Print("Failed to send!")
            Return False
        End Try
    End Function
    Public Function ConvertToCountryCodedNumber(mobileNumber As String) As String
        Dim newMobileNumber As String = mobileNumber
        Debug.Print("Original: {0}", mobileNumber)
        If newMobileNumber.StartsWith("09") Then
            Debug.Print("Starts with 09")
            newMobileNumber = newMobileNumber.Remove(0, 2)
            newMobileNumber = newMobileNumber.Insert(0, "+639")
        ElseIf newMobileNumber.StartsWith("639") Then
            Debug.Print("Starts with 639")
            newMobileNumber = newMobileNumber.Remove(0, 3)
            newMobileNumber = newMobileNumber.Insert(0, "+639")
        End If
        Debug.Print("Converted: {0}", newMobileNumber)
        Return newMobileNumber

    End Function
End Module

