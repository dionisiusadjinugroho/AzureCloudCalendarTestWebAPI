using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace AzureCloudCalendarTestWebAPI.Models
{
    public partial class AzureCloudDBContext : DbContext
    {
        public AzureCloudDBContext()
        {
        }

        public AzureCloudDBContext(DbContextOptions<AzureCloudDBContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Calendar> Calendar { get; set; }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseSqlServer("Server=diontestazuresqldb.database.windows.net;Database=AzureCloudDB;User ID=rootadmin;Password=p@55w0rd;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Calendar>(entity =>
            {
                entity.Property(e => e.CalendarId)
                    .HasColumnName("CalendarID")
                    .ValueGeneratedNever();

                entity.Property(e => e.CalendarCategory).HasMaxLength(100);

                entity.Property(e => e.CalendarName).HasMaxLength(250);

                entity.Property(e => e.CreatedBy).HasMaxLength(100);

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.LastUpdatedDate).HasColumnType("datetime");

                entity.Property(e => e.ReminderAction).HasMaxLength(100);

                entity.Property(e => e.Timezone).HasMaxLength(100);
                entity.Property(e => e.CalendarEventStartDate).HasColumnType("datetime");

                entity.Property(e => e.CalendarEventEndDate).HasColumnType("datetime");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
