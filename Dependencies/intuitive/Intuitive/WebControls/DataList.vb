Imports System
Imports System.Collections
Imports System.Collections.Generic
Imports System.Text.RegularExpressions
Imports System.Web.UI
Imports WebControls = System.Web.UI.WebControls
Imports System.Text
Imports System.Configuration
Imports Intuitive
Imports Intuitive.Functions
Imports System.Web.HttpUtility
Imports System.Xml

Namespace WebControls

	'*** DataList ***'
	Public Class DataList
		Inherits ControlBase
		Implements INamingContainer

#Region "Variables"

		Private oColumns As New Columns
		Private oListCommands As New ListCommands
		Private oStyleOverrides As New StyleOverrides
		Private iSelectedItemID As Integer = 0
		Private iSortColumn As Integer = -1
		Private bSortAscending As Boolean
		Private bShowDataPoint As Boolean = True
		Private sEditImage As String = "Edit.gif"
		Private sDeleteImage As String = "Delete.gif"
		Private sEditCommandOverride As String = ""
		Private sDeleteCommandOverride As String = ""
		Private bAddEditDelete As Boolean = False
		Private bSuppressEditDelete As Boolean = False
		Private oDataView As DataView
		Private sGroupColumn As String = ""
		Private bCacheSetup As Boolean = False
		Private bSuppressCellClass As Boolean = True
        Private bOverrideIDWithIndex As Boolean = False
        Private oOverrideDataTable As DataTable

        ''' <summary>
        ''' ID column index.
        ''' </summary>
        Private iIDColumnIndex As Integer = -1

		Private bStyleOverrideGroupClass As Boolean = False
		Private bExcel As Boolean = False
		Private bPaged As Boolean = False
		Private iRowsPerPage As Integer = 0
		Private iTotalLinksToDisplay As Integer = 10
		Public UseHashedID As Boolean = True
		Private iExcelColumns As Integer = 0
		Private sImageRoot As String = "/images/"
		Public OneClick As Boolean = True
		Public ClientFilter As Boolean = True
		Private bHeadersEnabled As Boolean = True

#End Region

#Region "Properties"

		Public Property SourceSql() As String
			Get
				If Me.Page Is Nothing Then Return ""
				Return SafeString(Me.ViewState(Me.ID & Me.Page.ToString & "SourceSql"))
			End Get
			Set(ByVal Value As String)
				Me.ViewState(Me.ID & Me.Page.ToString & "SourceSql") = Value
				Me.Clear()
			End Set
		End Property

		Public Property SourceXML() As String
			Get
				If Me.Page Is Nothing Then Return ""
				Return SafeString(Me.ViewState(Me.ID & Me.Page.ToString & "SourceXML"))
			End Get
			Set(ByVal Value As String)
				Me.ViewState(Me.ID & Me.Page.ToString & "SourceXML") = Value
				Me.Clear()
			End Set
		End Property

		Public Property CurrentPage() As Integer
			Get
				If Me.Page Is Nothing Then Return 1

				Dim iCurrentPage As Integer = Functions.SafeInt(Me.ViewState(Me.ID & Me.Page.ToString & "CurrentPage"))
				If iCurrentPage = 0 Then iCurrentPage = 1
				Return iCurrentPage
			End Get
			Set(ByVal Value As Integer)
				Me.ViewState(Me.ID & Me.Page.ToString & "CurrentPage") = Value
			End Set
		End Property

		Public Property Columns() As Columns
			Get
				Return oColumns
			End Get
			Set(ByVal Value As Columns)
				oColumns = Value
			End Set
		End Property

		''' <summary>
		''' The footer columns.
		''' </summary>
		Public FooterColumns As New FooterColumns

		Public Property ListCommands() As ListCommands
			Get
				Return oListCommands
			End Get
			Set(ByVal Value As ListCommands)
				oListCommands = Value
			End Set
		End Property

		Public Property StyleOverrides() As StyleOverrides
			Get
				Return oStyleOverrides
			End Get
			Set(ByVal Value As StyleOverrides)
				oStyleOverrides = Value
			End Set
		End Property

		Public Property SelectedItemID() As Integer
			Get
				Return iSelectedItemID
			End Get
			Set(ByVal Value As Integer)
				iSelectedItemID = Value

				'remove the table
				If Page IsNot Nothing Then
					Context.Session.Remove("DLTable" & Me.ID & Me.Page.ToString)
				End If
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

		Public Property ShowDataPoint() As Boolean
			Get
				Return bShowDataPoint
			End Get
			Set(ByVal Value As Boolean)
				bShowDataPoint = Value
			End Set
		End Property

		Public Property EditImage() As String
			Get
				Return sEditImage
			End Get
			Set(ByVal Value As String)
				sEditImage = Value
			End Set
		End Property

		Public Property DeleteImage() As String
			Get
				Return sDeleteImage
			End Get
			Set(ByVal Value As String)
				sDeleteImage = Value
			End Set
		End Property

		Public Property EditCommandOverride() As String
			Get
				Return sEditCommandOverride
			End Get
			Set(ByVal Value As String)
				sEditCommandOverride = Value
			End Set
		End Property

		Public Property DeleteCommandOverride() As String
			Get
				Return sDeleteCommandOverride
			End Get
			Set(ByVal Value As String)
				sDeleteCommandOverride = Value
			End Set
		End Property

		Public Property AddEditDelete() As Boolean
			Get
				Return bAddEditDelete
			End Get
			Set(ByVal Value As Boolean)
				bAddEditDelete = Value
			End Set
		End Property

		Public Property GroupColumn() As String
			Get
				Return sGroupColumn
			End Get
			Set(ByVal Value As String)
				sGroupColumn = Value
			End Set
		End Property

		Public Property SuppressEditDelete() As Boolean
			Get
				Return bSuppressEditDelete
			End Get
			Set(ByVal Value As Boolean)
				bSuppressEditDelete = Value
			End Set
		End Property

		Public ReadOnly Property HasData() As Boolean
			Get
				Dim oView As DataView = Me.GetDataView
				Return Not oView Is Nothing AndAlso oView.Count > 0
			End Get
		End Property

		Public ReadOnly Property ListCount() As Integer
			Get
				Dim oView As DataView = Me.GetDataView
				If Not oView Is Nothing Then
					Return oView.Count
				Else
					Return 0
				End If
			End Get
		End Property

		Public Property CacheSetup() As Boolean
			Get
				Return bCacheSetup
			End Get
			Set(ByVal Value As Boolean)
				bCacheSetup = Value
			End Set
		End Property

		Public Property SuppressCellClass() As Boolean
			Get
				Return bSuppressCellClass
			End Get
			Set(ByVal Value As Boolean)
				bSuppressCellClass = Value
			End Set
		End Property

		Public Property OverrideIDWithIndex() As Boolean
			Get
				Return bOverrideIDWithIndex
			End Get
			Set(ByVal Value As Boolean)
				bOverrideIDWithIndex = Value
			End Set
		End Property

		''' <summary>
		''' ID column index.
		''' </summary>
		''' <returns>ID column index.</returns>
		Public Property IDColumnIndex As Integer
			Get
				If iIDColumnIndex < 0 Then Return oDataView.Table.Columns.Count - 1

				Return iIDColumnIndex
			End Get
			Set(value As Integer)
				iIDColumnIndex = value
			End Set
		End Property

		Public Property GroupStyleOverride() As Boolean
			Get
				Return bStyleOverrideGroupClass
			End Get
			Set(ByVal Value As Boolean)
				bStyleOverrideGroupClass = Value
			End Set
		End Property

		Public Property Excel() As Boolean
			Get
				Return bExcel
			End Get
			Set(ByVal Value As Boolean)
				bExcel = Value
			End Set
		End Property

		Public Property ExcelColumns() As Integer
			Get
				Return iExcelColumns
			End Get
			Set(ByVal Value As Integer)
				iExcelColumns = Value
			End Set
		End Property

		Public ReadOnly Property HasDataInSession() As Boolean
			Get
				Return Not Context.Session("DataList" & Me.ID & Me.Page.ToString) Is Nothing
			End Get
		End Property

        Public Property HeadersEnabled() As Boolean
            Get
                Return bHeadersEnabled
            End Get
            Set(ByVal value As Boolean)
                bHeadersEnabled = value
            End Set
        End Property

        Public Property OverrideDataTable() As DataTable
            Get
                Return oOverrideDataTable
            End Get
            Set
                oOverrideDataTable = Value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets a value indicating whether the footer is enabled.
        ''' </summary>
        ''' <value>
        '''   <c>true</c> if the footer is enabled; otherwise, <c>false</c>.
        ''' </value>
        Public Property FooterEnabled As Boolean = False

		Public Property DisableLinks As Boolean = False

#Region "Paging Properties"

		Public Property Paged() As Boolean
			Get
				Return bPaged
			End Get
			Set(ByVal value As Boolean)
				bPaged = value
			End Set
		End Property

		Public Property RowsPerPage() As Integer
			Get
				Return iRowsPerPage
			End Get
			Set(ByVal value As Integer)
				iRowsPerPage = value
			End Set
		End Property

		Public Property TotalLinksToDisplay() As Integer
			Get
				Return iTotalLinksToDisplay
			End Get
			Set(ByVal value As Integer)
				iTotalLinksToDisplay = value
			End Set
		End Property

#End Region

#End Region

