using System.ComponentModel.DataAnnotations;
using DTO;
using Entity;

namespace Presentation.DTO;


public class CustomerDto : BaseDto<CustomerDto, Customer>
{
    [Display(Name = "عنوان")]
    [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
    public string Title { get; set; }
    
    [Display(Name = "پلن")]
    [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
    public int PlanId { get; set; }
    
    [Display(Name = "والد")]
    public int? ParentId { get; set; }
}
public class CustomerResDto: BaseDto<CustomerResDto, Customer>
{
    [Display(Name = "عنوان")]
    public string Title { get; set; }
    [Display(Name = "والد")]
    public string ParentTitle { get; set; }
    [Display(Name = "پلن")]
    public string PlanTitle { get; set; }
    
}