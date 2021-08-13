Imports System.Xml
Imports ivci = iVectorConnectInterface

Public Class BookingTrackingAffiliate

#Region "Get Tracking Affiliates"

    Public Shared Function GetTrackingAffiliates() As GetTrackingAffiliatesReturn

        Dim oGetTrackingAffiliatesReturn As New GetTrackingAffiliatesReturn
        Dim oGetTrackingAffiliatesRequest As New iVectorConnectInterface.GetTrackingAffiliatesRequest
        Dim oTrackingAffiliateXML As New XmlDocument

        Try

            'Add the login details 
            oGetTrackingAffiliatesRequest.LoginDetails = BookingBase.IVCLoginDetails

            If  String.IsNullOrEmpty(oGetTrackingAffiliatesRequest.LoginDetails.AgentReference) Then
                oGetTrackingAffiliatesRequest.LoginDetails.AgentReference = BookingBase.Params.OverrideAgentReference
            end If

            oGetTrackingAffiliatesRequest.BrandID = BookingBase.Params.BrandID
            oGetTrackingAffiliatesRequest.CMSWebsiteID = BookingBase.Params.CMSWebsiteID


            'Send request to iVectorConnect
            Dim oIVCReturn As Utility.iVectorConnect.iVectorConnectReturn = _
            Utility.iVectorConnect.SendRequest(Of ivci.GetTrackingAffiliatesResponse)(oGetTrackingAffiliatesRequest)


            If oIVCReturn.Success Then
                Dim oGetTrackingAffiliatesResponse As ivci.GetTrackingAffiliatesResponse = CType(oIVCReturn.ReturnObject, 
                    iVectorConnectInterface.GetTrackingAffiliatesResponse)
                oGetTrackingAffiliatesReturn.TrackingAffiliatesXML = Intuitive.Serializer.Serialize(oGetTrackingAffiliatesResponse.TrackingAffiliates, True)
                oGetTrackingAffiliatesReturn.TrackingAffiliateTypeIDs = oGetTrackingAffiliatesResponse.TrackingAffiliateTypeIDs
            Else
                oGetTrackingAffiliatesReturn.OK = False
            End If


        Catch ex As Exception

            oGetTrackingAffiliatesReturn.OK = False
            oGetTrackingAffiliatesReturn.Warnings.Add(ex.ToString)

        End Try


        Return oGetTrackingAffiliatesReturn

    End Function

#End Region


#Region "Return class"

    Public Class GetTrackingAffiliatesReturn

        Property OK As Boolean = True
        Property Warnings As New Generic.List(Of String)
        Property TrackingAffiliatesXML As XmlDocument
        Property TrackingAffiliateTypeIDs As String

    End Class

#End Region

End Class


