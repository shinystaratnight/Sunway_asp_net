Imports System.IO
Imports System.Text
Imports System.Security.Cryptography

Namespace Security

	''' <summary>
	''' Class for encrypting and decrypting strings
	''' </summary>
	Public Class SecretKeeper

#Region "Fields"

		''' <summary>
		''' The encryption key string
		''' </summary>
		Private sKey As String

		''' <summary>
		''' Byte array for encryption string
		''' </summary>
		Private aKey() As Byte = {}

		''' <summary>
		''' Initialization vector
		''' </summary>
        Private aIV() As Byte = {&H14, &H1D, &H25, &H27, &H6D, &H3E, &H13, &H30, &HA, &H2C, &HF, &H6C, &H40, &HC8, &H92, &HA3}

		''' <summary>
		''' The encoding
		''' </summary>
		Private oEncoding As System.Text.Encoding

		''' <summary>
        ''' AES encryption service provider
		''' </summary>
        Private oEncryptor As SymmetricAlgorithm

#End Region

#Region "Properties"

		''' <summary>
		''' Sets the encryption key.
		''' </summary>
		''' <remarks>
		''' The key must be at least 8 characters long or an exception will be thrown.
		''' </remarks>
		Public WriteOnly Property Key() As String
			Set(ByVal Value As String)

				sKey = Value

				If sKey.Length >= 8 Then
                    Dim iKeyLength As Integer = sKey.Length - (sKey.Length Mod 8)
					aKey = Encoding.UTF8.GetBytes(sKey.Substring(0, iKeyLength))
				Else
					Throw New ArgumentException("The key supplied it not long enough. Minimum length is 8.")
				End If

			End Set
		End Property

#End Region

