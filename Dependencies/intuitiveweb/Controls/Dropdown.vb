Namespace Controls

	Public Class Dropdown
		Inherits Intuitive.WebControls.DropDown

		Public Property Lookup As Lookups.LookupTypes = Lookups.LookupTypes.None
		Public Property IncludeParent As Boolean = False
		Public Property AddBlankOverride As Boolean = True

		Public Property KeyValuePairs As New Lookups.KeyValuePairs



		Protected Overrides Sub Render(writer As System.Web.UI.HtmlTextWriter)
			Me.AutoFilter = False
			Me.AddBlank = Me.AddBlankOverride

			If Me.Lookup <> Lookups.LookupTypes.None AndAlso Me.IncludeParent Then
				Me.ProcessKeyValuePairsWithParent(Me.Lookup)
				Me.AutoGroupType = "standard"
			ElseIf Me.Lookup <> Lookups.LookupTypes.None Then
				Me.ProcessKeyValuePairs(Me.Lookup)
			ElseIf Me.KeyValuePairs.Count > 0 Then
				Me.ProcessKeyValuePairs(Me.KeyValuePairs)
			End If

			MyBase.Render(writer)
		End Sub


		Public Sub ProcessKeyValuePairs(ByVal Lookup As Lookups.LookupTypes)

			'TODO - add cache + synclock

			'1. get pairs
			Dim oKeyValuePairs As Lookups.KeyValuePairs = BookingBase.Lookups.GenerateKeyValuePairs(Lookup)
			Me.ProcessKeyValuePairs(oKeyValuePairs)

		End Sub


		Public Sub ProcessKeyValuePairs(ByVal KeyValuePairs As Lookups.KeyValuePairs)

			'2. scan through and build list
			Dim sb As New System.Text.StringBuilder
			For Each oPair As Generic.KeyValuePair(Of Integer, String) In KeyValuePairs
				sb.Append(oPair.Value).Append("|").Append(oPair.Key).Append("#")
			Next

			'3. set options
			Me.Options = Intuitive.Functions.Chop(sb.ToString)

		End Sub


		Public Sub ProcessKeyValuePairsAlphabetically(ByVal KeyValuePairs As Lookups.KeyValuePairs)

			'1. scan through and build list
			Dim sb As New System.Text.StringBuilder
			For Each oPair As Generic.KeyValuePair(Of Integer, String) In KeyValuePairs.OrderBy(Function(o) o.Value)
				sb.Append(oPair.Value).Append("|").Append(oPair.Key).Append("#")
			Next

			'2. set options
			Me.Options = Intuitive.Functions.Chop(sb.ToString)

		End Sub


		Public Sub ProcessKeyValuePairsWithParent(ByVal Lookup As Lookups.LookupTypes)

			'TODO - add cache + synclock

			'1. get pairs
			Dim oKeyValuePairsWithParent As Lookups.KeyValuePairsWithParent = BookingBase.Lookups.GenerateKeyValuePairsWithParent(Lookup)
			Me.ProcessKeyValuePairsWithParent(oKeyValuePairsWithParent)

		End Sub


		Public Sub ProcessKeyValuePairsWithParent(ByVal KeyValuePairsWithParent As Lookups.KeyValuePairsWithParent)


			'2. scan through and build list
			Dim sb As New System.Text.StringBuilder
			For Each oPair As Lookups.KeyValuePairWithParent In KeyValuePairsWithParent
				sb.Append(oPair.Parent).Append("~").Append(oPair.Value).Append("|").Append(oPair.ID).Append("#")
			Next

			'3. set options
			Me.Options = Intuitive.Functions.Chop(sb.ToString)

		End Sub

		Public Sub AddBlankOption(ByVal BlankOptionText As String)

			'Set the option to be appended
			Dim sBlankOption As String = BlankOptionText & "|0"

			'Add values for blank selections to allow validation to be performed correctly
			Me.AddBlankOverride = False

			'If for whatever reason we don't have anything in our DDL just add the blank option
			If Me.Options = Nothing Then
				Me.Options = sBlankOption
			Else
				Me.Options = sBlankOption & "#" & Me.Options
			End If

		End Sub

	End Class



End Namespace
