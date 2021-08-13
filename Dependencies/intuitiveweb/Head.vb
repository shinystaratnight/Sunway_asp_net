Imports System.IO
Imports System.Text

Public Class Head
	Inherits System.Web.UI.Control

	Public Property Title As String
	Public Property Description As String
	Public Property KeyWords As String
	Public Property CanonicalURL As String
	Public Property Robots As String
	Public Property ForceRobosNoIndexNoFollow As Boolean = False
	Public PageName As String = ""
	Public CSSOverrideEnabled As Boolean

	Private Property CSS As New Generic.List(Of String)
	Private Property CSSOverrides As New Generic.List(Of String)
	Private Property JavaScript As New Generic.List(Of String)
	Private Property CustomNode As New Generic.List(Of String)
	Private Property BottomCustomNodes As New Generic.List(Of String)

	Public Property OpenGraph As OpenGraph


	Protected Overrides Sub Render(writer As System.Web.UI.HtmlTextWriter)

		Dim sb As New System.Text.StringBuilder
		sb.AppendLine("<head>")

		'Custom nodes
		For Each sCustomNode As String In Me.CustomNode
			sb.AppendFormat("\t{0}\n", sCustomNode)
		Next

        'title and meta description
        If Me.Title IsNot Nothing AndAlso Not Me.Title = "" Then
            sb.AppendFormat("\t<title>{0}</title>\n", Me.Title)
        End If

        If Me.Description IsNot Nothing AndAlso Not Me.Description = "" Then
            sb.AppendFormat("\t<meta name=""description"" content=""{0}""/>\n", Me.Description)
        End If

        If Me.KeyWords IsNot Nothing AndAlso Not Me.KeyWords = "" Then
            sb.AppendFormat("\t<meta name=""keywords"" content=""{0}""/>\n", Me.KeyWords)
        End If

        sb.AppendFormat("\t<meta name=""viewport"" content=""width=device-width, minimum-scale=1.0, maximum-scale=1.0, user-scalable=no""/>\n")

        If Me.CanonicalURL IsNot Nothing AndAlso Not Me.CanonicalURL = "" Then
			sb.AppendFormat("\t<link rel=""canonical"" href=""{0}"">\n", Me.CanonicalURL)
		End If

		sb.Append("\t<meta charset=""utf-8"">")

		'robots
		If Me.ForceRobosNoIndexNoFollow Then
			sb.Append("\t<meta name=""robots"" content=""noindex, nofollow"" />\n")
		ElseIf Me.Robots <> "" Then
			sb.AppendFormat("\t<meta name=""robots"" content=""{0}"" />\n", Me.Robots)
		End If

		'Open-graph meta tags
		If Me.OpenGraph IsNot Nothing Then
			sb.Append(Me.OpenGraph.ToHTML())
		End If

		'css
		For Each sCSS As String In CSS
			sb.AppendFormat("\t<link rel=""stylesheet"" type=""text/css"" href=""{0}""/>\n", sCSS)
		Next

		'add any CSS overrides
		For Each sCSSOverride As String In CSSOverrides
			sb.Append("\t<style>\n")
			sb.Append(sCSSOverride)
			sb.Append("\t</style>\n")
		Next

		'js
		For Each sJavaScript As String In JavaScript
			sb.AppendFormat("\t<script type=""text/javascript"" src=""{0}""></script>\n", sJavaScript)
		Next

        sb.AppendFormat("<script>var INTV = {{}}; INTV.Settings = {{}}; INTV.Settings.PageName = '{0}';" &
         "INTV.Settings.RootPath = '{1}';INTV.Settings.VisitID = {2};</script>", Me.PageName, BookingBase.Params.RootPath, BookingBase.VisitID)

		'Custom nodes
		For Each sBottomCustomNode As String In Me.BottomCustomNodes
			sb.AppendFormat("\t{0}\n", sBottomCustomNode)
		Next

		sb.AppendLine("</head>")

        If WebSupportToolbar.LoggedIn Then
            sb.AppendLine("<script>$('document').ready(function() {(function(){var sEmail='paul';var sPassword='****';int.ff.Call('=IntuitiveWeb.WebSupportToolbar.Login', function(s) {if (s=='') {alert('Unrecognised email/password');} else {var doc=document.createDocumentFragment();var container=document.createElement('div');container.innerHTML=s;doc.appendChild(container);document.body.appendChild(doc);}}, sEmail, sPassword);})();})</script>")
        End If

		writer.Write(sb.ToString.Replace("\n", ControlChars.NewLine).Replace("\t", ControlChars.Tab))

	End Sub
    

	Public Sub AddCSS(ByVal URL As String)
		If URL <> "" AndAlso Not Me.CSS.Contains(URL) AndAlso
		 File.Exists(HttpContext.Current.Server.MapPath(URL)) Then

			URL = HttpContext.Current.Request.ApplicationPath & URL.Replace("~", "")
			URL = URL.Replace("//", "/")

			URL = WebAssets.Assets.Instance.GetFilePath(URL, BookingBase.Params.UseVersionedAssets)

			If Not BookingBase.Params.RootPath = "" Then
				URL = BookingBase.Params.RootPath & URL
			End If

			Me.CSS.Add(URL)
		End If
	End Sub

	Public Sub AddCSSOverrides(ByVal oCSSOverrides As Generic.List(Of CSSOverrides.CSSOverride))

		For Each oCSSOverride As CSSOverrides.CSSOverride In oCSSOverrides
			If oCSSOverride.Enabled Then
				Me.CSSOverrides.Add(oCSSOverride.CSSOverride)
			End If
		Next

	End Sub

	'this should only ever be relative URLs
	Public Sub AddJavaScript(ByVal URL As String)
		If URL <> "" AndAlso Not Me.JavaScript.Contains(URL) AndAlso
		 File.Exists(HttpContext.Current.Server.MapPath(URL)) Then

			URL = HttpContext.Current.Request.ApplicationPath & URL.Replace("~", "")
			URL = URL.Replace("//", "/")

			URL = WebAssets.Assets.Instance.GetFilePath(URL, BookingBase.Params.UseVersionedAssets)

			If Not BookingBase.Params.RootPath = "" Then
				URL = BookingBase.Params.RootPath & URL
			End If

			Me.JavaScript.Add(URL)
		End If
	End Sub

	'this should only ever be absolute URLs eg external resources
	Public Sub AddAbsoluteJavaScript(URL As String)
		If URL <> "" AndAlso Not Me.JavaScript.Contains(URL) Then
			Me.JavaScript.Add(URL)
		End If
	End Sub


    Public sub AddAbsoluteCSS(byval URL As string)
        If URL <> "" AndAlso Not Me.CSS.Contains(URL) Then
            me.CSS.Add(URL)
        End If
    End sub


	Public Sub AddCustomNode(ByVal Node As String)
		If Node <> "" Then
			Me.CustomNode.Add(Node)
		End If
    End Sub

    Public Sub AddConditionalIeCss(ByVal Path As String, BrowserNumber As Integer, ConditionalOperator As String)

		If Path <> "" AndAlso File.Exists(HttpContext.Current.Server.MapPath(Path)) Then

			If Not ConditionalOperator = "" Then ConditionalOperator = " " & ConditionalOperator
			Dim conditional As String = String.Format("<!--[if{0} IE {1}>", ConditionalOperator, BrowserNumber)
			Dim script As String = String.Format("<link rel=""stylesheet"" type=""text/css"" href=""{0}""/>", Path)

			Dim sb As New StringBuilder
			sb.AppendLine(conditional)
			sb.AppendLine(script)
			sb.AppendLine("<![endif]-->")

			Me.CustomNode.Add(sb.ToString)

		End If

	End Sub

	Public Sub AddCustomNodeAtBottom(ByVal Node As String)
		If Node <> "" Then
			Me.BottomCustomNodes.Add(Node)
		End If
	End Sub

End Class
