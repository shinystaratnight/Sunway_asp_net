Imports ivci = iVectorConnectInterface
Imports System.Xml
Imports System.Xml.Serialization
Imports Intuitive.Functions

Public Class BookingsHandler

#Region "properties"

	Public iVectorConnectBookings As New Generic.List(Of ivci.SearchBookingsResponse.Booking)
	Public WorkTable As New Generic.List(Of WorkTableItem)
	Public BookingsFilter As New Filters
	Public BookingsSort As New Sort


	Public CurrentPage As Integer = 1

#End Region

#Region "Save"

	'Save
	Public Sub Save(ByVal iVectorConnectBookings As Generic.List(Of ivci.SearchBookingsResponse.Booking))

		'1. Clear Work Table
		Me.ClearWorkTable()


		'2. store results as class property
		Me.iVectorConnectBookings = iVectorConnectBookings

		'3. populate the work table with one work item per property
		Dim iIndex As Integer = 0
		For Each oBooking As ivci.SearchBookingsResponse.Booking In Me.iVectorConnectBookings

			'4a. Create ResultIndex and set index
			Dim oItem As New WorkTableItem(iIndex, oBooking.BookingReference)

			'4b. Add ResultIndex to WorkTable
			Me.WorkTable.Add(oItem)

			iIndex += 1

		Next

        '5a initial Filter
        Me.BookingsFilter.ArrivalStartDate = datetime.Now.AddDays(1)
        Me.FilterResults(Me.BookingsFilter)

		'5. Store Result Handler On SearchBookings
		BookingBase.SearchBookings.Bookings = Me

	End Sub

#End Region

