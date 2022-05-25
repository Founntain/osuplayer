namespace OsuPlayer.Network.Security;

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
    public static bool CheckIfPasswordMeetsRequirements(string password)
    {
        var textCharArray = password.ToCharArray();
        var errorMessages = new List<string>();

        if (password.Length < 8) errorMessages.Add("At least 8 characters long");

        if (!textCharArray.Any(char.IsDigit)) errorMessages.Add("At least 1 number");

        if (textCharArray.All(char.IsLetterOrDigit)) errorMessages.Add("At least one special character");

        if (!(textCharArray.Any(char.IsUpper) && textCharArray.Any(char.IsLower)))
            errorMessages.Add("At least one uppercase and one lowercase character");

        if (errorMessages.Count > 0)
        {
            var s = "Your password needs to meet the following requirements:\n";

            foreach (var errorMessage in errorMessages) s += errorMessage + "\n";

            // OsuPlayerMessageBox.Show(s, "Fix password errors");

            return false;
        }

        return true;
    }

    /// <summary>
    /// Checks if the given password meets our password requierements
    /// </summary>
    /// <param name="password"></param>
    /// <returns>A boolean if the password meets the requirements and if not also returns a error list</returns>
    public static (bool, string?) CheckIfPasswordMeetsRequirementsWithErrorList(string password)
    {
        var textCharArray = password.ToCharArray();
        var errorMessages = new List<string>();

        if (password.Length < 8) errorMessages.Add("At least 8 characters long");

        if (!textCharArray.Any(char.IsDigit)) errorMessages.Add("At least 1 number");

        if (textCharArray.All(char.IsLetterOrDigit)) errorMessages.Add("At least one special character");

        if (!(textCharArray.Any(char.IsUpper) && textCharArray.Any(char.IsLower)))
            errorMessages.Add("At least one uppercase and one lowercase character");

        if (errorMessages.Count > 0)
        {
            var s = "Your password needs to meet the following requirements:\n";

            foreach (var errorMessage in errorMessages) s += errorMessage + "\n";

            // OsuPlayerMessageBox.Show(s, "Fix password errors");

            return (false, s);
        }

        return (true, null);
    }
}