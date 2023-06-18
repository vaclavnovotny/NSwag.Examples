using System;
using System.Collections.Generic;
using System.Linq;

namespace NSwag.Examples;

internal class SwaggerExampleTypeMapper
{
    private readonly List<KeyValuePair<Type, Type>> _mapper = new List<KeyValuePair<Type, Type>>();

    internal void Add(Type valueType, Type providerType) {
        _mapper.Add(new KeyValuePair<Type, Type>(valueType, providerType));
    }

    internal IEnumerable<Type> GetProviderTypes(Type valueType) {
        return _mapper.Where(x => x.Key == valueType).Select(x => x.Value);
    }
}