Imports Intuitive
Imports Intuitive.Functions
Imports System.Text.RegularExpressions
Imports System.Configuration.ConfigurationManager
Imports System.IO
Imports Intuitive.Web.Widgets

Public MustInherit Class MasterPage
    Inherits Master
    Protected Overrides Sub OnInit(ByVal e As System.EventArgs)

        If (Request.Url.AbsolutePath.ToString.ToLower <> "/booking-login" AndAlso Request.Url.AbsolutePath.ToString.ToLower <> "/paymentauthorisation")  AndAlso iVectorWidgets.MyBookingsLogin.MyBookingsReference = "" Then
             WidgetBase.Response.Redirect("/booking-login")
        End If

        '0. clear cache, but not on every ajax request!
        If Not Me.Request Is Nothing AndAlso Me.Request.RawUrl.ToLower.IndexOf("clearcache") > -1 _
         AndAlso Me.Request.RawUrl.ToLower.IndexOf("executeformfunction") = -1 Then
            Intuitive.Functions.ClearCache()
            Net.WebRequests.GetURL(Config.ServiceURL & "ivectorconnect.ashx?clearcache")
            Dim oBooking As Booking = New Booking
        End If


        'css override
        Me.CSSOverrides = New CSSOverrides

        'set overbranding
        Me.Overbranding = New Overbranding


#If DEBUG Then
        Dim oSelectedOverbrand As Overbranding.Overbrand = Me.Overbranding.SelectOverbrand(Config.WebsiteURL)
#Else
		Dim oSelectedOverbrand As Overbranding.Overbrand = Me.Overbranding.SelectOverbrand(Functions.GetBaseURL)
