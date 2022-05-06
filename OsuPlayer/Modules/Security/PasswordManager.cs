using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OsuPlayer.UI_Extensions;

namespace OsuPlayer.Modules.Security;

/// <summary>
/// A static class with some helpful static methods to validate passwords
/// </summary>
public static class PasswordManager
{
    /// <summary>
    /// Checks if the given password meets our password requierements
    /// </summary>
    /// <param name="password"></param>
    /// <returns></returns>
    public static async Task<bool> CheckIfPasswordMeetsRequirements(string password)
    {
        var textCharArray = password.ToCharArray();
        var errorMessages = new List<string>();

        if (password.Length < 8) errorMessages.Add("At least 8 characters long");

        if (!textCharArray.Any(char.IsDigit)) errorMessages.Add("At least 1 number");

        if (textCharArray.All(char.IsLetterOrDigit)) errorMessages.Add("At least one special character");

        if (!(textCharArray.Any(char.IsUpper) && textCharArray.Any(char.IsLower)))
            errorMessages.Add("At least one uppercase and one lowercase character");

        if (errorMessages.Count <= 0) return true;

        var s = "Your password needs to meet the following requirements:\n";

        foreach (var errorMessage in errorMessages) s += errorMessage + "\n";

        await MessageBox.ShowDialogAsync(Core.Instance.MainWindow, s, "Fix password errors");

        return false;
    }
}