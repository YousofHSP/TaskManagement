using System.ComponentModel.DataAnnotations;
using DTO;
using Entity;
using Presentation.Attributes;
using Presentation.Models;

namespace Presentation.DTO;

public class EventDto: BaseDto<EventDto, Event>
{
    [Display(Name = "عنوان")]
    [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
    [Field(FieldType.Text)]
    public string Title { get; set; }

    [Display(Name = "توضیحات")]
    [Field(FieldType.Text)]
    public string? Description { get; set; }

}
