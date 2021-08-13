
Public Class TitleAttribute
	Inherits Attribute
	Public Title As String
	Public Sub New(Title As String)
		Me.Title = Title
	End Sub
End Class

Public Class DeveloperOnlyAttribute
	Inherits Attribute
	Public DeveloperOnly As Boolean
	Public Sub New(DeveloperOnly As Boolean)
		Me.DeveloperOnly = DeveloperOnly
	End Sub
End Class
