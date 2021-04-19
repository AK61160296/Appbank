using Microsoft.EntityFrameworkCore;

namespace Project_Appbank.Models.DBModels
{
    public partial class appbankContext : DbContext
    {
        public appbankContext()
        {
        }

        public appbankContext(DbContextOptions<appbankContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Account> Account { get; set; }
        public virtual DbSet<Transaction> Transaction { get; set; }
        public virtual DbSet<User> User { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseMySql("server=localhost;port=3307;user=root;password=2feet254233;database=appbank", x => x.ServerVersion("8.0.23-mysql"));
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Account>(entity =>
            {
                entity.HasKey(e => e.AcId)
                    .HasName("PRIMARY");

                entity.ToTable("account");

                entity.HasIndex(e => e.UserId)
                    .HasName("fk_user_idx");

                entity.Property(e => e.AcId).HasColumnName("ac_id");

                entity.Property(e => e.AcBalance)
                    .HasColumnName("ac_balance")
                    .HasColumnType("decimal(10,2)");

                entity.Property(e => e.AcIsActive).HasColumnName("ac_is_active");

                entity.Property(e => e.AcName)
                    .HasColumnName("ac_name")
                    .HasColumnType("varchar(255)")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.AcNumber)
                    .IsRequired()
                    .HasColumnName("ac_number")
                    .HasColumnType("varchar(10)")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.UserId).HasColumnName("user_id");
            });

            modelBuilder.Entity<Transaction>(entity =>
            {
                entity.HasKey(e => e.TsId)
                    .HasName("PRIMARY");

                entity.ToTable("transaction");

                entity.HasIndex(e => e.TsAcId)
                    .HasName("ts_ac_id_idx1");

                entity.HasIndex(e => new { e.TsAcDestinationId, e.TsAcId })
                    .HasName("ts_ac_id_idx");

                entity.Property(e => e.TsId).HasColumnName("ts_id");

                entity.Property(e => e.TsAcDestinationId).HasColumnName("ts_ac_destination_id");

                entity.Property(e => e.TsAcId).HasColumnName("ts_ac_id");

                entity.Property(e => e.TsBalance)
                    .HasColumnName("ts_balance")
                    .HasColumnType("decimal(10,2)");

                entity.Property(e => e.TsDate)
                    .HasColumnName("ts_date")
                    .HasColumnType("date");

                entity.Property(e => e.TsDetail)
                    .IsRequired()
                    .HasColumnName("ts_detail")
                    .HasColumnType("varchar(255)")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.TsMoney)
                    .HasColumnName("ts_money")
                    .HasColumnType("decimal(10,2)");

                entity.Property(e => e.TsNote)
                    .HasColumnName("ts_note")
                    .HasColumnType("varchar(255)")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_0900_ai_ci");

                entity.Property(e => e.TsType).HasColumnName("ts_type");

                entity.HasOne(d => d.TsAc)
                    .WithMany(p => p.Transaction)
                    .HasForeignKey(d => d.TsAcId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("ts_ac_id");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("user");

                entity.Property(e => e.UserId).HasColumnName("user_id");

                entity.Property(e => e.UserEmail)
                    .IsRequired()
                    .HasColumnName("user_email")
                    .HasColumnType("varchar(45)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.UserIsActive).HasColumnName("user_is_active");

                entity.Property(e => e.UserName)
                    .IsRequired()
                    .HasColumnName("user_name")
                    .HasColumnType("varchar(45)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
