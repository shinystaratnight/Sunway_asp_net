Imports System.Web.UI

Public Class ControlBase
	Inherits Control

	Private sCSSClass As String

#Region "Generated Properties"

	Public ReadOnly Property Command() As String
		Get
			Return Page.Request.Form("Command")
		End Get
	End Property

	Public ReadOnly Property Argument() As String
		Get
			Return Page.Request.Form("Argument")
		End Get
	End Property

	Public ReadOnly Property ImageRoot() As String
		Get
			Return "/images/"
		End Get
	End Property

	Public Property CSSClass() As String
		Get
			Return sCSSClass
		End Get
		Set(ByVal Value As String)
			sCSSClass = Value
		End Set
	End Property

	Public ReadOnly Property UniqueControlName() As String
		Get
			Return Me.Page.Request.FilePath.Replace("/", "").Replace(".aspx", "") & Me.ID
		End Get
	End Property
#End Region

	Public Sub New()

	End Sub
End Class