namespace NSwag.Examples
{
    public class OpenApiExample<T> : IOpenApiExample<T>
    {
        public OpenApiExample(string name, T value)
        {
            this.Name = name;
            this.Value = value;
        }

        public string Name { get; set; }

        public T Value { get; set; }
    }
}
