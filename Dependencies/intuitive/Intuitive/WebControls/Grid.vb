Imports System.Web.UI
Imports System.Text
Imports System.Configuration
Imports Intuitive
Imports Intuitive.Functions

Namespace WebControls
	Public Class Grid
		Inherits ControlBase
		Implements INamingContainer

		Private sSourceSql As String = ""
		Private sEditImage As String = "Edit.gif"
		Private sDeleteImage As String = "Delete.gif"
		Private oColumns As New GridColumns
		Public oColumnGroups As New ColumnGroups
		Private oListCommands As New ListCommands
		Private iExtraRows As Integer = 10
		Private oPostValues(,) As Object = Nothing
		Private aPostIDs As New ArrayList
		Private aPostGroups As New ArrayList
		Private sLastWarning As String = ""
		Private bCleared As Boolean = False
		Private bGridNavigation As Boolean = False
		Private dlastDate As Date = DateFunctions.EmptyDate
		Private bAddEdit As Boolean = False
		Private bAddDelete As Boolean = False
		Private sIDColumn As String = ""
		Private RemoveGridRowID As Integer = 0
		Private aErrorIDs As New ArrayList
		Private sDuplicateColumn As String = ""
		Private bSuppressTableCSSClass As Boolean = True
		Private sGroupColumn As String = ""
		Private bAllowDragCopy As Boolean = False
		Private bEnabled As Boolean = True
		Private iSortColumn As Integer = -1
		Private bSortAscending As Boolean
		Private sRowClassConditionColumn As String
		Private sConditionalRowClass As String


		Public DynamicControl As Boolean = False

#Region "Properties"
		Public Property SourceSql() As String
			Get
				Return sSourceSql
			End Get
			Set(ByVal Value As String)
				sSourceSql = Value
				Clear()
			End Set
		End Property
		Public Property Columns() As GridColumns
			Get

				If Me.HasGroups Then
					oColumns.Clear()

					For Each oGroup As ColumnGroup In oColumnGroups
						For Each oCol As GridColumn In oGroup.Columns
							oColumns.Add(oCol)
						Next
					Next
				End If

				Return oColumns
			End Get
			Set(ByVal Value As GridColumns)
				oColumns = Value
			End Set
		End Property

		Public ReadOnly Property HasGroups As Boolean
			Get
				Return oColumnGroups.Count > 0
			End Get
		End Property

		Public Property ExtraRows() As Integer
			Get
				Return iExtraRows
			End Get
			Set(ByVal Value As Integer)
				iExtraRows = Value
			End Set
		End Property

		Public Property LastWarning() As String
			Get
				Return sLastWarning
			End Get
			Set(ByVal Value As String)
				sLastWarning = Value
			End Set
		End Property

		Public Property GridNavigation() As Boolean
			Get
				Return bGridNavigation
			End Get
			Set(ByVal Value As Boolean)
				bGridNavigation = Value
			End Set
		End Property

		Public Property LastDate() As Date
			Get
				Return dlastDate
			End Get
			Set(ByVal Value As Date)
				dlastDate = Value
			End Set
		End Property

		Public ReadOnly Property RowCount() As Integer
			Get
				Dim oDataTable As DataTable = Me.GetDataTable
				If oDataTable IsNot Nothing Then
					Return oDataTable.Rows.Count
				Else
					Return 0
				End If
			End Get
		End Property

		Public Property PostData() As Object(,)
			Get
				Return oPostValues
			End Get
			Set(ByVal Value As Object(,))
				oPostValues = Value
			End Set
		End Property

		Public Property ListCommands() As ListCommands
			Get
				Return oListCommands
			End Get
			Set(ByVal Value As ListCommands)
				oListCommands = Value
			End Set
		End Property

		Public Property AddDelete() As Boolean
			Get
				Return bAddDelete
			End Get
			Set(ByVal Value As Boolean)
				bAddDelete = Value
			End Set
		End Property

		Public Property AddEdit() As Boolean
			Get
				Return bAddEdit
			End Get
			Set(ByVal Value As Boolean)
				bAddEdit = Value
			End Set
		End Property

		Public Property DataTable() As DataTable
			Get
				Return Me.GetDataTable
			End Get
			Set(ByVal Value As DataTable)
				Context.Session("grid" & Me.ID & Page.ToString) = Value.DefaultView
				Me.ClearPostValues()
			End Set
		End Property

		Public Property IDColumn() As String
			Get
				Return sIDColumn
			End Get
			Set(ByVal Value As String)
				sIDColumn = Value
			End Set
		End Property

		Public ReadOnly Property HashValue() As String
			Get
				Dim iHash As Integer = Me.ID.GetHashCode Mod 100
				If iHash < 0 Then iHash = iHash * -1

				Return iHash.ToString
			End Get
		End Property

		Public Property ErrorIDs() As ArrayList
			Get
				Return aErrorIDs
			End Get
			Set(ByVal Value As ArrayList)
				aErrorIDs = Value
			End Set
		End Property

		Public Property DuplicateColumn() As String
			Get
				Return sDuplicateColumn
			End Get
			Set(ByVal Value As String)
				sDuplicateColumn = Value
			End Set
		End Property

		Public ReadOnly Property SuperHeader() As Boolean
			Get
				'loop through the grid columns and return true if we find a superheader
				For Each oGridColumn As GridColumn In Me.oColumns
					If oGridColumn.SuperHeader <> "" Then
						Return True
					End If
				Next
				Return False
			End Get
		End Property

		Public Property SuppressTableCSSClass() As Boolean
			Get
				Return bSuppressTableCSSClass
			End Get
			Set(ByVal Value As Boolean)
				bSuppressTableCSSClass = Value
			End Set
		End Property

		Public Property GroupColumn() As String
			Get
				Return sGroupColumn
			End Get
			Set(ByVal value As String)
				sGroupColumn = value
			End Set
		End Property

		Public Property AllowDragCopy() As Boolean
			Get
				Return bAllowDragCopy
			End Get
			Set(value As Boolean)
				bAllowDragCopy = value
			End Set
		End Property

		Public Property Enabled() As Boolean
			Get
				Return bEnabled
			End Get
			Set(value As Boolean)
				bEnabled = value
			End Set
		End Property
		Public Property SortColumn() As Integer
			Get
				Return iSortColumn
			End Get
			Set(ByVal Value As Integer)
				iSortColumn = Value
			End Set
		End Property
		Public Property RowClassConditionColumn() As String
			Get
				Return sRowClassConditionColumn
			End Get
			Set(ByVal Value As String)
				sRowClassConditionColumn = Value
			End Set
		End Property
		Public Property ConditionalRowClass() As String
			Get
				Return sConditionalRowClass
			End Get
			Set(ByVal Value As String)
				sConditionalRowClass = Value
			End Set
		End Property
#End Region

#Region "Viewstate Management"

		Protected Overrides Function SaveViewState() As Object
			Dim oState(15) As Object

			oState(0) = MyBase.SaveViewState
			oState(1) = sSourceSql
			oState(2) = oColumns
			oState(3) = dlastDate
			oState(4) = bGridNavigation
			oState(5) = oListCommands.GetViewState
			oState(6) = bAddEdit
			oState(7) = bAddDelete
			oState(8) = sIDColumn
			oState(9) = sDuplicateColumn
			oState(10) = sGroupColumn
			oState(11) = bAllowDragCopy
			oState(12) = oColumnGroups
			oState(13) = bEnabled
			oState(14) = iSortColumn
			oState(15) = bSortAscending

			Return oState
		End Function

		Protected Overrides Sub LoadViewState(ByVal savedState As Object)

			If Not savedState Is Nothing Then
				Dim oState As Object() = CType(savedState, Object())

				MyBase.LoadViewState(oState(0))

				sSourceSql = CType(oState(1), String)
				oColumns = CType(oState(2), GridColumns)
				dlastDate = CType(oState(3), Date)
				bGridNavigation = CType(oState(4), Boolean)
				oListCommands.RestoreFromViewState(oState(5).ToString)
				bAddEdit = CType(oState(6), Boolean)
				bAddDelete = CType(oState(7), Boolean)
				sIDColumn = oState(8).ToString
				sDuplicateColumn = oState(9).ToString
				sGroupColumn = oState(10).ToString
				bAllowDragCopy = SafeBoolean(oState(11))
				oColumnGroups = CType(oState(12), ColumnGroups)
				bEnabled = SafeBoolean(oState(13))
				iSortColumn = SafeInt(oState(14))
				bSortAscending = SafeBoolean(oState(15))
			End If
		End Sub