#Region "Get"

	'Get Results XML
	Public Function GetBookingsXML(ByVal Page As Integer) As XmlDocument

		'1. Get oIVectorConnectResultsIndexs
		Dim oIVectorConnectResultsIndexs As Generic.List(Of IVectorConnectResultsIndex) = Me.GetRange(Page)


		'2. List of PropertyResult to return as XML
		Dim oIVectorConnectResults As New Generic.List(Of ivci.SearchBookingsResponse.Booking)
		For Each oIVectorConnectResultsIndex As IVectorConnectResultsIndex In oIVectorConnectResultsIndexs
			If oIVectorConnectResultsIndex.BookingResult IsNot Nothing Then
				oIVectorConnectResultsIndex.BookingResult.ArrivalDateReached = oIVectorConnectResultsIndex.BookingResult.ArrivalDate <= DateTime.Now
				oIVectorConnectResults.Add(oIVectorConnectResultsIndex.BookingResult)
			End If
		Next

		'3. Serialize oIVectorConnectResults into XML
		Dim oXML As XmlDocument = Intuitive.Serializer.Serialize(oIVectorConnectResults, True)


		'4. Return XML
		Return oXML

	End Function


	'Get Range
	Public Function GetRange(ByVal Page As Integer) As Generic.List(Of IVectorConnectResultsIndex)

		'1. List Of iVectorConnectResultIndex
		Dim oIVectorConnectResultsIndexs As New Generic.List(Of IVectorConnectResultsIndex)

		'2. Get List of WorkTableItems
		Dim oWorkTable As Generic.List(Of WorkTableItem) = Me.GetWorkTable(Page)

		'3. Add PropertyResult to oIVectorConnectResultsIndex from iVectorConnectResults using index
		For Each oItem As WorkTableItem In oWorkTable
			Dim oIVectorConnectResultsIndex As New IVectorConnectResultsIndex
			oIVectorConnectResultsIndex.Index = oItem.Index
			If Me.iVectorConnectBookings.Count > oItem.Index Then
				oIVectorConnectResultsIndex.BookingResult = (Me.iVectorConnectBookings(oItem.Index))
			End If
			oIVectorConnectResultsIndexs.Add(oIVectorConnectResultsIndex)
		Next

		'4. Return
		Return oIVectorConnectResultsIndexs

	End Function


	'Get Work Table Item - Get Range of work table items using Page to generate a range
	Public Function GetWorkTable(ByVal Page As Integer) As Generic.List(Of WorkTableItem)

		Dim oWorkTable As New Generic.List(Of WorkTableItem)

		Try

			'1. Select WorkItems where Display = true
			oWorkTable = Me.WorkTable.Where(Function(o) o.Display = True AndAlso o.ForceHidden = False).ToList

			'2. Set range indexes
			Dim iStartIndex As Integer = (Page - 1) * BookingBase.Params.BookingsPerPage
			Dim iCount As Integer = Functions.IIf(oWorkTable.Count < BookingBase.Params.BookingsPerPage, oWorkTable.Count, BookingBase.Params.BookingsPerPage)
			iCount = Functions.IIf(iStartIndex + iCount > oWorkTable.Count, oWorkTable.Count - iStartIndex, iCount)

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
	Public Sub SortResults(ByVal SortBy As String, ByVal SortOrder As String)

		'get enum values
		Dim eSortBy As eSortBy = Intuitive.Functions.SafeEnum(Of BookingsHandler.eSortBy)(SortBy)
		Dim eSortOrder As eSortOrder = Intuitive.Functions.SafeEnum(Of BookingsHandler.eSortOrder)(SortOrder)

		'sort results
		Me.SortResults(eSortBy, eSortOrder)

	End Sub

	'Sort Results (enum)
	Public Sub SortResults(ByVal SortBy As eSortBy, ByVal SortOrder As eSortOrder)

		Me.BookingsSort.SortBy = SortBy
		Me.BookingsSort.SortOrder = SortOrder

		For Each oItem As WorkTableItem In Me.WorkTable


			If oItem.Display AndAlso Me.iVectorConnectBookings.Count > oItem.Index Then

				Dim oBooking As ivci.SearchBookingsResponse.Booking = Me.iVectorConnectBookings(oItem.Index)

				If oItem.BookingReference = "" Then
					oItem.BookingReference = oBooking.BookingReference
				End If

				If oItem.LeadCustomerLastName = "" Then
					oItem.LeadCustomerLastName = oBooking.LeadCustomerLastName
				End If

				If oItem.LeadCustomerFirstName = "" Then
					oItem.LeadCustomerFirstName = oBooking.LeadCustomerFirstName
				End If

				If oItem.BookingDate = Nothing Then
					oItem.BookingDate = oBooking.BookingDate
				End If

				If oItem.ArrivalDate = Nothing Then
					oItem.ArrivalDate = oBooking.ArrivalDate
				End If

				If oItem.Resort = "" AndAlso oBooking.GeographyLevel3ID > 0 Then
					Dim resortname As String = ""
					Dim location As Lookups.Location = BookingBase.Lookups.GetLocationFromResort(oBooking.GeographyLevel3ID)
					If location IsNot Nothing Then
						resortname = location.GeographyLevel3Name
					End If
					oItem.Resort = resortname
				End If

				If oItem.Status = "" Then
					oItem.Status = oBooking.Status
				End If

				If oItem.AccountStatus = "" Then
					oItem.AccountStatus = oBooking.AccountStatus
				End If

			End If

		Next


		Select Case SortBy
			Case eSortBy.BookingReference
				Me.WorkTable = Me.WorkTable.OrderBy(Function(o) o.BookingReference).ToList
			Case eSortBy.LeadCustomerName
				Me.WorkTable = Me.WorkTable.OrderBy(Function(o) o.LeadCustomerFirstName).OrderBy(Function(o) o.LeadCustomerLastName).ToList
			Case eSortBy.BookingDate
				Me.WorkTable = Me.WorkTable.OrderBy(Function(o) o.BookingDate).ToList
			Case eSortBy.ArrivalDate
				Me.WorkTable = Me.WorkTable.OrderBy(Function(o) o.ArrivalDate).ToList
			Case eSortBy.Resort
				Me.WorkTable = Me.WorkTable.OrderBy(Function(o) o.Resort).ToList
			Case eSortBy.Status
				Me.WorkTable = Me.WorkTable.OrderBy(Function(o) o.Status).ToList
			Case eSortBy.PaymentStatus
				Me.WorkTable = Me.WorkTable.OrderBy(Function(o) o.AccountStatus).ToList
		End Select

		If SortOrder = eSortOrder.Descending Then
			Me.WorkTable.Reverse()
		End If


		'1. Set Sort Value For each ResultIndex in WorkTable
		'For Each oItem As WorkTableItem In Me.WorkTable

		'	If oItem.Display Then

		'		'1.a. Select Property Using Index
		'		Dim oBookingResult As ivci.SearchBookingsResponse.Booking = Me.iVectorConnectBookings(oItem.Index)

		'		'1.b. Set SortValue
		'		Dim sSortValue As String = ""
		'		sSortValue = XMLFunctions.SafeNodeValue(oPropertyResult.SearchResponseXML, sSortXPath)
		'		oItem.SortValue = sSortValue.ToLower

		'	Else
		'		oItem.SortValue = ""
		'	End If

		'Next


		''2. Order WorkTable
		'If sSortXPath = "" Then
		'	If SortOrder = eSortOrder.Ascending Then
		'		Me.WorkTable = Me.WorkTable.OrderBy(Function(o) o.Display = True).OrderBy(Function(o) o.MinPrice).ToList
		'	Else
		'		Me.WorkTable = Me.WorkTable.OrderBy(Function(o) o.Display = True).OrderByDescending(Function(o) o.MinPrice).ToList
		'	End If
		'Else
		'	If SortOrder = eSortOrder.Ascending Then
		'		Me.WorkTable = Me.WorkTable.OrderBy(Function(o) o.Display = True).OrderBy(Function(o) o.MinPrice).OrderBy(Function(o) o.SortValue).ToList
		'	Else
		'		Me.WorkTable = Me.WorkTable.OrderBy(Function(o) o.Display = True).OrderBy(Function(o) o.MinPrice).OrderByDescending(Function(o) o.SortValue).ToList
		'	End If

		'End If

	End Sub

	Public Function GetSortXML() As XmlDocument

		Dim oXML As XmlDocument = Intuitive.Serializer.Serialize(Me.BookingsSort)

		Return oXML

	End Function

