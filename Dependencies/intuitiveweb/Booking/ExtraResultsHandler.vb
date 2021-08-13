Imports ivci = iVectorConnectInterface
Imports System.Xml
Imports System.Xml.Serialization
Imports Intuitive.Functions
Imports iVectorConnectInterface.Extra

Public Class ExtraResultHandler

#Region "Extras"

	Public iVectorConnectResults As New Dictionary(Of String, Generic.List(Of ivci.Extra.SearchResponse.Extra))
	Public FulliVectorConnectResults As New Generic.List(Of ivci.Extra.SearchResponse.ExtraType) ' does this need an identifier dictionary like above?

	Public WorkTable As New Dictionary(Of String, Generic.List(Of WorkTableItem))
	Public ResultsFilter As New Dictionary(Of String, Filters)
	Public ResultsSort As New Dictionary(Of String, Sort)

	Public ExtraSearch As New BookingExtra.ExtraSearch

	Public MarkupAmount As Decimal
	Public MarkupPercentage As Decimal

	Public BookingTokens As New Dictionary(Of String, String)

#End Region


#Region "Save"

	'Save
	Public Sub Save(ByVal sIdentifier As String,
					ByVal iVectorConnectResults As Generic.List(Of ivci.Extra.SearchResponse.ExtraType),
					Optional ByVal bAppend As Boolean = False,
					Optional ByVal oExtraSearch As BookingExtra.ExtraSearch = Nothing)

		'If we're not appending the results we want to clear down
		If Not bAppend Then
			'1. Clear Work Table and IVC results
			Me.ClearWorkTable(sIdentifier)
		End If

		'Reset the markup values, and then set it up based on the search
		SetupExtraMarkup(oExtraSearch)

		'Save the search
		Me.ExtraSearch = oExtraSearch
		Me.FulliVectorConnectResults = iVectorConnectResults


		'2. populate the work table with one work item per Extra
		Dim iIndex As Integer = 0
		If Not Me.WorkTable.ContainsKey(sIdentifier) Then
			Me.WorkTable.Add(sIdentifier, New List(Of WorkTableItem))
		ElseIf Me.WorkTable(sIdentifier).Count > 0 Then
			iIndex = Me.WorkTable(sIdentifier).Max(Function(o) o.Index)
		End If


		For Each oExtraType As ivci.Extra.SearchResponse.ExtraType In iVectorConnectResults

			For Each oExtraSubType As ivci.Extra.SearchResponse.ExtraSubType In oExtraType.ExtraSubTypes

				For Each oExtra As ivci.Extra.SearchResponse.Extra In oExtraSubType.Extras
					ProcessExtra(sIdentifier, oExtraType, oExtraSubType, oExtra, iIndex)
					iIndex += 1
				Next

			Next

		Next

	End Sub

	Private Sub ProcessExtra(sIdentifier As String, oExtraType As SearchResponse.ExtraType, oExtraSubType As SearchResponse.ExtraSubType, oExtra As SearchResponse.Extra, iIndex As Integer)

					'add mark up to total
					For Each oOption As iVectorConnectInterface.Extra.SearchResponse.Option In oExtra.Options

						Dim sExtraKey As String = Intuitive.Functions.SafeString(oExtraType.ExtraTypeID &
																				 "-" &
																				 oExtraType.ExtraSubTypes.IndexOf(oExtraSubType) &
																				  "-" &
																				   oExtraSubType.Extras.IndexOf(oExtra) &
																				   "-" &
																				   oExtra.Options.IndexOf(oOption))


						If Me.BookingTokens.ContainsKey(sExtraKey) Then
							Me.BookingTokens(sExtraKey) = oOption.BookingToken
						Else
							Me.BookingTokens.Add(sExtraKey, oOption.BookingToken)
						End If

						oOption.TotalPrice += Me.MarkupAmount
						oOption.TotalPrice *= (Me.MarkupPercentage / 100) + 1
					Next

					'3. store results as class Extra
					Me.iVectorConnectResults(sIdentifier).Add(oExtra)

					'3b
					Dim oOptionTokens As New Generic.List(Of String)
					If oExtra.Options.Count > 1 Then

						For Each oOption As ivci.Extra.SearchResponse.Option In oExtra.Options
							oOptionTokens.Add(oOption.BookingToken)
						Next

					End If

					'4a. Create ResultIndex and set index
					Dim oItem As New WorkTableItem(iIndex, oExtraType.ExtraTypeID, oExtra.Options(0).BookingToken, oOptionTokens)

					'4b. Add ResultIndex to WorkTable
					Me.WorkTable(sIdentifier).Add(oItem)

	End Sub

	Private Sub SetupExtraMarkup(oExtraSearch As BookingExtra.ExtraSearch)

		'Get Extra Markups
		Dim aExtraMarkups As List(Of BookingBase.Markup) = Me.GetExtraMarkups(oExtraSearch)

		'reset the markup
		Me.MarkupAmount = 0
		Me.MarkupPercentage = 0

		'update with new markup
		For Each oMarkup As BookingBase.Markup In aExtraMarkups
			Select Case oMarkup.Type
				Case BookingBase.Markup.eType.Amount
					Me.MarkupAmount += oMarkup.Value
				Case BookingBase.Markup.eType.AmountPP
					Me.MarkupAmount += oMarkup.Value * (BookingBase.SearchDetails.TotalAdults + BookingBase.SearchDetails.TotalChildren)
				Case BookingBase.Markup.eType.Percentage
					Me.MarkupPercentage = oMarkup.Value
			End Select
		Next
	End Sub

