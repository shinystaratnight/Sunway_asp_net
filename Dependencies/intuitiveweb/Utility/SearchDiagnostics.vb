Imports System
Imports Intuitive.Functions
Imports System.IO
Imports System.Drawing
Imports System.Web
Imports Intuitive

Public Class SearchDiagnostics

    Private Const constTicksPerSecond As Long = System.TimeSpan.TicksPerSecond

#Region "Email functions"

	Public Shared Sub EmailSearchImage(ByVal Times As ProcessTimer.TimerItems, ByVal EmailAddress As String)

		Try
			Dim sEmailTo As String = String.Empty
			sEmailTo = EmailAddress

			'see if we have any debugging triggers
			Dim oSearchImageRequests As SearchDiagnostics.SearchImageRequests = SearchDiagnostics.CheckForTriggers(Times)

			'build and send email
			If Not sEmailTo = String.Empty Then

				'Check to see if the type of image has been specified
				Dim eImageVerbosity As SearchImage.Verbosity = SearchImage.Verbosity.Summary

				'set the subject
				Dim sSubject As String = String.Format("Search Times Image - {0}", DateTime.Now)

				'add in the querystring driven image request
				oSearchImageRequests.Add(sEmailTo, sSubject, eImageVerbosity)

			End If

			SearchDiagnostics.SendIndividualEmail(oSearchImageRequests, Times)

		Catch

		End Try

	End Sub

	Private Shared Sub SendIndividualEmail(ByVal SearchImageRequests As SearchImageRequests, ByVal Times As ProcessTimer.TimerItems)

		'I was hoping to draw the image once and email multiple times but I am under time pressure. A dictionary keyed on verbosity would do here!

		For Each oImageRequest As SearchImageRequest In SearchImageRequests

			Dim oBitmapImage As Bitmap = DrawImage(Times, oImageRequest.Verbosity)

			Dim sFolderBase As String = FileFunctions.LogPath & Now.ToString("yyMM MMM yy") & "\SearchImage\" & Now.ToString("yyMMdd dd MMM yy")
			Intuitive.FileFunctions.CreateFolder(sFolderBase)
			oBitmapImage.Save(String.Format("{0}\Search Performance - {1}.png", sFolderBase, DateTime.Now.ToString("yyMMdd hhmmssfff")), Imaging.ImageFormat.Png)

			'Send the email
			If oImageRequest.EmailTo <> String.Empty Then
				SendEmail(oImageRequest.EmailTo, oImageRequest.Subject, oBitmapImage)
			End If

			oBitmapImage.Dispose()

		Next

	End Sub

	Private Shared Sub SendEmail(ByVal EmailTo As String, ByVal Subject As String, ByVal Image As Bitmap)

		Dim oEmail As New Email
		Dim oMail As New System.Net.Mail.MailMessage

		Dim oMemoryStreamAttachment As New MemoryStream
		Image.Save(oMemoryStreamAttachment, Imaging.ImageFormat.Png)
		oMemoryStreamAttachment.Seek(0, SeekOrigin.Begin)

		Dim oContentType As New System.Net.Mime.ContentType
		oContentType.Name = String.Format("Search Performance - {0}", DateTime.Now.ToString)
		oContentType.MediaType = System.Net.Mime.MediaTypeNames.Image.Jpeg

		Dim oAttachment As New System.Net.Mail.Attachment(oMemoryStreamAttachment, oContentType)
		oMail.Attachments.Add(oAttachment)

		oMail.Subject = Subject
		oMail.From = New System.Net.Mail.MailAddress("support@intuitivesystems.co.uk", String.Format("{0} - Search Image", Environment.MachineName))
		oMail.To.Add(New System.Net.Mail.MailAddress(EmailTo))

		Dim oClient As System.Net.Mail.SmtpClient

#If DEBUG Then
		oClient = New System.Net.Mail.SmtpClient("sqldev")
#Else
		oClient = New System.Net.Mail.SmtpClient(BookingBase.Params.SMTPHost)
#End If

		'get the image inline
		Dim oMemoryStreamInline As New MemoryStream
		Image.Save(oMemoryStreamInline, Imaging.ImageFormat.Png)
		oMemoryStreamInline.Seek(0, SeekOrigin.Begin)
		Dim sImageID As String = "ImageTag"
		Dim sImageHTML As String = String.Format("<img width=""{0}"" height=""{1}"" src=cid:{2}>", Image.Width, Image.Height, sImageID)
		Dim oHTMLView As System.Net.Mail.AlternateView = System.Net.Mail.AlternateView.CreateAlternateViewFromString(sImageHTML, New System.Net.Mime.ContentType("text/html")) ' add to html as required
		Dim oLinkedResource As New System.Net.Mail.LinkedResource(oMemoryStreamInline)
		oLinkedResource.ContentId = sImageID
		oHTMLView.LinkedResources.Add(oLinkedResource)
		oMail.AlternateViews.Add(oHTMLView)



		'send the email
		oClient.Send(oMail)

		'dispose of the stuff we need
		oMemoryStreamAttachment.Dispose()
		oMemoryStreamAttachment.Close()

		oMemoryStreamInline.Dispose()
		oMemoryStreamInline.Close()

	End Sub

