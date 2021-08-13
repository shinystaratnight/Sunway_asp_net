Public Class ViewDocumentationRequest
	Implements iVectorConnectInterface.Interfaces.IVectorConnectRequest

#Region "Properties"

     Public Property LoginDetails As LoginDetails Implements Interfaces.IVectorConnectRequest.LoginDetails

    Public Property BookingReference As String
    Public Property QuoteExternalReference As String
    Public Property QuoteID As Integer
    Public Property BookingDocumentationID As Integer
    Public Property ComponentFilters As New Generic.List(Of Intuitive.Domain.Utility.ComponentFilter)

#End Region

#Region "Validation"

    Public Function Validate(Optional ValidationType As interfaces.eValidationType = Interfaces.eValidationType.None) As System.Collections.Generic.List(Of String) Implements Interfaces.IVectorConnectRequest.Validate

		Dim aWarnings As New Generic.List(Of String)

		'booking reference/QuoteExternalReference/QuoteID
		If Me.BookingReference = "" AndAlso Me.QuoteExternalReference = "" AndAlso Me.QuoteID = 0 Then aWarnings.Add(Support.WarningMessage.ReferenceNotSpecified)

		'Check whether more than one is specified
		CheckReferences(aWarnings)

		'booking documentation id
		If Me.BookingDocumentationID = 0 Then aWarnings.Add(Support.WarningMessage.BookingDocumentationIDNotSpecified)

		Return aWarnings

	End Function

	Public Sub CheckReferences(ByVal awarnings As List(Of String))
		Dim i As Integer = 0

		If Not Me.QuoteID = 0 Then
			i += 1
		End If

		If Not Me.BookingReference = "" Then
			i += 1
		End If

		If Not Me.QuoteExternalReference = "" Then
			i += 1
		End If

		If i > 1 Then
			awarnings.Add(Support.WarningMessage.TooManyReferencesSpecified)
		End If
	End Sub


#End Region

End Class
