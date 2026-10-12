using AutoMapper;
using Core.CrossCuttingConcerns.Response;
using Core.Security.Services.Abstracts;
using FootballSocialMedia.Model.Follows.Dtos.Requests;
using FootballSocialMedia.Model.Follows.Dtos.Responses;
using FootballSocialMedia.Model.Follows.Entities;
using FootballSocialMedia.Model.Notifications.Entities;
using FootballSocialMedia.Model.Notifications.Enums;
using FootballSocialMedia.Model.Users.Entities;
using FootballSocialMedia.Repository.Follows.Abstracts;
using FootballSocialMedia.Repository.UnitOfWorks.Abstracts;
using FootballSocialMedia.Service.Follows.Abstracts;
using FootballSocialMedia.Service.RabbitMQ;
using Microsoft.AspNetCore.Identity;
using System.Net;

namespace FootballSocialMedia.Service.Follows.Concretes;

public class FollowService(IFollowRepository followRepository, IMapper mapper, IUnitOfWork unitOfWork,
    INotificationPublisher notificationPublisher, IIdentityService identityService, UserManager<User> userManager) : IFollowService
{
    public async Task<ServiceResult<FollowDto>> FollowAsync(CreateFollowRequest request)
    {
        var me = identityService.GetUserId;

        if (me == request.FollowingId)
            return ServiceResult<FollowDto>.Fail("You cannot follow yourself.", HttpStatusCode.BadRequest);

        if (await userManager.FindByIdAsync(request.FollowingId) is null)
            return ServiceResult<FollowDto>.Fail("User not found.", HttpStatusCode.NotFound);

        var existingFollow = await followRepository.GetByIdAsync(me, request.FollowingId);
        if (existingFollow != null)
            return ServiceResult<FollowDto>.Fail("Already following.", HttpStatusCode.BadRequest);

        var follow = new Follow { FollowerId = me, FollowingId = request.FollowingId };
        await followRepository.AddAsync(follow);
        await unitOfWork.SaveChangesAsync();

        notificationPublisher.SendNotification(new Notification
        {
            UserId = request.FollowingId,
            SenderId = me,
            SenderUserName = identityService.UserName,
            Type = NotificationType.Follow,
            Content = $"{identityService.UserName} started following you."
        });

        var followResponse = mapper.Map<FollowDto>(follow);
        return ServiceResult<FollowDto>.SuccessAsCreated(followResponse, $"api/v1/follows/{follow.FollowerId}/{follow.FollowingId}");
    }

    public async Task<ServiceResult> UnfollowAsync(string followingId)
    {
        var follow = await followRepository.GetByIdAsync(identityService.GetUserId, followingId);
        if (follow == null)
            return ServiceResult.Fail("Follow relationship not found.", HttpStatusCode.NotFound);

        followRepository.Delete(follow);
        await unitOfWork.SaveChangesAsync();
        return ServiceResult.Success(HttpStatusCode.NoContent);
    }

    public async Task<ServiceResult<List<FollowDto>>> GetFollowingAsync(string followerId)
    {
        var follows = await followRepository.GetByFollowerIdAsync(followerId);
        return ServiceResult<List<FollowDto>>.Success(mapper.Map<List<FollowDto>>(follows), HttpStatusCode.OK);
    }

    public async Task<ServiceResult<List<FollowDto>>> GetFollowersAsync(string followingId)
    {
        var follows = await followRepository.GetByFollowingIdAsync(followingId);
        return ServiceResult<List<FollowDto>>.Success(mapper.Map<List<FollowDto>>(follows), HttpStatusCode.OK);
    }
}
