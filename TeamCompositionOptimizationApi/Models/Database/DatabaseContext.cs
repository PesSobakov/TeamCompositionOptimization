using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using Microsoft.EntityFrameworkCore;
using TeamCompositionOptimizationApi.Models.Optimization;

namespace TeamCompositionOptimizationApi.Models.Database;

public partial class DatabaseContext : DbContext
{
    public DatabaseContext()
    {
    }

    public DatabaseContext(DbContextOptions<DatabaseContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Candidate> Candidates { get; set; }

    public virtual DbSet<CandidateCompetency> CandidateCompetencies { get; set; }

    public virtual DbSet<Competency> Competencies { get; set; }

    public virtual DbSet<Compliance> Compliances { get; set; }

    public virtual DbSet<HelpPage> HelpPages { get; set; }

    public virtual DbSet<Indicator> Indicators { get; set; }

    public virtual DbSet<OptimizationResult> OptimizationResults { get; set; }

    public virtual DbSet<Subscription> Subscriptions { get; set; }

    public virtual DbSet<TeamOption> TeamOptions { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        //optionsBuilder.UseNpgsql($"Server=localhost;Port=5432;Database=team_composition;User Id=testuser;Password=testuser;");
    }
}
