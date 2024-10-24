using System.Reflection;
using RestApia.Shared.Common.Enums;
using RestApia.Shared.Common.Models;
namespace RestApia.Shared.Common.Services;

public static class VariablesConverter
{
    /// <summary>
    /// Convert object public properties to list of extensions variables.
    /// </summary>
    public static IReadOnlyCollection<ValueModel> Serialize(object data) => data
        .GetType()
        .GetProperties(BindingFlags.Public | BindingFlags.Instance)
        .Select(x => new ValueModel
        {
            Name = x.Name,
            Type = ValueTypeEnum.Variable,
            Value = x.GetValue(data)?.ToString() ?? string.Empty,
        })
        .ToList();

    /// <summary>
    /// Create a new object and set public properties from variables by names.
    /// </summary>
    public static BoolResult TryDeserialize<T>(IReadOnlyCollection<ValueModel> values, out T result)
    {
        result = Activator.CreateInstance<T>();
        var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

        var variables = values
            .Where(x => x.Type == ValueTypeEnum.Variable)
            .Join(properties, value => value.Name, property => property.Name, (v, p) => new
            {
                Value = v,
                Property = p,
            });

        var errors = new List<string>();
        foreach (var variable in variables)
        {
            try
            {
                var value = Convert.ChangeType(variable.Value.Value.ToString(), variable.Property.PropertyType);
                variable.Property.SetValue(result, value);
            }
            catch
            {
                errors.Add($"Cannot set value '{variable.Value.Value}' to property '{variable.Property.Name}' of type '{variable.Property.PropertyType.Name}'");
            }
        }

        return errors.Count == 0 ? BoolResult.True : BoolResult.FalseError(errors.JoinString("\r\n"));
    }
}
