Imports Intuitive
Imports Intuitive.Web
Imports Intuitive.Web.Widgets
Imports System.Xml
Imports System.ComponentModel


Public Class BookingConfirmationPrint
	Inherits WidgetBase

	Public Shared Shadows Property CustomSettings As CustomSetting

		Get
			If HttpContext.Current.Session("bookingConfirmationPrint_customsettings") IsNot Nothing Then
				Return CType(HttpContext.Current.Session("bookingConfirmationPrint_customsettings"), CustomSetting)
			End If
			Return New CustomSetting
		End Get
		Set(value As CustomSetting)
			HttpContext.Current.Session("bookingConfirmationPrint_customsettings") = value
		End Set

	End Property

	Public Overrides Sub Draw(writer As System.Web.UI.HtmlTextWriter)

        SetupCustomSettings()

		Dim oXML As New XmlDocument

		Dim oXSLParams As New Intuitive.WebControls.XSL.XSLParams
		oXSLParams.AddParam("BookingReference", BookingBase.Basket.BookingReference)
		oXSLParams.AddParam("HeaderText", Me.GetSetting(eSetting.HeaderText))
        oXSLParams.AddParam("BookingDocument", Me.GetSetting(eSetting.BookingDocument))
		oXSLParams.AddParam("HideDocuments", Me.GetSetting(eSetting.HideDocuments).ToSafeBoolean())

		Me.XSLTransform(oXML, res.BookingConfirmationPrint, writer, oXSLParams)

	End Sub

    Public Sub SetupCustomSettings()
        Dim oCustomSettings As New CustomSetting
        With oCustomSettings
            .GetDocumentsFromBigC = Intuitive.ToSafeBoolean(Me.GetSetting(eSetting.GetDocumentsFromBigC))
        End With
        BookingConfirmationPrint.CustomSettings = oCustomSettings
    End Sub

#Region "Show Documentation"

	Public Function ShowDocumentation(ByVal sBookingReference As String, ByVal sBookingDocument As String) As String

		'create return object
        Dim oViewDocumentationReturn As New BookingManagement.ViewDocumentationReturn

		'get documentation id
        Dim iDocumentationID As Integer = 0

        If BookingConfirmationPrint.CustomSettings.GetDocumentsFromBigC Then
            iDocumentationID = GetDocumentationID(sBookingDocument)
        Else
            iDocumentationID = Lookups.GetKeyPairID(Intuitive.Web.Lookups.LookupTypes.BookingDocumentation, sBookingDocument)
        End If

		'get documentation from ivector connect if we have an id
		If iDocumentationID > 0 Then
			oViewDocumentationReturn = BookingManagement.ViewBookingDocumentationRequest(sBookingReference, iDocumentationID)
		Else
			oViewDocumentationReturn.OK = False
        End If

		'return
        Return Newtonsoft.Json.JsonConvert.SerializeObject(oViewDocumentationReturn)

    End Function


    Public Shared Function GetDocumentationID(ByVal sDocument As String) As Integer

        Dim iDocumentationID As Integer = 0
        Dim oXML As XmlDocument = Utility.BigCXML("BookingDocumentation", 1, 60)

        If sDocument = "Agent Confirmation" Then
            If BookingBase.Trade.Commissionable Then
                iDocumentationID = XMLFunctions.SafeNodeValue(oXML, "//BookingDocumentation/CommissionableAgentDocumentationID").ToSafeInt
            Else
                iDocumentationID = XMLFunctions.SafeNodeValue(oXML, "//BookingDocumentation/NetAgentDocumentationID").ToSafeInt
            End If
        Else
            iDocumentationID = XMLFunctions.SafeNodeValue(oXML, "//BookingDocumentation/ClientDocumentationID").ToSafeInt
        End If

        If iDocumentationID = 0 Then
            Dim oBookingDocuments As Generic.List(Of BookingDocument) = Utility.XMLToGenericList(Of BookingDocument)(oXML)

            For Each oDocument As BookingDocument In oBookingDocuments
                If oDocument.DocumentName = sDocument Then
                    iDocumentationID = oDocument.BookingDocumentationID
                End If
            Next
        End If

        Return iDocumentationID

    End Function

#End Region


#Region "Settings"

	Public Enum eSetting

		<Title("Header Text")>
		<Description("Heading text for booking confirmation print section")>
		HeaderText

		<Title("Booking Document")>
		<Description("The name of the booking document setup in iVector")>
		BookingDocument

        <Title("Hide Document")>
        <Description("Hide access to booking documents")>
        HideDocuments

        <Title("Get Documents From BigC")>
        <Description("Decides whether we should find the document from BigC or not")>
        GetDocumentsFromBigC

	End Enum

#End Region

    Public Class BookingDocument
        Public BookingDocumentationID As Integer
        Public DocumentName As String
    End Class


    Public Class CustomSetting
        Public GetDocumentsFromBigC As Boolean
    End Class

End Class