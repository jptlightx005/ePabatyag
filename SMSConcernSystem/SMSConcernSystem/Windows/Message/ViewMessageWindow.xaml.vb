Public Class ViewMessageWindow
    Public selectedMessage As Dictionary(Of String, String)

    Private Sub Window_Loaded(sender As Object, e As RoutedEventArgs)
        txtSender.Text = selectedMessage("mobile_number")
        txtMessage.Text = selectedMessage("message_content")
    End Sub

    Private Sub txtSender_MouseUp(sender As Object, e As MouseButtonEventArgs) Handles txtSender.MouseUp
       
    End Sub

    Private Sub txtSender_MouseLeftButtonUp(sender As Object, e As MouseButtonEventArgs) Handles txtSender.MouseLeftButtonUp
       
    End Sub

    Private Sub txtSender_PreviewMouseUp(sender As Object, e As MouseButtonEventArgs) Handles txtSender.PreviewMouseUp
        Debug.Print("did click")
        'Dim query As String = String.Format("SELECT * FROM tbl_contacts WHERE ID = {0}", selectedMessage("studentID"))
        'LookUpStudent(Sub(contact)
        '                  Dim regWindow As New RegistrationWindow
        '                  Dim imageSource As String = System.IO.Path.Combine(smsSystemImages, String.Format("contact-image-{0}.jpg", contact.ID))

        '                  If (System.IO.File.Exists(imageSource)) Then
        '                      contact.contactImageSource = imageSource
        '                  End If

        '                  regWindow.contactInfo = contact

        '                  regWindow.isUpdating = True
        '                  regWindow.parentForm = Me
        '                  regWindow.ShowDialog()
        '              End Sub)
    End Sub

    Private Sub btnRespond_Click(sender As Object, e As RoutedEventArgs) Handles btnRespond.Click


        Debug.Print("did click")
        Dim recipients As New List(Of ContactInformation)

        Dim recipientInfo As New ContactInformation

        recipientInfo.studentID = ""
        recipientInfo.contactNo = selectedMessage("mobile_number")
        recipientInfo.firstName = ""
        recipientInfo.lastName = ""
        recipientInfo.course = ""
        recipientInfo.yearSection = ""
        recipientInfo.gender = ""
        recipientInfo.address = ""
        recipientInfo.email = ""

        recipients.Add(recipientInfo)

        Dim composeMessageWindow As New ComposeMessageWindow
        composeMessageWindow.recipients = recipients
        composeMessageWindow.ShowDialog()
        'LookUpStudent(Sub(contact)
        '                  Dim recipients As New List(Of ContactInformation)
        '                  recipients.Add(contact)
        '                  Dim composeMessageWindow As New ComposeMessageWindow
        '                  composeMessageWindow.recipients = recipients
        '                  composeMessageWindow.ShowDialog()
        '              End Sub)
    End Sub

    Private Sub LookUpStudent(completionBlock As Action(Of ContactInformation))
        'Dim query As String = String.Format("SELECT * FROM tbl_contacts WHERE ID = {0}", selectedMessage("studentID"))

        'Dim result = SelectQuery(query)
        'If result.Count > 0 Then
        '    Dim student = result.First
        '    Dim studentInfo As New ContactInformation
        '    studentInfo.ID = student("ID")
        '    studentInfo.studentID = student("student_id")
        '    studentInfo.contactNo = student("mobile_number")
        '    studentInfo.firstName = student("first_name")
        '    studentInfo.lastName = student("last_name")
        '    studentInfo.course = student("course")
        '    studentInfo.yearSection = student("year_section")
        '    studentInfo.gender = student("gender")
        '    If Not student("date_of_birth") = "" Then
        '        studentInfo.dateOfBirth = Date.Parse(student("date_of_birth"))
        '    End If
        '    studentInfo.address = student("address")
        '    studentInfo.email = student("email")

        '    completionBlock(studentInfo)
        'Else
        '    MsgBox("ERROR! STUDENT NOT FOUND!", vbOK + vbExclamation)
        'End If
    End Sub

    Private Sub btnCancel_Click(sender As Object, e As RoutedEventArgs) Handles btnCancel.Click
        Me.Close()
    End Sub
End Class