#End Region

#Region "Draw Image"

	Public Shared Function DrawImage(ByVal oTimes As ProcessTimer.TimerItems, _
	   Optional ByVal Verbosity As SearchImage.Verbosity = SearchImage.Verbosity.Summary) As Bitmap

		'create an image item
		Dim oImage As New SearchImage(oTimes, Verbosity)

		oImage.AddApplicationColour("ThirdPartyInterface", Color.Wheat)
		oImage.AddApplicationColour("iVectorBooking", Color.FromArgb(200, 200, 200))

		oImage.AddItemTypeColour(ProcessTimerItemType.General, Color.Gray)
		oImage.AddItemTypeColour(ProcessTimerItemType.Property, Color.DarkGreen)
		oImage.AddItemTypeColour(ProcessTimerItemType.Flight, Color.DarkRed)


		'resize the base graphics objects to accommodate the sizes which we need
		Dim oBaseBitmap As New Bitmap(oImage.CanvasWidth, oImage.CanvasHeight)
		Dim oBaseGraphic As Graphics = Graphics.FromImage(oBaseBitmap)


		'set the quality of the graphics objects
		oBaseGraphic.InterpolationMode = Drawing2D.InterpolationMode.HighQualityBicubic
		oBaseGraphic.SmoothingMode = Drawing2D.SmoothingMode.HighQuality
		oBaseGraphic.TextRenderingHint = Drawing.Text.TextRenderingHint.ClearTypeGridFit
		oBaseGraphic.CompositingQuality = Drawing2D.CompositingQuality.HighQuality


		'Brushes
		Dim oLabelBrush As Brush = Brushes.Black
		Dim oGridBrush As Brush = Brushes.Gray
		Dim oGridPen As New Pen(oGridBrush, 1)
		Dim oErrorEventBrush As Brush = Brushes.PaleVioletRed


		'to ensure that the true type fonts work, we paint a background
		oBaseGraphic.FillRectangle(Brushes.White, New Rectangle(0, 0, oImage.CanvasWidth, oImage.CanvasHeight))


		'Draw the header
		oImage.DrawHeader(oBaseGraphic)


		Dim iMaxSeconds As Integer = oImage.MaxSeconds

		'Order the times by when the event started
		oImage.Sort(Function(a, b) a.StepNumber.CompareTo(b.StepNumber))


		'draw out the labels first
		Dim iTop As Integer = 20 + oImage.HeaderHeight
		Dim i As Integer = 0


		For Each oItem As SearchImageItem In oImage.MainItems

			iTop += 20
			i += 1

			'Line background
			Dim iRectangleLeft As Integer = CType(oImage.StepLabelLeft + oItem.StepLabelSize.Width + 20, Integer)
			Dim iRectangleWidth As Integer = CType(RoundUp(oImage.GridLeft - iRectangleLeft + oImage.GridWidth(30), 0), Integer)
			Dim oWideRectangle As New Rectangle(iRectangleLeft, iTop + 2, iRectangleWidth, 16)

			'set the brish and draw the rectangle
			Dim oTimelineColor As Color = oImage.ApplicationColor(oItem.Application)
			Dim oTimelineBrush As Drawing2D.HatchBrush = TimeLineBrush(oTimelineColor, i)

			'Draw the background line
			oBaseGraphic.FillRectangle(oTimelineBrush, oWideRectangle)


			'draw the labels
			oBaseGraphic.DrawString(oItem.Application, oImage.Font, oLabelBrush, oImage.ApplicationLabelLeft, iTop + 4)
			oBaseGraphic.DrawString(oItem.Label, oImage.Font, oLabelBrush, oImage.StepLabelLeft, iTop + 4)


			'Draw time bars
			If oItem.IsError Then
				oBaseGraphic.FillRectangle(oErrorEventBrush, New Rectangle(oImage.GridLeft + oItem.TimerLeft, iTop + 6, oItem.TimerWidth, 8))
			Else
				Dim oEventBrush As New SolidBrush(oImage.ItemTypeColour(oItem.ItemType))

				'If the length of time was 0 (for markers or quick events) then add a triangle to show where they occured, otherwise draw the square
				If oItem.TimerWidth = 0 Then
					oBaseGraphic.FillPolygon(oEventBrush, {
					 New Point(oImage.GridLeft + oItem.TimerLeft, iTop + 6),
					 New Point(oImage.GridLeft + oItem.TimerLeft - 4, iTop + 14),
					 New Point(oImage.GridLeft + oItem.TimerLeft + 4, iTop + 14)})
				Else
					oBaseGraphic.FillRectangle(oEventBrush, New Rectangle(oImage.GridLeft + oItem.TimerLeft, iTop + 6, oItem.TimerWidth, 8))
				End If
			End If



			'Add labels at the end of step with the time taken 
			oBaseGraphic.DrawString(oItem.TimeTakenLabel, oImage.Font, oLabelBrush,
			 CType(oImage.CanvasWidth - 20 - oBaseGraphic.MeasureString(oItem.TimeTakenLabel, oImage.Font).Width, Single), iTop + 4)


			Dim iThreadLabelLeft As Single = CType(oImage.CanvasWidth - 110 - oBaseGraphic.MeasureString(oItem.ThreadLabel, oImage.Font).Width, Single)
			oBaseGraphic.DrawString(oItem.ThreadLabel, oImage.Font, oLabelBrush, iThreadLabelLeft, iTop + 4)


		Next


		'Draw the second labels and lines
		If oImage.MainItems.Count > 0 Then
			For iSeconds As Integer = 0 To iMaxSeconds
				Dim iLeft As Integer = oImage.GridLeft + (iSeconds * 30)
				oBaseGraphic.DrawLine(oGridPen, iLeft, 20 + oImage.HeaderHeight, iLeft, 50 + oImage.HeaderHeight + (oImage.MainItems.Count * 20))
				oBaseGraphic.DrawString(String.Format("{0}s", iSeconds), oImage.Font,
				   IIf(iSeconds Mod 2 = 0, Brushes.Gray, Brushes.Black), iLeft, 20 + oImage.HeaderHeight)
			Next

			Dim iThreadTitleLeft As Single = CType(oImage.CanvasWidth - 110 - oBaseGraphic.MeasureString("Threads", oImage.Font).Width, Single)
			oBaseGraphic.DrawString("Threads", oImage.Font, oLabelBrush, iThreadTitleLeft, 20 + oImage.HeaderHeight)

		End If


		'Draw a graph for the individual processes
		'Dim oProcesses As IEnumerable(Of String) = (From Time In oTimes Select Time.Value.Process.Split("_"c)(0)).Distinct

		For Each sThirdParty As String In oImage.ThirdParties

			If sThirdParty = "Main" Then Continue For

			Dim process As String = sThirdParty

			Dim oThirdPartyImageItems As List(Of SearchImageItem) = oImage.ThirdPartyItems.Where( _
			 Function(o) o.Process.Split("_"c)(0) = process).ToList

			If oThirdPartyImageItems.Count = 0 Then Continue For

			iTop += 50
			Dim iTopForLabels As Integer = iTop	'+ 16

			For Each oItem As SearchImageItem In oThirdPartyImageItems

				iTop += 20
				i += 1

				'Line background
				Dim iRectangleLeft As Integer = CType(oImage.StepLabelLeft + oItem.StepLabelSize.Width + 20, Integer)
				Dim iRectangleWidth As Integer = CType(RoundUp(oImage.GridLeft - iRectangleLeft + oImage.GridWidth(30), 0), Integer)
				Dim oWideRectangle As New Rectangle(iRectangleLeft, iTop + 2, iRectangleWidth, 16)

				'set the brish and draw the rectangle
				Dim oTimelineColor As Color = oImage.ApplicationColor(oItem.Application)
				Dim oTimelineBrush As Drawing2D.HatchBrush = TimeLineBrush(oTimelineColor, i)

				'Draw the background line
				oBaseGraphic.FillRectangle(oTimelineBrush, oWideRectangle)


				'draw the labels
				oBaseGraphic.DrawString(oItem.Application, oImage.Font, oLabelBrush, oImage.ApplicationLabelLeft, iTop + 4)
				oBaseGraphic.DrawString(oItem.Label, oImage.Font, oLabelBrush, oImage.StepLabelLeft, iTop + 4)


				'Draw time bars
				If oItem.IsError Then
					oBaseGraphic.FillRectangle(oErrorEventBrush, New Rectangle(oImage.GridLeft + oItem.TimerLeft, iTop + 6, oItem.TimerWidth, 8))
				Else
					Dim oEventColor As Color = oImage.ItemTypeColour(oItem.ItemType)
					Dim oEventBrush As New SolidBrush(oEventColor)
					Dim iColourAlteration As Integer = 20
					Dim oEventHashBrush As Drawing2D.HatchBrush = EventTimeBrush(oEventColor)

					'If the length of time was 0 (for markers or quick events) then add a triangle to show where they occured, otherwise draw the square
					If oItem.TimerWidth = 0 Then
						If oItem.Level > 0 Then
							oBaseGraphic.FillPolygon(oEventHashBrush, {
							  New Point(oImage.GridLeft + oItem.TimerLeft, iTop + 6),
							  New Point(oImage.GridLeft + oItem.TimerLeft - 4, iTop + 14),
							  New Point(oImage.GridLeft + oItem.TimerLeft + 4, iTop + 14)})
						Else
							oBaseGraphic.FillPolygon(oEventBrush, {
							  New Point(oImage.GridLeft + oItem.TimerLeft, iTop + 6),
							  New Point(oImage.GridLeft + oItem.TimerLeft - 4, iTop + 14),
							  New Point(oImage.GridLeft + oItem.TimerLeft + 4, iTop + 14)})
						End If
					Else
						If oItem.Level > 0 Then
							oBaseGraphic.FillRectangle(oEventHashBrush, New Rectangle(oImage.GridLeft + oItem.TimerLeft, iTop + 6, oItem.TimerWidth, 8))
						Else
							oBaseGraphic.FillRectangle(oEventBrush, New Rectangle(oImage.GridLeft + oItem.TimerLeft, iTop + 6, oItem.TimerWidth, 8))
						End If

					End If
				End If



				'Add labels at the end of step with the time taken 
				oBaseGraphic.DrawString(oItem.TimeTakenLabel, oImage.Font, oLabelBrush,
				 CType(oImage.CanvasWidth - 20 - oBaseGraphic.MeasureString(oItem.TimeTakenLabel, oImage.Font).Width, Single), iTop + 4)

				Dim iThreadLabelLeft As Single = CType(oImage.CanvasWidth - 110 - oBaseGraphic.MeasureString(oItem.ThreadLabel, oImage.Font).Width, Single)
				oBaseGraphic.DrawString(oItem.ThreadLabel, oImage.Font, oLabelBrush, iThreadLabelLeft, iTop + 4)


			Next


			'Header
			Dim oThirdPartyBrush As Brush = Brushes.Gray
			Dim oThirdPartyFamily As New FontFamily("Verdana")
			Dim oThirdPartyFont As New Font(oThirdPartyFamily, 11, FontStyle.Bold, GraphicsUnit.Pixel)
			oBaseGraphic.DrawString(sThirdParty, oThirdPartyFont, oThirdPartyBrush, oImage.ApplicationLabelLeft, iTopForLabels)

			'Second headers
			For iSeconds As Integer = 0 To iMaxSeconds
				Dim iLeft As Integer = oImage.GridLeft + (iSeconds * 30)
				oBaseGraphic.DrawLine(oGridPen, iLeft, iTopForLabels, iLeft, iTopForLabels + 20 + (oThirdPartyImageItems.Count * 20))
				oBaseGraphic.DrawString(String.Format("{0}s", iSeconds), oImage.Font,
				   IIf(iSeconds Mod 2 = 0, Brushes.Gray, Brushes.Black), iLeft, iTopForLabels)

			Next

		Next

		oBaseGraphic.Dispose()

		Return oBaseBitmap

	End Function

