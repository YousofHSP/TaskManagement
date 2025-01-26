using System.ComponentModel.DataAnnotations;
using System.Globalization;
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

    [Display(Name = "سال")]
    [Required(ErrorMessage = "سال تولد اجباری است")]
    [DataType(DataType.PhoneNumber)]
    public int? Year { get; set; }

    [Display(Name = "ماه")]
    [Required(ErrorMessage = "ماه تولد اجباری است")]
    [DataType(DataType.PhoneNumber)]
    public int? Month { get; set; }

    [Display(Name = "روز")]
    [Required(ErrorMessage = "روز تولد اجباری است")]
    [DataType(DataType.PhoneNumber)]
    public int? Day { get; set; }

    [Required(ErrorMessage = "پایه تحصیلی اجباری است")]
    [Display(Name = "پایه تحصیلی")]
    public Grade Grade { get; set; }

    [Required(ErrorMessage = "رشته تحصیلی اجباری است")]
    [Display(Name = "رشته تحصیلی")]
    public FieldOfStudy FieldOfStudy { get; set; }

    [Display(Name = "وضعیت")]
    public UserStatus Status { get; set; }
    [DataType(DataType.Password)]
    [Display(Name = "رمز عبور")]
    public string? Password { get; set; } = string.Empty;

    [Display(Name = "نقش")]
    [Required(ErrorMessage= "نقش اجباری است")]
    public string RoleName { get; set; } = string.Empty;
    public List<SelectListItem>? Roles { get; set; }
    protected override void CustomMappings(IMappingExpression<User, UserDto> mapping)
    {
        mapping.ForMember(
            dest => dest.Year,
            config => config.MapFrom(src => (new PersianCalendar()).GetYear(src.BirthDate.DateTime)));
        mapping.ForMember(
            dest => dest.Month,
            config => config.MapFrom(src => (new PersianCalendar()).GetMonth(src.BirthDate.DateTime)));
        mapping.ForMember(
            dest => dest.Day,
            config => config.MapFrom(src => (new PersianCalendar()).GetDayOfMonth(src.BirthDate.DateTime)));
        base.CustomMappings(mapping);
    }
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