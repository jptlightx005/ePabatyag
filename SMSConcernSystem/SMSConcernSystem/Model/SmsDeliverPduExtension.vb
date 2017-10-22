Imports System.Runtime.CompilerServices
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
    Public Function Ratings(ByVal message As SmsDeliverPdu) As Rating
        Dim msgRatings As New Rating
        If message.UserDataText.Length > 0 Then

            Dim words = Split(message.UserDataText)

            msgRatings.QualityOfService = 0
            msgRatings.TimelinessOfService = 0
            msgRatings.ProfesionalismOfPersonel = 0

            If words.Length > 1 Then
                Dim secondKeyword = words(1)
                If secondKeyword(0).ToString.ToLower = "q" Then
                    Dim qRate As Integer = Integer.Parse(secondKeyword(1))
                    If qRate > 0 And qRate <= 5 Then
                        msgRatings.QualityOfService = qRate
                    End If
                End If
            End If
            If words.Length > 2 Then
                Dim thirdKeyword = words(2)
                If thirdKeyword(0).ToString.ToLower = "t" Then
                    Dim tRate As Integer = Integer.Parse(thirdKeyword(1))
                    If tRate > 0 And tRate <= 5 Then
                        msgRatings.TimelinessOfService = tRate
                    End If
                End If
            End If

            If words.Length > 3 Then
                Dim fourthKeyword = words(3)
                If fourthKeyword(0).ToString.ToLower = "p" Then
                    Dim pRate As Integer = Integer.Parse(fourthKeyword(1))
                    If pRate > 0 And pRate <= 5 Then
                        msgRatings.ProfesionalismOfPersonel = pRate
                    End If
                End If
            End If
        End If
        Return msgRatings
    End Function

    <Extension()>
    Public Function MessagePos(ByVal message As SmsDeliverPdu) As Integer
        Dim pos As Integer = message.Keyword.Length
        If message.UserDataText.Length > 0 Then
            pos = message.Keyword.Length

            Dim words = Split(message.UserDataText)
            If words.Length > 4 Then
                Dim secondKeyword = words(1)
                If secondKeyword(0).ToString.ToLower = "q" Then
                    pos = message.UserDataText.IndexOf(secondKeyword) + secondKeyword.Length
                End If

                Dim thirdKeyword = words(2)
                If thirdKeyword(0).ToString.ToLower = "t" Then
                    pos = message.UserDataText.IndexOf(thirdKeyword) + thirdKeyword.Length
                End If
                Dim fourthKeyword = words(3)
                If fourthKeyword(0).ToString.ToLower = "p" Then
                    pos = message.UserDataText.IndexOf(fourthKeyword) + fourthKeyword.Length
                End If
            End If
        End If
        Return pos
    End Function
    <Extension()>
    Public Function ActualMessage(ByVal message As SmsDeliverPdu) As String
        Dim msg = ""
        If message.UserDataText.Length > 0 Then
            msg = message.UserDataText.Substring(message.MessagePos, message.UserDataText.Length - message.MessagePos)
            msg = msg.Trim()
        End If
        Return msg
    End Function
End Module