#End Region

#Region "AddColumn"

		Public Sub AddColumn(ByVal GridColumn As WebControls.GridColumn)
			oColumns.Add(GridColumn)
		End Sub

		Public Sub AddColumn(
			ByVal Caption As String,
			Optional ByVal GridControlType As GridControlType = GridControlType.Textbox,
			Optional ByVal GridDataType As GridDataType = GridDataType.Int,
			Optional ByVal MaxColumnLength As Integer = 0,
			Optional ByVal CSSClass As String = "",
			Optional ByVal ColumnID As Integer = 0,
			Optional ByVal AllowCopy As Boolean = False,
			Optional ByVal OnBlur As String = "",
			Optional ByVal OnKeyUp As String = "",
			Optional ByVal SuperHeader As String = "",
			Optional ByVal OnChange As String = "",
			Optional ByVal ExtraColumnClass As String = "",
			Optional ByVal UniqueName As String = "",
			Optional ByVal OnFocus As String = "",
			Optional ByVal DataColumn As String = "",
			Optional ByVal GroupColumn As String = "",
			Optional ByVal DecimalPlacesRestriction As Integer = 0,
			Optional ByVal [ReadOnly] As Boolean = False)

			oColumns.Add(New GridColumn(Caption, GridControlType, GridDataType, MaxColumnLength, CSSClass, ColumnID,
			  AllowCopy, OnBlur, OnKeyUp, SuperHeader, OnChange, ExtraColumnClass, UniqueName, OnFocus,
			  DataColumn, GroupColumn, DecimalPlacesRestriction, [ReadOnly]))

		End Sub

#End Region

