Public Class Log_in

    Private Sub btnLogin_Click(sender As Object, e As RoutedEventArgs) Handles btnLogin.Click
        If txtUsrn.Text = "admin" And txtPssw.Password = "password" Then
            MessageBox.Show("Successfully logged in!", "Log-in", MessageBoxButton.OK, MessageBoxImage.Information)
            Dim mainWindow As New MainWindow
            mainWindow.Show()
            Me.Close()
        Else
            MessageBox.Show("Failed to log in!", "Log-in", MessageBoxButton.OK, MessageBoxImage.Exclamation)
        End If
    End Sub
End Class
