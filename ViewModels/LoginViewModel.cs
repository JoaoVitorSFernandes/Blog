using System.ComponentModel.DataAnnotations;

namespace Blog.ViewModel;

public class LoginViewModel
{
    [Required(ErrorMessage = "The email is mandatory")]
    [EmailAddress(ErrorMessage = "Email invalid")]
    public string Email { get; set; }

    [Required(ErrorMessage = "The password is mandatory")]
    public string Password { get; set; }
}