#Region "Viewstate Management"

		Protected Overrides Function SaveViewState() As Object

			Dim oState(5) As Object

			oState(0) = MyBase.SaveViewState
			oState(1) = iSortColumn
			oState(2) = bSortAscending
			oState(3) = iSelectedItemID

			'build up array of setup stuff
			Dim oSetup(12) As Object
			oSetup(0) = oColumns.GetViewState()
			oSetup(1) = oStyleOverrides.GetViewState()
			oSetup(2) = oListCommands.GetViewState()
			oSetup(3) = sEditCommandOverride
			oSetup(4) = bAddEditDelete
			oSetup(5) = sGroupColumn
			oSetup(6) = bSuppressEditDelete
			oSetup(7) = bShowDataPoint
			oSetup(8) = bPaged
			oSetup(9) = iRowsPerPage
			oSetup(10) = iTotalLinksToDisplay
			oSetup(11) = FooterColumns.GetViewState()

			'if not caching then add setup to the standard state
			If Not Me.CacheSetup Then
				ReDim Preserve oState(15)
				For i As Integer = 0 To 11
					oState(i + 4) = oSetup(i)
				Next
			End If

			'if not cached then store in viewstate as normal
			'otherwise we'll pop it in the cache
			If Me.CacheSetup Then
				If CType(Me.Page.Cache(Me.UniqueControlName), Object()) Is Nothing Then
					Me.Page.Cache(Me.UniqueControlName) = oSetup
				End If
			End If

			Return oState

		End Function

		Protected Overrides Sub LoadViewState(ByVal savedState As Object)

			Dim oState As Object() = CType(savedState, Object())

			If Me.CacheSetup AndAlso Not Me.Page.Cache(Me.UniqueControlName) Is Nothing Then
				ReDim Preserve oState(14)

				Dim oSetup As Object() = CType(Me.Page.Cache(Me.UniqueControlName), Object())
				For i As Integer = 0 To oSetup.Length - 1
					oState(i + 5) = oSetup(i)
				Next
			End If

			MyBase.LoadViewState(oState(0))
			iSortColumn = SafeInt(oState(1))
			bSortAscending = SafeBoolean(oState(2))
			iSelectedItemID = CType(oState(3), Integer)
			oColumns.RestoreFromViewState(CType(oState(4), String))
			oStyleOverrides.RestoreFromViewState(CType(oState(5), String))
			oListCommands.RestoreFromViewState(CType(oState(6), String))
			sEditCommandOverride = oState(7).ToString
			bAddEditDelete = CType(oState(8), Boolean)
			sGroupColumn = oState(9).ToString
			bSuppressEditDelete = CType(oState(10), Boolean)
			bShowDataPoint = CType(oState(11), Boolean)
			bPaged = CType(oState(12), Boolean)
			iRowsPerPage = SafeInt(oState(13))
			iTotalLinksToDisplay = SafeInt(oState(14))
			FooterColumns.SetViewState(CType(oState(15), String))

		End Sub

#End Region

#Region "Events"

		Public Event Edit(ByVal iTableID As Integer)
		Public Event Delete(ByVal iTableID As Integer)
		Public Event PageChange(ByVal iPage As Integer)
		Public Event CustomEvent(ByVal sType As String, ByVal iID As Integer)

#End Region

#Region "New"

		Public Sub New()
			MyBase.New()
			Me.Clear()
		End Sub

#End Region

#Region "Clear"

		Public Sub Clear(Optional ByVal SelectedItemID As Integer = -1)

			'remove saved dataview and remove saved table html
			If Me.Page IsNot Nothing Then
				Context.Session.Remove("DataList" & Me.ID & Me.Page.ToString)
				Context.Session.Remove("DLTable" & Me.ID & Me.Page.ToString)
				Me.CurrentPage = 1
			End If

			If SelectedItemID <> -1 Then
				iSelectedItemID = SelectedItemID
			End If

		End Sub

#End Region

