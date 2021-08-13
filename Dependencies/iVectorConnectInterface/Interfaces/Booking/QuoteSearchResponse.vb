Public Class QuoteSearchResponse
	Implements iVectorConnectInterface.Interfaces.iVectorConnectResponse

	Public Property ReturnStatus As New ReturnStatus Implements Interfaces.iVectorConnectResponse.ReturnStatus
	Public Property Quotes As New Generic.List(Of Quote)

End Class