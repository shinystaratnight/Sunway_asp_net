Imports System.IO
Imports System.Xml

Public Class Overbranding

	Public ReadOnly Property OverbandingCollection() As Generic.List(Of Overbrand)

		Get
			'Check if we havne't cached it already cache it
			If HttpContext.Current.Session("OverBranding_Collection") Is Nothing Then

				'Get the overbranding XML
				Dim oOverbrandingXML As System.Xml.XmlDocument = Utility.BigCXML("Overbranding", 1, 1440, 0, 0)

				'Deserialize it
				HttpContext.Current.Session("OverBranding_Collection") = Utility.XMLToGenericList(Of Overbrand)(oOverbrandingXML)

			End If

			Return CType(HttpContext.Current.Session("OverBranding_Collection"), Generic.List(Of Overbrand))

		End Get

	End Property
	Public Property SelectedOverbranding As Overbrand
	Public Property isOverbranded As Boolean = False

	Public Function SelectOverbrand(ByVal sURL As String) As Overbrand

		Dim oSelectedOverbranding As New Overbrand

        'Loop through the list of overbrandings we got from BigC, compare our url with these and return the correct object
        For Each oOverbranding As Overbrand In Me.OverbandingCollection.Where(Function(o) o.URL.ToLower = sURL.ToLower OrElse o.URL.ToLower = sURL.Replace("https", "http").ToLower())
            SelectedOverbranding = oOverbranding
            Me.isOverbranded = True
            BookingBase.iVectorConnectLoginOverride = oOverbranding.Login
            BookingBase.iVectorConnectPasswordOverride = oOverbranding.Password
        Next

        Return SelectedOverbranding

	End Function

	Public Function BuildCSS(ByVal Path As String, ByVal FileName As String) As String

		Dim sBaseFile As String = Path & FileName & ".css"

		If Me.isOverbranded Then

			'1.  overbranding key and directory info
			Dim sOverbrandingKey As String = FileName & Me.SelectedOverbranding.OverbrandIdentifier
			Dim sFolderPath As String = "/themes/overbranding" & Path
			Dim sFilePath As String = sFolderPath & sOverbrandingKey & ".css"
			Dim sCSS As String

			'2 use file path as cache key, if this is stored then just return it, if not build the CSS again 
			If HttpContext.Current.Cache(sFilePath) IsNot Nothing Then
				Return Functions.SafeString(HttpContext.Current.Cache(sFilePath))
			Else
				Dim sFontFamilyHeader As String = Me.SelectedOverbranding.FontFamilyHeaderName
				Dim sFontFamily As String = Me.SelectedOverbranding.FontFamilyName

				If sFontFamilyHeader <> "" Then
					sFontFamilyHeader += ","
				End If

				If sFontFamily <> "" Then
					sFontFamily += ","
				End If

				sCSS = File.ReadAllText(HttpContext.Current.Server.MapPath(sBaseFile))

				sCSS = sCSS.Replace("@Primary", "#" & Me.SelectedOverbranding.PrimaryColour)
				sCSS = sCSS.Replace("@Secondary", "#" & Me.SelectedOverbranding.SecondaryColour)
				sCSS = sCSS.Replace("@Tertiary", "#" & Me.SelectedOverbranding.TertiaryColour)
				sCSS = sCSS.Replace("@Background", "#" & Me.SelectedOverbranding.Background)
				sCSS = sCSS.Replace("@Foreground", "#" & Me.SelectedOverbranding.Foreground)
				sCSS = sCSS.Replace("@FooterBackground", "#" & Me.SelectedOverbranding.FooterBackground)
				sCSS = sCSS.Replace("@Border", "#" & Me.SelectedOverbranding.Border)
				sCSS = sCSS.Replace("@NavigationPrimary", "#" & Me.SelectedOverbranding.NavigationPrimary)
				sCSS = sCSS.Replace("@NavigationSecondary", "#" & Me.SelectedOverbranding.NavigationSecondary)
				sCSS = sCSS.Replace("@NavigationTertiary", "#" & Me.SelectedOverbranding.NavigationTertiary)
				sCSS = sCSS.Replace("@FontFamilyImportURLs", Me.SelectedOverbranding.FontFamilyImportURLs)
				sCSS = sCSS.Replace("@FontFamilyHeader", sFontFamilyHeader)
				sCSS = sCSS.Replace("@FontFamily", sFontFamily)

				'3. Cache for an hour
				If Not Functions.IsDebugging Then Intuitive.Functions.AddToCache(sFilePath, sFilePath, 1440)


				'2 Create folder if it doesnt exist
				Intuitive.FileFunctions.CreateFolder(HttpContext.Current.Server.MapPath(sFolderPath))

				'Generate CSS file
				File.WriteAllText(HttpContext.Current.Server.MapPath(sFilePath), sCSS)

				'return file
				Return sFilePath

			End If

		Else
			Return sBaseFile
		End If

	End Function

	Public Class Overbrand

		Public Name As String
		Public Trade As Integer
		Public Brand As Integer
		Public Login As String
		Public Password As String
		Public URL As String
		Public Logo As String
		Public PrimaryColour As String
		Public SecondaryColour As String
		Public TertiaryColour As String
		Public Background As String
		Public Foreground As String
		Public FooterBackground As String
		Public Border As String
		Public NavigationPrimary As String
		Public NavigationSecondary As String
		Public ABTAATOLNumber As String
		Public TelephoneNumber As String
		Public Domain As String
		Public NavigationTertiary As String
		Public FontFamily As String
		Public FontFamilyHeader As String
		Public SecurePaymentPage As Boolean = False

		Public CustomContent As XmlDocument

		Public ReadOnly Property OverbrandIdentifier As Integer
			Get
				If Brand > 0 Then
					Return Brand
				Else
					Return Trade
				End If
			End Get
		End Property

		Public ReadOnly Property FontFamilyName As String
			Get
				If FontFamily Is Nothing Then
					Return ""
				Else
					'Remove any styles, replace pluses with spaces
					Return FontFamily.Split(":"c)(0).Replace("+", " ")
				End If
			End Get
		End Property

		Public ReadOnly Property FontFamilyHeaderName As String
			Get
				If FontFamilyHeader Is Nothing Then
					Return ""
				Else
					'Remove any styles, replace pluses with spaces
					Return FontFamilyHeader.Split(":"c)(0).Replace("+", " ")
				End If
			End Get
		End Property

		Public ReadOnly Property FontFamilyImportURLs As String
			Get

				Dim sImportURL As String = "@import url(https://fonts.googleapis.com/css?family={0});"
				Dim sbImportURLs As New System.Text.StringBuilder

				With sbImportURLs

					'Want to ignore Lato as we already import this by default in case the specified font family fails
					If FontFamilyName <> "" Then
						.AppendFormatLine(sImportURL, FontFamily)
					End If

					'Only want to import the font family header if it differs from the font family
					If FontFamilyHeaderName <> "" AndAlso FontFamilyHeaderName <> FontFamilyName Then
						.AppendFormat(sImportURL, FontFamilyHeader)
					End If

				End With

				Return sbImportURLs.ToString

			End Get
		End Property

	End Class

End Class