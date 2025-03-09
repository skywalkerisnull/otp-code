using System.Net.Mail;
using System.Text;
using SimpleBase;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
public class OTPQRCode : BaseQRCode
{
    [Required(ErrorMessage = "AccountName is required.")]
    public string? AccountName { get; set; }

    [Required(ErrorMessage = "Issuer is required.")]
    public string? Issuer { get; set; }

    [Required(ErrorMessage = "Secret is required.")]
    public string? Secret { get; set; }

    public OTPType Type { get; set; } = OTPType.TOTP;
    public Algorithm Algorithm { get; set; } = Algorithm.SHA1;

    [Range(1, int.MaxValue, ErrorMessage = "Digits must be greater than 0.")]
    public int Digits { get; set; } = 6;

    [Range(1, int.MaxValue, ErrorMessage = "Period must be greater than 0.")]
    public int Period { get; set; } = 30;

    // Only required if the type is OTPType.HOTP
    [Range(0, int.MaxValue, ErrorMessage = "Counter must be non-negative for HOTP.")]
    public int Counter { get; set; } = 0;

    public OTPQRCode(string uri) : base(uri)
    {

        if (string.IsNullOrWhiteSpace(uri))
        {
            return;
        }

        var uriObj = new Uri(uri);
        var queryParams = System.Web.HttpUtility.ParseQueryString(uriObj.Query);

        if (queryParams["applicationName"] != null)
        {
            Issuer = queryParams["applicationName"];
        }
        if (queryParams["otpSeed"] != null)
        {
            Secret = queryParams["otpSeed"];
        }
        if (queryParams["accountName"] != null)
        {
            AccountName = queryParams["accountName"];
        }
        if (queryParams["type"] != null && Enum.TryParse(queryParams["type"], out OTPType type))
        {
            Type = type;
        }
        if (queryParams["algorithm"] != null && Enum.TryParse(queryParams["algorithm"], out Algorithm algorithm))
        {
            Algorithm = algorithm;
        }
        if (queryParams["digits"] != null && int.TryParse(queryParams["digits"], out int digits))
        {
            Digits = digits;
        }
        if (queryParams["period"] != null && int.TryParse(queryParams["period"], out int period))
        {
            Period = period;
        }
        if (queryParams["counter"] != null && int.TryParse(queryParams["counter"], out int counter))
        {
            Counter = counter;
        }
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

    public override string ToString()
    {
        if (!ValidateInput())
        {
            throw new InvalidOperationException("Invalid input");
        }

        var typeString = Type.ToString().ToLower();
        var issuerEncoded = Uri.EscapeDataString(Issuer);
        var accountNameEncoded = AccountNameEncode();
        var secretEncoded = SecretEncode();
        var algorithmString = Algorithm.ToString().ToUpper();

        var uriBuilder = new StringBuilder();
        uriBuilder.Append($"otpauth://{typeString}/{issuerEncoded}:{accountNameEncoded}");
        uriBuilder.Append($"?secret={secretEncoded}");
        uriBuilder.Append($"&issuer={issuerEncoded}");
        uriBuilder.Append($"&algorithm={algorithmString}");
        uriBuilder.Append($"&digits={Digits}");
        uriBuilder.Append($"&period={Period}");

        if (Type == OTPType.HOTP)
        {
            uriBuilder.Append($"&counter={Counter}");
        }

        return uriBuilder.ToString();
    }

    private string AccountNameEncode()
    {
        if (string.IsNullOrWhiteSpace(AccountName))
        {
            AccountName = string.Empty;
            return AccountName;
        }

        if (IsValidEmail(AccountName))
        {
            return AccountName;
        }
        else
        {
            return Uri.EscapeDataString(AccountName);
        }
    }

    private static bool IsValidEmail(string? email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            return false;
        }

        try
        {
            var mailAddress = new MailAddress(email);
            return true;
        }
        catch (FormatException)
        {
            return false;
        }
    }

    private string SecretEncode()
    {
        try
        {
            // Try to decode the secret to check if it is already BASE32 encoded
            Base32.Rfc4648.Decode(Secret);
            return Secret; // If decoding succeeds, return the original secret
        }
        catch
        {
            // If decoding fails, encode the secret in BASE32
            var secretBytes = Encoding.UTF8.GetBytes(Secret);
            return Base32.Rfc4648.Encode(secretBytes);
        }
    }
}

public enum OTPType { TOTP, HOTP };

public enum Algorithm { SHA1, SHA256, SHA512 };