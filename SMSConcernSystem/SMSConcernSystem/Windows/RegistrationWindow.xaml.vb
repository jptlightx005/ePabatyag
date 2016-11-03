Imports System.Data.SQLite
Public Class RegistrationWindow
    Inherits SMSConcernWindow

    Private Sub Grid_Loaded(sender As Object, e As RoutedEventArgs)
        Dim columns As New ArrayList()
        columns.Add("First Name")
        columns.Add("Last Name")
        columns.Add("Mobile Number")

    End Sub

    Private Sub btnClose_Click(sender As Object, e As RoutedEventArgs) Handles btnClose.Click
        Me.Close()
    End Sub

    Private Sub btnRegister_Click(sender As Object, e As RoutedEventArgs) Handles btnRegister.Click
        Me.Close()
    End Sub
End Class
