using NSwag.Examples;

namespace NSwagWithExamples.Models.Examples;

public class CustomInternalErrorOnMethodLevelExample : IExampleProvider<CustomInternalErrorOnMethodLevel>
{
    public CustomInternalErrorOnMethodLevel GetExample()
    {
        return new CustomInternalErrorOnMethodLevel { Reason = "Some error occurred", Severity = 50, AdditionalErrorInfo = "This person is DEAD :(" };
    }
}