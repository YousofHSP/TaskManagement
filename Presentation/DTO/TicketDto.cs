using AutoMapper;
using DTO;
using Entity;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace Presentation.DTO
{
    public class TicketDto : BaseDto<TicketDto, Ticket>
    {
        [Display(Name = "کاربر")]
        public User? User { get; set; }

        [Required(ErrorMessage = "عنوان اجباری است")]
        [Display(Name = "عنوان")]
        public string Title { get; set; } = null!;

        [Required(ErrorMessage = "متن تیکت اجباری است")]
        [Display(Name = "متن تیکت")]
        public string Text { get; set; } = null!;

        [Display(Name = "اولیت")]
        public TicketPriority Priority { get; set; }

        public int? ParentId { get; set; }
        [Display(Name = "تاریخ ایجاد")]
        public string? Date { get; set; } = string.Empty;
        public IEnumerable<TicketDto>? Children { get; set; }
        protected override void CustomMappings(IMappingExpression<Ticket, TicketDto> mapping)
        {
            mapping.ForMember(
            dest => dest.Date,
            config => config.MapFrom(src => (new PersianCalendar()).GetYear(src.CreatedAt.DateTime) + "/" +
            (new PersianCalendar()).GetMonth(src.CreatedAt.DateTime) + "/" +
            (new PersianCalendar()).GetDayOfMonth(src.CreatedAt.DateTime) + " " +
            (new PersianCalendar()).GetHour(src.CreatedAt.DateTime) + ":" +
            (new PersianCalendar()).GetMinute(src.CreatedAt.DateTime)));
            base.CustomMappings(mapping);
        }
    }
}
