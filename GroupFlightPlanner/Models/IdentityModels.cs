using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace GroupFlightPlanner.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit https://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        //Add an Airline model as a db Table
        public DbSet<Airline> Airlines { get; set; }

        //Add an Airplane model as a db Table
        public DbSet<Airplane> Airplanes { get; set; }

        //Add an Flight model as a db Table
        public DbSet<Flight> Flights { get; set; }

        //Add an Event model as a db Table
        public DbSet<Event> Events { get; set; }

        //Add a Location model as a db Table
        public DbSet<Location> Locations { get; set; }

        //Add an Organization model as a db Table
        public DbSet<Organization> Organizations { get; set; }

        //Add an Activity model as a db Table
        public DbSet<Activity> Activities { get; set; }

        //Add a Group model as a db Table
        public DbSet<Group> Groups { get; set; }

        //Add a Volunteer model as a db Table
        public DbSet<Volunteer> Volunteers { get; set; }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
    }
}