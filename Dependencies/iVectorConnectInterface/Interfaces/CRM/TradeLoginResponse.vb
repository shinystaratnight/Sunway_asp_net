Public Class TradeLoginResponse
    Implements iVectorConnectInterface.Interfaces.IVectorConnectResponse

    Public Property ReturnStatus As New ReturnStatus Implements Interfaces.IVectorConnectResponse.ReturnStatus

	Public Property TradeID As Integer
    Public Property TradeContactID As Integer
	Public Property CreditCardAgent As Boolean
	Public Property ABTAATOL As String
	Public Property Commissionable As Boolean

End Class
