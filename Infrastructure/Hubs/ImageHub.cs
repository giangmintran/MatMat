using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace MatMatShop.Infrastructure.Hubs
{
    public class ImageHub : Hub
    {
        //public async Task SendNewImage(int imageId, string imageUrl, string tags)
        //{
        //    await Clients.All.SendAsync("ReceiveNewImage", imageId, imageUrl, tags);
        //}

        //// Phát tin nhắn cho tất cả client khi có ảnh bị xóa
        //public async Task SendDeleteImage(int imageId)
        //{
        //    await Clients.All.SendAsync("ReceiveDeleteImage", imageId);
        //}

        //public async Task SendRestoreImage(int imageId, string imageUrl, string tags)
        //{
        //    await Clients.All.SendAsync("ReceiveDeleteImage", imageId, imageUrl, tags);
        //}
    }
}
