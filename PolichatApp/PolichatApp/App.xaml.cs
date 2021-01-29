using Newtonsoft.Json;
using PolichatApp.API;
using System;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PolichatApp
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();


            string logged = Preferences.Get("logged", "0");

            NavigationPage page = null;

            if (logged.Equals("1"))
            {
                string serializedUser = Preferences.Get("user", "null");

                User user = JsonConvert.DeserializeObject<User>(serializedUser);

                page = new NavigationPage(new Chat(user) { Title = user.Login }) { Title = user.Login };
            }
            else
            {
                page = new NavigationPage(new Login())
                {
                    Title = "Авторизация"
                };
            }

            MainPage = page;
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
