using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LibraryDomain.Model;

public partial class Book: Entity
{
    [Display(Name = "Назва")]
    [Required(ErrorMessage = "Поле не повинно бути  порожнім")]
    [StringLength(255, ErrorMessage = "Назва не має перевищувати 255 символів")]
    public string Title { get; set; } = null!;

    [Display(Name = "Опис")]
    [Required(ErrorMessage = "Поле не повинно бути  порожнім")]
    public string Description { get; set; } = null!;

    [Display(Name = "Видавництво")]
    public int PublisherId { get; set; }
    [Display(Name = "Видавництво")]
    public virtual Publisher Publisher { get; set; } = null!;

    public virtual ICollection<Author> Authors { get; set; } = new List<Author>();

    public virtual ICollection<Library> Libraries { get; set; } = new List<Library>();
}
