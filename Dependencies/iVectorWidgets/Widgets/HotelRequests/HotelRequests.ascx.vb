Imports Intuitive.Web
Imports Intuitive.Web.Widgets
Imports System.Xml

Public Class HotelRequestsControl
	Inherits UserControlBase

	Public Overrides Sub ApplySettings(ByVal Settings As PageDefinition.WidgetSettings)

		'get settings
		Dim sTitle As String = Intuitive.Functions.SafeString(Settings.GetValue("Title"))
		Dim sText As String = Intuitive.Functions.SafeString(Settings.GetValue("Text"))
        Dim sTextboxPlaceholder As String = Intuitive.Functions.SafeString(Settings.GetValue("TextboxPlaceholder"))
		Dim sClassOverride As String = Intuitive.Functions.SafeString(Settings.GetValue("CSSClassOverride"))
		Dim bArrivalTime As Boolean = Intuitive.Functions.SafeBoolean(Settings.GetValue("ArrivalTime"))

		'set values of elements if settings exist
		If Not sTitle = "" Then
			Me.hHotelRequests_Title.InnerHtml = sTitle
		End If

		If Not sText = "" Then
			Me.pHotelRequests_Text.InnerHtml = sText
		End If

		If Not sTextboxPlaceholder = "" Then
			Me.txtHotelRequests_Requests.Attributes.Add("placeholder", sTextboxPlaceholder)
        End If

        If Not sClassOverride = "" Then
            Me.divHotelRequests.Attributes("class") = sClassOverride
		End If

		If bArrivalTime Then
			Me.divArrivalTime.Visible = True

			'Dim dDateStart As DateAndTime()

			Dim sb As New StringBuilder
			Dim dStartTime As New DateTime(1, 1, 1, 0, 0, 0)
			Dim dEndTime As New DateTime(1, 1, 1, 23, 59, 0)
			Dim dCurrentTime As New DateTime(1, 1, 1, 0, 0, 0)

			While (dCurrentTime <= dEndTime)
				sb.Append(Format(dCurrentTime, "HH:mm"))
				dCurrentTime = DateAdd(DateInterval.Minute, 15, dCurrentTime)
				sb.Append("#")
			End While

			Me.ddlArrivalTime.Options = sb.ToString
		End If

	End Sub

End Class