using Bunit;
using qr_code_generator.Pages;

public class HomeTests : TestContext
{
    [Fact]
    public void HomeComponentRendersCorrectly()
    {
        // Arrange
        JSInterop.SetupVoid("addShortcutListener", _ => true);
        var component = RenderComponent<Home>();

        // Act
        var h1Element = component.Find("h1");

        // Assert
        Assert.Equal("QR Code Generator", h1Element.TextContent);
    }
}