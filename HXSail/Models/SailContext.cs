using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace HXSail.Models
{
    public partial class SailContext : DbContext
    {
        public SailContext()
        {
        }

        public SailContext(DbContextOptions<SailContext> options)
            : base(options)
        {
        }

        public virtual DbSet<AnnualFeeStructure> AnnualFeeStructure { get; set; }
        public virtual DbSet<Boat> Boat { get; set; }
        public virtual DbSet<BoatType> BoatType { get; set; }
        public virtual DbSet<Country> Country { get; set; }
        public virtual DbSet<Member> Member { get; set; }
        public virtual DbSet<Membership> Membership { get; set; }
        public virtual DbSet<MembershipType> MembershipType { get; set; }
        public virtual DbSet<MemberTask> MemberTask { get; set; }
        public virtual DbSet<Parking> Parking { get; set; }
        public virtual DbSet<Province> Province { get; set; }
        public virtual DbSet<Surcharge> Surcharge { get; set; }
        public virtual DbSet<Tasks> Tasks { get; set; }
        public virtual DbSet<Town> Town { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseSqlServer("Server=.\\sqlexpress;Database=Sail;Trusted_Connection=True;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AnnualFeeStructure>(entity =>
            {
                entity.HasKey(e => e.Year)
                    .ForSqlServerIsClustered(false);

                entity.ToTable("annualFeeStructure");

                entity.Property(e => e.Year)
                    .HasColumnName("year")
                    .ValueGeneratedNever();

                entity.Property(e => e.AnnualFee).HasColumnName("annualFee");

                entity.Property(e => e.EarlyDiscountEndDate)
                    .HasColumnName("earlyDiscountEndDate")
                    .HasColumnType("date");

                entity.Property(e => e.EarlyDiscountedFee).HasColumnName("earlyDiscountedFee");

                entity.Property(e => e.ForthAndSubsequentBoatFee).HasColumnName("forthAndSubsequentBoatFee");

                entity.Property(e => e.NewMember25DiscountDate)
                    .HasColumnName("newMember25DiscountDate")
                    .HasColumnType("date");

                entity.Property(e => e.NewMember50DiscountDate)
                    .HasColumnName("newMember50DiscountDate")
                    .HasColumnType("date");

                entity.Property(e => e.NewMember75DiscountDate)
                    .HasColumnName("newMember75DiscountDate")
                    .HasColumnType("date");

                entity.Property(e => e.NonSailFee).HasColumnName("nonSailFee");

                entity.Property(e => e.RenewDeadlineDate)
                    .HasColumnName("renewDeadlineDate")
                    .HasColumnType("date");

                entity.Property(e => e.SecondBoatFee).HasColumnName("secondBoatFee");

                entity.Property(e => e.TaskExemptionFee).HasColumnName("taskExemptionFee");

                entity.Property(e => e.ThirdBoatFee).HasColumnName("thirdBoatFee");
            });

            modelBuilder.Entity<Boat>(entity =>
            {
                entity.HasKey(e => e.BoatId)
                    .ForSqlServerIsClustered(false);

                entity.ToTable("boat");

                entity.Property(e => e.BoatId).HasColumnName("boatId");

                entity.Property(e => e.BoatClass)
                    .HasColumnName("boatClass")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.BoatTypeId).HasColumnName("boatTypeId");

                entity.Property(e => e.HullColour)
                    .HasColumnName("hullColour")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.HullLength).HasColumnName("hullLength");

                entity.Property(e => e.MemberId).HasColumnName("memberId");

                entity.Property(e => e.ParkingCode)
                    .HasColumnName("parkingCode")
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.SailNumber)
                    .HasColumnName("sailNumber")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.HasOne(d => d.BoatType)
                    .WithMany(p => p.Boat)
                    .HasForeignKey(d => d.BoatTypeId)
                    .HasConstraintName("FK_boat_boatType");

                entity.HasOne(d => d.Member)
                    .WithMany(p => p.Boat)
                    .HasForeignKey(d => d.MemberId)
                    .HasConstraintName("FK_boat_member");

                entity.HasOne(d => d.ParkingCodeNavigation)
                    .WithMany(p => p.Boat)
                    .HasForeignKey(d => d.ParkingCode)
                    .HasConstraintName("FK_boat_parking");
            });

            modelBuilder.Entity<BoatType>(entity =>
            {
                entity.HasKey(e => e.BoatTypeId)
                    .ForSqlServerIsClustered(false);

                entity.ToTable("boatType");

                entity.Property(e => e.BoatTypeId).HasColumnName("boatTypeId");

                entity.Property(e => e.Chargeable).HasColumnName("chargeable");

                entity.Property(e => e.Description)
                    .HasColumnName("description")
                    .IsUnicode(false);

                entity.Property(e => e.Image)
                    .HasColumnName("image")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Sail).HasColumnName("sail");
            });

            modelBuilder.Entity<Country>(entity =>
            {
                entity.HasKey(e => e.CountryCode);

                entity.ToTable("country");

                entity.Property(e => e.CountryCode)
                    .HasColumnName("countryCode")
                    .HasMaxLength(2)
                    .IsUnicode(false)
                    .ValueGeneratedNever();

                entity.Property(e => e.FederalSalesTax).HasColumnName("federalSalesTax");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name")
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.PhonePattern)
                    .HasColumnName("phonePattern")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.PostalPattern)
                    .HasColumnName("postalPattern")
                    .HasMaxLength(255)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Member>(entity =>
            {
                entity.HasKey(e => e.MemberId)
                    .ForSqlServerIsClustered(false);

                entity.ToTable("member");

                entity.Property(e => e.MemberId).HasColumnName("memberId");

                entity.Property(e => e.City)
                    .HasColumnName("city")
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.Comment)
                    .HasColumnName("comment")
                    .IsUnicode(false);

                entity.Property(e => e.Email)
                    .HasColumnName("email")
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.FirstName)
                    .IsRequired()
                    .HasColumnName("firstName")
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.FullName)
                    .HasColumnName("fullName")
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.HomePhone)
                    .HasColumnName("homePhone")
                    .HasMaxLength(14)
                    .IsUnicode(false);

                entity.Property(e => e.LastName)
                    .IsRequired()
                    .HasColumnName("lastName")
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.PostalCode)
                    .HasColumnName("postalCode")
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.ProvinceCode)
                    .HasColumnName("provinceCode")
                    .HasMaxLength(2)
                    .IsUnicode(false);

                entity.Property(e => e.SpouseFirstName)
                    .HasColumnName("spouseFirstName")
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.SpouseLastName)
                    .HasColumnName("spouseLastName")
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.Street)
                    .HasColumnName("street")
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.TaskExempt).HasColumnName("taskExempt");

                entity.Property(e => e.UseCanadaPost).HasColumnName("useCanadaPost");

                entity.Property(e => e.YearJoined).HasColumnName("yearJoined");

                entity.HasOne(d => d.ProvinceCodeNavigation)
                    .WithMany(p => p.Member)
                    .HasForeignKey(d => d.ProvinceCode)
                    .HasConstraintName("FK_member_province");
            });

            modelBuilder.Entity<Membership>(entity =>
            {
                entity.ToTable("membership");

                entity.Property(e => e.MembershipId).HasColumnName("membershipId");

                entity.Property(e => e.Comments)
                    .HasColumnName("comments")
                    .IsUnicode(false);

                entity.Property(e => e.Fee).HasColumnName("fee");

                entity.Property(e => e.MemberId).HasColumnName("memberId");

                entity.Property(e => e.MembershipTypeName)
                    .IsRequired()
                    .HasColumnName("membershipTypeName")
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.Paid).HasColumnName("paid");

                entity.Property(e => e.Year).HasColumnName("year");

                entity.HasOne(d => d.Member)
                    .WithMany(p => p.Membership)
                    .HasForeignKey(d => d.MemberId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_membership_member");

                entity.HasOne(d => d.MembershipTypeNameNavigation)
                    .WithMany(p => p.Membership)
                    .HasForeignKey(d => d.MembershipTypeName)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_membership_membershipType");
            });

            modelBuilder.Entity<MembershipType>(entity =>
            {
                entity.HasKey(e => e.MembershipTypeName)
                    .ForSqlServerIsClustered(false);

                entity.ToTable("membershipType");

                entity.Property(e => e.MembershipTypeName)
                    .HasColumnName("membershipTypeName")
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .ValueGeneratedNever();

                entity.Property(e => e.Description)
                    .HasColumnName("description")
                    .IsUnicode(false);

                entity.Property(e => e.RatioToFull).HasColumnName("ratioToFull");
            });

            modelBuilder.Entity<MemberTask>(entity =>
            {
                entity.HasKey(e => e.MemberTaskId)
                    .ForSqlServerIsClustered(false);

                entity.ToTable("memberTask");

                entity.Property(e => e.MemberTaskId).HasColumnName("memberTaskId");

                entity.Property(e => e.Comment)
                    .HasColumnName("comment")
                    .IsUnicode(false);

                entity.Property(e => e.MemberId).HasColumnName("memberId");

                entity.Property(e => e.TaskId).HasColumnName("taskId");

                entity.Property(e => e.WednesdayDate)
                    .HasColumnName("wednesdayDate")
                    .HasColumnType("datetime");

                entity.HasOne(d => d.Member)
                    .WithMany(p => p.MemberTask)
                    .HasForeignKey(d => d.MemberId)
                    .HasConstraintName("FK_memberTask_member");

                entity.HasOne(d => d.Task)
                    .WithMany(p => p.MemberTask)
                    .HasForeignKey(d => d.TaskId)
                    .HasConstraintName("FK_memberTask_tasks");
            });

            modelBuilder.Entity<Parking>(entity =>
            {
                entity.HasKey(e => e.ParkingCode)
                    .ForSqlServerIsClustered(false);

                entity.ToTable("parking");

                entity.Property(e => e.ParkingCode)
                    .HasColumnName("parkingCode")
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .ValueGeneratedNever();

                entity.Property(e => e.ActualBoatId)
                    .HasColumnName("actualBoatId")
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.BoatTypeId).HasColumnName("boatTypeId");

                entity.HasOne(d => d.BoatType)
                    .WithMany(p => p.Parking)
                    .HasForeignKey(d => d.BoatTypeId)
                    .HasConstraintName("FK_parking_boatType");
            });

            modelBuilder.Entity<Province>(entity =>
            {
                entity.HasKey(e => e.ProvinceCode);

                entity.ToTable("province");

                entity.Property(e => e.ProvinceCode)
                    .HasColumnName("provinceCode")
                    .HasMaxLength(2)
                    .IsUnicode(false)
                    .ValueGeneratedNever();

                entity.Property(e => e.Capital)
                    .HasColumnName("capital")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.CountryCode)
                    .IsRequired()
                    .HasColumnName("countryCode")
                    .HasMaxLength(2)
                    .IsUnicode(false);

                entity.Property(e => e.IncludesFerderalTax).HasColumnName("includesFerderalTax");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.TaxCode)
                    .HasColumnName("taxCode")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.TaxRate).HasColumnName("taxRate");

                entity.HasOne(d => d.CountryCodeNavigation)
                    .WithMany(p => p.Province)
                    .HasForeignKey(d => d.CountryCode)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_province_country");
            });

            modelBuilder.Entity<Surcharge>(entity =>
            {
                entity.HasKey(e => e.SurchargeId)
                    .ForSqlServerIsClustered(false);

                entity.ToTable("surcharge");

                entity.Property(e => e.SurchargeId).HasColumnName("surchargeId");

                entity.Property(e => e.Amount).HasColumnName("amount");

                entity.Property(e => e.Description)
                    .HasColumnName("description")
                    .IsUnicode(false);

                entity.Property(e => e.EndYear).HasColumnName("endYear");

                entity.Property(e => e.Name)
                    .HasColumnName("name")
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.StartYear).HasColumnName("startYear");
            });

            modelBuilder.Entity<Tasks>(entity =>
            {
                entity.HasKey(e => e.TaskId)
                    .ForSqlServerIsClustered(false);

                entity.ToTable("tasks");

                entity.Property(e => e.TaskId).HasColumnName("taskId");

                entity.Property(e => e.Description)
                    .HasColumnName("description")
                    .IsUnicode(false);

                entity.Property(e => e.Name)
                    .HasColumnName("name")
                    .HasMaxLength(255)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Town>(entity =>
            {
                entity.HasKey(e => e.TownName);

                entity.ToTable("town");

                entity.Property(e => e.TownName)
                    .HasColumnName("townName")
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .ValueGeneratedNever();

                entity.Property(e => e.ProvinceCode)
                    .HasColumnName("provinceCode")
                    .HasMaxLength(2)
                    .IsUnicode(false);
            });
        }
    }
}
