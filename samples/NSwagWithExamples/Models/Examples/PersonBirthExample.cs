using System;
using NSwag.Examples;

namespace NSwagWithExamples.Models.Examples;

public class PersonBirthExample : IExampleProvider<DateTime>
{
    public DateTime GetExample() => DateTime.UtcNow;
}