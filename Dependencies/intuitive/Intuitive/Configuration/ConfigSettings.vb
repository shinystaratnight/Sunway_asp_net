Imports System
Imports System.Configuration
Imports System.Xml
Imports System.Reflection.Assembly
Imports System.Diagnostics

Public Class ConfigSettings

	''' <summary>
	''' The path to the configuration file
	''' </summary>
	Private sPath As String

	''' <summary>
	''' The type of configuration
	''' </summary>
	Public Enum ConfigurationType

		''' <summary>
		''' An app.config file
		''' </summary>
		Application

		''' <summary>
		''' A web.config file
		''' </summary>
		Web
	End Enum

	''' <summary>
	''' Initializes a new instance of the <see cref="ConfigSettings"/> class.
	''' </summary>
	''' <param name="ConfigType">Type of the configuration, from <see cref="ConfigSettings.ConfigurationType"/>.</param>
	Public Sub New(Optional ByVal ConfigType As ConfigurationType = ConfigurationType.Web)
		'work out config path by type
		If ConfigType = ConfigurationType.Application Then
			'assembly name + exe.config
			sPath = "" & System.Reflection.Assembly.GetEntryAssembly.GetName.Name
			sPath += ".exe.config"
		Else
			sPath = System.Web.HttpContext.Current.Server.MapPath("~/web.config")
		End If
	End Sub

	''' <summary>
	''' Initializes a new instance of the <see cref="ConfigSettings"/> class.
	''' </summary>
	''' <param name="ConfigurationPath">The path for the configuration file.</param>
	Public Sub New(ByVal ConfigurationPath As String)
		sPath = ConfigurationPath
	End Sub

	''' <summary>
	''' Sets the value of the specified key to the value provided.
	''' </summary>
	''' <param name="Key">The key you want to change the value of.</param>
	''' <param name="Value">The value you want to set for the key.</param>
	Public Sub Write(ByVal Key As String, ByVal Value As String)

		'get the xml doc
		Dim oXMLDoc As XmlDocument = Me.GetConfigDocument
		Dim oRootNode As XmlNode
		Dim oNode As XmlNode

		If Not oXMLDoc Is Nothing Then
			'get the appsettings node
			oRootNode = oXMLDoc.SelectSingleNode("//appSettings")

			'get our xml element for the given key in the appsettings (case sensitive)
			oNode = oRootNode.SelectSingleNode("//add[@key='" + Key + "']")

			'set value and save
			If Not oNode Is Nothing Then
				oNode.Attributes("value").Value = Value
				Save(oXMLDoc)
			Else
				'create it?
			End If
		End If
	End Sub

	''' <summary>
	''' Returns the value of the specified key in the config document.
	''' </summary>
	''' <param name="Key">The key you wish to retrieve the value for.</param>
	''' <returns></returns>
	Public Overloads Function Read(ByVal Key As String) As String
		'return as string
		Dim oXMLDoc As XmlDocument = Me.GetConfigDocument
		Dim oNode As XmlNode

		If Not oXMLDoc Is Nothing Then
			'the key is case sensitive at the mo
			oNode = oXMLDoc.SelectSingleNode("//appSettings/add[@key='" + Key + "']")

			If Not oNode Is Nothing Then
				'return the value attribute
				Return oNode.Attributes("value").Value
			End If
		End If

		Return ""
	End Function

	''' <summary>
	''' Gets the date value from the specified key, if the value of the key is not a valid date this will return an empty date.
	''' </summary>
	''' <param name="Key">The key you want to retrieve the value for.</param>
	''' <returns>Date</returns>
	Public Overloads Function ReadDate(ByVal Key As String) As Date
		'return as date
		Dim sReturn As String = Read(Key)

		'return
		If DateFunctions.IsDate(sReturn) Then
			Return CType(sReturn, Date)
		Else
			Return DateFunctions.EmptyDate
		End If
	End Function

	''' <summary>
	''' Returns the integer value of the specified key in the config file, if the value in the config file is not a number, it returns -1
	''' </summary>
	''' <param name="Key">The key you want to retrieve the value for.</param>
	''' <returns></returns>
	Public Overloads Function ReadInt(ByVal Key As String) As Integer
		'return as int
		Dim sReturn As String = Read(Key)

		'return
		Dim iValue As Integer
		If Integer.TryParse(sReturn, iValue) Then
			Return iValue
		Else
			Return -1
		End If

	End Function

	''' <summary>
	''' Saces the specified xml to the configuration file
	''' </summary>
	''' <param name="oXML">The o XML to save.</param>
	''' <returns></returns>
	Private Function Save(ByVal oXML As XmlDocument) As Boolean

		Using oWriter As New XmlTextWriter(sPath, Nothing)

			'bit of indetation
			oWriter.Formatting = Formatting.Indented

			'write to file
			oXML.WriteTo(oWriter)

			'tidy
			oWriter.Flush()
			oWriter.Close()

		End Using

		Return True

	End Function

	''' <summary>
	''' Returns the configuration for this instance of <see cref="ConfigSettings"/>.
	''' </summary>
	Private Function GetConfigDocument() As XmlDocument
		Dim oConfig As New XmlDocument

		Try
			'load the xml config file
			oConfig.Load(sPath)

			'return it
			Return oConfig
		Catch ex As Exception
			'err
		End Try

		Return Nothing
	End Function

End Class