#Region "Constructor"

		''' <summary>
		''' Initializes a new instance of the <see cref="SecretKeeper"/> class with the specified encryption key.
		''' </summary>
		''' <param name="Key">The encryption key.</param>
		''' <param name="CipherMode">The mode.</param>
        Public Sub New(ByVal Key As String, ByVal EncryptionType As EncryptionType, ByVal CipherMode As CipherMode)
            Me.New(Key, EncryptionType)
            oEncryptor.Mode = CipherMode
		End Sub

		''' <summary>
		''' Initializes a new instance of the <see cref="SecretKeeper"/> class with the specified encryption key.
		''' </summary>
		''' <param name="Key">The encryption key.</param>
		Public Sub New(ByVal Key As String)
            Me.New(Key, EncryptionType.Des)
        End Sub

        ''' <summary>
        ''' Initializes a new instance of the <see cref="SecretKeeper"/> class with the specified encryption key.
        ''' </summary>
        ''' <param name="Key">The encryption key.</param>
        Public Sub New(ByVal Key As String, ByVal EncryptionType As EncryptionType)

            'Create the encryption provider
            Select Case EncryptionType
                Case EncryptionType.Aes
                    oEncryptor = New AesCryptoServiceProvider
                Case EncryptionType.Des
                    oEncryptor = New DESCryptoServiceProvider
            End Select

			'Setup the encoding
			oEncoding = Encoding.UTF8()

			'Set the key
			Me.Key = Key

		End Sub

#End Region

#Region "Encrypt"

		''' <summary>
		''' Encrypts the specified string.
		''' </summary>
		''' <param name="StringToEncrypt">The string to encrypt.</param>
		''' <exception cref="System.Exception">No encryption key has been supplied, Error encrypting</exception>
		Public Function Encrypt(ByVal StringToEncrypt As String) As String
			Return Convert.ToBase64String(EncryptToBytes(StringToEncrypt))
		End Function

		''' <summary>
		''' Encrypts the specified string.
		''' </summary>
		''' <param name="StringToEncrypt">The string to encrypt.</param>
		''' <exception cref="System.Exception">No encryption key has been supplied, Error encrypting</exception>
		Public Function EncryptToBytes(ByVal StringToEncrypt As String) As Byte()

			If sKey = String.Empty Then
				Throw New Exception("No encryption key has been supplied")
			End If

			Dim Encrypted As Byte()

			Try

				'Convert input string to a byte array
				Dim aArrayToEncrypt() As Byte = Encoding.UTF8.GetBytes(StringToEncrypt)

				'now encrypt the bytearray
				Using oMemoryStream As New MemoryStream
                    Using oCryptoStream As New CryptoStream(oMemoryStream, oEncryptor.CreateEncryptor(aKey, aIV), CryptoStreamMode.Write)
				oCryptoStream.Write(aArrayToEncrypt, 0, aArrayToEncrypt.Length)
				oCryptoStream.FlushFinalBlock()
						Encrypted = oMemoryStream.ToArray()
					End Using
				End Using

			Catch ex As Exception
				Throw New Exception($"Error encrypting {StringToEncrypt}: {ex.Message}")
			End Try

			Return Encrypted

		End Function

		''' <summary>
		''' Encrypts the specified string.
		''' </summary>
		''' <param name="StringToEncrypt">The string to decrypt.</param>
		Public Function EncryptToHex(ByVal StringToEncrypt As String) As String
			Return Intuitive.Functions.BytesToHex(Me.EncryptToBytes(StringToEncrypt))
		End Function

#End Region

#Region "Decrypt"

		''' <summary>
		''' Decrypts the specified string.
		''' </summary>
		''' <param name="StringToDecrypt">The string to decrypt.</param>
		''' <exception cref="System.Exception">No encryption key has been supplied, Error decrypting string</exception>
		Public Function Decrypt(ByVal StringToDecrypt As String) As String

			Dim sDecrypted As String

			Try

				Dim aArrayToDecrypt(StringToDecrypt.Length) As Byte
				aArrayToDecrypt = Convert.FromBase64String(StringToDecrypt.Replace(" ", "+"))

				sDecrypted = Decrypt(aArrayToDecrypt)

			Catch ex As Exception
				Throw New Exception($"Error decrypting {StringToDecrypt}: {ex.Message}")
			End Try

			Return sDecrypted

		End Function

		''' <summary>
		''' Decrypts the specified string.
		''' </summary>
		''' <param name="BytesToDecrypt">The string to decrypt.</param>
		''' <exception cref="System.Exception">No encryption key has been supplied, Error decrypting string</exception>
		Public Function Decrypt(ByVal BytesToDecrypt As Byte()) As String

			If sKey = String.Empty Then
				Throw New Exception("No encryption key has been supplied")
			End If

			Dim sDecrypted As String

				'Create stream using string and key and decrypt it
			Using oMemoryStream As New MemoryStream
                Using oCryptoStream As New CryptoStream(oMemoryStream, oEncryptor.CreateDecryptor(aKey, aIV), CryptoStreamMode.Write)
					oCryptoStream.Write(BytesToDecrypt, 0, BytesToDecrypt.Length)
				oCryptoStream.FlushFinalBlock()
				sDecrypted = oEncoding.GetString(oMemoryStream.ToArray())
				End Using
			End Using

			Return sDecrypted

		End Function

		''' <summary>
		''' Decrypts the specified string.
		''' </summary>
		''' <param name="StringToDecrypt">The string to decrypt.</param>
		Public Function SafeDecrypt(ByVal StringToDecrypt As String) As String

			Try
				Return Me.Decrypt(StringToDecrypt)
			Catch ex As Exception
				Return String.Empty
			End Try

		End Function

		''' <summary>
		''' Decrypts the specified string.
		''' </summary>
		''' <param name="HexStringToDecrypt">The string to decrypt.</param>
		Public Function DecryptHex(ByVal HexStringToDecrypt As String) As String
			Return Me.Decrypt(Intuitive.Functions.HexToBytes(HexStringToDecrypt))
		End Function

		''' <summary>
		''' Decrypts the specified string.
		''' </summary>
		''' <param name="HexStringToDecrypt">The string to decrypt.</param>
		Public Function SafeDecryptHex(ByVal HexStringToDecrypt As String) As String
			Try
				Return Me.Decrypt(Intuitive.Functions.HexToBytes(HexStringToDecrypt))
			Catch ex As Exception
				Return String.Empty
			End Try
		End Function

#End Region

        Public Enum EncryptionType
            Des
            Aes
        End Enum

	End Class

End Namespace