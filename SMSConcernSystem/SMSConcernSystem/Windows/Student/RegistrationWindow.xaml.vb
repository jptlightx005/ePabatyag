Imports System.Data.SQLite
Imports Microsoft.Win32
Imports System.Windows.Media.Imaging
Imports System.IO
Public Class RegistrationWindow
    Public contactInfo As ContactInformation
    Public isUpdating As Boolean
    Public parentForm As Window

    Dim didChange As Boolean
    Dim defaultImageSource As ImageSource
    Private Sub btnClose_Click(sender As Object, e As RoutedEventArgs) Handles btnClose.Click
        didChange = False
        Me.Close()
    End Sub

    Private Sub btnRegister_Click(sender As Object, e As RoutedEventArgs) Handles btnRegister.Click
        If ValidateData() Then
            If Not isUpdating Then
                RegisterNewContact()
            Else
                UpdateContact()
            End If
        Else
            MsgBox("Please enter all required data!", vbExclamation)
        End If
    End Sub
    Sub RegisterNewContact()
        Dim parameters As New Dictionary(Of String, String)
        parameters.Add("student_id", SQLInject(txtStudentID.Text))
        parameters.Add("mobile_number", SQLInject(txtContact.Text))
        parameters.Add("first_name", SQLInject(txtFirstName.Text))
        parameters.Add("last_name", SQLInject(txtLastName.Text))
        parameters.Add("course", SQLInject(txtCourse.Text))
        parameters.Add("year_section", SQLInject(txtYearSection.Text))
        parameters.Add("gender", SQLInject(IIf(cmbGender.SelectedIndex >= 0, cmbGender.Text, "")))
        parameters.Add("date_of_birth", SQLInject(IIf(dtBirth.SelectedDate.HasValue, Format(dtBirth.SelectedDate, "yyyy-MM-dd"), "")))
        parameters.Add("address", SQLInject(txtAddress.Text))
        parameters.Add("email", SQLInject(txtEmail.Text))
        parameters.Add("date_registered", SQLInject(Format(Now, "yyyy-MM-dd HH:mm:ss")))

        Dim sqlBuilder As New System.Text.StringBuilder
        sqlBuilder.AppendLine(String.Format("INSERT INTO tbl_contacts({0}) VALUES({1});",
            pList(parameters.Keys),
            pList(parameters.Values)))
        sqlBuilder.Append("SELECT last_insert_rowid();")

        SelectQuery(sqlBuilder.ToString, Sub(result)
                                             If result.Count > 0 Then
                                                 Dim a As Dictionary(Of String, String) = result.First
                                                 Debug.Print(a("last_insert_rowid()"))
                                                 MsgBox("Student succesfully registered!", vbInformation)
                                                 If Not imgContactImage.Source Is defaultImageSource Then
                                                     contactInfo.contactImageSource = System.IO.Path.Combine(smsSystemImages, String.Format("contact-image-{0}.jpg", a("last_insert_rowid()")))
                                                     Dim encoder As New PngBitmapEncoder()
                                                     encoder.Frames.Add(BitmapFrame.Create(CType(imgContactImage.Source, BitmapSource)))
                                                     Using stream As New FileStream(contactInfo.contactImageSource, FileMode.Create)
                                                         encoder.Save(stream)
                                                     End Using
                                                 End If
                                                 didChange = True
                                                 Me.Close()
                                             Else
                                                 MsgBox("Failed to register!", vbExclamation)
                                             End If
                                         End Sub)
    End Sub

    Sub UpdateContact()
        Dim parameters As New Dictionary(Of String, String)
        parameters.Add("student_id", SQLInject(txtStudentID.Text))
        parameters.Add("mobile_number", SQLInject(txtContact.Text))
        parameters.Add("first_name", SQLInject(txtFirstName.Text))
        parameters.Add("last_name", SQLInject(txtLastName.Text))
        parameters.Add("course", SQLInject(txtCourse.Text))
        parameters.Add("year_section", SQLInject(txtYearSection.Text))
        parameters.Add("gender", SQLInject(IIf(cmbGender.SelectedIndex >= 0, cmbGender.Text, "")))
        parameters.Add("date_of_birth", SQLInject(IIf(dtBirth.SelectedDate.HasValue, Format(dtBirth.SelectedDate, "yyyy-MM-dd"), "")))
        parameters.Add("address", SQLInject(txtAddress.Text))
        parameters.Add("email", SQLInject(txtEmail.Text))

        Dim setQuery As String = ""
        For Each b As KeyValuePair(Of String, String) In parameters
            setQuery = setQuery + String.Format("`{0}` = {1}, ", b.Key, b.Value)
        Next
        setQuery = setQuery.Substring(0, setQuery.Length - 2)
        Dim registerQuery As String = String.Format("UPDATE tbl_contacts SET {0} WHERE ID = {1}",
            setQuery,
            contactInfo.ID)

        ExecuteQuery(registerQuery, Sub(success)
                                        If (success) Then
                                            MsgBox("Contact succesfully updated!", vbInformation)
                                            If Not imgContactImage.Source Is defaultImageSource Then
                                                contactInfo.contactImageSource = System.IO.Path.Combine(smsSystemImages, String.Format("contact-image-{0}.jpg", contactInfo.ID))
                                                Dim encoder As New PngBitmapEncoder()
                                                encoder.Frames.Add(BitmapFrame.Create(CType(imgContactImage.Source, BitmapSource)))
                                                Using stream As New FileStream(contactInfo.contactImageSource, FileMode.Create)
                                                    encoder.Save(stream)
                                                End Using
                                            End If
                                            didChange = True
                                            Me.Close()
                                        Else
                                            MsgBox("Failed to update!", vbExclamation)
                                        End If
                                    End Sub)
    End Sub
    Function ValidateData() As Boolean
        Dim isValid As Boolean = True
        isValid = isValid And contactInfo.studentID.Length > 0
        isValid = isValid And contactInfo.contactNo.Length > 0
        isValid = isValid And contactInfo.firstName.Length > 0
        isValid = isValid And contactInfo.lastName.Length > 0
        isValid = isValid And contactInfo.course.Length > 0
        isValid = isValid And contactInfo.yearSection.Length > 0
        Return isValid
    End Function

    Private Sub Grid_Loaded(sender As Object, e As RoutedEventArgs)
        If Not isUpdating Then
            contactInfo = New ContactInformation
        Else
            txtStudentID.Text = contactInfo.studentID
            txtContact.Text = contactInfo.contactNo
            txtFirstName.Text = contactInfo.firstName
            txtLastName.Text = contactInfo.lastName
            txtCourse.Text = contactInfo.course
            txtYearSection.Text = contactInfo.yearSection
            cmbGender.Text = contactInfo.gender
            If Not contactInfo.dateOfBirth Is Nothing Then
                dtBirth.SelectedDate = contactInfo.dateOfBirth
            End If
            txtAddress.Text = contactInfo.address
            txtEmail.Text = contactInfo.email

            If Not contactInfo.contactImageSource = String.Empty Then
                Debug.Print("meh ""{0}""", contactInfo.contactImageSource)
                imgContactImage.Source = New BitmapImage(New Uri(contactInfo.contactImageSource))
            End If

            btnRegister.Content = "Update"
            Me.Title = "Update Student"
        End If
        defaultImageSource = imgContactImage.Source
    End Sub

    Private Sub Window_Closing(sender As Object, e As ComponentModel.CancelEventArgs)
        If isUpdating And didChange Then
            If TypeOf parentForm Is StudentListWindow Then
                CType(parentForm, StudentListWindow).UpdateTableView()
            End If
        End If
    End Sub

    Private Sub imgContactImage_MouseUp(sender As Object, e As MouseButtonEventArgs) Handles imgContactImage.MouseUp
        Debug.Print("Did click meh")
        Dim fileDialog As New OpenFileDialog
        fileDialog.InitialDirectory = myDocumentsFolder
        fileDialog.Filter = "Image files (*.png;*.jpg)|*.png;*.jpg|All files (*.*)|*.*"
        If fileDialog.ShowDialog Then
            imgContactImage.Source = New BitmapImage(New Uri(fileDialog.FileName))
        End If
    End Sub

    Private Sub textBox_LostFocus(sender As Object, e As RoutedEventArgs) Handles txtStudentID.LostFocus, txtContact.LostFocus, txtFirstName.LostFocus, txtLastName.LostFocus, txtCourse.LostFocus, txtYearSection.LostFocus, txtAddress.LostFocus, txtEmail.LostFocus
        If sender Is txtStudentID Then
            contactInfo.studentID = AllTrim(txtStudentID.Text)
            Debug.Print(contactInfo.studentID)

        ElseIf sender Is txtContact Then
            contactInfo.contactNo = AllTrim(txtContact.Text)
            Debug.Print(contactInfo.contactNo)

        ElseIf sender Is txtFirstName Then
            contactInfo.firstName = AllTrim(txtFirstName.Text)
            Debug.Print(contactInfo.firstName)

        ElseIf sender Is txtLastName Then
            contactInfo.lastName = AllTrim(txtLastName.Text)
            Debug.Print(contactInfo.lastName)

        ElseIf sender Is txtCourse Then
            contactInfo.course = AllTrim(txtCourse.Text)
            Debug.Print(contactInfo.course)

        ElseIf sender Is txtYearSection Then
            contactInfo.yearSection = AllTrim(txtYearSection.Text)
            Debug.Print(contactInfo.yearSection)

        ElseIf sender Is txtAddress Then
            contactInfo.address = AllTrim(txtAddress.Text)
            Debug.Print(contactInfo.address)

        ElseIf sender Is txtStudentID Then
            contactInfo.email = AllTrim(txtEmail.Text)
            Debug.Print(contactInfo.email)

        End If
    End Sub

    Private Sub cmbGender_SelectionChanged(sender As Object, e As SelectionChangedEventArgs) Handles cmbGender.SelectionChanged
        contactInfo.gender = CType(cmbGender.SelectedItem, ComboBoxItem).Content
        Debug.Print(contactInfo.gender)
    End Sub

    Private Sub dtBirth_SelectedDateChanged(sender As Object, e As SelectionChangedEventArgs) Handles dtBirth.SelectedDateChanged
        contactInfo.dateOfBirth = dtBirth.SelectedDate
        Debug.Print(contactInfo.dateOfBirth)
    End Sub

    Private Sub imgContactImage_SourceUpdated(sender As Object, e As DataTransferEventArgs) Handles imgContactImage.SourceUpdated
        
    End Sub

    Private Sub imgContactImage_TargetUpdated(sender As Object, e As DataTransferEventArgs) Handles imgContactImage.TargetUpdated

    End Sub
End Class
