using Microsoft.JSInterop;

namespace Savings.SPA.Services;

public class SavingsThemeService : ISavingsThemeService
{
    private readonly IJSRuntime _jsRuntime;
    private bool _isDarkMode = false;

    public event Action? ThemeChanged;
    public bool IsDarkMode => _isDarkMode;

    public SavingsThemeService(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    public async Task InitializeAsync()
    {
        try
        {
            // Check localStorage for saved theme preference
            var savedTheme = await _jsRuntime.InvokeAsync<string?>("localStorage.getItem", "theme-preference");
            
            if (!string.IsNullOrEmpty(savedTheme))
            {
                _isDarkMode = savedTheme == "dark";
            }
            else
            {
                // Check system preference
                _isDarkMode = await _jsRuntime.InvokeAsync<bool>("window.matchMedia('(prefers-color-scheme: dark)').matches");
            }

            await ApplyThemeAsync();
        }
        catch
        {
            // Fallback to light theme if anything fails
            _isDarkMode = false;
            await ApplyThemeAsync();
        }
    }

    public async Task ToggleThemeAsync()
    {
        _isDarkMode = !_isDarkMode;
        await SaveThemePreferenceAsync();
        await ApplyThemeAsync();
        ThemeChanged?.Invoke();
    }

    private async Task ApplyThemeAsync()
    {
        try
        {
            var themeName = _isDarkMode ? "standard-dark" : "default-base";
            var themeUrl = $"_content/Radzen.Blazor/css/{themeName}.css";
            
            // Switch the Radzen theme CSS file
            await _jsRuntime.InvokeVoidAsync("eval", $@"
                const themeLink = document.getElementById('theme-link');
                if (themeLink) {{
                    themeLink.href = '{themeUrl}';
                }}
            ");

            // Also set a data attribute on the body for custom CSS targeting
            await _jsRuntime.InvokeVoidAsync("eval", $@"
                document.body.setAttribute('data-theme', '{(_isDarkMode ? "dark" : "light")}');
            ");
        }
        catch
        {
            // Ignore JS interop errors during initial load
        }
    }

    private async Task SaveThemePreferenceAsync()
    {
        try
        {
            var theme = _isDarkMode ? "dark" : "light";
            await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "theme-preference", theme);
        }
        catch
        {
            // Ignore storage errors
        }
    }
}