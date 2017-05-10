Imports System.IO.File
Public Class MonthlyReportWindow
    Private Sub Grid_Loaded(sender As Object, e As RoutedEventArgs)
        Dim base_dir = AppDomain.CurrentDomain.BaseDirectory
        Dim HTMLReportPage As String = ReadAllText(base_dir & "\MonthlyReports\MonthlyReportPage.html")

        Dim tryMeh = HTMLReportPage.Replace("{0}", "Gotta touch this =)")

    End Sub
End Class
