using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Autofac;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace MAVN.Service.CustomerAPI.Tests
{
    public class SensitiveDataTests
    {
        private readonly Type _refitGetAttrType = typeof(HttpGetAttribute);

        private readonly List<string> _sensitiveParamsNames = new List<string>
        {
            "fullname", "email", "phone"
        };

        [Fact]
        public void CheckQueryParamsTest()
        {
            var hostProjectAssembly = typeof(Startup).Assembly;

            var controllerTypes = hostProjectAssembly.GetExportedTypes()
                .Where(t => t.IsAssignableTo<ControllerBase>());

            var sensitiveDataParams = new List<string>();

            foreach (var controllerType in controllerTypes)
            {
                var controllerMethods = controllerType.GetMethods(
                    BindingFlags.Public | BindingFlags.InvokeMethod | BindingFlags.Instance | BindingFlags.DeclaredOnly);

                foreach (var controllerMethod in controllerMethods)
                {
                    var refitGetAttr = controllerMethod.CustomAttributes.FirstOrDefault(a => _refitGetAttrType == a.AttributeType);
                    if (refitGetAttr == null)
                        continue;

                    if (controllerMethod.CustomAttributes.Any(a => a.AttributeType == typeof(ObsoleteAttribute)))
                        continue;

                    var methodParams = controllerMethod.GetParameters();
                    var paramsWithSensitiveData = methodParams.Where(p => _sensitiveParamsNames.Any(s => p.Name.ToLower().Contains(s)));

                    sensitiveDataParams.AddRange(
                        paramsWithSensitiveData.Select(i => $"{i.Name} from {controllerType.Name}.{controllerMethod.Name}"));

                    foreach (var methodParam in methodParams)
                    {
                        if (!methodParam.ParameterType.Assembly.FullName.StartsWith("Lykke.Service.AdminApi")
                            || !methodParam.ParameterType.IsClass)
                            continue;

                        var paraAttrs = methodParam.CustomAttributes;
                        if (paraAttrs.Any(p => p.AttributeType == typeof(FromQueryAttribute)))
                        {
                            var paramFields = methodParam.ParameterType.GetProperties();
                            var propertiesWithSensitiveData = paramFields.Where(p => _sensitiveParamsNames.Any(s => p.Name.ToLower().Contains(s)));
                            sensitiveDataParams.AddRange(
                                propertiesWithSensitiveData.Select(i => $"{methodParam.Name}.{i.Name} from {controllerType.Name}.{controllerMethod.Name}"));
                        }
                    }
                }
            }

            Assert.True(
                sensitiveDataParams.Count == 0,
                "These parameters might lead to passing sensitive data when building url via refit: " + string.Join(", ", sensitiveDataParams));
        }
    }
}
