using FootballSocialMedia.Model.Polls.Dtos.Requests;
using FootballSocialMedia.Model.Users.Entities;
using FootballSocialMedia.Repository.Context;
using FootballSocialMedia.Repository.Polls.Concretes;
using FootballSocialMedia.Repository.UnitOfWorks.Concretes;
using FootballSocialMedia.Service.Polls.Concretes;

namespace Service.Test;

[TestFixture]
public class PollServiceTest
{
    private AppDbContext _context = null!;
    private TestDb.FakeIdentity _identity = null!;
    private PollService _pollService = null!;
    private User _ali = null!;
    private User _veli = null!;

    [SetUp]
    public void SetUp()
    {
        _context = TestDb.CreateContext();
        _identity = new TestDb.FakeIdentity();
        _pollService = new PollService(new PollRepository(_context), new UnitOfWork(_context), TestDb.IdGenerator, _identity.Object);
        _ali = TestDb.AddUser(_context, "ali");
        _veli = TestDb.AddUser(_context, "veli");
    }

    [TearDown]
    public void TearDown() => _context.Dispose();

    [Test]
    public async Task VoteAsync_OneVotePerUser()
    {
        var poll = (await _pollService.CreateAsync(new CreatePollRequest("Derbiyi kim kazanır?",
            new List<string> { "Fenerbahçe", "Galatasaray", "Beraberlik" }, DateTime.UtcNow.AddDays(1)))).Data!;

        _identity.LoginAs(_ali);
        var first = await _pollService.VoteAsync(poll.Id, poll.Options[0].Id);
        var second = await _pollService.VoteAsync(poll.Id, poll.Options[1].Id);

        _identity.LoginAs(_veli);
        var third = await _pollService.VoteAsync(poll.Id, poll.Options[1].Id);

        Assert.That(first.IsSuccess, Is.True);
        Assert.That(first.Data!.MyVoteOptionId, Is.EqualTo(poll.Options[0].Id));
        Assert.That(second.IsSuccess, Is.False);
        Assert.That(third.IsSuccess, Is.True);
        Assert.That(third.Data!.TotalVotes, Is.EqualTo(2));
    }

    [Test]
    public async Task VoteAsync_ExpiredPoll_Fails()
    {
        var poll = (await _pollService.CreateAsync(new CreatePollRequest("Eski anket",
            new List<string> { "A", "B" }, DateTime.UtcNow.AddMinutes(-1)))).Data!;

        _identity.LoginAs(_ali);
        var result = await _pollService.VoteAsync(poll.Id, poll.Options[0].Id);

        Assert.That(result.IsSuccess, Is.False);
        Assert.That(result.ErrorMessage!.First(), Is.EqualTo("This poll has expired."));
    }

    [Test]
    public async Task GetAllAsync_ShowsMyVote()
    {
        var poll = (await _pollService.CreateAsync(new CreatePollRequest("Soru",
            new List<string> { "A", "B" }, DateTime.UtcNow.AddDays(1)))).Data!;

        _identity.LoginAs(_ali);
        await _pollService.VoteAsync(poll.Id, poll.Options[1].Id);

        var asAli = (await _pollService.GetAllAsync()).Data!.Single();
        _identity.LoginAs(_veli);
        var asVeli = (await _pollService.GetAllAsync()).Data!.Single();

        Assert.That(asAli.MyVoteOptionId, Is.EqualTo(poll.Options[1].Id));
        Assert.That(asVeli.MyVoteOptionId, Is.Null);
    }
}
