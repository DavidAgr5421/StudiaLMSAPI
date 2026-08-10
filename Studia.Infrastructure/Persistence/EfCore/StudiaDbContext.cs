using Microsoft.EntityFrameworkCore;
using Studia.Domain.Activities;
using Studia.Domain.Auth;
using Studia.Domain.Cohorts;
using Studia.Domain.Courses;
using Studia.Domain.Enrollments;
using Studia.Domain.Notifications;
using Studia.Domain.Sections;
using Studia.Domain.Submissions;
using Studia.Domain.Users;

namespace Studia.Infrastructure.Persistence.EfCore;

public class StudiaDbContext(DbContextOptions<StudiaDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();
    public DbSet<Course> Courses => Set<Course>();
    public DbSet<Cohort> Cohorts => Set<Cohort>();
    public DbSet<Enrollment> Enrollments => Set<Enrollment>();
    public DbSet<Section> Sections => Set<Section>();
    public DbSet<Activity> Activities => Set<Activity>();
    public DbSet<Submission> Submissions => Set<Submission>();
    public DbSet<Notification> Notifications => Set<Notification>();
    public DbSet<RevokedToken> RevokedTokens => Set<RevokedToken>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(StudiaDbContext).Assembly);
    }
}
