Namespace Interfaces

	Public Interface iVectorConnectRequest
		Property LoginDetails As LoginDetails
		Function Validate(Optional ValidationType As eValidationType = eValidationType.None) As List(Of String)
	End Interface

	Public Interface iVectorConnectResponse
		Property ReturnStatus As ReturnStatus
	End Interface

	Public Interface IRequest
		Function Validate() As List(Of String)
	End Interface

	Public Interface IHandler(Of TRequest As IRequest, TResponse As iVectorConnectResponse)
		Function Process(Request As TRequest) As TResponse
	End Interface

	Public Interface IItineraryRequest
		Inherits IRequest
		ReadOnly Property Response As iVectorConnectResponse
	End Interface

	Public Interface IItineraryHandler(Of T As IItineraryRequest)
		Function Process(Request As T) As iVectorConnectResponse
	End Interface

	Public Interface IItinerary
		Property Token As String
		Property Type As String
		Property BookingSourceID As Integer
		Property SalesChannelID As Integer
		Property TradeID As Integer
		Property SellingCountryID As Integer
		Property CustomerCurrencyID As Integer
		Property GroupBooking As Boolean

		Function Validate() As Generic.List(Of String)

	End Interface

	Public Interface IItineraryComponent
		Property Type As String
		Property ComponentReference As String
		Property Source As String
		Property StartDate As DateTime
		Property StartTime As String
		Property ReturnDate As DateTime
		Property ReturnTime As String
		Property Duration As Integer
		Property OnRequest As Boolean
		Property Price As Decimal
		Property Cost As Decimal
		Property Margin As Decimal
		Property Commission As Decimal
		Property Booked As Boolean
		Property GeographyLevel1ID As Integer
		Property GeographyLevel2ID As Integer
		Property GeographyLevel3ID As Integer
		Property BuyingExchangeRate As Decimal
		Property SellingExchangeRate As Decimal
		Property LocalCost As Decimal
		Property CustomerPrice As Decimal

		Property MetaData As Dictionary(Of String, String)

		Function Validate() As Generic.List(Of String)

	End Interface

	Public Interface IItineraryPassenger
		Property Title As String
		Property FirstName As String
		Property Surname As String
		Property DateOfBirth As DateTime
		Property Age As Integer
		Property PassportNumber As String

		Function Validate() As Generic.List(Of String)

	End Interface

	Public Interface IItineraryComponentExtra
		Property Type As String
		Property Description As String
		Property Quantity As Integer
		Property Price As Decimal
		Property Cost As Decimal
		Property BuyingExchangeRate As Decimal
		Property SellingExchangeRate As Decimal
		Property LocalCost As Decimal
		Property CustomerPrice As Decimal
		Property [Optional] As Boolean
		Property PayLocal As Boolean
	End Interface

	Public Enum eValidationType
		None
		[Public]
		Trade
	End Enum

	Public Interface IEncryptedRequest
        Sub Decrypt(EncryptionKey As String, EncryptionType As Security.SecretKeeper.EncryptionType)
	End Interface

    Public Interface ISearchRequest
        Property ArrivalDate As Date
    End Interface

End Namespace