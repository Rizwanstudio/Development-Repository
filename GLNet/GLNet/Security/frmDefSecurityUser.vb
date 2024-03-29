''/////////////////////////////////////////////////////////////////////////////////////////
''//                      GL Net
''/////////////////////////////////////////////////////////////////////////////////////////
''//-------------------------------------------------------------------------------------
''// File Name       : frmDefSecurityUser.vb           				                            
''// Programmer	     : Rizwan Asif
''// Creation Date	 : 16-Jul-2009
''// Description     :                         
''// Function List   : 								                                    
''//											                                            
''//-------------------------------------------------------------------------------------
''// Date Modified     Modified by         Brief Description			                
''//------------------------------------------------------------------------------------
''/////////////////////////////////////////////////////////////////////////////////////////


Imports DAL
Imports Model
Imports System.Collections.Specialized
Imports System.Data
Imports Utility.Utility

Public Class frmDefSecurityUser
    Implements IGeneral

#Region "Variables"
    ''This collection will hold the controls' names, upon which the logged in user has rights
    Private mobjControlList As NameValueCollection
    ''This is the model object which will be set with data values and refered for Save|Update|Delete|Loading Record in Edit Mode
    Private mobjModel As SecurityUser
    Private intPkId As Integer
#End Region

#Region "Enumerations"

    ''This is the representation of grid columns, and will be used while referring grid values, 
    ''instead of giving hard coded column's indexes
    Private Enum EnumGrid
        UserID = 0
        UserName = 1
        UserLogID = 2
        UserLogPassword = 3
        UserEmail = 4
        MobileNo = 5
        UserComments = 6
        Block = 7
    End Enum

#End Region


