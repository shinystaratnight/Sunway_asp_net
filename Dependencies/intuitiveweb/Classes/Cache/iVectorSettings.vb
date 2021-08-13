Namespace CacheControl


	Public Class iVectorSettings

#Region "Properties"

		Public Shared ReadOnly Property Cache As iVectorSettings
			Get
				Dim oIvectorSettings As iVectorSettings = CacheControl.Cache.GetCache(Of iVectorSettings)("iVectorSettings")
				If oIvectorSettings Is Nothing Then
					oIvectorSettings = New iVectorSettings
					CacheControl.Cache.AddToCache("iVectorSettings", oIvectorSettings, 60)
					Return oIvectorSettings
				Else
					Return oIvectorSettings
				End If
			End Get
		End Property

		Public Settings As New Generic.List(Of Setting)

#End Region


#Region "Constructor"

		Public Sub New()

			'get settings from BigC
			Dim oXML As System.Xml.XmlDocument = Utility.BigCXML("iVectorSettings", 1, 60, 0, 0)

			'convert xml to generic list
			Try
				Me.Settings = Utility.XMLToGenericList(Of Setting)(oXML)
			Catch ex As Exception
			End Try

		End Sub

#End Region


#Region "Setting Value"

		Public Function SettingValue(ByVal [Module] As String, ByVal Setting As String) As String

			Dim oSetting As Setting = Me.Settings.Where(Function(o) o.Module = [Module] AndAlso o.Setting = Setting).FirstOrDefault

			If Not oSetting Is Nothing Then
				Return oSetting.SettingValue
			Else
				Return ""
			End If

		End Function

#End Region


#Region "Support Class - Setting"

		Public Class Setting
			Public [Module] As String
			Public Setting As String
			Public SettingValue As String
			Public DefaultValue As String
		End Class

#End Region


	End Class

End Namespace


