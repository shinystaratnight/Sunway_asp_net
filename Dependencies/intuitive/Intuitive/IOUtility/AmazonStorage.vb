Imports System.IO
Imports System.Text
Imports Amazon.S3
Imports Amazon.S3.Model
Imports Amazon.Runtime.Internal.Util

Public Class AmazonStorage

#Region "Properties"

	Private Const S3Url As String = "s3-eu-west-1.amazonaws.com"
	Private ReadOnly Client As IAmazonS3
	Private ReadOnly Bucket As String
	Private Stream As Stream

#End Region

#Region "Constructors"

	''' <summary>
	''' Initilizes a new instance of the <see cref="AmazonStorage"/> class
	''' </summary>
	''' <remarks>Poor mans IoC</remarks>
	Public Sub New(bucket As String)
		Me.New(New AmazonS3Client(Amazon.RegionEndpoint.EUWest1), bucket)
	End Sub

	''' <summary>
	''' Initilizes a new instance of the <see cref="AmazonStorage"/> class
	''' </summary>
	''' <param name="client">The Amazon S3 client</param>
	''' <param name="bucket">The S3 bucket name</param>
	Public Sub New(client As IAmazonS3, bucket As String)
		Me.Client = client
		Me.Bucket = bucket
	End Sub

#End Region

#Region "Save File"

	Public Sub TempSaveFile(Key As String, Path As String)

		If Not KeyExists(Key) Then
			Return
		End If

		Dim Response As GetObjectResponse = Client.GetObject(Bucket, Key)
		Response.WriteResponseStreamToFile(Path & Key)

	End Sub

	Public Function SaveFile(Key As String) As SaveReturn

		Dim saveReturn As New SaveReturn()

		Try
			If Not KeyExists(Key) Then
				saveReturn.Success = False
				Return saveReturn
			End If

			Dim Response As GetObjectResponse = Client.GetObject(Bucket, Key)

			saveReturn.Stream = Response.ResponseStream

		Catch ex As Exception
			saveReturn.Exception = ex
			saveReturn.Success = False
		End Try

		Return saveReturn

	End Function

#End Region

#Region "Store File"

	Public Function StoreFile([Module] As String, Text As String, fileInfo As TextFileInfo, oStream As Stream) As StoreReturn
		Stream = oStream
		Return StoreFile([Module], Text, fileInfo, False)
	End Function

	Public Function StoreFile([Module] As String, Text As String, fileInfo As TextFileInfo, Optional newStream As Boolean = True) As StoreReturn
		Dim storeReturn As New StoreReturn()

		Try
			Dim key As String = SafeKeyName([Module], fileInfo.ToString)

			If KeyExists(key) Then
				key = SafeKeyName([Module], fileInfo.ToString)
			End If

			If newStream Then Stream = New MemoryStream()

			Using Writer As New StreamWriter(Stream, New UnicodeEncoding())

				If newStream Then
					Writer.Write(Text)
					Writer.Flush()
				End If
				Stream.Seek(0, SeekOrigin.Begin)

				Dim PutRequest As New PutObjectRequest()
				With PutRequest
					.BucketName = Bucket
					.Key = key
					.InputStream = Stream
				End With

				Dim Response As PutObjectResponse = Client.PutObject(PutRequest)

				If Response.HttpStatusCode = System.Net.HttpStatusCode.OK Then
					storeReturn.Url = $"Http://{Bucket}.{S3Url}/{PutRequest.Key}"
				Else
					storeReturn.Success = False
				End If

			End Using

		Catch ex As Exception
			storeReturn.Success = False
			storeReturn.Exception = ex
		End Try

		Return storeReturn

	End Function

#End Region

#Region "Keys"

	Public Function KeyExists(key As String) As Boolean

		Dim exists As Boolean = True

		Try
			Client.GetObjectMetadata(Bucket, key)
		Catch ex As AmazonS3Exception
			exists = False
		End Try

		Return exists

	End Function

	Public Function SafeKeyName([Module] As String, Title As String) As String

		Dim sDaySuffix As String
		Select Case Date.Now.Day
			Case 1, 21, 31
				sDaySuffix = "st"
			Case 2, 22
				sDaySuffix = "nd"
			Case 3, 23
				sDaySuffix = "rd"
			Case Else
				sDaySuffix = "th"
		End Select

		Dim sFolderBase As String = [Module] & "/" & Date.Now.ToString("MMM") & "/" & Date.Now.Day.ToString & sDaySuffix & Date.Now.ToString("MMM") & "/"
		Dim sLogFileName As String = String.Concat(FileFunctions.SafeFileName(Title, False), Date.Now.ToString("ddMMyyyyHHmmss") & Date.Now.Millisecond.ToString)

		Return sFolderBase + sLogFileName

	End Function

#End Region

#Region "Helper Classes"

	Public Class StoreReturn

		Public Property Success As Boolean
		Public Property Url As String
		Public Property PublicSecureUrl As String
		Public Property FileSize As Double
		Public Property Exception As Exception

		Public Sub New()
			Success = True
			Url = String.Empty
			PublicSecureUrl = String.Empty
			FileSize = 0
			Exception = Nothing
		End Sub

	End Class

	Public Class SaveReturn

		Public Property Success As Boolean
		Public Property Stream As Stream
		Public Property Exception As Exception

		Public Sub New()
			Success = True
			Stream = Nothing
			Exception = Nothing
		End Sub

		Public Function BaseStream() As Stream

			Dim base As MD5Stream = CType(Stream, MD5Stream)

			If base Is Nothing Then
				Throw New Exception("Stream was not an MD5Stream")
			End If

			Return base

		End Function

	End Class

	Public Class TextFileInfo

		Public Property FilePath As String
		Public Property FileName As String

		Public Overrides Function ToString() As String
			Return FilePath & FileName
		End Function

		Public Sub New(Path As String, Name As String)
			FilePath = Path
			FileName = Name
		End Sub

	End Class

#End Region

End Class