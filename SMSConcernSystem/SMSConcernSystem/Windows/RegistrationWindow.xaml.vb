Imports System.Data.SQLite
Public Class RegistrationWindow
    Public selectedStudent As Dictionary(Of String, String)
    Public isFromStudentList As Boolean
    Public parentForm As StudentListWindow

    Dim didChange As Boolean
    Private Sub btnClose_Click(sender As Object, e As RoutedEventArgs) Handles btnClose.Click
        didChange = False
        Me.Close()
    End Sub

    Private Sub btnRegister_Click(sender As Object, e As RoutedEventArgs) Handles btnRegister.Click
        If ValidateData() Then
            If selectedStudent Is Nothing Then
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

        Dim registerQuery As String = String.Format("INSERT INTO tbl_contacts({0}) VALUES({1})",
            pList(parameters.Keys),
            pList(parameters.Values))

        ExecuteQuery(registerQuery, Sub(success)
                                        If (success) Then
                                            MsgBox("Student succesfully registered!", vbInformation)
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
            selectedStudent("ID"))

        ExecuteQuery(registerQuery, Sub(success)
                                        If (success) Then
                                            MsgBox("Contact succesfully updated!", vbInformation)
                                            didChange = True
                                            Me.Close()
                                        Else
                                            MsgBox("Failed to update!", vbExclamation)
                                        End If
                                    End Sub)
    End Sub
    Function ValidateData() As Boolean
        Dim isValid As Boolean = True
        isValid = isValid And AllTrim(txtStudentID.Text).Length > 0
        isValid = isValid And AllTrim(txtContact.Text).Length > 0
        isValid = isValid And AllTrim(txtFirstName.Text).Length > 0
        isValid = isValid And AllTrim(txtLastName.Text).Length > 0
        isValid = isValid And AllTrim(txtCourse.Text).Length > 0
        isValid = isValid And AllTrim(txtYearSection.Text).Length > 0
        Return isValid
    End Function

    Private Sub Grid_Loaded(sender As Object, e As RoutedEventArgs)
        If Not selectedStudent Is Nothing Then
            txtStudentID.Text = selectedStudent("student_id")
            txtContact.Text = selectedStudent("mobile_number")
            txtFirstName.Text = selectedStudent("first_name")
            txtLastName.Text = selectedStudent("last_name")
            txtCourse.Text = selectedStudent("course")
            txtYearSection.Text = selectedStudent("year_section")
            cmbGender.Text = selectedStudent("gender")
            dtBirth.Text = selectedStudent("date_of_birth")
            txtAddress.Text = selectedStudent("address")
            txtEmail.Text = selectedStudent("email")

            btnRegister.Content = "Update"
            Me.Title = "Update Student"
        End If
    End Sub

    Private Sub Window_Closing(sender As Object, e As ComponentModel.CancelEventArgs)
        If isFromStudentList And didChange Then
            parentForm.UpdateTableView()
        End If
    End Sub
End Class
