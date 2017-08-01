Imports System.IO.File
Imports System.Collections.ObjectModel
Public Class MonthlyReportWindow
    Public Property department_list As New ObservableCollection(Of String)
    Public Sub New()

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        cmb_dep.ItemsSource = department_list
        department_list.Add("ALL")
        cmb_dep.SelectedIndex = 0
        For Each kw In keywords
            department_list.Add(kw)
        Next
    End Sub
    Private Sub Grid_Loaded(sender As Object, e As RoutedEventArgs)


    End Sub

    Private Sub btn_close_Click(sender As Object, e As RoutedEventArgs) Handles btn_close.Click
        Me.Close()
    End Sub

    Private Sub btn_print_Click(sender As Object, e As RoutedEventArgs) Handles btn_print.Click
        Dim webReportWindow As New MonthlyReportWebWindow
        webReportWindow.allMonth = chkAllMonths.IsChecked
        If Not chkAllMonths.IsChecked Then
            If fromPicker.SelectedDate Is Nothing And toPicker.SelectedDate Is Nothing Then
                MsgBox("Please select dates!", vbExclamation)
                Return
            End If
            webReportWindow.fromMonth = fromPicker.SelectedDate.Value
            webReportWindow.toMonth = toPicker.SelectedDate.Value

        End If

        webReportWindow.selectedDep = department_list(cmb_dep.SelectedIndex)
        webReportWindow.ShowDialog()
    End Sub

    Private Sub chkAllMonths_Click(sender As Object, e As RoutedEventArgs) Handles chkAllMonths.Click
        If chkAllMonths.IsChecked Then
            fromPicker.IsEnabled = False
            toPicker.IsEnabled = False
        Else
            fromPicker.IsEnabled = True
            toPicker.IsEnabled = True
        End If
    End Sub
End Class