#End Region


#Region "Get"

	'Get Single Extra
	Public Function GetSingleExtra(ByVal sIdentifier As String, ByVal sBookingToken As String, Optional ByVal iIndex As Integer = 0) As Extra

		Dim oBasketExtra As New Extra

		Try
			If iIndex = 0 Then
				For Each oItem As WorkTableItem In Me.WorkTable(sIdentifier)


					If oItem.BookingToken = sBookingToken Then
						Dim oExtra As ivci.Extra.SearchResponse.Extra = Me.iVectorConnectResults(sIdentifier)(oItem.Index)
						oBasketExtra = Me.GenerateExtra(oExtra, oItem.Index, sIdentifier, sBookingToken)
						Exit For
					End If

					'The Booking token in the work table is set to that of the first option (as we need something for to be a unique identifier)
					'However in extras that use multiple options, we need to loop through all the booking tokens to see if we have a match.
					If oItem.OptionTokens.Count > 1 Then

						For Each sToken As String In oItem.OptionTokens

							If sToken = sBookingToken Then
								Dim oExtra As ivci.Extra.SearchResponse.Extra = Me.iVectorConnectResults(sIdentifier)(oItem.Index)
								oBasketExtra = Me.GenerateExtra(oExtra, oItem.Index, sIdentifier, sBookingToken)
								Exit For
							End If

						Next

					End If

					If oBasketExtra.ExtraName <> Nothing Then
						Exit For
					End If

				Next

			Else
				Dim oWorkTableItem As WorkTableItem = Me.WorkTable(sIdentifier)(iIndex)
				Dim oExtra As ivci.Extra.SearchResponse.Extra = Me.iVectorConnectResults(sIdentifier)(oWorkTableItem.Index)
				oBasketExtra = Me.GenerateExtra(oExtra, oWorkTableItem.Index, sIdentifier, sBookingToken)
			End If
		Catch ex As Exception
			'in case indexes do not exist
		End Try

		Return oBasketExtra

	End Function


	'Generate Extra
	Public Function GenerateExtra(ByVal oExtraResult As ivci.Extra.SearchResponse.Extra, ByVal iIndex As Integer, ByVal sIdentifier As String, Optional ByVal sBookingToken As String = "") As Extra

		Dim oWorkTableItem As WorkTableItem = Me.WorkTable(sIdentifier)(iIndex)

		'1. Create new Extra result object
		Dim oExtra As New Extra
		With oExtra
			.ExtraID = oExtraResult.ExtraID
			.ExtraName = oExtraResult.ExtraName
			.Index = iIndex
			.ExtraTypeID = oWorkTableItem.ExtraTypeID
			.ExtraType = BookingBase.Lookups.NameLookup(Lookups.LookupTypes.ExtraType, "ExtraType", "ExtraTypeID", oWorkTableItem.ExtraTypeID)
			.RecordWeight = oExtraResult.RecordWeight
			.Notes = oExtraResult.Notes
			.ExtraLocations = oExtraResult.ExtraLocations
			.ProductAttributes = oExtraResult.ProductAttributes

			For Each oOption As ivci.Extra.SearchResponse.Option In oExtraResult.Options

				'If the option does not conform to the rules of the filter don't include it
				If Not DateFunctions.IsEmptyDate(Me.ResultsFilter(sIdentifier).StartDate) Then

					If oOption.StartDate <> DateFunctions.SafeDate(DateFunctions.ShortDate(Me.ResultsFilter(sIdentifier).StartDate)) Then
						Continue For
					End If

				End If

				If Not DateFunctions.IsEmptyDate(Me.ResultsFilter(sIdentifier).EndDate) Then

					If oOption.EndDate <> DateFunctions.SafeDate(DateFunctions.ShortDate(Me.ResultsFilter(sIdentifier).EndDate)) Then
						Continue For
					End If

				End If

				If sBookingToken <> "" AndAlso oOption.BookingToken <> sBookingToken Then
					Continue For
				End If

				Dim oExtraOption As New Extra.Option
				With oExtraOption
					.ExtraCategoryID = oOption.ExtraCategoryID


					.ExtraCategoryGroup = Split(oOption.ExtraCategory, "###", , CompareMethod.Text)(0)

					If Split(oOption.ExtraCategory, "###", , CompareMethod.Text).Count > 1 Then
						.ExtraCategory = Split(oOption.ExtraCategory, "###", , CompareMethod.Text)(1)
					Else
						.ExtraCategory = oOption.ExtraCategory
					End If

					.BookingToken = oOption.BookingToken

					If Me.BookingTokens.ContainsValue(oOption.BookingToken) Then
						.BookingTokenKey = Me.BookingTokens.Where(Function(o) o.Value = oOption.BookingToken).FirstOrDefault().Key()
					End If

					.DateRequired = oOption.DateRequired
					.StartDate = oOption.StartDate


					.TimeRequired = oOption.TimeRequired
					If oOption.TimeRequired Then
						.StartTime = oOption.StartTime
					End If

					.EndDate = oOption.EndDate
					.Duration = (oOption.EndDate - oOption.StartDate).TotalDays.ToSafeInt() + 1

					.PricingType = oOption.PricingType
					.AdultPrice = oOption.AdultPrice
					.ChildPrice = oOption.ChildPrice
					.InfantPrice = oOption.InfantPrice
					.TotalPrice = oOption.TotalPrice

					.TimeRequired = oOption.TimeRequired


					.MinChildAge = oOption.MinChildAge
					.MaxChildAge = oOption.MaxChildAge
					.SupplierID = oOption.SupplierID
					.MoreInfoKey = oOption.MoreInfoKey
					.CanBookMultiple = oOption.MultiBook
					.SeniorPrice = oOption.SeniorPrice
					.SeniorAge = oOption.SeniorAge


					If oOption.AttractionDetails IsNot Nothing Then
						.Image = oOption.AttractionDetails.Image
						.ImageThumbnail = oOption.AttractionDetails.ImageThumbnail

						.Description = oOption.AttractionDetails.Description

						For Each oItem As ivci.Extra.SearchResponse.IndividualInfoItem In oOption.AttractionDetails.IndividualInfoItems
							.IndividualInfoItems.Add(oItem)
						Next
					End If

					If oOption.GenericDetails IsNot Nothing Then
						.GenericDetails = oOption.GenericDetails
					End If


				End With

				.Options.Add(oExtraOption)


			Next


		End With


		'8. Return Extra
		Return oExtra

	End Function



	'Get Results XML
	Public Function GetExtrasXML(ByVal Identifier As String, ByVal Page As Integer) As XmlDocument


		'1. Get a list of extras
		Dim oIVectorConnectResults As Generic.List(Of Extra) = GetExtras(Identifier, Page)

		'3. Serialize oIVectorConnectResults into XML
		Dim oXML As XmlDocument = Intuitive.Serializer.Serialize(oIVectorConnectResults, True)

		'4. Return XML
		Return oXML

	End Function

	Public Function GetExtras(ByVal Identifier As String, ByVal Page As Integer) As Generic.List(Of Extra)

		'1. Get oIVectorConnectResultsIndexs
		Dim oIVectorConnectResultsIndexs As Generic.List(Of IVectorConnectResultsIndex) = Me.GetRange(Identifier, Page)


		Dim oIVectorConnectResults As New Generic.List(Of Extra)

		'2. loop through the indexs and build up the extras
		For Each oIVectorConnectResultsIndex As IVectorConnectResultsIndex In oIVectorConnectResultsIndexs
			oIVectorConnectResults.Add(GenerateExtra(oIVectorConnectResultsIndex.ExtraResult, oIVectorConnectResultsIndex.Index, Identifier))
		Next

		'3. Return the results
		Return oIVectorConnectResults

	End Function



	'Get Range
	Public Function GetRange(ByVal Identifier As String, ByVal Page As Integer) As Generic.List(Of IVectorConnectResultsIndex)

		'1. list of ivectorconnectresultindex
		Dim oivectorconnectresultsindexs As New Generic.List(Of IVectorConnectResultsIndex)

		'2. get list of worktableitems
		Dim oWorktable As Generic.List(Of WorkTableItem) = Me.GetWorkTable(Identifier, Page)

		'3. add Extraresult to oivectorconnectresultsindex from ivectorconnectresults using index
		For Each oitem As WorkTableItem In oWorktable
			Dim oivectorconnectresultsindex As New IVectorConnectResultsIndex
			oivectorconnectresultsindex.Index = oitem.Index
			oivectorconnectresultsindex.ExtraResult = (Me.iVectorConnectResults(Identifier)(oitem.Index))
			oivectorconnectresultsindexs.Add(oivectorconnectresultsindex)
		Next

		'4. return
		Return oivectorconnectresultsindexs

	End Function

	'Get Work Table Item - Get Range of work table items using Page to generate a range
	Public Function GetWorkTable(ByVal Identifier As String, ByVal Page As Integer) As Generic.List(Of WorkTableItem)

		Dim oWorkTable As New Generic.List(Of WorkTableItem)

		Try

			'1. Select WorkItems where Display = true
			oWorkTable = Me.WorkTable(Identifier).Where(Function(o) o.Display = True).ToList

			'2. Set range indexes, hard coded for now, but if we have a widget that wants to use pagination we'll need to set these here.
			Dim iStartIndex As Integer = 0
			Dim iCount As Integer = oWorkTable.Count

			'3. Select WorkTableItems within range
			oWorkTable = oWorkTable.GetRange(iStartIndex, iCount)

		Catch ex As Exception
			oWorkTable = New Generic.List(Of WorkTableItem)
		End Try

		'3. Return WorkTable
		Return oWorkTable

	End Function
