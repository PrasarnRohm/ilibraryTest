<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class Form1
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.btnLotSetUp = New System.Windows.Forms.Button()
        Me.tbxLotNo = New System.Windows.Forms.TextBox()
        Me.lbMessage = New System.Windows.Forms.Label()
        Me.btnReload = New System.Windows.Forms.Button()
        Me.tbxAppName = New System.Windows.Forms.TextBox()
        Me.tbxFunctionName = New System.Windows.Forms.TextBox()
        Me.Button1 = New System.Windows.Forms.Button()
        Me.tbxOPNO = New System.Windows.Forms.TextBox()
        Me.Button2 = New System.Windows.Forms.Button()
        Me.btnLotEnd = New System.Windows.Forms.Button()
        Me.btnFirstIns = New System.Windows.Forms.Button()
        Me.btnLotCancel = New System.Windows.Forms.Button()
        Me.SuspendLayout()
        '
        'btnLotSetUp
        '
        Me.btnLotSetUp.Location = New System.Drawing.Point(12, 20)
        Me.btnLotSetUp.Name = "btnLotSetUp"
        Me.btnLotSetUp.Size = New System.Drawing.Size(130, 59)
        Me.btnLotSetUp.TabIndex = 0
        Me.btnLotSetUp.Text = "LotSetUp"
        Me.btnLotSetUp.UseVisualStyleBackColor = True
        '
        'tbxLotNo
        '
        Me.tbxLotNo.Location = New System.Drawing.Point(159, 14)
        Me.tbxLotNo.Name = "tbxLotNo"
        Me.tbxLotNo.Size = New System.Drawing.Size(130, 20)
        Me.tbxLotNo.TabIndex = 1
        Me.tbxLotNo.Text = "LotNo"
        '
        'lbMessage
        '
        Me.lbMessage.BackColor = System.Drawing.Color.Peru
        Me.lbMessage.Dock = System.Windows.Forms.DockStyle.Right
        Me.lbMessage.Location = New System.Drawing.Point(463, 0)
        Me.lbMessage.Name = "lbMessage"
        Me.lbMessage.Size = New System.Drawing.Size(378, 497)
        Me.lbMessage.TabIndex = 2
        Me.lbMessage.Text = "Label1"
        '
        'btnReload
        '
        Me.btnReload.Location = New System.Drawing.Point(372, 459)
        Me.btnReload.Name = "btnReload"
        Me.btnReload.Size = New System.Drawing.Size(86, 34)
        Me.btnReload.TabIndex = 3
        Me.btnReload.Text = "Reload"
        Me.btnReload.UseVisualStyleBackColor = True
        '
        'tbxAppName
        '
        Me.tbxAppName.Enabled = False
        Me.tbxAppName.Location = New System.Drawing.Point(159, 40)
        Me.tbxAppName.Name = "tbxAppName"
        Me.tbxAppName.Size = New System.Drawing.Size(130, 20)
        Me.tbxAppName.TabIndex = 1
        Me.tbxAppName.Text = "AppName"
        '
        'tbxFunctionName
        '
        Me.tbxFunctionName.Enabled = False
        Me.tbxFunctionName.Location = New System.Drawing.Point(159, 66)
        Me.tbxFunctionName.Name = "tbxFunctionName"
        Me.tbxFunctionName.Size = New System.Drawing.Size(130, 20)
        Me.tbxFunctionName.TabIndex = 1
        Me.tbxFunctionName.Text = "FuntionName"
        '
        'Button1
        '
        Me.Button1.Location = New System.Drawing.Point(371, 419)
        Me.Button1.Name = "Button1"
        Me.Button1.Size = New System.Drawing.Size(86, 34)
        Me.Button1.TabIndex = 4
        Me.Button1.Text = "PackageEnable"
        Me.Button1.UseVisualStyleBackColor = True
        '
        'tbxOPNO
        '
        Me.tbxOPNO.Location = New System.Drawing.Point(295, 14)
        Me.tbxOPNO.Name = "tbxOPNO"
        Me.tbxOPNO.Size = New System.Drawing.Size(130, 20)
        Me.tbxOPNO.TabIndex = 1
        Me.tbxOPNO.Text = "OP No."
        '
        'Button2
        '
        Me.Button2.Location = New System.Drawing.Point(12, 183)
        Me.Button2.Name = "Button2"
        Me.Button2.Size = New System.Drawing.Size(130, 59)
        Me.Button2.TabIndex = 0
        Me.Button2.Text = "LotStart"
        Me.Button2.UseVisualStyleBackColor = True
        '
        'btnLotEnd
        '
        Me.btnLotEnd.Location = New System.Drawing.Point(12, 313)
        Me.btnLotEnd.Name = "btnLotEnd"
        Me.btnLotEnd.Size = New System.Drawing.Size(130, 59)
        Me.btnLotEnd.TabIndex = 0
        Me.btnLotEnd.Text = "LotEnd"
        Me.btnLotEnd.UseVisualStyleBackColor = True
        '
        'btnFirstIns
        '
        Me.btnFirstIns.Location = New System.Drawing.Point(12, 248)
        Me.btnFirstIns.Name = "btnFirstIns"
        Me.btnFirstIns.Size = New System.Drawing.Size(130, 59)
        Me.btnFirstIns.TabIndex = 0
        Me.btnFirstIns.Text = "FirstIns"
        Me.btnFirstIns.UseVisualStyleBackColor = True
        '
        'btnLotCancel
        '
        Me.btnLotCancel.Location = New System.Drawing.Point(12, 85)
        Me.btnLotCancel.Name = "btnLotCancel"
        Me.btnLotCancel.Size = New System.Drawing.Size(130, 59)
        Me.btnLotCancel.TabIndex = 0
        Me.btnLotCancel.Text = "LotCancel"
        Me.btnLotCancel.UseVisualStyleBackColor = True
        '
        'Form1
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(841, 497)
        Me.Controls.Add(Me.Button1)
        Me.Controls.Add(Me.btnReload)
        Me.Controls.Add(Me.lbMessage)
        Me.Controls.Add(Me.tbxOPNO)
        Me.Controls.Add(Me.tbxFunctionName)
        Me.Controls.Add(Me.tbxAppName)
        Me.Controls.Add(Me.tbxLotNo)
        Me.Controls.Add(Me.btnLotEnd)
        Me.Controls.Add(Me.btnFirstIns)
        Me.Controls.Add(Me.btnLotCancel)
        Me.Controls.Add(Me.Button2)
        Me.Controls.Add(Me.btnLotSetUp)
        Me.Name = "Form1"
        Me.Text = "DB-AS-00"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents btnLotSetUp As Button
    Friend WithEvents tbxLotNo As TextBox
    Friend WithEvents lbMessage As Label
    Friend WithEvents btnReload As Button
    Friend WithEvents tbxAppName As TextBox
    Friend WithEvents tbxFunctionName As TextBox
    Friend WithEvents Button1 As Button
    Friend WithEvents tbxOPNO As TextBox
    Friend WithEvents Button2 As Button
    Friend WithEvents btnLotEnd As Button
    Friend WithEvents btnFirstIns As Button
    Friend WithEvents btnLotCancel As Button
End Class
