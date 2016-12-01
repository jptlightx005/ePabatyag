Public Class ViewMessageWindow
    Public selectedMessage As Dictionary(Of String, String)

    Private Sub Window_Loaded(sender As Object, e As RoutedEventArgs)
        txtSender.Text = String.Format("{0} {1} ({2})", selectedMessage("first_name"), selectedMessage("last_name"), selectedMessage("mobile_number"))
        txtMessage.Text = selectedMessage("message_content")
    End Sub

    Private Sub txtSender_MouseUp(sender As Object, e As MouseButtonEventArgs) Handles txtSender.MouseUp
       
    End Sub

    Private Sub txtSender_MouseLeftButtonUp(sender As Object, e As MouseButtonEventArgs) Handles txtSender.MouseLeftButtonUp
       
    End Sub

    Private Sub txtSender_PreviewMouseUp(sender As Object, e As MouseButtonEventArgs) Handles txtSender.PreviewMouseUp
        Debug.Print("did click")
        Dim query As String = String.Format("SELECT * FROM tbl_contacts WHERE ID = {0}", selectedMessage("studentID"))
        SelectQuery(query, Sub(result)
                               If result.Count > 0 Then
                                   Dim regWindow As New RegistrationWindow
                                   Dim student = result.First
                                   Dim studentInfo As New ContactInformation
                                   studentInfo.ID = student("ID")
                                   studentInfo.studentID = student("student_id")
                                   studentInfo.contactNo = student("mobile_number")
                                   studentInfo.firstName = student("first_name")
                                   studentInfo.lastName = student("last_name")
                                   studentInfo.course = student("course")
                                   studentInfo.yearSection = student("year_section")
                                   studentInfo.gender = student("gender")
                                   If Not student("date_of_birth") = "" Then
                                       studentInfo.dateOfBirth = Date.Parse(student("date_of_birth"))
                                   End If
                                   studentInfo.address = student("address")
                                   studentInfo.email = student("email")

                                   Dim imageSource As String = System.IO.Path.Combine(smsSystemImages, String.Format("contact-image-{0}.jpg", studentInfo.ID))
                                   If (System.IO.File.Exists(imageSource)) Then
                                       studentInfo.contactImageSource = imageSource
                                   End If

                                   regWindow.contactInfo = studentInfo

                                   regWindow.isUpdating = True
                                   regWindow.parentForm = Me
                                   regWindow.ShowDialog()
                               Else
                                   MsgBox("ERROR! STUDENT NOT FOUND!", vbOK + vbExclamation)
                               End If
                           End Sub)
    End Sub
End Class
