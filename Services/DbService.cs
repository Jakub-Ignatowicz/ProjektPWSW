using Microsoft.EntityFrameworkCore;
using Projekt.Models;

namespace Projekt.Services;

public class DbService : IDbService
{
    private readonly ApplicationDbContext _context;
    public DbService(ApplicationDbContext context)
    {
        _context = context;
    }


    public async Task<Tracker> CreateTracker(Tracker tracker)
    {
        var trackerNew = await _context.Trackers.AddAsync(tracker);
        await _context.SaveChangesAsync();

        return trackerNew.Entity;
    }

    public async Task CreateUser(User user)
    {
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();
    }


    public async Task<User?> GetUser(string username)
    {
        return await _context.Users.Include(e => e.Trackers).FirstOrDefaultAsync(c => c.Username == username);
    }

    public async Task UpdateTracker(int trackerID, float value)
    {
        var tracker = await _context.Trackers.FindAsync(trackerID);

        if (tracker == null)
        {
            throw new KeyNotFoundException($"Tracker with ID {trackerID} not found.");
        }

        tracker.Progress += value;
        await _context.SaveChangesAsync();
    }
}