#End Region

#Region "Brushes functions"

	Private Shared Function EventTimeBrush(ByRef EventColour As Color) As Drawing2D.HatchBrush

		Dim iColourAlteration As Integer = 100
		Dim iRed As Integer = Math.Min(255, EventColour.R + iColourAlteration)
		Dim iGreen As Integer = Math.Min(255, EventColour.G + iColourAlteration)
		Dim iBlue As Integer = Math.Min(255, EventColour.B + iColourAlteration)

		Return New Drawing2D.HatchBrush(Drawing2D.HatchStyle.NarrowVertical, EventColour, Color.FromArgb(iRed, iGreen, iBlue))

	End Function

	Private Shared Function TimeLineBrush(ByRef Colour As Color, ByVal PositionIndex As Integer) As Drawing2D.HatchBrush

		'For alternating the colours every other row
		Dim oHatchBrush As New System.Drawing.Drawing2D.HatchBrush( _
		   Drawing2D.HatchStyle.BackwardDiagonal, Color.White, Colour)

		If PositionIndex Mod 2 = 0 Then
			Dim iColourAlteration As Integer = 20
			oHatchBrush = New Drawing2D.HatchBrush(Drawing2D.HatchStyle.BackwardDiagonal, Color.White,
			   Color.FromArgb(oHatchBrush.BackgroundColor.R - iColourAlteration,
			   oHatchBrush.BackgroundColor.G - iColourAlteration, oHatchBrush.BackgroundColor.B - iColourAlteration))
		End If

		Return oHatchBrush

	End Function

