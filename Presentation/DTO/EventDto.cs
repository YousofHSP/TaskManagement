using System.ComponentModel.DataAnnotations;
using DTO;
using Entity;

namespace Presentation.DTO;

public class EventDto: BaseDto<EventDto, Event>
{
    [Display(Name = "عنوان")]
    [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
    public string Title { get; set; }

    [Display(Name = "توضیحات")] public string? Description { get; set; }

}
