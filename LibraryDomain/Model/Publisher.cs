using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LibraryDomain.Model;

public partial class Publisher: Entity
{
    [Display(Name = "Назва")]
    [Required(ErrorMessage = "Поле не повинно бути  порожнім")]
    [StringLength(50, ErrorMessage = "Назва не має перевищувати 50 символів")]
    public string Name { get; set; } = null!;

    [Display(Name = "Про видавництво")]
    [Required(ErrorMessage = "Поле не повинно бути  порожнім")]
    public string Info { get; set; } = null!;

    public virtual ICollection<Book> Books { get; set; } = new List<Book>();
}