#End Region

#Region "Check For Triggers"

	Public Shared Function CheckForTriggers(ByVal Times As ProcessTimer.TimerItems) As SearchDiagnostics.SearchImageRequests

		Dim oReturn As SearchImageRequests = New SearchImageRequests

		'hard coded for a specific bug
		Try

			Dim iTotalTime As Double = Times.Max(Function(o) o.Value.EndTicks - o.Value.StartTicks) / constTicksPerSecond
			Dim oFlightTimes As List(Of KeyValuePair(Of String, ProcessTimer.TimerItem)) = Times.Where(Function(o) o.Value.Step.StartsWith("FlightSearchRequest")).ToList
			If oFlightTimes.Count > 0 Then

				'get the maximum 3rd party flight times
				Dim iFlightRequestTime As Double = oFlightTimes.Max(Function(o) o.Value.EndTicks - o.Value.StartTicks) / constTicksPerSecond

				If iTotalTime > 10 AndAlso iFlightRequestTime < 5 Then

					Dim oResponse As New SearchImageRequest
                    oResponse.EmailTo = "adam@intuitivesystems.co.uk"
					oResponse.Subject = "good flight provider response - bad total time"
					oReturn.Add(oResponse)

				End If

			End If

			Dim oWaitForThreadsThread As List(Of KeyValuePair(Of String, ProcessTimer.TimerItem)) = Times.Where(Function(o) o.Value.Step.ToLower.StartsWith("waitforthreads")).ToList
			Dim oFlightBaseSearch As List(Of KeyValuePair(Of String, ProcessTimer.TimerItem)) = Times.Where(Function(o) o.Value.Step.ToLower.StartsWith("flightsearch")).ToList
			If oWaitForThreadsThread.Count > 0 AndAlso oFlightBaseSearch.Count > 0 Then

				'get the maximum 3rd party flight times
				Dim iFirstBaseTime As Double = oFlightBaseSearch.Min(Function(o) o.Value.StartTicks)
				Dim iLastWaitTime As Double = oWaitForThreadsThread.Max(Function(o) o.Value.StartTicks)
				Dim iMissingTime As Double = (iLastWaitTime - iFirstBaseTime) / constTicksPerSecond

				If iMissingTime > 2.0 Then

					Dim oResponse As New SearchImageRequest
                    oResponse.EmailTo = "adam@intuitivesystems.co.uk"
					oResponse.Subject = "wait between thread creation and thread start"
					oReturn.Add(oResponse)

				End If

			End If


		Catch ex As Exception

		End Try

		Return oReturn

	End Function


