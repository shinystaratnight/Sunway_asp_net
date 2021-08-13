Imports ivci = iVectorConnectInterface
Imports System.Xml
Imports System.Xml.Serialization
Imports Intuitive.Functions
Imports iVectorConnectInterface.Extra

Public Class TwoStepExtraResultsHandler

	Public AvailabilityConnectResults As New Dictionary(Of String, Generic.List(Of ivci.Extra.AvailabilityResponse.Extra))
	Public OptionConnectResults As New Dictionary(Of String, Generic.List(Of ivci.Extra.OptionsResponse.Option))

	Public WorkTable As New Dictionary(Of String, Generic.List(Of WorkTableItem))
	Public OptionWorkTable As New Dictionary(Of String, List(Of OptionWorkTableItem))

	Public ExtraSearch As New Dictionary(Of String, BookingExtra.ExtraSearch)

    Public ResultsFilter As New Dictionary(Of String, Filters)
	Public ResultsSort As New Dictionary(Of String, Sort)

	Public MarkupAmount As Decimal
	Public MarkupPercentage As Decimal

#Region "Save"

	'Save
	Public Sub SaveAvailability(sIdentifier As String,
					iVectorConnectResults As Generic.List(Of ivci.Extra.AvailabilityResponse.ExtraType),
					Optional ByVal bAppend As Boolean = False,
					Optional ByVal oExtraSearch As BookingExtra.ExtraSearch = Nothing)

		'If we're not appending the results we want to clear down
		If Not bAppend Then
			'1. Clear Work Table and IVC results
			Me.ClearWorkTable(sIdentifier)
		End If

		'Save the search
		Me.ExtraSearch(sIdentifier) = oExtraSearch

		'2. populate the work table with one work item per Extra
		Dim iIndex As Integer = 0
		If Not Me.WorkTable.ContainsKey(sIdentifier) Then
			Me.WorkTable.Add(sIdentifier, New List(Of WorkTableItem))
		ElseIf Me.WorkTable(sIdentifier).Count > 0 Then
			iIndex = Me.WorkTable(sIdentifier).Max(Function(o) o.Index)
		End If


		For Each oExtraType As ivci.Extra.AvailabilityResponse.ExtraType In iVectorConnectResults

			For Each oExtraSubType As ivci.Extra.AvailabilityResponse.ExtraSubType In oExtraType.ExtraSubTypes

				For Each oExtra As ivci.Extra.AvailabilityResponse.Extra In oExtraSubType.Extras
					ProcessAvailability(sIdentifier, oExtraType, oExtraSubType, oExtra, iIndex)
					iIndex += 1
				Next

			Next

		Next
	End Sub

	Public Sub SaveOption(ByVal sIdentifier As String, ByVal iVectorConnectResult As List(Of ivci.Extra.OptionsResponse.Option))

        iVectorConnectResult = iVectorConnectResult.OrderBy(function(o) o.TotalPrice).ThenBy(Function(o) o.ExtraCategory).ToList()

		Me.OptionConnectResults(sIdentifier) = iVectorConnectResult

		'1. populate the work table with one work item per Extra
		Dim iIndex As Integer = 0
		If Not Me.OptionWorkTable.ContainsKey(sIdentifier) Then
			Me.OptionWorkTable.Add(sIdentifier, New List(Of OptionWorkTableItem))
		ElseIf Me.OptionWorkTable(sIdentifier).Count > 0 Then
			iIndex = Me.OptionWorkTable(sIdentifier).Max(Function(o) o.Index)
		End If

		For Each oOption As OptionsResponse.Option In iVectorConnectResult

			ProcessOption(sIdentifier, oOption, iIndex)
			iIndex += 1

		Next


	End Sub

	Private Sub ProcessOption(sIdentifier As String, oOption As OptionsResponse.Option, iIndex As Integer)

		Dim oItem As New OptionWorkTableItem(iIndex, oOption.BookingToken)
		Me.OptionWorkTable(sIdentifier).Add(oItem)

	End Sub

	Private Sub ProcessAvailability(sIdentifier As String, oExtraType As AvailabilityResponse.ExtraType, oExtraSubType As AvailabilityResponse.ExtraSubType, oExtra As AvailabilityResponse.Extra, iIndex As Integer)

		'1. store results as class Extra
		Me.AvailabilityConnectResults(sIdentifier).Add(oExtra)

		'2. Create ResultIndex and set index
		Dim oItem As New WorkTableItem(iIndex, oExtraType.ExtraTypeID, oExtra.UseDates)

		'3. Add ResultIndex to WorkTable
		Me.WorkTable(sIdentifier).Add(oItem)

	End Sub

