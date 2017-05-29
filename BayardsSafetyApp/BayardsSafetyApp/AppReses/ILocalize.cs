using System.Globalization;

namespace BayardsSafetyApp.AppRes
{
    //This class is used to support multilangual functionality
    public interface ILocalize
    {
        CultureInfo GetCurrentCultureInfo();
        void SetLocale(CultureInfo ci);
    }
}