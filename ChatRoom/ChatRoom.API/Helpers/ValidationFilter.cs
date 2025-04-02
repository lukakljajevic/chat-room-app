using FluentValidation;

namespace ChatRoom.API.Helpers;

public class ValidationFilter<T>(IValidator<T> validator) : IEndpointFilter
    where T : class
{
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        if (context.Arguments.FirstOrDefault(x => x?.GetType() == typeof(T)) is not T argument)
        {
            return Results.BadRequest("Request object is null");
        }

        var validationResult = await validator.ValidateAsync(argument);
        
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors
                .GroupBy(e => e.PropertyName)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(e => e.ErrorMessage).ToArray()
                );
                
            return Results.ValidationProblem(errors);
        }
        
        return await next(context);
    }
}