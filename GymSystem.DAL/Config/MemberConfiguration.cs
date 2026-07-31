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
    public class MemberConfiguration : UserConfiguration<Member> ,IEntityTypeConfiguration<Member>

    {
        public new void Configure(EntityTypeBuilder<Member> builder)
        {
            base.Configure(builder);
            builder.Property(m => m.CreatedAt).HasColumnName("JoinDate").HasDefaultValueSql("GETDATE()");
        }
    }
}
