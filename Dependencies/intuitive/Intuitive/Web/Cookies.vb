Imports System.Web

Namespace CookieFunctions

	''' <summary>
	''' Contains functions for getting/setting the value of cookies, checking they exist and removing them.
	''' </summary>
    Public Class Cookies

        ''' <summary>
        ''' Sets the value of the specified cookie. 
        ''' If the cookie doesn't exist it creates it.
        ''' </summary>
        ''' <param name="CookieName">Name of the cookie to set the value of.</param>
        ''' <param name="Value">The value to set.</param>
        ''' <param name="Expires">The expiry date of the cookie.</param>
        ''' <param name="SpecifiedDays">How many days the cookie should last, only used with <see cref="CookieExpiry.SpecifiedDays"/> Expires param.</param>
        Public Shared Sub SetValue(
            ByVal CookieName As String,
            ByVal Value As String,
            Optional ByVal Expires As CookieExpiry = CookieExpiry.Session,
            Optional ByVal SpecifiedDays As Integer = 0)

            Dim oCookie As HttpCookie = Cookies.CreateCookie(CookieName, Expires, SpecifiedDays)
            oCookie.Value = Value

        End Sub

        ''' <summary>
        ''' Gets the cookie with the specified name.
        ''' If the cookie doesn't exist it creates it.
        ''' </summary>
        ''' <param name="CookieName">Name of the cookie to set the value of.</param>
        ''' <param name="Expires">The expiry date of the cookie.</param>
        ''' <param name="SpecifiedDays">How many days the cookie should last, only used with <see cref="CookieExpiry.SpecifiedDays"/> Expires param.</param>
        ''' <returns>The cookie mathcing the specified cookie name</returns>
        Public Shared Function CreateCookie(
            ByVal CookieName As String,
            Optional ByVal Expires As CookieExpiry = CookieExpiry.Session,
            Optional ByVal SpecifiedDays As Integer = 0) As HttpCookie

            Dim oCookie As HttpCookie = HttpContext.Current.Request.Cookies(CookieName)
            If oCookie Is Nothing Then
                oCookie = New HttpCookie(CookieName)
            End If

            Select Case Expires
                Case CookieExpiry.OneHour
					oCookie.Expires = Date.Now.AddHours(1)
				Case CookieExpiry.OneDay
					oCookie.Expires = Date.Now.AddDays(1)
				Case CookieExpiry.OneWeek
					oCookie.Expires = Date.Now.AddDays(7)
				Case CookieExpiry.OneMonth
					oCookie.Expires = Date.Now.AddMonths(1)
				Case CookieExpiry.ThreeMonths
					oCookie.Expires = Date.Now.AddMonths(3)
				Case CookieExpiry.SpecifiedDays
					oCookie.Expires = Date.Now.AddDays(SpecifiedDays)
			End Select

            HttpContext.Current.Response.Cookies.Add(oCookie)
            Return oCookie

        End Function

        ''' <summary>
        ''' Gets the value of the specified cookie.
        ''' </summary>
        ''' <param name="CookieName">Name of the cookie.</param>
        Public Shared Function GetValue(ByVal CookieName As String) As String
            Dim sReturn As String = Cookies.GetValueFromContext(HttpContext.Current, CookieName)
            Return sReturn
        End Function

        ''' <summary>
        ''' Gets the cookie value from given context.
        ''' </summary>
        ''' <param name="Context">The context.</param>
        ''' <param name="CookieName">Name of the cookie.</param>
        ''' <returns>System.String.</returns>
        Public Shared Function GetValueFromContext(ByVal Context As HttpContext, ByVal CookieName As String) As String
            Dim sReturn As String = ""

            Try
                If Context IsNot Nothing _
                    AndAlso Context.Request IsNot Nothing _
                    AndAlso Context.Request.Cookies IsNot Nothing Then
                    Dim oCookie As HttpCookie = Context.Request.Cookies(CookieName)
                    If oCookie IsNot Nothing Then
                        sReturn = Functions.SafeString(oCookie.Value)
                    End If
                End If
            Catch ex As Exception
            End Try

            Return sReturn
        End Function

        ''' <summary>
        ''' Checks that the specified cookie exists
        ''' </summary>
        ''' <param name="CookieName">Name of the cookie.</param>
        Public Shared Function Exists(ByVal CookieName As String) As Boolean
            Return Not HttpContext.Current.Request.Cookies(CookieName) Is Nothing
        End Function

		''' <summary>
		''' Removes the specified cookie
		''' </summary>
		''' <param name="CookieName">Name of the cookie to remove.</param>
        Public Shared Sub Remove(ByVal CookieName As String)
            If Exists(CookieName) Then
                Dim oCookie As System.Web.HttpCookie = HttpContext.Current.Request.Cookies(CookieName)
				oCookie.Expires = Date.Now.AddDays(-1)
				HttpContext.Current.Response.Cookies.Add(oCookie)
            End If
        End Sub

    End Class

	''' <summary>
	''' Enum of possible expiry lengths for cookies
	''' </summary>
    Public Enum CookieExpiry
        Session
        OneHour
        OneDay
        OneWeek
        OneMonth
        ThreeMonths
        SpecifiedDays
    End Enum

End Namespace