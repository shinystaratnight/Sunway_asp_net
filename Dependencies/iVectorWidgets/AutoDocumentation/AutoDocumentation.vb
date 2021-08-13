Imports Intuitive.Web
Imports Intuitive.Web.Widgets
Imports Intuitive
Imports Intuitive.Functions
Imports Intuitive.Web.Translation
Imports System.Xml
Imports System.Reflection
Imports System.ComponentModel
Imports System.Xml.Serialization
Imports System.IO

Public Class AutoDocumentation

	Public Sub Generate()

		'1. get widgets
		Dim oWidgets As IEnumerable(Of Type) = Me.GetWidgets()
		Dim oDocs As New Documentation

		'2. loop through widgets, get each setting and attributes
		For Each oWidget As Type In oWidgets
			Dim widget As New Widget
			widget.Name = oWidget.Name
			widget.Settings = Me.GetSettings(oWidget)
			oDocs.Widgets.Add(widget)
		Next

		oDocs.MissingSettings = oDocs.Widgets.Where(Function(o) o.Settings.Count = 0).Count
		GenerateDocs(oDocs)


	End Sub


	Private Function GetWidgets() As IEnumerable(Of Type)

		Dim widgets As IEnumerable(Of Type) = Assembly.GetAssembly(GetType(SearchTool)).GetTypes().Where(Function(cl) cl.IsClass _
		  AndAlso Not cl.IsAbstract AndAlso cl.IsSubclassOf(GetType(WidgetBase)))

		Return widgets

	End Function


	Private Function GetSettings(widget As Type) As List(Of Setting)

		Dim memInfo As MemberInfo() = widget.GetMember("eSetting")
		Dim oList As New List(Of Setting)

		For Each oInfo As MemberInfo In memInfo

			For Each oEnum As Array In [Enum].GetValues(CType(oInfo, Type))

				Dim oSetting As New Setting
				Dim attributes As Object() = oEnum.GetType().GetMember(oEnum.ToString)(0).GetCustomAttributes(False)

				For Each oAttribute As Object In attributes

					If oAttribute.GetType() = GetType(DescriptionAttribute) Then
						Dim attribute As DescriptionAttribute = CType(oAttribute, DescriptionAttribute)
						oSetting.Description = attribute.Description
					ElseIf oAttribute.GetType() = GetType(TitleAttribute) Then
						Dim attribute As TitleAttribute = CType(oAttribute, TitleAttribute)
						oSetting.Title = attribute.Title
					ElseIf oAttribute.GetType() = GetType(DeveloperOnlyAttribute) Then
						Dim attribute As DeveloperOnlyAttribute = CType(oAttribute, DeveloperOnlyAttribute)
						oSetting.DeveloperOnly = attribute.DeveloperOnly
					End If

				Next

				oList.Add(oSetting)

			Next

		Next

		Return oList

	End Function

	Private Sub GenerateDocs(Documentation As Documentation)

		Dim xml As XmlDocument = Intuitive.Serializer.Serialize(Documentation)
		File.WriteAllText("c:\temp\widgets.xml", xml.InnerXml)

		Dim clientDocs As String = Intuitive.XMLFunctions.XMLStringTransformToString(xml, AutoDocs.Resources.AutoDocsResource.ClientDoc)
		File.WriteAllText("c:\temp\widgets.html", clientDocs)

	End Sub

	Public Class Documentation
		Public Widgets As New List(Of Widget)
		Public MissingSettings As Integer = 0
	End Class

	Public Class Widget
		Public Name As String = ""
		Public Settings As New List(Of Setting)
		Public DeveloperOnly As Boolean = False
	End Class

	Public Class Setting
		Public Title As String = ""
		Public Description As String = ""
		Public DeveloperOnly As Boolean = False
	End Class

End Class
