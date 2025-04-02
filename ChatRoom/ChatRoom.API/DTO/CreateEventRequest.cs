using FluentValidation;

namespace ChatRoom.API.DTO;

public record CreateEventRequest
{
    public required string EventType { get; set; }
    public required string Username { get; set; }
    public string? CommentText { get; set; }
    public string? Recipient { get; set; }
}

public class CreateEventRequestValidator : AbstractValidator<CreateEventRequest>
{
    public CreateEventRequestValidator()
    {
        RuleFor(x => x.EventType)
            .NotEmpty().WithMessage("Event type is required.")
            .Must(BeValidEventType).WithMessage("Event type must be one of: EnterRoom, LeaveRoom, Comment, HighFive.");

        RuleFor(x => x.Username)
            .NotEmpty().WithMessage("Username is required.");
        
        When(x => x.EventType.Equals("comment", StringComparison.CurrentCultureIgnoreCase), () => {
            RuleFor(x => x.CommentText)
                .NotEmpty().WithMessage("Comment text is required for comment events.");
        });

        When(x => x.EventType.Equals("highfive", StringComparison.CurrentCultureIgnoreCase), () => {
            RuleFor(x => x.Recipient)
                .NotEmpty().WithMessage("Recipient username is required for high-five events.");
        });
    }

    private bool BeValidEventType(string eventType)
    {
        var validTypes = new[] { "enterroom", "leaveroom", "comment", "highfive" };
        return validTypes.Contains(eventType.ToLower());
    }
}