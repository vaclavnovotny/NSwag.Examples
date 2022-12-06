namespace NSwag.Examples
{
    public interface IOpenApiExample<out T>
    {
        string Name { get; }

        T Value { get; }
    }
}
