Imports System.Data.SQLite
Public Class LogInWindow
    Inherits SMSConcernWindow
    Private Sub btnLogin_Click(sender As Object, e As RoutedEventArgs) Handles btnLogin.Click
        Login(txtUsrn.Text, txtPssw.Password,
              Function(result)
                  If (result.Count > 0) Then
                      MessageBox.Show("Successfully logged in!", "Log-in", MessageBoxButton.OK, MessageBoxImage.Information)
                      Dim mainWindow As New MainWindow
                      mainWindow.Show()
                      Me.Close()
                  Else
                      MessageBox.Show("Failed to log in!", "Log-in", MessageBoxButton.OK, MessageBoxImage.Exclamation)
                  End If

                  Return 0
              End Function)
    End Sub
End Class
