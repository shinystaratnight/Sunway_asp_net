' -----------------------------------------------------------------------
'  <copyright file="ILogWriter.vb" company="intuitive">
'      Copyright © 2015 intuitive. All rights reserved.
'  </copyright>
' -----------------------------------------------------------------------

''' <summary>
''' Interface for logging content
''' </summary>
Public Interface ILogWriter

    ''' <summary>
    ''' Creates a log.
    ''' </summary>
    ''' <param name="module">The module or grouping of the log.</param>
    ''' <param name="title">The title of the log.</param>
    ''' <param name="content">The content for the log.</param>
    Sub Write(ByVal [module] As String, ByVal title As String, ByVal content As String)

End Interface
