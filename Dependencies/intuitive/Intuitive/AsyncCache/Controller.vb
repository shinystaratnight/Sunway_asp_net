Imports System.Text
Imports System.IO
Imports System.Xml
Imports System.Web.Caching
Imports System.Threading
Imports System.Runtime.InteropServices
Imports System.Runtime.Remoting.Messaging
Imports System.Runtime.Serialization.Formatters.Binary
Imports System.Text.RegularExpressions
Imports Intuitive.Functions
Imports Newtonsoft.Json

Namespace AsyncCache

	''' <summary>
	''' Class representing an asynchronous controller used for caching
	''' </summary>
	''' <typeparam name="tObjectType">The type of the object type.</typeparam>
	Public Class Controller(Of tObjectType As Class)

		''' <summary>
		''' The object used for synclocking
		''' </summary>
		Public Shared LockObject As New Object

		''' <summary>
		''' Enum of possible status of <see cref="Container"/>
		''' </summary>
		Public Enum eStatus
			Empty
			Loading
			Passive
		End Enum

		''' <summary>
		''' Gets the name of the AsynchCache.
		''' </summary>
		Public Shared ReadOnly Property StoreCacheName(sCacheKey As String) As String
			Get
				Return "AsynchCache." & sCacheKey
			End Get
		End Property

		''' <summary>
		''' Gets the location of the local cache store.
		''' </summary>
		Public Shared ReadOnly Property LocalCacheStore As String
			Get
				Return System.Web.Hosting.HostingEnvironment.MapPath("~\LocalCacheStore")
			End Get
		End Property

		''' <summary>
		''' Generates hash key based on sql and it's parameters
		''' </summary>
		''' <param name="SQL">The SQL.</param>
		''' <param name="params">The parameters for the sql.</param>
		Public Shared Function GenerateKey(SQL As String, ParamArray params() As Object) As String

			Dim oSB As New StringBuilder
			oSB.Append(SQL)
			For Each oParam As Object In params
				oSB.Append("|").Append(oParam.ToString)
			Next
			Return GetType(tObjectType).Name & "." & SQL & "." & Math.Abs(oSB.ToString.GetHashCode).ToString

		End Function

		''' <summary>
		''' Retrieves object from the cache, if it's not in the cache, builds the cache, 
		''' otherwise returns the object and refreshes the cache asynchronously.
		''' </summary>
		''' <param name="CacheKey">The cache key.</param>
		''' <param name="CacheMinutes">How long to store the cache for in minutes.</param>
		''' <param name="oCacheFunc">The function used to build the cache.</param>
		Public Shared Function GetCache(
		 CacheKey As String,
		 CacheMinutes As Decimal,
		 oCacheFunc As Func(Of tObjectType)) As tObjectType

			Return GetCache(CacheKey, CacheMinutes, oCacheFunc, String.Empty, String.Empty, False, True, "")

		End Function

		''' <summary>
		''' Retrieves object from the cache, if it's not in the cache, builds the cache, 
		''' otherwise returns the object and refreshes the cache asynchronously.
		''' </summary>
		''' <param name="CacheKey">The cache key.</param>
		''' <param name="CacheMinutes">How long to store the cache for in minutes.</param>
		''' <param name="oCacheFunc">The function used to build the cache.</param>
		''' <param name="sExcludeRegex">Regex used to determine whether we want to ignore an xml result when we store locally.</param>
		Public Shared Function GetCache(
		 CacheKey As String,
		 CacheMinutes As Decimal,
		 oCacheFunc As Func(Of tObjectType),
		 sExcludeRegex As String) As tObjectType

			Return GetCache(CacheKey, CacheMinutes, oCacheFunc, String.Empty, String.Empty, False, True, sExcludeRegex)

		End Function

		''' <summary>
		''' Retrieves object from the cache, if it's not in the cache, builds the cache, 
		''' otherwise returns the object and refreshes the cache asynchronously.
		''' </summary>
		''' <param name="CacheKey">The cache key.</param>
		''' <param name="CacheMinutes">How long to store the cache for in minutes.</param>
		''' <param name="oCacheFunc">The function used to build the cache.</param>
		''' <param name="RecordCallback">If set to <c>true</c>, store an onRemoveCallback in the cache as well.</param>
		Public Shared Function GetCache(
		 CacheKey As String,
		 CacheMinutes As Decimal,
		 oCacheFunc As Func(Of tObjectType),
		 RecordCallback As Boolean) As tObjectType

			Return GetCache(CacheKey, CacheMinutes, oCacheFunc, String.Empty, String.Empty, RecordCallback, True, "")

		End Function

		''' <summary>
		''' Retrieves object from the cache, if it's not in the cache, builds the cache, 
		''' otherwise returns the object and refreshes the cache asynchronously.
		''' </summary>
		''' <param name="CacheKey">The cache key.</param>
		''' <param name="CacheMinutes">How long to store the cache for in minutes.</param>
		''' <param name="oCacheFunc">The function used to build the cache.</param>
		''' <param name="RecordCallback">If set to <c>true</c>, store an onRemoveCallback in the cache as well.</param>
		''' <param name="StoreLocally">If set to <c>true</c>, will also store the cache locally.</param>
		Public Shared Function GetCache(
		 CacheKey As String,
		 CacheMinutes As Decimal,
		 oCacheFunc As Func(Of tObjectType),
		 RecordCallback As Boolean,
		 StoreLocally As Boolean) As tObjectType

			Return GetCache(CacheKey, CacheMinutes, oCacheFunc, String.Empty, String.Empty, RecordCallback, StoreLocally, "")

		End Function

		''' <summary>
		''' Retrieves object from the cache, if it's not in the cache, builds the cache, 
		''' otherwise returns the object and refreshes the cache asynchronously.
		''' </summary>
		''' <param name="CacheKey">The cache key.</param>
		''' <param name="CacheMinutes">How long to store the cache for in minutes.</param>
		''' <param name="oCacheFunc">The function used to build the cache.</param>
		''' <param name="SMTPHost">Not used.</param>
		''' <param name="SystemEmail">Not used.</param>
		''' <param name="RecordCallback">If set to <c>true</c>, store an onRemoveCallback in the cache as well.</param>
		''' <param name="StoreLocally">If set to <c>true</c>, will also store the cache locally.</param>
		''' <param name="ExcludeRegex">Regex used to determine whether we want to ignore an xml result when we store locally.</param>
		Public Shared Function GetCache(
		 CacheKey As String,
		 CacheMinutes As Decimal,
		 oCacheFunc As Func(Of tObjectType),
		 SMTPHost As String,
		 SystemEmail As String,
		 RecordCallback As Boolean,
		 StoreLocally As Boolean,
		 ExcludeRegex As String) As tObjectType

			SyncLock LockObject

				Dim o As Container = Functions.GetCache(Of Container)(CacheKey)

				If o Is Nothing Then

					'get a new cache object and wait
					o = New Container
					o.Load(CacheKey, oCacheFunc, CacheMinutes, RecordCallback, StoreLocally, ExcludeRegex)

				Else

					Dim dNow As DateTime = Date.Now
					If o.Status = eStatus.Empty OrElse (o.Status = eStatus.Passive AndAlso o.ExpiryDate < dNow) Then
						Debug.WriteLine("refresh")
						o.Refresh(CacheKey, CacheMinutes, oCacheFunc, RecordCallback, StoreLocally, ExcludeRegex)
					End If

				End If

				Return o.Item

			End SyncLock

		End Function

		''' <summary>
		''' Add a new object to the cache.
		''' </summary>
		''' <param name="CacheName">Name of the cache.</param>
		''' <param name="ContainerObject">The container object.</param>
		''' <param name="CacheMinutes">How long to store the cache for in minutes.</param>
		''' <param name="StoreLocally">If set to <c>true</c>, will also store the cache locally.</param>
		''' <param name="RecordCallback">If set to <c>true</c>, store an onRemoveCallback in the cache as well.</param>
		''' <param name="ExcludeRegex">Regex used to determine whether we want to ignore an xml result when we store locally.</param>
		Public Shared Sub AddToCache(
		  CacheName As String,
		  ContainerObject As Container,
		  CacheMinutes As Integer,
		  StoreLocally As Boolean,
		  RecordCallback As Boolean,
		  ExcludeRegex As String)

			'something to record the clear down
			Dim onRemove As CacheItemRemovedCallback
			onRemove = New CacheItemRemovedCallback(AddressOf RemovedCallback)

			Dim dExpiration As Date = Date.Now.AddMinutes(CacheMinutes)
			If CacheMinutes = 0 Then dExpiration = System.Web.Caching.Cache.NoAbsoluteExpiration

			'cache the stuff locally, so we can reload quickly
			Dim bValid As Boolean = True
			If StoreLocally Then
				bValid = LocalStore.Serialize(StoreCacheName(CacheName), ContainerObject.Item, ExcludeRegex)
			End If

			'store in the cache (if the serializer comes back as valid)
			If bValid Then
				If RecordCallback Then
					System.Web.HttpRuntime.Cache.Insert(CacheName, ContainerObject, Nothing, dExpiration, Nothing,
														CacheItemPriority.Default, onRemove)
				Else
					System.Web.HttpRuntime.Cache.Insert(CacheName, ContainerObject, Nothing, dExpiration, Nothing)
				End If
			End If

		End Sub

		''' <summary>
		''' Function to be called when an item is removed from the cache
		''' </summary>
		''' <param name="k">The key.</param>
		''' <param name="v">The object removed.</param>
		''' <param name="r">The removal reason.</param>
		''' <exception cref="System.Exception">serialization empty</exception>
		Public Shared Sub RemovedCallback(k As String, v As Object, r As CacheItemRemovedReason)

			'If Not settings.Settings.Config.LogRunTimeCacheExit Then Return

			Dim sLog As String = String.Empty
			Try
				sLog = JsonConvert.SerializeObject(v)
				If sLog = String.Empty Then Throw New Exception("serialization empty")
			Catch ex As Exception
				sLog = ex.Message
			End Try

			'log the fact that this has been removed. This is for audit
			Intuitive.FileFunctions.AddLogEntry("CacheRemoval", r.ToString & "_" & k.ToString, sLog)

		End Sub

		''' <summary>
		''' Class used to store and retrieve locally cached objects
		''' </summary>
		Public Class LocalStore

			''' <summary>
			''' Serializes the specified object and saves it to a textfile in the <see cref="LocalCacheStore"/>
			''' </summary>
			''' <param name="CacheKey">The cache key.</param>
			''' <param name="CacheItem">The object to cache.</param>
			''' <param name="ExcludeRegex">Regex used to determine whether we want to ignore an xml result when we store locally.</param>
			Public Shared Function Serialize(CacheKey As String, CacheItem As tObjectType, ExcludeRegex As String) As Boolean

				'only do this if we have a local cache store, otherwise we force console applications to fall over
				If LocalCacheStore Is Nothing Then Return True

				Dim sLog As String = String.Empty

				Try

					If GetType(tObjectType).IsSerializable Then
						Dim MemoryStream As New MemoryStream()
						Dim binaryFormatter As New BinaryFormatter()
						binaryFormatter.Serialize(MemoryStream, CacheItem)
						MemoryStream.Flush()
						MemoryStream.Position = 0
						sLog = Convert.ToBase64String(MemoryStream.ToArray())
					Else
						'special case with XML documents
						If GetType(tObjectType) = GetType(XmlDocument) Then
							Dim oXML As XmlDocument = CType(Convert.ChangeType(CacheItem, GetType(XmlDocument)), XmlDocument)
							sLog = oXML.InnerXml
						End If

						'now run our regex to decide if we should exclude this XML from being stored
						'eg IVC lookups sometimes fail but we have no way of knowing
						'instead we pass some criteria through, if the regex matches then we ignore it
						If Not ExcludeRegex = "" Then
							Dim oRegex As New Regex(ExcludeRegex)
							If oRegex.IsMatch(sLog) Then
								Return False
							End If
						End If

					End If

				Catch ex As Exception

				End Try

				'create the local cache folder location
				Dim sCacheFolder As String = LocalCacheStore

				Intuitive.FileFunctions.CreateFolder(sCacheFolder)

				'store the contents locally to reload when the app pool drops
				Dim sCacheLocation As String = String.Format("{0}\{1}.txt", sCacheFolder, CacheKey)

				Intuitive.FileFunctions.TextToFile(sCacheLocation, sLog)

				Return True

			End Function

			''' <summary>
			''' Gets the contents of a file in the local cache, deserializes it into an object
			''' </summary>
			''' <param name="CacheKey">The cache key.</param>
			''' <exception cref="System.Exception">invalid type for local cache deserialization</exception>
			Public Shared Function Deserialize(CacheKey As String) As tObjectType

				'this function takes the base 64 string and deserializes back into an object

				Try

					'try to find the file
					Dim sLocalCacheStoreFile As String = String.Format("{0}\{1}.txt", LocalCacheStore, StoreCacheName(CacheKey))
					If Not File.Exists(sLocalCacheStoreFile) Then Return Nothing

					'get the contents of the file
					Dim sLocalCacheStoreFileContents As String = Intuitive.FileFunctions.FileToText(sLocalCacheStoreFile)

					If GetType(tObjectType).IsSerializable Then
						'deserialize back into an object
						Dim b() As Byte = Convert.FromBase64String(sLocalCacheStoreFileContents)
						Dim stream As New MemoryStream(b)

						Dim formatter As New BinaryFormatter()
						stream.Seek(0, SeekOrigin.Begin)
						Return CType(formatter.Deserialize(stream), tObjectType)

					ElseIf GetType(tObjectType) = GetType(XmlDocument) Then
						'special case
						Dim oXML As New XmlDocument
						oXML.LoadXml(sLocalCacheStoreFileContents)
						Return CType(Convert.ChangeType(oXML, GetType(tObjectType)), tObjectType)
					Else
						Throw New Exception("invalid type for local cache deserialization")
					End If

				Catch ex As Exception
					Return Nothing
				End Try

			End Function

		End Class

		''' <summary>
		''' Class used to invoke functions
		''' </summary>
		Public Class Invoker

			''' <summary>
			''' Invokes the specified function
			''' </summary>
			''' <param name="oFunc">The function to invoke.</param>
			''' <param name="threadId">The thread identifier.</param>
			Public Function Go(oFunc As Func(Of tObjectType), <Out()> ByRef threadId As Integer) As tObjectType
				threadId = Thread.CurrentThread.ManagedThreadId()
				Return oFunc.Invoke()
			End Function

		End Class

		''' <summary>
		''' Class representing an object to be stored in the cache
		''' </summary>
		Public Class Container

			''' <summary>
			''' The object to be cached
			''' </summary>
			Public Item As tObjectType
			''' <summary>
			''' The status of the container
			''' </summary>
			Public Status As eStatus = eStatus.Empty
			''' <summary>
			''' The date when the cache will expire
			''' </summary>
			Public ExpiryDate As DateTime = DateFunctions.EmptyDate
			''' <summary>
			''' The SMTP host to send error emails from
			''' </summary>
			Public SMTPHost As String = String.Empty
			''' <summary>
			''' The email address to send error emails to
			''' </summary>
			Public SystemEmail As String = String.Empty

			''' <summary>
			''' Try to load cache from local cache store, if it's not in the local cache, 
			''' we build the cache using the cache building function and store the results.
			''' </summary>
			''' <param name="CacheKey">The cache key.</param>
			''' <param name="oCacheFunc">The function to build the cache.</param>
			''' <param name="CacheMinutes">How long to store the cache for in minutes.</param>
			''' <param name="RecordCallback">If set to <c>true</c>, store an onRemoveCallback in the cache as well.</param>
			''' <param name="StoreLocally">If set to <c>true</c>, will also store the cache locally.</param>
			''' <param name="ExcludeRegex">Regex used to determine whether we want to ignore an xml result when we store locally.</param>
			Public Sub Load(CacheKey As String,
			 oCacheFunc As Func(Of tObjectType),
			 CacheMinutes As Decimal,
			 RecordCallback As Boolean,
			 StoreLocally As Boolean,
			 ExcludeRegex As String)

				'load from a local file if we can
				Dim bLoadedFromLocalFile As Boolean = True
				Dim oCacheItem As tObjectType = LocalStore.Deserialize(CacheKey)

				'if we cannot reload from file then load by calling the function
				If oCacheItem Is Nothing Then
					oCacheItem = oCacheFunc.Invoke
					bLoadedFromLocalFile = False
				End If

				'once we have an object then load into the cache
				Me.LoadCacheContent(CacheKey, oCacheItem, CacheMinutes, StoreLocally AndAlso Not bLoadedFromLocalFile, RecordCallback, ExcludeRegex)

				'refresh if we have loaded from file
				If bLoadedFromLocalFile Then Me.Refresh(CacheKey, CacheMinutes, oCacheFunc, RecordCallback, StoreLocally, ExcludeRegex)

			End Sub

			''' <summary>
			''' Loads item into the cache.
			''' </summary>
			''' <param name="CacheKey">The cache key.</param>
			''' <param name="CacheItem">The item to cache.</param>
			''' <param name="CacheMinutes">How long to store the cache for in minutes.</param>
			''' <param name="StoreLocally">If set to <c>true</c>, will also store the cache locally.</param>
			''' <param name="RecordCallBack">If set to <c>true</c>, store an onRemoveCallback in the cache as well.</param>
			''' <param name="ExcludeRegex">Regex used to determine whether we want to ignore an xml result when we store locally.</param>
			Private Sub LoadCacheContent(CacheKey As String, CacheItem As tObjectType, CacheMinutes As Decimal,
			  StoreLocally As Boolean, RecordCallBack As Boolean, ExcludeRegex As String)

				'set the expiry based on the cache minutes
				Me.ExpiryDate = Date.Now.AddSeconds(CacheMinutes * 60)

				'load the item
				Me.Item = CacheItem

				'reset the status to passive
				Me.Status = eStatus.Passive

				'store in the cache. 0 is deliberate so it does not drop from the cache
				AddToCache(CacheKey, Me, SafeInt(CacheMinutes), StoreLocally, RecordCallBack, ExcludeRegex)

			End Sub

			''' <summary>
			''' Rebuilds specified cache
			''' </summary>
			''' <param name="CacheKey">The cache key.</param>
			''' <param name="CacheMinutes">How long to store the cache for in minutes.</param>
			''' <param name="oCacheFunc">The function to build the cache.</param>
			''' <param name="RecordCallback">If set to <c>true</c>, store an onRemoveCallback in the cache as well.</param>
			''' <param name="StoreLocally">If set to <c>true</c>, will also store the cache locally.</param>
			''' <param name="ExcludeRegex">Regex used to determine whether we want to ignore an xml result when we store locally.</param>
			Public Sub Refresh(CacheKey As String, CacheMinutes As Decimal, oCacheFunc As Func(Of tObjectType),
			 RecordCallback As Boolean, StoreLocally As Boolean, ExcludeRegex As String)

				'set the status so nothing else tries to change it
				Me.Status = eStatus.Loading
				Intuitive.Functions.AddToCache(CacheKey, Me, SafeInt(CacheMinutes))

				Dim iThreadId As Integer

				'async reload the stuff
				Dim oInvoker As New Invoker()
				Dim oCaller As New AsyncMethodCaller(AddressOf oInvoker.Go)

				Dim oCallBackMeta As New CallBackMeta
				oCallBackMeta.CacheKey = CacheKey
				oCallBackMeta.CacheMinutes = CacheMinutes
				oCallBackMeta.SMTPHost = Me.SMTPHost
				oCallBackMeta.SystemEmail = Me.SystemEmail
				oCallBackMeta.RecordCallback = RecordCallback
				oCallBackMeta.Storelocally = StoreLocally
				oCallBackMeta.ExcludeRegex = ExcludeRegex

				Dim oResult As IAsyncResult = oCaller.BeginInvoke(oCacheFunc, iThreadId, AddressOf CallbackMethod, oCallBackMeta)

			End Sub

			''' <summary>
			''' Callback method to perform on asynchronous results
			''' </summary>
			''' <param name="AsyncResult">The asynchronous result.</param>
			Private Sub CallbackMethod(AsyncResult As IAsyncResult)

				Dim oCallBackMeta As CallBackMeta = Nothing

				Try

					' Retrieve the delegate.
					Dim oResult As AsyncResult = CType(AsyncResult, AsyncResult)
					Dim oCaller As AsyncMethodCaller = CType(oResult.AsyncDelegate, AsyncMethodCaller)

					oCallBackMeta = CType(AsyncResult.AsyncState, CallBackMeta)

					Dim iThreadId As Integer = 0
					Dim oCacheItem As tObjectType = oCaller.EndInvoke(iThreadId, AsyncResult)

					Me.LoadCacheContent(oCallBackMeta.CacheKey, oCacheItem, oCallBackMeta.CacheMinutes, oCallBackMeta.Storelocally,
					  oCallBackMeta.RecordCallback, oCallBackMeta.ExcludeRegex)
					Debug.WriteLine("reload content")

				Catch ex As Exception
					Me.SendCallBackErrorEmail(oCallBackMeta, ex)

				End Try

			End Sub

			''' <summary>
			''' Delegate function for async method
			''' </summary>
			''' <param name="oFunc">The function.</param>
			''' <param name="threadId">The thread identifier.</param>
			Private Delegate Function AsyncMethodCaller(
			  oFunc As Func(Of tObjectType),
			  <Out()> ByRef threadId As Integer) As tObjectType

			''' <summary>
			''' Sends error email detailing call back exception
			''' </summary>
			''' <param name="oCallBackMeta">The call back meta information.</param>
			''' <param name="ex">The exception.</param>
			Private Sub SendCallBackErrorEmail(oCallBackMeta As CallBackMeta, ex As Exception)

				If Not oCallBackMeta.SMTPHost = String.Empty AndAlso oCallBackMeta.SystemEmail <> String.Empty Then

					Try

						'create some diagnostics
						Dim oSB As New StringBuilder
						With oSB
							.AppendFormatLine("Machine: {0}", Environment.MachineName)
							.AppendFormatLine("Root path: {0}", System.Web.Hosting.HostingEnvironment.MapPath("~\"))
							If Not oCallBackMeta Is Nothing Then
								.AppendFormatLine("Cache Key: {0}", oCallBackMeta.CacheKey)
							End If
							.AppendLine()
							.AppendLine("Exception")
							.AppendLine("=========")
							.AppendLine(ex.ToString)
						End With

						'send the email
						Dim oEmail As New Email
						With oEmail
							.From = "asynchronous cache loader"
							.FromEmail = "support@ivector.co.uk"
							.EmailTo = oCallBackMeta.SystemEmail
							.Subject = "Cache callback failure"
							.Body = oSB.ToString
							.SMTPHost = oCallBackMeta.SMTPHost
							.SendEmail(False)
						End With

					Catch

					End Try

				End If

			End Sub

		End Class

		''' <summary>
		''' Class used to store information used in call back methods and sending error emails/>
		''' </summary>
		Public Class CallBackMeta
			''' <summary>
			''' How long to store the cache for in minutes
			''' </summary>
			Public CacheMinutes As Decimal = 60
			''' <summary>
			''' The cache key
			''' </summary>
			Public CacheKey As String = String.Empty
			''' <summary>
			''' The call back function
			''' </summary>
			Public CallBackFunction As CacheItemRemovedCallback
			''' <summary>
			''' The SMTP host to send the email from
			''' </summary>
			Public SMTPHost As String = String.Empty
			''' <summary>
			''' The email address to send emails to
			''' </summary>
			Public SystemEmail As String = String.Empty
			''' <summary>
			''' Specifies whether to store an onRemoveCallback in the cache as well.
			''' </summary>
			Public RecordCallback As Boolean = False
			''' <summary>
			''' Specifies whether to also store the cache locally
			''' </summary>
			Public Storelocally As Boolean = True
			''' <summary>
			''' Regex used to determine whether we want to ignore an xml result when we store locally.
			''' </summary>
			Public ExcludeRegex As String = String.Empty
		End Class

		''' <summary>
		''' Class with functions to retrieve xml documents from the cache.
		''' </summary>
		Public Class SQL

			''' <summary>
			''' Gets XML document from the cache.
			''' </summary>
			''' <param name="SQL">The SQL used to build the cache/return document.</param>
			Public Shared Function GetXMLDocument(SQL As String) As XmlDocument
				Return GetXMLDocument(Intuitive.SQL.ConnectString, SQL, 60)
			End Function

			''' <summary>
			''' Gets XML document from the cache.
			''' </summary>
			''' <param name="SQL">The SQL used to build the cache/return document.</param>
			''' <param name="params">The parameters used with the sql.</param>
			Public Shared Function GetXMLDocument(SQL As String, ParamArray params() As Object) As XmlDocument
				Return GetXMLDocument(Intuitive.SQL.ConnectString, SQL, 60, params)
			End Function

			''' <summary>
			''' Gets XML document from the cache.
			''' </summary>
			''' <param name="SQL">The SQL used to build the cache/return document.</param>
			''' <param name="CacheMinutes">How long to store the cache for in minutes.</param>
			''' <param name="params">The parameters used with the sql.</param>
			Public Shared Function GetXMLDocument(SQL As String, CacheMinutes As Decimal, ParamArray params() As Object) As XmlDocument
				Return GetXMLDocument(Intuitive.SQL.ConnectString, SQL, CacheMinutes, params)
			End Function

			''' <summary>
			''' Gets XML document from the cache.
			''' </summary>
			''' <param name="ConnectString">The connect string.</param>
			''' <param name="SQL">The SQL used to build the cache/return document.</param>
			''' <param name="CacheMinutes">How long to store the cache for in minutes.</param>
			''' <param name="params">The parameters used with the sql.</param>
			Public Shared Function GetXMLDocument(ConnectString As String, SQL As String, CacheMinutes As Decimal, ParamArray params() As Object) As XmlDocument
				Dim sSQL As String = Intuitive.SQL.FormatStatement(SQL, params)
				Return GetXMLDocument(ConnectString, sSQL, CacheMinutes)
			End Function

			''' <summary>
			''' Gets XML document from the cache.
			''' </summary>
			''' <param name="ConnectString">The connect string.</param>
			''' <param name="SQL">The SQL used to build the cache/return document.</param>
			''' <param name="CacheMinutes">How long to store the cache for in minutes.</param>
			Public Shared Function GetXMLDocument(ConnectString As String, SQL As String, CacheMinutes As Decimal) As XmlDocument

				Dim sKey As String = "xml" & SQL.GetHashCode.ToString

				Dim oItem As XmlDocument =
				 AsyncCache.Controller(Of XmlDocument).GetCache(
				 sKey, CacheMinutes,
				 Function()
					 Return Intuitive.SQL.GetXMLDocWithConnectString(ConnectString, SQL)
				 End Function)

				Return oItem

			End Function

		End Class

	End Class

End Namespace