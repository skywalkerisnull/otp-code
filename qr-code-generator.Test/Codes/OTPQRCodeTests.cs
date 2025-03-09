public class OTPQRCodeTests
{
    [Fact]
    public void ToString_ShouldReturnCorrectUri()
    {
        // Arrange
        var otpQRCode = new OTPQRCode("")
        {
            Issuer = "ACME Co",
            AccountName = "john.doe@email.com",
            Secret = "HXDMVJECJJWSRB3HWIZR4IFUGFTMXBOZ",
        };

        string expectedUri = "otpauth://totp/ACME%20Co:john.doe@email.com?secret=HXDMVJECJJWSRB3HWIZR4IFUGFTMXBOZ&issuer=ACME%20Co&algorithm=SHA1&digits=6&period=30";

        // Act
        string result = otpQRCode.ToString();

        // Assert
        Assert.Equal(expectedUri, result);
    }

    [Fact]
    public void ToString_ShouldReturnCorrectUriForHOTP()
    {
        // Arrange
        var otpQRCode = new OTPQRCode("")
        {
            Issuer = "ACME Co",
            AccountName = "john.doe@email.com",
            Secret = "HXDMVJECJJWSRB3HWIZR4IFUGFTMXBOZ",
            Algorithm = Algorithm.SHA1,
            Digits = 6,
            Period = 30,
            Type = OTPType.HOTP,
            Counter = 1
        };

        string expectedUri = "otpauth://hotp/ACME%20Co:john.doe@email.com?secret=HXDMVJECJJWSRB3HWIZR4IFUGFTMXBOZ&issuer=ACME%20Co&algorithm=SHA1&digits=6&period=30&counter=1";

        // Act
        string result = otpQRCode.ToString();

        // Assert
        Assert.Equal(expectedUri, result);
    }
}