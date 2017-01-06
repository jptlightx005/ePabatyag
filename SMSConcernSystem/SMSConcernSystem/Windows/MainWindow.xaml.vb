Imports System.Data
Class MainWindow
    Dim messageList As List(Of Dictionary(Of String, String))

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

        Dim sql As String = String.Format("SELECT tbl_inbox.*, tbl_contacts.ID as studentID, tbl_contacts.student_id, tbl_contacts.mobile_number, tbl_contacts.first_name, tbl_contacts.last_name, tbl_contacts.course, tbl_contacts.year_section FROM tbl_inbox JOIN tbl_contacts ON tbl_inbox.contact_id = tbl_contacts.ID")

        SelectQuery(sql, Sub(result)
                             messageList = result
                         End Sub)

        gridInbox.IsReadOnly = True
    End Sub

    Private Sub gridInboxRow_DoubleClick(sender As Object, e As MouseButtonEventArgs)
        Debug.Print(String.Format("Did select row number: {0}", gridInbox.SelectedIndex))
        Dim messageWindow As New ViewMessageWindow
        messageWindow.selectedMessage = messageList(gridInbox.SelectedIndex)
        messageWindow.ShowDialog()
    End Sub

    Private Sub btnRead_Click(sender As Object, e As RoutedEventArgs) Handles btnRead.Click
        If gridInbox.SelectedIndex >= 0 Then
            Dim messageWindow As New ViewMessageWindow
            messageWindow.selectedMessage = messageList(gridInbox.SelectedIndex)
            messageWindow.ShowDialog()
        End If
    End Sub

    Private Sub mnu_logout_Click(sender As Object, e As RoutedEventArgs) Handles mnu_logout.Click
        Dim loginWindow As New LogInWindow
        My.Settings.isLoggedIn = False
        My.Settings.Save()
        loginWindow.Show()
        Me.Close()
    End Sub

    Private Sub mnu_register_Click(sender As Object, e As RoutedEventArgs) Handles mnu_register.Click
        Dim regWindow As New RegistrationWindow
        regWindow.ShowDialog()
    End Sub

    Private Sub mnu_studentlist_Click(sender As Object, e As RoutedEventArgs) Handles mnu_studentlist.Click
        Dim studentListWindow As New StudentListWindow
        studentListWindow.ShowDialog()
    End Sub

    Private Sub mnuSettings_Click(sender As Object, e As RoutedEventArgs) Handles mnuSettings.Click
        Dim settingsWindow As New SettingsWindow
        settingsWindow.ShowDialog()
    End Sub

    Private Sub btnGetMessages_Click(sender As Object, e As RoutedEventArgs) Handles btnGetMessages.Click
        GetAllUnreadMessages()

    End Sub
End Class
