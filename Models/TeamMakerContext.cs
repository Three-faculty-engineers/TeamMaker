using Microsoft.EntityFrameworkCore;
namespace Models;

public class TeamMakerContext : DbContext
{
    public DbSet<Korisnik> Korisnici {get;set;}
    public DbSet<Team> Timovi {get;set;}

    public DbSet<Task> Taskovi {get;set;}

    public DbSet<Sprint> Sprints {get;set;}


    public DbSet<Poruka> Poruke {get;set;}

    public DbSet<Ocena> Ocene {get;set;}

    public DbSet<Objava> Objave {get;set;}

    public DbSet<Invite> Invites {get;set;}


    public TeamMakerContext(DbContextOptions options):base(options)
    {

    }

}