#End Region


#Region "Sort"

	'Sort Results (string)
	Public Sub SortResults(ByVal Identifier As String, ByVal SortBy As String, ByVal SortOrder As String)

		'get enum values
		Dim eSortBy As eSortBy = Intuitive.Functions.SafeEnum(Of ExtraResultHandler.eSortBy)(SortBy)
		Dim eSortOrder As eSortOrder = Intuitive.Functions.SafeEnum(Of ExtraResultHandler.eSortOrder)(SortOrder)

		'sort results
		Me.SortResults(Identifier, eSortBy, eSortOrder)

	End Sub

	'Sort Results (enum)
	Public Sub SortResults(ByVal Identifier As String, ByVal SortBy As eSortBy, ByVal SortOrder As eSortOrder)

		If Not Me.ResultsSort.ContainsKey(Identifier) Then
			Me.ResultsSort.Add(Identifier, New Sort)
		End If

		With Me.ResultsSort(Identifier)
			.SortBy = SortBy
			.SortOrder = SortOrder
		End With

		Select Case SortBy
			Case eSortBy.ExtraName
				Me.WorkTable(Identifier) = Me.WorkTable(Identifier).OrderBy(Function(o) o.ExtraName).ToList
			Case eSortBy.StartDate
				Me.WorkTable(Identifier) = Me.WorkTable(Identifier).OrderBy(Function(o) o.StartDate).ToList
		End Select

		If SortOrder = eSortOrder.Descending Then Me.WorkTable(Identifier).Reverse()

	End Sub

	Public Function GetSortXML(ByVal Identifier As String) As XmlDocument

		Dim oXML As XmlDocument = Intuitive.Serializer.Serialize(Me.ResultsSort(Identifier))

		Return oXML

	End Function

