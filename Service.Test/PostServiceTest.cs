using AutoMapper;
using Core.Infrastructure.Services.CloudinaryService.Abstracts;
using Core.Infrastructure.Services.IdGenerator.Abstracts;
using FootballSocialMedia.Model.Posts.Dtos.Requests;
using FootballSocialMedia.Model.Posts.Dtos.Responses;
using FootballSocialMedia.Model.Posts.Entities;
using FootballSocialMedia.Repository.Posts.Abstracts;
using FootballSocialMedia.Repository.UnitOfWorks.Abstracts;
using FootballSocialMedia.Service.Elasticsearch.Abstracts;
using FootballSocialMedia.Service.Posts.Concretes;
using Moq;
using System.Net;

namespace Service.Test
{
    [TestFixture]
    public class PostServiceTest
    {
        private Mock<IPostRepository> _postRepositoryMock;
        private Mock<IMapper> _mapperMock;
        private Mock<IUnitOfWork> _unitOfWorkMock;
        private Mock<IIdGenerator> _idGeneratorMock;
        private Mock<ICloudinaryService> _cloudinaryServiceMock;
        private Mock<IElasticsearchService> _elasticsearchServiceMock;
        private PostService _postService;

        [SetUp]
        public void SetUp()
        {
            _postRepositoryMock = new Mock<IPostRepository>();
            _mapperMock = new Mock<IMapper>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _idGeneratorMock = new Mock<IIdGenerator>();
            _cloudinaryServiceMock = new Mock<ICloudinaryService>();
            _elasticsearchServiceMock = new Mock<IElasticsearchService>();

            _postService = new PostService(
                _postRepositoryMock.Object,
                _mapperMock.Object,
                _unitOfWorkMock.Object,
                _idGeneratorMock.Object,
                _cloudinaryServiceMock.Object,
                _elasticsearchServiceMock.Object
                
            );
        }

        [Test]
        public async Task CreateAsync_WhenCalled_ReturnsSuccessResult()
        {
            // Arrange
            var request = new CreatePostRequest("UserId","Test content", null, null);

            var post = new Post
            {
                Id = Guid.NewGuid(),
                Content = request.Content
            };

            _mapperMock.Setup(m => m.Map<Post>(request)).Returns(post);
            _idGeneratorMock.Setup(id => id.GenerateId()).Returns(post.Id);
            _postRepositoryMock.Setup(r => r.AddAsync(post)).Returns(ValueTask.CompletedTask);
            _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

            var expectedResponse = new CreatePostResponse(post.Id, post.UserId,post.Content, new List<string>(), new List<string>(), DateTime.UtcNow);
            _mapperMock.Setup(m => m.Map<CreatePostResponse>(post)).Returns(expectedResponse);


            // Act
            var result = await _postService.CreateAsync(request);

            // Assert
            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(HttpStatusCode.Created, result.Status);
            Assert.AreEqual(expectedResponse.Id, result.Data.Id);
            _postRepositoryMock.Verify(r => r.AddAsync(post), Times.Once);
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
        }

        [Test]
        public async Task DeleteAsync_PostNotFound_ReturnsFailResult()
        {
            // Arrange
            var postId = Guid.NewGuid();
            _postRepositoryMock.Setup(r => r.GetByIdAsync(postId)).ReturnsAsync((Post)null);

            // Act
            var result = await _postService.DeleteAsync(postId);

            // Assert
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual(HttpStatusCode.NotFound, result.Status);
            Assert.AreEqual("Post not found", result.ErrorMessage.FirstOrDefault());
            _postRepositoryMock.Verify(r => r.GetByIdAsync(postId), Times.Once);
            _postRepositoryMock.Verify(r => r.Delete(It.IsAny<Post>()), Times.Never);
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Never);
        }

        [Test]
        public async Task DeleteAsync_PostExists_ReturnsSuccessResult()
        {
            // Arrange
            var post = new Post { Id = Guid.NewGuid() };
            _postRepositoryMock.Setup(r => r.GetByIdAsync(post.Id)).ReturnsAsync(post);

            // Act
            var result = await _postService.DeleteAsync(post.Id);

            // Assert
            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(HttpStatusCode.NoContent, result.Status);
            _postRepositoryMock.Verify(r => r.GetByIdAsync(post.Id), Times.Once);
            _postRepositoryMock.Verify(r => r.Delete(post), Times.Once);
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
        }

        [Test]
        public async Task GetByIdAsync_PostNotFound_ReturnsFailResult()
        {
            // Arrange
            var postId = Guid.NewGuid();
            _postRepositoryMock.Setup(r => r.GetByIdAsync(postId)).ReturnsAsync((Post)null);

            // Act
            var result = await _postService.GetByIdAsync(postId);

            // Assert
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual(HttpStatusCode.NotFound, result.Status);
            Assert.AreEqual("Post not found", result.ErrorMessage?.FirstOrDefault());
            _postRepositoryMock.Verify(r => r.GetByIdAsync(postId), Times.Once);
        }

        [Test]
        public async Task GetByIdAsync_PostExists_ReturnsSuccessResult()
        {
            // Arrange
            var post = new Post { Id = Guid.NewGuid(), Content = "Test Content" };
            var postDto = new PostDto { Id = post.Id, Content = post.Content };

            _postRepositoryMock.Setup(r => r.GetByIdAsync(post.Id)).ReturnsAsync(post);
            _mapperMock.Setup(m => m.Map<PostDto>(post)).Returns(postDto);

            // Act
            var result = await _postService.GetByIdAsync(post.Id);

            // Assert
            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(HttpStatusCode.OK, result.Status);
            Assert.AreEqual(postDto.Id, result.Data.Id);
            _postRepositoryMock.Verify(r => r.GetByIdAsync(post.Id), Times.Once);
        }
    }
}
