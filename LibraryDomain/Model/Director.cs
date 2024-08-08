using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LibraryDomain.Model;

public partial class Director: Entity
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

    [Display(Name = "Номер телефону")]
    [Required(ErrorMessage = "Поле не повинно бути  порожнім")]
    [RegularExpression(@"^\+380\d{9}$", ErrorMessage = "Потрібний формат номеру телефону: +380XXXXXXXXX")]
    public string PhoneNumber { get; set; } = null!;

    [Display(Name = "Адреса електронної пошти")]
    [Required(ErrorMessage = "Поле не повинно бути  порожнім")]
    [RegularExpression(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$", ErrorMessage = "Потрібний формат email: example@example.com")]
    public string Email { get; set; } = null!;

    [Display(Name = "Стать")]
    public int GenderId { get; set; }

    public virtual Gender Gender { get; set; } = null!;

    public virtual ICollection<Library> Libraries { get; set; } = new List<Library>();
}