#End Region

#Region "Support"

#Region "Filter"

	'Filter Results
	Public Sub FilterResults(ByVal Filter As Filters)

		'1. Store filter as class property
		Me.BookingsFilter = Filter

		'2. Set Display value for each work item
		For Each oItem As WorkTableItem In Me.WorkTable

			'2.a Select Property using Item index
			If Me.iVectorConnectBookings.Count > oItem.Index Then
				Dim ivcBookingResult As ivci.SearchBookingsResponse.Booking = Me.iVectorConnectBookings(oItem.Index)

				'2.b Filter PropertyResult
				Me.FilterItem(ivcBookingResult, oItem)
			End If

		Next

	End Sub

	'Filter Item
	Public Sub FilterItem(ByVal ivcBookingResult As ivci.SearchBookingsResponse.Booking, ByVal oWorkTableItem As WorkTableItem)

		'1. Declare Filter Value Variables
		Dim bDisplay As Boolean = True
		Dim sBookingReference As String = ""
		Dim sLeadCustomerFirstName As String = ""
		Dim sLeadCustomerLastName As String = ""
		Dim dBookingDate As Date
		Dim dArrivalDate As Date
        Dim sStatus As String = ""
		'2. Check which bookings should show using filters
		'2.a. Booking Reference
		sBookingReference = Functions.IIf(Not oWorkTableItem.BookingReference = "", oWorkTableItem.BookingReference, ivcBookingResult.BookingReference)
		If bDisplay AndAlso Me.BookingsFilter.Reference <> "" Then
			If sBookingReference Is Nothing OrElse Not sBookingReference.ToLower.Contains(Me.BookingsFilter.Reference.ToLower) Then
				bDisplay = False
			End If
		End If


		'2.b. Guest Name
		sLeadCustomerFirstName = Functions.IIf(Not oWorkTableItem.LeadCustomerFirstName = "", oWorkTableItem.LeadCustomerFirstName, ivcBookingResult.LeadCustomerFirstName)
		sLeadCustomerLastName = Functions.IIf(Not oWorkTableItem.LeadCustomerLastName = "", oWorkTableItem.LeadCustomerLastName, ivcBookingResult.LeadCustomerLastName)
		If bDisplay AndAlso Me.BookingsFilter.GuestName <> "" Then
			If Not sLeadCustomerFirstName.ToLower.Contains(Me.BookingsFilter.GuestName.ToLower) _
			 AndAlso Not sLeadCustomerLastName.ToLower.Contains(Me.BookingsFilter.GuestName.ToLower) Then
				bDisplay = False
			End If
		End If

		'2.c. Booking Start Date
		dBookingDate = Functions.IIf(Not oWorkTableItem.BookingDate = Nothing, oWorkTableItem.BookingDate, ivcBookingResult.BookingDate)
		If bDisplay AndAlso Not DateFunctions.IsEmptyDate(Me.BookingsFilter.BookedStartDate) Then
			If dBookingDate < Me.BookingsFilter.BookedStartDate Then
				bDisplay = False
			End If
		End If
		If bDisplay AndAlso Not DateFunctions.IsEmptyDate(Me.BookingsFilter.BookedEndDate) Then
			If dBookingDate > Me.BookingsFilter.BookedEndDate Then
				bDisplay = False
			End If
		End If

		'2.d. Arrival Date
		dArrivalDate = Functions.IIf(Not oWorkTableItem.ArrivalDate = Nothing, oWorkTableItem.ArrivalDate, ivcBookingResult.ArrivalDate)
		If bDisplay AndAlso Not DateFunctions.IsEmptyDate(Me.BookingsFilter.ArrivalStartDate) Then
			If dArrivalDate < Me.BookingsFilter.ArrivalStartDate Then
				bDisplay = False
			End If
		End If
		If bDisplay AndAlso Not DateFunctions.IsEmptyDate(Me.BookingsFilter.ArrivalEndDate) Then
			If dArrivalDate > Me.BookingsFilter.ArrivalEndDate Then
				bDisplay = False
			End If
		End If

        '2.e. Status
	      sStatus = Functions.IIf(Not oWorkTableItem.Status = Nothing, oWorkTableItem.Status, ivcBookingResult.Status)

        If bDisplay And sStatus <> "Live" then
            If(sStatus <> me.BookingsFilter.Status)
                bDisplay = false
            End If
        End If
		'3. Set Work Table Item Properties
		oWorkTableItem.SetProperties(sBookingReference, sLeadCustomerFirstName, sLeadCustomerLastName, dBookingDate, dArrivalDate, bDisplay, sStatus)

	End Sub

	'Filter Items
	Public Sub FilterItems(ByVal WorkTable As Generic.List(Of WorkTableItem))

		For Each oItem As WorkTableItem In WorkTable

			Dim ivcPropertyResult As ivci.SearchBookingsResponse.Booking = Me.iVectorConnectBookings(oItem.Index)

			Me.FilterItem(ivcPropertyResult, oItem)
		Next

	End Sub

