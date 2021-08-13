Imports Intuitive.Functions
Imports Intuitive.Validators
Imports System.Xml.Serialization

Namespace CarHire

    <XmlType("CarHireBookRequest")>
    <XmlRoot("CarHireBookRequest")>
    Public Class BookRequest

#Region "Properties"


        Public Property BookingToken As String
        Public Property ExpectedTotal As Decimal = -1
		<XmlArrayItem("GuestID")>
		Public Property GuestIDs As New Generic.List(Of Integer)
		<XmlArrayItem("GuestID")>
		Public Property Drivers As New Generic.List(Of Integer)
		Public Property BookingTags As New Generic.List(Of Support.BookingTag)
        Public Property PayLocal As Boolean


#End Region

#Region "Validation"

        Public Function Validate(ByVal oGuestDetails As Generic.List(Of Support.GuestDetail), Optional ByVal ValidationType As Interfaces.eValidationType = Interfaces.eValidationType.None) As Generic.List(Of String)

            Dim aWarnings As New Generic.List(Of String)
            Dim aGuestIDs As New ArrayList

			'get the guest ids from guestdetails
			Dim aGuestDetailIDs As Generic.List(Of Integer) = oGuestDetails.Select(Function(x) x.GuestID).ToList()

			'booking token
			If Me.BookingToken = "" Then aWarnings.Add("A booking token must be specified.")

            'expected total
            If Me.ExpectedTotal < 0 Then aWarnings.Add("An expected total must be specified.")

			'guests
			If Me.GuestIDs.Count = 0 Then aWarnings.Add("At least one Guest must be specified for each CarHire booking")

			If Me.GuestIDs.GroupBy(Function(x) x).Where(Function(x) x.Count > 1).Select(Function(x) x).Count > 0 Then
				aWarnings.Add("The same Guest cannot be specified twice on an CarHire booking")
			End If

			If Not Me.GuestIDs.All(Function(x) aGuestDetailIDs.Contains(x)) Then
				aWarnings.Add("Guest Details must be specified for each Guest")
			End If

			'drivers
			If Me.Drivers.GroupBy(Function(x) x).Where(Function(x) x.Count > 1).Select(Function(x) x).Count > 0 Then
				aWarnings.Add("The same guest cannot be specified twice as a driver")
			End If

            If Not Me.Drivers.All(Function(x) aGuestDetailIDs.Contains(x)) Then
                aWarnings.Add("The driver Guest IDs must match the provided Guests")
            End If

            Return aWarnings

        End Function

#End Region


    End Class

End Namespace
