Imports Microsoft.EntityFrameworkCore

Public Class BloggingDatabaseFixture

    Private Const ConnectionString = "Server=(localdb)\MSSQLLocalDB;Database=Blogging;Trusted_Connection=True"

    Private Shared ReadOnly _lock As New Object
    Private Shared _databaseInitialized As Boolean

    Public Sub New()
        SyncLock _lock
            If Not _databaseInitialized Then
                Using context = CreateEFCoreContext()
                    context.Database.EnsureDeleted()
                    context.Database.EnsureCreated()
                End Using

                _databaseInitialized = True
            End If
        End SyncLock
    End Sub

    Public Function CreateEFCoreContext() As EFCoreBloggingContext
        Dim ob As New DbContextOptionsBuilder(Of EFCoreBloggingContext)
        ob.UseSqlServer(ConnectionString, Sub(o)
                                              o.UseRelationalNulls()
                                          End Sub)

        Return New EFCoreBloggingContext(ob.Options)
    End Function

    Public Function CreateEF6Context() As EF6BloggingContext
        Return New EF6BloggingContext(ConnectionString)
    End Function

End Class
