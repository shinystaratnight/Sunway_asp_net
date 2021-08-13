Imports System.Xml.Serialization

Public Class QuoteSearchRequest
	Implements iVectorConnectInterface.Interfaces.iVectorConnectRequest

	Public Property LoginDetails As LoginDetails Implements Interfaces.iVectorConnectRequest.LoginDetails
	Public Property CustomerID As Integer
	Public Property TradeContactID As Integer
	Public Property QuoteReference As String
	Public Property TradeReference As String
	Public Property EarliestBookingDate As Date
	Public Property EarliestBookingTime As String
	Public Property LatestBookingDate As Date
	Public Property LatestBookingTime As String
	Public Property EarliestDepartureDate As Date
	Public Property LatestDepartureDate As Date
	Public Property Duration As Integer
	<XmlArrayItem("BrandID")>
	Public Property BrandIDs As New Generic.List(Of Integer)
    Public Property Source As String

    Public Function Validate(Optional ValidationType As Interfaces.eValidationType = Interfaces.eValidationType.None) As System.Collections.Generic.List(Of String) Implements Interfaces.iVectorConnectRequest.Validate

        Dim aWarnings As New Generic.List(Of String)

        If Me.BrandIDs.Any(Function(i) i < 1) Then
            aWarnings.Add(Support.WarningMessage.BrandIDsNotPositive)
        End If

        Return aWarnings
    End Function

End Class