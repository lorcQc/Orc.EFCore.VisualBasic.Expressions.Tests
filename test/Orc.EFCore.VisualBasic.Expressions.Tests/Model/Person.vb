Public Class Person
    Public Property PersonId As Integer
    Public Property Name As String

    Public Property AuthoredPosts As List(Of Post)
    Public Property OwnedBlogs As List(Of Blog)

    Public Property PhotoId As Integer?
    Public Property Photo As PersonPhoto

    Public Property MuEnum As Test?
End Class

Public Enum Test As Short
    a
    b
    c
End Enum