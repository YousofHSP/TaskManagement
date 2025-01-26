using System.ComponentModel.DataAnnotations;
using Entity;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DTO;

public class UserDto : BaseDto<UserDto, User>
{
    [Required(ErrorMessage = "نام و نام خانوادگی اجباری است")]
    [MaxLength(255)]
    [Display(Name = "نام و نام خانوادگی")]
    public string FullName { get; set; }

    [Required(ErrorMessage = "شماره موبایل اجباری است")]
    [StringLength(11, ErrorMessage = "شماره موبایل معتبر نیست")]
    [Display(Name = "شماره موبایل")]
    public string PhoneNumber { get; set; }

    [Display(Name = "وضعیت")]
    public UserStatus Status { get; set; }
    [DataType(DataType.Password)]
    [Display(Name = "رمز عبور")]
    public string? Password { get; set; } = string.Empty;

    [Display(Name = "نقش")]
    [Required(ErrorMessage= "نقش اجباری است")]
    public string RoleName { get; set; } = string.Empty;
    public List<SelectListItem>? Roles { get; set; }
}

public class UserCreateDto : UserDto
{
    [DataType(DataType.Password)]
    [Display(Name = "رمز عبور")]
    [Required(ErrorMessage = "رمز عبور اجباری است")]
    public string Password { get; set; }
}

public class UserListDto : BaseDto<UserListDto, User>
{
    [Display(Name = "نام")] public string FullName { get; set; } = null!;

    [Display(Name = "شماره موبایل")] public string PhoneNumber { get; set; } = null!;
    [Display(Name = "وضیعت")] public UserStatus Status { get; set; }
}

public class UserDeleteDto : BaseDto<UserDeleteDto, User>
{
    [Display(Name = "نام")] public string FullName { get; set; } = null!;
    [Display(Name = "شماره موبایل")]public string PhoneNumber { get; set; } = null!;

}

public class AddUserRoleDto
{
    public string? FullName { get; set; }
    public string? PhoneNumber { get; set; }
    public int Id { get; set; }
    public string Role { get; set; }
    public List<SelectListItem> Roles { get; set; }
}