using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LibraryDomain.Model;

public partial class Author: Entity
{
    [Display(Name = "Ім'я")]
    [Required(ErrorMessage = "Поле не повинно бути  порожнім")]
    [StringLength(50, ErrorMessage = "Ім'я не має перевищувати 50 символів")]
    [RegularExpression(@"^[^\d]*$", ErrorMessage = "Ім'я не повинно містити цифр")]
    public string Name { get; set; } = null!;

    [Display(Name = "Прізвище")]
    [Required(ErrorMessage = "Поле не повинно бути  порожнім")]
    [StringLength(50, ErrorMessage = "Прізвище не має перевищувати 50 символів")]
    [RegularExpression(@"^[^\d]*$", ErrorMessage = "Прізвище не повинно містити цифр")]
    public string Surname { get; set; } = null!;

    public virtual ICollection<Book> Books { get; set; } = new List<Book>();
}
