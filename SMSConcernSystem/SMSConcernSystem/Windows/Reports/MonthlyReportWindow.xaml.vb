Imports System.IO.File
Public Class MonthlyReportWindow
    Private Sub Grid_Loaded(sender As Object, e As RoutedEventArgs)
        Dim base_dir = AppDomain.CurrentDomain.BaseDirectory
        Dim HTMLReportPage As String = ReadAllText(base_dir & "\Resources\MonthlyReportPage.html")
        Debug.Print(HTMLReportPage)

        Dim tryMeh = String.Format(HTMLReportPage, "Gotta touch this =)")
        Debug.Print(tryMeh)
    End Sub
End Class
