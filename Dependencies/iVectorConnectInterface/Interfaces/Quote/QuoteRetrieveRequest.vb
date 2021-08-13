Imports iVectorConnectInterface.Interfaces

Public Class QuoteRetrieveRequest
    Implements iVectorConnectRequest

    Public Property LoginDetails As LoginDetails Implements Interfaces.iVectorConnectRequest.LoginDetails

    Public Property QuoteReference As String
    Public Property RepriceQuote As Boolean

    Public Function Validate(Optional ValidationType As eValidationType = eValidationType.None) As List(Of String) Implements iVectorConnectRequest.Validate

        Dim oWarnings As New List(Of String)

        If String.IsNullOrWhiteSpace(Me.QuoteReference) Then
            oWarnings.Add("A Quote Reference must be provided")
        End If

        Return oWarnings

    End Function
End Class