Imports System.Web.UI

Public Class PageBase
	Inherits System.Web.UI.Page
    Private sExtensions As String = ""



	Private Sub Page_Load(sender As Object, e As System.EventArgs) Handles Me.Load

		If Me.Request.RawUrl.ToLower.IndexOf("logallxml") > -1 Then
			BookingBase.LogAllXML = True
		End If

		'form functions
		If Me.Request.RawUrl.ToLower.IndexOf("executeformfunction") > -1 Then
			Me.ExecuteFormFunction()
		End If

		'this will directly email out anything from the latest booking journey searches that are stored on the web support class
		If Me.Request.RawUrl.ToLower.IndexOf("sendlogs") > -1 Then
			'can have form function part of query string in format ?emailsearchtimes=email@email.com?formfunction=etc
			'so strip this out if this has happened
			Dim sEmailTo As String = HttpContext.Current.Request.QueryString("sendlogs")
			If sEmailTo.Contains("?") Then sEmailTo = sEmailTo.Split("?"c)(0)
			WebSupportToolbar.EmailSearchLogs(sEmailTo)
		End If

        If Me.Request.QueryString.ToSafeString.ToLower.Contains("clearextensionscache") Then
            Extensions.ClearCache()
        End If

        Dim oExtensions As New Extensions(Me.Request.Url)
        Me.sExtensions = oExtensions.RetrieveExtensions()

	End Sub


#Region "formfunction execute"

	Private Function GetAssembly(ByVal Name As String) As System.Reflection.Assembly
		For Each oAssembly As System.Reflection.Assembly In AppDomain.CurrentDomain.GetAssemblies
			If oAssembly.ManifestModule.Name.ToLower = Name.ToLower & ".dll" Then
				Return oAssembly
			End If
		Next
		Return Nothing
	End Function

	Public Sub ExecuteFormFunction()

		' get object and method name, either
		'	a) method (of form with no / or .)
		'	b) control/method (control on form)
		'	c) object.method (object in current assembly) or namespace.object.method
		'	d) =assembly.class.method

		Dim sFunction As String = Me.Request.QueryString("function")
		Dim oControl As Object
		Dim oType As Type = Nothing
		Dim oMethod As System.Reflection.MethodInfo

		If Not (sFunction.Contains("/") OrElse sFunction.Contains(".")) Then
			oControl = Me
			oMethod = oControl.GetType().GetMethod(sFunction)
		ElseIf sFunction.Contains("/") Then
			oControl = Me.FindControl(sFunction.Split("/"c)(0))
			oMethod = oControl.GetType().GetMethod(sFunction.Split("/"c)(1))
		ElseIf sFunction.StartsWith("=") Then

			Dim sAssembly As String = sFunction.Split("."c)(0).Substring(1)
			Dim sClassName As String = sFunction.Split("."c)(1)
			Dim sMethod As String = sFunction.Split("."c)(2)
            Dim oAssembly As System.Reflection.Assembly = Me.GetAssembly(sAssembly)

            Dim assemblyIsNull As Boolean = oAssembly is Nothing

            If sAssembly = "IntuitiveWeb" Then
                oType = oAssembly.GetType("Intuitive.Web." & sClassName)
            Else
                oType = oAssembly.GetType(sAssembly & "." & sClassName)
            End If

            oControl = oType
            oMethod = oType.GetMethod(sMethod)

        Else
            Dim sNamespace As String = Me.GetType().BaseType.Namespace
            Dim sClassName As String = ""
            Dim sMethod As String = ""

            If sFunction.Split("."c).Length = 3 Then
                sNamespace = sNamespace & "." & sFunction.Split("."c)(0)
                sClassName = sFunction.Split("."c)(1)
                sMethod = sFunction.Split("."c)(2)
            Else
                sClassName = sFunction.Split("."c)(0)
                sMethod = sFunction.Split("."c)(1)
            End If

            oType = Me.GetType.BaseType.Assembly.GetType(sNamespace & "." & sClassName)
            oControl = oType
            oMethod = oType.GetMethod(sMethod)
        End If


        'check
        If oControl Is Nothing Then Throw New Exception("Could not execute form function " & Me.Request.QueryString.ToString)


        ' get params (also deals with the case that we expect one paramter, and it's blank)
        Dim Params() As String
		If Me.Request.Form("params") <> "" OrElse (oMethod.GetParameters.Length = 1 AndAlso Me.Request.Form("params") IsNot Nothing) Then
			'seemingly the data is decoded as part of being posted
			Params = Me.Request.Form("params").Split("|"c)
		Else
			Params = CType(Array.CreateInstance("".GetType(), 0), String())
		End If

        'checks
        If oMethod.GetParameters.Length <> Params.Length Then Throw New Exception("Invalid number of parameters passed")

        'cache the page name for this request so any classes needing it have access
        HttpContext.Current.Items("__current_request_pagename") = Me.Request.Form("pagename")

        'build up parameter object
        Dim oParams(Params.Length - 1) As Object
        For i As Integer = 0 To Params.Length - 1

            Select Case oMethod.GetParameters(i).ParameterType.Name
                Case "String"
                    oParams(i) = Params(i).ToString.Replace("/\pipe\/", "|")
                Case "DateTime"
                    oParams(i) = Intuitive.DateFunctions.SafeDate(Params(i).ToString)
                Case "Int32"
                    oParams(i) = Intuitive.Functions.SafeInt(Params(i).ToString)
                Case "Decimal"
                    oParams(i) = Intuitive.Functions.SafeDecimal(Params(i).ToString)
                Case "Boolean"
                    oParams(i) = Intuitive.Functions.SafeBoolean(Params(i).ToString)
                Case Else
                    Select Case oMethod.GetParameters(i).ParameterType.BaseType.Name
                        Case "Enum"
                            oParams(i) = Intuitive.Functions.SafeEnum(oMethod.GetParameters(i).ParameterType, Params(i).ToString)
                    End Select
            End Select

        Next


        'invoke method (slightly different depending on whether we have a type or not)
        Dim oReturn As Object
        If oType Is Nothing Then
            oReturn = oMethod.Invoke(oControl, oParams)
        Else
            oReturn = oMethod.Invoke(Activator.CreateInstance(oType), oParams)
        End If

        If oReturn IsNot Nothing Then
            Response.Write(oReturn.ToString)
        End If

        Response.End()

    End Sub


#End Region

#Region "Extensions"

    Private Sub AddExtensionsControl()

        If Not String.IsNullOrEmpty(Me.sExtensions) Then
            Dim oControl As New LiteralControl(Me.sExtensions)
            Me.Controls.Add(oControl)
        End If

    End Sub

#End Region



End Class
