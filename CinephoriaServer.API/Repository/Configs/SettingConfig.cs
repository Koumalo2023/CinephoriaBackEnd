using CinephoriaServer.API.Models.PostgresqlDb;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CinephoriaServer.API.Repository.Configs
{
    public class SettingConfig : IEntityTypeConfiguration<Setting>
    {
        public void Configure(EntityTypeBuilder<Setting> builder)
        {
            builder.ToTable("settings");

            builder.HasKey(s => s.Id);

            builder.Property(s => s.Id)
                .HasColumnName("id")
                .ValueGeneratedOnAdd();

            builder.Property(s => s.SettingKey)
                .HasColumnName("setting_key")
                .HasMaxLength(255)
                .IsRequired();

            builder.Property(s => s.SettingValue)
                .HasColumnName("setting_value")
                .IsRequired();

            builder.Property(s => s.Category)
                .HasColumnName("category")
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(s => s.Description)
                .HasColumnName("description")
                .HasMaxLength(500);

            builder.Property(s => s.CreatedAt)
                .HasColumnName("created_at")
                .IsRequired();

            builder.Property(s => s.UpdatedAt)
                .HasColumnName("updated_at")
                .IsRequired();

            // Index unique sur la clé du paramètre
            builder.HasIndex(s => s.SettingKey)
                .IsUnique();

            // Index sur la catégorie pour les requêtes par catégorie
            builder.HasIndex(s => s.Category);
        }
    }
}