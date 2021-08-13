Namespace Support

    Public Class Erratum
        Public Property ErratumSubject As String
        Public Property ErratumDescription As String
        Public Property ErratumType As String

        Public Sub New()
            MyBase.New()
        End Sub

        Public Sub New(ByVal ErratumSubject As String, ByVal ErratumDescription As String, ByVal ErratumType As String)
            Me.ErratumSubject = ErratumSubject
            Me.ErratumDescription = ErratumDescription
            Me.ErratumType = ErratumType
        End Sub

        Public Sub New(ByVal ErratumSubject As String, ByVal ErratumDescription As String)
            Me.New(ErratumSubject, ErratumDescription, String.Empty)
        End Sub
    End Class

End Namespace