#Region "Render"

		Protected Overrides Sub Render(ByVal writer As System.Web.UI.HtmlTextWriter)

			Dim oSB As New StringBuilder
			Dim sHashedID As String = GetHashedID()

			'sanity checks
			If Me.Paged And Me.RowsPerPage = 0 Then Throw New Exception("paged datalists must define the number of rows per page")

			'get tut data
			Dim oDataView As DataView = GetDataView()

			'create the holding, header table first row and cell
			oSB.Append(String.Format("<div id=""{0}"" class=""datalist{1}"">\n", Me.ID, IIf(Me.CSSClass = "", "", " " & Me.CSSClass).ToString))

			'if we are allowing a client filter, bish in the textbox and icon
			If Me.ClientFilter AndAlso Not bExcel AndAlso Not oDataView Is Nothing AndAlso oDataView.Count > 10 Then
				oSB.Append(String.Format("<div class=""clientfilter"">\n" &
				 "<input name="""" id=""txt{0}clientfilter"" onkeyup=""javascript:Datalist.ClientFilter(event, '{0}')"" style=""display:none"" " &
				 "value="""">\n" &
				 "<img src=""{1}datalistfilter.gif"" alt=""Search list"" title=""Search list"" onclick=""Datalist.ToggleSearch('{0}')""/>\n" &
				 "</div>\n", Me.ID, sImageRoot))
			End If

			oSB.Append("<table class=""header"">\n")
			oSB.AppendFormat("<tr class=""cols{0}"">\n",
				Columns.RenderColumnsCount)

			If Me.ShowDataPoint Then
				oSB.Append("<td class=""point"">&nbsp;</td>\n")
			End If

			'create the header columns
			Dim sDataColumn As String
			Dim sHeaderBase As String

			'for the time being until we come up with something tricksy tricksy, disable ordering when paged
			If Me.Paged OrElse Not Me.HeadersEnabled Then
				sHeaderBase = "<td class=""c{0}{4}"">{2}</td>\n"
			Else

				sHeaderBase = "<td class=""c{0}{4}""><a href=""javaScript:Postback('" &
				  "{1}HeaderClick',{3});"">{2}</a></td>\n"
			End If

			Dim iColumn As Integer = 1
			For Each oColumn As Column In oColumns
				If oColumn.Render = False Then Continue For

				If Not oDataView Is Nothing Then
					sDataColumn = oDataView.Table.Columns(iColumn - 1).ColumnName
				End If

				'If we have a numeric column then add a class so we can right align and stuff
				Dim sClass As String = ""
				If oColumn.ColumnType = ColumnTypes.Numeric Then sClass = " numeric"

				' For control type add class and header
				Dim sHeader As String = oColumn.ColumnName
				Select Case oColumn.ControlType

					' Checkbox
					Case ColumnControlTypes.Checkbox
						sClass = " checkbox"
						sHeader = String.Format("<label><input type=""checkbox"" onchange=""Datalist.HeaderCheckbox(this, {0});"" /> {1}</label>",
							ID,
							oColumn.ColumnName)

				End Select

				'if it's an image column then we only want to add an empty cell
				If oColumn.ColumnType <> ColumnTypes.Image Then
					oSB.AppendFormat(sHeaderBase,
						iColumn.ToString,
						sHashedID,
						sHeader,
						iColumn - 1,
						sClass)
				Else
					oSB.AppendFormat("<td class=""c{0}"">&nbsp</td>\n", iColumn)
				End If

				iColumn += 1
			Next

			'add final column and close row and table, work out final column content
			Dim sFinalColumnContent As String

			If Not bExcel Then
				sFinalColumnContent = "&nbsp;"
			Else

				Dim iColumnCount As Integer = IIf(iExcelColumns = 0, Me.Columns.Count, iExcelColumns)
				sFinalColumnContent = String.Format("<a href=""Javascript:ExcelExport('{0}','{1}','{2}',{3});"" " &
				 "alt=""Excel Export"" title=""Excel Export""><img src=""{4}""/></a>",
				 UrlEncode(Me.ResolveUrl("~/Secure/WebServices/DatalistExcelExport.aspx")),
				 UrlEncode(Page.ToString), UrlEncode(Me.ID), iColumnCount,
				 sImageRoot & "excel-export.gif")
			End If

			oSB.AppendFormat("<td class=""c{0} last"">{1}</td>\n", iColumn, sFinalColumnContent)
			oSB.Append("</tr>\n")
			oSB.Append("</table>\n\n")

			'create scrolling div and table header
			oSB.AppendFormat("<div id=""{0}Scroll"" class=""scroll"">\n", Me.ID)

			'if we have some data, let's do this
			If oDataView IsNot Nothing AndAlso oDataView.Count > 0 AndAlso oDataView.Table.Rows.Count > 0 Then

				Dim sLastGroup As String = "cockpisspartridge"

				'add the table header
				oSB.AppendFormat("<table id=""{0}"" class=""dl"">\n", sHashedID)

				'sort the data if required
				If iSortColumn <> -1 Then
					oDataView.Sort = oDataView.Table.Columns(iSortColumn).ColumnName & IIf(bSortAscending, "", " desc")
				End If

				' Work out the ID column number
				Dim iIDColumnIndex As Integer = IDColumnIndex

				'for each row in the dataview
				For iRow As Integer = 0 To oDataView.Count - 1

					' Get the current row ID, or set to the row index if we are overriding
					Dim iID As Integer

					If Not bOverrideIDWithIndex Then
						iID = CType(oDataView(iRow).Item(iIDColumnIndex), Integer)
					Else
						iID = iRow + 1
					End If

					Dim sRowClass As String = ""
					Dim sGroupRowClass As String = ""
					Dim bInlineImage As Boolean = False
					Dim bPointCell As Boolean = False

					'and add style for style overrides if necessary
					If oStyleOverrides.Count > 0 Then
						For Each oStyleOverride As StyleOverride In oStyleOverrides
							If oDataView(iRow).Item(oStyleOverride.ColumnName).ToString = oStyleOverride.Condition Then
								sRowClass += " " & oStyleOverride.ClassName
								If oStyleOverride.PointCell Then
									bPointCell = True
								End If

							End If

							If oStyleOverride.ImageColumn <> "" Then bInlineImage = True
						Next
					End If

					'if we have a group column  then check if it differs from
					'last group value and add group header
					If Me.GroupColumn <> "" Then
						Dim sGroup As String = SafeString(oDataView(iRow).Item(sGroupColumn))

						'stick a group header in if necessary
						If sGroup <> sLastGroup Then
							Dim sClass As String = IIf(sLastGroup = "cockpisspartridge", "lstfirstgrouprow", "lstgrouprow").ToString

							'add the style override class if necessary
							If bStyleOverrideGroupClass Then
								sGroupRowClass += sRowClass
							End If

							oSB.AppendFormat("<tr class=""{0}"">\n", sGroupRowClass)
							oSB.AppendFormat("<td class=""{0}"" colspan=""" & Me.Columns.Count + 2 & """>{1}</td>\n", sClass, sGroup)
							oSB.Append("</tr>\n")
						End If

						sLastGroup = sGroup
					End If

					'work out the row style
					If iID = iSelectedItemID Then
						sRowClass += " selected"
					End If

					sRowClass = sRowClass.Trim

					'write out the row start and stick the ol' data point in
					If sRowClass = "" Then
						oSB.AppendFormat("<tr id=""{0}_{1}"" class=""cols{2}"">\n",
							IIf(Me.UseHashedID, sHashedID, Me.ID),
							iID,
							Columns.RenderColumnsCount)
					Else
						oSB.Append(String.Format("<tr id=""{0}_{1}"" class=""{2}"">\n",
						 IIf(Me.UseHashedID, sHashedID, Me.ID), iID, sRowClass))
					End If

					If Me.ShowDataPoint AndAlso bPointCell Then

						Dim sInlineImage As String = ""
						Dim sTooltip As String

						For Each oStyleOverride As StyleOverride In oStyleOverrides

							If oStyleOverride.ClassName = sRowClass.Split(" "c)(0) Then
								If oStyleOverride.Tooltip <> "" Then
									sTooltip = oStyleOverride.Tooltip
								ElseIf oStyleOverride.TooltipColumn <> "" Then
									sTooltip = oDataView(iRow).Item(oStyleOverride.TooltipColumn).ToString
								Else
									sTooltip = ""
								End If

								'construct the image
								sInlineImage = String.Format("<img src=""{0}{1}"" {2} class=""inline{3}""/>",
								 sImageRoot, oStyleOverride.Image,
								 IIf(sTooltip <> "", String.Format("alt=""{0}"" title=""{0}""", sTooltip), ""),
								 IIf(sTooltip <> "", " tooltip", ""))

								Exit For
							End If
						Next

						oSB.AppendFormat("<td class=""point pointimage"">{0}</td>\n", sInlineImage)
					ElseIf Me.ShowDataPoint Then
						oSB.Append("<td class=""point""></td>\n")
					End If

					'for each column
					For iCol As Integer = 0 To oColumns.Count - 1

						Dim oColumn As Column = oColumns(iCol)
						Dim sClass As String = ""
						Dim sTooltip As String

						If oColumn.Render = False Then Continue For

						'if we've got any tooltips then sort it out here,
						'we set a flag when scanning through earlier so just check that
						Dim sInlineImage As String = ""

						If bInlineImage Then
							For Each oStyleOverride As StyleOverride In oStyleOverrides
								If oDataView(iRow).Item(oStyleOverride.ColumnName).ToString = oStyleOverride.Condition AndAlso
								 oColumn.ColumnName = oStyleOverride.ImageColumn Then

									'get the tooltip text
									If oStyleOverride.Tooltip <> "" Then
										sTooltip = oStyleOverride.Tooltip
									ElseIf oStyleOverride.TooltipColumn <> "" Then
										sTooltip = oDataView(iRow).Item(oStyleOverride.TooltipColumn).ToString
									Else
										sTooltip = ""
									End If

									'construct the image
									sInlineImage = String.Format("<img src=""{0}{1}"" {2} class=""inline{3}""/>",
									 sImageRoot, oStyleOverride.Image,
									 IIf(sTooltip <> "", String.Format("alt=""{0}"" title=""{0}""", sTooltip), ""),
									 IIf(sTooltip <> "", " tooltip", ""))

								End If
							Next
						End If

						'sort out the datavalue
						Dim sDataValue As String = oDataView.Item(iRow).Item(iCol).ToString.Trim

						Select Case oColumn.ColumnType
							Case ColumnTypes.Text
								If oColumn.MaxFieldLength > 0 AndAlso sDataValue.Length > oColumn.MaxFieldLength Then
									sDataValue = sDataValue.Substring(0, oColumn.MaxFieldLength).Trim & ".."
								End If
							Case ColumnTypes.DateFormat
								'If empty Date then just display nothing
								If Not sDataValue = "" Then
									sDataValue = DateFunctions.DisplayDate(CType(sDataValue, Date))
								End If
							Case ColumnTypes.DateTimeFormat
								'If empty Date then just display nothing
								If Not sDataValue = "" Then
									sDataValue = DateFunctions.DisplayDateTime(CType(sDataValue, Date))
								End If
							Case ColumnTypes.Money

								Dim oRegex As New Regex("[-+]?[0-9]+([.][0-9]+)?")

								' Get value and currency
								Dim sValue As String = oRegex.Match(sDataValue).Value
								Dim dValue As Decimal = sValue.ToSafeDecimal
								Dim sCurrency As String = sDataValue.Replace(sValue, String.Empty)

								If oColumn.SuppressZeros AndAlso dValue = 0 Then
									sDataValue = String.Empty
								Else
									sDataValue = sCurrency & dValue.ToString("#0.00")
								End If

								sClass = " numeric"
							Case ColumnTypes.NumberInt
								If SafeInt(sDataValue) <> 0 OrElse Not oColumn.SuppressZeros Then
									sDataValue = SafeInt(sDataValue).ToString
								Else
									sDataValue = ""
								End If

							Case ColumnTypes.CustomHTML
								If iRow = 0 Then
									sDataValue = String.Format(Columns(iCol).FirstRowHTML, iID.ToString)
								ElseIf iRow = oDataView.Count - 1 Then
									sDataValue = String.Format(Columns(iCol).LastRowHTML, iID.ToString)
								Else
									sDataValue = String.Format(Columns(iCol).RowHTML, iID.ToString)
								End If
							Case ColumnTypes.Numeric
								sClass = " numeric"

							Case ColumnTypes.Bool
								Select Case oColumn.ControlType

									' Checkbox
									Case ColumnControlTypes.Checkbox
										sDataValue = String.Format("<input type=""checkbox"" name=""chk{0}_{1}_{2}"" value=""{3}"" {4} />",
											ID,
											iID,
											iCol,
											sDataValue,
											IIf(sDataValue.ToSafeInt = 1 OrElse sDataValue.ToLower = "true", "checked", ""))
										sClass = " checkbox"

										' Default
									Case Else
										sDataValue = IIf(sDataValue.ToLower = "true", "Yes", "").ToString

								End Select

							Case ColumnTypes.Image
								If sDataValue <> "" Then
									sDataValue = String.Format("<img src=""{0}{1}""/>", Me.Columns(iCol).ImageBase, sDataValue)
								End If

						End Select

						If (iRow = 0 OrElse Not Me.SuppressCellClass OrElse Me.StyleOverrides.Count > 0) _
						  AndAlso oColumn.Mouseover <> "" AndAlso oColumn.Mouseout <> "" Then

							Dim sMouseover As String = oColumn.Mouseover
							Dim sMouseout As String = oColumn.Mouseout

							If oColumn.Mouseover.Contains("{0}") Then sMouseover = String.Format(oColumn.Mouseover, iID.ToString)
							If oColumn.Mouseout.Contains("{0}") Then sMouseout = String.Format(oColumn.Mouseout, iID.ToString)

							oSB.Append(String.Format("<td class=""c{0}{2}"" onmouseover=""{4}"" onmouseout=""{5}"">{3}{1}</td>\n", iCol + 1, sDataValue,
							   sClass, sInlineImage, sMouseover, sMouseout))
						ElseIf iRow = 0 OrElse Not Me.SuppressCellClass OrElse Me.StyleOverrides.Count > 0 Then
							oSB.Append($"<td {oColumn.ContentEditableTag} class=""c{iCol + 1}{sClass}"">{sInlineImage}{sDataValue}</td>\n")
						ElseIf oColumn.Mouseover <> "" AndAlso oColumn.Mouseout <> "" Then

							Dim sMouseover As String = oColumn.Mouseover
							Dim sMouseout As String = oColumn.Mouseout

							If oColumn.Mouseover.Contains("{0}") Then sMouseover = String.Format(oColumn.Mouseover, iID.ToString)
							If oColumn.Mouseout.Contains("{0}") Then sMouseout = String.Format(oColumn.Mouseout, iID.ToString)

							oSB.Append(String.Format("<td onmouseover=""{2}"" onmouseout=""{3}"">{1}{0}</td>\n", sDataValue, sInlineImage, sMouseover, sMouseout))
						Else
							oSB.Append($"<td {oColumn.ContentEditableTag}>{sInlineImage}{sDataValue}</td>\n")
						End If

					Next iCol

					'set up defualt list commands
					Dim bClearListCommand As Boolean = False

					If (oListCommands.Count = 0 OrElse bAddEditDelete) AndAlso Not bSuppressEditDelete Then
						bClearListCommand = True
						If sEditCommandOverride = "" Then
							Me.AddListCommand(sEditImage, "E", "Edit Item")
						Else
							Dim oListCommand As New ListCommand(sEditImage, "", "Edit Item")
							oListCommand.URL = String.Format(sEditCommandOverride, iID.ToString)
							oListCommands.Add(oListCommand)
						End If

						If sDeleteCommandOverride = "" Then
							Me.AddListCommand(sDeleteImage, "D", "Delete Item", True)
						Else
							Dim oListCommand As New ListCommand(sDeleteImage, "", "Delete Item")
							oListCommand.URL = String.Format(sDeleteCommandOverride, iID.ToString)
							oListCommands.Add(oListCommand)
						End If

					End If

					'start the operation cell
					oSB.Append("<td class=""dlop"">\n")

					'now write the list command cells
					'for each list command add new cell/hyperlink combo
					For Each oListCommand As ListCommand In oListCommands
						If oListCommand.Condition = "" OrElse oListCommand.Condition = oDataView(iRow).Item(oListCommand.ColumnName).ToString Then
							oListCommand.DisableLinks = Me.DisableLinks
							Dim sHyperlink As String = oListCommand.GetHyperlink(sHashedID, iID, oDataView(iRow),
							 IIf(Me.OverrideIDWithIndex, Me.LookupSelectedID(iID), iID))
							oSB.Append(sHyperlink).Append("\n")
						End If
					Next

					'clear out edit/delete, get rid of last two
					If bClearListCommand Then
						oListCommands.RemoveAt(oListCommands.Count - 1)
						oListCommands.RemoveAt(oListCommands.Count - 1)
					End If

					'close off the drop cell and the row for that matter
					oSB.Append("</td>\n").Append("</tr>\n")
				Next iRow

				'close the table
				oSB.Append("</table>\n")
			End If

			'close content table + scrolling div
			oSB.Append("</div>\n")

			' Footer
			If FooterEnabled Then
				oSB.Append("<table class=""footer"">")
				oSB.Append("<tfoot>")
				oSB.AppendFormat("<tr class=""cols{0}"">",
					Columns.RenderColumnsCount)
				oSB.Append("<td class=""point"">&nbsp;</td>")

				iColumn = 0
				For Each oColumn As Column In Columns
					If oColumn.Render = False Then Continue For

					Dim sClass As String = ""
					Dim sContent As String = ""

					If oColumn.ColumnType = ColumnTypes.Numeric Then sClass = "numeric"

					If FooterColumns.ContainsKey(iColumn) Then

						Select Case FooterColumns.Item(iColumn).Type

							' Label
							Case FooterColumnTypes.Label
								sClass += " label"
								sContent = FooterColumns.Item(iColumn).Name

								' Sum
							Case FooterColumnTypes.Sum
								If FooterColumns.Item(iColumn).Name IsNot Nothing Then sContent = FooterColumns.Item(iColumn).Name

						End Select

					End If

					oSB.AppendFormat("<td class=""c{0} {1}"">{2}</td>", iColumn + 1, sClass, sContent)

					iColumn += 1
				Next

				oSB.AppendFormat("<td class=""c{0} last""></td>", iColumn + 1)
				oSB.Append("</tr>")
				oSB.Append("</tfoot>")
				oSB.Append("</table>")
			End If

			'add the paging control if required
			If Me.Paged AndAlso Not oDataView Is Nothing AndAlso oDataView.Count > 0 AndAlso
				oDataView.Table.Rows.Count > 0 AndAlso SafeInt(oDataView(0)("totalrowcount")) > Me.RowsPerPage Then

				oSB.Append("<div class=""paging"">\n")
				Dim oPagingControl As New Paging
				Dim iPage As Integer = Me.CurrentPage
				With oPagingControl
					.CurrentPage = iPage
					.TotalPages = SafeInt(Math.Ceiling(SafeInt(oDataView(0)("totalrowcount")) / SafeDecimal(Me.RowsPerPage)))
					.TotalLinksToDisplay = Me.TotalLinksToDisplay
					.ScriptPage = "Postback('" & sHashedID & "SetPage',{0});"
					.ScriptPrevious = String.Format("Postback('{0}SetPage',{1});", sHashedID, iPage - 1)
					.ScriptNext = String.Format("Postback('{0}SetPage',{1});", sHashedID, iPage + 1)
				End With
				oSB.Append(Intuitive.Functions.RenderControlToString(oPagingControl))
				oSB.Append("</div>\n")
			End If

			'close final div
			oSB.Append("</div>\n")

			'add a call to the scroll top function if we have a selected item
			If Me.SelectedItemID > 0 Then
				Page.ClientScript.RegisterStartupScript(GetType(Page), Me.GetHashedID & "SetScroll",
				 String.Format("<script language=""javascript"">DataListSetScroll('{0}','{1}_{2}');</script>",
				  Me.ID, Me.GetHashedID, Me.SelectedItemID))
			End If

			If Me.OneClick AndAlso Not oDataView Is Nothing AndAlso oDataView.Count > 0 Then
				oSB.Append("<script type=""text/javascript"">\n")
				oSB.AppendFormat("f.AttachEvent('{0}', 'click', function(oEvent) {{ Datalist.ClickHandler(f.GetObjectFromEvent(oEvent)) }});", sHashedID)
				oSB.Append("</script>")
			End If

			writer.Write(oSB.ToString.Replace("\n", Environment.NewLine))

		End Sub

