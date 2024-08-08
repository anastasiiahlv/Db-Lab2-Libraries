using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryDomain.Model
{
    public partial class Gender : Entity
    {
        [Display(Name = "Стать")]
        public string Name { get; set; } = null!;

        public virtual ICollection<Director> Directors { get; set; } = new List<Director>();
    }
}
