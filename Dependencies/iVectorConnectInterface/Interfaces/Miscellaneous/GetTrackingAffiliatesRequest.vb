Public Class GetTrackingAffiliatesRequest
	Implements iVectorConnectInterface.Interfaces.IVectorConnectRequest

#Region "Properties"

	Public Property LoginDetails As LoginDetails Implements Interfaces.IVectorConnectRequest.LoginDetails
	Public Property BrandID As Integer = 0
	Public Property CMSWebsiteID As Integer = 0
	Public Property SalesChannelID As Integer = 0

#End Region

#Region "Validation"

	Public Function Validate(Optional ValidationType As Interfaces.eValidationType = Interfaces.eValidationType.None) As System.Collections.Generic.List(Of String) Implements Interfaces.IVectorConnectRequest.Validate
		Dim aWarnings As New Generic.List(Of String)

		Return aWarnings

	End Function

#End Region

End Class
