using Entity;
using System.ComponentModel.DataAnnotations;

namespace DTO
{
    public class RoleDto: BaseDto<RoleDto, Role>
    {
        [Required(ErrorMessage = "نام اجباری است")]
        [Display(Name = "نام")]
        public string Name { get; set; } = null!;
        [Display(Name = "توضیحات")]
        public string? Description { get; set; }
        [Display(Name = "دسترسی ها")] public List<string> Permissions { get; set; }
    }
}