#End Region


#Region "Filter"

	'Filter Results
	Public Sub FilterResults(ByVal Identifier As String, ByVal Filter As Filters)

		'1. Store filter as class Extra
		If Me.ResultsFilter.ContainsKey(Identifier) Then
			Me.ResultsFilter(Identifier) = Filter
		Else
			Me.ResultsFilter.Add(Identifier, Filter)
		End If

		'2. Set Display value for each work item
		For Each oItem As WorkTableItem In Me.WorkTable(Identifier)

			'2.a Select Extra using Item index
			Dim ivcExtraResult As ivci.Extra.SearchResponse.Extra = Me.iVectorConnectResults(Identifier)(oItem.Index)

			'2.b Filter ExtraResult
			Me.FilterItem(Identifier, ivcExtraResult, oItem)

		Next

	End Sub

	'Filter Item
	Public Sub FilterItem(ByVal sIdentifier As String, ByVal ivcExtraResult As ivci.Extra.SearchResponse.Extra, ByVal oWorkTableItem As WorkTableItem)

		'1. Declare Filter Value Variables
		Dim bDisplay As Boolean = True
		Dim sExtraName As String = ""
		Dim sExtraCategory As String = ""


		'2. Check which Extras should show using filters

		'2.a. extra name
		sExtraName = Functions.IIf(Not oWorkTableItem.ExtraName = "", oWorkTableItem.ExtraName, ivcExtraResult.ExtraName)
		If bDisplay AndAlso Me.ResultsFilter(sIdentifier).ExtraName <> "" Then
			If sExtraName Is Nothing OrElse Not sExtraName.ToLower.Contains(Me.ResultsFilter(sIdentifier).ExtraName.ToLower) Then
				bDisplay = False
			End If
		End If

		'2.b. Category
		sExtraCategory = Functions.IIf(Not oWorkTableItem.ExtraCategory = "", oWorkTableItem.ExtraCategory, ivcExtraResult.Options(0).ExtraCategory)
		If bDisplay AndAlso Me.ResultsFilter(sIdentifier).ExtraCategory <> "" Then
			If sExtraName Is Nothing OrElse Not sExtraName.ToLower.Contains(Me.ResultsFilter(sIdentifier).ExtraCategory.ToLower) Then
				bDisplay = False
			End If
		End If

		'2.c. Start Date/ End Date
		If bDisplay Then
			Dim bShowExtra As Boolean = False
			For Each oExtraOption As ivci.Extra.SearchResponse.Option In ivcExtraResult.Options.OrderBy(Function(o) o.TotalPrice)
				Dim bValidOption As Boolean = True

				If Not DateFunctions.IsEmptyDate(Me.ResultsFilter(sIdentifier).StartDate) Then
					If oExtraOption.StartDate <> DateFunctions.SafeDate(DateFunctions.ShortDate(Me.ResultsFilter(sIdentifier).StartDate)) Then
						bValidOption = False
					End If
				End If

				If Not DateFunctions.IsEmptyDate(Me.ResultsFilter(sIdentifier).EndDate) Then
					If oExtraOption.EndDate <> DateFunctions.SafeDate(DateFunctions.ShortDate(Me.ResultsFilter(sIdentifier).EndDate)) Then
						bValidOption = False
					End If
				End If

				'2.d. If Valid option show extra
				If bValidOption Then
					bShowExtra = True
				End If

			Next

			'2.e. Display extra
			bDisplay = bShowExtra

		End If



		'3. Set Work Table Item Properties
		oWorkTableItem.SetProperties(sExtraName, sExtraCategory, bDisplay)

	End Sub

	'Filter Items
	Public Sub FilterItems(ByVal sIdentifier As String, ByVal WorkTable As Generic.List(Of WorkTableItem))

		For Each oItem As WorkTableItem In WorkTable

			Dim ivcExtraResult As ivci.Extra.SearchResponse.Extra = Me.iVectorConnectResults(sIdentifier)(oItem.Index)

			Me.FilterItem(sIdentifier, ivcExtraResult, oItem)
		Next

	End Sub


