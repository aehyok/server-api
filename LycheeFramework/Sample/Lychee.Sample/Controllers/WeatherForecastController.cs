using Lychee.EntityFramework;
using Lychee.Sample.Domain;
using Lychee.TypeFinder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Lychee.Sample.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IEnumerable<WeatherForecast> Get([FromServices] DbContext dbContext)
        {
            var mappingConfigurations = FindTypes.InAllAssemblies
                .ThatInherit(typeof(IMappingConfiguration))
                .Excluding(typeof(IMappingConfiguration<>))
                .Where(a => !a.IsAbstract)
                .ToList();

            dbContext.Set<User>().Add(new Domain.User { Id = 1, IsDeleted = false });

            dbContext.SaveChanges();

            var rng = new Random();
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            })
            .ToArray();
        }
    }

    public interface I
    { }

    public interface IA<T> : I
    { }

    public interface IB<T> : IA<T>
    { }

    public class C
    { }

    public abstract class A : IB<C>
    { }

    public class B : A
    { }
}