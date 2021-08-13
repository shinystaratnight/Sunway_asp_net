Imports ivci = iVectorConnectInterface
Imports Intuitive.Functions
Imports System.Xml
Imports System.Text.RegularExpressions
Imports System.Text
Imports System.Collections.Generic


Namespace Tracking

    Public Class WebsiteTracking


#Region "Private members"

        Private ReadOnly Property CacheKey As String
            Get
                Return "trackingaffiliates_" & BookingBase.Params.BrandID & "_" & BookingBase.Params.CMSWebsiteID
            End Get
        End Property

        Private ReadOnly Property TrackingAffiliates As Generic.List(Of Tracking.TrackingAffiliate)
            Get

                'Get Cache
                Dim oTrackingAffiliates As List(Of Tracking.TrackingAffiliate) =
                    GetCache(Of Generic.List(Of Tracking.TrackingAffiliate))(CacheKey)


                'If Empty Generate
                If oTrackingAffiliates Is Nothing Then

                    oTrackingAffiliates = New Generic.List(Of Tracking.TrackingAffiliate)
                    Dim oTrackingAffiliatesReturn As BookingTrackingAffiliate.GetTrackingAffiliatesReturn =
                        BookingTrackingAffiliate.GetTrackingAffiliates()


                    If Not oTrackingAffiliatesReturn.TrackingAffiliatesXML Is Nothing Then
                        oTrackingAffiliates = Utility.XMLToGenericList(Of Tracking.TrackingAffiliate)(oTrackingAffiliatesReturn.TrackingAffiliatesXML)
                    End If


                    If oTrackingAffiliates.Count > 0 Then
                        oTrackingAffiliates(0).BookingTypeIDs = oTrackingAffiliatesReturn.TrackingAffiliateTypeIDs

                    'add to cache
                    Intuitive.Functions.AddToCache(CacheKey, oTrackingAffiliates, 720)
                    End If

                    'TEMP - Log when we have no tracking to see what was in the response so we can better handle this case.
                    if oTrackingAffiliates.Count = 0 Then
                        Intuitive.FileFunctions.AddLogEntry("Tracking", "Failed Setup", oTrackingAffiliatesReturn.TrackingAffiliatesXML?.InnerXml)
                    End If

                End If

                Return oTrackingAffiliates

            End Get
        End Property


        Private Affiliate As New Tracking.Affiliate
        Private Scripts As New tracking.Scripts


#End Region


#Region "public methods"

        Public Sub Setup(oPageDef As PageDefinition, ByVal RequestURL As String)
            Me.SetupAffiliate(oPageDef, RequestURL)
            Me.SetupScripts(oPageDef, RequestURL)
        End Sub


        Public Sub Render(oHead As Web.Head, oTopBody As Web.TrackingControl, oBottomBody As Web.TrackingControl)


            oHead.AddCustomNode(Me.Scripts.TopHeaderScripts)
            oHead.AddCustomNodeAtBottom(Me.Scripts.BottomHeaderScripts)

            If Not oTopBody Is Nothing Then
                oTopBody.AddCustomNode(Me.Scripts.TopBodyScripts)
            End If

            If Not oBottomBody Is Nothing Then
                oBottomBody.AddCustomNode(Me.Scripts.BottomBodyScripts)
            End If

        End Sub

#End Region

