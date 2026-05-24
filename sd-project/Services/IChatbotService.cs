using System.Threading.Tasks;

namespace EventBooking.Services
{
    public interface IChatbotService
    {
        Task<string> GetResponseAsync(string userMessage);
    }
}
