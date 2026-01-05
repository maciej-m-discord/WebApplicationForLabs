using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WebApplication.Data.Models;

namespace WebApplication.Data;

public class AppDbContext :IdentityDbContext<User,IdentityRole<int>,int>
{
  public AppDbContext(DbContextOptions<AppDbContext> options) :base(options)
  {
    
  }
  public DbSet<Post> Posts { get; set; }
  public DbSet<User> Users { get; set; }
  public DbSet<Like> Likes { get; set; }
  public DbSet<Comment> Comments { get; set; }
  public DbSet<Bookmark> Bookmarks { get; set; }
  public DbSet<Report> Reports { get; set; }
  public DbSet<Story> Stories { get; set; }
  public DbSet<Hashtag> Hashtags { get; set; }
  public DbSet<Follow> Follows { get; set; }
  public DbSet<Mutual> Mutuals { get; set; }
  public DbSet<Notification> Notifications { get; set; }
  
  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    
    modelBuilder.Entity<User>()
      .HasMany(u => u.Posts)
      .WithOne(p => p.User)
      .HasForeignKey(p => p.UserId);
    
    modelBuilder.Entity<Like>()
      .HasKey(l => new { l.PostId, l.UserId });
    
    modelBuilder.Entity<Like>()
      .HasOne(l => l.Post)
      .WithMany(p => p.Likes)
      .HasForeignKey(l=>l.PostId)
      .OnDelete(DeleteBehavior.Cascade);

    modelBuilder.Entity<Like>()
      .HasOne(l => l.User)
      .WithMany(u => u.Likes)
      .HasForeignKey(l =>l.UserId)
      .OnDelete(DeleteBehavior.Restrict);
    
    modelBuilder.Entity<Comment>()
      .HasOne(l => l.Post)
      .WithMany(p => p.Comments)
      .HasForeignKey(l=>l.PostId)
      .OnDelete(DeleteBehavior.NoAction);

    modelBuilder.Entity<Comment>()
      .HasOne(l => l.User)
      .WithMany(u => u.Comments)
      .HasForeignKey(l =>l.UserId)
      .OnDelete(DeleteBehavior.Restrict);

    modelBuilder.Entity<Bookmark>()
      .HasKey(f => new { f.PostId, f.UserId });
    
    modelBuilder.Entity<Bookmark>()
      .HasOne(l => l.Post)
      .WithMany(p => p.Bookmarks)
      .HasForeignKey(l=>l.PostId)
      .OnDelete(DeleteBehavior.NoAction);

    modelBuilder.Entity<Bookmark>()
      .HasOne(l => l.User)
      .WithMany(u => u.Bookmarks)
      .HasForeignKey(l =>l.UserId)
      .OnDelete(DeleteBehavior.Restrict);
    
    modelBuilder.Entity<Report>()
      .HasKey(f => new { f.PostId, f.UserId });
    
    modelBuilder.Entity<Report>()
      .HasOne(l => l.Post)
      .WithMany(p => p.Reports)
      .HasForeignKey(l=>l.PostId)
      .OnDelete(DeleteBehavior.NoAction);

    modelBuilder.Entity<Report>()
      .HasOne(l => l.User)
      .WithMany(u => u.Reports)
      .HasForeignKey(l =>l.UserId)
      .OnDelete(DeleteBehavior.Restrict);

    modelBuilder.Entity<User>()
      .HasMany(u => u.Stories)
      .WithOne(u => u.User)
      .HasForeignKey(u => u.UserId);
    
    
    base.OnModelCreating(modelBuilder);
    
    modelBuilder.Entity<User>().ToTable("Users");
    modelBuilder.Entity<IdentityRole<int>>().ToTable("Roles");
    modelBuilder.Entity<IdentityUserRole<int>>().ToTable("UserRoles");
    modelBuilder.Entity<IdentityUserClaim<int>>().ToTable("UserClaims");
    modelBuilder.Entity<IdentityUserLogin<int>>().ToTable("UserLogins");
    modelBuilder.Entity<IdentityRoleClaim<int>>().ToTable("RoleClaims");
    modelBuilder.Entity<IdentityUserToken<int>>().ToTable("UserTokens");
    
    //Follow Configuration
    modelBuilder.Entity<Follow>()
      .HasOne(f => f.Sender)
      .WithMany()
      .HasForeignKey(f => f.SenderId)
      .OnDelete(DeleteBehavior.NoAction);
    
    modelBuilder.Entity<Follow>()
      .HasOne(f => f.Receiver)
      .WithMany()
      .HasForeignKey(f => f.ReceiverId)
      .OnDelete(DeleteBehavior.Cascade);
    
    modelBuilder.Entity<Mutual>()
      .HasOne(m => m.Sender)
      .WithMany()
      .HasForeignKey(m => m.SenderId)
      .OnDelete(DeleteBehavior.NoAction);
    
    modelBuilder.Entity<Mutual>()
      .HasOne(m => m.Receiver)
      .WithMany()
      .HasForeignKey(m => m.ReceiverId)
      .OnDelete(DeleteBehavior.Cascade);
  }
}