#End Region

#Region "Get Extras"
	'Get Results XML
	Public Function GetExtrasXml(ByVal identifier As String, ByVal page As Integer) As XmlDocument
        
		'1. Get a list of all options and availability.
		Dim availability As List(Of Availability) = GetAvailabilities(identifier, page)
		Dim options As List(Of Extra) = GetOptions(identifier, page)

        '2. serialize the above lists into xml
		Dim oAvailabilityXml As XmlDocument = Serializer.Serialize(availability, True)
		Dim oOptionXml As XmlDocument = Serializer.Serialize(options, True)

		'3. merge it all together.
		Dim oXml As XmlDocument = XMLFunctions.MergeXMLDocuments("TwoStepExtras", oAvailabilityXml, oOptionXml)

		'4. Return XML
		Return oXml

	End Function


	Public Function GetOptions(ByVal Identifier As String, ByVal Page As Integer) As Generic.List(Of Extra)
		
        '1. Get oIVectorConnectResultsIndexs
		Dim oIVectorConnectResultsIndexs As Generic.List(Of IVectorConnectResultsIndex) = Me.GetOptionRange(Identifier, Page)


		Dim oIVectorConnectResults As New Generic.List(Of Extra)

		'2. loop through the indexs and build up the extras
		For Each oIVectorConnectResultsIndex As IVectorConnectResultsIndex In oIVectorConnectResultsIndexs
			oIVectorConnectResults.Add(GenerateOption(oIVectorConnectResultsIndex.OptionResult, oIVectorConnectResultsIndex.Index, Identifier))
		Next

		'3. Return the results
		Return oIVectorConnectResults

	End Function


	Public Function GetAvailabilities(ByVal Identifier As String, ByVal Page As Integer) As Generic.List(Of Availability)

		'1. Get oIVectorConnectResultsIndexs
		Dim oIVectorConnectResultsIndexs As Generic.List(Of IVectorConnectResultsIndex) = Me.GetAvailabilityRange(Identifier, Page)


		Dim oIVectorConnectResults As New Generic.List(Of Availability)

		'2. loop through the indexs and build up the extras
		For Each oIVectorConnectResultsIndex As IVectorConnectResultsIndex In oIVectorConnectResultsIndexs
			oIVectorConnectResults.Add(GenerateAvailability(oIVectorConnectResultsIndex.AvailabilityResult, oIVectorConnectResultsIndex.Index, Identifier))
		Next

		'3. Return the results
		Return oIVectorConnectResults

	End Function

	Public Function GetOptionRange(ByVal Identifier As String, ByVal Page As Integer) As Generic.List(Of IVectorConnectResultsIndex)

		'1. list of ivectorconnectresultindex
		Dim oConnectResultsIndexes As New Generic.List(Of IVectorConnectResultsIndex)

		'2. get list of worktableitems
		Dim oWorktable As Generic.List(Of OptionWorkTableItem) = Me.GetOptionWorkTable(Identifier, Page)

		'3. add Extraresult to oivectorconnectresultsindex from ivectorconnectresults using index
		For Each oitem As OptionWorkTableItem In oWorktable
			Dim oConnectResultsIndex As New IVectorConnectResultsIndex
			oConnectResultsIndex.Index = oitem.Index
			oConnectResultsIndex.OptionResult = (Me.OptionConnectResults(Identifier)(oitem.Index))
			oConnectResultsIndexes.Add(oConnectResultsIndex)
		Next

		'4. return
		Return oConnectResultsIndexes

	End Function

	Public Function GetAvailabilityRange(ByVal Identifier As String, ByVal Page As Integer) As Generic.List(Of IVectorConnectResultsIndex)

		'1. list of ivectorconnectresultindex
		Dim oConnectResultsIndexes As New Generic.List(Of IVectorConnectResultsIndex)

		'2. get list of worktableitems
		Dim oWorktable As Generic.List(Of WorkTableItem) = Me.GetWorkTable(Identifier, Page)

		'3. add Extraresult to oivectorconnectresultsindex from ivectorconnectresults using index
		For Each oItem As WorkTableItem In oWorktable
			Dim oConnectResultsIndex As New IVectorConnectResultsIndex
			oConnectResultsIndex.Index = oItem.Index
			oConnectResultsIndex.AvailabilityResult = (Me.AvailabilityConnectResults(Identifier)(oitem.Index))
			oConnectResultsIndexes.Add(oConnectResultsIndex)
		Next

		'4. return
		Return oConnectResultsIndexes

	End Function

	'Generate Extra
	Public Function GenerateOption(ByVal oOptionResult As ivci.Extra.OptionsResponse.Option, ByVal iIndex As Integer, ByVal sIdentifier As String, Optional ByVal sBookingToken As String = "") As Extra

		'1. Create new Extra result object
		Dim oExtra As New Extra
		With oExtra
			.ExtraID = Me.ExtraSearch(sIdentifier).ExtraID
			.ExtraName = Me.AvailabilityConnectResults(sIdentifier)(0).ExtraName
			.Index = iIndex
			.AdultPrice = oOptionResult.AdultPrice
			.ChildPrice = oOptionResult.ChildPrice
			.InfantPrice = oOptionResult.InfantPrice
			.SeniorPrice = oOptionResult.SeniorPrice
			.TotalPrice = oOptionResult.TotalPrice
			.BookingToken = oOptionResult.BookingToken
			.MinChildAge = oOptionResult.MinChildAge
			.MaxChildAge = oOptionResult.MaxChildAge
			.DateRequired = oOptionResult.DateRequired
			.Description = oOptionResult.Description
			.EndDate = oOptionResult.EndDate
			.EndTime = oOptionResult.EndTime
			.ExtraCategory = oOptionResult.ExtraCategory
			.Notes = oOptionResult.Notes
			.PricingType = oOptionResult.PricingType
			.SeniorAge = oOptionResult.SeniorAge
			.SupplierID = oOptionResult.SupplierID
			.TimeRequired = oOptionResult.TimeRequired
			.StartDate = oOptionResult.UseDate
			.StartTime = oOptionResult.UseTime
		End With

		'8. Return Extra
		Return oExtra

	End Function

	'Generate Extra
	Public Function GenerateAvailability(ByVal oExtraResult As ivci.Extra.AvailabilityResponse.Extra, ByVal iIndex As Integer, ByVal sIdentifier As String, Optional ByVal sBookingToken As String = "") As Availability

		Dim oWorkTableItem As WorkTableItem = Me.WorkTable(sIdentifier)(iIndex)

		'1. Create new Extra result object
		Dim oAvailability As New Availability
		With oAvailability
			.ExtraID = oExtraResult.ExtraID
			.ExtraName = oExtraResult.ExtraName
			.Index = iIndex
			.ExtraTypeID = oWorkTableItem.ExtraTypeID
			.ExtraType = BookingBase.Lookups.NameLookup(Lookups.LookupTypes.ExtraType, "ExtraType", "ExtraTypeID", oWorkTableItem.ExtraTypeID)
			.UseDates = oExtraResult.UseDates
		End With

		'8. Return Extra
		Return oAvailability

	End Function
    
	'Get Single Extra
	Public Function GetSingleExtra(ByVal sIdentifier As String, ByVal sBookingToken As String, Optional ByVal iIndex As Integer = 0) As Extra

		Dim oBasketExtra As New Extra

		Try
			If iIndex = 0 Then
				For Each oItem As OptionWorkTableItem In Me.OptionWorkTable(sIdentifier)


					If oItem.BookingToken = sBookingToken Then
						Dim oExtra As ivci.Extra.OptionsResponse.Option = Me.OptionConnectResults(sIdentifier)(oItem.Index)
						oBasketExtra = Me.GenerateOption(oExtra, oItem.Index, sIdentifier, sBookingToken)
						Exit For
					End If
				Next

			Else
				Dim oWorkTableItem As OptionWorkTableItem = Me.OptionWorkTable(sIdentifier)(iIndex)
				Dim oExtra As ivci.Extra.OptionsResponse.Option = Me.OptionConnectResults(sIdentifier)(oWorkTableItem.Index)
				oBasketExtra = Me.GenerateOption(oExtra, oWorkTableItem.Index, sIdentifier, sBookingToken)
			End If
		Catch ex As Exception
			'in case indexes do not exist
		End Try

		Return oBasketExtra

	End Function

	Public Function ExtraHashToken(ByVal Identifier As String, ByVal Index As Integer, ByVal BookingToken As String, Optional ByVal OverrideAdults As Integer = -1,
	Optional ByVal OverrideChildren As Integer = -1, Optional ByVal OverrideInfants As Integer = -1, Optional ByVal OverrideChildAges As Generic.List(Of Integer) = Nothing,
	Optional ByVal OverrideAdultAges As Generic.List(Of Integer) = Nothing) As String

		Dim oExtraSearch As BookingExtra.ExtraSearch = Me.ExtraSearch(Identifier)

		Dim oExtraResult As ivci.Extra.OptionsResponse.Option = Me.OptionConnectResults(Identifier)(Index)

		Dim availabilityResult As ivci.Extra.AvailabilityResponse.Extra = Me.AvailabilityConnectResults(Identifier).FirstOrDefault(Function(o) o.ExtraID = oExtraSearch.ExtraID)

		Dim oBasketExtraOption As New BookingExtra.BasketExtra.BasketExtraOption

		Dim oWorkTableExtra As WorkTableItem = Me.WorkTable(Identifier)(0)
		
		With oBasketExtraOption

			.BookingToken = oExtraResult.BookingToken
			.Adults = IIf((oExtraResult.AdultPrice <> 0 OrElse oExtraResult.TotalPrice = 0), IIf(OverrideAdults > -1, OverrideAdults, oExtraSearch.Adults), 1) 'Check if adults need specifying, then check if we're using an override
			.Children = IIf((oExtraResult.ChildPrice <> 0 OrElse oExtraResult.TotalPrice = 0), IIf(OverrideChildren > -1, OverrideChildren, oExtraSearch.Children), 0)
			.Infants = IIf((oExtraResult.InfantPrice <> 0 OrElse oExtraResult.TotalPrice = 0), IIf(OverrideInfants > -1, OverrideInfants, oExtraSearch.Infants), 0)

			.AdultAges = IIf(OverrideAdultAges IsNot Nothing, OverrideAdultAges, oExtraSearch.AdultAges)
			.ChildAges = IIf(OverrideChildAges IsNot Nothing, OverrideChildAges, oExtraSearch.ChildAges)

			.SeniorAge = oExtraResult.SeniorAge

			.MinChildAge = oExtraResult.MinChildAge
			.MaxChildAge = oExtraResult.MaxChildAge

			.AdultPrice = oExtraResult.AdultPrice
			.ChildPrice = oExtraResult.ChildPrice
			.InfantPrice = oExtraResult.InfantPrice
			.TotalPrice = oExtraResult.TotalPrice
			.SeniorPrice = oExtraResult.SeniorPrice

			.ExtraTypeID = oWorkTableExtra.ExtraTypeID
			.ExtraType = BookingBase.Lookups.NameLookup(Lookups.LookupTypes.ExtraType, "ExtraType", "ExtraTypeID", oWorkTableExtra.ExtraTypeID)
			.ExtraID = oExtraSearch.ExtraID
			.ExtraName = availabilityResult.ExtraName
			.ExtraCategory = oExtraResult.ExtraCategory
			.Notes = oExtraResult.Notes
			.Duration = oExtraResult.Duration

			.DateRequired = oExtraResult.DateRequired
			.StartDate = oExtraResult.UseDate
			.EndDate = oExtraResult.EndDate

			.SupplierID = oExtraResult.SupplierID

			.TimeRequired = oExtraResult.TimeRequired
			If oExtraResult.TimeRequired Then
				.EndTime = IIf(oExtraResult.EndTime <> "", oExtraResult.EndTime, "11:00")
				.StartTime = IIf(oExtraResult.UseTime <> "", oExtraResult.UseTime, "11:00")
			End If

			'Get Extra Markups
			Dim aExtraMarkups As Generic.List(Of BookingBase.Markup) = BookingBase.Markups.Where(Function(o) o.Component = BookingBase.Markup.eComponentType.Extra AndAlso Not o.Value = 0).ToList 'evaluate here so we're not evaluating every time in the loop

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

		Return oBasketExtraOption.HashToken

	End Function
 #End Region