#End Region

#Region "Support Functions"

		Public Sub SetSourceSQL(ByVal sSQL As String, ByVal ParamArray aParams() As Object)
			Me.SourceSql = SQL.FormatStatement(sSQL, aParams)
		End Sub

		Public Function GetHashedID() As String
			Dim sID As String = Me.ID
			Return "lst" & (sID.GetHashCode Mod 1000).ToString
		End Function

		Public Overloads Sub AddStyleOverride(ByVal ClassName As String, ByVal ColumnName As String,
		 ByVal Condition As String, Optional ByVal ImageColumn As String = "",
		 Optional ByVal Image As String = "", Optional ByVal TooltipColumn As String = "",
		 Optional ByVal Tooltip As String = "", Optional ByVal PointCell As Boolean = False)

			oStyleOverrides.Add(New StyleOverride(ClassName, ColumnName, Condition, ImageColumn,
			 Image, TooltipColumn, Tooltip, PointCell))
		End Sub

		Public Sub AddListCommand(ByVal Image As String, ByVal Command As String, ByVal ToolTip As String,
		 Optional ByVal Confirm As Boolean = False, Optional ByVal ImageClass As String = "",
		 Optional ByVal ConfirmVerb As String = "", Optional ByVal ConfirmNoun As String = "")
			oListCommands.Add(New ListCommand(Image, Command, ToolTip, Confirm, ImageClass,
			 ConfirmVerb, ConfirmNoun))
		End Sub

		Public Sub AddScriptCommand(ByVal Image As String, ByVal Script As String,
		 ByVal ToolTip As String, Optional ByVal ImageClass As String = "")

			Dim oListCommand As New ListCommand
			oListCommand.Image = Image
			oListCommand.Script = Script
			oListCommand.Tooltip = ToolTip
			oListCommand.ImageClass = ImageClass

			oListCommands.Add(oListCommand)

		End Sub

		Public Sub AddHoverCommand(ByVal Image As String, ByVal MouseOver As String,
		 ByVal MouseOut As String, Optional ByVal ImageClass As String = "")

			Dim oListCommand As New ListCommand
			oListCommand.Image = Image
			oListCommand.MouseOver = MouseOver
			oListCommand.MouseOut = MouseOut
			oListCommand.ImageClass = ImageClass

			oListCommands.Add(oListCommand)

		End Sub

		Private Function ModifiedPagedSQL() As String

			Dim aSQL() As String = Me.SourceSql.Trim.Split(" "c)
			Dim bHasParamsAlready As Boolean = False
			If aSQL(0) = "exec" Then
				bHasParamsAlready = aSQL.Length > 2
			Else
				bHasParamsAlready = aSQL.Length > 1
			End If

			Dim osbPaging As New StringBuilder
			osbPaging.Append(Me.SourceSql)

			If bHasParamsAlready Then osbPaging.Append(",")
			osbPaging.Append(" {0}, {1}")
			Return SQL.FormatStatement(osbPaging.ToString, Me.CurrentPage, Me.RowsPerPage)

		End Function

#End Region

