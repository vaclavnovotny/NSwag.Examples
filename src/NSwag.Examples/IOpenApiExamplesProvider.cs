namespace NSwag.Examples
{
    using System.Collections.Generic;

    public interface IOpenApiExamplesProvider<T>
    {
        IEnumerable<OpenApiExample<T>> GetExamples();
    }
}
