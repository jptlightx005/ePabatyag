Public Class CredentialEditor

    Private Sub chkUsrnUnchange_Checked(sender As Object, e As RoutedEventArgs) Handles chkUsrnUnchange.Checked
        txtNewUsrn.IsEnabled = Not chkUsrnUnchange.IsChecked

        btnChange.IsEnabled = Not (chkUsrnUnchange.IsChecked And chkPsswUnchange.IsChecked)
    End Sub

    Private Sub chkPsswUnchange_Checked(sender As Object, e As RoutedEventArgs) Handles chkPsswUnchange.Checked
        txtNewPssw.IsEnabled = Not chkPsswUnchange.IsChecked
        txtConfPssw.IsEnabled = Not chkPsswUnchange.IsChecked

        btnChange.IsEnabled = Not (chkUsrnUnchange.IsChecked And chkPsswUnchange.IsChecked)
    End Sub
End Class
