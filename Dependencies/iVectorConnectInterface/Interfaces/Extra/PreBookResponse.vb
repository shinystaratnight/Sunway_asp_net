Imports System.Xml.Serialization
Imports Intuitive.Domain.Financial

Namespace Extra
	<XmlType("ExtraPreBookResponse")>
	<XmlRoot("ExtraPreBookResponse")>
	Public Class PreBookResponse
        Implements iVectorConnectInterface.Interfaces.IVectorConnectResponse

        Public Property ReturnStatus As New ReturnStatus Implements Interfaces.IVectorConnectResponse.ReturnStatus

		Public Property BookingToken As String
		Public Property TotalPrice As Decimal
		Public Property TotalCommission As Decimal
		Public Property VATOnCommission As Decimal
		Public Property ExtraOptions As List(Of ExtraOption)
		Public Property Cancellations As New CancellationsList(Of Cancellation)
		Public Property PaymentsDue As New List(Of Support.PaymentDue)
		Public Property CommissionType As String
		Public Property CommissionValue As String
        Public Property ExtraLocations As ExtraLocations
        Public Property BondingID As Integer
        Public Property BookingQuestions As New List(Of BookingQuestion)
		Public Property JoiningInstructions As String

        <XmlIgnore()>
		Public Property hlpExtraBookingID As Integer

		Public Class ExtraOption
			Public Property BookingToken As String
            Public Property ExtraCategoryDescription As String

			Public Sub New()
			End Sub

			Public Sub New(BookingToken As String)
				Me.BookingToken = BookingToken
			End Sub
		End Class
	End Class
End Namespace
