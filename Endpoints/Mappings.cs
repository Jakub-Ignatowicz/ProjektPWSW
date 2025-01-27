using Projekt.Models;
using System.Security.Claims;
using Projekt.DTOs;
using Projekt.Services;
using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;
using Isopoh.Cryptography.Argon2;

namespace Projekt.Endpoints;

public static class Mappings
{
    public static void MapEndpoints(this WebApplication app)
    {
        app.MapPost("/api/auth/register", async (UserDTO userDTO, [FromServices] IDbService db) =>
        {
            User? user = await db.GetUser(userDTO.Username);

            if (user != null)
            {
                return Results.BadRequest("User with this username exists.");
            }

            if (!ValidatePassword(userDTO.Password))
            {
                return Results.BadRequest("Password has to have number, small char, capital char and be longer than 6.");
            }


            var passwordHash = Argon2.Hash(userDTO.Password);

            await db.CreateUser(new User
            {
                Username = userDTO.Username,
                PasswordHash = passwordHash,
            });

            return Results.Ok("User created.");
        });

        app.MapPost("/api/auth/login", async (UserDTO userDTO, [FromServices] IDbService db, [FromServices] JWTService jwtService) =>
        {
            User? user = await db.GetUser(userDTO.Username);

            if (user == null)
            {
                return Results.BadRequest("User with this username doesn't exists.");
            }

            if (!Argon2.Verify(user.PasswordHash, userDTO.Password))
            {
                return Results.BadRequest("Invalid password.");
            }

            var token = jwtService.GenerateToken(userDTO.Username);

            return Results.Ok(new { Token = token });
        });

        app.MapGet("/api/user/{username}", async (string username, [FromServices] IDbService db, ClaimsPrincipal user) =>
        {
            if (user.Identity?.Name != username)
            {
                return Results.Unauthorized();
            }

            var userData = await db.GetUser(username);

            if (userData == null)
            {
                return Results.NotFound("User not found.");
            }

            return Results.Ok(new
            {
                Username = username,
                Trackers = userData.Trackers.Select(tracker => new
                {
                    TrackerID = tracker.TrackerID,
                    Progress = tracker.Progress,
                    TargetValue = tracker.TargetValue,
                    StartDate = tracker.StartDate,
                    TrackerType = tracker.GetType().ToString().Split(".")[2],
                    FinishDate = tracker.FinishDate,
                    UserId = tracker.UserID,
                })
            });
        })
        .RequireAuthorization();

        app.MapPost("/api/user/{username}/trackers", async (string username, TrackerDTO tracker, [FromServices] IDbService db, ClaimsPrincipal user) =>
        {
            if (user.Identity?.Name != username)
            {
                return Results.Unauthorized();
            }

            Tracker trackerNew = null;


            var userData = await db.GetUser(username);

            if (userData == null)
            {
                return Results.NotFound("User not found.");
            }
            switch (tracker.TrackerType)
            {
                case "PhisicalActivityTracker":
                    trackerNew = new PhisicalActivityTracker
                    {
                        TargetValue = tracker.TargetValue,
                        Progress = tracker.Progress,
                        ActivityTypeId = (int)tracker.ActivityTypeId,
                        FinishDate = tracker.FinishDate,
                        StartDate = tracker.StartDate,
                        User = userData
                    };
                    break;
                case "SleepTracker":
                    trackerNew = new SleepTracker
                    {
                        TargetValue = tracker.TargetValue,
                        Progress = tracker.Progress,
                        FinishDate = tracker.FinishDate,
                        StartDate = tracker.StartDate,
                        User = userData
                    };
                    break;
                case "WaterIntakeTracker":
                    trackerNew = new WaterIntakeTracker
                    {
                        TargetValue = tracker.TargetValue,
                        Progress = tracker.Progress,
                        FinishDate = tracker.FinishDate,
                        StartDate = tracker.StartDate,
                        User = userData
                    };
                    break;
            }

            var trackerCreated = await db.CreateTracker(trackerNew);

            return Results.Ok(new
            {
                TrackerID = trackerCreated.TrackerID,
                TargetValue = tracker.TargetValue,
                Progress = tracker.Progress,
                FinishDate = tracker.FinishDate,
                StartDate = tracker.StartDate,
            });
        })
        .RequireAuthorization();

        app.MapPatch("/api/user/{username}/trackers/{trackerID}", async (string username, int trackerID, float value, TrackerDTO tracker, [FromServices] IDbService db, ClaimsPrincipal user) =>
        {
            if (user.Identity?.Name != username)
            {
                return Results.Unauthorized();
            }

            try
            {
                await db.UpdateTracker(trackerID, value);
            }
            catch
            {
                return Results.BadRequest($"Tracker with ID {trackerID} not found.");
            }

            return Results.Ok();
        });
    }

    private static bool ValidatePassword(string password)
    {
        if (password.Length < 6)
        {
            return false;
        }

        string pattern = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).+$";

        if (Regex.IsMatch(password, pattern))
        {
            return true;
        }

        return false;
    }

}
