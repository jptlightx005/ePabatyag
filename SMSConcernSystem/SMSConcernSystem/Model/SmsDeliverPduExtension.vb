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
            If words.Length > 0 Then
                Dim qualityRating = words(1)
                Dim timelinessRating = words(2)
                Dim professionalismRating = words(3)

                If qualityRating.Length = 2 Then
                    If qualityRating(0).ToString.ToLower = "q" Then
                        Dim qRate As Integer = Integer.Parse(qualityRating(1))
                        If qRate > 0 And qRate <= 5 Then
                            msgRatings.QualityOfService = qRate
                        Else
                            Return Nothing
                        End If
                    Else
                        Return Nothing
                    End If
                Else
                    Return Nothing
                End If

                If timelinessRating.Length = 2 Then
                    If timelinessRating(0).ToString.ToLower = "t" Then
                        Dim tRate As Integer = Integer.Parse(timelinessRating(1))
                        If tRate > 0 And tRate <= 5 Then
                            msgRatings.TimelinessOfService = tRate
                        Else
                            Return Nothing
                        End If
                    Else
                        Return Nothing
                    End If
                Else
                    Return Nothing
                End If

                If professionalismRating.Length = 2 Then
                    If professionalismRating(0).ToString.ToLower = "p" Then
                        Dim pRate As Integer = Integer.Parse(professionalismRating(1))
                        If pRate > 0 And pRate <= 5 Then
                            msgRatings.ProfesionalismOfPersonel = pRate
                        Else
                            Return Nothing
                        End If
                    Else
                        Return Nothing
                    End If
                Else
                    Return Nothing
                End If
            End If
        End If
        Return msgRatings
    End Function

    <Extension()>
    Public Function ActualMessage(ByVal message As SmsDeliverPdu) As String
        Dim msg = ""
        Dim hasRatings As Boolean = Not (message.Ratings Is Nothing)
        If message.UserDataText.Length > 0 Then
            Dim keywordsLength As Integer = message.Keyword.Length + IIf(hasRatings, 9, 0)
            msg = message.UserDataText.Substring(keywordsLength, message.UserDataText.Length - keywordsLength)
            msg = msg.Trim()
        End If
        Return msg
    End Function
End Module
