using System.Threading.Tasks;

namespace MatMatShop.Interfaces
{
    public interface IImageClient
    {
        Task ReceiveNewImage(int id, string imageUrl, string tag);
        Task ReceiveDeleteImage(int id, string imageUrl, string tag);
        Task ReceiveRestoreImage(int id, string imageUrl, string tag);
    }
}