#Region "Work Tables"
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

	'Get Work Table Item - Get Range of work table items using Page to generate a range
	Public Function GetOptionWorkTable(ByVal Identifier As String, ByVal Page As Integer) As Generic.List(Of OptionWorkTableItem)

		Dim oWorkTable As New Generic.List(Of OptionWorkTableItem)

		Try

			'1. Select WorkItems where Display = true
			oWorkTable = Me.OptionWorkTable(Identifier).Where(Function(o) o.Display = True).ToList

			'2. Set range indexes, hard coded for now, but if we have a widget that wants to use pagination we'll need to set these here.
			Dim iStartIndex As Integer = 0
			Dim iCount As Integer = oWorkTable.Count

			'3. Select WorkTableItems within range
			oWorkTable = oWorkTable.GetRange(iStartIndex, iCount)

		Catch ex As Exception
			oWorkTable = New Generic.List(Of OptionWorkTableItem)
		End Try

		'3. Return WorkTable
		Return oWorkTable

	End Function

	Public Sub ClearWorkTable()
		Me.AvailabilityConnectResults.Clear()
	End Sub

	'Clear Work Table
	Public Sub ClearWorkTable(ByVal sIdentifier As String)

		If Me.AvailabilityConnectResults.ContainsKey(sIdentifier) Then
			Me.AvailabilityConnectResults(sIdentifier).Clear()
		Else
			Me.AvailabilityConnectResults.Add(sIdentifier, New Generic.List(Of ivci.Extra.AvailabilityResponse.Extra))
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
	End Sub
