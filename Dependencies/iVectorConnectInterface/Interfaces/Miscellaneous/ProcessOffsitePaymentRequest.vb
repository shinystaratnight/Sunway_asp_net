Imports System.Xml.Serialization

Public Class ProcessOffsitePaymentRedirectRequest
	Implements iVectorConnectInterface.Interfaces.IVectorConnectRequest

	Public Property LoginDetails As LoginDetails Implements Interfaces.IVectorConnectRequest.LoginDetails

	Public Property URL As String
	Public Property QueryString As String
	Public Property Headers As New List(Of Header)
	Public Property Cookies As New List(Of Cookie)
	Public Property Body As String
	Public Property PaymentToken As String

	Public Property BookingDetails As GetOffsitePaymentRedirectRequest.BookingDetailsDef

#Region "validation"


	Public Function Validate(Optional ValidationType As Interfaces.eValidationType = Interfaces.eValidationType.None) As System.Collections.Generic.List(Of String) Implements Interfaces.IVectorConnectRequest.Validate

		Dim aWarnings As New Generic.List(Of String)

		'url
		If Me.URL = "" Then aWarnings.Add("A URL must be specified")

		'booking details?
		If Me.BookingDetails Is Nothing Then

			aWarnings.Add("Booking Details must be specified")

		Else

			'booking details validation
			aWarnings.AddRange(Me.BookingDetails.Validate(ValidationType))

		End If

		Return aWarnings

	End Function

#End Region

#Region "helper classes"

	Public Class Header
		Public Property Name As String
		Public Property Value As String
	End Class

	Public Class Cookie
		Public Property Name As String
		Public Property Value As String
	End Class

#End Region

End Class
