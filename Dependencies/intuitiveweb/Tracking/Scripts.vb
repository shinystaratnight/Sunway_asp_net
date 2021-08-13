
Namespace Tracking

    Public Class Scripts


#Region "public members"

        Public TopHeaderScripts As String
        Public BottomHeaderScripts As String
        Public TopBodyScripts As String
        Public BottomBodyScripts As String

#End Region


#Region "Constructors"

        Public Sub New()
        End Sub

        Public Sub New(sTopHeaderScripts As String, sBottomHeaderScripts As String, sTopBodyScripts As String, sBottomBodyScripts As String)
            Me.TopHeaderScripts = sTopHeaderScripts
            Me.BottomHeaderScripts = sBottomHeaderScripts
            Me.TopBodyScripts = sTopBodyScripts
            Me.BottomBodyScripts = sBottomBodyScripts
        End Sub

#End Region


    End Class


End Namespace

