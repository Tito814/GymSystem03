using GymSystem.DAL.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymSystem.DAL.Config
{
    public class UserConfiguration<T> : IEntityTypeConfiguration<T> where T : User
    {
        public void Configure(EntityTypeBuilder<T> builder)
        {
            builder.Property(u => u.Name)
                .IsRequired().HasColumnType("varchar(50)");

            builder.Property(u => u.Email)
                .IsRequired().HasColumnType("varchar(100)");
            builder.HasIndex(u => u.Email).IsUnique();

            builder.Property(u => u.Phone)
                .IsRequired().HasColumnType("varchar(15)");
            builder.HasIndex(u => u.Phone).IsUnique();

            builder.OwnsOne(u => u.address);

            builder.ToTable(tb => { 
                tb.HasCheckConstraint("Email_Check","Email like '_%@_%._%'");
                tb.HasCheckConstraint("Phone_Check","Phone like '010%' or Phone like '011%' or Phone like '012%' or Phone like '015%' and len(Phone) >= 10 and len(Phone) <= 15");
            });

        }
    }
}
