using System.Text;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
public class WifiQRCode : BaseQRCode
{
    [Required(ErrorMessage = "SSID is required.")]
    public string? SSID { get; set; }

    public string? Password { get; set; }

    public WifiSecurity Security { get; set; } = WifiSecurity.WPA;

    public WifiQRCode(string uri) : base(uri)
    {
        if (string.IsNullOrWhiteSpace(uri))
        {
            return;
        }

        var uriObj = new Uri(uri);
        var queryParams = System.Web.HttpUtility.ParseQueryString(uriObj.Query);

        foreach (var param in queryParams.AllKeys)
        {
            var property = GetType().GetProperty(param, BindingFlags.Public | BindingFlags.Instance);
            if (property != null)
            {
                try
                {
                    var value = queryParams[param];
                    if (property.PropertyType.IsEnum)
                    {
                        if (Enum.TryParse(property.PropertyType, value, true, out var enumValue))
                        {
                            property.SetValue(this, enumValue);
                        }
                        else
                        {
                            throw new ArgumentException($"Invalid value '{value}' for enum type '{property.PropertyType.Name}'");
                        }
                    }
                    else
                    {
                        property.SetValue(this, Convert.ChangeType(value, property.PropertyType));
                    }
                }
                catch (Exception ex)
                {
                    throw new ArgumentException($"Error setting property '{param}' to value '{queryParams[param]}'", ex);
                }
            }
        }
    }

    public override string ToString()
    {
        var wifiBuilder = new StringBuilder();
        wifiBuilder.Append("WIFI:");
        wifiBuilder.Append($"S:{SSID};");
        wifiBuilder.Append($"T:{Security};");
        if (!string.IsNullOrWhiteSpace(Password))
        {
            wifiBuilder.Append($"P:{Password};");
        }
        wifiBuilder.Append(";");
        return wifiBuilder.ToString();
    }

    public override IEnumerable<InputDefinition> GetInputDefinitions()
    {
        return new List<InputDefinition>
        {
            new InputDefinition
            {
                Name = "SSID",
                Type = InputType.String,
                Placeholder = SSID
            },
            new InputDefinition
            {
                Name = "Password",
                Type = InputType.String
            },
            new InputDefinition
            {
                Name = "Security",
                Type = InputType.Dropdown,
                DropdownOptions = Enum.GetNames(typeof(WifiSecurity)).ToList()
            }
        };
    }

    public override bool ValidateInput()
    {
                var errors = new List<string>();
        var properties = GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

        foreach (var property in properties)
        {
            var value = property.GetValue(this);
            var requiredAttribute = property.GetCustomAttribute<RequiredAttribute>();
            var rangeAttribute = property.GetCustomAttribute<RangeAttribute>();

            if (requiredAttribute != null && value == null)
            {
                errors.Add(requiredAttribute.ErrorMessage ?? $"{property.Name} is required.");
            }

            if (rangeAttribute != null && value is int intValue)
            {
                if (intValue < (int)rangeAttribute.Minimum || intValue > (int)rangeAttribute.Maximum)
                {
                    errors.Add(rangeAttribute.ErrorMessage ?? $"{property.Name} must be between {rangeAttribute.Minimum} and {rangeAttribute.Maximum}.");
                }
            }
        }

        if (errors.Count > 0)
        {
            throw new InvalidOperationException(string.Join(" ", errors));
        }

        return true;
    }
}

public enum WifiSecurity
{
    [Display(Name = "WPA")]
    WPA,
    [Display(Name = "WEP")]
    WEP,
    [Display(Name = "None")]
    None
}