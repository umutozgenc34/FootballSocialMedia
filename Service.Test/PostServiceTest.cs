using AutoMapper;
using Core.Infrastructure.Services.CloudinaryService.Abstracts;
using FootballSocialMedia.Model.Follows.Entities;
using FootballSocialMedia.Model.Likes.Entities;
using FootballSocialMedia.Model.Comments;
using FootballSocialMedia.Model.Posts.Dtos.Requests;
using FootballSocialMedia.Model.Posts.Entities;
using FootballSocialMedia.Model.Search.Dtos;
using FootballSocialMedia.Model.Users.Entities;
using FootballSocialMedia.Repository.Context;
using FootballSocialMedia.Repository.Follows.Concretes;
using FootballSocialMedia.Repository.Posts.Concretes;
using FootballSocialMedia.Repository.UnitOfWorks.Concretes;
using FootballSocialMedia.Service.Elasticsearch.Abstracts;
using FootballSocialMedia.Service.Posts.Concretes;
using Moq;
using System.Net;

namespace Service.Test;

[TestFixture]
public class PostServiceTest
{
    private AppDbContext _context = null!;
    private TestDb.FakeIdentity _identity = null!;
    private Mock<IElasticsearchService> _elasticsearchMock = null!;
    private PostService _postService = null!;
    private User _ali = null!;
    private User _veli = null!;

    [SetUp]
    public void SetUp()
    {
        _context = TestDb.CreateContext();
        _identity = new TestDb.FakeIdentity();
        _elasticsearchMock = new Mock<IElasticsearchService>();

        _postService = new PostService(
            new PostRepository(_context),
            new Mock<IMapper>().Object,
            new UnitOfWork(_context),
            TestDb.IdGenerator,
            new Mock<ICloudinaryService>().Object,
            _elasticsearchMock.Object,
            _identity.Object,
            new FollowRepository(_context));

        _ali = TestDb.AddUser(_context, "ali");
        _veli = TestDb.AddUser(_context, "veli");
    }

    [TearDown]
    public void TearDown() => _context.Dispose();

    private Post AddPost(User owner, string content)
    {
        var post = new Post { Id = Guid.NewGuid(), Content = content, UserId = owner.Id };
        _context.Posts.Add(post);
        _context.SaveChanges();
        return post;
    }

    [Test]
    public async Task CreateAsync_UsesUserFromToken_AndIndexesToElasticsearch()
    {
        _identity.LoginAs(_ali);

        var result = await _postService.CreateAsync(new CreatePostRequest("Fenerbahçe bu sene şampiyon!", null, null));

        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.Status, Is.EqualTo(HttpStatusCode.Created));
        Assert.That(result.Data!.UserId, Is.EqualTo(_ali.Id));
        Assert.That(result.Data.Username, Is.EqualTo("ali"));
        Assert.That(_context.Posts.Single().UserId, Is.EqualTo(_ali.Id));
        _elasticsearchMock.Verify(e => e.IndexPostAsync(It.Is<PostSearchDocument>(d => d.UserName == "ali")), Times.Once);
    }

    [Test]
    public async Task GetFeedAsync_ReturnsCountsAndLikedByMe()
    {
        var post = AddPost(_ali, "Derbi yorumları");
        _context.Likes.Add(new Like { Id = Guid.NewGuid(), PostId = post.Id, UserId = _veli.Id });
        _context.Comments.Add(new Comment { Id = Guid.NewGuid(), PostId = post.Id, UserId = _veli.Id, Content = "Katılıyorum" });
        _context.SaveChanges();

        _identity.LoginAs(_veli);
        var result = await _postService.GetFeedAsync("all", 1, 10);

        var dto = result.Data!.Single();
        Assert.That(dto.LikeCount, Is.EqualTo(1));
        Assert.That(dto.CommentCount, Is.EqualTo(1));
        Assert.That(dto.IsLikedByMe, Is.True);

        _identity.LoginAs(_ali);
        var asAli = await _postService.GetFeedAsync("all", 1, 10);
        Assert.That(asAli.Data!.Single().IsLikedByMe, Is.False);
    }

    [Test]
    public async Task GetFeedAsync_FollowingMode_OnlyReturnsFollowedUsersAndMe()
    {
        var charlie = TestDb.AddUser(_context, "charlie");
        AddPost(_ali, "ali post");
        AddPost(_veli, "veli post");
        AddPost(charlie, "charlie post");
        _context.Follows.Add(new Follow { FollowerId = _ali.Id, FollowingId = _veli.Id });
        _context.SaveChanges();

        _identity.LoginAs(_ali);
        var result = await _postService.GetFeedAsync("following", 1, 10);

        Assert.That(result.Data!.Select(p => p.Content), Is.EquivalentTo(new[] { "ali post", "veli post" }));
    }

    [Test]
    public async Task DeleteAsync_PostNotFound_ReturnsNotFound()
    {
        _identity.LoginAs(_ali);
        var result = await _postService.DeleteAsync(Guid.NewGuid());

        Assert.That(result.IsSuccess, Is.False);
        Assert.That(result.Status, Is.EqualTo(HttpStatusCode.NotFound));
    }

    [Test]
    public async Task DeleteAsync_NotOwner_ReturnsForbidden()
    {
        var post = AddPost(_ali, "ali'nin postu");
        _identity.LoginAs(_veli);

        var result = await _postService.DeleteAsync(post.Id);

        Assert.That(result.Status, Is.EqualTo(HttpStatusCode.Forbidden));
        Assert.That(_context.Posts.Count(), Is.EqualTo(1));
    }

    [Test]
    public async Task DeleteAsync_PostWithLikesAndComments_DeletesEverything()
    {
        var post = AddPost(_ali, "silinecek post");
        var comment = new Comment { Id = Guid.NewGuid(), PostId = post.Id, UserId = _veli.Id, Content = "yorum" };
        _context.Comments.Add(comment);
        _context.Likes.Add(new Like { Id = Guid.NewGuid(), PostId = post.Id, UserId = _veli.Id });
        _context.Likes.Add(new Like { Id = Guid.NewGuid(), CommentId = comment.Id, UserId = _ali.Id });
        _context.SaveChanges();
        _context.ChangeTracker.Clear();

        _identity.LoginAs(_ali);
        var result = await _postService.DeleteAsync(post.Id);

        Assert.That(result.Status, Is.EqualTo(HttpStatusCode.NoContent));
        Assert.That(_context.Posts.Count(), Is.Zero);
        Assert.That(_context.Comments.Count(), Is.Zero);
        Assert.That(_context.Likes.Count(), Is.Zero);
        _elasticsearchMock.Verify(e => e.DeletePostAsync(post.Id), Times.Once);
    }

    [Test]
    public async Task GetByIdAsync_PostNotFound_ReturnsNotFound()
    {
        _identity.LoginAs(_ali);
        var result = await _postService.GetByIdAsync(Guid.NewGuid());

        Assert.That(result.IsSuccess, Is.False);
        Assert.That(result.Status, Is.EqualTo(HttpStatusCode.NotFound));
        Assert.That(result.ErrorMessage!.First(), Is.EqualTo("Post not found"));
    }

    [Test]
    public async Task GetByIdAsync_PostExists_ReturnsDto()
    {
        var post = AddPost(_ali, "Test Content");
        _identity.LoginAs(_veli);

        var result = await _postService.GetByIdAsync(post.Id);

        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.Data!.Content, Is.EqualTo("Test Content"));
        Assert.That(result.Data.Username, Is.EqualTo("ali"));
        Assert.That(result.Data.ImageUrls, Is.Not.Null);
    }
}
