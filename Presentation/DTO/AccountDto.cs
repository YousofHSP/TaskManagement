using Entity;
using System.ComponentModel.DataAnnotations;

namespace DTO;

public class RegisterDto
{
    [Required(ErrorMessage = "نام و نام خانوادگی اجباری است")]
    [MaxLength(255)]
    public string FullName { get; set; }
    [Required(ErrorMessage = "رمز عبور اجباری است")]
    [MinLength(4, ErrorMessage = "رمز عبور نمیتواند کم تر است ۴ حرف باشد")]
    [DataType(DataType.Password)]
    public string Password { get; set; }
    [Required(ErrorMessage = "شماره موبایل اجباری است")]
    [StringLength(11,ErrorMessage = "شماره موبایل معتبر نیست")]
    public string PhoneNumber { get; set; }
}

public class LoginDto
{
    [Display(Name = "شماره موبایل")]
    [Required(ErrorMessage = "شماره موبایل اجباری است")]
    public string PhoneNumber { get; set; }
    [Required(ErrorMessage = "رمز عبور اجباری است")]
    [Display(Name = "رمز")]
    public string Password { get; set; }
    public string ReturnUrl { get; set; }
}

