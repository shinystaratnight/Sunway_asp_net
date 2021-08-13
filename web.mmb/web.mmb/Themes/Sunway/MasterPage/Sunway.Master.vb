Imports System.IO
Imports Intuitive
Imports Newtonsoft.Json.Linq

Public Class SunwayMaster
    Inherits MasterPage

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
    End Sub

    Protected Overrides Sub OnPreRender(e As EventArgs)

        Dim sJSON As String = File.ReadAllText(HttpContext.Current.Server.MapPath("~/themes/sunway/siteimportconfig.json"))
        Dim oSiteConfig As JObject = JObject.Parse(sJSON)

        For Each website As JObject In oSiteConfig.Item("Websites")
            If Functions.SafeString(website("Url")) = Intuitive.Functions.GetBaseURL() Then

                For Each cssLink As String In website("Css").ToObject(Of List(Of String))()
                    Me.GetPageHead().AddAbsoluteCSS(cssLink)
                Next

                Dim sb As New StringBuilder
                For each jsLink as String in website("Scripts").ToObject(of List(Of string))()
                    sb.AppendFormat("<script type=""text/javascript"" src=""{0}""></script>", jsLink)
                Next
                Me.divImportedScripts.InnerHtml = sb.ToString()

                me.hidApiBaseUrl.Value = Functions.SafeString(website("ApiBaseUrl"))

                Exit For
            End If
        Next

        MyBase.OnPreRender(e)
    End Sub

End Class