#End Region

#Region "Support Classes"

	Public Class SearchImageRequests
		Inherits List(Of SearchImageRequest)

		Public Overloads Sub Add(ByVal EmailTo As String, ByVal Subject As String, ByVal Verbosity As SearchImage.Verbosity)
			Dim oRequest As New SearchImageRequest(EmailTo, Subject, Verbosity)
			Me.Add(oRequest)
		End Sub

	End Class

	Public Class SearchImageRequest

		Public EmailTo As String = String.Empty
		Public Subject As String = String.Empty
		Public Verbosity As SearchImage.Verbosity = SearchImage.Verbosity.Summary

		Public Sub New()

		End Sub

		Public Sub New(ByVal EmailTo As String, ByVal Subject As String, ByVal Verbosity As SearchImage.Verbosity)
			Me.EmailTo = EmailTo
			Me.Subject = Subject
			Me.Verbosity = Verbosity
		End Sub

	End Class

#End Region

End Class

#Region "Search Image"


Public Class SearchImage
	Inherits List(Of SearchImageItem)

	Public Font As Font
	Public StepLabelLeft As Integer = 0
	Public ApplicationLabelLeft As Integer = 0
	Public GridLeft As Integer = 0
	Public GridRight As Integer = 0
	Public ImageBoardWidth As Integer = 20
	Public ApplicationColours As New Dictionary(Of String, Color)
	Public ItemTypeColours As New Dictionary(Of ProcessTimerItemType, Color)
	Private TicksOffset As Long = 0
	Public ThirdParties As List(Of String) = New List(Of String)

	Public Sub New(ByVal Times As ProcessTimer.TimerItems, Optional ByVal Verbosity As Verbosity = Verbosity.Summary)

		For Each sItemKey As String In Times.Keys

			Dim oItem As ProcessTimer.TimerItem = Times(sItemKey)

			Dim oSearchImageItem As New SearchImageItem
			With oSearchImageItem

				.RenderType = Me.Display(oItem, Verbosity)
				.StepNumber = oItem.StepNumber
				.Level = oItem.Level

				'label and application
				.Application = oItem.hlpApplicationName
				.Label = oItem.Step
				If oItem.Process <> String.Empty Then
					.Label += " - " & oItem.Process
					.Process = oItem.Process
				End If

				.ItemType = oItem.ItemType
				.TicksStart = oItem.StartTicks
				.TicksEnd = oItem.EndTicks
				.SetupThreadDetails(oItem.MaxThreadCount, oItem.MinThreadCount, oItem.AvailableThreadCount)

			End With

			Me.Add(oSearchImageItem)

		Next


		'adjust the ticks to remove the minimum to offset to zero
		Dim iMinTicks As Long = Me.Min(Function(o) o.TicksStart)
		Dim iMaxTicks As Long = Me.Max(Function(o) o.TicksEnd)

		For Each oItem As SearchImageItem In Me
			oItem.TicksStart -= iMinTicks
			If oItem.TicksEnd = 0 Then
				oItem.TicksEnd = iMaxTicks
				oItem.IsError = True
			End If
			oItem.TicksEnd -= iMinTicks
		Next

		Me.TicksOffset = iMinTicks

		'set the font
		Dim oFontFamily As New FontFamily("Verdana")
		Me.Font = New Font(oFontFamily, 11, FontStyle.Regular, GraphicsUnit.Pixel)

		'sort out how large the labels are based on the font
		Me.SetLabelSizes()

		'store the third parties
		Me.ThirdParties = (From Time In Times Select Time.Value.Process.Split("_"c)(0)).Distinct.ToList


	End Sub

	Private Sub SetLabelSizes()

		'set up a base set of grapihcs objects which we will use to measure fonts and resize later
		Dim oBaseBitmap As New Bitmap(1, 1)
		Dim oBaseGraphic As Graphics = Graphics.FromImage(oBaseBitmap)

		'get the maximum size of the labels which we are using for "label" and "application"
		Dim iMaxLabelWidth As Integer = 0
		Dim iMaxApplicationWidth As Integer = 0
		Dim iMaxTimeLabelWidth As Integer = 0
		Dim iMaxThreadLabelWidth As Integer = 0

		For Each oItem As SearchImageItem In Me

			oItem.ApplicationLabelSize = oBaseGraphic.MeasureString(oItem.Application, Me.Font)
			oItem.StepLabelSize = oBaseGraphic.MeasureString(oItem.Label, Me.Font)

			If oItem.ApplicationLabelSize.Width > iMaxApplicationWidth Then iMaxApplicationWidth = CType(oItem.ApplicationLabelSize.Width, Integer)
			If oItem.StepLabelSize.Width > iMaxLabelWidth Then iMaxLabelWidth = CType(oItem.StepLabelSize.Width, Integer)

			'Work out the size of the time taken label for the right hand side of the graph
			Dim iTimeTakenLength As Integer = oBaseGraphic.MeasureString(oItem.TimeTakenLabel, Me.Font).Width.ToSafeInt

			If iTimeTakenLength > iMaxTimeLabelWidth Then iMaxTimeLabelWidth = iTimeTakenLength


			'Work out the size of the thread label on the right hand size of the graph
			Dim iThreadLabelWidth As Integer = oBaseGraphic.MeasureString(oItem.ThreadLabel, Me.Font).Width.ToSafeInt
			If iThreadLabelWidth > iMaxThreadLabelWidth Then iMaxThreadLabelWidth = iThreadLabelWidth

		Next

		'Work out the size of the thread label on the right hand size of the graph
		Dim iThreadTitleWidth As Integer = oBaseGraphic.MeasureString("Threads", Me.Font).Width.ToSafeInt
		If iThreadTitleWidth > iMaxThreadLabelWidth Then iMaxThreadLabelWidth = iThreadTitleWidth

		Me.ApplicationLabelLeft = 40
		Me.StepLabelLeft = Me.ApplicationLabelLeft + iMaxApplicationWidth + 40
		Me.GridLeft = Me.StepLabelLeft + iMaxLabelWidth + 40
		Me.GridRight = 2 + iMaxTimeLabelWidth + 5 + iMaxThreadLabelWidth ' padding + time label + padding + thread label

	End Sub

	Public Sub AddApplicationColour(ByVal Application As String, ByVal ApplicationColour As Color)

		If Me.ApplicationColours.Keys.Contains(Application) Then Return
		Me.ApplicationColours.Add(Application, ApplicationColour)

	End Sub

	Public ReadOnly Property ApplicationColor(ByVal Application As String) As Color
		Get
			If Not Me.ApplicationColours.Keys.Contains(Application) Then Return Color.WhiteSmoke
			Return Me.ApplicationColours(Application)
		End Get
	End Property

	Public Sub AddItemTypeColour(ByVal ItemType As ProcessTimerItemType, ByVal ApplicationColour As Color)

		If Me.ItemTypeColours.Keys.Contains(ItemType) Then Return
		Me.ItemTypeColours.Add(ItemType, ApplicationColour)

	End Sub

	Public ReadOnly Property ItemTypeColour(ByVal ItemType As ProcessTimerItemType) As Color
		Get
			If Not Me.ItemTypeColours.Keys.Contains(ItemType) Then Return Color.Crimson
			Return Me.ItemTypeColours(ItemType)
		End Get
	End Property

	Public ReadOnly Property CanvasWidth As Integer
		Get
			Return Math.Max(Me.GridLeft + ((Me.MaxSeconds + 1) * 30) + 40 + Me.GridRight, 500)
		End Get
	End Property

	Public ReadOnly Property MainItems As List(Of SearchImageItem)
		Get
			Return Me.Where(Function(o) o.RenderType = SearchImageItem.enumRenderType.All OrElse o.RenderType = SearchImageItem.enumRenderType.MainOnly).ToList
		End Get
	End Property

	Public ReadOnly Property ThirdPartyItems As List(Of SearchImageItem)
		Get
			Return Me.Where(Function(o) o.RenderType = SearchImageItem.enumRenderType.All OrElse o.RenderType = SearchImageItem.enumRenderType.ThirdPartyOnly).ToList
		End Get
	End Property

	Public ReadOnly Property CanvasHeight As Integer
		Get
			'start with the items which we are going to display in the main list
			Dim iHeight As Integer = (Me.MainItems.Count * 20) + 20 + 150 + Me.HeaderHeight

			'space for the headers for each third party
			iHeight += (Me.ThirdParties.Count - 1) * 50

			'add in each third party row
			iHeight += Me.ThirdPartyItems.Count * 20
			'add in a section for all the third parties

			Return iHeight

		End Get
	End Property

