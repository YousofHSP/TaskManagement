using System.ComponentModel.DataAnnotations;
using DTO;
using Entity;
using Presentation.Attributes;
using Presentation.Models;

namespace Presentation.DTO;


public class CustomerDto : BaseDto<CustomerDto, Customer>
{
    [Display(Name = "عنوان")]
    [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
    [Field(FieldType.Text)]
    public string Title { get; set; }
    
    [Display(Name = "والد")]
    [Field(FieldType.Select)]
    public int? ParentId { get; set; }
}
public class CustomerResDto: BaseDto<CustomerResDto, Customer>
{
    [Display(Name = "عنوان")]
    public string Title { get; set; }
    [Display(Name = "والد")]
    public string ParentTitle { get; set; }
    
}