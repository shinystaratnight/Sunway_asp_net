
'also see CommonElement class (at bottom of file)

Public Class PageDefinition

	Public Property PageName As String
	Public Property Head As PageHeader
	Public Property URLRewriteInfo As URLRewriteInfoDef
	Public Property Sections As New Generic.List(Of Section)
	Public Property ExcludeSections As New Generic.List(Of Section)
	Public Property SharedObjects As New SharedObjectCollection
	Public Property Content As CMS.URLContent
	Public Property PageSettings As New Generic.List(Of PageSetting)
	Public Property Widgets As New Generic.List(Of Widget)
	Public Property RequiresLogin As Boolean = False
	Public Property ExcludeCommonElements As Boolean = False
	Public Property Overbranding As New Overbranding

	'process widgets - basically the original format was too verbose
	'so we simplified by moving the section name into the widget definition
	'however, we need backwards compatibility to this routine turns the nice
	'concise format to work in the old way
	Public Sub ProcessWidgets()

		'each widget in root
		For Each oWidget As Widget In Me.Widgets
			If oWidget.Section = "" Then Throw New Exception("Section must specified in widget " & oWidget.Name)

			Dim oSection As Section = Nothing
			For Each oExistingSection As Section In Me.Sections
				If oExistingSection.Container = oWidget.Section Then
					oSection = oExistingSection
					Exit For
				End If
			Next
			If oSection Is Nothing Then
				oSection = New Section
				oSection.Container = oWidget.Section
				Me.Sections.Add(oSection)
			End If

			oSection.Widgets.Add(oWidget)
		Next

	End Sub


#Region "classdefs"

	Public Class PageSetting
		Public Key As String
		Public Value As String
	End Class

	Public Class PageHeader
		Public Property FromXML As Boolean = False
		Public Property ObjectType As String
		Public Property Title As String
		Public Property Description As String
		Public Property KeyWords As String
		Public Property CanonicalURL As String
		Public Property OpenGraphXPath As String
	End Class

	Public Class Section
		Public Property Container As String
		Public Property OverrideOuterHTML As Boolean = False
		Public Property Widgets As New WidgetCollection
	End Class

	Public Class WidgetCollection
		Inherits Generic.List(Of Widget)
	End Class

	Public Class Widget
		Public Property Name As String
		Public Property Section As String
		Public Property Type As String
		Public Property ParentWidget As String
		Public Property FromLibrary As Boolean
		Public Property ThemeSpecific As Boolean
		Public Property Devices As String
		Public Property RawHTML As String
		Public Property ObjectType As String
		Public Property ObjectID As Integer
		Public Property CacheMinutes As Integer = 0
		Public Property Settings As New WidgetSettings
        Public Property LoginStatus As LoginState
        Public Property BookingJourneys As String
	End Class


	Public Class WidgetSettings
		Inherits Generic.List(Of WidgetSetting)

		Public ReadOnly Property GetValue(ByVal Key As String, Optional ByVal MustExist As Boolean = False) As String
			Get
				For Each oSetting As WidgetSetting In Me
					If oSetting.Key.ToLower = Key.ToLower Then
						Return oSetting.Value
					End If
				Next

				If MustExist Then Throw New Exception("Could not find setting with key " & Key)
				Return ""

			End Get
		End Property

	End Class

	Public Class WidgetSetting

        Public Sub New()
		End Sub

        Public Sub New(ByVal key As String, byval value As string)
            Me.Key = key
            Me.Value = value
		End Sub

		Public Property Key As String
		Public Property Value As String
	End Class

	Public Enum LoginState
		None
		LoggedIn
		LoggedOut
	End Enum

	Public Class SharedObjectCollection
		Inherits Generic.List(Of SharedObject)

		Public Overloads Sub Add(ByVal Key As String, ByVal Value As Object)

			For Each oItem As SharedObject In Me
				If oItem.Key = Key Then Throw New Exception("Key already added")
			Next

			Dim oSharedObject As New SharedObject With {.Key = Key, .Value = Value}

			Me.Add(oSharedObject)

		End Sub

		Public Shadows Function Contains(ByVal Key As String) As Boolean

			For Each oItem As SharedObject In Me
				If oItem.Key = Key Then Return True
			Next

			Return False

		End Function

		Public Shadows Function Item(ByVal Key As String) As Object

			For Each oItem As SharedObject In Me
				If oItem.Key = Key Then Return oItem.Value
			Next

			Return Nothing

		End Function

	End Class

	Public Class SharedObject
		Public Key As String
		Public Value As Object
	End Class

	Public Class URLRewriteInfoDef
		Public Property DataObject As String
		Public Property ID As Integer

		Public Sub New(ByVal URL As String)

			'Get DataObject and ID for URL
			Dim dr As DataRow = SQL.GetDataRow("select top 1 DataObject, ID from CustomURLRewrite where URL='{0}'", URL)

			If dr Is Nothing Then
				Throw New Exception(String.Format("URLRewriteInfo could not be found for URL {0}", URL))
			End If

			Me.DataObject = dr("DataObject").ToString
			Me.ID = dr("ID").ToString.ToSafeInt

		End Sub

		Public Sub New()
		End Sub

	End Class



	Public Class CommonElements
		Public Property Sections As New Generic.List(Of PageDefinition.Section)
		Public Property Widgets As New Generic.List(Of PageDefinition.Widget)
	End Class

#End Region


End Class