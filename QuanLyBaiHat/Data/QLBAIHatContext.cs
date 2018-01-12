namespace QuanLyBaiHat.Data
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class QLBAIHatContext : DbContext
    {
        public QLBAIHatContext()
            : base("name=QLBAIHatContext")
        {
        }

        public virtual DbSet<BAIHAT> BAIHATs { get; set; }
        public virtual DbSet<CASI> CASIs { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
        }
    }
}
