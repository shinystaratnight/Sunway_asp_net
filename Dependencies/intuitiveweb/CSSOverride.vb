Imports System.Xml

Public Class CSSOverrides

	Public Property CSSOverrides As Generic.List(Of CSSOverride)

	Public Property CSSOverrideEnabled As Boolean

		Get
			If HttpContext.Current.Session("cssoverride_enabled") IsNot Nothing Then
				Return CType(HttpContext.Current.Session("cssoverride_enabled"), Boolean)
			End If
			Return True
		End Get
		Set(value As Boolean)
			HttpContext.Current.Session("cssoverride_enabled") = value
		End Set

	End Property

	Public Sub New()

		'get collection of css overrides and add them to a collection
		Dim oCSSOverrideXML As System.Xml.XmlDocument = Utility.BigCXML("CSSOverrides", 1, 60)
		Me.CSSOverrides = Utility.XMLToGenericList(Of CSSOverride)(oCSSOverrideXML)

		'check query string to see if overrides need to be disabled, then store on session
		'this is for debugging/testing
		If HttpContext.Current.Request.QueryString("overridecss").ToSafeString.ToLower = "false" Then
			Me.CSSOverrideEnabled = False
		ElseIf HttpContext.Current.Request.QueryString("overridecss").ToSafeString.ToLower = "true" Then
			Me.CSSOverrideEnabled = True
		End If

	End Sub

	Public Class CSSOverride
		Public Name As String
		Public CSSOverride As String
		Public Enabled As Boolean
	End Class

End Class
