Namespace Extra

    Public Class ExtraLocation
        Public Property ExtraLocationID As Integer
        Public Property ExtraLocationType As String
    End Class

    Public Class ExtraLocations
        Inherits Generic.List(Of ExtraLocation)

        Public Sub New()
            MyBase.New()
        End Sub

        Public Sub New(ByVal ExtraID As Integer)

            Dim dt As DataTable = SQL.GetDataTableCache("select * from dbo.Extra_ListGeographyByExtraId(" & ExtraID & ")")

            For Each dr As DataRow In dt.Rows
                Dim oExtraLocation As New ExtraLocation
                With oExtraLocation
                    .ExtraLocationID = CInt(dr("ParentID"))
                    .ExtraLocationType = CStr(dr("Type"))
                End With
                Me.Add(oExtraLocation)
            Next
        End Sub

    End Class

End Namespace