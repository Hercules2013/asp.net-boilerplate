using Abp.Application.Services.Dto;

namespace Taskever.Application.Services.Dto.Friendships
{
    public class AcceptFriendshipInput : IInputDto
    {
        public int Id { get; set; } //TODO: Get UserId and FriendId instead of Id?
    }
}