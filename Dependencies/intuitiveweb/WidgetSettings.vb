Public Class WidgetSettings

	Public Class Settings
		Inherits Generic.List(Of WidgetSettings.Setting)

		Public ReadOnly Property GetValue(ByVal Key As String, Optional ByVal MustExist As Boolean = False) As String
			Get
				For Each oSetting As WidgetSettings.Setting In Me
					If oSetting.Key.ToLower = Key.ToLower Then
						Return oSetting.Value
					End If
				Next

				If MustExist Then Throw New Exception("Could not find setting with key " & Key)
				Return ""

			End Get
		End Property

	End Class

	Public Class Setting
		Public Property Key As String
		Public Property Value As String
	End Class

End Class
