Imports System.Xml.Serialization
Imports Intuitive.DateFunctions

Namespace Flight
	<XmlRoot("FlightSearchRequest")>
	Public Class SearchRequest
        Implements iVectorConnectInterface.Interfaces.IVectorConnectRequest, Interfaces.ISearchRequest

#Region "Properties"

        Public Property LoginDetails As LoginDetails Implements Interfaces.IVectorConnectRequest.LoginDetails
		Public Property DepartureAirportID As Integer
		Public Property DepartureAirportGroupID As Integer
		Public Property ArrivalAirportID As Integer
		Public Property ArrivalAirportGroupID As Integer
		Public Property AltReturnAirportID As Integer
		Public Property RegionID As Integer
		<XmlArrayItem("ResortID")>
		Public Property Resorts As New Generic.List(Of Integer)

        Public Property DepartureDate As Date Implements Interfaces.ISearchRequest.ArrivalDate

		Public Property Duration As Integer
		Public Property OneWay As Boolean
		Public Property GuestConfiguration As New Support.GuestConfiguration
		Public Property ThirdPartyID As Integer
		Public Property ExactMatch As Boolean
		Public Property FlightAndHotel As Boolean
		Public Property AllowMultisectorFlights As Boolean = True
		Public Property MaxResults As Integer

		'needed to store search results by user ip
		Public Property IPAddress As String

		'needed to distinguish websites
		Public Property URL As String

		Public Property FlightClassID As Integer
        Public Property InboundFlightClassID As Integer
		Public Property FlightCarrierID As Integer

        Public Property MajorityFlightCarrierID As Integer

		''' <summary>
		'''     Gets or sets a value indicating whether multi carrier (mix and match).
		''' </summary>
		''' <value>
		'''     <c>true</c> if multi carrier (mix and match); otherwise, <c>false</c>.
		''' </value>
		Public Property MultiCarrier As Boolean = True

		Public Property CarouselMode As Boolean = False

		Public Property WidenSearch As Boolean = True

		Public Property CacheOnly As Boolean = False

        Public Property MixedFlightClasses As Boolean = False

        Public Property BookingDate As DateTime

#End Region

#Region "Validation"

		Public Function Validate(Optional ValidationType As Interfaces.eValidationType = Interfaces.eValidationType.None) _
            As List(Of String) Implements Interfaces.IVectorConnectRequest.Validate

			Dim aWarnings As New List(Of String)

			'departure airport
			If Me.DepartureAirportID = 0 AndAlso Me.DepartureAirportGroupID = 0 Then _
				aWarnings.Add("A Departure Airport ID or Departure Airport Group ID must be specified.")

			'arrival airport
			If Me.ArrivalAirportID = 0 AndAlso Me.ArrivalAirportGroupID = 0 AndAlso Me.RegionID = 0 AndAlso Me.Resorts.Sum = 0 _
				Then aWarnings.Add("An Arrival Airport ID, Airport Group ID, Region ID, or Resort ID must be specified.")

			'departure date
			If Not DateFunctions.IsEmptyDate(Me.DepartureDate) AndAlso Me.DepartureDate < Now.Date Then
				aWarnings.Add("The departure date must not be in the past.")
			ElseIf DateFunctions.IsEmptyDate(Me.DepartureDate) Then
				aWarnings.Add("A valid departure date must be specified.")
			End If

            Dim sNoPassengersWarning As String = "At least one passenger must be specified."
			If Not Me.GuestConfiguration Is Nothing Then
                aWarnings.AddRange(Me.GuestConfiguration.Validate(sNoPassengersWarning))
			Else
                aWarnings.Add(sNoPassengersWarning)
			End If

			Return aWarnings
		End Function

#End Region

#Region "Helper Classes"

		Public Class FlightLeg
			Public Property Position As Integer
			Public Property DepatureAirportID As Integer
			Public Property ArrivalAirportID As Integer
			Public Property DepartureDate As Date
		End Class

#End Region

	End Class
End Namespace
