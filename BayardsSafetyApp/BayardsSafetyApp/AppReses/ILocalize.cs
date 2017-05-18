using System.Globalization;

namespace BayardsSafetyApp.AppRes
{
    public interface ILocalize
    {
        CultureInfo GetCurrentCultureInfo();
        void SetLocale(CultureInfo ci);
    }
}