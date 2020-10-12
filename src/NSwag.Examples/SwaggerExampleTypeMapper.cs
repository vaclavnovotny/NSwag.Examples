using System;
using System.Collections.Generic;

namespace NSwag.Examples
{
    internal class SwaggerExampleTypeMapper
    {
        private readonly Dictionary<Type, Type> _mapper = new Dictionary<Type, Type>();
        
        internal void Add(Type valueType, Type providerType)
        {
            _mapper.Add(valueType, providerType);
        }

        internal bool TryGetProviderType(Type valueType, out Type providerType)
        {
            return _mapper.TryGetValue(valueType, out providerType);
        }
    }
}