#End Region

#Region "Work Table Item"

	Public Class WorkTableItem

		Public Index As Integer
		Public BookingReference As String
		Public SortValue As String = ""
		Public Display As Boolean = True
		Public ForceHidden As Boolean = False

		Public LeadCustomerFirstName As String
		Public LeadCustomerLastName As String
		Public BookingDate As Date
		Public ArrivalDate As Date
		Public Resort As String
		Public Status As String
		Public AccountStatus As String

		Public Sub New(ByVal Index As Integer, ByVal BookingReference As String)
			Me.Index = Index
			Me.BookingReference = BookingReference
		End Sub


		'Set Work Table Item Values
		Public Sub SetProperties(ByVal sBookingReference As String, ByVal sLeadCustomerFirstName As String, ByVal sLeadCustomerLastName As String, ByVal dBookingDate As Date,
		ByVal dArrivalDate As Date, ByVal bDisplay As Boolean, sStatus As String)

			Me.BookingReference = sBookingReference
			Me.LeadCustomerFirstName = sLeadCustomerFirstName
			Me.LeadCustomerLastName = sLeadCustomerLastName
			Me.BookingDate = dBookingDate
			Me.ArrivalDate = dArrivalDate
			Me.Display = bDisplay
            me.Status = sStatus
		End Sub


	End Class

#End Region

#Region "Filter"

	Public Class Filters

		'Filters
		Public Reference As String = ""
		Public GuestName As String = ""
		Public BookedStartDate As Date = DateFunctions.EmptyDate
		Public BookedEndDate As Date = DateFunctions.EmptyDate
		Public ArrivalStartDate As Date = DateFunctions.EmptyDate
		Public ArrivalEndDate As Date = DateFunctions.EmptyDate
		Public Booked As String = "Any"
		Public Arrival As String = "Any"
        Public Status As String = "Live"
	End Class

#End Region

#Region "Sorting"

	Public Class Sort

		Public SortBy As eSortBy = eSortBy.BookingReference
		Public SortOrder As eSortOrder = eSortOrder.Ascending

		<XmlIgnore()> Public NameXPath As String = "Booking/LeadCustomerName"
		<XmlIgnore()> Public BookingDateXPath As String = "Booking/BookingDate"
		<XmlIgnore()> Public ArrivalDateXPath As String = "Booking/ArrivalDate"
		<XmlIgnore()> Public ResortXPath As String = "Booking/Resort"
		<XmlIgnore()> Public StatusXPath As String = "Booking/Status"
		<XmlIgnore()> Public PaymentStatusXPath As String = "Booking/PaymentStatus"

		'Sort X Path
		Public Function SortXPath(ByVal SortBy As eSortBy) As String

			Select Case SortBy
				Case eSortBy.LeadCustomerName
					Return Me.NameXPath
				Case eSortBy.BookingDate
					Return Me.BookingDateXPath
				Case eSortBy.ArrivalDate
					Return Me.ArrivalDateXPath
				Case eSortBy.Resort
					Return Me.ResortXPath
				Case eSortBy.Status
					Return Me.StatusXPath
				Case eSortBy.PaymentStatus
					Return Me.PaymentStatusXPath
			End Select

			Return ""

		End Function

	End Class

#End Region


#Region "Classes"

	Public Class IVectorConnectResultsIndex
		Public Index As Integer
		Public BookingResult As ivci.SearchBookingsResponse.Booking
	End Class

#End Region

#Region "Functions"

	'Clear Work Table
	Public Sub ClearWorkTable()
		Me.iVectorConnectBookings.Clear()
		Me.WorkTable.Clear()
	End Sub

#End Region

#Region "Enums"

	Public Enum eSortBy
		BookingReference
		LeadCustomerName
		BookingDate
		ArrivalDate
		Resort
		Status
		PaymentStatus
	End Enum

	Public Enum eSortOrder
		Ascending
		Descending
	End Enum

#End Region

#End Region


End Class