#Region "Header"

	Public ReadOnly Property HeaderHeight As Integer
		Get
			Return 24 * 4 + 2 'Line height * number of items in the header + some space
		End Get
	End Property

	Friend Sub DrawHeader(ByRef GraphicObject As Graphics)

		Dim dStartDate As Date = New Date(Me.Min(Function(o) o.TicksStart) + Me.TicksOffset)
		Dim dEndDate As Date = New Date(Me.Max(Function(o) o.TicksEnd) + Me.TicksOffset)

		Dim iMinThreads As Integer = Me.Min(Function(o) o.MinThreads)
		Dim iMaxThreads As Integer = Me.Max(Function(o) o.MaxThreads)


		Dim oHeaderWriter As New HeaderWriter(Me.ApplicationLabelLeft, GraphicObject, Me.Font)

		oHeaderWriter.AppendLine("Search Representation")
		oHeaderWriter.AppendLine("Start Time - {0}:{1}", dEndDate.ToString, dEndDate.Millisecond.ToString)
		oHeaderWriter.AppendLine("End Time - {0}:{1}", dEndDate.ToString, dEndDate.Millisecond.ToString)

		oHeaderWriter.AppendLine("Machine Name - {0}", Environment.MachineName)

		'Search information
		oHeaderWriter.AppendLine("Min Threads - {0}", iMinThreads.ToString)
		oHeaderWriter.AppendLine("Max Threads - {0}", iMaxThreads.ToString)
		oHeaderWriter.Flush() ' Write out any remaining text

	End Sub

	Private Class HeaderWriter

		Private sb As New System.Text.StringBuilder
		Private LeftOffset As Integer
		Private CurrentLines As Integer = 0
		Private Graphics As Graphics
		Private Font As Font

		Public Sub New(ByVal LeftOffset As Integer, ByVal Graphics As Graphics, ByVal Font As Font)
			Me.LeftOffset = LeftOffset
			Me.Graphics = Graphics
			Me.Font = Font
		End Sub

		Public Sub AppendLine(ByVal s As String, ByVal ParamArray Values As String())

			Me.sb.AppendLine(String.Format(s, Values))

			'Only draw three lines at a time
			Me.CurrentLines += 1
			If Me.CurrentLines = 4 Then

				Me.Graphics.DrawString(sb.ToString, Me.Font, Brushes.Black, Me.LeftOffset, 34)
				Me.LeftOffset += Graphics.MeasureString(sb.ToString, Me.Font).Width.ToSafeInt + 15
				Me.sb.Clear()
				Me.CurrentLines = 0

			End If

		End Sub

		Public Sub Flush()
			Me.Graphics.DrawString(sb.ToString, Me.Font, Brushes.Black, Me.LeftOffset, 34)
		End Sub

	End Class

