Imports System.Xml.Serialization
Imports Intuitive.DateFunctions
Imports Intuitive.Validators
Imports System.Text.RegularExpressions

Namespace CarHire

    <XmlRoot("CarHireInsuranceQuoteRequest")>
    Public Class InsuranceQuoteRequest
        Implements iVectorConnectInterface.Interfaces.IVectorConnectRequest

#Region "Properties"

        Public Property LoginDetails As LoginDetails Implements Interfaces.IVectorConnectRequest.LoginDetails

        Public Property BookingToken As String
        Public Property LeadGuestFirstName As String
        Public Property LeadGuestLastName As String

#End Region

#Region "Validation"

        Public Function Validate(Optional ValidationType As Interfaces.eValidationType = Interfaces.eValidationType.None) As Generic.List(Of String) Implements Interfaces.IVectorConnectRequest.Validate

            Dim aWarnings As New Generic.List(Of String)

            If Me.BookingToken = "" Then aWarnings.Add("A car hire booking token must be specified")

            If Me.LeadGuestFirstName = "" Then aWarnings.Add("A lead guest first name must be specified")

            If Me.LeadGuestLastName = "" Then aWarnings.Add("A lead guest last name must be specified")

            Return aWarnings

        End Function

#End Region


    End Class

End Namespace