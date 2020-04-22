using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Autofac;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace MAVN.Service.CustomerAPI.Tests
{
    public class OperationsIdsTest
    {
        [Fact]
        public void CheckOperationsIdsTest()
        {
            var hostProjectAssembly = typeof(Startup).Assembly;

            var controllerTypes = hostProjectAssembly.GetExportedTypes()
                .Where(t => t.IsAssignableTo<ControllerBase>());

            var endpointsMapping = new Dictionary<string, List<string>>();

            foreach (var controllerType in controllerTypes)
            {
                var controllerMethods = controllerType.GetMethods(
                    BindingFlags.Public | BindingFlags.InvokeMethod | BindingFlags.Instance | BindingFlags.DeclaredOnly);

                foreach (var controllerMethod in controllerMethods)
                {
                    var methodName = controllerMethod.Name;
                    if (endpointsMapping.ContainsKey(methodName))
                        endpointsMapping[methodName].Add(controllerType.Name);
                    else
                        endpointsMapping.Add(methodName, new List<string> { controllerType.Name });
                }
            }

            var duplicatedEndpoints = endpointsMapping
                .Where(i => i.Value.Count > 1)
                .Select(i => $"Method: {i.Key}, Controllers: {string.Join(", ", i.Value)}")
                .ToList();

            Assert.True(
                duplicatedEndpoints.Count == 0,
                "These endpoints have non-unique names: " + string.Join("; ", duplicatedEndpoints));
        }
    }
}
