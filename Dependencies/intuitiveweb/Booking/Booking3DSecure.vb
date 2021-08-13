Imports System.Xml.Serialization
Imports Intuitive
Imports Intuitive.Functions
Imports ivci = iVectorConnectInterface

Public Class Booking3DSecure
	Implements IBooking3DSecure

#Region "Class Definitions"

	Public Class Get3DSecureRedirectReturn
		Implements IGet3DSecureRedirectReturn
		Public Property Success As Boolean = True Implements IGet3DSecureRedirectReturn.Success
		Public Property RequiresSecureAuthenticationSuccess As Boolean Implements IGet3DSecureRedirectReturn.RequiresSecureAuthenticationSuccess
		Public Property Enrollment As Boolean Implements IGet3DSecureRedirectReturn.Enrollment
		Public Property HTMLData As String Implements IGet3DSecureRedirectReturn.HTMLData
		Public Property PaymentToken As String Implements IGet3DSecureRedirectReturn.PaymentToken
		Public Property Warnings As New List(Of String) Implements IGet3DSecureRedirectReturn.Warnings
	End Class

	Public Class Process3DSecureReturn
		Public Success As Boolean = True
		Public PaymentToken As String
		Public ThreeDSecureCode As String
		Public Warnings As New Generic.List(Of String)
	End Class

	Public Class Get3DSecureRedirectDetails
		Implements IGet3DSecureRedirectDetails
		Public Property BookingReference As String Implements IGet3DSecureRedirectDetails.BookingReference
		Public Property BrowserAcceptHeader As String Implements IGet3DSecureRedirectDetails.BrowserAcceptHeader
		Public Property PaymentDetails As iVectorConnectInterface.Support.PaymentDetails Implements IGet3DSecureRedirectDetails.PaymentDetails
		Public Property RedirectURL As String Implements IGet3DSecureRedirectDetails.RedirectURL
		Public Property UserAgentHeader As String Implements IGet3DSecureRedirectDetails.UserAgentHeader
		Public Property UserIPAddress As String Implements IGet3DSecureRedirectDetails.UserIPAddress
	End Class

	Public Class Process3DSecureDetails
		Public QueryString As String
		Public PaymentDetails As ivci.Support.PaymentDetails
		Public FormValues As String
	End Class

#End Region

#Region "Get 3D Secure Redirect"

	Public Shared Function Get3DSecureRedirect(ByVal oGet3DSecureRedirectDetails As Get3DSecureRedirectDetails) As Get3DSecureRedirectReturn

		Dim oGet3DSecureRedirectReturn As New Get3DSecureRedirectReturn

		Try
			Dim oGet3DSecureRedirectRequest As New ivci.Get3DSecureRedirectRequest
			With oGet3DSecureRedirectRequest
				.LoginDetails = BookingBase.IVCLoginDetails
				.BookingReference = oGet3DSecureRedirectDetails.BookingReference
				.BrowserAcceptHeader = oGet3DSecureRedirectDetails.BrowserAcceptHeader
				.Payment = oGet3DSecureRedirectDetails.PaymentDetails
				.RedirectURL = oGet3DSecureRedirectDetails.RedirectURL
				.UserAgentHeader = oGet3DSecureRedirectDetails.UserAgentHeader
				.UserIPAddress = oGet3DSecureRedirectDetails.UserIPAddress

				'Do the iVectorConnect validation procedure
				Dim oWarnings As Generic.List(Of String) = .Validate()

				If oWarnings.Count > 0 Then
					oGet3DSecureRedirectReturn.Success = False
					oGet3DSecureRedirectReturn.Warnings.AddRange(oWarnings)
				End If

			End With

			'If everything is ok then serialise the request to xml
			If oGet3DSecureRedirectReturn.Success Then

				Dim oIVCReturn As New Utility.iVectorConnect.iVectorConnectReturn
				oIVCReturn = Utility.iVectorConnect.SendRequest(Of ivci.Get3DSecureRedirectResponse)(oGet3DSecureRedirectRequest)

				Dim oGet3DSecureRedirectResponse As New ivci.Get3DSecureRedirectResponse

				If oIVCReturn.Success Then
					oGet3DSecureRedirectResponse = CType(oIVCReturn.ReturnObject, ivci.Get3DSecureRedirectResponse)

					oGet3DSecureRedirectReturn.RequiresSecureAuthenticationSuccess = oGet3DSecureRedirectResponse.RequiresSecureAuthenticationSuccess
					oGet3DSecureRedirectReturn.Enrollment = oGet3DSecureRedirectResponse.Enrollment
					oGet3DSecureRedirectReturn.HTMLData = oGet3DSecureRedirectResponse.HTMLData
					oGet3DSecureRedirectReturn.PaymentToken = oGet3DSecureRedirectResponse.PaymentToken
				Else
					oGet3DSecureRedirectReturn.Success = False
				End If
			End If

		Catch ex As Exception
			oGet3DSecureRedirectReturn.Success = False
			oGet3DSecureRedirectReturn.Warnings.Add(ex.ToString)
		End Try

		Return oGet3DSecureRedirectReturn

	End Function

#End Region

