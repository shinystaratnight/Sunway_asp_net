
Namespace Basket.Collections

    Public Class BasketList(Of t)
        Implements IList(Of t)

        Private AllowEvent As Boolean = False 'false by default so we know when it is being serialized/deserialized

        'do not use this constructor for explicit instantiation, it is for serialization only!!
        Public Sub New()

        End Sub

        Public Sub New(AllowEvent As Boolean)
            Me.AllowEvent = AllowEvent
        End Sub

        Private _list As New System.Collections.Generic.List(Of t)

        Public Sub Add(item As t) Implements ICollection(Of t).Add
            _list.Add(item)
            ListChanged()
        End Sub

        Public Sub Clear() Implements ICollection(Of t).Clear
            _list.Clear()
            ListChanged()
        End Sub

        Public Function Contains(item As t) As Boolean Implements ICollection(Of t).Contains
            Return _list.Contains(item)
        End Function

        Public Sub CopyTo(array() As t, arrayIndex As Integer) Implements ICollection(Of t).CopyTo
            _list.CopyTo(array, arrayIndex)
        End Sub

        Public ReadOnly Property Count As Integer Implements ICollection(Of t).Count
            Get
                Return _list.Count
            End Get
        End Property

        Public ReadOnly Property IsReadOnly As Boolean Implements ICollection(Of t).IsReadOnly
            Get
                Return False
            End Get
        End Property

        Public Function Remove(item As t) As Boolean Implements ICollection(Of t).Remove
            Return _list.Remove(item)
            ListChanged()
        End Function

        Public Function GetEnumerator() As IEnumerator(Of t) Implements IEnumerable(Of t).GetEnumerator
            Return _list.GetEnumerator()
        End Function

        Public Function IndexOf(item As t) As Integer Implements IList(Of t).IndexOf
            Return _list.IndexOf(item)
        End Function

        Public Sub Insert(index As Integer, item As t) Implements IList(Of t).Insert
            _list.Insert(index, item)
            ListChanged()
        End Sub

        Default Public Property Item(index As Integer) As t Implements IList(Of t).Item
            Get
                Return _list(index)
            End Get
            Set(value As t)
                _list(index) = value
            End Set
        End Property

        Public Sub RemoveAt(index As Integer) Implements IList(Of t).RemoveAt
            _list.RemoveAt(index)
            ListChanged()
        End Sub

        Public Function GetEnumerator1() As IEnumerator Implements IEnumerable.GetEnumerator
            Return _list.GetEnumerator()
        End Function

        Public Sub AddRange(List As BasketList(Of t))
            For Each item As t In List
                _list.Add(item)
            Next
            ListChanged()
        End Sub

        Public Event ListChangedEvent As ListChangedHandler
        Public Delegate Sub ListChangedHandler(sender As Object)
        Protected Sub ListChanged()
            If AllowEvent Then RaiseEvent ListChangedEvent(Me)
        End Sub

    End Class


End Namespace
