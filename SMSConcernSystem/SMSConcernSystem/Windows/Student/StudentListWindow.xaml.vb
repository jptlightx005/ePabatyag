Imports System.Data
Public Class StudentListWindow
    Public studentList As List(Of ContactInformation)
    Private Sub btnAdd_Click(sender As Object, e As RoutedEventArgs) Handles btnAdd.Click
        Dim regWindow As New RegistrationWindow
        regWindow.parentForm = Me
        regWindow.ShowDialog()
    End Sub

    Private Sub btnEdit_Click(sender As Object, e As RoutedEventArgs) Handles btnEdit.Click
        Dim regWindow As New RegistrationWindow
        Dim currentRowIndex = gridStudents.Items.IndexOf(gridStudents.SelectedItem)
        Debug.Print(currentRowIndex)
        If (currentRowIndex >= 0 Or currentRowIndex < gridStudents.Items.Count) Then
            regWindow.contactInfo = studentList(currentRowIndex)
            regWindow.isUpdating = True
            regWindow.parentForm = Me
            regWindow.ShowDialog()
        End If
    End Sub

    Private Sub btnClose_Click(sender As Object, e As RoutedEventArgs) Handles btnClose.Click
        Me.Close()
    End Sub


    Private Sub Window_Loaded(sender As Object, e As RoutedEventArgs)
        UpdateTableView()
    End Sub

    Public Sub UpdateTableView()
        Dim selectStudentsQuery As String = "SELECT student_id As `Student ID`," & _
                                             "(first_name || ' ' || last_name) AS `Name`," & _
                                             "course As `Course`," & _
                                             "year_section As `Year & Section`," & _
                                             "mobile_number As `Mobile No` " & _
                                            "FROM `tbl_contacts`"

        Dim dataSet As New DataSet()
        SelectData(selectStudentsQuery).Fill(dataSet)
        gridStudents.ItemsSource = dataSet.Tables(0).DefaultView

        Dim sql As String = "SELECT * FROM tbl_contacts"
        
        Dim result = SelectQuery(sql)

        studentList = New List(Of ContactInformation)
        For Each student In result
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
            studentList.Add(studentInfo)
            Dim imageSource As String = System.IO.Path.Combine(smsSystemImages, String.Format("contact-image-{0}.jpg", studentInfo.ID))
            Debug.Print("meh {0}, {1}", studentInfo.firstName, studentInfo.contactImageSource)
            If (System.IO.File.Exists(imageSource)) Then
                studentInfo.contactImageSource = imageSource
            End If
        Next
        Debug.Print("Student list count is {0}", studentList.Count)

        gridStudents.IsReadOnly = True
    End Sub
    Private Sub gridStudents_SelectionChanged(sender As Object, e As SelectionChangedEventArgs) Handles gridStudents.SelectionChanged
        If (gridStudents.SelectedIndex >= 0) Then
            Debug.Print(gridStudents.SelectedIndex)
            btnEdit.IsEnabled = True
            btnDelete.IsEnabled = True
        Else
            btnEdit.IsEnabled = False
            btnDelete.IsEnabled = False
        End If
    End Sub

    Private Sub Window_Closing(sender As Object, e As ComponentModel.CancelEventArgs)

    End Sub
End Class