#End If


        'Add meta tag to render IE in the latest mode
        Me.GetPageHead.AddCustomNode("<meta http-equiv=""X-UA-Compatible"" content=""IE=EDGE"">")
        Me.GetPageHead.AddCustomNode("<meta name=""viewport"" content=""width=device-width, maximum-scale=1.0, initial-scale=1.0"" />")

        'Stop iOS Safari ruining styling of telephone numbers
        Me.GetPageHead.AddCustomNode("<meta name=""format-detection"" content=""telephone=no"" />")

        'favicon

        Dim domainUri As New Uri(Booking.Params.Domain)
        Dim domainName As String = domainUri.Host.Substring(0, domainUri.Host.IndexOf("."))

        If File.Exists(
            Server.MapPath(
                String.Format(
                    "{0}/themes/{1}/images/favicons/{2}/favicon.ico",
                    Booking.Params.RootPath,
                    Booking.Params.Theme,
                    domainName))) Then

            Me.GetPageHead.AddCustomNode(
                String.Format("<link rel=""shortcut icon"" href=""{0}/themes/{1}/images/favicons/{2}/favicon.ico"" />",
                Booking.Params.RootPath,
                Booking.Params.Theme,
                domainName))
            Me.GetPageHead.AddCustomNode(
                String.Format("<link rel=""icon"" type=""image/x-icon"" href=""{0}/themes/{1}/images/favicons/{2}/favicon.ico"" />",
                Booking.Params.RootPath,
                Booking.Params.Theme,
                domainName))
        Else
            Me.GetPageHead.AddCustomNode(
                String.Format("<link rel=""shortcut icon"" href=""{0}/themes/{1}/images/favicon.ico"" />",
                Booking.Params.RootPath,
                Booking.Params.Theme))
            Me.GetPageHead.AddCustomNode(
                String.Format("<link rel=""icon"" type=""image/x-icon"" href=""{0}/themes/{1}/images/favicon.ico"" />",
                Booking.Params.RootPath,
                Booking.Params.Theme))
        End If

        'Add global javascript
        Me.GetPageHead.AddAbsoluteJavaScript("//ajax.googleapis.com/ajax/libs/jquery/1.8.2/jquery.min.js")
        Me.GetPageHead.AddJavaScript("/script/jquery.popupoverlay.js")
        Me.GetPageHead.AddJavaScript("/script/polyfills.js")
        Me.GetPageHead.AddJavaScript("/script/lib.js")
        Me.GetPageHead.AddJavaScript("/script/forms.js")
        Me.GetPageHead.AddJavaScript("/script/datepicker.js")
        Me.GetPageHead.AddJavaScript("/script/fastclick.js")

        Dim sLanguageCode As String = BookingBase.Lookups.GetKeyPairValue(Lookups.LookupTypes.LanguageCode, BookingBase.DisplayLanguageID).ToLower()

        If sLanguageCode = "en" Then
            sLanguageCode = "en-GB"
        End If

        Dim sDatepickerLanguage As String = "/script/LanguageScripts/datepicker-" + sLanguageCode + ".js"
        Me.GetPageHead.AddJavaScript(sDatepickerLanguage)

        Me.GetPageHead.AddAbsoluteJavaScript("https://www.google.com/jsapi")
        Me.GetPageHead.AddJavaScript("/script/googlemap.js")
        Me.GetPageHead.AddAbsoluteJavaScript("https://maps.googleapis.com/maps/api/js?v=3&libraries=geometry&sensor=false")
        Me.GetPageHead.AddJavaScript("/script/json2.js")

        'captcha script
        Me.GetPageHead.AddAbsoluteJavaScript("https://www.google.com/recaptcha/api.js?onload=contactUsBuildCaptcha&render=explicit")

        'add core stytlesheets
        Me.GetPageHead.AddCSS("/style/masterlayout.css")


        'Apply theme stylesheets
        If (Config.UseTheme) Then

            If Booking.Params.UseTranspiledResources Then
                Me.GetPageHead().AddCSS((String.Format("/assets/css/themes/{0}/main.css", Booking.Params.Theme)))
            Else
                Me.GetPageHead.AddCSS(Me.Overbranding.BuildCSS(String.Format("/themes/{0}/", Booking.Params.Theme), "main"))
            End If

            Me.GetPageHead.AddCSS(String.Format("/themes/{0}/font-awesome.min.css", Booking.Params.Theme))

            Me.GetPageHead.AddConditionalIeCss(String.Format("/themes/{0}/ie9.css", Booking.Params.Theme), 9, "lte")

            Dim bIsMobileDevice As Boolean = Intuitive.Web.Support.BrowserSupport.IsMobile
            Dim bIsTabletDevice As Boolean = Intuitive.Web.Support.BrowserSupport.IsTablet

            'apply extra stylesheets
            If bIsTabletDevice Then
                Me.GetPageHead.AddCSS(Me.Overbranding.BuildCSS(String.Format("/themes/{0}/", Booking.Params.Theme), "tablet"))
            ElseIf Request.Browser.Browser = "Chrome" Then
                Me.GetPageHead.AddCSS(Me.Overbranding.BuildCSS(String.Format("/themes/{0}/", Booking.Params.Theme), "chrome"))
            ElseIf Request.Browser.Browser = "Safari" Then
                Me.GetPageHead.AddCSS(Me.Overbranding.BuildCSS(String.Format("/themes/{0}/", Booking.Params.Theme), "safari"))
            ElseIf Request.Browser.Browser = "Firefox" Then
                Me.GetPageHead.AddCSS(Me.Overbranding.BuildCSS(String.Format("/themes/{0}/", Booking.Params.Theme), "firefox"))
            ElseIf Request.Browser.Browser = "InternetExplorer" Then
                Me.GetPageHead.AddCSS(Me.Overbranding.BuildCSS(String.Format("/themes/{0}/", Booking.Params.Theme), "ie"))
            End If

        End If


        'apply any available css overrides
        If Me.CSSOverrides.CSSOverrides.Count > 0 And Me.CSSOverrides.CSSOverrideEnabled Then
            Me.GetPageHead.AddCSSOverrides(Me.CSSOverrides.CSSOverrides)
        End If

        MyBase.OnInit(e)



    End Sub


    Protected Overrides Sub Render(ByVal writer As System.Web.UI.HtmlTextWriter)

        Logging.Current.PageEvents.Log(Logging.RequestTime.eEvent.Setup, Logging.RequestTime.eStage.End)

        'create a html writer
        Dim sb As New StringBuilder
        Dim sw As New System.IO.StringWriter(sb)
        Dim htmlWriter As New HtmlTextWriter(sw)

        Logging.Current.PageEvents.Log(Logging.RequestTime.eEvent.MainRender, Logging.RequestTime.eStage.Start)
        MyBase.Render(htmlWriter)
        Logging.Current.PageEvents.Log(Logging.RequestTime.eEvent.MainRender, Logging.RequestTime.eStage.End)

        'grab our page html from the html writer
        Dim sHTML As String = sb.ToString


        'Translate the page
        Logging.Current.PageEvents.Log(Logging.RequestTime.eEvent.Translation, Logging.RequestTime.eStage.Start)
        If Not BookingBase.Params.PreTranslateTemplates Then
            sHTML = Intuitive.Web.Translation.TranslateHTML(sHTML)
        End If
        Logging.Current.PageEvents.Log(Logging.RequestTime.eEvent.Translation, Logging.RequestTime.eStage.End)


        'set the autoversioning of css and js
        Logging.Current.PageEvents.Log(Logging.RequestTime.eEvent.VersionFiles, Logging.RequestTime.eStage.Start)
        If Not BookingBase.Params.UseVersionedAssets Then
            sHTML = VersionLinkedFiles.SetVersionLinkedFiles(sHTML)
        End If
        Logging.Current.PageEvents.Log(Logging.RequestTime.eEvent.VersionFiles, Logging.RequestTime.eStage.End)


        'amend img src and <a> hrefs with the root path if needed
        If Not BookingBase.Params.RootPath = "" Then
            Logging.Current.PageEvents.Log(Logging.RequestTime.eEvent.LinkChecker, Logging.RequestTime.eStage.Start)
            sHTML = LinkChecker.Go(sHTML)
            Logging.Current.PageEvents.Log(Logging.RequestTime.eEvent.LinkChecker, Logging.RequestTime.eStage.End)
        End If


        'Finally write the html out to the browser
        writer.Write(sHTML)


    End Sub

    Public Overrides ReadOnly Property GetPageServer As PageServer
        Get
            Return New PageServer_Beta(Config.iVectorWidgetPath,
                                  System.Reflection.Assembly.GetAssembly(GetType(iVectorWidgets.Global_asax)),
                                  System.Reflection.Assembly.GetExecutingAssembly)
        End Get
    End Property


End Class
