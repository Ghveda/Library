using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace LibraryData.Models
{
    public class Holds
    {
        [Key]

        public int Id { get; set; }
        public virtual LibraryAsset LibraryAsset { get; set; }
        public virtual LibraryCard  LibraryCard{ get; set; }
        public DateTime HoldPlaced { get; set; }
    }
}
