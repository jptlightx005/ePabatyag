Imports GsmComm.GsmCommunication
Public Class SettingsWindow
    Dim ports As List(Of String)
    Dim selectedPort As String
    Dim portHasDevice As Boolean

    Private Sub btnClose_Click(sender As Object, e As RoutedEventArgs) Handles btnClose.Click
        Me.Close()
    End Sub

    Private Sub Window_Loaded(sender As Object, e As RoutedEventArgs)
        ReloadDeviceSetting()
        portHasDevice = False
        If smsComm.IsOpen Then
            smsComm.Close()
        End If
    End Sub

    Sub ReloadDeviceSetting()
        ports = GetPorts()
        Dim portIndex = -1
        For Each port In ports
            Dim menuItem As New ComboBoxItem
            Dim comm As New GsmCommMain(port)
            Dim info As IdentificationInfo
            Dim portText As String
            Try
                comm.Open()
                info = comm.IdentifyDevice()
                portText = String.Format("{0} {1} ({2})", info.Manufacturer.ToUpper, info.Model, port)
                comm.Close()
            Catch ex As Exception
                portText = port
                If comm.IsOpen() Then
                    comm.Close()
                End If
            End Try
            menuItem.Content = portText
            cmbDevices.Items.Add(menuItem)
        Next

    End Sub

    Private Sub btnSave_Click(sender As Object, e As RoutedEventArgs) Handles btnSave.Click
        'Device Selection
        If cmbDevices.SelectedIndex >= 0 Then
            'If portHasDevice Then
            '    My.Settings.smsDevicePort = selectedPort
            '    My.Settings.Save()
            '    smsDevicePort = selectedPort
            '    smsComm = New GsmCommMain(smsDevicePort)
            '    Try
            '        smsComm.Open()
            '    Catch ex As Exception
            '        smsComm.Close()
            '        MsgBox("Error has occured while connecting!", vbExclamation, "Connection Error")
            '    End Try
            'End If
        End If
        Me.Close()
    End Sub

    Private Sub btnTest_Click(sender As Object, e As RoutedEventArgs) Handles btnTest.Click
        Dim comm As New GsmCommMain(selectedPort)
        Try
            comm.Open()
            While Not comm.IsConnected
                Dim a = MsgBox("No device is connected on the selected port.", vbRetryCancel + vbExclamation, "Connection Setup")
                If a = vbCancel Then
                    comm.Close()
                    Exit Sub
                End If
            End While
            comm.Close()
        Catch ex As Exception
            Dim errMessage = String.Format("Connection error: {0}", ex.Message)
            MsgBox(errMessage, vbOKOnly + vbExclamation, "Connection Setup")
        End Try
        portHasDevice = True
        imgConnected.Visibility = Windows.Visibility.Visible
        lblConnected.Content = "Connected"
        MsgBox("Device is working!", vbOKOnly + vbInformation, "Connection Setup")
    End Sub

    Private Sub cmbDevices_SelectionChanged(sender As Object, e As SelectionChangedEventArgs) Handles cmbDevices.SelectionChanged
        If cmbDevices.SelectedIndex >= 0 Then
            selectedPort = ports(cmbDevices.SelectedIndex)
            portHasDevice = False
            imgConnected.Visibility = Windows.Visibility.Hidden
            lblConnected.Content = "Not Connected"
            Debug.Print("Selected port is {0}", selectedPort)
        End If
    End Sub
End Class
