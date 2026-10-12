using AutoMapper;
using FootballSocialMedia.Model.Comments;
using FootballSocialMedia.Model.Likes.Dtos.Requests;
using FootballSocialMedia.Model.Notifications.Entities;
using FootballSocialMedia.Model.Notifications.Enums;
using FootballSocialMedia.Model.Posts.Entities;
using FootballSocialMedia.Model.Users.Entities;
using FootballSocialMedia.Repository.Comments.Concretes;
using FootballSocialMedia.Repository.Context;
using FootballSocialMedia.Repository.Likes.Concretes;
using FootballSocialMedia.Repository.Posts.Concretes;
using FootballSocialMedia.Repository.UnitOfWorks.Concretes;
using FootballSocialMedia.Service.Likes.Concretes;
using FootballSocialMedia.Service.RabbitMQ;
using Moq;
using System.Net;

namespace Service.Test;

[TestFixture]
public class LikeServiceTest
{
    private AppDbContext _context = null!;
    private TestDb.FakeIdentity _identity = null!;
    private Mock<INotificationPublisher> _publisherMock = null!;
    private LikeService _likeService = null!;
    private User _ali = null!;
    private User _veli = null!;
    private Post _post = null!;
    private Comment _comment1 = null!;
    private Comment _comment2 = null!;

    [SetUp]
    public void SetUp()
    {
        _context = TestDb.CreateContext();
        _identity = new TestDb.FakeIdentity();
        _publisherMock = new Mock<INotificationPublisher>();

        _likeService = new LikeService(
            new LikeRepository(_context),
            new PostRepository(_context),
            new CommentRepository(_context),
            new Mock<IMapper>().Object,
            new UnitOfWork(_context),
            TestDb.IdGenerator,
            _identity.Object,
            _publisherMock.Object);

        _ali = TestDb.AddUser(_context, "ali");
        _veli = TestDb.AddUser(_context, "veli");

        _post = new Post { Id = Guid.NewGuid(), Content = "ali'nin postu", UserId = _ali.Id };
        _comment1 = new Comment { Id = Guid.NewGuid(), PostId = _post.Id, UserId = _ali.Id, Content = "yorum 1" };
        _comment2 = new Comment { Id = Guid.NewGuid(), PostId = _post.Id, UserId = _ali.Id, Content = "yorum 2" };
        _context.Posts.Add(_post);
        _context.Comments.AddRange(_comment1, _comment2);
        _context.SaveChanges();
    }

    [TearDown]
    public void TearDown() => _context.Dispose();

    [Test]
    public async Task LikeAsync_SendsNotificationToPostOwner_NotToLiker()
    {
        _identity.LoginAs(_veli);

        var result = await _likeService.LikeAsync(new CreateLikeRequest(_post.Id, null));

        Assert.That(result.Status, Is.EqualTo(HttpStatusCode.Created));
        Assert.That(result.Data!.LikeCount, Is.EqualTo(1));
        _publisherMock.Verify(p => p.SendNotification(It.Is<Notification>(n =>
            n.UserId == _ali.Id && n.SenderId == _veli.Id && n.Type == NotificationType.Like && n.SenderUserName == "veli")), Times.Once);
    }

    [Test]
    public async Task LikeAsync_OwnPost_DoesNotSendNotification()
    {
        _identity.LoginAs(_ali);

        await _likeService.LikeAsync(new CreateLikeRequest(_post.Id, null));

        _publisherMock.Verify(p => p.SendNotification(It.IsAny<Notification>()), Times.Never);
    }

    [Test]
    public async Task LikeAsync_SecondCommentAfterFirst_IsNotTreatedAsDuplicate()
    {
        // Eski bug: (PostId == null || ...) koşulu yüzünden ikinci yoruma beğeni "Already liked" dönüyordu
        _identity.LoginAs(_veli);

        var first = await _likeService.LikeAsync(new CreateLikeRequest(null, _comment1.Id));
        var second = await _likeService.LikeAsync(new CreateLikeRequest(null, _comment2.Id));

        Assert.That(first.IsSuccess, Is.True);
        Assert.That(second.IsSuccess, Is.True, string.Join(",", second.ErrorMessage ?? new()));
    }

    [Test]
    public async Task LikeAsync_SameTargetTwice_ReturnsAlreadyLiked()
    {
        _identity.LoginAs(_veli);

        await _likeService.LikeAsync(new CreateLikeRequest(_post.Id, null));
        var second = await _likeService.LikeAsync(new CreateLikeRequest(_post.Id, null));

        Assert.That(second.IsSuccess, Is.False);
        Assert.That(second.ErrorMessage!.First(), Is.EqualTo("Already liked."));
    }

    [Test]
    public async Task GetLikeCountAsync_CountsOnlyTheRequestedTarget()
    {
        // Eski bug: post beğeni sayısına tüm "CommentId == null" kayıtları da sayılıyordu
        var otherPost = new Post { Id = Guid.NewGuid(), Content = "başka post", UserId = _veli.Id };
        _context.Posts.Add(otherPost);
        _context.SaveChanges();

        _identity.LoginAs(_veli);
        await _likeService.LikeAsync(new CreateLikeRequest(_post.Id, null));
        await _likeService.LikeAsync(new CreateLikeRequest(null, _comment1.Id));
        _identity.LoginAs(_ali);
        await _likeService.LikeAsync(new CreateLikeRequest(otherPost.Id, null));

        Assert.That((await _likeService.GetLikeCountAsync(_post.Id, null)).Data, Is.EqualTo(1));
        Assert.That((await _likeService.GetLikeCountAsync(otherPost.Id, null)).Data, Is.EqualTo(1));
        Assert.That((await _likeService.GetLikeCountAsync(null, _comment1.Id)).Data, Is.EqualTo(1));
        Assert.That((await _likeService.GetLikeCountAsync(null, _comment2.Id)).Data, Is.EqualTo(0));
    }

    [Test]
    public async Task UnlikeAsync_RemovesOnlyMyLike()
    {
        _identity.LoginAs(_veli);
        await _likeService.LikeAsync(new CreateLikeRequest(_post.Id, null));
        _identity.LoginAs(_ali);
        await _likeService.LikeAsync(new CreateLikeRequest(_post.Id, null));

        var result = await _likeService.UnlikeAsync(_post.Id, null);

        Assert.That(result.Status, Is.EqualTo(HttpStatusCode.NoContent));
        Assert.That(_context.Likes.Single().UserId, Is.EqualTo(_veli.Id));
    }
}