#Region "Event Checking On Load"

		Protected Overrides Sub OnLoad(ByVal e As System.EventArgs)
			Dim sHashedID As String = GetHashedID()

			'if this page is a postback and the command
			'is set to tabclick, set the selected page
			'picked up from the standard command/argument structure
			If Page.IsPostBack AndAlso Not Me.Command Is Nothing Then

				If Me.Command = sHashedID & "E" Then
					Me.SelectedItemID = CType(Me.Argument, Integer)
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

				ElseIf Me.Command = sHashedID & "SetPage" Then
					Me.Clear()
					Me.CurrentPage = SafeInt(Me.Argument)
					RaiseEvent PageChange(SafeInt(Me.Argument))

				ElseIf Me.Command.StartsWith(sHashedID) Then

					Dim sType As String = Me.Command.Replace(sHashedID, "")
					RaiseEvent CustomEvent(sType, CType(Me.Argument, Integer))
				End If

				'do we need to clear the Datalist HTML Output
				'for re-generation?
				If Me.Command = sHashedID & "E" OrElse
				 Me.Command = sHashedID & "D" OrElse
				 Me.Command = sHashedID & "HeaderClick" Then

					'remove saved session DL Table html
					Context.Session.Remove("DLTable" & Me.ID & Page.ToString)
				End If

			End If

		End Sub

#End Region

#Region "Set Session Variables"

#Region "Get/Set Data View"

        'set dataview
        Public Function GetDataView() As DataView
            Dim oDataTable As DataTable

            If Not Me.OverrideDataTable Is Nothing Then
                oDataTable = Me.OverrideDataTable
                oDataView = oDataTable.DefaultView
            Else
                Dim sSQL As String = Me.SourceSql

                'if we have xml the override the sql
                If Me.SourceXML <> "" Then
                    Return Me.GetDataViewFromXML()
                End If

                oDataView = CType(Context.Session("DataList" & Me.ID & Me.Page.ToString), DataView)
                If oDataView Is Nothing Then
                    If sSQL <> "" Then

                        'clear the Data list output html
                        Context.Session("DLOutput" & Me.ID & Me.Page.ToString) = ""

                        'if we have paging then stick on the paging params
                        If Me.Paged Then sSQL = Me.ModifiedPagedSQL()

                        'get the datatable and store it in the datasession
                        oDataTable = SQL.GetDataTable(sSQL)

                        If Me.Paged AndAlso oDataTable.Columns("totalrowcount") Is Nothing Then
                            Throw New Exception("paged data list stored procedure must return a totalrowcount column")
                        End If
                        oDataView = oDataTable.DefaultView
                        Me.SetDataView(oDataView)

                    End If
                End If
            End If

            Return oDataView
        End Function

        Public Function GetDataViewFromXML() As DataView

			'NOTE - to use xml a strict xml structure must be followed
			'eg. <Roots>
			'		<Root>
			'			<RootID/>
			'			<ColumnName/>
			'			...
			'		</Root>
			'	</Roots>

			Dim oSourceXML As New XmlDocument
			oSourceXML.LoadXml(Me.SourceXML)

			oDataView = CType(Context.Session("DataList" & Me.ID & Me.Page.ToString), DataView)
			If oDataView Is Nothing Then

				'clear the Data list output html
				Context.Session("DLOutput" & Me.ID & Me.Page.ToString) = ""

				'get the datatable and store it in the datasession
				Dim oDataTable As New DataTable

				'loop through xml nodes and add rows to the table
				Dim sRootName As String = ""

				If oSourceXML.FirstChild.Name <> "xml" Then
					sRootName = oSourceXML.FirstChild.Name
				Else
					oSourceXML.RemoveChild(oSourceXML.FirstChild)
					sRootName = oSourceXML.FirstChild.Name
				End If

				For Each oColumn As Intuitive.WebControls.Column In Me.Columns
					oDataTable.Columns.Add(oColumn.ColumnName.Replace(" ", ""))
				Next

				oDataTable.Columns.Add(sRootName.Chop & "ID")

				For Each oNode As XmlNode In oSourceXML.SelectNodes(sRootName & "/" & sRootName.Chop)
					Dim dr As DataRow = oDataTable.NewRow()

					For Each oColumn As Intuitive.WebControls.Column In Me.Columns

						If oNode.SelectSingleNode(oColumn.ColumnName.Replace(" ", "")) IsNot Nothing Then
							dr(oColumn.ColumnName.Replace(" ", "")) = oNode.SelectSingleNode(oColumn.ColumnName.Replace(" ", "")).InnerText
						End If
					Next

					dr(sRootName.Chop & "ID") = oNode.SelectSingleNode(sRootName.Chop & "ID").InnerText

					oDataTable.Rows.Add(dr)
				Next

				If Me.Paged AndAlso oDataTable.Columns("totalrowcount") Is Nothing Then
					Throw New Exception("paged data list stored procedure must return a totalrowcount column")
				End If

				oDataView = oDataTable.DefaultView
				Me.SetDataView(oDataView)

			End If

			Return oDataView
		End Function

		'set data view
		Public Sub SetDataView(ByVal oDataView As DataView)
			Context.Session("DataList" & Me.ID & Page.ToString) = oDataView
		End Sub

#End Region

#Region "Get/Set Session Current Page"

		Public Property SessionCurrentPage() As Integer
			Get
				If Me.Page Is Nothing Then Return 1
				Dim iCurrentPage As Integer = SafeInt(Context.Session("DataListCurrentPage" & Me.ID & Me.Page.ToString))
				If iCurrentPage = 0 Then iCurrentPage = 1
				Return iCurrentPage
			End Get
			Set(ByVal value As Integer)
				Context.Session("DataListCurrentPage" & Me.ID & Me.Page.ToString) = value
			End Set
		End Property

#End Region

#Region "Lookup Functions"

		Public Function LookUpColumnByID(ByVal iID As Integer, ByVal sColumnName As String) As String

			Dim oView As DataView = Me.GetDataView()
			For Each dr As DataRow In oView.Table.Rows

				If (SafeInt(dr(oView.Table.Columns.Count - 1)) = iID) Then
					Return dr(sColumnName).ToString
				End If
			Next

			Return ""

		End Function

		Public Function LookupColumn(ByVal iRowIndex As Integer, ByVal sColumnName As String) As String
			Dim oView As DataView = Me.GetDataView()
			Return oView.Table.Rows(iRowIndex - 1)(sColumnName).ToString()
		End Function

		Public Function LookupSelectedID(ByVal iRowIndex As Integer) As Integer
			Dim oView As DataView = Me.GetDataView()

			If iRowIndex <= oView.Table.Rows.Count AndAlso iRowIndex > 0 Then
				Return SafeInt(oView.Table.Rows(iRowIndex - 1)(oView.Table.Columns.Count - 1))
			Else
				Return 0
			End If

		End Function

		Public Function LookupIndex(ByVal iID As Integer, ByVal sColumnName As String,
		  ByVal sColumnValue As String, Optional ByVal bSetIndex As Boolean = False) As Integer

			Dim oView As DataView = Me.GetDataView()
			Dim dr As DataRow
			Dim iIndex As Integer = 0
			For iLoop As Integer = 0 To oView.Table.Rows.Count

				dr = oView.Table.Rows(iLoop)
				If dr(sColumnName).ToString = sColumnValue AndAlso
				  SafeInt(dr(oView.Table.Columns.Count - 1)) = iID Then
					iIndex = iLoop
					Exit For
				End If
			Next

			Return iIndex + 1

		End Function

#End Region

