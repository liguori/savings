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
            var savedTheme = await _jsRuntime.InvokeAsync<string?>("themeUtils.getSavedTheme");
            
            if (!string.IsNullOrEmpty(savedTheme))
            {
                _isDarkMode = savedTheme == "dark";
            }
            else
            {
                // Check system preference
                _isDarkMode = await _jsRuntime.InvokeAsync<bool>("themeUtils.prefersDarkMode");
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
            await _jsRuntime.InvokeVoidAsync("themeUtils.applyTheme", _isDarkMode);
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
            await _jsRuntime.InvokeVoidAsync("themeUtils.saveTheme", theme);
        }
        catch
        {
            // Ignore storage errors
        }
    }
}