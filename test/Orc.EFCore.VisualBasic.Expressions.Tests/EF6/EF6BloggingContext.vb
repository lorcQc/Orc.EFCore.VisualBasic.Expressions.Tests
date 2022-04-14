Imports System.Data.Entity

Partial Public Class EF6BloggingContext
    Inherits DbContext

    Public Sub New(cnxString As String)
        MyBase.New(cnxString)
    End Sub

    Public Overridable Property Blogs As DbSet(Of Blog)
    Public Overridable Property People As DbSet(Of Person)
    Public Overridable Property Posts As DbSet(Of Post)
    Public Overridable Property PostTags As DbSet(Of PostTag)
    Public Overridable Property Tags As DbSet(Of Tag)

    Protected Overrides Sub OnModelCreating(modelBuilder As DbModelBuilder)

        modelBuilder.Entity(Of Person)().Ignore(Function(e) e.Photo)
        modelBuilder.Entity(Of Person)().Ignore(Function(e) e.PhotoId)

        modelBuilder.Entity(Of Person)().
            HasMany(Function(e) e.OwnedBlogs).
            WithRequired(Function(e) e.Owner).
            HasForeignKey(Function(e) e.OwnerId)

        modelBuilder.Entity(Of Person)().
            HasMany(Function(e) e.AuthoredPosts).
            WithRequired(Function(e) e.Author).
            HasForeignKey(Function(e) e.AuthorId)

        modelBuilder.Entity(Of Blog)().
            HasMany(Function(b) b.Posts).
            WithRequired(Function(p) p.Blog).
            WillCascadeOnDelete(False)

        modelBuilder.Entity(Of PostTag)().
            HasRequired(Function(pt) pt.Post).
            WithMany(Function(p) p.Tags).
            HasForeignKey(Function(pt) pt.PostId)

        modelBuilder.Entity(Of PostTag)().
            HasRequired(Function(pt) pt.Tag).
            WithMany(Function(t) t.Posts).
            HasForeignKey(Function(pt) pt.TagId)

    End Sub
End Class
