namespace NSwag.Examples;

public interface IExampleProvider<out T>
{
    T GetExample();
}