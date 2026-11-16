using Kofschip.Core.Model;
using Microsoft.EntityFrameworkCore;

namespace Kofschip.Data;

public class KofschipDbContext(DbContextOptions<KofschipDbContext> options) : DbContext(options)
{
    public DbSet<Verb> Verbs => Set<Verb>();

    public DbSet<Sentence> Sentences => Set<Sentence>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Verb>(verb =>
        {
            verb.Property(v => v.Infinitive).HasMaxLength(60);
            verb.Property(v => v.Stem).HasMaxLength(60);
            verb.HasIndex(v => v.Infinitive).IsUnique();
        });

        modelBuilder.Entity<Sentence>(sentence =>
        {
            sentence.Property(s => s.Text).HasMaxLength(200);

            // Stored as text so the table stays readable.
            sentence.Property(s => s.Tense).HasConversion<string>().HasMaxLength(20);
            sentence.Property(s => s.Person).HasConversion<string>().HasMaxLength(10);
            sentence.HasIndex(s => s.Tense);

            sentence.HasOne(s => s.Verb)
                .WithMany(v => v.Sentences)
                .HasForeignKey(s => s.VerbId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
