Namespace Support

	Public Class FlightErratum

		Public Property ErratumSubject As String
		Public Property ErratumDescription As String

		Public Sub New()
			MyBase.New()
		End Sub

		Public Sub New(ByVal ErratumSubject As String, ByVal ErratumDescription As String)
			Me.ErratumSubject = ErratumSubject
			Me.ErratumDescription = ErratumDescription
		End Sub

	End Class

End Namespace