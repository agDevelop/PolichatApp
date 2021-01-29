using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using Newtonsoft.Json;
using System.Net.Http;
using System.Net;
using PolichatApp.API;
using System.IO;
using Xamarin.Essentials;

namespace PolichatApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Login : ContentPage
    {
        private string polichatServerURI = "http://polilabi-001-site1.etempurl.com";

        private string logInURI = "Chat/Login";
        private string regURI = "Chat/Register";

        Entry loginEntry;
        Entry passEntry;

        Frame frame = new Frame
        {
            Content = new Label { Text = "by Анрей Гончаров, 181-321", HorizontalOptions = LayoutOptions.Center },
            CornerRadius = 8,
            BorderColor = Color.Green,
            Margin = new Thickness(5, 50, 5, 20)
        };

        public Login()
        {
            InitializeComponent();

            ShowLoginLayout();
        }

        private void ShowLoginLayout()
        {
            StackLayout logStackLayout = new StackLayout();

            Label regLabel = new Label()
            {
                Text = "Вход",
                FontAttributes = FontAttributes.Bold,
            };

            loginEntry = new Entry();
            loginEntry.Placeholder = "Логин";

            passEntry = new Entry();
            passEntry.Placeholder = "Пароль";
            passEntry.IsPassword = true;

            Button logButton = new Button()
            {
                Text = "Войти"
            };

            logButton.Clicked += async (s, e) =>
            {
                if (string.IsNullOrEmpty(loginEntry.Text) || string.IsNullOrEmpty(passEntry.Text))
                {
                    await DisplayAlert("Ошибка!", "Заполните все поля!", "OK");
                }
                else
                {
                    LogIn(new User() { Login = loginEntry.Text, Password = CryptoProvider.GetMD5Hash(passEntry.Text) });
                }
            };




            Label notSignedUpLabel = new Label()
            {
                Text = "Нет аккаунта?",
                FontAttributes = FontAttributes.Bold,
            };

            Button goToRegLayoutButton = new Button()
            {
                Text = "Зарегистрироваться!"
            };

            goToRegLayoutButton.Clicked += (s, e) =>
            {
                ShowRegistrationLayout();
            };

            logStackLayout.Children.Add(regLabel);

            logStackLayout.Children.Add(loginEntry);
            logStackLayout.Children.Add(passEntry);

            logStackLayout.Children.Add(logButton);

            logStackLayout.Children.Add(notSignedUpLabel);
            logStackLayout.Children.Add(goToRegLayoutButton);

            logStackLayout.Children.Add(frame);

            Content = logStackLayout;
        }

        private void ShowRegistrationLayout()
        {
            StackLayout regStackLayout = new StackLayout();

            Label regLabel = new Label()
            {
                Text = "Регистрация",
                FontAttributes = FontAttributes.Bold,
            };

            Entry emailEntry = new Entry();
            emailEntry.Placeholder = "Email";

            Entry loginEntry = new Entry();
            loginEntry.Placeholder = "Логин";

            Entry passEntry = new Entry();
            passEntry.Placeholder = "Пароль";
            passEntry.IsPassword = true;

            Button regButton = new Button()
            {
                Text = "Зарегистрироваться!"
            };

            regButton.Clicked += async (s, e) =>
            {
                if (string.IsNullOrEmpty(emailEntry.Text) || string.IsNullOrEmpty(loginEntry.Text) || string.IsNullOrEmpty(passEntry.Text))
                {
                    await DisplayAlert("Ошибка!", "Заполните все поля!", "OK");
                }
                else
                {
                    Register(new User() { Email = emailEntry.Text, Login = loginEntry.Text, Password = CryptoProvider.GetMD5Hash(passEntry.Text) });
                }
            };

            Label alreadySignedUpLabel = new Label()
            {
                Text = "Есть аккаунт?",
                FontAttributes = FontAttributes.Bold,
            };

            Button goToLogLayoutButton = new Button()
            {
                Text = "Войти"
            };

            goToLogLayoutButton.Clicked += (s, e) =>
            {
                ShowLoginLayout();
            };

            regStackLayout.Children.Add(regLabel);

            regStackLayout.Children.Add(emailEntry);
            regStackLayout.Children.Add(loginEntry);
            regStackLayout.Children.Add(passEntry);

            regStackLayout.Children.Add(regButton);

            regStackLayout.Children.Add(alreadySignedUpLabel);
            regStackLayout.Children.Add(goToLogLayoutButton);

            regStackLayout.Children.Add(frame);

            Content = regStackLayout;
        }

        private async void LogIn(User user)
        {
            try
            {
                WebRequest request = WebRequest.Create($"{polichatServerURI}/{logInURI}");

                request.Method = "POST";

                string serializedUser = await Task.Run(() => JsonConvert.SerializeObject(user));
                string postData = $"user={serializedUser}";
                byte[] byteArray = Encoding.UTF8.GetBytes(postData);

                request.ContentType = "application/x-www-form-urlencoded";
                request.ContentLength = byteArray.Length;

                Stream dataStream = request.GetRequestStream();
                dataStream.Write(byteArray, 0, byteArray.Length);
                dataStream.Close();

                WebResponse response = request.GetResponse();

                using (dataStream = response.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(dataStream);

                    string responseFromServer = reader.ReadToEnd();

                    //await DisplayAlert("Ответ", responseFromServer, "OK");

                    string responseCode = JsonConvert.DeserializeObject<string>(responseFromServer);

                    if (responseCode.Equals("1"))
                    {
                        Preferences.Set("logged", "1");
                        Preferences.Set("user", serializedUser);

                        await Navigation.PushAsync(new Chat(user) { Title = user.Login });
                    }
                    else
                    {
                        await DisplayAlert("Ошибка", "Вход не удался", "Ок");
                    }
                }

                response.Close();
            }
            catch (Exception)
            {
                await DisplayAlert("Ошибка", "Что-то пошло не так...", "Ок");
            }
        }

        private async void Register(User user)
        {
            try
            {
                WebRequest request = WebRequest.Create($"{polichatServerURI}/{regURI}");
                request.Method = "POST";

                string serializedUser = await Task.Run(() => JsonConvert.SerializeObject(user));
                string postData = $"user={serializedUser}";
                byte[] byteArray = Encoding.UTF8.GetBytes(postData);

                request.ContentType = "application/x-www-form-urlencoded";
                request.ContentLength = byteArray.Length;

                Stream dataStream = request.GetRequestStream();
                dataStream.Write(byteArray, 0, byteArray.Length);
                dataStream.Close();

                WebResponse response = request.GetResponse();

                using (dataStream = response.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(dataStream);
                    string responseFromServer = reader.ReadToEnd();

                    //await DisplayAlert("Ответ", responseFromServer, "OK");

                    string responseCode = JsonConvert.DeserializeObject<string>(responseFromServer);

                    if (responseCode.Equals("1"))
                    {
                        Preferences.Set("logged", "1");
                        Preferences.Set("user", serializedUser);

                        await Navigation.PushAsync(new Chat(user) { Title = user.Login });
                    }
                    else
                    {
                        await DisplayAlert("Ошибка", "Регистрация не удалась", "Ок");
                    }
                }

                response.Close();
            }
            catch (Exception)
            {
                await DisplayAlert("Ошибка", "Что-то пошло не так...", "Ок");
            }
        }
    }
}