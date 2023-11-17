using System.ComponentModel.DataAnnotations;

namespace Blog.ViewModels.Accounts;

public class RegisterViewModel
{
    [Required(ErrorMessage = "The name is mandatory")]
    public string Name { get; set; }

    [Required(ErrorMessage = "The email is mandatory")]
    [EmailAddress(ErrorMessage = "Email invalid")]
    public string Email { get; set; }
}