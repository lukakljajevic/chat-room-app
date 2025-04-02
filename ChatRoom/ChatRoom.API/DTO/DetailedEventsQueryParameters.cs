using System.ComponentModel;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace ChatRoom.API.DTO;

public record DetailedEventsQueryParameters
{
    [FromQuery(Name = "startDate")]
    [TypeConverter(typeof(DateTimeConverter))]
    public DateTime? StartDate { get; set; }
    
    [FromQuery(Name = "endDate")]
    [TypeConverter(typeof(DateTimeConverter))]
    public DateTime? EndDate { get; set; }
}

public class DetailedEventsQueryParametersValidator : AbstractValidator<DetailedEventsQueryParameters>
{
    public DetailedEventsQueryParametersValidator()
    {
        When(x => x.StartDate.HasValue && x.EndDate.HasValue, () => {
            RuleFor(x => x.StartDate)
                .LessThanOrEqualTo(x => x.EndDate)
                .WithMessage("Start date must be less than or equal to end date");
        });
    }
}