using MY.QuickAPI.Settings;

namespace MY.QuickAPI.Example.Settings;

public class TokenServiceSettings : ISettings
{
    public TimeSpan ExpiryDuration { get; set; }
    public string Key { get; set; } = "C2C963B5F00ADC4D1D52A79B1762B808AC9120AEC2598122F10ABD227302D328"; 
    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
}