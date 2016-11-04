Public Class StudentListWindow
    Private Sub btnAdd_Click(sender As Object, e As RoutedEventArgs) Handles btnAdd.Click
        Dim regWindow As New RegistrationWindow
        regWindow.ShowDialog()
    End Sub

    Private Sub btnEdit_Click(sender As Object, e As RoutedEventArgs) Handles btnEdit.Click
        Dim regWindow As New RegistrationWindow
        regWindow.ShowDialog()
    End Sub

    Private Sub btnClose_Click(sender As Object, e As RoutedEventArgs) Handles btnClose.Click
        Me.Close()
    End Sub
End Class
