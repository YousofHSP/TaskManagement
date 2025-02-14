using System.ComponentModel.DataAnnotations;
using AutoMapper;
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
}

public class UserResDto : BaseDto<UserResDto, User>
{
    [Display(Name = "نام")] public string FullName { get; set; } = null!;

    [Display(Name = "شماره موبایل")]
    public string PhoneNumber { get; set; } = null!;
    [Display(Name = "نقش")]
    public string RoleName { get; set; } = null!;
    [Display(Name = "وضیعت")] public UserStatus Status { get; set; }

    protected override void CustomMappings(IMappingExpression<User, UserResDto> mapping)
    {
        mapping.ForMember(
            d => d.RoleName,
            s => s.MapFrom(m => m.Roles.FirstOrDefault()!.Name ?? "")
            );
    }
}