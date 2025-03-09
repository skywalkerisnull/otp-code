using System.Net.Mail;
public class OTPQRCode : BaseQRCode
{
    public string? AccountName { get; set; }
    public string? Issuer { get; set; }
    public string? Secret { get; set; }

    public OTPType Type { get; set; } = OTPType.TOTP;
    public Algorithm Algorithm { get; set; } = Algorithm.SHA1;
    public int Digits { get; set; } = 6;
    public int Period { get; set; } = 30;

    // Only required if the type is OTPType.HOTP
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
        var accountNameEncoded = accountNameEncode();
        var secretEncoded = Uri.EscapeDataString(Secret);
        var algorithmString = Algorithm.ToString().ToUpper();

        var uri = $"otpauth://{typeString}/{issuerEncoded}:{accountNameEncoded}?secret={secretEncoded}&issuer={issuerEncoded}&algorithm={algorithmString}&digits={Digits}&period={Period}";

        if (Type == OTPType.HOTP)
        {
            uri += $"&counter={Counter}";
        }

        return uri;
    }

    private string accountNameEncode()
    {
        if (IsValidEmail(AccountName))
        {
            return AccountName;
        }
        else
        {
            return Uri.EscapeDataString(AccountName);
        }
    }

    private bool IsValidEmail(string? email)
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
}

public enum OTPType { TOTP, HOTP };

public enum Algorithm { SHA1, SHA256, SHA512 };