#End Region

		''' <summary>
		''' Gets the checked IDs.
		''' </summary>
		''' <param name="ColumnIndex">Index of the column.</param>
		''' <returns>List of checked IDs.</returns>
		Public Function GetCheckedIDs(ColumnIndex As Integer) As List(Of Integer)

			Dim aIDs As New List(Of Integer)

			For Each oKey As String In Page.Request.Form.AllKeys
				If oKey.StartsWith(String.Format("chk{0}_", ID)) AndAlso oKey.EndsWith(ColumnIndex.ToString) Then
					Dim aSections() As String = oKey.Split("_".ToCharArray)

					aIDs.Add(aSections(1).ToSafeInt)
				End If
			Next

			Return aIDs

		End Function

	End Class

	'columns
#Region "Columns"

	''' <summary>
	''' Column control types.
	''' </summary>
	Public Enum ColumnControlTypes

		''' <summary>
		''' Checkbox.
		''' </summary>
		Checkbox = 1

	End Enum

	Public Class Columns
		Inherits Generic.List(Of Column)

		''' <summary>
		''' Gets the render columns count.
		''' </summary>
		''' <value>
		''' The render columns count.
		''' </value>
		Public ReadOnly Property RenderColumnsCount As Integer
			Get
				Dim iCount As Integer = 0

				For Each oColumn As Column In Me
					If oColumn.Render = True Then iCount += 1
				Next

				Return iCount
			End Get
		End Property

#Region "Add Column"

		''' <summary>
		''' Adds a column.
		''' </summary>
		''' <param name="Name">The column name.</param>
		''' <param name="Type">The column type.</param>
		''' <param name="ControlType">Type of the column control.</param>
		Public Overloads Sub Add(Name As String, Type As ColumnTypes, ControlType As ColumnControlTypes)

			Me.Add(New Column(Name, Type, ControlType))

		End Sub

		''' <summary>
		''' Adds a column.
		''' </summary>
		''' <param name="ControlType">Type of the column control.</param>
		Public Overloads Sub Add(ControlType As ColumnControlTypes)

			Me.Add(New Column(ControlType))

		End Sub

		''' <summary>
		''' Adds a column.
		''' </summary>
		''' <param name="Render">If set to <c>true</c> render column.</param>
		Public Overloads Sub Add(Render As Boolean)

			Me.Add(New Column(Render))

		End Sub

		Public Sub AddColumn(ByVal ColumnName As String,
		 Optional ByVal ColumnType As ColumnTypes = ColumnTypes.Text,
		 Optional ByVal MaxColumnLength As Integer = 0,
		 Optional ByVal SuppressZeros As Boolean = False,
		 Optional ByVal ImageBase As String = "",
		 Optional ByVal ContentEditable As Boolean = False)

			Me.Add(New Column(ColumnName, ColumnType, MaxColumnLength, SuppressZeros, ImageBase, ContentEditable))

		End Sub

#End Region

#Region "Viewstate Compressors"

		'get view state from columns
		Public Function GetViewState() As String

			Dim oSB As New StringBuilder

			For Each oColumn As Column In Me
				oSB.Append(String.Join("|",
					oColumn.ColumnName,
					oColumn.FirstRowHTML,
					oColumn.RowHTML,
					oColumn.LastRowHTML,
					oColumn.MaxFieldLength,
					oColumn.ColumnType,
					oColumn.SuppressZeros,
					oColumn.ImageBase,
					oColumn.Mouseover,
					oColumn.Mouseout,
					oColumn.ControlType,
					oColumn.ContentEditable))
				oSB.Append("^")
			Next

			Return oSB.ToString

		End Function

		'restore columns from viewstate
		Public Sub RestoreFromViewState(ByVal sString As String)

			Dim aColumns() As String = sString.Split("^".ToCharArray)

			For iColumn As Integer = 0 To aColumns.Length - 2

				Dim sColumn As String = aColumns(iColumn)
				Dim aProperties() As String = sColumn.Split("|".ToCharArray)
				Dim oColumn As New Column

				With oColumn
					.ColumnName = aProperties(0)
					.FirstRowHTML = aProperties(1)
					.RowHTML = aProperties(2)
					.LastRowHTML = aProperties(3)
					.MaxFieldLength = CType(aProperties(4), Integer)
					Select Case aProperties(5)
						Case "Text"
							.ColumnType = ColumnTypes.Text
						Case "DateFormat"
							.ColumnType = ColumnTypes.DateFormat
						Case "NumberInt"
							.ColumnType = ColumnTypes.NumberInt
						Case "Money"
							.ColumnType = ColumnTypes.Money
						Case "CustomHTML"
							.ColumnType = ColumnTypes.CustomHTML
						Case "Numeric"
							.ColumnType = ColumnTypes.Numeric
						Case "Bool"
							.ColumnType = ColumnTypes.Bool
						Case "DateTimeFormat"
							.ColumnType = ColumnTypes.DateTimeFormat
						Case "Image"
							.ColumnType = ColumnTypes.Image
					End Select
					.SuppressZeros = CType(aProperties(6), Boolean)
					.ImageBase = aProperties(7)
					.Mouseover = aProperties(8)
					.Mouseout = aProperties(9)

					Select Case aProperties(10)

						' Checkbox
						Case "Checkbox"
							.ControlType = ColumnControlTypes.Checkbox

					End Select

					.ContentEditable = aProperties(11).ToSafeBoolean

				End With

				Me.Add(oColumn)

			Next

		End Sub

#End Region

	End Class

#End Region

#Region "Column Type Enum"

	Public Enum ColumnTypes
		Text = 1
		DateFormat = 2
		NumberInt = 3
		Money = 4
		CustomHTML = 5
		Numeric = 6
		Bool = 7
		DateTimeFormat = 8
		Image = 9
	End Enum

#End Region

#Region "Column"

	Public Class Column

#Region "Variables"

		Private sColumnName As String = ""
		Private oColumnType As ColumnTypes
		Private sFirstRowHTML As String = ""
		Private sRowHTML As String = ""
		Private sLastRowHTML As String = ""
		Private iMaxFieldLength As Integer = 0
		Private bSuppressZeros As Boolean = False
		Private sImageBase As String = ""
		Private sMouseover As String = ""
		Private sMouseout As String = ""

#End Region

#Region "Properties"

		Public Property ColumnName() As String
			Get
				Return sColumnName
			End Get
			Set(ByVal Value As String)
				sColumnName = Value
			End Set
		End Property

		Public Property ColumnType() As ColumnTypes
			Get
				Return oColumnType
			End Get
			Set(ByVal Value As ColumnTypes)
				oColumnType = Value
			End Set
		End Property

		''' <summary>
		''' The column control type.
		''' </summary>
		Public ControlType As ColumnControlTypes

		Public Property FirstRowHTML() As String
			Get
				Return sFirstRowHTML
			End Get
			Set(ByVal Value As String)
				sFirstRowHTML = Value
			End Set
		End Property

		Public Property RowHTML() As String
			Get
				Return sRowHTML
			End Get
			Set(ByVal Value As String)
				sRowHTML = Value
			End Set
		End Property

		Public Property LastRowHTML() As String
			Get
				Return sLastRowHTML
			End Get
			Set(ByVal Value As String)
				sLastRowHTML = Value
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

		Public Property SuppressZeros() As Boolean
			Get
				Return bSuppressZeros
			End Get
			Set(ByVal Value As Boolean)
				bSuppressZeros = Value
			End Set
		End Property

		Public Property ImageBase() As String
			Get
				Return sImageBase
			End Get
			Set(ByVal value As String)
				sImageBase = value
			End Set
		End Property

		Public Property Mouseover As String
			Get
				Return sMouseover
			End Get
			Set(value As String)
				sMouseover = value
			End Set
		End Property

		Public Property Mouseout As String
			Get
				Return sMouseout
			End Get
			Set(value As String)
				sMouseout = value
			End Set
		End Property

		''' <summary>
		''' Render column.
		''' </summary>
		Public Render As Boolean = True

		Public Property ContentEditable As Boolean

		Public ReadOnly Property ContentEditableTag As String
			Get
				Return IIf(ContentEditable, $"ContentEditable = {ContentEditable}", "")
			End Get
		End Property

#End Region

#Region "Constructors"

		Public Sub New(ByVal ColumnName As String,
		 Optional ByVal ColumnType As ColumnTypes = ColumnTypes.Text,
		 Optional ByVal MaxColumnLength As Integer = 0,
		 Optional ByVal SuppressZeros As Boolean = False,
		 Optional ByVal ImageBase As String = "",
		 Optional ByVal bContentEditable As Boolean = False)

			sColumnName = ColumnName
			oColumnType = ColumnType
			iMaxFieldLength = MaxColumnLength
			bSuppressZeros = SuppressZeros
			sImageBase = ImageBase
			ContentEditable = bContentEditable

		End Sub

		''' <summary>
		''' Initializes a new instance of the <see cref="Column"/> class.
		''' </summary>
		''' <param name="Name">The column name.</param>
		''' <param name="Type">The column type.</param>
		''' <param name="ControlType">Type of the column control.</param>
		Public Sub New(Name As String, Type As ColumnTypes, ControlType As ColumnControlTypes)

			ColumnName = Name
			ColumnType = Type
			Me.ControlType = ControlType

		End Sub

		''' <summary>
		''' Initializes a new instance of the <see cref="Column"/> class.
		''' </summary>
		''' <param name="ControlType">Type of the column control.</param>
		Public Sub New(ControlType As ColumnControlTypes)

			Me.ControlType = ControlType

		End Sub

		''' <summary>
		''' Initializes a new instance of the <see cref="Column"/> class.
		''' </summary>
		''' <param name="Render">If set to <c>true</c> render column.</param>
		Public Sub New(Render As Boolean)

			Me.Render = Render

		End Sub

		Public Sub New()

		End Sub

#End Region

	End Class

#End Region

	''' <summary>
	''' Footer column types.
	''' </summary>
	Public Enum FooterColumnTypes
		Label
		Sum
	End Enum

	''' <summary>
	''' Footer columns.
	''' </summary>
	''' <seealso cref="System.Collections.Generic.Dictionary" />
	Public Class FooterColumns
		Inherits Dictionary(Of Integer, FooterColumn)

		''' <summary>
		''' Adds the footer column.
		''' </summary>
		''' <param name="Index">The column index.</param>
		''' <param name="Name">The footer column name.</param>
		''' <param name="Type">The footer column type.</param>
		Public Overloads Sub Add(Index As Integer, Name As String, Type As FooterColumnTypes)

			Add(Index, New FooterColumn(Name, Type))

		End Sub

		''' <summary>
		''' Adds the specified index.
		''' </summary>
		''' <param name="Index">The footer column index.</param>
		''' <param name="Type">The footer column type.</param>
		Public Overloads Sub Add(Index As Integer, Type As FooterColumnTypes)

			Add(Index, New FooterColumn(Type))

		End Sub

		''' <summary>
		''' Gets the state of the view.
		''' </summary>
		''' <returns></returns>
		Public Function GetViewState() As String

			Dim oSB As New StringBuilder

			For Each oColumn As KeyValuePair(Of Integer, FooterColumn) In Me
				oSB.Append(String.Join("|",
					oColumn.Key,
					oColumn.Value.Name,
					oColumn.Value.Type))
				oSB.Append("^")
			Next

			Return oSB.ToString

		End Function

		''' <summary>
		''' Sets the state of the view.
		''' </summary>
		''' <param name="ViewState">State of the view.</param>
		Public Sub SetViewState(ViewState As String)

			Dim aColumns() As String = ViewState.Split("^".ToCharArray)

			For iColumnIndex As Integer = 0 To aColumns.Length - 2

				Dim sColumn As String = aColumns(iColumnIndex)
				Dim aProperties() As String = sColumn.Split("|".ToCharArray)

				Dim oColumn As New FooterColumn

				With oColumn
					.Name = aProperties(1)

					Select Case aProperties(2)

						' Label
						Case "Label"
							.Type = FooterColumnTypes.Label

							' Sum
						Case "Sum"
							.Type = FooterColumnTypes.Sum

					End Select

				End With

				Add(aProperties(0).ToSafeInt, oColumn)

			Next

		End Sub

	End Class

	''' <summary>
	''' Footer column.
	''' </summary>
	Public Class FooterColumn

		''' <summary>
		''' The footer column name.
		''' </summary>
		Public Name As String

		''' <summary>
		''' The footer column type.
		''' </summary>
		Public Type As FooterColumnTypes

		''' <summary>
		''' Initializes a new instance of the <see cref="FooterColumn"/> class.
		''' </summary>
		''' <param name="Name">The footer column name.</param>
		''' <param name="Type">The footer column type.</param>
		Public Sub New(Name As String, Type As FooterColumnTypes)

			Me.Name = Name
			Me.Type = Type

		End Sub

		''' <summary>
		''' Initializes a new instance of the <see cref="FooterColumn"/> class.
		''' </summary>
		''' <param name="Type">The footer column type.</param>
		Public Sub New(Type As FooterColumnTypes)

			Me.Type = Type

		End Sub

		''' <summary>
		''' Initializes a new instance of the <see cref="FooterColumn"/> class.
		''' </summary>
		Public Sub New()
		End Sub

	End Class

	'list commands
