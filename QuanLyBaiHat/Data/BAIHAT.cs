namespace QuanLyBaiHat.Data
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("BAIHAT")]
    public partial class BAIHAT
    {
        public int ID { get; set; }

        public string TEN { get; set; }

        public int? CASIID { get; set; }

        public int? NAMPHATHANH { get; set; }

        public virtual CASI CASI { get; set; }
    }
}
