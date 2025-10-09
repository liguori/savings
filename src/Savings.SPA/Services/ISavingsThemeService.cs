using System;

namespace Savings.SPA.Services;

public interface ISavingsThemeService
{
    event Action? ThemeChanged;
    bool IsDarkMode { get; }
    Task ToggleThemeAsync();
    Task InitializeAsync();
}