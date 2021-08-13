Imports System.Web.UI

Namespace WebControls

	Public MustInherit Class WebControlBase
		Inherits ControlBase

		Private oControlSize As ControlSizes = ControlSizes.Normal
		Private iTabIndex As Integer = 0
		Private sAttributeName As String = ""
		Private sAttributeValue As String = ""
		Private bEnabled As Boolean = True

#Region "Generated Properties"

		Public Property ControlSize() As ControlSizes
			Get
				Return oControlSize
			End Get
			Set(ByVal Value As ControlSizes)
				oControlSize = Value
			End Set
		End Property

		Public Overridable Property MaxLength() As Integer

		Public Property TabIndex() As Integer
			Get
				Return iTabIndex
			End Get
			Set(ByVal Value As Integer)
				iTabIndex = Value
			End Set
		End Property

		Public Property AttributeName() As String
			Get
				Return sAttributeName
			End Get
			Set(ByVal Value As String)
				sAttributeName = Value
			End Set
		End Property

		Public Property AttributeValue() As String
			Get
				Return sAttributeValue
			End Get
			Set(ByVal Value As String)
				sAttributeValue = Value
			End Set
		End Property

		Public Property Enabled() As Boolean
			Get
				Return bEnabled
			End Get
			Set(ByVal Value As Boolean)
				bEnabled = Value
			End Set
		End Property

		Public MustOverride Property Value() As String
		Public MustOverride Property IsValid() As Boolean

#End Region

		Public Enum ControlSizes
			Tiny
			Small
			Normal
			Large
		End Enum

		Public Sub New()
			'Me.EnableViewState = False
		End Sub
	End Class

End Namespace