#Region "render"
		Protected Overrides Sub Render(ByVal writer As System.Web.UI.HtmlTextWriter)
			Dim dtSource As DataTable = Nothing
			Dim oSB As New StringBuilder
			Dim iHeader As Integer = 1
			Dim iColumn As Integer
			Dim iRow As Integer
			Dim bReDraw As Boolean
			Dim oDropDowns As New Hashtable
			Dim oListCommand As ListCommand
			Dim sHyperlink As String = "<a href="" {0}"" ><img src="" {1}"" alt=""{2}"" title=""{2}""/></a>"
			Dim sHashedID As String = GetHashedID()
			Dim iID As Integer

			'first work out if we are going to regenerate the data
			'or redisplay from the postcollection
			If oPostValues Is Nothing OrElse bCleared Then
				bReDraw = True
			End If

			'get the data if we are redrawing
			If bReDraw Then
				dtSource = Me.GetDataTable
			End If

			'replace columns with what we have in grouped columns
			oColumns = Me.Columns

			'set up the holding div
			oSB.AppendFormat("<div id="" {0}"" class=""grid{2}{1}"">\n",
			 Me.ID, IIf(Me.CSSClass = "", "", " " & Me.CSSClass).ToString,
			 IIf(Me.SuperHeader, " superheader", "").ToString)

			'Add Edit and Delete if needed
			If bAddEdit Then
				Me.AddListCommand(sEditImage, "E", "Edit Item")
			End If

			If bAddDelete Then
				Me.AddListCommand(sDeleteImage, "D", "Delete Item", True)
			End If

			'create the header
			oSB.Append("<table class="" header"">\n")

			'create the super header row if needed
			If Me.SuperHeader Then

				Dim oGridColumn As GridColumn
				Dim iColspan As Integer
				Dim sLastSuperHeader As String = "cockpisspartridge"
				Dim sSuperHeader As String = ""

				oSB.Append("<tr class="" superheader"">\n")
				For iColumnCount As Integer = 0 To Me.oColumns.Count - 1

					oGridColumn = oColumns(iColumnCount)

					'check that if it's the first column it has a superheader
					If iColumnCount = 0 AndAlso oGridColumn.SuperHeader = "" Then
						Throw New Exception("You must specify a Superheader for the first grid column")
					End If

					If oGridColumn.SuperHeader <> "" Then
						sSuperHeader = oGridColumn.SuperHeader
					End If

					'open the table header if needed, reset colspan to 0
					If sSuperHeader <> sLastSuperHeader AndAlso sSuperHeader <> "" Then

						oSB.Append("<th ")
						sLastSuperHeader = sSuperHeader
						iColspan = 0
					End If

					'increment the colspan
					iColspan += 1

					'work out if we need to close the cell
					If iColumnCount = Me.oColumns.Count - 1 OrElse
					 (sSuperHeader <> oColumns(iColumnCount + 1).SuperHeader AndAlso
					 oColumns(iColumnCount + 1).SuperHeader <> "") Then

						'close the cell
						oSB.AppendFormat("colspan=""{0}"">{1}</th>", iColspan, sLastSuperHeader)

					End If
				Next

				'add the cell to go above the scrollbar
				oSB.Append("<th class="" last"">&nbsp;</th>")

				'close the row
				oSB.Append("</tr>\n")
			End If

			If Me.HasGroups Then
				Dim iGroupCount As Integer = 1
				oSB.Append("<tr>")
				Dim sGroupColumn As String = "<th class="" g{0} group"" colspan=""{1}"">{2}</th>\n"

				For Each oGroup As ColumnGroup In oColumnGroups
					oSB.AppendFormat(sGroupColumn, iGroupCount, oGroup.Columns.Count, oGroup.GroupName)
					iGroupCount += 1
				Next

				oSB.Append("<th class="" last group"">&nbsp;</th>")

				oSB.Append("</tr>\n")
			End If

			'create the normal header row
			oSB.Append("<tr>")

			'create the header columns
			' Dim sHeaderColumn As String = "<th class="" c{1}{2}"">{0}</th>\n"
			Dim sCopyHeaderColumn As String = "<th class="" c{1}{4}"">" &
			 "<a href="" javascript:CopyColumn({3},{2});{5}"" class=""gridcopy"">{0}</a></th>\n"
			Dim sSortHeaderColumn As String = "<th class="" c{1}{4}"">" &
			 "<a href="" javascript:Postback('{3}HeaderClick',{2})"" >{0}</a></th>\n"


			Dim oColumn As GridColumn
			Dim sExtraClass As String = ""
			For Each oColumn In oColumns
				sExtraClass = IIf(oColumn.ExtraColumnClass <> "", " " & oColumn.ExtraColumnClass, "")
				If Not oColumn.AllowCopy Then
					oSB.Append(String.Format(sSortHeaderColumn, oColumn.Caption, iHeader, iHeader - 1,
					 Me.GetHashedID, sExtraClass))
				Else
					oSB.Append(String.Format(sCopyHeaderColumn, oColumn.Caption, iHeader, iHeader - 1,
					 Me.HashValue, sExtraClass, oColumn.PostCopyScript))
				End If

				iHeader += 1

			Next

			'add a header div for the list commands
			For Each oListCommand In oListCommands
				oSB.Append("<th class="" dlop""></th>")
			Next

			oSB.Append("<th class="" last"">&nbsp;</th>")

			'close header row and table
			oSB.Append("</tr>\n</table>\n")

			'start scrolling div and table
			Dim sScrollScript As String = IIf(Me.AllowDragCopy, "onmouseout=""oGridFunction.Mouseout(this);""", "")
			oSB.AppendFormat("<div class="" scroll"" {0}>\n", sScrollScript)
			oSB.Append("<table class="" grid"">\n")

			'add hidden variables
			Dim oHiddenMaxColumn As New WebControls.Hidden
			With oHiddenMaxColumn
				.ID = "hidGridMaxColumnNumber"
				.Value = (Me.Columns.Count - 1).ToString
			End With

			oSB.Append(RenderControlToString(oHiddenMaxColumn))

			Dim oHiddenMaxRow As New WebControls.Hidden
			With oHiddenMaxRow
				.ID = "hidGridMaxRowNumber"
				.Value = (Me.RowCount + Me.ExtraRows - 1).ToString
			End With

			oSB.Append(RenderControlToString(oHiddenMaxRow))

			Dim oHiddenHashCode As New WebControls.Hidden
			With oHiddenHashCode
				.ID = "hidGridHashCode"
				.Value = (Me.HashValue).ToString
			End With

			oSB.Append(RenderControlToString(oHiddenHashCode))

			'spasticated method of getting the data row count, iif seems to evaluate all the
			'bits for some stupid reason
			Dim iDataRowCount As Integer
			If bReDraw Then
				If Not dtSource Is Nothing Then
					iDataRowCount = dtSource.Rows.Count
				Else
					iDataRowCount = 0
				End If
			Else
				iDataRowCount = oPostValues.GetUpperBound(0) + 1
			End If

			'need to scan through and see if we have any drop down lists
			'if we do then (trouble) build up the select/option stuff
			'and add to a hash table
			For Each oColumn In oColumns

				If oColumn.ControlType = GridControlType.DropDown Then
					Dim oDropDown As New StringBuilder
					Dim sSplit() As String
					Dim sValue As String

					'start and a blank option
					oDropDown.Append("<td class="" {0}""><select name="" c{1}_{2}_{3}"" id=""c{1}_{2}_{3}""{4}>")

					If oColumn.AddBlankOption Then
						oDropDown.Append("<option></option>")
					End If

					'options
					For Each sValue In oColumn.Values
						sSplit = sValue.Split(CType("#", Char))
						If sSplit.Length > 1 Then
							oDropDown.AppendFormat("<option value="" {0}"">{1}</option>", sSplit(0), sSplit(1))
						Else
							oDropDown.AppendFormat("<option value="" {0}"">{0}</option>", sValue)
						End If
					Next

					'end
					oDropDown.Append("</select></td>\n")

					'add to hash table
					oDropDowns.Add(oColumn.Caption, oDropDown.ToString)
				End If
			Next

			'set up default string for textbox entries
			Dim sCellScript As String = IIf(Me.AllowDragCopy, "onmousedown=""oGridFunction.Click(this);"" onmouseover=""oGridFunction.MouseOver(this);"" onmouseup=""oGridFunction.Unclick(this);"" onmouseout=""oGridFunction.Mouseout(this);""", "")
			Dim sTextboxScript As String = IIf(Me.AllowDragCopy, "onmousedown=""oGridFunction.Click(this.parentNode);"" onmouseover=""oGridFunction.MouseOver(this.parentNode);"" onmouseup=""oGridFunction.Unclick(this.parentNode);"" ", "")

			Dim sTextbox As String = "<td unselectable="" on"" class="" {5}{9}"" " & sCellScript & ">" &
			   "<input type="" text"" autocomplete="" off"" name="" c{0}_{1}_{8}"" title=""{2}"" value=""{2}"" " &
			  "class=""{4} textbox"" {3} {6} {7} onfocus=""{10}"" id=""c{0}_{1}_{8}"" " &
			  sTextboxScript & "/></td>\n"

			'set up default string for checkbox entries
			Dim sCheckbox As String = "<td class="" {3}""><input type="" checkbox"" name="" c{0}_{1}_{4}"" " &
			 "{2} class=""checkbox"" id=""c{0}_{1}"" {5}/></td>\n"

			'for each row to be displayed
			Dim sLastGroupColumn As String = "cockpisspartridge"
			Dim sCurrentGroupColumn As String = ""
			For iRow = 0 To iDataRowCount + iExtraRows - 1

				'If we have just removed a row then do a check on the ID to see if we want to render this row
				If Me.IDColumn = "" OrElse Me.RemoveGridRowID = 0 OrElse
				   SafeInt(Me.Page.Request(String.Format("gid_{0}_{1}", iRow, Me.HashValue))) <> RemoveGridRowID Then

					'get the current grouping column
					If sGroupColumn <> "" AndAlso Not dtSource Is Nothing AndAlso iRow < iDataRowCount Then
						If bReDraw Then
							sCurrentGroupColumn = dtSource.Rows(iRow)(sGroupColumn).ToString
						Else
							sCurrentGroupColumn = aPostGroups(iRow).ToString
						End If

					End If

					'if we've got a group column and it's time to render then render
					If sGroupColumn <> "" AndAlso sCurrentGroupColumn <> sLastGroupColumn Then

						oSB.AppendFormat("<tr class=""lstgrouprow"" {2}><td colspan=""{0}"">{1}</td></tr>\n",
						   oColumns.Count, sCurrentGroupColumn, IIf(Me.AllowDragCopy, "unselectable=""on""", ""))
					End If

					'start the row
					If Me.IDColumn = "" OrElse Not iRow <= (iDataRowCount - 1) OrElse Me.ErrorIDs.Count = 0 Then

						Dim sClass As String = String.Empty

						If Not String.IsNullOrEmpty(Me.ConditionalRowClass) AndAlso
							Not String.IsNullOrEmpty(Me.RowClassConditionColumn) AndAlso
							dtSource IsNot Nothing AndAlso
							dtSource.Rows.Count > iRow AndAlso
							SafeBoolean(dtSource.Rows(iRow)(Me.RowClassConditionColumn)) Then

							sClass = $"class=""{Me.ConditionalRowClass}"""

						End If


						oSB.AppendFormat("<tr{0} {1} {2}>", IIf(iRow = iDataRowCount + iExtraRows - 1,
						  " class=""lastrow""", ""), IIf(Me.AllowDragCopy, "unselectable=""on""", ""), sClass)
					Else

						'work out the current rowid
						Dim iRowID As Integer
						If bReDraw Then
							iRowID = SafeInt(dtSource.Rows(iRow)(Me.IDColumn))
						Else
							iRowID = SafeInt(aPostIDs(iRow))
						End If

						If Me.ErrorIDs.Contains(iRowID) Then
							oSB.AppendFormat("<tr class=""rowerror{0}"">", IIf(iRow = iDataRowCount + iExtraRows - 1,
							 " lastrow", ""))
						Else
							oSB.Append("<tr>")
						End If
					End If

					'add in hidden input for the grouping
					If sGroupColumn <> "" Then

						oSB.AppendFormat("<input type=""hidden"" id=""ggrp_{0}_{1}"" name=""ggrp_{0}_{1}"" " &
						 "value=""{2}""/>", iRow, Me.HashValue, sCurrentGroupColumn)
					End If

					'for each column
					For iColumn = 0 To oColumns.Count - 1
						oColumn = oColumns(iColumn)
						Dim sColumnClass As String = "c" & (iColumn + 1).ToString
						Dim sValue As String = ""
						Dim sClass As String = ""
						Dim sOnEnter As String = ""
						Dim sMaxLength As String = ""
						Dim sOnBlur As String = oColumn.OnBlur
						Dim sOnKeyUp As String = oColumn.OnKeyUp

						sExtraClass = IIf(oColumn.ExtraColumnClass <> "", " " & oColumn.ExtraColumnClass, "")

						'set the class and onenter values depending on the
						'data type and format value if required
						Select Case oColumn.DataType
							Case GridDataType.Money, GridDataType.SmallMoney
								sClass = "grdnum"
								sOnEnter = String.Format("onkeypress=""return NumberOnly(event,{2});""{0}{1}",
								 IIf(sOnBlur <> "", " onBlur=""" & sOnBlur & """", ""),
								 IIf(sOnKeyUp <> "", " onkeyup=""" & sOnKeyUp & """", ""),
								 oColumn.DecimalPlaceRestriction)
							Case GridDataType.MoneyWithNA
								sClass = "grdnum"
								sOnEnter = String.Format("onkeypress=""return Grid_NumberOnlyWithNA(this, event);""{0}{1}",
								 IIf(sOnBlur <> "", " onBlur=""Grid_NumberOnlyWithNAValidate(this); " & sOnBlur & """", " onBlur=""Grid_NumberOnlyWithNAValidate(this);"""),
								   IIf(sOnKeyUp <> "", " onkeyup=""" & sOnKeyUp & """", ""))
							Case GridDataType.Int
								sClass = "grdnum"
								sOnEnter = String.Format("onkeypress=""return IntegerOnly(event);""{0}{1}",
								 IIf(sOnBlur <> "", " onBlur=""" & sOnBlur & """", ""),
								 IIf(sOnKeyUp <> "", " onkeyup=""" & sOnKeyUp & """", ""))
							Case GridDataType.IntMinus
								sClass = "grdnum"
								sOnEnter = String.Format("onkeypress=""return IntegerOnly(event);""{0}{1}",
								 IIf(sOnBlur <> "", " onBlur=""" & sOnBlur & """", ""),
								 IIf(sOnKeyUp <> "", " onkeyup=""" & sOnKeyUp & """", ""))
							Case GridDataType.Text
								If oColumn.ControlType = GridControlType.Textbox Then
									sClass = "grdtext"
								Else
									sClass = "grdddl"
								End If

								If sOnBlur <> "" Then
									sOnEnter = "onBlur=""" & sOnBlur & """"
								End If

								If sOnKeyUp <> "" Then
									sOnEnter += String.Format(" onkeyup=""javascript:{0}""", sOnKeyUp)
								End If
							Case GridDataType.GridDate
								sClass = "grddate"
								sOnEnter = String.Format("onBlur=""javascript:ParseDate(this);{0}"" {1}",
								 sOnBlur, IIf(sOnKeyUp <> "",
								 String.Format(" onkeyup=""javascript:{0}""", sOnKeyUp), ""))
							Case GridDataType.Time
								sClass = "grdtime"
								sOnEnter = String.Format("onBlur=""{0}"" onkeypress=""{1}""", "ParseTime(this);", "return TimeOnly(event);")

						End Select

						'if we're doing a textbox and the data type is text
						'the check the max length

						If oColumn.DataType = GridDataType.Text AndAlso
						  oColumn.ControlType = GridControlType.Textbox AndAlso
						  oColumn.MaxFieldLength > 0 Then
							sMaxLength = String.Format("maxlength=""{0}""",
							 oColumn.MaxFieldLength)
						End If

						'work out the value
						'if we are on a current row (i.e. one with data)
						'otherwise we'll just use the default value (i.e. "") of sValue
						If iRow <= (iDataRowCount - 1) Then

							'either get value from the table (if redraw) or from posted values
							If bReDraw Then
								If oColumn.DataColumn <> "" Then
									sValue = dtSource.Rows(iRow)(oColumn.DataColumn).ToString()
								Else
									sValue = dtSource.Rows(iRow)(iColumn).ToString()
								End If
							ElseIf iRow <= oPostValues.GetUpperBound(0) _
							  AndAlso iColumn <= oPostValues.GetUpperBound(1) _
							  AndAlso Not oPostValues(iRow, iColumn) Is Nothing Then
								Try
									sValue = oPostValues(iRow, iColumn).ToString
								Catch
									sValue = ""
								End Try
							End If

							'if the column is money and the value is numeric then
							'format with 2dps
							Dim nValue As Double
							Dim bDouble As Boolean = Double.TryParse(sValue, nValue)
							Select Case oColumn.DataType
								Case GridDataType.Money, GridDataType.MoneyWithNA
									If bDouble Then
										sValue = nValue.ToString("###,##0.00")
									End If
								Case GridDataType.GridDate
									If Validators.IsDisplayDate(sValue) Then
										sValue = sValue.ToString
									ElseIf DateFunctions.IsDate(sValue) Then
										sValue = DateFunctions.DisplayDate(DateTime.Parse(sValue))
									Else
										sValue = sValue.ToString
									End If
								Case GridDataType.SmallMoney
									If bDouble Then
										sValue = nValue.ToString("###,##0.00000")
									End If

							End Select
						End If

						'add additional cssclass information off the column if
						'it has been specified
						If oColumn.CSSClass <> "" Then
							sClass += " " & oColumn.CSSClass()
						End If

						'cheeky little readonly check
						'e-i-e-i-o
						Dim bReadOnly As Boolean = oColumn.ReadOnly
						If Not dtSource Is Nothing AndAlso oColumn.ReadOnlyIfColumn <> "" Then
							bReadOnly = dtSource.Rows(iRow)(oColumn.ReadOnlyIfColumn).ToString = oColumn.ReadOnlyIfValue
						End If

						'if this isn't enabled we ant to override the readonly to always be true
						If Not Me.Enabled Then bReadOnly = True

						'write out textbox or dropdown or checkbox
						If oColumn.ControlType = GridControlType.Textbox AndAlso Not bReadOnly Then
							Dim sUpDown As String = "onkeydown=""return GridMoveKey();"""

							oSB.AppendFormat(sTextbox, iRow, iColumn, sValue,
							 sOnEnter, sClass,
							 sColumnClass & IIf(bSuppressTableCSSClass, "", " " & oColumn.CSSClass).ToString,
							 sMaxLength, IIf(bGridNavigation, sUpDown, "").ToString, Me.HashValue, sExtraClass, oColumn.OnFocus)

						ElseIf oColumn.ControlType = GridControlType.DropDown Then

							'cheeky jink here to select those of the correct value
							'oh-eh-oh-eh-oh-da-dar-dar
							Dim sDropDown As String
							Dim sOnChange As String = oColumn.OnChange
							sDropDown = oDropDowns(oColumn.Caption).ToString
							If sValue <> "" Then
								sDropDown = sDropDown.Replace("value=""" & sValue & """>",
								 "value=""" & sValue & """ selected=""selected"">")
							ElseIf oColumn.DefaultValue > 0 Then
								sDropDown = sDropDown.Replace("value=""" & oColumn.DefaultValue & """>",
								 "value=""" & oColumn.DefaultValue & """ selected=""selected"">")
							End If

							oSB.AppendFormat(sDropDown, sColumnClass & " " & sClass, iRow, iColumn, Me.HashValue,
							 IIf(sOnChange <> "", String.Format(" onchange=""{0}""", sOnChange), ""))

						ElseIf oColumn.ControlType = GridControlType.Checkbox Then
							Dim sOnChange As String = oColumn.OnChange
							'work out if we need to check the checkbox
							If sValue <> "" Then
								sValue = IIf(sValue = "on" OrElse sValue = "1" OrElse sValue = "True",
								  "checked=""checked""", "").ToString
							End If

							oSB.AppendFormat(sCheckbox, iRow, iColumn, sValue, sColumnClass, Me.HashValue, IIf(oColumn.OnChange <> "", "onChange=""" + oColumn.OnChange + """", ""))

						ElseIf oColumn.ControlType = GridControlType.Label OrElse bReadOnly Then
							Dim sLabel As String = "<td class=""{5} readonly{9}""><input type=""text"" autocomplete=""off"" name=""c{0}_{1}_{8}"" value=""{2}"" " &
							   "class=""{4} textbox"" {3} {6} {7} id=""c{0}_{1}_{8}"" readonly  /></td>\n"
							Dim sUpDown As String = "onkeydown=""return GridMoveKey();"""

							'add values to the readOnly textfield (label)
							oSB.AppendFormat(sLabel, iRow, iColumn, sValue,
							 sOnEnter, "readonly " & sClass,
							 sColumnClass & IIf(bSuppressTableCSSClass, "", " " & oColumn.CSSClass).ToString,
							 sMaxLength, IIf(bGridNavigation, sUpDown, "").ToString, Me.HashValue, sExtraClass)

						ElseIf oColumn.ControlType = GridControlType.Empty Then
							Dim sEmpty As String = "<td class=""{0}"">&nbsp;</td>\n"
							oSB.AppendFormat(sEmpty, sColumnClass)
						ElseIf oColumn.ControlType = GridControlType.AutoComplete Then

							Dim oACP As New AutoComplete

							oACP.Table = oColumn.AutocompleteTable
							oACP.Expression = oColumn.AutocompleteExpression
							oACP.ID = String.Format("c{0}_{1}_{2}", iRow, iColumn, Me.HashValue)
							oACP.Value = sValue
							oSB.AppendFormat("<td class=""{1}"">{0}</td>\n", Intuitive.Functions.RenderControlToString(oACP), sColumnClass)

						End If

					Next

					'edit + delete list commands
					If Me.AddEdit OrElse Me.AddDelete OrElse oListCommands.Count > 0 Then

						'start the operation cell
						oSB.Append("<td class=""dlop"">\n")

						'check we still on the data
						If iRow <= iDataRowCount - 1 Then

							'Get the current rowID
							If Me.IDColumn = "" Then
								Throw New Exception("You must set the ID column property")
							End If

							If Not dtSource Is Nothing Then

								'Get the ID from the datatable
								If CheckInt(dtSource.Rows(iRow).Item(Me.IDColumn)) Then
									iID = CInt(dtSource.Rows(iRow).Item(Me.IDColumn))
								Else
									Throw New Exception("The last column of the datatable is not an Integer")
								End If

							Else

								'Get ID from the PostValues
								iID = SafeInt(Me.Page.Request(String.Format("gid_{0}_{1}", iRow, Me.HashValue)))
							End If

							'if we haven't already removed this row then write out the commands
							If iID <> 0 Then

								'now write the list command cells
								'for each list command add new cell/hyperlink combo
								For Each oListCommand In oListCommands
									sHyperlink = oListCommand.GetHyperlink(sHashedID, iID)
									oSB.Append(sHyperlink).Append("\n")
								Next
							End If
						End If

						'close the operation cell
						oSB.Append("</td>\n")
					End If

					oSB.Append("</tr>\n")
				End If

				'set the last group column
				If sGroupColumn <> "" Then sLastGroupColumn = sCurrentGroupColumn

			Next

			'close table and scrolling div and the main div
			oSB.Append("</table>\n")

			'if we have an id column specified then store the ids in hidden fields
			If Me.IDColumn <> "" Then
				Dim sID As String = "<input type=""hidden"" id=""gid_{0}_{2}"" name=""gid_{0}_{2}"" value=""{1}"">"
				Dim drRow As DataRow
				Dim iRowPoint As Integer = 0

				If Not dtSource Is Nothing Then
					For Each drRow In dtSource.Rows
						oSB.AppendFormat(sID, iRowPoint, drRow(Me.IDColumn), Me.HashValue)
						iRowPoint += 1
					Next
				Else
					Dim iRowID As Integer
					For Each iRowID In aPostIDs

						oSB.AppendFormat(sID, aPostIDs.IndexOf(iRowID), iRowID, Me.HashValue)
					Next

				End If

			End If

			oSB.Append("</div>\n")
			oSB.Append("</div>\n")

			writer.Write(oSB.ToString.Replace("\n", Environment.NewLine))

			'add the javascript 2 set the lastDate
			If dlastDate.ToShortDateString <> DateFunctions.EmptyDate.ToShortDateString Then
				Page.ClientScript.RegisterStartupScript(GetType(Page), "SetLastDate",
				 String.Format("<script language=""javascript"">SetLastGridDate({0},{1},{2});</script>",
				 dlastDate.Year, dlastDate.Month - 1, dlastDate.Day))
			End If
		End Sub

#End Region

#Region "GetGridData"

		Public Function GetGridData(Optional ByVal NoFormat As Boolean = False) As ArrayList

			Dim aGridRows As New ArrayList

			'if there are no values then return an empty arraylist
			If Me.PostData Is Nothing Then
				Return aGridRows
			End If

			Dim iColumns As Integer = Me.Columns.Count
			Dim iRow As Integer
			Dim iColumn As Integer
			Dim sColumnName As String = ""
			Dim oColumnValue As Object
			Dim sColumnValue As String = ""
			Dim oGridRow As GridRow

			For iRow = 0 To Me.PostData.GetUpperBound(0)

				oGridRow = New GridRow

				'if we have an IDColumn specified, pick this up from the
				'page.request
				If Me.IDColumn <> "" Then
					oGridRow.RowID = SafeInt(Me.Page.Request("gid_" & iRow & "_" & Me.HashValue))
				Else
					oGridRow.RowID = iRow
				End If

				For iColumn = 0 To iColumns - 1

					sColumnName = IIf(Me.Columns(iColumn).UniqueName = "", Me.Columns(iColumn).Caption, Me.Columns(iColumn).UniqueName)
					oColumnValue = SafeString(Me.PostData(iRow, iColumn))

					'if this isn't a duplicate column then retrieve it
					If sColumnName <> sDuplicateColumn Then

						If Not NoFormat Then
							Select Case Me.Columns(iColumn).DataType
								Case GridDataType.GridDate
									sColumnValue = SQL.GetSqlValue(DateFunctions.SafeDate(oColumnValue.ToString), "Date")
								Case GridDataType.Int, GridDataType.IntMinus
									sColumnValue = SQL.GetSqlValue(oColumnValue, "Integer")
								Case GridDataType.Text, GridDataType.Time
									sColumnValue = SQL.GetSqlValue(oColumnValue, "String")
								Case GridDataType.Money, GridDataType.SmallMoney
									sColumnValue = SQL.GetSqlValue(oColumnValue, "Numeric")
								Case GridDataType.Bool
									sColumnValue = SQL.GetSqlValue(SafeBoolean(oColumnValue), "Boolean")
							End Select
						Else
							sColumnValue = oColumnValue.ToString
						End If

						oGridRow.Add(sColumnName, sColumnValue)
					End If
				Next

				aGridRows.Add(oGridRow)
			Next

			Return aGridRows

		End Function

		'grid row
		Public Class GridRow
			Private oHashTable As New Hashtable
			Public RowID As Integer

			Public Sub Add(ByVal ColumnName As String, ByVal Value As String)
				oHashTable.Add(ColumnName, Value)
			End Sub

			Public Sub ChangeValue(ByVal ColumnName As String, ByVal Value As Object)
				oHashTable(ColumnName) = Value
			End Sub

			Public Function SafeValue(ByVal ColumnName As String) As String

				Return SafeString(oHashTable(ColumnName))
			End Function

			Public Function NonSQLStringValue(ByVal ColumnName As String) As String

				Dim sValue As String = SafeString(oHashTable(ColumnName))
				If sValue = "''" Then
					Return ""
				Else
					Return ChopLeft(Chop(sValue.Replace("''", "'")))
				End If

			End Function

			Default Public ReadOnly Property Item(ByVal ColumnName As String) As String
				Get
					Return oHashTable(ColumnName).ToString
				End Get
			End Property
		End Class

#End Region

#Region "getdatatable"

		Public Function GetDataTable(Optional ByVal forceRefresh As Boolean = False) As DataTable
			Dim oDataTable As DataTable
			Dim oDataView As DataView

			'retrieve Dataview from session
			oDataView = CType(Context.Session("grid" & Me.ID & Page.ToString), DataView)
			'check if its nothing
			If forceRefresh Or (oDataView Is Nothing OrElse oDataView.Count = 0 OrElse oDataView.Table.Rows.Count = 0) Then
				'if nothing -retrieve datatable from db

				If sSourceSql <> "" Then

					'get the datatable and store it in the session
					oDataTable = SQL.GetDataTable(sSourceSql)
					'assign dataview from datatable

					oDataView = oDataTable.DefaultView
					'assign dataview to session
					Context.Session("grid" & Me.ID & Page.ToString) = oDataView
					'exit if

				End If

			End If
			'check for sort - apply if needed
			If iSortColumn <> -1 Then
				oDataView.Sort = oDataView.Table.Columns(iSortColumn).ColumnName & IIf(bSortAscending, "", " desc")
			End If
			'assign to datatable
			If oDataView IsNot Nothing Then
				oDataTable = oDataView.ToTable
			Else
				oDataTable = Nothing
			End If

			Return oDataTable
		End Function

		Public Function LookupColumn(ByVal ColumnName As String, ByVal RowIndex As Integer) As String

			Dim oDatatable As DataTable = Me.GetDataTable
			Return oDatatable.Rows(RowIndex)(ColumnName).ToString

		End Function

#End Region

#Region "clear"
		Public Sub Clear()
			Context.Session("grid" & Me.ID & Page.ToString) = Nothing
			bCleared = True
		End Sub

		Public Sub RemoveRow(ByVal iTableID As Integer)

			RemoveGridRowID = iTableID
		End Sub

		Private Sub ClearPostValues()
			oPostValues = Nothing
		End Sub
#End Region

#Region "retrieve postback information"

		Protected Overrides Sub OnLoad(ByVal e As System.EventArgs)

			'if we're a dynamic control don't do this
			If Not Me.DynamicControl Then

				'check for list commands
				Dim sHashedID As String = GetHashedID()

				'if this page is a postback and the command
				'is set to tabclick, set the selected page
				'picked up from the standard command/argument structure
				If Page.IsPostBack AndAlso Not Me.Command Is Nothing Then

					If Me.Command = sHashedID & "E" Then
						RaiseEvent Edit(CType(Me.Argument, Integer))
					ElseIf Me.Command = sHashedID & "D" Then
						RaiseEvent Delete(CType(Me.Argument, Integer))

					ElseIf Me.Command = sHashedID & "HeaderClick" Then

						Dim iNewSortColumn As Integer = SafeInt(Me.Argument)
						If iNewSortColumn <> iSortColumn Then
							bSortAscending = True
						Else
							bSortAscending = Not bSortAscending
						End If

						iSortColumn = iNewSortColumn

					End If


					'do we need to clear the Datalist HTML Output
					'for re-generation?
					If Me.Command = sHashedID & "E" Or
					 Me.Command = sHashedID & "D" Then

						'remove saved session DL Table html
						Context.Session.Remove("DataList" & Me.ID & Page.ToString)
					End If
				End If

				If Page.IsPostBack Then
					oPostValues = GetRawData()
				End If

			End If
			MyBase.OnLoad(e)
		End Sub

		Public Sub PopulatePostData()
			oPostValues = GetRawData()
		End Sub

		Private Function GetRawData() As Object(,)

			'if we have groups need to do this to get columns into post data
			oColumns = Me.Columns

			Dim aLines(100000, oColumns.Count - 1) As Object
			Dim aLine(oColumns.Count - 1) As String
			Dim iLine As Integer = 0
			Dim bDone As Boolean = False
			Dim sCellValue As String
			Dim iCol As Integer
			Dim bRowHasValue As Boolean

			For iRow As Integer = 0 To Me.RowCount + Me.ExtraRows - 1

				'scan in values and set row has value flag
				bRowHasValue = False
				For iCol = 0 To oColumns.Count - 1

					Dim sCellName As String = String.Format("c{0}_{1}_{2}", iRow, iCol, Me.HashValue)

					'check for autocompletes
					If Page.Request(sCellName & "Hidden") IsNot Nothing Then
						sCellValue = Page.Request(sCellName & "Hidden")

						'if auotcomplete is 0 then no value set
						If sCellValue = "0" Then
							sCellValue = ""
						End If

					Else
						sCellValue = Page.Request(sCellName)
					End If

					If sCellValue <> "" Then
						bRowHasValue = True
					End If
					aLine(iCol) = sCellValue
				Next

				'if we have any values, add them
				If bRowHasValue Then
					For iCol = 0 To oColumns.Count - 1
						aLines(iLine, iCol) = aLine(iCol)
					Next
					iLine += 1
				End If

			Next

			If iLine > 0 Then
				Dim aReturn(iLine - 1, oColumns.Count - 1) As Object

				For iRow As Integer = 0 To iLine - 1

					'check we have an ID column specified if necessary
					If RemoveGridRowID > 0 AndAlso Me.IDColumn = "" Then
						Throw New Exception("The ID Column Property must be set")
					End If

					'Get the IDs into the arraylist if we have any
					Dim sControl As String = String.Format("gid_{0}_{1}", iRow, Me.HashValue)
					Dim sValue As String = Me.Page.Request(sControl)
					If Me.IDColumn <> "" AndAlso Not sValue Is Nothing _
					  AndAlso SafeInt(sValue) <> RemoveGridRowID Then
						aPostIDs.Add(SafeInt(sValue))
					End If

					'get the groups into the arraylist if we have any
					Dim sGroup As String = Me.Page.Request(String.Format("ggrp_{0}_{1}", iRow, Me.HashValue))
					If sGroupColumn <> "" Then
						aPostGroups.Add(sGroup)
					End If

					'Check to see if this row needs omitting
					If RemoveGridRowID = 0 OrElse SafeInt(sValue) <> RemoveGridRowID Then
						For iCol = 0 To oColumns.Count - 1
							aReturn(iRow, iCol) = aLines(iRow, iCol)
						Next
					End If

				Next

				Return aReturn
			Else
				Return Nothing
			End If

		End Function
#End Region

#Region "validation"

		'validate min/max
		Public Function ValidateMinMax(Optional ByVal bContiguousQuantities As Boolean = True,
		   Optional ByVal bCheckForOverlap As Boolean = False,
		   Optional ByVal bAllowSingleBands As Boolean = False) _
		  As FunctionReturn

			Dim oReturn As New FunctionReturn

			Dim aPostValues(,) As Object = Me.PostData()
			Dim iRow As Integer
			Dim iMinQuantity As Integer
			Dim iMaxQuantity As Integer
			Dim iLastMaximumQuantity As Integer = -999

			If Not aPostValues Is Nothing Then
				For iRow = 0 To aPostValues.GetUpperBound(0)

					iMinQuantity = SafeInt(aPostValues(iRow, 0))
					iMaxQuantity = SafeInt(aPostValues(iRow, 1))

					'min quantity
					If iMinQuantity <= 0 Then
						oReturn.AddWarning("The Min Quantity must be a valid number greater than 0 on Row {0}",
						 iRow + 1)
					End If

					'max quantity > min quantity
					If Not bAllowSingleBands AndAlso iMaxQuantity <= iMinQuantity Then
						oReturn.AddWarning("The Max Quantity must be greater than the Min Quantity on Row {0}",
						 iRow + 1)
					ElseIf bAllowSingleBands AndAlso iMaxQuantity < iMinQuantity Then
						oReturn.AddWarning("The Max Quantity cannot be smaller than the Min Quantity on Row {0}",
						 iRow + 1)
					End If

					'check min quantity=last max quantity +1
					If bContiguousQuantities AndAlso
					 iLastMaximumQuantity > -999 AndAlso iMinQuantity <> iLastMaximumQuantity + 1 Then
						oReturn.AddWarning("The Minimum Quantity on Row {0} must follow " _
						 & "the Maximum Quantity on Row {1}", iRow + 1, iRow)
					End If

					iLastMaximumQuantity = iMaxQuantity
				Next
			End If

			'if everything else is good then check for overlapping
			'dates overlap - only if other checks have passed
			'otherwise we have all sorts of trouble
			If bCheckForOverlap And oReturn.Warnings.Count = 0 And Not aPostValues Is Nothing Then
				Dim iCheckRow As Integer
				Dim iStartMin As Integer
				Dim iEndMax As Integer
				Dim iCheckStartMin As Integer
				Dim iCheckEndMin As Integer

				For iRow = 0 To aPostValues.GetUpperBound(0) - 1
					iStartMin = SafeInt(aPostValues(iRow, 0))
					iEndMax = SafeInt(aPostValues(iRow, 1))

					For iCheckRow = iRow + 1 To aPostValues.GetUpperBound(0)
						iCheckStartMin = SafeInt(aPostValues(iCheckRow, 0))
						iCheckEndMin = SafeInt(aPostValues(iCheckRow, 1))

						If NumberRangeOverlaps(iCheckStartMin, iCheckEndMin, iStartMin, iEndMax) Then
							oReturn.AddWarning(String.Format("The Numbers on Row {0} overlap with those on Row {1}",
							 iCheckRow + 1, iRow + 1))
						End If
					Next
				Next
			End If

			Return oReturn

		End Function

		'validate datebands
		Public Function ValidateDatebands(Optional ByVal bContiguousDates As Boolean = True,
		 Optional ByVal dLowerDateBound As Date = #1/1/1900#,
		 Optional ByVal dUpperDateBound As Date = #1/1/1900#,
		 Optional ByVal bDateRangeCovered As Boolean = False,
		 Optional ByVal bCheckForOverlap As Boolean = False,
		 Optional ByVal bAllowSingleDatebands As Boolean = False) As ArrayList

			Dim aWarnings As New ArrayList

			'validate dates
			Dim aPostValues(,) As Object = Me.PostData()
			Dim iRow As Integer
			Dim dLastEndDate As Date = Nothing
			Dim dEarliestStartDate As Date = New Date(1900, 1, 1)
			Dim dLatestEndDate As Date = New Date(1900, 1, 1)

			If Not aPostValues Is Nothing Then
				For iRow = 0 To aPostValues.GetUpperBound(0)

					Dim oStartDate As Object = aPostValues(iRow, 0)
					Dim oEndDate As Object = aPostValues(iRow, 1)
					Dim bValidDates As Boolean = True

					'check valid dates
					If Not DateFunctions.IsDisplayDate(oStartDate.ToString) Then
						aWarnings.Add(String.Format("The Start Date on Row {0} is not a valid date", iRow + 1))
						bValidDates = False
					End If
					If Not DateFunctions.IsDisplayDate(oEndDate.ToString) Then
						aWarnings.Add(String.Format("The End Date on Row {0} is not a valid date", iRow + 1))
						bValidDates = False
					End If

					'if dates valid andalso allowsingle datebands is false then check enddate>startdate
					If bValidDates AndAlso Not bAllowSingleDatebands Then
						If DateFunctions.DisplayDateToDate(oStartDate.ToString) >
						 DateFunctions.DisplayDateToDate(oEndDate.ToString) Then
							aWarnings.Add("The Start Date must be before the End Date on Row " & (iRow + 1))
						End If
					End If

					'if dates valid and lower and upper date boundaries have been specified then
					'check 'em
					If bValidDates AndAlso dLowerDateBound.Year > 1990 Then
						If DateFunctions.DisplayDateToDate(oStartDate.ToString) < dLowerDateBound Then
							aWarnings.Add("The Start Date cannot be before " & DateFunctions.DisplayDate(dLowerDateBound) &
							 " on Row " & (iRow + 1))
						End If
					End If

					If bValidDates AndAlso dUpperDateBound.Year > 1990 Then
						If DateFunctions.DisplayDateToDate(oEndDate.ToString) > dUpperDateBound Then
							aWarnings.Add("The End Date cannot be later than " & DateFunctions.DisplayDate(dUpperDateBound) &
							 " on Row " & (iRow + 1))
						End If
					End If

					'if contiguous dates then make sure that the startdate=last enddate+1
					If bContiguousDates AndAlso bValidDates AndAlso Not dLastEndDate = Nothing Then

						Dim oSpan As TimeSpan
						oSpan = DateFunctions.DisplayDateToDate(oStartDate.ToString).Subtract(dLastEndDate)
						If oSpan.Days <> 1 Then
							aWarnings.Add(String.Format(
							 "The Start Date on Row {0} must be the date after the End Date on Row {1}",
							 iRow + 1, iRow))
						End If
					End If

					'store the earliest start date if irow=0 and bdaterangecovered=true
					If bValidDates AndAlso iRow = 0 AndAlso bDateRangeCovered Then
						dEarliestStartDate = DateFunctions.DisplayDateToDate(oStartDate.ToString)
					End If

					'store the latest end date if iRow=aPostValues.GetUpperBound(0)
					If bValidDates AndAlso iRow = aPostValues.GetUpperBound(0) AndAlso bDateRangeCovered Then
						dLatestEndDate = DateFunctions.DisplayDateToDate(oEndDate.ToString)
					End If

					'store the last end date if the dates are ok
					If bValidDates Then
						dLastEndDate = DateFunctions.DisplayDateToDate(oEndDate.ToString)
					End If
				Next
			End If

			'do date range covered
			If bDateRangeCovered And Not aPostValues Is Nothing Then
				If dEarliestStartDate <> dLowerDateBound OrElse dLatestEndDate <> dUpperDateBound Then
					aWarnings.Add(String.Format("The dates must cover the period from {0} to {1}",
					 DateFunctions.DisplayDate(dLowerDateBound), DateFunctions.DisplayDate(dUpperDateBound)))
				End If
			End If

			'dates overlap - only if other checks have passed
			'otherwise we have all sorts of trouble
			If bCheckForOverlap And aWarnings.Count = 0 And Not aPostValues Is Nothing Then
				Dim iCheckRow As Integer
				Dim dStartDate As Date
				Dim dEndDate As Date
				Dim dCheckStartDate As Date
				Dim dCheckEndDate As Date

				For iRow = 0 To aPostValues.GetUpperBound(0) - 1
					dStartDate = DateFunctions.SafeDate(aPostValues(iRow, 0))
					dEndDate = DateFunctions.SafeDate(aPostValues(iRow, 1))

					For iCheckRow = iRow + 1 To aPostValues.GetUpperBound(0)
						dCheckStartDate = DateFunctions.SafeDate(aPostValues(iCheckRow, 0))
						dCheckEndDate = DateFunctions.SafeDate(aPostValues(iCheckRow, 1))

						If DatesOverlap(dStartDate, dEndDate, dCheckStartDate, dCheckEndDate) Then
							aWarnings.Add(String.Format("The Dates on Row {0} overlap with those on Row {1}",
							 iCheckRow + 1, iRow + 1))
						End If
					Next
				Next
			End If
			Return aWarnings
		End Function

		'datesoverlap
		Public Shared Function DatesOverlap(ByVal dStartDate1 As Date,
		 ByVal dEndDate1 As Date, ByVal dStartDate2 As Date, ByVal dEndDate2 As Date) As Boolean

			Return (dStartDate2 >= dStartDate1 AndAlso dStartDate2 <= dEndDate1) _
			 OrElse (dEndDate2 >= dStartDate1 AndAlso dEndDate2 <= dEndDate1) _
			 OrElse (dStartDate2 <= dStartDate1 AndAlso dEndDate2 >= dEndDate1)
		End Function

		Public Function ValidateDecimalBands() As ArrayList
			Dim aWarnings As New ArrayList

			Dim aPostValues(,) As Object = Me.PostData()
			Dim iRow As Integer

			If Not aPostValues Is Nothing Then
				For iRow = 0 To aPostValues.GetUpperBound(0)

					Dim oFromCost As Object = aPostValues(iRow, 0)
					Dim oToCost As Object = aPostValues(iRow, 1)
					Dim oMargin As Object = aPostValues(iRow, 2)
					Dim bValidCosts As Boolean = True

					'check valid costs
					If oFromCost Is "" Then
						aWarnings.Add(String.Format("The From Cost on Row {0} is not a valid number", iRow + 1))
						bValidCosts = False
					End If
					If oToCost Is "" Then
						aWarnings.Add(String.Format("The To Cost on Row {0} is not a valid number", iRow + 1))
						bValidCosts = False
					End If
					If oMargin Is "" Then
						aWarnings.Add(String.Format("The Margin on Row {0} is not a valid number", iRow + 1))
						bValidCosts = False
					End If

					'if costs valid is false then check tocost>fromcost
					If bValidCosts Then
						If SafeDecimal(oFromCost) > SafeDecimal(oToCost) Then
							aWarnings.Add("The From Cost must be less than the To Cost on Row " & (iRow + 1))
						End If
					End If
				Next
			End If

			'dates overlap - only if other checks have passed
			'otherwise we have all sorts of trouble
			If aWarnings.Count = 0 And Not aPostValues Is Nothing Then
				Dim iCheckRow As Integer
				Dim nFromCost As Decimal
				Dim nToCost As Decimal
				Dim dCheckFromCost As Decimal
				Dim dCheckToCost As Decimal

				For iRow = 0 To aPostValues.GetUpperBound(0) - 1
					nFromCost = SafeDecimal(aPostValues(iRow, 0))
					nToCost = SafeDecimal(aPostValues(iRow, 1))

					For iCheckRow = iRow + 1 To aPostValues.GetUpperBound(0)
						dCheckFromCost = SafeDecimal(aPostValues(iCheckRow, 0))
						dCheckToCost = SafeDecimal(aPostValues(iCheckRow, 1))

						If CostsOverlap(nFromCost, nToCost, dCheckFromCost, dCheckToCost) Then
							aWarnings.Add(String.Format("The cost band on Row {0} overlaps with the cost band on Row {1}",
							 iCheckRow + 1, iRow + 1))
						End If
					Next
				Next
			End If
			Return aWarnings
		End Function

		Public Shared Function CostsOverlap(ByVal nFromCost1 As Decimal,
		 ByVal nToCost1 As Decimal, ByVal nFromCost2 As Decimal, ByVal nToCost2 As Decimal) As Boolean

			Return (nFromCost2 >= nFromCost1 AndAlso nFromCost2 <= nToCost1) _
			 OrElse (nToCost2 >= nFromCost1 AndAlso nToCost2 <= nToCost1) _
			 OrElse (nFromCost2 <= nFromCost1 AndAlso nToCost2 >= nToCost1)
		End Function

#End Region

#Region "Set Source SQL (helper)"
		Public Sub SetSourceSQL(ByVal SQL As String, ByVal ParamArray aParams() As Object)
			Me.SourceSql = Intuitive.SQL.FormatStatement(SQL, aParams)
		End Sub
#End Region

#Region "Support Functions"
		Public Sub AddListCommand(ByVal Image As String, ByVal Command As String, ByVal ToolTip As String,
		 Optional ByVal Confirm As Boolean = False, Optional CustomScript As String = "")
			oListCommands.Add(New ListCommand(Image, Command, ToolTip, Confirm, "", CustomScript))
		End Sub

		Public Function GetHashedID() As String
			Dim sID As String = Me.ID
			Return "grd" & (sID.GetHashCode Mod 100).ToString
		End Function
#End Region

#Region "events"
		Public Event Edit(ByVal iTableID As Integer)
		Public Event Delete(ByVal iTableID As Integer)
#End Region

	End Class

#Region "Grid Columns"

	'FlexColumn Collection
	<Serializable()> Public Class GridColumns
		Inherits System.Collections.CollectionBase

		Public Sub Add(ByVal Column As GridColumn)
			List.Add(Column)
		End Sub

		Public Sub Remove(ByVal Index As Integer)
			If Index < Me.Count And Index >= 0 Then
				List.RemoveAt(Index)
			End If
		End Sub

		Default Public ReadOnly Property Item(ByVal Index As Integer) As GridColumn
			Get
				Return CType(List(Index), GridColumn)
			End Get
		End Property
	End Class
#End Region

#Region "GridColumn"
	<Serializable()> Public Class GridColumn
		Private sCaption As String = ""
		Private sSuperHeader As String = ""
		Private oControlType As GridControlType
		Private oDataType As GridDataType
		Private iMaxFieldLength As Integer = 0
		Private sCSSClass As String = ""
		Public Values As New ArrayList
		Private iColumnID As Integer = 0
		Private iDefaultValue As Integer = 0
		Private bAllowCopy As Boolean = False
		Private sOnBlur As String = ""
		Private sOnKeyUp As String = ""
		Private sOnChange As String = ""
		Private sExtraColumnClass As String = ""
		Private sUniqueName As String = ""
		Private sPostCopyScript As String = ""
		Public AddBlankOption As Boolean = True
		Public ReadOnlyIfColumn As String = ""
		Public ReadOnlyIfValue As String = ""
		Public DataColumn As String = ""
		Public GroupColumn As String = ""
		Public AutocompleteTable As String = ""
		Public AutocompleteExpression As String
		Public DecimalPlaceRestriction As Integer

#Region "Properties Setup"
		Public Property Caption() As String
			Get
				Return sCaption
			End Get
			Set(ByVal Value As String)
				sCaption = Value
			End Set
		End Property

		Public Property ControlType() As GridControlType
			Get
				Return oControlType
			End Get
			Set(ByVal Value As GridControlType)
				oControlType = Value
			End Set
		End Property

		Public Property DataType() As GridDataType
			Get
				Return oDataType
			End Get
			Set(ByVal Value As GridDataType)
				oDataType = Value
			End Set
		End Property

		Public Property MaxFieldLength() As Integer
			Get
				Return iMaxFieldLength
			End Get
			Set(ByVal Value As Integer)
				iMaxFieldLength = Value
			End Set
		End Property

		Public Property CSSClass() As String
			Get
				Return sCSSClass
			End Get
			Set(ByVal Value As String)
				sCSSClass = Value
			End Set
		End Property

		Public Property ColumnID() As Integer
			Get
				Return iColumnID
			End Get
			Set(ByVal Value As Integer)
				iColumnID = Value
			End Set
		End Property

		Public Property DefaultValue() As Integer
			Get
				Return iDefaultValue
			End Get
			Set(ByVal Value As Integer)
				iDefaultValue = Value
			End Set
		End Property

		Public Property AllowCopy() As Boolean
			Get
				Return bAllowCopy
			End Get
			Set(ByVal Value As Boolean)
				bAllowCopy = Value
			End Set
		End Property

		Public Property OnBlur() As String
			Get
				Return sOnBlur
			End Get
			Set(ByVal Value As String)
				sOnBlur = Value
			End Set
		End Property

		Public Property OnKeyUp() As String
			Get
				Return sOnKeyUp
			End Get
			Set(ByVal Value As String)
				sOnKeyUp = Value
			End Set
		End Property

		Public Property SuperHeader() As String
			Get
				Return sSuperHeader
			End Get
			Set(ByVal Value As String)
				sSuperHeader = Value
			End Set
		End Property

		Public Property OnChange() As String
			Get
				Return sOnChange
			End Get
			Set(ByVal Value As String)
				sOnChange = Value
			End Set
		End Property

		Public Property ExtraColumnClass() As String
			Get
				Return sExtraColumnClass
			End Get
			Set(ByVal value As String)
				sExtraColumnClass = value
			End Set
		End Property

		Public Property UniqueName() As String
			Get
				Return sUniqueName
			End Get
			Set(ByVal value As String)
				sUniqueName = value
			End Set
		End Property

		Public Property PostCopyScript() As String
			Get
				Return sPostCopyScript
			End Get
			Set(ByVal value As String)
				sPostCopyScript = value
			End Set
		End Property

		Public Property OnFocus As String

		Public Property [ReadOnly] As Boolean

#End Region

		Public Overloads Sub AddValue(ByVal Value As String)
			Me.Values.Add(Value)
		End Sub

		Public Overloads Sub AddValue(ByVal Value As String, ByVal Name As String)
			Me.Values.Add(Value & "#" & Name)
		End Sub

		Public Sub New(
			ByVal Caption As String,
			Optional ByVal GridControlType As GridControlType = GridControlType.Textbox,
			Optional ByVal GridDataType As GridDataType = GridDataType.Int,
			Optional ByVal MaxColumnLength As Integer = 0,
			Optional ByVal CSSClass As String = "",
			Optional ByVal ColumnID As Integer = 0,
			Optional ByVal AllowCopy As Boolean = False,
			Optional ByVal OnBlur As String = "",
			Optional ByVal OnKeyUp As String = "",
			Optional ByVal SuperHeader As String = "",
			Optional ByVal OnChange As String = "",
			Optional ByVal ExtraColumnClass As String = "",
			Optional ByVal UniqueName As String = "",
			Optional ByVal OnFocus As String = "",
			Optional ByVal DataColumn As String = "",
			Optional ByVal GroupColumn As String = "",
			Optional ByVal DecimalPlaceRestriction As Integer = 0,
			Optional ByVal [ReadOnly] As Boolean = False)

			sCaption = Caption
			oControlType = GridControlType
			oDataType = GridDataType
			iMaxFieldLength = MaxColumnLength
			sCSSClass = CSSClass
			iColumnID = ColumnID
			bAllowCopy = AllowCopy
			sOnBlur = OnBlur
			sOnKeyUp = OnKeyUp
			sSuperHeader = SuperHeader
			sOnChange = OnChange
			sExtraColumnClass = ExtraColumnClass
			sUniqueName = UniqueName
			Me.OnFocus = OnFocus
			Me.DataColumn = DataColumn
			Me.GroupColumn = GroupColumn
			Me.DecimalPlaceRestriction = DecimalPlaceRestriction
			Me.ReadOnly = [ReadOnly]

		End Sub

	End Class
#End Region

#Region "Column Groups"

	<Serializable()> Public Class ColumnGroups
		Inherits System.Collections.CollectionBase

		Public Sub AddGroup(ByVal oColumnGroup As ColumnGroup)
			List.Add(oColumnGroup)
		End Sub

	End Class

	<Serializable()> Public Class ColumnGroup

		Private oColumns As New GridColumns
		Private sGroupName As String = ""

#Region "Properties"
		Public Property GroupName As String
			Get
				Return sGroupName
			End Get
			Set(value As String)
				sGroupName = value
			End Set
		End Property

		Public ReadOnly Property Columns As GridColumns
			Get
				Return oColumns
			End Get
		End Property
#End Region

		Public Sub AddColumn(
			ByVal Caption As String,
			Optional ByVal GridControlType As GridControlType = GridControlType.Textbox,
			Optional ByVal GridDataType As GridDataType = GridDataType.Int,
			Optional ByVal MaxColumnLength As Integer = 0,
			Optional ByVal CSSClass As String = "",
			Optional ByVal ColumnID As Integer = 0,
			Optional ByVal AllowCopy As Boolean = False,
			Optional ByVal OnBlur As String = "",
			Optional ByVal OnKeyUp As String = "",
			Optional ByVal SuperHeader As String = "",
			Optional ByVal OnChange As String = "",
			Optional ByVal ExtraColumnClass As String = "",
			Optional ByVal UniqueName As String = "",
			Optional ByVal OnFocus As String = "",
			Optional ByVal DataColumn As String = "",
			Optional ByVal GroupColumn As String = "",
			Optional ByVal DecimalPlaceRestriction As Integer = 0,
			Optional ByVal [ReadOnly] As Boolean = False)

			oColumns.Add(New GridColumn(Caption, GridControlType, GridDataType, MaxColumnLength, CSSClass, ColumnID,
				AllowCopy, OnBlur, OnKeyUp, SuperHeader, OnChange, ExtraColumnClass, UniqueName,
				OnFocus, DataColumn, GroupColumn, DecimalPlaceRestriction, [ReadOnly]))
		End Sub

	End Class

#End Region

#Region "Enums"

	'GridColumnTypes
	Public Enum GridControlType
		Label
		Textbox
		DropDown
		Checkbox
		Empty
		AutoComplete
	End Enum

	'Grid Data Types
	Public Enum GridDataType
		Int
		IntMinus
		Money
		MoneyWithNA
		SmallMoney
		Text
		GridDate
		Empty
		Bool
		Time
	End Enum

#End Region

End Namespace