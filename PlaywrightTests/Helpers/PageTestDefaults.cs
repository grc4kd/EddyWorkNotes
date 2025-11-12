namespace PlaywrightTests.Helpers;


/// <summary>
/// Page test constant settings shared across referencing test classes.
/// </summary>
public static class PageTestDefaults
{
    public const string DefaultPort = "5085";
    public static readonly string DefaultBaseUrl = $"http://localhost:{DefaultPort}";
    public static readonly float DefaultTimeout = (float)TimeSpan.FromMilliseconds(100).TotalMilliseconds;
}