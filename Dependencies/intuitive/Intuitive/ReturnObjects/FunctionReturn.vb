''' <summary>
''' Class representing a function return
''' </summary>
Public Class FunctionReturn

    ''' <summary>
    ''' Indicates whether the function was a success.
    ''' </summary>
    Public Success As Boolean

    ''' <summary>
    ''' List of warnings from the function
    ''' </summary>
    Private aWarnings As ArrayList

    ''' <summary>
    ''' Type of the function return
    ''' </summary>
    Public Type As String

    ''' <summary>
    ''' Result object from the function
    ''' </summary>
    Public Result As Object

    ''' <summary>
    ''' Gets the warnings.
    ''' </summary>
    Public ReadOnly Property Warnings() As ArrayList
        Get
            Return aWarnings
        End Get
    End Property

    ''' <summary>
    ''' Initializes a new instance of the <see cref="FunctionReturn"/> class.
    ''' </summary>
    Public Sub New()
        Me.Success = True
        aWarnings = New ArrayList
    End Sub

    ''' <summary>
    ''' Initializes a new instance of the <see cref="FunctionReturn"/> class.
    ''' </summary>
    ''' <param name="Success">Sets value of <see cref="Success"/>.</param>
    Public Sub New(ByVal Success As Boolean)
		Me.Success = Success
		aWarnings = New ArrayList
    End Sub

    ''' <summary>
    ''' Adds warning to <see cref="Warnings"/>.
    ''' </summary>
    ''' <param name="sWarning">The warning to add.</param>
    Public Sub AddWarning(ByVal sWarning As String)
        Me.Warnings.Add(sWarning)
        Me.Success = False
    End Sub

    ''' <summary>
    ''' Adds formatted warning to <see cref="Warnings"/>.
    ''' </summary>
    ''' <param name="sWarning">The warning string, can be formatted using string formatting e.g. {0}.</param>
    ''' <param name="aParams">The parameters to pass into the warning string.</param>
    Public Sub AddWarning(ByVal sWarning As String, ByVal ParamArray aParams() As Object)
        Me.AddWarning(String.Format(sWarning, aParams))
    End Sub

End Class
