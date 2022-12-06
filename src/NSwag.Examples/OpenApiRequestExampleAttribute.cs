namespace NSwag.Examples
{
    using System;

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class OpenApiRequestExampleAttribute : Attribute
    {
        public OpenApiRequestExampleAttribute(Type requestType, Type examplesProviderType)
        {
            RequestType = requestType;
            ExamplesProviderType = examplesProviderType;
        }

        public Type ExamplesProviderType { get; }

        public Type RequestType { get; }
    }
}
