Imports System.Configuration

Namespace Configuration

	Public Class ConfigReader
		Implements IConfigReader

		Public Function GetSetting(Setting As String) As String Implements IConfigReader.GetSetting
			Return ConfigurationManager.AppSettings(Setting)
		End Function
	End Class

End Namespace