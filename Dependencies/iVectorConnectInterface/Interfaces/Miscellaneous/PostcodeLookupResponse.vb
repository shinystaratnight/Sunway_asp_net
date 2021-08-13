Public Class PostcodeLookupResponse
	Implements iVectorConnectInterface.Interfaces.IVectorConnectResponse

	Public Property ReturnStatus As New ReturnStatus Implements Interfaces.IVectorConnectResponse.ReturnStatus

	Public Property Addresses As New Generic.List(Of Support.Address)


End Class