#Region "wrong one"

	'Public Shared Function UseSecureAuthentication(ByVal oUseSecureAuthenticationDetails As UseSecureAuthenticationDetails) As UseSecureAuthenticationReturn

	'	Dim oUseSecureAuthenticationReturn As New UseSecureAuthenticationReturn

	'	Try
	'		Dim oUseSecureAuthenticationRequest As New ivci.UseSecureAuthenticationRequest
	'		With oUseSecureAuthenticationRequest
	'			.LoginDetails = BookingBase.IVCLoginDetails
	'			'.BookingReference = oUseSecureAuthenticationDetails.BookingReference
	'			'.BrowserAcceptHeader = oUseSecureAuthenticationDetails.BrowserAcceptHeader
	'			.Payment = oUseSecureAuthenticationDetails.PaymentDetails
	'			'.UserAgentHeader = oUseSecureAuthenticationDetails.UserAgentHeader
	'			'.UserIPAddress = oUseSecureAuthenticationDetails.UserIPAddress

	'			'Do the iVectorConnect validation procedure
	'			Dim oWarnings As Generic.List(Of String) = .Validate()

	'			If oWarnings.Count > 0 Then
	'				oUseSecureAuthenticationReturn.Success = False
	'				oUseSecureAuthenticationReturn.Warnings.AddRange(oWarnings)
	'			End If

	'		End With

	'		'If everything is ok then serialise the request to xml
	'		If oUseSecureAuthenticationReturn.Success Then

	'			Dim oIVCReturn As New Utility.iVectorConnect.iVectorConnectReturn
	'			oIVCReturn = Utility.iVectorConnect.SendRequest(Of ivci.Get3DSecureRedirectResponse)(oUseSecureAuthenticationRequest)

	'			Dim oUseSecureAuthenticationResponse As New ivci.UseSecureAuthenticationResponse

	'			If oIVCReturn.Success Then
	'				oUseSecureAuthenticationResponse = CType(oIVCReturn.ReturnObject, ivci.UseSecureAuthenticationResponse)

	'				oUseSecureAuthenticationReturn.UseSecureAuthentication = oUseSecureAuthenticationResponse.UseSecureAuthentication
	'				oUseSecureAuthenticationReturn.iFrameHTML = oUseSecureAuthenticationResponse.iFrameHTML
	'			Else
	'				oUseSecureAuthenticationReturn.Success = False
	'			End If
	'		End If

	'	Catch ex As Exception
	'		oUseSecureAuthenticationReturn.Success = False
	'		oUseSecureAuthenticationReturn.Warnings.Add(ex.ToString)
	'	End Try

	'	Return oUseSecureAuthenticationReturn

	'End Function

#End Region

#Region "Process 3D Secure Return"

	Public Shared Function Process3DSecure(ByVal oProcess3DSecureDetails As Process3DSecureDetails) As Process3DSecureReturn

		Dim oProcess3DSecureReturn As New Process3DSecureReturn

		Try
			Dim oProcess3DSecureReturnRequest As New ivci.Process3DSecureReturnRequest
			With oProcess3DSecureReturnRequest
				.LoginDetails = BookingBase.IVCLoginDetails
				.QueryString = oProcess3DSecureDetails.QueryString
				.Payment = oProcess3DSecureDetails.PaymentDetails
				.FormValues = HttpUtility.UrlEncode(oProcess3DSecureDetails.FormValues)

				'Do the iVectorConnect validation procedure
				Dim oWarnings As Generic.List(Of String) = .Validate()

				If oWarnings.Count > 0 Then
					oProcess3DSecureReturn.Success = False
					oProcess3DSecureReturn.Warnings.AddRange(oWarnings)
					FileFunctions.AddLogEntry("Process3DSecure", "Error", String.Join(",", oWarnings))
				End If

			End With

			'If everything is ok then serialise the request to xml
			If oProcess3DSecureReturn.Success Then

				Dim oIVCReturn As New Utility.iVectorConnect.iVectorConnectReturn
				oIVCReturn = Utility.iVectorConnect.SendRequest(Of ivci.Process3DSecureReturnResponse)(oProcess3DSecureReturnRequest)

				Dim oProcess3DSecureReturnResponse As New ivci.Process3DSecureReturnResponse

				If oIVCReturn.Success Then
					oProcess3DSecureReturnResponse = CType(oIVCReturn.ReturnObject, ivci.Process3DSecureReturnResponse)

					oProcess3DSecureReturn.PaymentToken = oProcess3DSecureReturnResponse.PaymentToken
				Else
                    oProcess3DSecureReturn.Warnings.AddRange(oIVCReturn.Warning)
					oProcess3DSecureReturn.Success = False
				End If
			End If

		Catch ex As Exception
			oProcess3DSecureReturn.Success = False
			oProcess3DSecureReturn.Warnings.Add(ex.ToString)
			FileFunctions.AddLogEntry("Process3DSecure", "Error", ex.ToString)
		End Try

		Return oProcess3DSecureReturn

	End Function

#End Region

	Public Function GetRedirectDetails() As IGet3DSecureRedirectDetails Implements IBooking3DSecure.GetRedirectDetails
		Return New Get3DSecureRedirectDetails
	End Function

	Public Function Get3DSecureRedirect(oGet3DSecureRedirectDetails As IGet3DSecureRedirectDetails) As IGet3DSecureRedirectReturn Implements IBooking3DSecure.Get3DSecureRedirect
		Dim o3dReturn As Get3DSecureRedirectDetails = CType(oGet3DSecureRedirectDetails, Get3DSecureRedirectDetails)

		Return Get3DSecureRedirect(o3dReturn)
	End Function
End Class
