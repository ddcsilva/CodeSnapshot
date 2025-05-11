using System.Globalization;
using System.Resources;

namespace CodeSnapshot.Helpers;

public static class LocalizationHelper
{
    public static void SetCulture(string cultureCode)
    {
        var culture = new CultureInfo(cultureCode);
        Thread.CurrentThread.CurrentCulture = culture;
        Thread.CurrentThread.CurrentUICulture = culture;
    }
}