#End Region


#Region "Work Table Item"

	Public Class WorkTableItem

		Public Index As Integer
		Public ExtraTypeID As Integer
		Public BookingToken As String 'Used as unique identifier

		Public OptionTokens As Generic.List(Of String)

		Public TotalPrice As Decimal
		Public SortValue As String = ""
		Public Display As Boolean = True
		Public ExtraName As String
		Public ExtraCategory As String
		Public StartDate As Date
		Public EndDate As Date



		Public Sub New(ByVal Index As Integer, ByVal ExtraTypeID As Integer, ByVal BookingToken As String, ByVal OptionTokens As Generic.List(Of String))
			Me.Index = Index
			Me.ExtraTypeID = ExtraTypeID
			Me.BookingToken = BookingToken
			Me.OptionTokens = OptionTokens
		End Sub


		'Set Work Table Item Values
		Public Sub SetProperties(ByVal sExtraName As String, ByVal sExtraCategory As String, ByVal bDisplay As Boolean)

			Me.ExtraName = sExtraName
			Me.ExtraCategory = sExtraCategory
			'Me.StartDate = dStartDate

			Me.Display = bDisplay

		End Sub


	End Class

#End Region


#Region "Extra result"
	Public Class ExtraResult
		Inherits iVectorConnectInterface.Extra.SearchResponse.Extra

		Public MarkupAmount As Decimal
		Public MarkupPercentage As Decimal

	End Class
#End Region


