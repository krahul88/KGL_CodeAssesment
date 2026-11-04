using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using ProductionPlantEvaluation.Domain.DTO;

namespace ProductionPlantEvaluation.Infrastructure.Repositories
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(
             DbContextOptions<ApplicationDbContext> options)
             : base(options)
        {
        }

        public DbSet<Ruleset> Rulesets => Set<Ruleset>();
        public DbSet<Rule> Rules => Set<Rule>();
        public DbSet<Condition> Conditions => Set<Condition>();
        public DbSet<EvaluationLog> EvaluationLogs => Set<EvaluationLog>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Ruleset>().HasKey(x => x.RSId);
            base.OnModelCreating(modelBuilder);
        }
    }
}
