﻿using Microsoft.AspNetCore.Components.Forms;

namespace Savings.SPA.Components
{
    public class InputSelectNumber<T> : InputSelect<T>
    {
        protected override bool TryParseValueFromString(string? value, out T result, out string validationErrorMessage)
        {
            if (typeof(T) == typeof(int) || typeof(T) == typeof(long?))
            {
                if (typeof(T) == typeof(int) && int.TryParse(value, out var resultInt))
                {
                    result = (T)(object)resultInt;
                    validationErrorMessage = string.Empty;
                    return true;
                }
                else if (typeof(T) == typeof(long?) && long.TryParse(value, out var resultLong))
                {
                    result = (T)(object)resultLong;
                    validationErrorMessage = string.Empty;
                    return true;
                }
                else
                {
                    result = default!;
                    validationErrorMessage = "The chosen value is not a valid number.";
                    return false;
                }
            }
            else
            {
                return base.TryParseValueFromString(value, out result!, out validationErrorMessage!);
            }
        }
    }
}
