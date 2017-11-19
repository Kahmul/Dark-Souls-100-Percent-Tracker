<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class Main
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(Main))
        Me.btnHook = New System.Windows.Forms.Button()
        Me.btnUnhook = New System.Windows.Forms.Button()
        Me.treasureLocationsLabel = New System.Windows.Forms.Label()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.percentageLabel = New System.Windows.Forms.Label()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.Label6 = New System.Windows.Forms.Label()
        Me.Label7 = New System.Windows.Forms.Label()
        Me.treasureLocationsValueLabel = New System.Windows.Forms.Label()
        Me.npcQuestlinesValueLabel = New System.Windows.Forms.Label()
        Me.nonRespawningEnemiesValueLabel = New System.Windows.Forms.Label()
        Me.illusoryWallsValueLabel = New System.Windows.Forms.Label()
        Me.foggatesValueLabel = New System.Windows.Forms.Label()
        Me.shortcutsValueLabel = New System.Windows.Forms.Label()
        Me.bossesKilledValueLabel = New System.Windows.Forms.Label()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.SuspendLayout()
        '
        'btnHook
        '
        Me.btnHook.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.btnHook.FlatStyle = System.Windows.Forms.FlatStyle.System
        Me.btnHook.Location = New System.Drawing.Point(16, 272)
        Me.btnHook.Name = "btnHook"
        Me.btnHook.Size = New System.Drawing.Size(75, 23)
        Me.btnHook.TabIndex = 0
        Me.btnHook.Text = "Hook"
        Me.btnHook.UseVisualStyleBackColor = True
        '
        'btnUnhook
        '
        Me.btnUnhook.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.btnUnhook.Enabled = False
        Me.btnUnhook.FlatStyle = System.Windows.Forms.FlatStyle.System
        Me.btnUnhook.Location = New System.Drawing.Point(97, 272)
        Me.btnUnhook.Name = "btnUnhook"
        Me.btnUnhook.Size = New System.Drawing.Size(75, 23)
        Me.btnUnhook.TabIndex = 4
        Me.btnUnhook.Text = "Unhook"
        Me.btnUnhook.UseVisualStyleBackColor = True
        '
        'treasureLocationsLabel
        '
        Me.treasureLocationsLabel.AutoSize = True
        Me.treasureLocationsLabel.Font = New System.Drawing.Font("Calibri", 10.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.treasureLocationsLabel.ForeColor = System.Drawing.SystemColors.ButtonHighlight
        Me.treasureLocationsLabel.Location = New System.Drawing.Point(13, 27)
        Me.treasureLocationsLabel.Name = "treasureLocationsLabel"
        Me.treasureLocationsLabel.Size = New System.Drawing.Size(123, 17)
        Me.treasureLocationsLabel.TabIndex = 11
        Me.treasureLocationsLabel.Text = "Treasure Locations:"
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Font = New System.Drawing.Font("Calibri", 10.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label2.ForeColor = System.Drawing.SystemColors.ButtonHighlight
        Me.Label2.Location = New System.Drawing.Point(13, 80)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(162, 17)
        Me.Label2.TabIndex = 12
        Me.Label2.Text = "Non-respawning Enemies:"
        '
        'percentageLabel
        '
        Me.percentageLabel.AutoSize = True
        Me.percentageLabel.Font = New System.Drawing.Font("Calibri", 30.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.percentageLabel.ForeColor = System.Drawing.SystemColors.ButtonHighlight
        Me.percentageLabel.Location = New System.Drawing.Point(273, 86)
        Me.percentageLabel.Name = "percentageLabel"
        Me.percentageLabel.Size = New System.Drawing.Size(71, 49)
        Me.percentageLabel.TabIndex = 13
        Me.percentageLabel.Text = "0%"
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Font = New System.Drawing.Font("Calibri", 10.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label4.ForeColor = System.Drawing.SystemColors.ButtonHighlight
        Me.Label4.Location = New System.Drawing.Point(13, 108)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(101, 17)
        Me.Label4.TabIndex = 14
        Me.Label4.Text = "NPC Questlines:"
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.Font = New System.Drawing.Font("Calibri", 10.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label5.ForeColor = System.Drawing.SystemColors.ButtonHighlight
        Me.Label5.Location = New System.Drawing.Point(12, 138)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(163, 17)
        Me.Label5.TabIndex = 15
        Me.Label5.Text = "Shortcuts / Locked Doors:"
        '
        'Label6
        '
        Me.Label6.AutoSize = True
        Me.Label6.Font = New System.Drawing.Font("Calibri", 10.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label6.ForeColor = System.Drawing.SystemColors.ButtonHighlight
        Me.Label6.Location = New System.Drawing.Point(12, 165)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(90, 17)
        Me.Label6.TabIndex = 16
        Me.Label6.Text = "Illusory Walls:"
        '
        'Label7
        '
        Me.Label7.AutoSize = True
        Me.Label7.Font = New System.Drawing.Font("Calibri", 10.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label7.ForeColor = System.Drawing.SystemColors.ButtonHighlight
        Me.Label7.Location = New System.Drawing.Point(13, 192)
        Me.Label7.Name = "Label7"
        Me.Label7.Size = New System.Drawing.Size(65, 17)
        Me.Label7.TabIndex = 17
        Me.Label7.Text = "Foggates:"
        '
        'treasureLocationsValueLabel
        '
        Me.treasureLocationsValueLabel.AutoSize = True
        Me.treasureLocationsValueLabel.Font = New System.Drawing.Font("Calibri", 10.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.treasureLocationsValueLabel.ForeColor = System.Drawing.SystemColors.ButtonHighlight
        Me.treasureLocationsValueLabel.Location = New System.Drawing.Point(178, 27)
        Me.treasureLocationsValueLabel.Name = "treasureLocationsValueLabel"
        Me.treasureLocationsValueLabel.Size = New System.Drawing.Size(36, 17)
        Me.treasureLocationsValueLabel.TabIndex = 24
        Me.treasureLocationsValueLabel.Text = "X / X"
        '
        'npcQuestlinesValueLabel
        '
        Me.npcQuestlinesValueLabel.AutoSize = True
        Me.npcQuestlinesValueLabel.Font = New System.Drawing.Font("Calibri", 10.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.npcQuestlinesValueLabel.ForeColor = System.Drawing.SystemColors.ButtonHighlight
        Me.npcQuestlinesValueLabel.Location = New System.Drawing.Point(178, 108)
        Me.npcQuestlinesValueLabel.Name = "npcQuestlinesValueLabel"
        Me.npcQuestlinesValueLabel.Size = New System.Drawing.Size(36, 17)
        Me.npcQuestlinesValueLabel.TabIndex = 25
        Me.npcQuestlinesValueLabel.Text = "X / X"
        '
        'nonRespawningEnemiesValueLabel
        '
        Me.nonRespawningEnemiesValueLabel.AutoSize = True
        Me.nonRespawningEnemiesValueLabel.Font = New System.Drawing.Font("Calibri", 10.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.nonRespawningEnemiesValueLabel.ForeColor = System.Drawing.SystemColors.ButtonHighlight
        Me.nonRespawningEnemiesValueLabel.Location = New System.Drawing.Point(178, 80)
        Me.nonRespawningEnemiesValueLabel.Name = "nonRespawningEnemiesValueLabel"
        Me.nonRespawningEnemiesValueLabel.Size = New System.Drawing.Size(36, 17)
        Me.nonRespawningEnemiesValueLabel.TabIndex = 26
        Me.nonRespawningEnemiesValueLabel.Text = "X / X"
        '
        'illusoryWallsValueLabel
        '
        Me.illusoryWallsValueLabel.AutoSize = True
        Me.illusoryWallsValueLabel.Font = New System.Drawing.Font("Calibri", 10.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.illusoryWallsValueLabel.ForeColor = System.Drawing.SystemColors.ButtonHighlight
        Me.illusoryWallsValueLabel.Location = New System.Drawing.Point(178, 165)
        Me.illusoryWallsValueLabel.Name = "illusoryWallsValueLabel"
        Me.illusoryWallsValueLabel.Size = New System.Drawing.Size(36, 17)
        Me.illusoryWallsValueLabel.TabIndex = 27
        Me.illusoryWallsValueLabel.Text = "X / X"
        '
        'foggatesValueLabel
        '
        Me.foggatesValueLabel.AutoSize = True
        Me.foggatesValueLabel.Font = New System.Drawing.Font("Calibri", 10.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.foggatesValueLabel.ForeColor = System.Drawing.SystemColors.ButtonHighlight
        Me.foggatesValueLabel.Location = New System.Drawing.Point(178, 192)
        Me.foggatesValueLabel.Name = "foggatesValueLabel"
        Me.foggatesValueLabel.Size = New System.Drawing.Size(36, 17)
        Me.foggatesValueLabel.TabIndex = 28
        Me.foggatesValueLabel.Text = "X / X"
        '
        'shortcutsValueLabel
        '
        Me.shortcutsValueLabel.AutoSize = True
        Me.shortcutsValueLabel.Font = New System.Drawing.Font("Calibri", 10.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.shortcutsValueLabel.ForeColor = System.Drawing.SystemColors.ButtonHighlight
        Me.shortcutsValueLabel.Location = New System.Drawing.Point(178, 138)
        Me.shortcutsValueLabel.Name = "shortcutsValueLabel"
        Me.shortcutsValueLabel.Size = New System.Drawing.Size(36, 17)
        Me.shortcutsValueLabel.TabIndex = 29
        Me.shortcutsValueLabel.Text = "X / X"
        '
        'bossesKilledValueLabel
        '
        Me.bossesKilledValueLabel.AutoSize = True
        Me.bossesKilledValueLabel.Font = New System.Drawing.Font("Calibri", 10.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.bossesKilledValueLabel.ForeColor = System.Drawing.SystemColors.ButtonHighlight
        Me.bossesKilledValueLabel.Location = New System.Drawing.Point(178, 54)
        Me.bossesKilledValueLabel.Name = "bossesKilledValueLabel"
        Me.bossesKilledValueLabel.Size = New System.Drawing.Size(36, 17)
        Me.bossesKilledValueLabel.TabIndex = 30
        Me.bossesKilledValueLabel.Text = "X / X"
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Font = New System.Drawing.Font("Calibri", 10.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label1.ForeColor = System.Drawing.SystemColors.ButtonHighlight
        Me.Label1.Location = New System.Drawing.Point(13, 54)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(53, 17)
        Me.Label1.TabIndex = 31
        Me.Label1.Text = "Bosses:"
        '
        'Main
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.SystemColors.ActiveCaptionText
        Me.ClientSize = New System.Drawing.Size(459, 302)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.bossesKilledValueLabel)
        Me.Controls.Add(Me.shortcutsValueLabel)
        Me.Controls.Add(Me.foggatesValueLabel)
        Me.Controls.Add(Me.illusoryWallsValueLabel)
        Me.Controls.Add(Me.nonRespawningEnemiesValueLabel)
        Me.Controls.Add(Me.npcQuestlinesValueLabel)
        Me.Controls.Add(Me.treasureLocationsValueLabel)
        Me.Controls.Add(Me.Label7)
        Me.Controls.Add(Me.Label6)
        Me.Controls.Add(Me.Label5)
        Me.Controls.Add(Me.Label4)
        Me.Controls.Add(Me.percentageLabel)
        Me.Controls.Add(Me.btnUnhook)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.btnHook)
        Me.Controls.Add(Me.treasureLocationsLabel)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MaximizeBox = False
        Me.Name = "Main"
        Me.Text = "SpeedSouls - Dark Souls 100% Tracker"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents btnHook As Button
    Friend WithEvents btnUnhook As Button
    Friend WithEvents percentageLabel As Label
    Friend WithEvents Label2 As Label
    Friend WithEvents treasureLocationsLabel As Label
    Friend WithEvents Label4 As Label
    Friend WithEvents Label5 As Label
    Friend WithEvents Label6 As Label
    Friend WithEvents Label7 As Label
    Friend WithEvents treasureLocationsValueLabel As Label
    Friend WithEvents npcQuestlinesValueLabel As Label
    Friend WithEvents nonRespawningEnemiesValueLabel As Label
    Friend WithEvents illusoryWallsValueLabel As Label
    Friend WithEvents foggatesValueLabel As Label
    Friend WithEvents shortcutsValueLabel As Label
    Friend WithEvents bossesKilledValueLabel As Label
    Friend WithEvents Label1 As Label
End Class
