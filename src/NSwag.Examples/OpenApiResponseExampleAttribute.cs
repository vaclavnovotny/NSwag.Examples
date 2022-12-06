namespace NSwag.Examples
{
    using System;

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class OpenApiResponseExampleAttribute : Attribute
    {
        public OpenApiResponseExampleAttribute(int statusCode, Type examplesProviderType)
        {
            StatusCode = statusCode;
            ExamplesProviderType = examplesProviderType;
        }

        public Type ExamplesProviderType { get; }

        public int StatusCode { get; }
    }
}
