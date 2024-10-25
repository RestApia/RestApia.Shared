using RestApia.Shared.Common.Models;
using RestApia.Shared.Extensions.ValuesProviderService.Models;
namespace RestApia.Shared.Common.Services;

public static class ValuesProviderSettingsValidator
{
    /// <summary>
    /// Validate input values if the meet reserved values requirements.
    /// </summary>
    /// <param name="settings">Values provider settings.</param>
    /// <param name="inputValues">Input values from user.</param>
    /// <param name="errors">In case if any errors, will return the list of messages.</param>
    /// <returns>Returns 'true' if no errors found.</returns>
    public static bool ValidateReserved(
        this ValuesProviderSettings settings,
        IReadOnlyCollection<ValueModel> inputValues,
        out IReadOnlyCollection<string> errors)
    {
        if (settings.ReservedValues.Count == 0)
        {
            errors = [];
            return true;
        }

        errors = settings
            .ReservedValues
            .Select(reserved =>
            {
                var value = inputValues.FirstOrDefault(x => x.Name.Equals(reserved.Name, StringComparison.OrdinalIgnoreCase))?.Value.ToString();

                // when value cannot be empty
                if (reserved.IsRequired && value.IsEmpty())
                    return $"{reserved.Name}: Value cannot be empty";

                // when value limited to expected values
                var expectedValues = reserved.ExpectedValues.OfType<string>().ToList();
                if (expectedValues.Count > 0 && !reserved.ExpectedValues.Contains(value, StringComparer.OrdinalIgnoreCase))
                    return $"{reserved.Name}: Unexpected value ({expectedValues.JoinString()})";

                return null;
            })
            .OfType<string>()
            .ToList();

        return errors.Count == 0;
    }
}
