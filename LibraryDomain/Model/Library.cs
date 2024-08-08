using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LibraryDomain.Model;

public partial class Library: Entity
{
    [Display(Name = "Назва")]
    [Required(ErrorMessage = "Поле не повинно бути  порожнім")]
    [StringLength(50, ErrorMessage = "Назва не має перевищувати 50 символів")]
    public string Name { get; set; } = null!;

    [Display(Name = "Адреса")]
    [Required(ErrorMessage = "Поле не повинно бути  порожнім")]
    [StringLength(50, ErrorMessage = "Адреса не має перевищувати 50 символів")]
    public string Address { get; set; } = null!;

    [Display(Name = "Тип бібліотеки")]
    public int TypeId { get; set; }

    [Display(Name = "Директор")]
    public int? DirectorId { get; set; }

    [Display(Name = "Директор")]
    public virtual Director? Director { get; set; }

    [Display(Name = "Тип бібліотеки")]
    public virtual Type Type { get; set; } = null!;

    public virtual ICollection<Book> Books { get; set; } = new List<Book>();
}