#Region "AddToBasket"


	Public Function ExtraHashToken(ByVal Identifier As String, ByVal Index As Integer) As String

		Dim oWorkTable As WorkTableItem = Me.WorkTable(Identifier)(Index)

		Dim oExtraResult As ivci.Extra.SearchResponse.Extra = Me.iVectorConnectResults(Identifier)(Index)

		Dim oExtraOption As New BookingExtra.BasketExtra.BasketExtraOption

		Dim oExtraSearch As BookingExtra.ExtraSearch = Me.ExtraSearch

		With oExtraOption

			.BookingToken = oExtraResult.Options(0).BookingToken
			.Adults = IIf(oExtraResult.Options(0).AdultPrice <> 0 OrElse oExtraResult.Source.ToLower() = "own", oExtraSearch.Adults, 1)
			.Children = IIf(oExtraResult.Options(0).ChildPrice <> 0 OrElse oExtraResult.Source.ToLower() = "own", oExtraSearch.Children, 0)
			.Infants = IIf(oExtraResult.Options(0).InfantPrice <> 0 OrElse oExtraResult.Source.ToLower() = "own", oExtraSearch.Infants, 0)

			.AdultAges = oExtraSearch.AdultAges
			.ChildAges = oExtraSearch.ChildAges
			.SeniorAge = oExtraResult.Options(0).SeniorAge

			.MinChildAge = oExtraOption.MinChildAge
			.MaxChildAge = oExtraOption.MaxChildAge

			.TotalPrice = oExtraResult.Options(0).TotalPrice
			.AdultPrice = oExtraResult.Options(0).AdultPrice
			.ChildPrice = oExtraResult.Options(0).ChildPrice
			.InfantPrice = oExtraResult.Options(0).InfantPrice
			.SeniorPrice = oExtraResult.Options(0).SeniorPrice

			.ExtraTypeID = oWorkTable.ExtraTypeID
			.ExtraType = BookingBase.Lookups.NameLookup(Lookups.LookupTypes.ExtraType, "ExtraType", "ExtraTypeID", oWorkTable.ExtraTypeID)
			.ExtraID = oExtraResult.ExtraID
			.ExtraName = oExtraResult.ExtraName
			.ExtraCategory = oExtraResult.Options(0).ExtraCategory
			.Notes = oExtraResult.Options(0).Notes
			.Duration = oExtraResult.Options(0).Duration

			.DateRequired = oExtraResult.Options(0).DateRequired
			.StartDate = oExtraResult.Options(0).StartDate
			.EndDate = oExtraResult.Options(0).EndDate

			.SupplierID = oExtraOption.SupplierID

			.TimeRequired = oExtraResult.Options(0).TimeRequired
			If oExtraResult.Options(0).TimeRequired Then
				.EndTime = IIf(oExtraResult.Options(0).EndTime <> "", oExtraResult.Options(0).EndTime, "11:00")
				.StartTime = IIf(oExtraResult.Options(0).StartTime <> "", oExtraResult.Options(0).StartTime, "11:00")
			End If

			'Get Extra Markups
			Dim aExtraMarkups As Generic.List(Of BookingBase.Markup) = Me.GetExtraMarkups(Me.ExtraSearch)

			'Remove the markup
			For Each oMarkup As BookingBase.Markup In aExtraMarkups
				Select Case oMarkup.Type
					Case BookingBase.Markup.eType.Amount
						.TotalPrice -= oMarkup.Value
					Case BookingBase.Markup.eType.AmountPP
						.TotalPrice -= oMarkup.Value * (BookingBase.SearchDetails.TotalAdults + BookingBase.SearchDetails.TotalChildren)
					Case BookingBase.Markup.eType.Percentage
						.TotalPrice /= 1 + (oMarkup.Value / 100)
				End Select

			Next

		End With

		Return oExtraOption.HashToken

	End Function


	Public Function ExtraHashToken(ByVal Identifier As String, ByVal Index As Integer, ByVal BookingToken As String, Optional ByVal OverrideAdults As Integer = -1,
	Optional ByVal OverrideChildren As Integer = -1, Optional ByVal OverrideInfants As Integer = -1, Optional ByVal OverrideChildAges As Generic.List(Of Integer) = Nothing,
	Optional ByVal OverrideAdultAges As Generic.List(Of Integer) = Nothing) As String

		Dim oExtraResult As ivci.Extra.SearchResponse.Extra = Me.iVectorConnectResults(Identifier)(Index)

		Dim oBasketExtraOption As New BookingExtra.BasketExtra.BasketExtraOption

		Dim oWorkTableExtra As WorkTableItem = Me.WorkTable(Identifier)(Index)

		Dim oExtraSearch As BookingExtra.ExtraSearch = Me.ExtraSearch

		For Each oExtraOption As ivci.Extra.SearchResponse.Option In oExtraResult.Options

			If oExtraOption.BookingToken = BookingToken Then

				With oBasketExtraOption

					.BookingToken = oExtraOption.BookingToken

					.Adults = IIf(OverrideAdults > -1, OverrideAdults, oExtraSearch.Adults) 'Check if adults need specifying, then check if we're using an override
					.Children = IIf(OverrideChildren > -1, OverrideChildren, oExtraSearch.Children)
					.Infants = IIf(OverrideInfants > -1, OverrideInfants, oExtraSearch.Infants)

					.AdultAges = IIf(OverrideAdultAges IsNot Nothing, OverrideAdultAges, oExtraSearch.AdultAges)
					.ChildAges = IIf(OverrideChildAges IsNot Nothing, OverrideChildAges, oExtraSearch.ChildAges)

					.SeniorAge = oExtraResult.Options(0).SeniorAge

					.MinChildAge = oExtraOption.MinChildAge
					.MaxChildAge = oExtraOption.MaxChildAge

					.AdultPrice = oExtraOption.AdultPrice
					.ChildPrice = oExtraOption.ChildPrice
					.InfantPrice = oExtraOption.InfantPrice
					.TotalPrice = oExtraOption.TotalPrice
					.SeniorPrice = oExtraResult.Options(0).SeniorPrice

					.ExtraTypeID = oWorkTableExtra.ExtraTypeID
					.ExtraType = BookingBase.Lookups.NameLookup(Lookups.LookupTypes.ExtraType, "ExtraType", "ExtraTypeID", oWorkTableExtra.ExtraTypeID)
					.ExtraID = oExtraResult.ExtraID
					.ExtraName = oExtraResult.ExtraName
					.ExtraCategory = oExtraOption.ExtraCategory
					.Notes = oExtraOption.Notes
					.Duration = oExtraOption.Duration

					.DateRequired = oExtraResult.Options(0).DateRequired
					.StartDate = oExtraOption.StartDate
					.EndDate = oExtraOption.EndDate

					.SupplierID = oExtraOption.SupplierID

					.TimeRequired = oExtraResult.Options(0).TimeRequired
					If oExtraResult.Options(0).TimeRequired Then
						.EndTime = IIf(oExtraOption.EndTime <> "", oExtraOption.EndTime, "11:00")
						.StartTime = IIf(oExtraOption.StartTime <> "", oExtraOption.StartTime, "11:00")
					End If

					'Get Extra Markups
					Dim aExtraMarkups As Generic.List(Of BookingBase.Markup) = Me.GetExtraMarkups(Me.ExtraSearch)

					'Remove the markup
					For Each oMarkup As BookingBase.Markup In aExtraMarkups
						Select Case oMarkup.Type
							Case BookingBase.Markup.eType.Amount
								.TotalPrice -= oMarkup.Value
							Case BookingBase.Markup.eType.AmountPP
								.TotalPrice -= oMarkup.Value * (BookingBase.SearchDetails.TotalAdults + BookingBase.SearchDetails.TotalChildren)
							Case BookingBase.Markup.eType.Percentage
								.TotalPrice /= 1 + (oMarkup.Value / 100)
						End Select

					Next

				End With

			End If

		Next

		Return oBasketExtraOption.HashToken

	End Function

