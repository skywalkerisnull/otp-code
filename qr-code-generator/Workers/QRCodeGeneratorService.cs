using QRCoder;
using System;
using System.Threading.Tasks;

public class QRCodeGeneratorService
{
    public Task<string> GenerateQrCodeAsync(string otpUrl)
    {
        return Task.Run(() =>
        {
            using var qrGenerator = new QRCodeGenerator();
            using var qrCodeData = qrGenerator.CreateQrCode(otpUrl, QRCodeGenerator.ECCLevel.Q);
            using var pngQrCode = new PngByteQRCode(qrCodeData);
            var qrCodeBytes = pngQrCode.GetGraphic(20);
            return $"data:image/png;base64,{Convert.ToBase64String(qrCodeBytes)}";
        });
    }
}