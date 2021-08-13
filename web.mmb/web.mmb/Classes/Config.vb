Imports System.IO
Imports System.Configuration.ConfigurationManager
Imports Intuitive.Functions

Public Class Config

	Public Shared ReadOnly Property ServiceURL As String
		Get
			Return SafeString(Config.GetSetting("ServiceURL"))
		End Get
	End Property

	Public Shared ReadOnly Property WebsiteURL As String
		Get
			Return SafeString(Config.GetSetting("WebsiteURL"))
		End Get
	End Property

	Public Shared ReadOnly Property VersionLinkedFiles() As Boolean
		Get
			Return SafeBoolean(Config.GetSetting("VersionLinkedFiles", False))
		End Get
	End Property
	Public Shared ReadOnly Property UseTheme() As Boolean
		Get
			Return SafeBoolean(Config.GetSetting("UseTheme", False))
		End Get
	End Property

	Public Shared ReadOnly Property iVectorWidgetPath() As String
		Get
            Return Config.GetSetting("iVectorWidgetPath", False)
		End Get
	End Property

	Public Shared ReadOnly Property SMTPHost() As String
		Get
			Return Config.GetSetting("SMTPHost")
		End Get
	End Property

	Public Shared ReadOnly Property ErrorEmail() As String
		Get
			Return Config.GetSetting("ErrorEmail")
		End Get
	End Property


#Region "Get setting"

	Public Shared Function GetSetting(ByVal sSetting As String, Optional ByVal bMustHaveValue As Boolean = True) As String

		If Not AppSettings(sSetting) Is Nothing Then
			If bMustHaveValue = False OrElse Not AppSettings(sSetting).ToString = "" Then
				Return CStr(AppSettings(sSetting))
			Else
				Throw New Exception(sSetting & " was found in the config, but does not have a value")
			End If
		ElseIf bMustHaveValue Then
			Throw New Exception("Could not find " & sSetting & " setting")
		Else
			Return ""
		End If
	End Function

#End Region


End Class
