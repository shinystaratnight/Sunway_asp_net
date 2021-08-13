Imports Newtonsoft

Namespace WebAssets

    Public NotInheritable Class Assets

#Region "Constant Values"

        Private Const ASSET_MANIFEST_FILE As String = "rev-manifest.json"
        Private Const ASSET_BASE_PATH As String = "/webassets/"
        Private Const ASSET_FILE_MAPPINGS_CACHE_KEY As String = "__webassets_cached_mappings"
        Private ReadOnly Property ASSET_MANIFEST_FILE_PATH As String
            Get
                Return String.Format("{0}{1}", ASSET_BASE_PATH, ASSET_MANIFEST_FILE)
            End Get
        End Property


#End Region

#Region "Properties"

        Private Shared syncRoot As New Object
        Private Shared _Instance As WebAssets.Assets
        Public Shared ReadOnly Property Instance As WebAssets.Assets
            Get
                'thread safe singleton - using double locked checking to avoid expensive lock operation that only needs to happen once
                If _Instance Is Nothing Then
                    SyncLock syncRoot
                        If _Instance Is Nothing Then
                            _Instance = New WebAssets.Assets
                        End If
                    End SyncLock
                End If

                Return _Instance

            End Get
        End Property

#End Region

#Region "Constructor(s)"

        Private Sub New()

            Me.Setup()

        End Sub

#End Region

#Region "Public Api"

        Public Function GetFilePath(OriginalFilePath As String, UseVersionedAssets As Boolean) As String

            'if the config value was not set on instantiation
            If Not UseVersionedAssets Then Return OriginalFilePath

            'original file paths have '/' prepended and need it back on after
            ' our dictionary does not have this though so do a little check
            Dim editedOriginalFilePath As String = OriginalFilePath.Substring(1, OriginalFilePath.Length - 1)
            Dim versionedPath As String = ""
            Try

                versionedPath = Me.GetCacheManifestData(editedOriginalFilePath.ToLower())

            Catch ex As Exception
                'if key does not exist then log this error and return the original file at least
#If DEBUG Then
                Throw New Exception(String.Format("Asset manifest file is missing this key: {0} - please make sure the gulp build and code are working as expected. " & _
                                                  "This will fail at runtime", editedOriginalFilePath.ToLower()))
#End If
                LogError(ex)
                Return OriginalFilePath
            End Try

            Return ASSET_BASE_PATH & versionedPath

        End Function

#End Region

#Region "Internal functions"


        Private Sub Setup()

            Dim assetDictionary As Dictionary(Of String, String) = GetManifestData(ASSET_MANIFEST_FILE_PATH)
            Me.CacheManifestData(assetDictionary)

        End Sub


        Private Function GetManifestData(Path As String) As Dictionary(Of String, String)

            Dim assetDictionary As New Dictionary(Of String, String)

            Dim filePath As String = HttpContext.Current.Server.MapPath(Path)
            If System.IO.File.Exists(filePath) Then
                Dim assetFileJson As String = Intuitive.FileFunctions.FileToText(filePath)
                assetDictionary = Newtonsoft.Json.JsonConvert.DeserializeObject(Of Dictionary(Of String, String))(assetFileJson.ToLower())
            End If
        
            Return assetDictionary

        End Function

        Private Sub CacheManifestData(Data As Dictionary(Of String, String))
            Intuitive.Functions.AddToCache(Assets.ASSET_FILE_MAPPINGS_CACHE_KEY, Data)
        End Sub

        Private Function GetCacheManifestData() As Dictionary(Of String, String)

			Dim oCache As Dictionary(Of String, String) = Functions.GetCache(Of Dictionary(Of String, String))(Assets.ASSET_FILE_MAPPINGS_CACHE_KEY)

            'if the cache has dropped out then set it up again
            If oCache Is Nothing Then
				Me.Setup()
				Return Me.GetCacheManifestData()
			End If

			Return oCache

		End Function

        Private Sub LogError(ex As Exception)
            Logging.LogError("WebAssets", "Assets", ex)
        End Sub

#End Region


    End Class


End Namespace
