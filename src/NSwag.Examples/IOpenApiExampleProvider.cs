namespace NSwag.Examples
{
    public interface IOpenApiExampleProvider<T>
    {
        T GetExample();
    }
}
