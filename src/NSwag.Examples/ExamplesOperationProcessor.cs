namespace NSwag.Examples
{
    using Microsoft.Extensions.DependencyInjection;
    using NSwag;
    using NSwag.Generation.Processors;
    using NSwag.Generation.Processors.Contexts;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    public class ExamplesOperationProcessor : IOperationProcessor
    {
        private const string mediaTypeName = "application/json";

        private readonly ExamplesConverter examplesConverter;
        private readonly IServiceProvider serviceProvider;

        public ExamplesOperationProcessor(
            ExamplesConverter examplesConverter,
            IServiceProvider serviceProvider)
        {
            this.examplesConverter = examplesConverter;
            this.serviceProvider = serviceProvider;
        }

        public bool Process(OperationProcessorContext context)
        {
            this.SetRequestExamples(context);
            this.SetResponseExamples(context);

            return true;
        }

        private void SetRequestExamples(OperationProcessorContext context)
        {
            var attributes = context.MethodInfo.GetCustomAttributes<OpenApiRequestExampleAttribute>();
            foreach (var apiParameter in context.OperationDescription.Operation.Parameters)
            {
                var parameter = context.Parameters.SingleOrDefault(x => x.Value.Name == apiParameter.Name);
                if (parameter.Key == null)
                {
                    continue;
                }

                var parameterType = parameter.Key.ParameterType;
                var attribute = attributes.SingleOrDefault(x => x.RequestType == parameterType);
                if (attribute == null)
                {
                    continue;
                }

                var examplesProviderType = attribute.ExamplesProviderType;

                var mediaType = context.OperationDescription.Operation.RequestBody.Content[mediaTypeName];

                if (IsInstanceOfGenericInterfaceType(typeof(IOpenApiExampleProvider<>), examplesProviderType))
                {
                    mediaType.Example = this.GetExample(examplesProviderType);
                }

                if (IsInstanceOfGenericInterfaceType(typeof(IOpenApiExamplesProvider<>), examplesProviderType))
                {
                    var examples = this.GetExamples(examplesProviderType);
                    foreach (var example in examples)
                    {
                        mediaType.Examples.Add(example);
                    }
                }
            }
        }

        private void SetResponseExamples(OperationProcessorContext context)
        {
            var attributes = context.MethodInfo.GetCustomAttributes<OpenApiResponseExampleAttribute>();
            foreach (var apiResponse in context.OperationDescription.Operation.Responses)
            {
                if (!int.TryParse(apiResponse.Key, out int responseStatusCode))
                {
                    continue;
                }

                var attribute = attributes.SingleOrDefault(x => x.StatusCode == responseStatusCode);
                if (attribute == null)
                {
                    continue;
                }

                var examplesProviderType = attribute.ExamplesProviderType;
                var mediaType = apiResponse.Value.Content[mediaTypeName];

                if (IsInstanceOfGenericInterfaceType(typeof(IOpenApiExampleProvider<>), examplesProviderType))
                {
                    mediaType.Example = this.GetExample(examplesProviderType);
                }

                if (IsInstanceOfGenericInterfaceType(typeof(IOpenApiExamplesProvider<>), examplesProviderType))
                {
                    var examples = this.GetExamples(examplesProviderType);
                    foreach (var example in examples)
                    {
                        mediaType.Examples.Add(example);
                    }
                }
            }
        }

        private object GetExample(Type exampleProviderType)
        {
            var example = this.GetExampleFromExampleProvider(exampleProviderType);
            return this.examplesConverter.SerializeExampleJson(example);
        }

        private IDictionary<string, OpenApiExample> GetExamples(Type examplesProviderType)
        {
            var examples = this.GetExamplesFromExamplesProvider(examplesProviderType);
            var multipleExamples = examples as IEnumerable<IOpenApiExample<object>>;
            return this.examplesConverter.ToOpenApiExamplesDictionary(multipleExamples);
        }

        private object GetExampleFromExampleProvider(Type examplesProviderType)
        {
            return this.GetExamplesFromExampleProvider(
                examplesProviderType,
                "GetExample");
        }

        private object GetExamplesFromExamplesProvider(Type examplesProviderType)
        {
            return this.GetExamplesFromExampleProvider(
                examplesProviderType,
                "GetExamples");
        }

        private object GetExamplesFromExampleProvider(
            Type examplesProviderType,
            string examplesProviderTypeMethodName)
        {
            var examplesProvider = ActivatorUtilities.CreateInstance(this.serviceProvider, examplesProviderType);

            var exampleProviderGetExampleMethodInfo = examplesProviderType.GetMethod(examplesProviderTypeMethodName);
            return exampleProviderGetExampleMethodInfo.Invoke(examplesProvider, null);
        }

        private static bool IsInstanceOfGenericInterfaceType(Type genericInterfaceType, Type type)
            => type.GetInterfaces().Any(x => x.IsGenericType && x.GetGenericTypeDefinition() == genericInterfaceType);
    }
}