#End Region


#Region "Classes"


	Public Class Filters

		'Filters
		Public ExtraName As String = ""
		Public ExtraCategory As String = ""
		Public StartDate As Date = DateFunctions.EmptyDate
		Public EndDate As Date = DateFunctions.EmptyDate

	End Class


	Public Class Sort

		Public SortBy As eSortBy = eSortBy.ExtraName
		Public SortOrder As eSortOrder = eSortOrder.Ascending

		<XmlIgnore()> Public ExtraNameXPath As String = "Extra/ExtraName"
		<XmlIgnore()> Public StartDateXPath As String = "Extra/Options/Option/StartDate"

		'Sort X Path
		Public Function SortXPath(ByVal SortBy As eSortBy) As String

			Select Case SortBy
				Case eSortBy.ExtraName
					Return Me.ExtraNameXPath
				Case eSortBy.StartDate
					Return Me.StartDateXPath
			End Select

			Return ""

		End Function

	End Class

	Public Class IVectorConnectResultsIndex
		Public Index As Integer
		Public ExtraResult As ivci.Extra.SearchResponse.Extra
	End Class



	Public Class Extra

		Public ExtraID As Integer
		Public ExtraName As String
		Public Index As Integer
		Public ExtraTypeID As Integer
		Public ExtraType As String
		Public RecordWeight As Boolean
		Public Notes As String
		Public ExtraLocations As New ivci.Extra.ExtraLocations
		Public ProductAttributes As List(Of SearchResponse.ProductAttribute)

		Public Options As New Generic.List(Of [Option])



		Public Class [Option]
			Public ExtraCategoryGroup As String

			Public ExtraCategoryID As Integer
			Public ExtraCategory As String

			Public BookingToken As String
			Public BookingTokenKey As String
			Public StartDate As Date
			Public StartTime As String
			Public EndDate As Date
			Public Duration As Integer

			Public PricingType As String
			Public TotalPrice As Decimal
			Public AdultPrice As Decimal
			Public ChildPrice As Decimal
			Public InfantPrice As Decimal
			Public SeniorPrice As Decimal

			Public MinChildAge As Integer
			Public MaxChildAge As Integer

			Public SeniorAge As Integer

			Public TimeRequired As Boolean
			Public DateRequired As Boolean


			Public MoreInfoKey As String
			Public Image As String
			Public ImageThumbnail As String
			Public IndividualInfoItems As New Generic.List(Of ivci.Extra.SearchResponse.IndividualInfoItem)
			<XmlArrayItem("GenericDetail")>
			Public GenericDetails As New Generic.List(Of String)

			Public Description As String
			Public SupplierID As Integer

			Public CanBookMultiple As Boolean = False

		End Class

	End Class


#End Region


