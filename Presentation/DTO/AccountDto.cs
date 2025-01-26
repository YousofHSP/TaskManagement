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
    
    public int Year { get; set; }
    
    public int Month { get; set; }
    
    public int Day { get; set; }
    [Required(ErrorMessage = "پایه تحصیلی اجباری است")]
    public Grade Grade { get; set; }
    [Required(ErrorMessage = "رشته تحصیلی اجباری است")]
    public FieldOfStudy FieldOfStudy { get; set; }

}

public class LoginDto
{
    [Required(ErrorMessage = "شماره موبایل اجباری است")]
    public string PhoneNumber { get; set; }
    [Required(ErrorMessage = "رمز عبور اجباری است")]
    public string Password { get; set; }
    public string ReturnUrl { get; set; }
}

