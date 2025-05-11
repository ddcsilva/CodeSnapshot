using CodeSnapshot.Helpers;
using System.Globalization;

namespace CodeSnapshot
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            // Detecta idioma do sistema e aplica cultura
            string language = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;
            if (language == "pt")
                LocalizationHelper.SetCulture("pt-BR");
            else
                LocalizationHelper.SetCulture("en");

            // Inicia UI após aplicar cultura
            MainPage = new AppShell();
        }
    }
}
