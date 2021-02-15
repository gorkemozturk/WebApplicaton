using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication
{
    public static class SeedDatabase
    {
        public static async Task InitializeAsync(IServiceProvider service)
        {
            await InitialSeed(service.GetRequiredService<PrimaryDbContext>());
        }

        public static async Task InitialSeed(PrimaryDbContext context)
        {
            if (context.Rooms.Any())
            {
                return;
            }

            context.Rooms.Add(new Models.RoomEntity
            {
                Id = Guid.Parse("ee2b83be-91db-4de5-8122-35a9e9195976"),
                Name = "Driscoll Suite",
                Rate = 23959,
                CreatedAt = DateTime.Now.AddMonths(-2)
            });

            context.Rooms.Add(new Models.RoomEntity 
            {
                Id = Guid.Parse("301df04d-8679-4b1b-ab92-0a586ae53d08"),
                Name = "Oxford Suite",
                Rate = 10119,
                CreatedAt = DateTime.Now.AddMonths(-1)
            });

            context.Rooms.Add(new Models.RoomEntity
            {
                Id = Guid.Parse("301df04d-8679-4b1b-ab92-0a586ae53d01"),
                Name = "Premium Oxford Suite",
                Rate = 65896,
                CreatedAt = DateTime.Now.AddMonths(-4)
            });

            context.Rooms.Add(new Models.RoomEntity
            {
                Id = Guid.Parse("ee2b83be-91db-4de5-8122-35a9e9195972"),
                Name = "Newer Driscoll Suite",
                Rate = 23658,
                CreatedAt = DateTime.Now.AddMonths(-3)
            });

            await context.SaveChangesAsync();
        }
    }
}
