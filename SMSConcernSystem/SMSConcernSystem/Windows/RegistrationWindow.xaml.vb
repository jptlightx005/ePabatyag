Imports System.Data.SQLite
Public Class RegistrationWindow
    Private Sub btnClose_Click(sender As Object, e As RoutedEventArgs) Handles btnClose.Click
        Me.Close()
    End Sub

    Private Sub btnRegister_Click(sender As Object, e As RoutedEventArgs) Handles btnRegister.Click
        If ValidateData() Then
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

            Dim pList As Func(Of IEnumerable(Of String), String) = Function(enumerable)
                                                                       Dim list As New List(Of String)(enumerable)

                                                                       Return String.Join(",", list.ToArray)
                                                                   End Function
            Dim registerQuery As String = String.Format("INSERT INTO tbl_contacts({0}) VALUES({1})",
                pList(parameters.Keys),
                pList(parameters.Values))

            ExecuteQuery(registerQuery, Sub(success)
                                            If (success) Then
                                                MsgBox("Student succesfully registered!", vbInformation)
                                                Me.Close()
                                            Else
                                                MsgBox("Failed to register!", vbExclamation)
                                            End If
                                        End Sub)
        Else
            MsgBox("Please enter all required data!", vbExclamation)
        End If
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
    End Sub
End Class