#Region "private methods"

        Private Sub SetupAffiliate(oPageDef As PageDefinition, ByVal RequestURL As String)

            'set up affiliate first
            Dim oAffiliate As New Affiliate


            'always check the query string, as a newer affiliate should replace the old one
            oAffiliate.SetupFromQueryString(RequestURL, Me.TrackingAffiliates)


            If oAffiliate.TrackingAffiliateID > 0 Then
                Me.Affiliate = oAffiliate
            End If


            'always set this, we don't care whether it is from cookie / query string but needs adding to the basket
            BookingBase.Basket.TrackingAffiliateID = oAffiliate.TrackingAffiliateID

        End Sub


        Private Sub SetupScripts(oPageDef As PageDefinition, ByVal RequestURL As String)


            'handle any affiliate if it exists
            Dim oAffiliate As New Tracking.TrackingAffiliate
            If Me.Affiliate.TrackingAffiliateID > 0 Then
                Try

                    oAffiliate = Me.TrackingAffiliates.Where(Function(o) o.Type = "Affiliate" _
                                            And o.TrackingAffiliateID = Me.Affiliate.TrackingAffiliateID).First

                Catch ex As Exception
                    'if an affiliate ID is set it should be in the collection - but this is a failsafe, we don't need to catch this
                End Try
            End If


            Dim oPageTrackingAffiliates As New Generic.List(Of Tracking.TrackingAffiliate)
            oPageTrackingAffiliates.Add(oAffiliate)


            For Each oTrackingAffiliate As Tracking.TrackingAffiliate In Me.TrackingAffiliates

                If Not oTrackingAffiliate.Type = "Affiliate" Then

                    If oTrackingAffiliate.Pages <> "Selected" Then
                        Dim oTA As Tracking.TrackingAffiliate = oTrackingAffiliate.Clone()
                        oPageTrackingAffiliates.Add(oTA)
                    Else
                        For Each oSelectedPages As Tracking.TrackingAffiliate.SelectedPage In oTrackingAffiliate.SelectedPages
                            If oSelectedPages.Page.ToLower = oPageDef.PageName.ToLower Then
                                Dim oTA As Tracking.TrackingAffiliate = oTrackingAffiliate.Clone()
                                oPageTrackingAffiliates.Add(oTA)
                            End If
                        Next
                    End If

                End If

            Next




            ' tracking seperated by position
            Dim oTopHeaderTracking As IEnumerable(Of TrackingAffiliate) = oPageTrackingAffiliates.Where(Function(oTrackingAffiliate) oTrackingAffiliate.Position = "Top Header")
			Dim oBottomHeaderTracking As IEnumerable(Of TrackingAffiliate) = oPageTrackingAffiliates.Where(Function(oTrackingAffiliate) oTrackingAffiliate.Position = "Bottom Header")
			Dim oTopBodyTracking As IEnumerable(Of TrackingAffiliate) = oPageTrackingAffiliates.Where(Function(oTrackingAffiliate) oTrackingAffiliate.Position = "Top Body")
			Dim oBottomBodyTracking As IEnumerable(Of TrackingAffiliate) = oPageTrackingAffiliates.Where(Function(oTrackingAffiliate) oTrackingAffiliate.Position = "Bottom Body" OrElse
			   oTrackingAffiliate.Position = "")

            ' work out script type from page def, and generate token details based on current basket
            Dim sScriptType As String = "landingpage"
            If oPageDef.PageName.ToLower = "confirmation" Then
                sScriptType = "confirmation"
            End If


            ' we need to get hold of the tracking affiliate booking type setting - this is not ideal but prevents us from having to store it seperately
            Dim sBookingTypeIDs As String = ""
            Dim oDefaultTrackingAffiliate As Tracking.TrackingAffiliate = Me.TrackingAffiliates.FirstOrDefault
            If oDefaultTrackingAffiliate IsNot Nothing Then
                sBookingTypeIDs = oDefaultTrackingAffiliate.BookingTypeIDs
            End If
            Dim oTokenDetails As New Tracking.Token
            oTokenDetails.Generate(sBookingTypeIDs)

            'Top Header

            ' detokenize scripts
            oTopHeaderTracking = oTokenDetails.Detokenize(oTopHeaderTracking, sScriptType, oTokenDetails)
            oBottomHeaderTracking = oTokenDetails.Detokenize(oBottomHeaderTracking, sScriptType, oTokenDetails)
            oTopBodyTracking = oTokenDetails.Detokenize(oTopBodyTracking, sScriptType, oTokenDetails)
            oBottomBodyTracking = oTokenDetails.Detokenize(oBottomBodyTracking, sScriptType, oTokenDetails)

            ' add scripts to page
            Dim sTopHeaderScripts As String = Me.GetTrackingScripts(oTopHeaderTracking, sScriptType)
            Dim sBottomHeaderScripts As String = Me.GetTrackingScripts(oBottomHeaderTracking, sScriptType)
            Dim sTopBodyScripts As String = Me.GetTrackingScripts(oTopBodyTracking, sScriptType)
            Dim sBottomBodyScripts As String = Me.GetTrackingScripts(oBottomBodyTracking, sScriptType)

            Me.Scripts = New Tracking.Scripts(sTopHeaderScripts, sBottomHeaderScripts, sTopBodyScripts, sBottomBodyScripts)

        End Sub


        Private Function GetTrackingScripts(ByVal oTrackingAffiliates As IEnumerable(Of Tracking.TrackingAffiliate),
         ByVal sScriptType As String) As String

            Dim sb As New StringBuilder

			For Each oTrackingAffiliate As TrackingAffiliate In oTrackingAffiliates
                'Type = Affiliate
                If oTrackingAffiliate.Type = "Affiliate" AndAlso sScriptType = "confirmation" AndAlso oTrackingAffiliate.ConfirmationScript <> "" Then
					sb.Append(oTrackingAffiliate.ConfirmationScript).Append("\n\n")
				End If
				If oTrackingAffiliate.Type = "Affiliate" AndAlso sScriptType <> "confirmation" AndAlso oTrackingAffiliate.LandingPageScript <> "" Then
					sb.Append(oTrackingAffiliate.LandingPageScript).Append("\n\n")
				End If

                'Type = Tracking
                If oTrackingAffiliate.Type = "Tracking" AndAlso sScriptType = "confirmation" AndAlso oTrackingAffiliate.ConfirmationScript <> "" Then
					sb.Append(oTrackingAffiliate.ConfirmationScript).Append("\n\n")
				End If
				If oTrackingAffiliate.Type = "Tracking" AndAlso sScriptType <> "confirmation" AndAlso oTrackingAffiliate.Script <> "" Then
					sb.Append(oTrackingAffiliate.Script).Append("\n\n")
				End If

                'Type = Advanced Tracking
                If oTrackingAffiliate.Type = "Advanced Tracking" AndAlso oTrackingAffiliate.Secure = 1 AndAlso oTrackingAffiliate.SecureScript <> "" Then
					sb.Append(oTrackingAffiliate.SecureScript).Append("\n\n")
				End If
				If oTrackingAffiliate.Type = "Advanced Tracking" AndAlso oTrackingAffiliate.Secure = 0 AndAlso oTrackingAffiliate.Script <> "" Then
					sb.Append(oTrackingAffiliate.Script).Append("\n\n")
				End If
			Next

			If sb.ToString <> "" Then
                sb.Append("\n\n\n")
            End If

            Return sb.ToString.Replace("\n", ControlChars.NewLine)

        End Function


#End Region




    End Class


End Namespace
