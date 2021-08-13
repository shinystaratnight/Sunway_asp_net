Imports System.Xml.Serialization
Imports iVectorConnectInterface.Interfaces

Namespace Flight

	<XmlType("ReleaseFlightSeatLockRequest")>
	<XmlRoot("ReleaseFlightSeatLockRequest")>
	Public Class ReleaseFlightSeatLockRequest
		Implements Interfaces.iVectorConnectRequest

		Public Property LoginDetails As LoginDetails Implements iVectorConnectRequest.LoginDetails
		Public Property BookingToken As String
		Public Property FlightAndHotel As Boolean
		Public Property Duration As Integer
		Public Property SessionID As String

		Public Function Validate(Optional ValidationType As eValidationType = eValidationType.None) As List(Of String) Implements iVectorConnectRequest.Validate

			Dim warnings As New Generic.List(Of String)
			If SessionID = "" Then warnings.Add("A Session ID must be specified.")
			Return warnings

		End Function
	End Class

End Namespace