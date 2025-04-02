using System.ComponentModel;
using ChatRoom.API.Common;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using DateTimeConverter = ChatRoom.API.Helpers.DateTimeConverter;

namespace ChatRoom.API.DTO;

public record AggregatedEventsQueryParameters
{
    [FromQuery(Name = "granularity")]
    public string? Granularity { get; set; }
    
    [FromQuery(Name = "startDate")]
    [TypeConverter(typeof(DateTimeConverter))]
    public DateTime? StartDate { get; set; }
    
    [FromQuery(Name = "endDate")]
    [TypeConverter(typeof(DateTimeConverter))]
    public DateTime? EndDate { get; set; }

    public GranularityLevel GetGranularityLevel()
    {
        return Granularity?.ToLower() switch
        {
            "minute" => GranularityLevel.Minute,
            "hour" => GranularityLevel.Hour,
            "day" => GranularityLevel.Day,
            "month" => GranularityLevel.Month,
            _ => GranularityLevel.Hour,
        };
    }
}

public class AggregatedEventsQueryParametersValidator : AbstractValidator<AggregatedEventsQueryParameters>
{
    private readonly string[] _validGranularityLevels = ["minute", "hour", "day", "month"];
    
    public AggregatedEventsQueryParametersValidator()
    {
        When(x => x.StartDate.HasValue && x.EndDate.HasValue, () => {
            RuleFor(x => x.StartDate)
                .LessThanOrEqualTo(x => x.EndDate)
                .WithMessage("Start date must be less than or equal to end date");
        });
        
        When(x => !string.IsNullOrEmpty(x.Granularity), () => {
            RuleFor(x => x.Granularity)
                .Must(g => _validGranularityLevels.Contains(g!.ToLower()))
                .WithMessage($"Granularity must be one of: {string.Join(", ", _validGranularityLevels)}");
        });
    }
}