#Region "Functions"

	Public Sub ClearWorkTable()
		Me.iVectorConnectResults.Clear()
		Me.BookingTokens.Clear()
	End Sub

	'Clear Work Table
	Public Sub ClearWorkTable(ByVal sIdentifier As String)
		If Me.iVectorConnectResults.ContainsKey(sIdentifier) Then
			Me.iVectorConnectResults(sIdentifier).Clear()
		Else
			Me.iVectorConnectResults.Add(sIdentifier, New Generic.List(Of ivci.Extra.SearchResponse.Extra))
		End If
		If Me.WorkTable.ContainsKey(sIdentifier) Then
			Me.WorkTable(sIdentifier).Clear()
		Else
			Me.WorkTable.Add(sIdentifier, New Generic.List(Of WorkTableItem))
		End If
		If Me.ResultsSort.ContainsKey(sIdentifier) Then
			Me.ResultsSort(sIdentifier) = New Sort
		Else
			Me.ResultsSort.Add(sIdentifier, New Sort)
		End If
		If Me.ResultsFilter.ContainsKey(sIdentifier) Then
			Me.ResultsFilter(sIdentifier) = New Filters
		Else
			Me.ResultsFilter.Add(sIdentifier, New Filters)
		End If
		Me.BookingTokens.Clear()
	End Sub


	Public Function GetExtraMarkups(ByVal oExtraSearch As BookingExtra.ExtraSearch) As List(Of BookingBase.Markup)
		Dim oExtraTypes As List(Of String) = New List(Of String)
		If Not oExtraSearch Is Nothing AndAlso oExtraSearch.ExtraTypeIDs.Count > 0 Then
			For Each iExtraTypeID As Integer In oExtraSearch.ExtraTypeIDs
				Dim sExtraType As String = BookingBase.Lookups.GetKeyPairValue(Lookups.LookupTypes.ExtraType, iExtraTypeID)
				oExtraTypes.Add(sExtraType)
			Next
		End If

		Dim aExtraMarkups As Generic.List(Of BookingBase.Markup) = BookingBase.Markups.Where(Function(o) o.Component = BookingBase.Markup.eComponentType.Extra _
			AndAlso (oExtraTypes.Count = 0 OrElse o.SubComponent = BookingBase.Markup.eSubComponentType.None _
				OrElse oExtraTypes.Contains(o.SubComponent.ToString())) _
			AndAlso Not o.Value = 0).ToList

		Return aExtraMarkups
	End Function

	Public Sub UpdateMarkups()
		Dim aExtraMarkups As List(Of BookingBase.Markup) = Me.GetExtraMarkups(Me.ExtraSearch)

		'update with new markup
		For Each oMarkup As BookingBase.Markup In aExtraMarkups
			Select Case oMarkup.Type
				Case BookingBase.Markup.eType.Amount
					Me.MarkupAmount += oMarkup.Value
				Case BookingBase.Markup.eType.AmountPP
					Me.MarkupAmount += oMarkup.Value * (BookingBase.SearchDetails.TotalAdults + BookingBase.SearchDetails.TotalChildren)
				Case BookingBase.Markup.eType.Percentage
					Me.MarkupPercentage = oMarkup.Value
			End Select
		Next

		For Each oExtraResultSet As List(Of SearchResponse.Extra) In BookingBase.SearchDetails.ExtraResults.iVectorConnectResults.Values
			For Each oExtra As SearchResponse.Extra In oExtraResultSet
				For Each oOption As SearchResponse.Option In oExtra.Options

					'add mark up to total
					oOption.TotalPrice += Me.MarkupAmount
					oOption.TotalPrice *= (Me.MarkupPercentage / 100) + 1
				Next
			Next
		Next

	End Sub

	Public Sub RemoveMarkup()

		'Get Extra Markups
		Dim aExtraMarkups As Generic.List(Of BookingBase.Markup) = Me.GetExtraMarkups(Me.ExtraSearch)

		'Loop through Each Extra type
		For Each sIdentifier As String In Me.iVectorConnectResults.Keys

			'Loop through each extra of that type
			For Each oExtra As iVectorConnectInterface.Extra.SearchResponse.Extra In Me.iVectorConnectResults(sIdentifier)

				'Loop through each option of that extra
				For Each oOption As iVectorConnectInterface.Extra.SearchResponse.Option In oExtra.Options

					'Remove the markup
					For Each oMarkup As BookingBase.Markup In aExtraMarkups
						Select Case oMarkup.Type
							Case BookingBase.Markup.eType.Amount
								oOption.TotalPrice -= oMarkup.Value
							Case BookingBase.Markup.eType.AmountPP
								oOption.TotalPrice -= oMarkup.Value * (BookingBase.SearchDetails.TotalAdults + BookingBase.SearchDetails.TotalChildren)
							Case BookingBase.Markup.eType.Percentage
								oOption.TotalPrice /= 1 + (oMarkup.Value / 100)
						End Select

					Next

				Next
			Next
		Next

	End Sub

#End Region


#Region "Enums"

	Public Enum eSortBy
		ExtraName
		StartDate
	End Enum

	Public Enum eSortOrder
		Ascending
		Descending
	End Enum

#End Region



End Class