#Region "List Commands"

	Public Class ListCommands
		Inherits Generic.List(Of ListCommand)

		'get view state from columns
		Public Function GetViewState() As String

			Dim oSB As New StringBuilder

			For Each oListCommand As ListCommand In Me
				oSB.Append(String.Join("|", oListCommand.Command, oListCommand.Confirm, oListCommand.DataColumn,
				 oListCommand.Image, oListCommand.ConfirmVerb, oListCommand.ConfirmNoun, oListCommand.Tooltip,
				 oListCommand.Script, oListCommand.URL, oListCommand.URLExtraColumn, oListCommand.ColumnName,
				 oListCommand.Condition, oListCommand.ImageClass, oListCommand.MouseOver, oListCommand.MouseOut))
				oSB.Append("^")
			Next

			Return oSB.ToString

		End Function

		'restore columns from viewstate
		Public Sub RestoreFromViewState(ByVal sString As String)

			Dim aListCommands() As String = sString.Split("^".ToCharArray)

			For iListCommand As Integer = 0 To aListCommands.Length - 2

				Dim sListCommand As String = aListCommands(iListCommand)
				Dim aProperties() As String = sListCommand.Split("|"c)
				Dim oListCommand As New ListCommand

				With oListCommand
					.Command = aProperties(0)
					.Confirm = CType(aProperties(1), Boolean)
					.DataColumn = CType(aProperties(2), Integer)
					.Image = aProperties(3)
					.ConfirmVerb = aProperties(4)
					.ConfirmNoun = aProperties(5)
					.Tooltip = aProperties(6)
					.Script = aProperties(7)
					.URL = aProperties(8)
					.URLExtraColumn = aProperties(9)
					.ColumnName = aProperties(10)
					.Condition = aProperties(11)
					.ImageClass = aProperties(12)
					.MouseOver = aProperties(13)
					.MouseOut = aProperties(14)
				End With

				Me.Add(oListCommand)

			Next

		End Sub

	End Class

#End Region

#Region "List Command"

	Public Class ListCommand

#Region "Variables"

		Private sImage As String = ""
		Private sCommand As String = ""
		Private sURL As String = ""
		Private sTooltip As String = ""
		Private sImageClass As String = ""
		Private bConfirm As Boolean = False
		Private iDataColumn As Integer = -1
		Private sConfirmVerb As String = ""
		Private sConfirmNoun As String = ""
		Private sColumnName As String = ""
		Private sCondition As String = ""


		Public URLExtraColumn As String = ""
		Public Script As String = ""
		Public MouseOver As String = ""
		Public MouseOut As String = ""
		Public DisableLinks As Boolean

#End Region

#Region "Properties"

		Public Property Image() As String
			Get
				Return sImage
			End Get
			Set(ByVal Value As String)
				sImage = Value
			End Set
		End Property

		Public Property Command() As String
			Get
				Return sCommand
			End Get
			Set(ByVal Value As String)
				sCommand = Value
			End Set
		End Property

		Public Property Tooltip() As String
			Get
				Return sTooltip
			End Get
			Set(ByVal Value As String)
				sTooltip = Value
			End Set
		End Property

		Public Property URL() As String
			Get
				Return sURL
			End Get
			Set(ByVal Value As String)
				sURL = Value
			End Set
		End Property

		Public Property Confirm() As Boolean
			Get
				Return bConfirm
			End Get
			Set(ByVal Value As Boolean)
				bConfirm = Value
			End Set
		End Property

		Public Property DataColumn() As Integer
			Get
				Return iDataColumn
			End Get
			Set(ByVal Value As Integer)
				iDataColumn = Value
			End Set
		End Property

		Public Property ImageClass() As String
			Get
				Return sImageClass
			End Get
			Set(ByVal Value As String)
				sImageClass = Value
			End Set
		End Property

		Public Property ConfirmVerb() As String
			Get
				Return sConfirmVerb
			End Get
			Set(ByVal Value As String)
				sConfirmVerb = Value
			End Set
		End Property

		Public Property ConfirmNoun() As String
			Get
				Return sConfirmNoun
			End Get
			Set(ByVal Value As String)
				sConfirmNoun = Value
			End Set
		End Property

		Public Property Condition() As String
			Get
				Return sCondition
			End Get
			Set(ByVal Value As String)
				sCondition = Value
			End Set
		End Property

		Public Property ColumnName As String
			Get
				Return sColumnName
			End Get
			Set(value As String)
				sColumnName = value
			End Set
		End Property

#End Region

#Region "Constructors"

		Public Sub New(ByVal Image As String, ByVal Command As String, ByVal Tooltip As String)
			sImage = Image
			sCommand = Command
			sTooltip = Tooltip
		End Sub

		Public Sub New()
			MyBase.New()
		End Sub

		Public Sub New(ByVal Image As String, ByVal Command As String, ByVal Tooltip As String,
		 ByVal Confirm As Boolean)
			sImage = Image
			sCommand = Command
			sTooltip = Tooltip
			bConfirm = Confirm
		End Sub

		Public Sub New(ByVal Image As String, ByVal Command As String, ByVal Tooltip As String,
		 ByVal Confirm As Boolean, ByVal ImageClass As String)
			sImage = Image
			sCommand = Command
			sTooltip = Tooltip
			bConfirm = Confirm
			sImageClass = ImageClass
		End Sub

		Public Sub New(ByVal Image As String, ByVal Command As String, ByVal Tooltip As String,
		   ByVal Confirm As Boolean, ByVal ImageClass As String, ByVal ConfirmVerb As String,
		   ByVal ConfirmNoun As String)
			sImage = Image
			sCommand = Command
			sTooltip = Tooltip
			bConfirm = Confirm
			sImageClass = ImageClass
			sConfirmVerb = ConfirmVerb
			sConfirmNoun = ConfirmNoun
		End Sub

        Public Sub New(ByVal Image As String, ByVal Command As String, ByVal Tooltip As String,
         ByVal Confirm As Boolean, ByVal ImageClass As String, ByVal Script As String)
            Me.New(Image, Command, Tooltip, Confirm, ImageClass)
            Me.Script = Script
        End Sub

#End Region

