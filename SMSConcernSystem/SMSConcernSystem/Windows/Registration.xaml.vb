Imports System.Data.SQLite
Public Class Registration
    Inherits SMSConcernWindow

    Private Sub Grid_Loaded(sender As Object, e As RoutedEventArgs)
        Dim columns As New ArrayList()
        columns.Add("First Name")
        columns.Add("Last Name")
        columns.Add("Mobile Number")

    End Sub
End Class
