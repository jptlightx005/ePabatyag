Imports System.Data
Class MainWindow
    Dim messageList As List(Of Dictionary(Of String, String))
    Private Sub btnRegister_Click(sender As Object, e As RoutedEventArgs) Handles btnRegister.Click
        Dim regWindow As New RegistrationWindow
        regWindow.ShowDialog()
    End Sub

    Private Sub btnLogout_Click(sender As Object, e As RoutedEventArgs) Handles btnLogout.Click
        Dim loginWindow As New LogInWindow
        loginWindow.Show()
        Me.Close()
    End Sub

    Private Sub btnStudentList_Click(sender As Object, e As RoutedEventArgs) Handles btnStudentList.Click
        Dim studentListWindow As New StudentListWindow
        studentListWindow.ShowDialog()
    End Sub

    Private Sub Window_Loaded(sender As Object, e As RoutedEventArgs)
        UpdateTable()
    End Sub

    Private Sub UpdateTable()
        Dim selectStudentsQuery As String = "SELECT tbl_contacts.student_id As `Student ID`," & _
                                            "(first_name || ' ' || last_name) AS `Name`," & _
                                            "message_content AS `Message`," & _
                                            "mobile_number As `Mobile No`, " & _
                                            "course As `Course`," & _
                                            "year_section As `Year & Section` " & _
                                            "FROM tbl_inbox JOIN tbl_contacts ON tbl_inbox.contact_id = tbl_contacts.ID"

        SelectData(selectStudentsQuery,
                   Sub(dataAdapter)
                       Dim dataSet As New DataSet()
                       dataAdapter.Fill(dataSet)
                       gridInbox.ItemsSource = dataSet.Tables(0).DefaultView
                   End Sub)

        Dim sql As String = String.Format("SELECT * FROM tbl_inbox JOIN tbl_contacts ON tbl_inbox.contact_id = tbl_contacts.ID")

        SelectQuery(sql, Sub(result)
                             messageList = result
                         End Sub)

        gridInbox.IsReadOnly = True
    End Sub
End Class
