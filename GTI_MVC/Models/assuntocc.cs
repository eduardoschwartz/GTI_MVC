using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GTI_MVC.Models {
    public class Assuntocc {
        [Key]
        [Column(Order = 1)]
        [Index("AssuntoLocal_Index", IsUnique = true, Order = 1)]
        public short Codassunto { get; set; }
        [Key]
        [Column(Order = 2)]
        [Index("AssuntoLocal_Index", IsUnique = true, Order = 2)]
        public short Seq { get; set; }
        public short Codcc { get; set; }
    }
}
