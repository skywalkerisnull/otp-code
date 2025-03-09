public class OTPQRCodeTests
{
    //TODO: Create more sucessful tests for the OTPQRCode class. i.e. I need to do more "success" tests on the string that will be produced by the ToString method.
    
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
            OTPType = OTPType.HOTP,
            Counter = 1
        };

        string expectedUri = "otpauth://hotp/ACME%20Co:john.doe@email.com?secret=HXDMVJECJJWSRB3HWIZR4IFUGFTMXBOZ&issuer=ACME%20Co&algorithm=SHA1&digits=6&period=30&counter=1";

        // Act
        string result = otpQRCode.ToString();

        // Assert
        Assert.Equal(expectedUri, result);
    }

    [Fact]
    public void ValidateInput_ShouldThrowException_WhenAccountNameIsNull()
    {
        // Arrange
        var otpQRCode = new OTPQRCode("")
        {
            Issuer = "ACME Co",
            Secret = "HXDMVJECJJWSRB3HWIZR4IFUGFTMXBOZ",
        };

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() => otpQRCode.ValidateInput());
        Assert.Contains("AccountName is required.", exception.Message);
    }

    [Fact]
    public void ValidateInput_ShouldThrowException_WhenIssuerIsNull()
    {
        // Arrange
        var otpQRCode = new OTPQRCode("")
        {
            AccountName = "john.doe@email.com",
            Secret = "HXDMVJECJJWSRB3HWIZR4IFUGFTMXBOZ",
        };

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() => otpQRCode.ValidateInput());
        Assert.Contains("Issuer is required.", exception.Message);
    }

    [Fact]
    public void ValidateInput_ShouldThrowException_WhenSecretIsNull()
    {
        // Arrange
        var otpQRCode = new OTPQRCode("")
        {
            Issuer = "ACME Co",
            AccountName = "john.doe@email.com",
        };

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() => otpQRCode.ValidateInput());
        Assert.Contains("Secret is required.", exception.Message);
    }

    [Fact]
    public void ValidateInput_ShouldThrowException_WhenDigitsIsInvalid()
    {
        // Arrange
        var otpQRCode = new OTPQRCode("")
        {
            Issuer = "ACME Co",
            AccountName = "john.doe@email.com",
            Secret = "HXDMVJECJJWSRB3HWIZR4IFUGFTMXBOZ",
            Digits = 0
        };

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() => otpQRCode.ValidateInput());
        Assert.Contains("Digits must be greater than 0.", exception.Message);
    }

    [Fact]
    public void ValidateInput_ShouldThrowException_WhenPeriodIsInvalid()
    {
        // Arrange
        var otpQRCode = new OTPQRCode("")
        {
            Issuer = "ACME Co",
            AccountName = "john.doe@email.com",
            Secret = "HXDMVJECJJWSRB3HWIZR4IFUGFTMXBOZ",
            Period = 0
        };

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() => otpQRCode.ValidateInput());
        Assert.Contains("Period must be greater than 0.", exception.Message);
    }

    [Fact]
    public void ValidateInput_ShouldThrowException_WhenCounterIsInvalidForHOTP()
    {
        // Arrange
        var otpQRCode = new OTPQRCode("")
        {
            Issuer = "ACME Co",
            AccountName = "john.doe@email.com",
            Secret = "HXDMVJECJJWSRB3HWIZR4IFUGFTMXBOZ",
            OTPType = OTPType.HOTP,
            Counter = -1
        };

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() => otpQRCode.ValidateInput());
        Assert.Contains("Counter must be non-negative for HOTP.", exception.Message);
    }
}