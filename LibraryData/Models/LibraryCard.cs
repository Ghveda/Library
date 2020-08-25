using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace LibraryData.Models
{
    public class LibraryCard
    {
        [Key]
        public int Id { get; set; }
        public decimal Fees { get; set; }
        public DateTime Created { get; set; }
        public virtual IEnumerable<Checkout> Checkouts { get; set; }
    }
}
