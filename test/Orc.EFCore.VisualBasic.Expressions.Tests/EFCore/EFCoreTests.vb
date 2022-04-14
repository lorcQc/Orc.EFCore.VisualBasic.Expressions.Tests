Imports Microsoft.EntityFrameworkCore
Imports Microsoft.EntityFrameworkCore.Query.Internal
Imports Xunit
Imports Xunit.Abstractions

Public NotInheritable Class EFCoreTests
    Implements IClassFixture(Of BloggingDatabaseFixture)

    Private _Fixture As BloggingDatabaseFixture
    Private _Output As ITestOutputHelper

    Function GetContext() As EFCoreBloggingContext
        Return _Fixture.CreateEFCoreContext
    End Function

    Public Sub New(fixture As BloggingDatabaseFixture,
                   output As ITestOutputHelper)

        _Fixture = fixture
        _Output = output
    End Sub

    <Fact>
    Public Sub StringComparaison_QuerySyntaxWork()
        Using context = GetContext()
            Dim Query = From p In context.People
                        Where p.Name.Contains("Admin")

            Log(Query)
            Dim Result = Query.ToList()
            Assert.Single(Result)
        End Using
    End Sub

    <Fact>
    Public Sub StringComparaison_MethodSyntaxWork()
        Using context = GetContext()
            Dim Query = context.People.
                        Where(Function(p) p.Name.Contains("Admin"))

            Log(Query)
            Dim Result = Query.ToList()
            Assert.Single(Result)
        End Using
    End Sub

    <Fact>
    Public Sub NullableValueComparaison_QuerySyntaxWork()
        Using context = GetContext()
            Dim Query = From b In context.Blogs
                        Where b.Rating Is Nothing

            Log(Query)
            Dim Result = Query.ToList()
            Assert.Single(Result)
            Assert.DoesNotContain("COALESCE", Query.ToQueryString())
        End Using
    End Sub

    <Fact>
    Public Sub NullableValueComparaison_QuerySyntaxWork2()
        Using context = GetContext()
            Dim Query = From b In context.Blogs
                        Where b.Rating > 0

            Log(Query)
            Dim Result = Query.ToList()
            Assert.Single(Result)
            Assert.DoesNotContain("COALESCE", Query.ToQueryString())
        End Using
    End Sub

    <Fact>
    Public Sub NullableValueComparaison_QuerySyntaxWork3()
        Using context = GetContext()
            Dim Query = From b In context.Blogs
                        Where b.Rating = 5

            Log(Query)
            Dim Result = Query.ToList()
            Assert.Single(Result)
            Assert.DoesNotContain("COALESCE", Query.ToQueryString())
        End Using
    End Sub

    <Fact>
    Public Sub NullableValueComparaisonWork()
        Using context = GetContext()
            Dim Query = context.Blogs.
                        Where(Function(b) b.Rating Is Nothing)

            Log(Query)
            Dim Result = Query.ToList()
            Assert.Single(Result)
            Assert.DoesNotContain("COALESCE", Query.ToQueryString())
        End Using
    End Sub

    <Fact>
    Public Sub NullableValueComparaisonWork2()
        Using context = GetContext()
            Dim Query = context.Blogs.
                        Where(Function(b) b.Rating > 0)

            Log(Query)
            Dim Result = Query.ToList()
            Assert.Single(Result)
            Assert.DoesNotContain("COALESCE", Query.ToQueryString())
        End Using
    End Sub

    <Fact>
    Public Sub NullableValueComparaisonWork3()
        Using context = GetContext()
            Dim Query = context.Blogs.
                        Where(Function(b) b.Rating = 5)

            Log(Query)
            Dim Result = Query.ToList()
            Assert.Single(Result)
            Assert.DoesNotContain("COALESCE", Query.ToQueryString())
        End Using
    End Sub

    <Fact>
    Public Sub GroupByQuerySyntaxWork()
        Using context = GetContext()
            Dim Query = From b In context.Posts
                        Group By b.AuthorId Into Group
                        Select AuthorId, Group.Count

            Log(Query)
            Dim Result = Query.ToList()
            Assert.NotNull(Result.FirstOrDefault(Function(x) x.AuthorId = 3))
            Assert.Equal(2, Result.FirstOrDefault(Function(x) x.AuthorId = 3).Count)
        End Using
    End Sub

    <Fact>
    Public Sub GroupByMethodSyntaxWork()
        Using context = GetContext()
            Dim Query = context.Posts.
                            GroupBy(Function(b) b.AuthorId).
                            Select(Function(Group) New With {Group.Key, Group.Count})

            Log(Query)
            Dim Result = Query.ToList()
            Assert.NotNull(Result.FirstOrDefault(Function(x) x.Key = 3))
            Assert.Equal(2, Result.FirstOrDefault(Function(x) x.Key = 3).Count)
        End Using
    End Sub

    <Fact>
    Public Sub GroupByMethodSyntaxWork2()
        Using context = GetContext()
            Dim Query = context.Posts.
                            GroupBy(
                                keySelector:=Function(b) b.AuthorId,
                                resultSelector:=Function(AuthorId, Posts) New With {AuthorId, .Group = Posts}).
                            Select(Function(Group) New With {Group.AuthorId, Group.Group.Count})

            Log(Query)
            Dim Result = Query.ToList()
            Assert.NotNull(Result.FirstOrDefault(Function(x) x.AuthorId = 3))
            Assert.Equal(2, Result.FirstOrDefault(Function(x) x.AuthorId = 3).Count)
        End Using
    End Sub

    <Fact>
    Public Sub GroupByMethodSyntaxWork3()
        Using context = GetContext()
            Dim Query = context.Posts.
                            GroupBy(
                                keySelector:=Function(b) b.AuthorId,
                                elementSelector:=Function(x) x.AuthorId,
                                resultSelector:=Function(AuthorId, Posts) New With {AuthorId, .Group = Posts}).
                                Select(Function(Group) New With {Group.AuthorId, Group.Group.Count})

            Log(Query)
            Dim Result = Query.ToList()
            Assert.NotNull(Result.FirstOrDefault(Function(x) x.AuthorId = 3))
            Assert.Equal(2, Result.FirstOrDefault(Function(x) x.AuthorId = 3).Count)
        End Using
    End Sub

    Private Sub Log(query As IQueryable)
        Dim DebugView = DirectCast(GetPropValue(query, "DebugView"), QueryDebugView)
        _Output.WriteLine("Expression : " & Environment.NewLine & "{0}", DebugView.Expression)
        _Output.WriteLine("Query : " & Environment.NewLine & "{0}", DebugView.Query)
    End Sub

    Private Shared Function GetPropValue(src As Object, propName As String) As Object
        Return src.GetType().GetProperty(propName).GetValue(src)
    End Function

End Class
