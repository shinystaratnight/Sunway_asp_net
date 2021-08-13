' -----------------------------------------------------------------------
'  <copyright file="LogFileWriter.vb" company="intuitive">
'      Copyright © 2015 intuitive. All rights reserved.
'  </copyright>
' -----------------------------------------------------------------------

''' <summary>
''' Creating logs on the file system using existing intuitive methods
''' </summary>
''' <seealso cref="Intuitive.IlogWriter" />
Public Class LogFileWriter
    Implements ILogWriter

    ''' <summary>
    ''' Creates a log using the intuitive AddLogEntry method.
    ''' </summary>
    ''' <param name="module">The module or grouping of the log.</param>
    ''' <param name="title">The title of the log.</param>
    ''' <param name="content">The content for the log.</param>
    Public Sub Write(ByVal [module] As String, ByVal title As String, ByVal content As String) Implements ILogWriter.Write
        Intuitive.FileFunctions.AddLogEntry([module], title, content)
    End Sub

End Class
