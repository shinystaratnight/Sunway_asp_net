Imports System.Security.Cryptography.X509Certificates

Namespace Security

	''' <summary>
	''' Class containing functions for retrieving certificates and generating alphanumeric keys
	''' </summary>
	Public Class Functions

#Region "Generate Alphanumeric"

		''' <summary>
		''' Generates a random alphanumeric string
		''' </summary>
		Public Shared Function GenerateAlphanumeric() As String

			Dim sChars As String = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ"

			'get a random hex number as a string
			Dim sRandomHexString As String = System.Guid.NewGuid().ToString.Replace("-"c, "").ToUpper
			Dim sAlphanumeric As String = ""

			'step through our hex string 2 chars at a time and convert each set of chars into an alphanumeric character using a lookup
			For iIndex As Integer = 0 To sRandomHexString.Length - 2 Step 2
				sAlphanumeric &= sChars.Chars(Math.Abs(Convert.ToInt32(sRandomHexString.Substring(iIndex, 2), 16) Mod 36))
			Next

			Return sAlphanumeric

		End Function

		''' <summary>
		''' Generates a random alphanumeric string of fixed length
		''' </summary>
		''' <param name="Length">The length.</param>
		Public Shared Function GenerateFixedLengthAlphanumeric(ByVal Length As Integer) As String

			'generate a string that's at least as long as the required length
			Dim sAlphanumeric As String = ""
			While sAlphanumeric.Length < Length
				sAlphanumeric &= GenerateAlphanumeric()
			End While

			'chop it to the correct length and return it
			Return sAlphanumeric.Substring(0, Length)

		End Function

#End Region

#Region "Certificates"

		''' <summary>
		''' Gets the certificate.
		''' </summary>
		''' <param name="CertificateFriendlyName">Alias of the certificate to get.</param>
		''' <param name="StoreLocation">The location of the certificate store.</param>
		Public Shared Function GetCertificate(ByVal CertificateFriendlyName As String, Optional ByVal StoreLocation As StoreLocation = StoreLocation.LocalMachine) As X509Certificate2

			'Open the certificate store - use the location machine store by default as this is where most of them are stored,
			' this may need to change to local user depending on where the certificate has been installed
			Dim oStore As New X509Store(StoreLocation)
			oStore.Open(OpenFlags.ReadOnly)

			'Get the certificates out of the store
			Dim oCertificates As X509CertificateCollection = oStore.Certificates

			'Loop through each certificate and find the one with the right name
			Dim oCertificate As X509Certificate2 = Nothing

			For Each oCert As X509Certificate2 In oCertificates
				If oCert.FriendlyName = CertificateFriendlyName Then
					oCertificate = oCert
					Exit For
				End If
			Next

			'Close the store
			oStore.Close()

			Return oCertificate

		End Function

#End Region

	End Class

End Namespace