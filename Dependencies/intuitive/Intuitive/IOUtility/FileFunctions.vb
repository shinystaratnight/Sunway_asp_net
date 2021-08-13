Imports System.IO
Imports Intuitive.Functions
Imports System.Text
Imports System.Web
Imports System.Xml
Imports System.Configuration.ConfigurationManager
Imports System.Collections.Generic

Partial Public Class FileFunctions

	''' <summary>
	''' Deletes specified files in the specified path
	''' </summary>
	''' <param name="sPath">The path for the files to be deleted.</param>
	''' <param name="sFiles">The files to be deleted, can contain | delimted files or files with wildcards.</param>
	Public Shared Sub MultiDelete(ByVal sPath As String, ByVal sFiles As String)

		'build up files
		Dim aFileGroups As New Generic.List(Of String)
		If sFiles.Contains("|") Then
			aFileGroups.AddRange(sFiles.Split("|"c))
		Else
			aFileGroups.Add(sFiles)
		End If

		Dim aFiles As New Generic.List(Of String)
		For Each sFileGroup As String In aFileGroups
			If sFileGroup.Contains("*") Then
				aFiles.AddRange(Directory.GetFiles(sPath, sFileGroup))
			Else
				aFiles.Add(sPath & sFileGroup)
			End If
		Next

		'scan, make sure they exist then delete
		For Each sFullPath As String In aFiles
			If File.Exists(sFullPath) Then

				Try
					File.Delete(sFullPath)
				Catch ex As Exception

				End Try
			End If
		Next

	End Sub

	''' <summary>
	''' Makes sure files in a directory aren't readonly, then deletes the directory and all files/subdirectories
	''' </summary>
	''' <param name="sDirectory">The path of the directory to be deleted.</param>
	Public Shared Sub SafeDeleteDirectory(ByVal sDirectory As String)

		If Directory.Exists(sDirectory) Then

			For Each sFile As String In Directory.GetFiles(sDirectory, "*", SearchOption.AllDirectories)
				If (File.GetAttributes(sFile) And FileAttributes.ReadOnly) <> 0 Then File.SetAttributes(sFile, Not FileAttributes.ReadOnly)
			Next

			Directory.Delete(sDirectory, True)
		End If

	End Sub

	''' <summary>
	''' Ensures file is not readonly then deletes file
	''' </summary>
	''' <param name="sFile">The file.</param>
	Public Shared Sub SafeDeleteFile(ByVal sFile As String)

		If File.Exists(sFile) Then

			If (File.GetAttributes(sFile) And FileAttributes.ReadOnly) <> 0 Then File.SetAttributes(sFile, Not FileAttributes.ReadOnly)
			File.Delete(sFile)

		End If

	End Sub

	''' <summary>
	''' Creates folder for specified file path
	''' </summary>
	''' <param name="sFolder">The path for the folder.</param>
	Public Shared Sub CreateFolder(ByVal sFolder As String)

		Dim oDIRInfo As New DirectoryInfo(sFolder)
		If Not oDIRInfo.Exists Then
			oDIRInfo.Create()
		End If
	End Sub

	''' <summary>
	''' Searches all files and subdirectories in specified directory for the specified filter
	''' </summary>
	''' <param name="sDirectory">The directory to search.</param>
	''' <param name="sFilter">The search filter.</param>
	''' <returns></returns>
	Public Shared Function RecursiveFileSearch(ByVal sDirectory As String, Optional ByVal sFilter As String = "*.*") As Generic.List(Of String)

		Dim aList As New ArrayList
		FileFunctions.RecursiveFileSearch(sDirectory, aList, sFilter)

		Dim oReturn As New Generic.List(Of String)

		For Each oFileInfo As FileInfo In aList
			oReturn.Add(oFileInfo.FullName)
		Next

		oReturn.Sort(New FilenameSort)

		Return oReturn

	End Function

	''' <summary>
	''' Builds up a list of files that match the filter in the specified directory and its subdirectories
	''' </summary>
	''' <param name="sDirectory">The directory to search.</param>
	''' <param name="aArrayToFill">Holder array for files to be returned.</param>
	''' <param name="sFilter">The search filter.</param>
	''' <exception cref="System.UnauthorizedAccessException">
	''' </exception>
	Public Shared Sub RecursiveFileSearch(ByRef sDirectory As String,
			ByRef aArrayToFill As ArrayList, Optional ByVal sFilter As String = "*.*")

		Dim oDirectoryInfo As New DirectoryInfo(sDirectory)

		' Get the files for this directory
		Dim oFileInfo() As FileInfo
		Try
			oFileInfo = oDirectoryInfo.GetFiles(sFilter)
		Catch ex As UnauthorizedAccessException
			Throw New Exception(ex.Message)
		End Try

		aArrayToFill.AddRange(oFileInfo)

		' Subdirectories
		Dim oSubdirectoryInfo() As DirectoryInfo
		Try
			oSubdirectoryInfo = oDirectoryInfo.GetDirectories()
		Catch ex As UnauthorizedAccessException
			Throw New Exception(ex.Message)
		End Try

		'Do the recursive stuff
		Dim oIterateDirectory As DirectoryInfo
		For Each oIterateDirectory In oSubdirectoryInfo
			RecursiveFileSearch(oIterateDirectory.FullName, aArrayToFill, sFilter)
		Next
	End Sub

	''' <summary>
	''' Reads a stream to a file
	''' </summary>
	''' <param name="sFilename">The file path of the file to stream to.</param>
	''' <param name="oStream">The stream.</param>
	''' <param name="bResetPosition">if set to <c>true</c>, will reset stream position to 0 after creating a StreamReader.</param>
	Public Shared Sub StreamToFile(ByVal sFilename As String, ByVal oStream As Stream, Optional ByVal bResetPosition As Boolean = True)

		Dim oFile As New FileStream(sFilename, FileMode.Create, FileAccess.Write)
		Dim oStreamWriter As New StreamWriter(oFile)
		Dim oReader As New StreamReader(oStream)
		If bResetPosition Then oStream.Position = 0 'this has to be optional because some streams don't support seeking
		oStreamWriter.Write(oReader.ReadToEnd)
		oStreamWriter.Flush()
		oFile.Close()

	End Sub

	''' <summary>
	''' Converts stream to array of bytes with specified blocksize, then saves them to the specified file.
	''' </summary>
	''' <param name="sFilename">The filename to save the bytestream to.</param>
	''' <param name="oStream">The stream.</param>
	''' <param name="iBlockSize">Size of the block.</param>
	''' <param name="bResetPosition">if set to <c>true</c>, will reset stream position to 0 after creating a StreamReader.</param>
	Public Shared Sub ByteStreamToFile(ByVal sFilename As String, ByVal oStream As Stream, Optional ByVal iBlockSize As Integer = 512,
		Optional ByVal bResetPosition As Boolean = False)

		Dim oFile As New FileStream(sFilename, FileMode.Create, FileAccess.Write)
		If bResetPosition Then oStream.Position = 0 'this has to be optional because some streams don't support seeking

		Dim aBytes(iBlockSize - 1) As Byte

		Dim iNumberOfBytesRead As Integer = 0
		Do
			iNumberOfBytesRead = oStream.Read(aBytes, 0, aBytes.Length)
			oFile.Write(aBytes, 0, iNumberOfBytesRead)

			If iNumberOfBytesRead < iBlockSize Then Exit Do
		Loop

		oFile.Close()

	End Sub

	''' <summary>
	''' Writes text to file with the specified encoding
	''' </summary>
	''' <param name="sFileName">Name of the file.</param>
	''' <param name="sText">The text.</param>
	''' <param name="Encoding">The encoding, System.Text.Encoding.</param>
	Public Shared Sub TextToFile(ByVal sFileName As String, ByVal sText As String, ByVal Encoding As System.Text.Encoding)

		'using so dispose gets called on errors
		Using oFile As New FileStream(sFileName, FileMode.Create, FileAccess.Write)
			Using oStreamWriter As New StreamWriter(oFile, Encoding)
				oStreamWriter.Write(sText)
				oStreamWriter.Flush()
				oStreamWriter.Close()
				oStreamWriter.Dispose()
				oFile.Close()
				oFile.Dispose()
			End Using
		End Using

	End Sub

	''' <summary>
	''' Writes text to file with default encoding
	''' </summary>
	''' <param name="sFileName">Name of the file to write to.</param>
	''' <param name="sText">The text to write to the file.</param>
	Public Shared Sub TextToFile(ByVal sFileName As String, ByVal sText As String)

		'using so dispose gets called on errors
		Using oFile As New FileStream(sFileName, FileMode.Create, FileAccess.Write)
			Using oStreamWriter As New StreamWriter(oFile)
				oStreamWriter.Write(sText)
				oStreamWriter.Flush()
				oStreamWriter.Close()
				oStreamWriter.Dispose()
				oFile.Close()
				oFile.Dispose()
			End Using
		End Using

	End Sub


	''' <summary>
	''' Retrieves the text from a file
	''' </summary>
	''' <param name="sFileName">Name of the file.</param>
	''' <returns></returns>
	Public Shared Function FileToText(ByVal sFileName As String) As String

		Dim sText As String = ""
		Using oFile As New FileStream(sFileName, FileMode.Open, FileAccess.Read, FileShare.Read)
			Using oReader As New StreamReader(oFile)

				sText = oReader.ReadToEnd

				oReader.Close()
				oReader.Dispose()
				oFile.Close()
				oFile.Dispose()

			End Using
		End Using

		Return sText

	End Function

	''' <summary>
	''' Returns a filename with characters that are illegal in windows file system removed.
	''' </summary>
	''' <param name="Base">The base filename.</param>
	''' <param name="RemoveSpaces">if set to <c>true</c>, removes spaces from filename.</param>
	''' <returns></returns>
	Public Shared Function SafeFileName(ByVal Base As String, Optional ByVal RemoveSpaces As Boolean = True) As String

		'remove spaces iof requested
		If RemoveSpaces Then
			Base = Base.Replace(" ", "")
		End If

		'removes chars forbidden in windows file system
		Return Base.Replace("*", "").Replace(".", "").Replace("""", "").Replace("/", "").Replace("\", "").Replace("[", "").Replace("]", "").Replace(":", "").Replace(";", "").Replace(",", "").Replace("|", "").Replace("=", "").Replace("+", "").Replace("?", "").Replace("&", "").Replace("#", "").Replace("%", "").Replace(">", "").Replace("<", "")
	End Function

	''' <summary>
	''' Returns string containing the extention of the filepath
	''' </summary>
	''' <param name="sFilePath">The file path.</param>
	''' <returns></returns>
	<Obsolete("GetFileExtension is obsolete, use Path.GetExtension(sFilepath) instead")>
	Public Shared Function GetFileExtension(ByVal sFilePath As String) As String

		Path.GetExtension(sFilePath)

		Dim oFileNameParts As String() = sFilePath.Split("."c)
		Dim sExtension As String = ""

		If oFileNameParts.Length > 1 Then
			sExtension = oFileNameParts(oFileNameParts.Length - 1)
		End If

		Return sExtension.ToLower

	End Function

	''' <summary>
	''' Removes the file extension.
	''' </summary>
	''' <param name="sFilePath">The file path.</param>
	''' <returns></returns>
	Public Shared Function RemoveFileExtension(ByVal sFilePath As String) As String

		Dim oFileNameParts As String() = sFilePath.Split("."c)
		Dim sFileName As String = ""

		If oFileNameParts.Length > 1 Then
			For i As Integer = 0 To oFileNameParts.Length - 2
				sFileName &= oFileNameParts(i) & "."
			Next
		End If
		sFileName = Functions.Chop(sFileName)

		Return sFileName

	End Function

#Region "Convert File Extension To MIME Type"

#Region "MIME Types Dictionary"

	'believe it or not, there is nothing provided by dot net to this mapping, and the windows registry only supplies mappings for programs that are installed
	'This list is by no means exhaustive! I've only included the most common or important ones.
	''' <summary>
	''' Dictionary of File Extensions to MIME Types
	''' </summary>
	Private Shared MIMETypeDictionary As New Generic.Dictionary(Of String, String) From {
			{"*", "application/octet-stream"},
			{"js", "application/javascript"},
			{"json", "application/json"},
			{"doc", "application/msword"},
			{"dot", "application/msword"},
			{"pdf", "application/pdf"},
			{"ps", "application/postscript"},
			{"rtf", "application/rtf"},
			{"oxt", "vnd.openofficeorg.extension"},
			{"cab", "application/vnd.ms-cab-compressed"},
			{"xls", "application/vnd.ms-excel"},
			{"xlm", "application/vnd.ms-excel"},
			{"xla", "application/vnd.ms-excel"},
			{"xlc", "application/vnd.ms-excel"},
			{"xlt", "application/vnd.ms-excel"},
			{"xlw", "application/vnd.ms-excel"},
			{"xlam", "application/vnd.ms-excel.addin.macroenabled.12"},
			{"xlsb", "application/vnd.ms-excel.sheet.binary.macroenabled.12"},
			{"xlsm", "application/vnd.ms-excel.sheet.macroenabled.12"},
			{"xltm", "application/vnd.ms-excel.template.macroenabled.12"},
			{"ppt", "application/vnd.ms-powerpoint"},
			{"pps", "application/vnd.ms-powerpoint"},
			{"pot", "application/vnd.ms-powerpoint"},
			{"ppam", "application/vnd.ms-powerpoint.addin.macroenabled.12"},
			{"pptm", "application/vnd.ms-powerpoint.presentation.macroenabled.12"},
			{"sldm", "application/vnd.ms-powerpoint.slide.macroenabled.12"},
			{"ppsm", "application/vnd.ms-powerpoint.slideshow.macroenabled.12"},
			{"potm", "application/vnd.ms-powerpoint.template.macroenabled.12"},
			{"docm", "application/vnd.ms-word.document.macroenabled.12"},
			{"dotm", "application/vnd.ms-word.template.macroenabled.12"},
			{"wps", "application/vnd.ms-works"},
			{"wks", "application/vnd.ms-works"},
			{"wcm", "application/vnd.ms-works"},
			{"wdb", "application/vnd.ms-works"},
			{"xps", "application/vnd.ms-xpsdocument"},
			{"pptx", "application/vnd.openxmlformats-officedocument.presentationml.presentation"},
			{"sldx", "application/vnd.openxmlformats-officedocument.presentationml.slide"},
			{"ppsx", "application/vnd.openxmlformats-officedocument.presentationml.slideshow"},
			{"potx", "application/vnd.openxmlformats-officedocument.presentationml.template"},
			{"xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"},
			{"xlst", "application/vnd.openxmlformats-officedocument.spreadsheetml.template"},
			{"docx", "application/vnd.openxmlformats-officedocument.wordprocessingml.document"},
			{"dotx", "application/vnd.openxmlformats-officedocument.wordprocessingml.template"},
			{"tgz", "application/x-compressed"},
			{"gz", "application/x-gzip"},
			{"wri", "application/x-mswrite"},
			{"rar", "application/x-rar-compressed"},
			{"swf", "application/x-shockwave-flash"},
			{"sit", "application/x-stuffit"},
			{"tar", "application/x-tar"},
			{"xhtml", "application/xhtml+xml"},
			{"xht", "application/xhtml+xml"},
			{"xml", "application/xml"},
			{"xsl", "application/xml"},
			{"dtd", "application/xml-dtd"},
			{"xslt", "application/xslt+xml"},
			{"zip", "application/zip"},
			{"au", "audio/basic"},
			{"snd", "audio/basic"},
			{"mid", "audio/mid"},
			{"rmi", "audio/rmi"},
			{"mp4a", "audio/mp4"},
			{"m2a", "audio/mpeg"},
			{"m3a", "audio/mpeg"},
			{"mp2a", "audio/mpeg"},
			{"mp3", "audio/mpeg"},
			{"mpga", "audio/mpeg"},
			{"oga", "audio/ogg"},
			{"ogg", "audio/ogg"},
			{"spx", "audio/ogg"},
			{"aif", "audio/x-aiff"},
			{"aifc", "audio/x-aiff"},
			{"aiff", "audio/x-aiff"},
			{"m3u", "audio/x-mpegurl"},
			{"ra", "audio/x-pn-realaudio"},
			{"ram", "audio/x-pn-realaudio"},
			{"wma", "audio/x-wma"},
			{"wav", "audio/x-wav"},
			{"bmp", "image/bmp"},
			{"cod", "image/cis-cod"},
			{"gif", "image/gif"},
			{"jpe", "image/jpeg"},
			{"jpeg", "image/jpeg"},
			{"jpg", "image/jpg"},
			{"png", "image/png"},
			{"svg", "image/svg+xml"},
			{"svgz", "image/svg+xml"},
			{"tif", "image/tiff"},
			{"tiff", "image/tiff"},
			{"psd", "image/vnd.adobe.photoshop"},
			{"ras", "image/x-cmu-raster"},
			{"cmx", "image/x-cmx"},
			{"ico", "image/x-icon"},
			{"pnm", "image/x-portable-anymap"},
			{"pbm", "image/x-portable-bitmap"},
			{"pgm", "image/x-portable-graymap"},
			{"ppm", "image/x-portable-pixmap"},
			{"rgb", "image/x-rgb"},
			{"xbm", "image/x-xbitmap"},
			{"xpm", "image/x-xpixmap"},
			{"xwd", "image/x-xwindowdump"},
			{"csv", "text/csv"},
			{"css", "text/css"},
			{"htm", "text/html"},
			{"html", "text/html"},
			{"stm", "text/html"},
			{"bas", "text/plain"},
			{"c", "text/plain"},
			{"h", "text/plain"},
			{"txt", "text/plain"},
			{"rtx", "text/richtext"},
			{"tsv", "text/tab-seperated-values"},
			{"htt", "text/webviewhtml"},
			{"htc", "text/x-component"},
			{"vcs", "text/x-vcalendar"},
			{"vcf", "text/x-vcard"},
			{"h264", "video/h264"},
			{"jpgv", "video/jpeg"},
			{"mp4", "video/mp4"},
			{"mp4v", "video/mp4"},
			{"mpg4", "video/mp4"},
			{"mp2", "video/mpeg"},
			{"mpa", "video/mpeg"},
			{"mpe", "video/mpeg"},
			{"mpeg", "video/mpeg"},
			{"mpg", "video/mpeg"},
			{"mpv2", "video/mpeg"},
			{"ogv", "video/ogg"},
			{"mov", "video/quicktime"},
			{"qt", "video/quicktime"},
			{"lsf", "video/x-la-lsf"},
			{"lsx", "video/x-la-lsf"},
			{"asf", "video/x-ms-asf"},
			{"asr", "video/x-ms-asf"},
			{"asx", "video/x-ms-asf"},
			{"avi", "video/x-msvideo"},
			{"movie", "video/x-sgi-movie"},
			{"flr", "x-world/x-vrml"},
			{"vrml", "x-world/x-vrml"},
			{"wrl", "x-world/x-vrml"},
			{"wrz", "x-world/x-vrml"},
			{"xaf", "x-world/x-vrml"},
			{"xof", "x-world/x-vrml"}
		}

#End Region

	''' <summary>
	''' Retrieves MIME Type based on the file extension
	''' </summary>
	''' <param name="sExtension">The file extension.</param>
	''' <param name="bLookInRegistryIfNotKnown">if set to <c>true</c> will look in the registry if the mimetype isn't found in the dictionary.</param>
	''' <returns></returns>
	Public Shared Function ConvertFileExtensionToMIMEType(ByVal sExtension As String, Optional bLookInRegistryIfNotKnown As Boolean = True) As String
		Dim sMIMEType As String = Nothing

		If Not MIMETypeDictionary.TryGetValue(sExtension.ToLower, sMIMEType) Then
			If bLookInRegistryIfNotKnown Then
				sMIMEType = ConvertFileExtensionToMIMETypeFromWindowsRegistry(sExtension)
			Else
				sMIMEType = MIMETypeDictionary("*")
			End If
		End If

		Return sMIMEType

	End Function

	''' <summary>
	''' Gets the MIME Type for a file extension from the registry
	''' </summary>
	''' <param name="sExtension">The file extension.</param>
	''' <returns></returns>
	Public Shared Function ConvertFileExtensionToMIMETypeFromWindowsRegistry(ByVal sExtension As String) As String

		Dim sMIMEType As String = MIMETypeDictionary("*")

		Dim oRegistryKey As Microsoft.Win32.RegistryKey = Microsoft.Win32.Registry.ClassesRoot.OpenSubKey(sExtension.ToLower)
		If oRegistryKey IsNot Nothing AndAlso oRegistryKey.GetValue("Content Type") IsNot Nothing Then
			sMIMEType = oRegistryKey.GetValue("Content Type").ToString
		End If

		Return sMIMEType

	End Function

#End Region

#Region "File And Directory Filter"

	'This function is here because the built in dot net filter options for GetFiles and GetDirectories are complete rubbish
	''' <summary>
	''' Filters files and directories based on the provided filter
	''' </summary>
	''' <param name="oPaths">The paths to check.</param>
	''' <param name="sFilter">The filter.</param>
	''' <param name="bUseRegularExpressions">if set to <c>true</c>, uses regular expressions in the filter.</param>
	''' <param name="iMatchStartOffset">The offset for where to start matching the filename.</param>
	''' <returns></returns>
	Public Shared Function FileAndDirectoryFilter(ByVal oPaths As Generic.IEnumerable(Of String), ByVal sFilter As String,
		Optional ByVal bUseRegularExpressions As Boolean = False, Optional ByVal iMatchStartOffset As Integer = 0) As Generic.List(Of String)

		'create a regex for our filter
		If Not bUseRegularExpressions Then
			sFilter = String.Format("^{0}$", RegularExpressions.Regex.Escape(sFilter).Replace("\|", "|").Replace("\*", ".*?").Replace("\?", "."))
		End If
		Dim oFilterRegex As New RegularExpressions.Regex(sFilter, RegularExpressions.RegexOptions.IgnoreCase)

		'now filter down
		Dim oFilteredPaths As New Generic.List(Of String)
		For Each sFile As String In oPaths
			If oFilterRegex.IsMatch(sFile.Substring(iMatchStartOffset)) Then oFilteredPaths.Add(sFile)
		Next

		'return
		Return oFilteredPaths

	End Function

#End Region

	''' <summary>
	''' 
	''' </summary>
	Public Class TextFileReader

		Private File As System.IO.File
		Private Reader As System.IO.StreamReader
		Public LineNumber As Integer = 0

		''' <summary>
		''' Gets a value indicating whether this <see cref="TextFileReader" /> is done reading.
		''' </summary>
		Public ReadOnly Property Done() As Boolean
			Get
				Return Me.Reader.Peek = -1
			End Get
		End Property

		''' <summary>
		''' Initializes a new instance of the <see cref="TextFileReader"/> class.
		''' </summary>
		''' <param name="Filename">The filename.</param>
		Public Sub New(ByVal Filename As String)
			Me.New(Filename, System.Text.ASCIIEncoding.Default)
		End Sub

		''' <summary>
		''' Initializes a new instance of the <see cref="TextFileReader"/> class.
		''' </summary>
		''' <param name="FileName">Name of the file.</param>
		''' <param name="EncodingType">Type of encoding.</param>
		''' <exception cref="System.Exception">Could not find file with FileName</exception>
		Public Sub New(ByVal FileName As String, EncodingType As System.Text.Encoding)
			If Not File.Exists(FileName) Then Throw New Exception("Could not find file " & FileName)
			Me.Reader = New StreamReader(FileName, EncodingType)
			Me.LineNumber = 1
		End Sub

		''' <summary>
		''' Reads 1 line.
		''' </summary>
		Public Function ReadLine() As String
			Me.LineNumber += 1
			Return Me.Reader.ReadLine
		End Function

		''' <summary>
		''' Reads to end of file.
		''' </summary>
		Public Function ReadToEnd() As String
			Me.LineNumber = -1
			Return Me.Reader.ReadToEnd
		End Function

		''' <summary>
		''' Closes this instance.
		''' </summary>
		Public Sub Close()
			Me.Reader.Close()
		End Sub

		''' <summary>
		''' Allows an object to try to free resources and perform other cleanup operations before it is reclaimed by garbage collection.
		''' </summary>
		Protected Overrides Sub Finalize()
			Me.Close()
			MyBase.Finalize()
		End Sub
	End Class

#Region "File comparison"
	''' <summary>
	''' Checks whether 2 files have the same contents, first by filepath, then by content length, then by compareing content byte hash
	''' </summary>
	''' <param name="sFilePath1">The path for file 1.</param>
	''' <param name="sFilePath2">The path for file 2.</param>
	Public Shared Function SameFileContents(ByVal sFilePath1 As String, ByVal sFilePath2 As String) As Boolean

		' If user has selected the same file as file one and file two..
		If sFilePath1 = sFilePath2 Then Return True

		' User selected two different files; Compare length of file one to file two.
		Dim oFileStream1 As New FileStream(sFilePath1, FileMode.Open)
		Dim oFileStream2 As New FileStream(sFilePath2, FileMode.Open)

		' If the files are not the same length...
		If oFileStream1.Length <> oFileStream2.Length Then
			oFileStream1.Close()
			oFileStream2.Close()

			Return False
		Else
			oFileStream1.Close()
			oFileStream2.Close()
		End If

		' Files are the same length; compare file contents using .NET HMACSHA1 Class

		Dim oFileHash1 As Byte() = ComputeFileHashUsingHMACSHA1(sFilePath1)
		Dim oFileHash2 As Byte() = ComputeFileHashUsingHMACSHA1(sFilePath2)

		Dim bEqual As Boolean = True

		' Make a byte-by-byte comparrison of file one's hash to file two's hash.
		For i As Integer = 0 To oFileHash1.Length

			' If a byte from file one is not equal to a byte from file two..
			If oFileHash1(i) <> oFileHash2(i) Then

				' Files are not equal; byte in file one hash <> byte in file two hash.
				bEqual = False
				Exit For
			End If
		Next

		Return bEqual
	End Function

	''' <summary>
	''' Computes the hash for the specified file using <see cref="System.Security.Cryptography.HMACSHA1"/>
	''' </summary>
	''' <param name="sFilePath">The file path.</param>
	''' <returns></returns>
	Public Shared Function ComputeFileHashUsingHMACSHA1(ByVal sFilePath As String) As Byte()

		Dim oHMACSHA1 As System.Security.Cryptography.HMACSHA1
		Dim oKeyValue As Byte()
		Dim oHashValue As Byte()
		Dim oStream As Stream

		' Call the UnicodeEncoding class' shared GetBytes method to initialize theKeyValue object.
		oKeyValue = (New System.Text.UnicodeEncoding).GetBytes("HashingKey")

		' Call the HMACSHA1 class' constructor method to initialize theKeyValue object.
		oHMACSHA1 = New System.Security.Cryptography.HMACSHA1(oKeyValue, True)

		' Open the Stream.
		oStream = New FileStream(sFilePath, FileMode.Open, FileAccess.Read)

		' Calculate the Hash Value.
		oHashValue = oHMACSHA1.ComputeHash(oStream)

		' Close Stream.
		oStream.Close()

		Return oHashValue
	End Function

#End Region

#Region "Text File writer"

	Public Class TextFileWriter

		Private oFile As FileStream

		''' <summary>
		''' Initializes a new instance of the <see cref="TextFileWriter"/> class.
		''' </summary>
		''' <param name="Filename">The filename.</param>
		''' <param name="Overwrite">Not used</param>
		Public Sub New(ByVal Filename As String, Optional ByVal Overwrite As Boolean = False)
			oFile = New FileStream(Filename, FileMode.Create, FileAccess.Write)
		End Sub

		''' <summary>
		''' Writesthe specified line to the TextFileWriter file.
		''' </summary>
		''' <param name="Line">The line to write.</param>
		Public Sub Writeline(ByVal Line As String)

			Dim oUTF8 As New System.Text.UTF8Encoding(False)

			Dim Writer As New StreamWriter(oFile, oUTF8)
			Writer.WriteLine(Line)
			Writer.Flush()
		End Sub

		''' <summary>
		''' Closes the file
		''' </summary>
		Public Sub Done()
			oFile.Close()
		End Sub

	End Class

#End Region

#Region "gzip"

	''' <summary>
	''' Compresses the source file and saves to the destination file
	''' </summary>
	''' <param name="SourceFilePath">The source file path.</param>
	''' <param name="DestinationFilePath">The destination file path.</param>
	''' <returns></returns>
	Public Shared Function GZIPFile(ByVal SourceFilePath As String, ByVal DestinationFilePath As String) As Boolean

		Dim bSuccess As Boolean = True

		Try

			'set up io streams
			Using oFileStream As New FileStream(DestinationFilePath, FileMode.Create)
				Using oOutputStream As New Compression.GZipStream(oFileStream, Compression.CompressionMode.Compress)
					Using oInputStream As New FileStream(SourceFilePath, FileMode.Open)

						'copy
						oInputStream.CopyTo(oOutputStream)

						'flush and close
						oInputStream.Flush()
						oOutputStream.Flush()
						oInputStream.Close()
						oOutputStream.Close()

					End Using
				End Using
			End Using

		Catch ex As Exception
			bSuccess = False
			Intuitive.FileFunctions.AddLogEntry("GZip", "GZipping Exception", ex.ToString, True, "paul@intuitivesystems.co.uk")
		End Try

		Return bSuccess

	End Function

#End Region

End Class

#Region "Sort objects"

''' <summary>
''' Sorts file names
''' </summary>
''' <seealso cref="T:System.Collections.Generic.IComparer{T}" />
Public Class FilenameSort
	Implements IComparer(Of String)

	''' <summary>
	''' Compares 2 file names
	''' </summary>
	''' <param name="x">The first file name.</param>
	''' <param name="y">The second file name.</param>
	''' <returns></returns>
	Public Function Compare(ByVal x As String, ByVal y As String) As Integer Implements System.Collections.Generic.IComparer(Of String).Compare
		Dim iXSlashes As Integer = x.Split("\"c).Length - 1
		Dim iYSlashes As Integer = y.Split("\"c).Length - 1

		If iXSlashes < iYSlashes Then
			Return -1
		ElseIf iXSlashes > iYSlashes Then
			Return 1
		Else
			Return String.Compare(x, y)
		End If
	End Function
End Class

#End Region