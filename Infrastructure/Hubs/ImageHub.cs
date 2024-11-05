using System.Threading.Tasks;
using MatMatShop.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace MatMatShop.Infrastructure.Hubs
{
    public class ImageHub : Hub<IImageClient>
    {
        public async Task SendNewImage(int imageId, string imageUrl, string tag)
        {
            await Clients.All.ReceiveNewImage(imageId, imageUrl, tag);
        }

        // Phát tin nhắn cho tất cả client khi có ảnh bị xóa
        public async Task SendDeleteImage(int imageId, string imageUrl, string tag)
        {
            await Clients.All.ReceiveDeleteImage(imageId, imageUrl, tag);
        }

        public async Task SendRestoreImage(int imageId, string imageUrl, string tag)
        {
            await Clients.All.ReceiveRestoreImage(imageId, imageUrl, tag);
        }
    }
}
