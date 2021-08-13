Imports System.Xml.Serialization
Imports Intuitive.DateFunctions
Imports Intuitive.Validators
Imports System.Text.RegularExpressions
Imports iVectorConnectInterface.Interfaces

Namespace Extra

	<XmlRoot("ExtraOptionsRequest")>
	Public Class OptionsRequest
		Implements iVectorConnectInterface.Interfaces.iVectorConnectRequest

		Public Property LoginDetails As LoginDetails Implements iVectorConnectRequest.LoginDetails
		Public Property BookingToken As String
		Public Property ExtraID As Integer
		Public Property UseDate As Date
		Public Property EndDate As Date

		Public Function Validate(Optional ValidationType As eValidationType = eValidationType.None) As List(Of String) Implements iVectorConnectRequest.Validate
			Dim aWarning As New List(Of String)

			If ExtraID < 0 Then
				aWarning.Add("The ExtraID must be greater than zero.")
			End If

			If Not IsEmptyDate(UseDate) AndAlso UseDate < Date.Now Then
				aWarning.Add("The use date must not be in the past.")
			ElseIf IsEmptyDate(UseDate) Then
				aWarning.Add("A valid use date must be specified.")
			End If

			If Not IsEmptyDate(EndDate) AndAlso EndDate < UseDate Then
				aWarning.Add("The end date must not be before the use date.")
			ElseIf IsEmptyDate(EndDate) Then
				aWarning.Add("A valid end date must be specified.")
			End If

			Return aWarning
		End Function

	End Class

End Namespace
