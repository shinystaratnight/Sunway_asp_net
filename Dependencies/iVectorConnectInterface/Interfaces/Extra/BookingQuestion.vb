Imports System.Xml.Serialization

Public Class BookingQuestion
	Public Property Message As String
	Public Property Required As Boolean
	Public Property QuestionID As Integer
	Public Property Title As String
	Public Property SubTitle As String
	Public Property SortOrder As Integer

	<XmlArray("Options")>
	<XmlArrayItem("Option")>
	Public Property BookingQuestionOptions As New List(Of BookingQuestionOption)
End Class
Public Class BookingQuestionOption
	Public Property SortOrder As Integer
	Public Property OptionID As Integer
	Public Property OptionName As String
End Class
