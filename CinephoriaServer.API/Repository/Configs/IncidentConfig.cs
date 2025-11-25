using CinephoriaServer.API.Models.PostgresqlDb;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.AspNetCore.Components.RenderTree;

namespace CinephoriaServer.API.Repository
{
    public class IncidentConfig : IEntityTypeConfiguration<Incident>
    {
        public void Configure(EntityTypeBuilder<Incident> builder)
        {
            // Clé primaire
            builder.HasKey(i => i.IncidentId);

            // Propriétés simples
            builder.Property(i => i.Description)
                   .IsRequired()
                   .HasMaxLength(1000);

            builder.Property(i => i.Status)
                   .IsRequired();

            builder.Property(i => i.ReportedAt)
                   .IsRequired();

            builder.Property(i => i.ResolvedAt)
                   .IsRequired(false);

            
            builder.Property(i => i.ResolvedById)
                   .IsRequired(false);

            // Conversion de ImageUrls (IList<string> vers chaîne et vice versa)
            builder.Property(i => i.ImageUrls)
                   .HasConversion(
                       v => string.Join(",", v),
                       v => v.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList() 
                   )
                   .Metadata.SetValueComparer(
                       new ValueComparer<List<string>>(
                           (c1, c2) => c1.SequenceEqual(c2),
                           c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                           c => c.ToList()
                       )
                   );

            // Relation avec Theater
            builder.HasOne(i => i.Theater)
                   .WithMany(t => t.Incidents)
                   .HasForeignKey(i => i.TheaterId)
                   .OnDelete(DeleteBehavior.Cascade);

            // Relation avec AppUser (ReportedBy)
            builder.HasOne(i => i.ReportedBy)
                   .WithMany(u => u.ReportedIncidents)
                   .HasForeignKey(i => i.ReportedById)
                   .OnDelete(DeleteBehavior.Restrict);

            // Relation avec AppUser (ResolvedBy)
            builder.HasOne(i => i.ResolvedBy)
                   .WithMany(u => u.ResolvedByIncidents)
                   .HasForeignKey(i => i.ResolvedById)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
