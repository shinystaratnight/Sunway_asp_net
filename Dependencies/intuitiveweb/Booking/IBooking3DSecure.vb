Public Interface IBooking3DSecure

	Function GetRedirectDetails() As IGet3DSecureRedirectDetails
	Function Get3DSecureRedirect(ByVal oGet3DSecureRedirectDetails As IGet3DSecureRedirectDetails) As IGet3DSecureRedirectReturn

End Interface

Public Interface IGet3DSecureRedirectReturn
	Property Success As Boolean
	Property RequiresSecureAuthenticationSuccess As Boolean
	Property Enrollment As Boolean
	Property HTMLData As String
	Property PaymentToken As String
	Property Warnings As Generic.List(Of String)
End Interface

Public Interface IGet3DSecureRedirectDetails
	Property BookingReference As String
	Property BrowserAcceptHeader As String
	Property PaymentDetails As iVectorConnectInterface.Support.PaymentDetails
	Property RedirectURL As String
	Property UserAgentHeader As String
	Property UserIPAddress As String
End Interface