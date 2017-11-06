Imports System.Data.SQLite
Public Class LogInWindow
    Private Sub enterIsPressed(sender As Object, e As KeyEventArgs) Handles txtUsrn.KeyUp, txtPssw.KeyUp
        If e.Key = Key.Enter Then
            btnLogin_Click(sender, e)
        End If
    End Sub
    Private Sub btnLogin_Click(sender As Object, e As RoutedEventArgs) Handles btnLogin.Click
        Dim result = Login(txtUsrn.Text, txtPssw.Password)
        If (result.Count > 0) Then
            MessageBox.Show("Successfully logged in!", "Log-in", MessageBoxButton.OK, MessageBoxImage.Information)
            Dim user As Dictionary(Of String, String)
            user = result.First
            My.Settings.usrn = user("usrn")
            My.Settings.pssw = user("pssw")
            My.Settings.adminID = user("ID")
            My.Settings.isLoggedIn = True
            My.Settings.Save()
            Dim mainWindow As New MainWindow
            mainWindow.Show()
            Me.Close()
        Else
            MessageBox.Show("Failed to log in!", "Log-in", MessageBoxButton.OK, MessageBoxImage.Exclamation)
        End If
    End Sub
End Class
