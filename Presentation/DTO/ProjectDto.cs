using System.ComponentModel.DataAnnotations;
using DTO;
using Entity;
using Presentation.Attributes;
using Presentation.Models;

namespace Presentation.DTO;

public class ProjectDto: BaseDto<ProjectDto, Project>
{
    [Display(Name = "عنوان")]
    [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
    [Field(FieldType.Text)]
    public string Title { get; set; }

    [Display(Name = "مشتری")]
    [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
    [Field(FieldType.Select)]
    public int CustomerId { get; set; }
}

public class ProjectResDto : BaseDto<ProjectResDto, Project>
{
    [Display(Name = "عنوان")]
    public string Title { get; set; }

    [Display(Name = "مشتری")]
    public string CustomerTitle { get; set; }
}