#Region "Interface Methods"

    ''This will set the images of the buttons at runtime
    Public Sub SetButtonImages() Implements IGeneral.SetButtonImages

        Try

            If gEnumIsRightToLeft = Windows.Forms.RightToLeft.No Then
                Me.btnFirst.ImageList = gobjMyImageListForOperationBar
                Me.btnFirst.ImageKey = "First"

                Me.btnNext.ImageList = gobjMyImageListForOperationBar
                Me.btnNext.ImageKey = "Next"

                Me.btnPrevious.ImageList = gobjMyImageListForOperationBar
                Me.btnPrevious.ImageKey = "Previous"

                Me.btnLast.ImageList = gobjMyImageListForOperationBar
                Me.btnLast.ImageKey = "Last"


            Else
                Me.btnFirst.ImageList = gobjMyImageListForOperationBar
                Me.btnFirst.ImageKey = "Last"

                Me.btnNext.ImageList = gobjMyImageListForOperationBar
                Me.btnNext.ImageKey = "Previous"

                Me.btnPrevious.ImageList = gobjMyImageListForOperationBar
                Me.btnPrevious.ImageKey = "Next"

                Me.btnLast.ImageList = gobjMyImageListForOperationBar
                Me.btnLast.ImageKey = "First"
            End If

            Me.btnNew.ImageList = gobjMyImageListForOperationBar
            Me.btnNew.ImageKey = "New"

            Me.btnSave.ImageList = gobjMyImageListForOperationBar
            Me.btnSave.ImageKey = "Save"

            Me.btnUpdate.ImageList = gobjMyImageListForOperationBar
            Me.btnUpdate.ImageKey = "Update"

            Me.btnCancel.ImageList = gobjMyImageListForOperationBar
            Me.btnCancel.ImageKey = "Cancel"

            Me.btnDelete.ImageList = gobjMyImageListForOperationBar
            Me.btnDelete.ImageKey = "Delete"

            Me.btnExit.ImageList = gobjMyImageListForOperationBar
            Me.btnExit.ImageKey = "Exit"

        Catch ex As Exception
            Throw ex
        End Try

    End Sub

    ''Here will will use this function to fill-up all Combos and Listboxes on the form
    ''Optional condition would be used to fill-up combo or Listbox; which based on the selection of some other combo.
    Public Sub FillCombos(Optional ByVal Condition As String = "") Implements IGeneral.FillCombos
        Try
            ''filling Group  combo
            Dim strSQL As String = "select group_name, group_id from tblGLSecurityGroup order by  group_name"
            Dim dt As DataTable = DAL.UtilityDAL.GetDataTable(strSQL)
            Dim dr As DataRow = dt.NewRow
            dr.Item(0) = gstrComboZeroIndexString
            dr.Item(1) = 0
            dt.Rows.InsertAt(dr, 0)
            Me.cboGroup.DataSource = dt
            Me.cboGroup.DisplayMember = "group_name"
            Me.cboGroup.ValueMember = "group_id"

            ''filling company list
            strSQL = "SELECT location_name AS location_name, location_id From tblGlDefLocation ORDER BY sort_order, location_name"
            Me.lstCompany.ListItem.DisplayMember = "location_name"
            Me.lstCompany.ListItem.ValueMember = "location_id"
            Me.lstCompany.ListItem.DataSource = DAL.UtilityDAL.GetDataTable(strSQL)

        Catch ex As Exception
            Throw ex
        End Try
    End Sub

    ''Here we will use this procedure to load all master records; respective to the screen.
    Public Sub GetAllRecords(Optional ByVal Condition As String = "") Implements IGeneral.GetAllRecords

        Try

            Me.grdAllRecords.DataSource = Nothing

            ''Getting Datasource for Grid from DAL
            Dim dt As DataTable = New SecurityUserDAL().GetAll(Me.cboGroup.SelectedValue)
            Me.grdAllRecords.DataSource = dt
            Me.grdAllRecords.RetrieveStructure()

            ''Applying Grid Formatting Setting
            Me.ApplyGridSettings()

        Catch ex As Exception
            Throw ex
        End Try

    End Sub


    ''This procedure will be used to set the formatting of the grid on that form. For Example, Grid's columns show/Hide,
    '' Caption setting, columns' width etc.
    Public Sub ApplyGridSettings(Optional ByVal Condition As String = "") Implements IGeneral.ApplyGridSettings

        Try


            ''Columns with In-visible setting
            'Me.grdAllRecords.RootTable.Columns(EnumGrid.GroupID).FormatString = ""
            'Me.grdAllRecords.RootTable.Columns(EnumGrid.ShopID).FormatString = ""
            Me.grdAllRecords.RootTable.Columns(EnumGrid.UserID).FormatString = ""
            'Me.grdAllRecords.RootTable.Columns(EnumGrid.GroupID).Visible = False
            'Me.grdAllRecords.RootTable.Columns(EnumGrid.ShopID).Visible = False
            Me.grdAllRecords.RootTable.Columns(EnumGrid.UserID).Visible = False
            Me.grdAllRecords.RootTable.Columns(EnumGrid.UserLogPassword).Visible = False

            Me.grdAllRecords.TotalRow = Janus.Windows.GridEX.InheritableBoolean.False
            Me.grdAllRecords.AutoSizeColumns()

        Catch ex As Exception
            Throw ex
        End Try
    End Sub

    ''This procedure will be used (if applicable) to set Active/Deactive or Visible/Invisible some controls on form,
    ''which are based on System level configuration
    Public Sub SetConfigurationBaseSetting() Implements IGeneral.SetConfigurationBaseSetting

    End Sub

    ''This procedure will be used to set the navigation buttons as per Mode
    Public Sub SetNavigationButtons(ByVal mode As EnumDataMode, Optional ByVal Condition As String = "") Implements IGeneral.SetNavigationButtons
        Try

            If mode = EnumDataMode.[New] Then
                ''if New Mode then Set Disable all Navigation Buttons
                btnFirst.Enabled = False
                btnPrevious.Enabled = False
                btnNext.Enabled = False
                btnLast.Enabled = False

            ElseIf mode = EnumDataMode.Edit Then
                ''if New Mode then Set Enable all Navigation Buttons
                btnFirst.Enabled = True
                btnPrevious.Enabled = True
                btnNext.Enabled = True
                btnLast.Enabled = True

            End If
        Catch ex As Exception
            Throw ex
        End Try
    End Sub

    ''here we will clear all the contols of the screen for New Mode
    Public Sub ReSetControls(Optional ByVal Condition As String = "") Implements IGeneral.ReSetControls
        Try
            Me.intPkId = 0
            Me.txtUserName.Text = String.Empty
            Me.txtEmail.Text = String.Empty
            Me.txtComments.Text = String.Empty
            Me.lstCompany.DeSelect()
            Try
                Me.lstCompany.ListItem.SelectedIndex = 0
            Catch ex As Exception

            End Try

            Me.txtLoginID.Text = String.Empty
            Me.txtPassword.Text = String.Empty
            Me.txtConfirmPassword.Text = String.Empty
            Me.chkBlock.Checked = False
            Me.txtMobileNo.Text = String.Empty
            Me.txtUserName.Focus()

            ''Set New Mode and Applying Security Setting
            Call ApplySecurity(EnumDataMode.[New])

        Catch ex As Exception
            Throw ex
        End Try
    End Sub

    ''Here we will pass an argument MODE (New|Edit|Disabled), which will be overwritten according to the rights 
    ''available to user on that screen
    Public Sub ApplySecurity(ByVal Mode As Utility.Utility.EnumDataMode, Optional ByVal Condition As String = "") Implements IGeneral.ApplySecurity
        Try


            If Mode.ToString = EnumDataMode.Disabled.ToString Then

                btnNew.Enabled = False
                btnSave.Enabled = False
                btnUpdate.Enabled = False
                btnDelete.Enabled = False
                btnCancel.Enabled = False

                SetNavigationButtons(EnumDataMode.Edit)
                Me.grdAllRecords.Enabled = True

            ElseIf Mode.ToString = EnumDataMode.[New].ToString Then

                btnNew.Enabled = False

                If mobjControlList.Item("btnSave") Is Nothing Then
                    btnSave.Enabled = False
                Else
                    btnSave.Enabled = True
                End If

                btnUpdate.Enabled = False
                btnDelete.Enabled = False
                btnCancel.Enabled = True

                SetNavigationButtons(Mode)

                Me.grdAllRecords.Enabled = False

            ElseIf Mode.ToString = EnumDataMode.Edit.ToString Then

                btnNew.Enabled = True
                btnSave.Enabled = False

                If mobjControlList.Item("btnUpdate") Is Nothing Then
                    btnUpdate.Enabled = False
                Else
                    btnUpdate.Enabled = True
                End If

                If mobjControlList.Item("btnDelete") Is Nothing Then
                    btnDelete.Enabled = False
                Else
                    btnDelete.Enabled = True
                End If
                btnCancel.Enabled = False

                SetNavigationButtons(Mode)

                Me.grdAllRecords.Enabled = True

                Me.grdAllRecords.Focus()

            ElseIf Mode.ToString = EnumDataMode.ReadOnly.ToString Then

                btnNew.Enabled = True
                btnSave.Enabled = False


                btnUpdate.Enabled = False
                btnDelete.Enabled = False
                btnCancel.Enabled = False

                SetNavigationButtons(Mode)

                Me.grdAllRecords.Enabled = True

                Me.grdAllRecords.Focus()

            End If


            '' Disabl/Enable the Button that converts Grid data in Excel Sheet According to Login User rights
            If mobjControlList.Item("btnExport") Is Nothing Then
                Me.UiCtrlGridBar1.btnExport.Enabled = False
            End If


            '' Disabl/Enable the Button that Prints Grid data According to Login User rights
            If mobjControlList.Item("btnPrint") Is Nothing Then
                Me.UiCtrlGridBar1.btnPrint.Enabled = False
            End If


        Catch ex As Exception
            Throw ex
        End Try
    End Sub

    ''Here we will apply Front End Validations.
    Public Function IsValidate(Optional ByVal Mode As EnumDataMode = EnumDataMode.Disabled, Optional ByVal Condition As String = "") As Boolean Implements IGeneral.IsValidate

        Try
            ''casting selecting group to datarowview
            Dim dvr As DataRowView = CType(cboGroup.SelectedItem, DataRowView)

            ''1 First Check Front End Validations
            If Mode = EnumDataMode.[New] Or Mode = EnumDataMode.Edit Then

                ''check selecton for groups
                If Me.cboGroup.SelectedIndex <= 0 Then
                    ShowValidationMessage("Required Information missing ! ")
                    Me.cboGroup.Focus()
                    Return False

                    ''Check Name is Required
                ElseIf Me.txtUserName.Text.Trim = String.Empty Then
                    ShowValidationMessage("Required Information missing ! ")
                    Me.txtUserName.Focus()
                    Return False

                End If

                If Me.txtLoginID.Text.Trim = String.Empty Then
                    ShowValidationMessage("Required Information missing ! ")
                    Me.txtLoginID.Focus()
                    Return False
                End If

                If Me.txtPassword.Text.Trim = String.Empty Then
                    ShowValidationMessage("Password is required.")
                    Me.txtPassword.Focus()
                    Return False
                End If

                If Me.txtConfirmPassword.Text.Trim = String.Empty Then
                    ShowValidationMessage("Required Information missing ! ")
                    Me.txtConfirmPassword.Focus()
                    Return False
                End If

                If Me.txtPassword.Text.Trim <> Me.txtConfirmPassword.Text.Trim Then
                    ShowValidationMessage("Password and Confirm Password do not match.")
                    Me.txtPassword.Focus()
                    Return False
                End If

                If Me.lstCompany.SelectedIDs.Length = 0 Then
                    ShowValidationMessage("Please select at least one company for the user ! ")
                    Me.lstCompany.ListItem.Focus()
                    Return False
                End If

                If Mode = EnumDataMode.Edit Then
                    If Me.grdAllRecords.GetRow.Cells(EnumGrid.UserID).Text = gObjUserInfo.UserID Then
                        If Me.chkBlock.Checked = True Then
                            ShowValidationMessage("You cannot Block this user because its currently logged in")
                            Return False
                        End If
                    End If
                End If

            ElseIf Mode = EnumDataMode.Disabled Then

                'if row got the status of readonly then prompt user that it can not be modified
                If Me.grdAllRecords.GetRow.Cells(EnumGrid.UserEmail).Text.ToUpper = "READONLY" Then
                    ShowValidationMessage("You cannot delete this record")
                    Return False

                    'if user is currently login and want to delete the same user then stop
                ElseIf Me.grdAllRecords.GetRow.Cells(EnumGrid.UserID).Text = gObjUserInfo.UserID Then
                    ShowValidationMessage("You cannot delete this user because its currently logged in")
                    Return False

                    'ElseIf Not DropDefaultLocationRights(Me.grdAllRecords.GetRow.Cells(EnumGrid.UserID).Text) Then
                    '    ShowValidationMessage("Unable to drop the default user location")
                    '    Return False
                End If
            End If

            ''===========================================             
            Me.FillModel()

            ''Check Name or Code Duplication
            If Mode = EnumDataMode.[New] Then

                Call New SecurityUserDAL().IsValidateForSave(mobjModel)
            End If

            If Mode = EnumDataMode.Edit Then
                Call New SecurityUserDAL().IsValidateForUpdate(mobjModel)
            End If

            Return True

        Catch ex As Exception
            Throw ex
        End Try

    End Function

    ''Here we will create an instance of the class, according to the form, and will set the properties of the object
    ''Later this object will be refered in Save|Update|Delete function.
    Public Sub FillModel(Optional ByVal Condition As String = "") Implements IGeneral.FillModel

        Try
            ''Create Model object
            mobjModel = New SecurityUser
            With mobjModel
                ''Setting the Model Object Values
                .UserID = intPkId
                .GroupInfo.GroupID = Me.cboGroup.SelectedValue
                .UserName = Me.txtUserName.Text.Trim
                .UserEmail = Me.txtEmail.Text.Trim
                .MobileNo = Me.txtMobileNo.Text.Trim
                .UserComments = Me.txtComments.Text.Trim
                .LoginID = Me.txtLoginID.Text
                .LoginPassword = Me.txtPassword.Text
                .IsBlocked = Me.chkBlock.Checked

                .Companies = New List(Of Company)

                For Each obj As Object In Me.lstCompany.ListItem.SelectedItems
                    Dim drv As DataRowView = CType(obj, DataRowView)
                    Dim c As New Company
                    c.CompanyID = drv.Item(1)
                    c.CompanyName = drv.Item(0)
                    .Companies.Add(c)
                Next

                .ActivityLog = New ActivityLog
                .ActivityLog.ScreenTitle = Me.Text
                .ActivityLog.LogGroup = "Security"
                .ActivityLog.UserID = gObjUserInfo.UserID


            End With
        Catch ex As Exception
            Throw ex
        End Try

    End Sub

    ''Here we will call DAL Function for SAVE, and if the function successfully Saves the records
    ''then the function will return True, otherwise returns False
    Public Function Save(Optional ByVal Condition As String = "") As Boolean Implements IGeneral.Save
        Try

            ''Applying Front End Validation Checks
            If Me.IsValidate(EnumDataMode.[New]) Then

                Dim result As DialogResult = Windows.Forms.DialogResult.Yes

                If gblnShowSaveConfirmationMessages Then
                    ''Getting Save Confirmation from User
                    result = ShowConfirmationMessage("Do you want to save this record?", MessageBoxDefaultButton.Button1)
                End If

                If result = Windows.Forms.DialogResult.Yes Then

                    ''Create a DAL Object and calls its Add Method by passing Model Object
                    If New SecurityUserDAL().Add(Me.mobjModel) Then

                        ''Query to Database and get fressh modifications in the Grid
                        Me.GetAllRecords()

                        ''========
                        'to select the last updated record
                        For Rind As Int16 = 0 To (grdAllRecords.RowCount - 1)
                            If Me.grdAllRecords.GetRow(Rind).Cells(EnumGrid.UserID).Value = mobjModel.UserID Then
                                Me.grdAllRecords.Row = Rind
                                Exit For
                            End If
                        Next

                        Me.grdAllRecords_SelectionChanged(Nothing, Nothing)
                        ''========

                        ''Reset controls and set New Mode
                        Me.ReSetControls()

                    End If

                End If
            End If

        Catch ex As Exception
            If ex.Message = gstrDuplicateUserID Then
                ShowErrorMessage(ex.Message)
                Me.txtLoginID.Focus()
            Else
                Me.txtUserName.Focus()
                Throw ex
            End If
        End Try
    End Function

    ''Here we will call DAL Function for Update the selected record, and if the function successfully Updates the records
    ''then the function will return True, otherwise returns False
    Public Function Update(Optional ByVal Condition As String = "") As Boolean Implements IGeneral.Update
        Try

            ''Applying Front End Validation Checks
            If Me.IsValidate(EnumDataMode.Edit) Then

                Dim result As DialogResult = Windows.Forms.DialogResult.Yes
                ''Getting Save Confirmation from User
                result = ShowConfirmationMessage("Do you want to save changes in the selected record?", MessageBoxDefaultButton.Button1)

                If result = Windows.Forms.DialogResult.Yes Then

                    ''Create a DAL Object and calls its Update Method by passing Model Object
                    If New SecurityUserDAL().Update(Me.mobjModel) Then

                        ''Query to Database and get fressh modifications in the Grid
                        Me.GetAllRecords()

                        'to select the last updated record
                        For Rind As Int16 = 0 To (grdAllRecords.RowCount - 1)
                            If Me.grdAllRecords.GetRow(Rind).Cells(EnumGrid.UserID).Value = mobjModel.UserID Then
                                Me.grdAllRecords.Row = Rind
                                Exit For
                            End If
                        Next

                        Me.grdAllRecords_SelectionChanged(Nothing, Nothing)

                        'If gblnShowAfterUpdateMessages Then
                        '    ''Getting Save Confirmation from User
                        '    ShowInformationMessage(gstrMsgAfterUpdate)
                        'End If
                    End If
                End If
            End If
        Catch ex As Exception
            If ex.Message = gstrDuplicateUserID Then
                ShowErrorMessage(ex.Message)
                Me.txtLoginID.Focus()
            Else
                Me.txtUserName.Focus()
                Throw ex
            End If
        End Try
    End Function

    Public Function Delete(Optional ByVal Condition As String = "") As Boolean Implements IGeneral.Delete
        Try

            If Not Me.IsValidate(EnumDataMode.Disabled) Then Exit Function

            Dim result As DialogResult = Windows.Forms.DialogResult.Yes
            ''Getting Save Confirmation from User
            result = ShowConfirmationMessage("Do you want to Delete this record?", MessageBoxDefaultButton.Button2)


            If result = Windows.Forms.DialogResult.Yes Then

                ''Create a DAL Object and calls its Delete Method by passing Model Object
                If New SecurityUserDAL().Delete(Me.mobjModel) Then

                    ''This will hold row index of the selected row 
                    Dim intGridRowIndex As Integer
                    intGridRowIndex = Me.grdAllRecords.Row

                    ''Query to Database and get fressh modifications in the Grid
                    Me.GetAllRecords()

                    ''Call RowColumn Change Event
                    Me.grdAllRecords_SelectionChanged(Nothing, Nothing)

                    ''Reset the row index to the grid
                    If intGridRowIndex > (Me.grdAllRecords.RowCount - 1) Then intGridRowIndex = (Me.grdAllRecords.RowCount - 1)
                    If Not intGridRowIndex < 0 Then Me.grdAllRecords.Row = intGridRowIndex
                End If
            End If

        Catch ex As Exception
            Throw ex
        End Try
    End Function

