Public Class GetThirdPartySettingsResponse
	Implements iVectorConnectInterface.Interfaces.IVectorConnectResponse

	Public Property ReturnStatus As New ReturnStatus Implements Interfaces.IVectorConnectResponse.ReturnStatus

	Public Property ThirdPartySettings As New Generic.List(Of ThirdPartySetting)


	Public Class ThirdPartySetting
		Public Property Name As String
		Public Property Value As String

		Public Sub New()
			MyBase.New()
		End Sub

		Public Sub New(ByVal Name As String, ByVal Value As String)
			Me.Name = Name
			Me.Value = Value
		End Sub

	End Class

End Class