#End Region

	Public ReadOnly Property MaxSeconds As Integer
		Get
			Dim iMaxTicks As Decimal = Me.Max(Function(o) o.TicksEnd)
			Dim iTicksPerSeconds As Decimal = CType(10 ^ 7, Decimal)
			Return CType(RoundDown(iMaxTicks / iTicksPerSeconds, 0), Integer)
		End Get
	End Property

	Public ReadOnly Property GridWidth(ByVal SecondWidth As Integer) As Decimal
		Get
			Dim iMaxTicks As Decimal = Me.Max(Function(o) o.TicksEnd)
			Dim iTicksPerSeconds As Decimal = CType(10 ^ 7, Decimal)
			Return SecondWidth * (iMaxTicks) / iTicksPerSeconds
		End Get
	End Property

	Private Function Display(ByVal TimerItem As ProcessTimer.TimerItem,
	  ByVal Verbosity As Verbosity,
	  Optional ByVal Process As String = "Main") As SearchImageItem.enumRenderType

		Dim bDisplay As SearchImageItem.enumRenderType = SearchImageItem.enumRenderType.None

		Select Case Verbosity
			Case SearchImage.Verbosity.Full	' Return everything
				bDisplay = SearchImageItem.enumRenderType.All

			Case SearchImage.Verbosity.Summary ' Exclude all the additional third party info apart from when it started and stopped

				If TimerItem.Process.ToLower = "main" Then
					bDisplay = SearchImageItem.enumRenderType.MainOnly
				ElseIf TimerItem.hlpApplicationName.ToLower = "ivectorbooking" Then
					bDisplay = SearchImageItem.enumRenderType.MainOnly

				Else

					Select Case TimerItem.Step

						Case ProcessTimerStep.ThreadedSearch, _
						  Intuitive.ProcessTimerStep.SQLSearch, _
						  Intuitive.ProcessTimerStep.ThreadCancelled, _
						  Intuitive.ProcessTimerStep.ProcessingResponseException
							bDisplay = SearchImageItem.enumRenderType.All

						Case Else
							bDisplay = SearchImageItem.enumRenderType.ThirdPartyOnly

					End Select

				End If

			Case SearchImage.Verbosity.Process

				'I might have broken this....
				If TimerItem.Process Like Process & "*" Then ' use like as the third parties have some numerics after the process name for multi threads
					bDisplay = SearchImageItem.enumRenderType.All
				Else
					bDisplay = SearchImageItem.enumRenderType.None
				End If

		End Select

		Return bDisplay

	End Function

	Public Enum Verbosity
		Summary
		Full
		Process
	End Enum

