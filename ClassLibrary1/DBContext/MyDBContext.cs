using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace ClassLibrary1
{
    public partial class MyDBContext : DbContext
    {
        public MyDBContext()
        {
            
        }

        public MyDBContext(DbContextOptions<MyDBContext> options)
            : base(options)
        {
        }

        public virtual DbSet<AddressNotOwned> AddressNotOwned { get; set; }
        public virtual DbSet<ContactAddress> ContactAddress { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("ProductVersion", "2.2.2-servicing-10034");

            modelBuilder.Entity<AddressNotOwned>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Address1).IsRequired();

                entity.HasOne(d => d.IdNavigation)
                    .WithOne(p => p.AddressNotOwned)
                    .HasForeignKey<AddressNotOwned>(d => d.Id)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_AddressNotOwned");
            });
        }
    }
}
