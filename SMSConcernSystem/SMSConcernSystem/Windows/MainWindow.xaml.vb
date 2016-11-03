Class MainWindow 

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
End Class