End Class
#End Region

#Region "Search Image Item"

Public Class SearchImageItem

	Public StepNumber As Integer = 0
	Public Label As String = String.Empty
	Public Level As Integer = 0
	Public Application As String = String.Empty
	Public ItemType As ProcessTimerItemType
	Public Process As String = String.Empty
	Private sThreadLabel As String = String.Empty

	Public TicksStart As Long = 0
	Public TicksEnd As Long = 0

	Public ApplicationLabelSize As New SizeF
	Public StepLabelSize As New SizeF
	Public IsError As Boolean = False
	Public RenderType As enumRenderType = enumRenderType.All

	Public MinThreads As Integer = 0
	Public MaxThreads As Integer = 0

	Public Enum enumRenderType
		All
		MainOnly
		ThirdPartyOnly
		None
	End Enum


	Public ReadOnly Property TimerLeft As Integer
		Get
			Return CType(Me.TicksStart / 10 ^ 7 * 30, Integer)
		End Get
	End Property

	Public ReadOnly Property TimerWidth As Integer
		Get
			Return CType((Me.TicksEnd - Me.TicksStart) / 10 ^ 7 * 30, Integer)
		End Get
	End Property

	Public ReadOnly Property TimeTakenLabel As String
		Get
			Dim iTimeTaken As Integer = Math.Max((Me.TicksEnd - Me.TicksStart).ToSafeInt, 0)
			iTimeTaken = RoundUp(SafeDecimal(iTimeTaken / 10 ^ 4), 0).ToSafeInt
			Return iTimeTaken.ToString("###,##0") & " ms"
		End Get
	End Property

	Public ReadOnly Property ThreadLabel As String
		Get
			Return sThreadLabel
		End Get
	End Property

	Public Sub SetupThreadDetails(ByVal MaxThreadCount As Integer, ByVal MinThreadCount As Integer, ByVal AvailableThreadCount As Integer)
		'Me.sThreadLabel = String.Format("{0}, {1}, {2}", MaxThreadCount, MinThreadCount, AvailableThreadCount)
		Me.sThreadLabel = AvailableThreadCount.ToString
		Me.MinThreads = MinThreadCount
		Me.MaxThreads = MaxThreadCount
	End Sub

End Class

#End Region


Public Enum ProcessTimeSearchType
	CentralisedSearch
	PropertySplitSearch
	FlightSplitSearch
End Enum

