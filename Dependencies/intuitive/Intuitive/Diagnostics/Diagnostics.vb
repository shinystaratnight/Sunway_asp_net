Imports diag = System.Diagnostics

Public Class Diagnostics

	''' <summary>
	''' Returns a stacktrace
	''' </summary>
	''' <param name="IncludeSystem">if set to <c>true</c>, includes system stack trace.</param>
	''' <returns></returns>
	Public Shared Function StackTrace(Optional ByVal IncludeSystem As Boolean = False) As Generic.List(Of String)

		'1. get frames
		Dim aStackFrames() As diag.StackFrame = New diag.StackTrace().GetFrames()

		'2. build return (skip the 0th element - as that is this routine :-))
		Dim aReturn As New Generic.List(Of String)
		For i As Integer = 1 To aStackFrames.Length - 1

			Dim oStackFrame As diag.StackFrame = aStackFrames(i)

			Dim bIsSystem As Boolean = oStackFrame.GetMethod().DeclaringType.FullName.ToLower.StartsWith("system") OrElse 
				oStackFrame.GetMethod().DeclaringType.FullName.ToLower.StartsWith("microsoft")

			If IncludeSystem OrElse Not bIsSystem Then
				aReturn.Add(oStackFrame.GetMethod().DeclaringType.FullName & "." & oStackFrame.GetMethod().Name)
			End If

		Next

		Return aReturn

	End Function

End Class