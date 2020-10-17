using System;

namespace NSwag.Examples
{
    internal class SwaggerExampleProvider
    {
        private readonly SwaggerExampleTypeMapper _mapper;
        private readonly IServiceProvider _serviceProvider;

        internal SwaggerExampleProvider(SwaggerExampleTypeMapper mapper, IServiceProvider serviceProvider)
        {
            _mapper = mapper;
            _serviceProvider = serviceProvider;
        }

        internal object GetProviderValue(Type valueType)
        {
            if (valueType == null || !_mapper.TryGetProviderType(valueType, out var providerType))
                return null;

            var providerService = _serviceProvider.GetService(providerType);
            return providerService == null ? null : ((dynamic)providerService).GetExample();
        }
    }
}