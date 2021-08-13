Imports System
Imports System.Data
Imports System.Data.SqlClient
Imports System.Data.SqlTypes
Imports Microsoft.SqlServer.Server
Imports System.Security.Cryptography

''' <summary>
''' Hash code class
''' </summary>
Public Class HashCode

	''' <summary>
	''' Gets the hash.
	''' </summary>
	''' <param name="s">The string to get the hash of.</param>
	''' <returns></returns>
	Public Shared Function GetHash(ByVal s As String) As Integer
		Dim oCRC32 As New CRC32
		Return Math.Abs(BitConverter.ToInt32(oCRC32.ComputeHash(System.Text.Encoding.ASCII.GetBytes(s)), 0))
	End Function

#Region "crc32 implementation (3rd party, bit ugly but works just fine)"

	''' <summary>
	''' Implementation of the CRC32 hashing algorithm
	''' </summary>
	''' <seealso cref="System.Security.Cryptography.HashAlgorithm" />
	Private Class CRC32
		Inherits HashAlgorithm

#Region "Member Variables"
		Private Const _DefaultPolynomial As Integer = &HEDB88320
		Private _Table() As Integer
		Private _CRC32 As Integer = &HFFFFFFFF
		Private _Polynomial As Integer
#End Region

#Region "Contructors"

		''' <summary>
		''' Initializes a new instance of the <see cref="CRC32"/> class.
		''' </summary>
		Public Sub New()
			Me.HashSizeValue = 32 ' CRC32 is a 32bit hash
			_Polynomial = _DefaultPolynomial
			Initialize()
		End Sub

		''' <summary>
		''' Initializes a new instance of the <see cref="CRC32"/> class.
		''' </summary>
		''' <param name="Polynomial">The polynomial.</param>
		Public Sub New(ByVal Polynomial As Integer)
			_Polynomial = Polynomial
		End Sub

#End Region

#Region "HashAlgorithm"

		''' <summary>
		''' When overridden in a derived class, routes data written to the object into the hash algorithm for computing the hash.
		''' </summary>
		''' <param name="array">The input to compute the hash code for.</param>
		''' <param name="ibStart">The offset into the byte array from which to begin using data.</param>
		''' <param name="cbSize">The number of bytes in the byte array to use as data.</param>
		Protected Overrides Sub HashCore(ByVal array() As Byte, ByVal ibStart As Integer, ByVal cbSize As Integer)
			Dim intLookup As Integer
			For i As Integer = 0 To cbSize - 1
				intLookup = (_CRC32 And &HFF) Xor array(i)
				_CRC32 = ((_CRC32 And &HFFFFFF00) \ &H100) And &HFFFFFF
				_CRC32 = _CRC32 Xor _Table(intLookup)
			Next i
		End Sub

		''' <summary>
		''' When overridden in a derived class, finalizes the hash computation after the last data is processed by the cryptographic stream object.
		''' </summary>
		''' <returns>
		''' The computed hash code.
		''' </returns>
		Protected Overrides Function HashFinal() As Byte()
			Return BitConverter.GetBytes(Not _CRC32)
		End Function

		''' <summary>
		''' Initializes an implementation of the <see cref="T:System.Security.Cryptography.HashAlgorithm" /> class.
		''' </summary>
		Public Overrides Sub Initialize()
			_CRC32 = &HFFFFFFFF
			_Table = BuildTable(_Polynomial)
		End Sub

#End Region

#Region "Helper Methods"

		''' <summary>
		''' Builds the table.
		''' </summary>
		''' <param name="Polynomial">The polynomial.</param>
		''' <returns></returns>
		Private Shared Function BuildTable(ByVal Polynomial As Integer) As Integer()
			Dim Table(255) As Integer
			Dim Value As Integer
			For I As Integer = 0 To 255
				Value = I
				For X As Integer = 0 To 7
					If (Value And 1) = 1 Then
						Value = Convert.ToInt32(((Value And &HFFFFFFFE) \ 2&) And &H7FFFFFFF)
						Value = Value Xor Polynomial
					Else
						Value = Convert.ToInt32(((Value And &HFFFFFFFE) \ 2&) And &H7FFFFFFF)
					End If
				Next
				Table(I) = Value
			Next
			Return Table
		End Function

#End Region

	End Class

#End Region

End Class