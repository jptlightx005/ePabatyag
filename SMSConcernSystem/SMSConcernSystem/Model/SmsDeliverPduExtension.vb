﻿Imports System.Runtime.CompilerServices
Imports GsmComm.PduConverter
Module SmsDeliverPduExtension

    <Extension()>
    Public Function Keyword(ByVal message As SmsDeliverPdu) As String
        Dim msgKeyword As String = ""
        If message.UserDataText.Length > 0 Then
            Dim words = Split(message.UserDataText)
            If words.Length > 0 Then
                msgKeyword = words.First
            End If
        End If
        Return msgKeyword
    End Function

    <Extension()>
    Public Function ActualMessage(ByVal message As SmsDeliverPdu) As String
        Dim msg = ""
        If message.UserDataText.Length > 0 Then
            Dim words = Split(message.UserDataText)
            If words.Length > 0 Then
                words.ToList.RemoveAt(0)
                msg = Join(words)
            End If
        End If
        Return msg
    End Function
End Module