#End Region

#Region "Helper Classes"   
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
		Public Function SortXPath(sortBy As eSortBy) As String

			Select Case sortBy
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
		Public AvailabilityResult As ivci.Extra.AvailabilityResponse.Extra
		Public OptionResult As ivci.Extra.OptionsResponse.Option
	End Class

	Public Class WorkTableItem

		Public Index As Integer
		Public ExtraTypeID As Integer
		Public BookingToken As String 'Used as unique identifier
		Public Display As Boolean = True
		Public UseDates As New List(Of AvailabilityResponse.UseDate)

		Public Sub New(index As Integer, extraTypeId As Integer, useDates As List(Of AvailabilityResponse.UseDate))
			Me.Index = index
			Me.ExtraTypeID = extraTypeId
			Me.UseDates = useDates
		End Sub

	End Class

	Public Class OptionWorkTableItem
		Public Index As Integer
		Public BookingToken As String
		Public Display As Boolean = True

		Public Sub New(index As Integer, bookingToken As String)
			Me.Index = index
			Me.BookingToken = bookingToken
		End Sub

	End Class

	Public Class Availability
		Public ExtraID As Integer
		Public ExtraName As String
		Public UseDates As List(Of AvailabilityResponse.UseDate)
		Public Index As Integer
		Public ExtraTypeID As Integer
		Public ExtraType As String
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

		Public ExtraCategoryGroup As String

		Public ExtraCategoryID As Integer
		Public ExtraCategory As String

		Public BookingToken As String
		Public BookingTokenKey As String
		Public StartDate As Date
		Public StartTime As String
		Public EndTime As String
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
