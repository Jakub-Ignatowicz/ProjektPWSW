using Projekt.Models;
namespace Projekt.Services;

public interface IDbService
{
    Task<User?> GetUser(string username);
    Task<Tracker> CreateTracker(Tracker tracker);
    Task UpdateTracker(int trackerID, float value);
    Task CreateUser(User user);
}
