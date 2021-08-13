Imports System.Xml.Serialization
Imports System.Reflection
Imports Intuitive.Functions

Public Class CustomQueryRequest
	Implements iVectorConnectInterface.Interfaces.IVectorConnectRequest

#Region "Properties"

    Public Property LoginDetails As LoginDetails Implements Interfaces.IVectorConnectRequest.LoginDetails
	Public Property QueryName As String
	Public Property OverrideConnectString As String

    'helpers
    Public Parameters As New Parameters


#Region "SQL Params"

    Public ReadOnly Property SQLParams(aParams As Parameters.Params) As Object()
        Get
            Dim aSQLParams(aParams.Count - 1) As Object
            Dim iIndex As Integer = 0
            For Each oParameter As Parameters.Param In aParams
                aSQLParams(iIndex) = oParameter.SQLValue
                iIndex += 1
            Next

            Return aSQLParams
        End Get

    End Property

#End Region

#End Region





#Region "Validation"

    Public Function Validate(Optional ValidationType As interfaces.eValidationType = Interfaces.eValidationType.None) As Generic.List(Of String) Implements Interfaces.IVectorConnectRequest.Validate
        Dim aWarnings As New Generic.List(Of String)

        'query name
        If Me.QueryName = "" Then aWarnings.Add("A query name must be specified.")

        Return aWarnings

    End Function

#End Region

End Class

#Region "parameter class"

Public Class Parameters

    Public Param1 As String
    Public Param2 As String
    Public Param3 As String
    Public Param4 As String
    Public Param5 As String
    Public Param6 As String
    Public Param7 As String
    Public Param8 As String
    Public Param9 As String
    Public Param10 As String
    Public Param11 As String
    Public Param12 As String
    Public Param13 As String
    Public Param14 As String
    Public Param15 As String



    'generate param list 
    Public Function GenerateParamList() As Params

        Dim oParams As New Params

        For Each oField As FieldInfo In Me.GetType.GetFields
            If oField.Name.StartsWith("Param") AndAlso Not oField.GetValue(Me) Is Nothing Then
                Dim oParam As New Param
                oParam.Value = oField.GetValue(Me).ToString
                oParams.Add(oParam)
            End If
        Next

        Return oParams

    End Function

    Public Class Params
        Inherits Generic.List(Of Param)

    End Class


    Public Class Param
        Public Value As String
        Public ParameterName As String
        Public DataType As String
        Public SQLValue As String

        Public Function ValidateAndSet() As Boolean

            Dim bOK As Boolean = True
            Select Case DataType

                Case "String"
                    Me.SQLValue = Intuitive.SQL.GetSqlValue(Me.Value)

                Case "Integer"
                    If IsNumeric(Me.Value) AndAlso Me.Value.IndexOf(".") = -1 Then
                        Me.SQLValue = Intuitive.SQL.GetSqlValue(Me.Value, Intuitive.SQL.SqlValueType.Integer)
                    Else
                        bOK = False
                    End If

                Case "Date"
                    If IsDate(Me.Value) Then
                        Me.SQLValue = Intuitive.SQL.GetSqlValue(Me.Value, Intuitive.SQL.SqlValueType.Date)
                    Else
                        bOK = False
                    End If

                Case "Numeric"
                    If IsNumeric(Me.Value) Then
                        Me.SQLValue = Intuitive.SQL.GetSqlValue(Me.Value, Intuitive.SQL.SqlValueType.Numberic)
                    Else
                        bOK = False
                    End If

                Case Else
                    bOK = False

            End Select

            Return bOK
        End Function

    End Class

End Class

#End Region
