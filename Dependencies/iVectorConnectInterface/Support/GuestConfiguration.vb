Imports System.Xml.Serialization

Namespace Support

	''' <summary>
	''' Used by Search and prebook requests
	''' </summary>
	Public Class GuestConfiguration
		Public Property Adults As Integer
		Public Property Children As Integer
		Public Property Infants As Integer
		<XmlArrayItem("AdultAge")>
		Public Property AdultAges As New List(Of Integer)
		<XmlArrayItem("ChildAge")>
		Public Property ChildAges As New List(Of Integer)

        Public Function Validate(ByVal sNoGuestsWarning As String) As List(Of String)

            Dim aWarnings As New List(Of String)
            If Me.Adults + Me.Children + Me.Infants = 0 Then aWarnings.Add(sNoGuestsWarning)

            'number of child ages
            If Me.Children > Me.ChildAges.Count Then
                aWarnings.Add("A child age must be specified for each child.")
            End If

            'child ages
            For Each ChildAge As Integer In Me.ChildAges
                If ChildAge < 2 Or ChildAge >= 18 Then
                    aWarnings.Add("The child age specified must be between 2 and 17. Children under 2 are classed as infants.")
                End If
            Next

            Return aWarnings

        End Function

    End Class

End Namespace