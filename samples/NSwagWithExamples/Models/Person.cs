using System;

namespace NSwagWithExamples.Models;

public class Person
{
    static Random _random = new Random(DateTime.Now.Microsecond);

    public string FirstName { get; set; }
    public int Id { get; internal set; }
    public string LastName { get; set; }
    public DateTime BirthDay { get; set; }

    public Person()
    { 
    }
    public Person(int id, string firstName, string lastName, DateTime? birthDay = null)
    {
        Id = id;
        FirstName = firstName;
        LastName = lastName;
        BirthDay = birthDay ?? new DateTime(_random.Next(1920, DateTime.Now.Year), _random.Next(1, 13), _random.Next(1, 29));
    }

    public Person(string firstName, string lastName, DateTime? birthDay = null)
    {
        FirstName = firstName;
        LastName = lastName;
        BirthDay = birthDay ?? new DateTime(_random.Next(1920, DateTime.Now.Year), _random.Next(1, 13), _random.Next(1, 29));
    }

    public int Age
    {  
        get
        {
            var today = DateTime.Today;
            var age = today.Year - BirthDay.Year;
            if (BirthDay.Date > today.AddYears(-age)) age--;
            return age;
        }
    }

}