#Region "Get Hyperlink"

		Public Overloads Function GetHyperlink(
			ByVal sHashedID As String,
			ByVal iID As Integer,
			Optional ByVal oDataRowView As DataRowView = Nothing,
			Optional ByVal iRealID As Integer = 0) As String

			Return Me.GetHyperlink(sHashedID, iID, oDataRowView?.Row, iRealID)

		End Function

		Public Overloads Function GetHyperlink(
			ByVal sHashedID As String,
			ByVal iID As Integer,
			ByVal oDataRow As DataRow,
			Optional ByVal iRealID As Integer = 0) As String

			Dim sHyperlink As String
			Dim sLinkURL As String = ""
			Dim sImageRoot As String = "/images/"
			Dim sClass As String = ""
			Dim sID As String = "a_" & iID

			'if we have script bish it out nice and easy
			'rewritten to allow script and mouseover on the same elephant
			If Me.Script <> "" OrElse Me.MouseOver <> "" Then

				Dim sScript As String = String.Format(Me.Script, iID)
				If Me.DisableLinks Then
					sScript = "return false;"
				End If
				sClass = IIf(sImageClass <> "", "class=""" & sImageClass & """", "")

				Dim sMouseOver As String = String.Format(Me.MouseOver, iID)
				Dim sMouseOut As String = String.Format(Me.MouseOut, iID)

				Dim clickLink As String = String.Format("onclick=""{0}""", sScript)

				Return String.Format("<a id=""{6}"" href=""#"" {0} {1}><img src=""{2}"" alt=""{3}"" title=""{3}"" onmouseover=""{4}"" onmouseout=""{5}""/></a>",
				 clickLink, sClass, sImageRoot & Me.Image, Me.Tooltip, sMouseOver, sMouseOut, sID)
			End If

			'work out the url.  if a command then use the postback mechanism
			'else work out the url using the datacolumn in the row
			sHyperlink = "<a id=""{4}"" href=""{0}"" {3} {5}><img src=""{1}"" alt=""{2}"" title=""{2}""/></a>"

			If Not Me.DisableLinks Then
				If sCommand <> "" Then
					sLinkURL = "javascript:{4}Postback('{0}',{1}{2}{3})"

					Dim sFunction As String = ""

					'work out if we have any confirm verbs or nouns
					Dim sConfirm As String = ""

					If bConfirm AndAlso (sConfirmVerb <> "" OrElse sConfirmNoun <> "") Then
						sConfirm = String.Format(",'{0}','{1}'", sConfirmVerb, sConfirmNoun)

						sFunction = "Listbox"
					End If

					sLinkURL = String.Format(sLinkURL, sHashedID & sCommand, iID.ToString,
					 IIf(bConfirm, ",true", "").ToString, sConfirm, sFunction)
				Else

					sLinkURL = sURL
					If Me.URLExtraColumn <> "" AndAlso oDataRow IsNot Nothing Then
						sLinkURL = String.Format(sLinkURL, iRealID, oDataRow(Me.URLExtraColumn).ToString)
					ElseIf iDataColumn >= 0 AndAlso oDataRow IsNot Nothing Then
						sLinkURL = String.Format(sLinkURL, iRealID, oDataRow(iDataColumn).ToString)
					Else
						sLinkURL = String.Format(sLinkURL, iRealID)
					End If
				End If
			End If

			Dim sOnClick As String = IIf(Me.DisableLinks, "onclick=""return false;""", "")

			'work out the Image Class
			If sImageClass <> "" Then
				sClass = String.Format("class=""{0}""", sImageClass)
			End If

			'now build the megalink
			sHyperlink = String.Format(sHyperlink, sLinkURL, sImageRoot & sImage, sTooltip, sClass, sID, sOnClick)

			Return sHyperlink

		End Function

#End Region

	End Class

#End Region

	'style overrides
#Region "Style Overrides"

	Public Class StyleOverrides
		Inherits Generic.List(Of StyleOverride)

		'get view state from style overrides
		Public Function GetViewState() As String

			Dim oSB As New StringBuilder

			For Each oStyleOverride As StyleOverride In Me
				oSB.Append(String.Join("|", oStyleOverride.ColumnName, oStyleOverride.Condition, oStyleOverride.ClassName,
				 oStyleOverride.ImageColumn, oStyleOverride.Image, oStyleOverride.TooltipColumn, oStyleOverride.Tooltip,
				 oStyleOverride.HideCheckbox, oStyleOverride.DisableCheckbox))
				oSB.Append("^")
			Next

			Return oSB.ToString

		End Function

		'restore columns from viewstate
		Public Sub RestoreFromViewState(ByVal sString As String)

			Dim aStyleOverrides() As String = sString.Split("^".ToCharArray)

			For iStyleOverride As Integer = 0 To aStyleOverrides.Length - 2

				Dim sStyleOverride As String = aStyleOverrides(iStyleOverride)
				Dim aProperties() As String = sStyleOverride.Split("|".ToCharArray)
				Dim oStyleOverride As New StyleOverride

				With oStyleOverride
					.ColumnName = aProperties(0)
					.Condition = aProperties(1)
					.ClassName = aProperties(2)
					.ImageColumn = aProperties(3)
					.Image = aProperties(4)
					.TooltipColumn = aProperties(5)
					.Tooltip = aProperties(6)
					.HideCheckbox = SafeBoolean(aProperties(7))
					.DisableCheckbox = SafeBoolean(aProperties(8))
				End With

				Me.Add(oStyleOverride)

			Next

		End Sub

	End Class

#End Region

#Region "Style Override"

	Public Class StyleOverride

#Region "Variables"

		Private sClassName As String = ""
		Private sColumnName As String = ""
		Private sCondition As String = ""
		Private sImageColumn As String = ""
		Private sImage As String = ""
		Private sTooltipColumn As String = ""
		Private sTooltip As String = ""
		Private bPointCell As Boolean = False
		Private bHideCheckbox As Boolean = False
		Private bDisableCheckbox As Boolean = False

#End Region

#Region "Properties"

		Public Property ClassName() As String
			Get
				Return sClassName
			End Get
			Set(ByVal Value As String)
				sClassName = Value
			End Set
		End Property

		Public Property ColumnName() As String
			Get
				Return sColumnName
			End Get
			Set(ByVal Value As String)
				sColumnName = Value
			End Set
		End Property

		Public Property Condition() As String
			Get
				Return sCondition
			End Get
			Set(ByVal Value As String)
				sCondition = Value
			End Set
		End Property

		Public Property ImageColumn() As String
			Get
				Return sImageColumn
			End Get
			Set(ByVal Value As String)
				sImageColumn = Value
			End Set
		End Property

		Public Property Image() As String
			Get
				Return sImage
			End Get
			Set(ByVal Value As String)
				sImage = Value
			End Set
		End Property

		Public Property TooltipColumn() As String
			Get
				Return sTooltipColumn
			End Get
			Set(ByVal Value As String)
				sTooltipColumn = Value
			End Set
		End Property

		Public Property Tooltip() As String
			Get
				Return sTooltip
			End Get
			Set(ByVal Value As String)
				sTooltip = Value
			End Set
		End Property

		Public Property PointCell() As Boolean
			Get
				Return bPointCell
			End Get
			Set(ByVal Value As Boolean)
				bPointCell = Value
			End Set
		End Property

		Public Property HideCheckbox() As Boolean
			Get
				Return bHideCheckbox
			End Get
			Set(ByVal value As Boolean)
				bHideCheckbox = value
			End Set
		End Property

		Public Property DisableCheckbox() As Boolean
			Get
				Return bDisableCheckbox
			End Get
			Set(ByVal value As Boolean)
				bDisableCheckbox = value
			End Set
		End Property

#End Region

#Region "Constructors"

		Public Sub New(ByVal ClassName As String, ByVal ColumnName As String,
		 ByVal Condition As String, Optional ByVal ImageColumn As String = "",
		 Optional ByVal Image As String = "", Optional ByVal TooltipColumn As String = "",
		 Optional ByVal Tooltip As String = "", Optional ByVal PointCell As Boolean = False,
		 Optional ByVal HideCheckbox As Boolean = False, Optional ByVal DisableCheckbox As Boolean = False)
			sClassName = ClassName
			sColumnName = ColumnName
			sCondition = Condition
			sImageColumn = ImageColumn
			sImage = Image
			sTooltipColumn = TooltipColumn
			sTooltip = Tooltip
			bPointCell = PointCell
			bHideCheckbox = HideCheckbox
			bDisableCheckbox = DisableCheckbox
		End Sub

		Public Sub New()
			MyBase.New()
		End Sub

#End Region

	End Class

#End Region

#Region "Hidden Information Items"

    <Serializable()>
    Public Class HiddenInformationItems
        Inherits Generic.List(Of HiddenInformationItem)

        'get view state from style overrides
        Public Function GetViewState() As String

            Dim oSB As New StringBuilder

            For Each oHiddenInfo As HiddenInformationItem In Me
                oSB.Append(String.Join("|", oHiddenInfo.Name, oHiddenInfo.DataViewColumnName))
                oSB.Append("^")
            Next

            If oSB.Length > 0 Then
                oSB.Length = oSB.Length - 1
            End If

            Return oSB.ToString

        End Function

        'restore columns from viewstate
        Public Sub RestoreFromViewState(ByVal sString As String)

            Dim aHiddenInformationItems() As String = sString.Split("^".ToCharArray)

            For iStyleOverride As Integer = 0 To aHiddenInformationItems.Length - 1

                Dim sHiddenInformation As String = aHiddenInformationItems(iStyleOverride)
                Dim aProperties() As String = sHiddenInformation.Split("|".ToCharArray)
                Dim oHiddenInformationItem As New HiddenInformationItem

                If aProperties.Length > 1 Then
                    With oHiddenInformationItem
                        .Name = aProperties(0)
                        .DataViewColumnName = aProperties(1)
                    End With

                    Me.Add(oHiddenInformationItem)
                End If
            Next

        End Sub

    End Class


    <Serializable()>
    Public Class HiddenInformationItem

#Region "Variables"

        Private sName As String = ""
        Private sDataViewColumnName As String = ""

#End Region

#Region "Properties"

        Public Property Name() As String
            Get
                Return sName
            End Get
            Set(ByVal value As String)
                sName = value
            End Set
        End Property

        Public Property DataViewColumnName() As String
            Get
                Return sDataViewColumnName
            End Get
            Set(ByVal value As String)
                sDataViewColumnName = value
            End Set
        End Property

#End Region

#Region "Constructors"

        Public Sub New(ByVal Name As String, ByVal DataViewColumnName As String)
            sName = Name
            sDataViewColumnName = DataViewColumnName
        End Sub

        Public Sub New(ByVal DataViewColumnName As String)
            sName = DataViewColumnName
            sDataViewColumnName = DataViewColumnName
        End Sub

        Public Sub New()

        End Sub

#End Region

    End Class



#End Region

End Namespace