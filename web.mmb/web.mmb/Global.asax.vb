Imports System.Web.SessionState
Imports System.Web.Routing

Public Class Global_asax
    Inherits System.Web.HttpApplication

    Public Shared ErrorCounter As Integer = 0

    Sub Application_Start(ByVal sender As Object, ByVal e As EventArgs)

        'set up routing
        RegisterRoutes(RouteTable.Routes)


        'if running local then copy widgets
        If Server.MapPath("~/").ToLower.Contains("projectsstash") Then
            Intuitive.Web.Utility.CopyLibraryWidgets(Server.MapPath("~/"), Config.iVectorWidgetPath)
        End If


    End Sub

    Sub RegisterRoutes(ByVal routes As RouteCollection)


        'this works for routes with one param - we would need to extend for multiple params in the URL path
        routes.Add("services", New Route("services/{handler}/{key}", New HttpHandlerRouteHandler()))


        'adt search tool injector
        routes.MapPageRoute("adt injector", "injector/search/index/{*function}", "~/themes/adt/injector/searchtool.aspx")
        routes.MapPageRoute("offsitepaymentreturn", "OffsitePaymentReturn.ashx", "~/offsitepaymentreturn.ashx")

        'lastly - default route, constraint to ignore elmah logging URL
        Dim constraints As New RouteValueDictionary(New With {.urlname = "^((?!elmah).(?!elmah))*$"})
        routes.MapPageRoute("default", "{*urlname}", "~/Default.aspx", False, Nothing, constraints)



    End Sub


    Sub Session_Start(ByVal sender As Object, ByVal e As EventArgs)


        'create new booking entity
        Dim o As New Booking
        Dim browser As System.Web.HttpBrowserCapabilities = HttpContext.Current.Request.Browser
        Dim request As System.Web.HttpRequest = HttpContext.Current.Request
        Dim CustomerGuid As String = Intuitive.CookieFunctions.Cookies.GetValue(DataStore.Logger.CookieCustomerGuidKey)

        CustomerGuid = BookingBase.DataLogger.LogSessionStarted(CustomerGuid, DateTime.Now, request.UserAgent, browser.Browser,
                                                               browser.Version, browser.Platform, request.UserHostAddress,
                                                    System.Environment.MachineName, HttpContext.Current.Session.SessionID)

        If Not CustomerGuid = "" Then
            Intuitive.CookieFunctions.Cookies.SetValue(DataStore.Logger.CookieCustomerGuidKey, CustomerGuid, Intuitive.CookieFunctions.CookieExpiry.SpecifiedDays, 365 * 50)
        End If

    End Sub

    Sub Application_PreRequestHandlerExecute(ByVal sender As Object, ByVal e As EventArgs)

        Try

            If Not HttpContext.Current.Session Is Nothing Then
                Dim url As String = HttpContext.Current.Request.Url.ToString
                Dim watch As System.Diagnostics.Stopwatch = Stopwatch.StartNew()
                HttpContext.Current.Items.Add("__global_stopwatch", watch)
                Logging.Current.PageEvents.Log(Logging.RequestTime.eEvent.Setup, Logging.RequestTime.eStage.Start)
            End If

        Catch ex As Exception

        End Try
    End Sub

    Sub Application_PostRequestHandlerExecute(ByVal sender As Object, ByVal e As EventArgs)

        Try

            If Not HttpContext.Current.Session Is Nothing Then
                Dim times As Logging.PageRequestEvents = Logging.Current.PageEvents
                Dim watch As System.Diagnostics.Stopwatch = CType(HttpContext.Current.Items("__global_stopwatch"), System.Diagnostics.Stopwatch)
                Dim totalRequestTime As Double = watch.ElapsedMilliseconds / 1000
                times.PreviousEvents.Clear()
                times.PreviousEvents = New List(Of Logging.RequestTime)(times.CurrentEvents)
                times.CurrentEvents.Clear()
                Dim VisitID As Integer = BookingBase.VisitID
                Dim SessionID As String = HttpContext.Current.Session.SessionID
                Dim SearchGuid As String = BookingBase.SearchDetails.SearchGuid
                Dim Url As String = HttpContext.Current.Request.Path
                BookingBase.DataLogger.LogPageView(times.PreviousEvents, Logging.Current.WidgetSpeeds, VisitID,
                                                  SessionID, SearchGuid, Url, totalRequestTime)
            End If


        Catch ex As Exception

        End Try

    End Sub

    Sub Application_AuthenticateRequest(ByVal sender As Object, ByVal e As EventArgs)
        ' Fires upon attempting to authenticate the use
    End Sub



    Sub Application_Error(ByVal sender As Object, ByVal e As EventArgs)
        ' Fires when an error occurs

        Try

            ErrorCounter += 1

            If ErrorCounter = 1 Then

                Intuitive.Web.Logging.LogError("Global", "Application_Error", Server.GetLastError)

            End If

        Catch ex As Exception

        End Try

    End Sub

    Sub Session_End(ByVal sender As Object, ByVal e As EventArgs)
        ' Fires when the session ends

    End Sub

    Sub Application_End(ByVal sender As Object, ByVal e As EventArgs)
        ' Fires when the application ends
    End Sub


End Class