#End Region


#Region "Local Functions and Procedures"



#End Region


#Region "Form Controls Events"

    Private Sub frmDefSecurityUser_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Try

            ''Getting all available controls list to the user on this screen; in a collection 
            mobjControlList = GetFormSecurityControls(Me.Name)

            ''Assing Images to Buttons
            Me.SetButtonImages()

            Me.FillCombos()


            ''Reset the controls for new mode
            Call ReSetControls()

        Catch ex As Exception
            ShowErrorMessage(ex.Message)
        End Try


    End Sub

    Private Sub frmDefCity_Activated(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Activated
        ''To avoid implecit call of rowcol chang event , We are assinging event handler at runtime.
        AddHandler grdAllRecords.SelectionChanged, AddressOf Me.grdAllRecords_SelectionChanged
    End Sub


    Private Sub btnSave_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSave.Click, btnUpdate.Click, btnDelete.Click, btnCancel.Click, btnExit.Click, btnNew.Click

        Try


            'Dim btn As C1.Win.C1Input.C1Button = CType(sender, C1.Win.C1Input.C1Button)
            Dim btn As Windows.Forms.Button = CType(sender, Windows.Forms.Button)

            ''If New Button is Clicked
            If btn.Name = btnNew.Name Then
                ''Refresh the controls for new mode
                Me.ReSetControls()
            ElseIf btn.Name = btnSave.Name Then
                '' Call Save method to save the record
                Me.Save()
            ElseIf btn.Name = btnUpdate.Name Then
                '' Call Update method to update the record
                Me.Update()
            ElseIf btn.Name = btnDelete.Name Then
                '' Call Delete method to delete the record
                Me.Delete()
            ElseIf btn.Name = btnCancel.Name Then
                ''Load Selected record in Edit Mode
                Me.grdAllRecords_SelectionChanged(Nothing, Nothing)
            ElseIf btn.Name = btnExit.Name Then
                Me.Close()
            End If

        Catch ex As Exception
            ShowErrorMessage(ex.Message)
        Finally
            mobjModel = Nothing
        End Try

    End Sub

    Private Sub btnFirst_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnFirst.Click, btnNext.Click, btnPrevious.Click, btnLast.Click

        Try

            Dim btn As Button = CType(sender, Button)

            ''If Move First is clicked, then navigate to first record of the grid
            If btn.Name = Me.btnFirst.Name Then
                Me.grdAllRecords.MoveFirst()
            ElseIf btn.Name = Me.btnPrevious.Name Then
                Me.grdAllRecords.MovePrevious()
            ElseIf btn.Name = Me.btnNext.Name Then
                Me.grdAllRecords.MoveNext()
            ElseIf btn.Name = Me.btnLast.Name Then
                Me.grdAllRecords.MoveLast()
            End If

        Catch ex As Exception
            Throw ex
        End Try

    End Sub

    Private Sub grdAllRecords_SelectionChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) 'Handles grdAllRecords.SelectionChanged
        Try

            ''If there is no record found in grid then load the screen in new mode
            If grdAllRecords.RowCount = 0 Then
                Me.ReSetControls()
                Exit Sub
            End If

            If Me.grdAllRecords.GetRow().RowType = Janus.Windows.GridEX.RowType.TotalRow Then
                Me.grdAllRecords.Row = (Me.grdAllRecords.Row - 1)
                Exit Sub
            End If

            ''set the values of the selected record in editable controls
            Me.intPkId = IIf(IsDBNull(Me.grdAllRecords.GetValue(EnumGrid.UserID).ToString), 0, Convert.ToInt32(Me.grdAllRecords.GetValue(EnumGrid.UserID).ToString))
            Me.txtUserName.Text = IIf(IsDBNull(Me.grdAllRecords.GetValue(EnumGrid.UserName).ToString), " ", Me.grdAllRecords.GetValue(EnumGrid.UserName).ToString)
            Me.txtEmail.Text = IIf(IsDBNull(Me.grdAllRecords.GetValue(EnumGrid.UserEmail).ToString), " ", Me.grdAllRecords.GetValue(EnumGrid.UserEmail).ToString)
            Me.txtComments.Text = IIf(IsDBNull(Me.grdAllRecords.GetValue(EnumGrid.UserComments).ToString), " ", Me.grdAllRecords.GetValue(EnumGrid.UserComments).ToString)
            Me.txtLoginID.Text = IIf(IsDBNull(Me.grdAllRecords.GetValue(EnumGrid.UserLogID).ToString), " ", Me.grdAllRecords.GetValue(EnumGrid.UserLogID).ToString)
            Me.txtPassword.Text = IIf(IsDBNull(Me.grdAllRecords.GetValue(EnumGrid.UserLogPassword).ToString), " ", DecryptWithALP(Me.grdAllRecords.GetValue(EnumGrid.UserLogPassword).ToString))
            Me.txtConfirmPassword.Text = IIf(IsDBNull(Me.grdAllRecords.GetValue(EnumGrid.UserLogPassword).ToString), " ", DecryptWithALP(Me.grdAllRecords.GetValue(EnumGrid.UserLogPassword).ToString))
            Me.txtMobileNo.Text = IIf(IsDBNull(Me.grdAllRecords.GetValue(EnumGrid.MobileNo).ToString), " ", Me.grdAllRecords.GetValue(EnumGrid.MobileNo).ToString)
            Me.chkBlock.Checked = Me.grdAllRecords.GetRow.Cells(EnumGrid.Block).Value


            Me.lstCompany.DeSelect()
            'select User Companies
            Dim strIDs As String = Me.GetUserCompanies(Me.intPkId)
            If strIDs.Length > 0 Then
                Me.lstCompany.SelectItemsByIDs(strIDs)
            Else
                Me.lstCompany.DeSelect()
            End If

            Call ApplySecurity(EnumDataMode.Edit)

        Catch ex As Exception
            ShowErrorMessage(ex.Message)
        End Try

    End Sub

    Private Sub grdAllRecords_LoadingRow(ByVal sender As Object, ByVal e As Janus.Windows.GridEX.RowLoadEventArgs) Handles grdAllRecords.LoadingRow

        Try

            If e.Row.RowType = Janus.Windows.GridEX.RowType.TotalRow Then

                ''to view the Total Records in Grid Footer
                Dim dv As DataView = GetFilterDataFromDataTable(CType(gObjMyAppHashTable.Item(EnumHashTableKeyConstants.GetLanguageBasedControlList.ToString()), DataTable), "[Control Type]= 'DataDictionary'  AND [Control Name] = 'GridRowCount'")
                Dim strTotalRecords As String = String.Empty
                If Not dv Is Nothing Then
                    If Not dv.Count = 0 Then
                        strTotalRecords = dv.Item(0).Item(dv.Table.Columns(gstrSystemLanguage).ColumnName)
                    End If
                End If

                e.Row.Cells(EnumGrid.UserName).Text = strTotalRecords & " (" & Me.grdAllRecords.GetTotal(Me.grdAllRecords.RootTable.Columns(EnumGrid.UserName), Janus.Windows.GridEX.AggregateFunction.Count) & ") "

            End If

        Catch ex As Exception
            ShowErrorMessage(ex.Message)
        End Try

    End Sub

    Private Sub frmDefSecurityGroup_KeyDown(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles MyBase.KeyDown
        Try
            If e.Control And e.KeyCode = Keys.S Then
                If Me.btnSave.Enabled = True Then Me.Save()
            ElseIf e.Control And e.KeyCode = Keys.U Then
                If Me.btnUpdate.Enabled = True Then Me.Update()
            ElseIf e.Control And e.KeyCode = Keys.D Then
                ' If Me.btnDelete.Enabled = True Then Me.Delete()
                ShowInformationMessage(gstrMsgDeleteByEndDate)
            ElseIf e.Control And e.KeyCode = Keys.N Then
                If Me.btnNew.Enabled = True Then Me.ReSetControls()
            ElseIf e.Control And e.KeyCode = Keys.E Then
                If Me.btnCancel.Enabled = True Then Me.grdAllRecords_SelectionChanged(Nothing, Nothing)
            ElseIf e.Control And e.KeyCode = Keys.X Then
                If Me.btnExit.Enabled = True Then Me.Close()
            End If
        Catch ex As Exception
            ShowErrorMessage(ex.Message)
        End Try
    End Sub

    Private Sub cboGroup_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles cboGroup.SelectedIndexChanged
        Try
            If Me.cboGroup.SelectedIndex > 0 Then
                Me.GetAllRecords()
            End If
        Catch ex As Exception
            Throw ex
        End Try
    End Sub

    Private Function GetUserCompanies(ByVal UserID) As String
        Try
            Dim strsql As String = "select location_id from tblglsecurityuserlocation where user_id =  " & UserID
            Dim dt As DataTable = UtilityDAL.GetDataTable(strsql)
            Dim strIDs As String = String.Empty
            If dt.Rows.Count > 0 Then
                For Each dr As DataRow In dt.Rows
                    If strIDs.Length = 0 Then
                        strIDs = dr.Item(0)
                    Else
                        strIDs = strIDs & "," & dr.Item(0)
                    End If
                Next
            End If
            Return strIDs
        Catch ex As Exception
            Throw ex
        End Try
    End Function


    'This function will drop the default user rights to default location
    'Private Function DropDefaultLocationRights(ByVal lngUserID As Long) As Boolean
    '    Try

    '        'local variables
    '        Dim lngUserLocationID As Long        'store the new saved id

    '        'set the default value in return value
    '        DropDefaultLocationRights = True

    '        'this sql will get the locations
    '        Dim strSql As String = "SELECT location_id, location_code FROM tblGlDefLocation"

    '        Dim dt As DataTable = UtilityDAL.GetDataTable(strSql)
    '        'check if there exits the locations
    '        If Not dt.Rows.Count = 0 Then

    '            'if only one location is defined then assign the location rights
    '            If dt.Rows.Count = 1 Then

    '                'get the newly added user id
    '                strSql = "SELECT user_location_id From tblGLSecurityUserLocation WHERE (user_id = " & lngUserID & ")"

    '                Dim dr As DataRow = UtilityDAL.ReturnDataRow(strSql)
    '                If Not dr Is Nothing Then
    '                    lngUserLocationID = dr.Item(0)

    '                    'insert the newly added user rights with location
    '                    strSql = "DELETE FROM tblGLSecurityUserLocation WHERE (user_location_id = " & lngUserLocationID & ")"
    '                    DropDefaultLocationRights = UtilityDAL.ExecuteQuery(strSql)
    '                End If
    '            End If
    '        End If

    '    Catch ex As Exception
    '        DropDefaultLocationRights = False
    '        Throw ex
    '    End Try


    'End Function


    Private Sub txtUserName_Leave(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles txtUserName.Leave
        Me.txtLoginID.Text = Me.txtUserName.Text.Trim
    End Sub

#End Region



End Class