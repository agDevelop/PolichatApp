using Newtonsoft.Json;
using PolichatApp.API;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PolichatApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Chat : ContentPage
    {
        private string polichatServerURI = "http://polilabi-001-site1.etempurl.com";

        private string getMsgsURI = "Chat/GetMessages";
        private string sendMsgURI = "Chat/SendMessage";

        private User user = null;

        ObservableCollection<Message> messages = null;

        //List<Message> msgsList = null;

        public Chat(User user)
        {
            InitializeComponent();

            this.user = user;
        }

        protected override async void OnAppearing()
        {
            messages = new ObservableCollection<Message>(await GetMessages(0, user));

            Button logoutButton = new Button()
            {
                Text = "Выход"
            };

            logoutButton.Clicked += async (s, e) =>
            {
                Preferences.Set("logged", "0");
                Preferences.Set("user", string.Empty);

                await Navigation.PopAsync();

                Environment.Exit(0);
            };

            ListView messagesListView = new ListView
            {
                HasUnevenRows = true,
                // Определяем источник данных
                ItemsSource = messages,

                // Определяем формат отображения данных
                ItemTemplate = new DataTemplate(() =>
                {
                    // привязка к свойству Name
                    Label textLabel = new Label() { FontSize = 18, FontAttributes = FontAttributes.Bold, TextColor = Color.Black };
                    textLabel.SetBinding(Label.TextProperty, "Text");

                    StackLayout lbls = new StackLayout()
                    {
                        Orientation = StackOrientation.Horizontal
                    };

                    // привязка к свойству Company
                    Label userNameLabel = new Label() { FontSize = 12, FontAttributes = FontAttributes.Bold, TextColor = Color.Gray};
                    userNameLabel.SetBinding(Label.TextProperty, "UserName");

                    // привязка к свойству Price
                    Label addedLabel = new Label() { FontSize = 12, FontAttributes = FontAttributes.Italic, TextColor =  Color.Gray};
                    addedLabel.SetBinding(Label.TextProperty, "Added");
                    //priceLabel.Text = Convert.ToDateTime(priceLabel.Text).ToString("dd MMM yyyy hh:mm");

                    lbls.Children.Add(userNameLabel);
                    lbls.Children.Add(addedLabel);

                    // создаем объект ViewCell.
                    return new ViewCell
                    {
                        View = new StackLayout
                        {
                            Padding = new Thickness(10, 10, 10, 10),
                            Orientation = StackOrientation.Vertical,
                            Children = { textLabel, lbls }
                        },
                    };
                }),

                BackgroundColor = Color.White,
                SelectionMode = ListViewSelectionMode.None
            };


            StackLayout messageSendLayout = new StackLayout();
            messageSendLayout.Orientation = StackOrientation.Horizontal;

            Editor messageEntry = new Editor()
            {
                Placeholder = "Сообщение",
                HorizontalOptions = LayoutOptions.FillAndExpand
            };

            Button messageSendButton = new Button()
            {
                Text = ">",
                FontSize = 18,
                FontAttributes = FontAttributes.Bold,
                BackgroundColor = Color.Green
            };

            messageSendButton.Clicked += async (s, e) =>
            {
                await SendMesagge(new Message() { Text = messageEntry.Text, UserName = user.Login }, user);

                messageEntry.Text = string.Empty;

                await UpdateMessageListAsync();
            };

            messageSendLayout.Children.Add(messageEntry);
            messageSendLayout.Children.Add(messageSendButton);

            //MessagingCenter.Send<object>(, "MessageReceived");

            this.Content = new StackLayout { Children = { logoutButton, messagesListView,  messageSendLayout}};

            //Message tmp = new Message() { Text = "test", UserName = "test", Added = DateTime.Now };
            //messages.Add(tmp);
            //messagesListView.ScrollTo(messages[messages.Count - 1], ScrollToPosition.End, true);

            Device.StartTimer(TimeSpan.FromSeconds(5), () =>
            {
                Task.Run(async () =>
                {
                    await UpdateMessageListAsync();
                });
                return true;
            });

            base.OnAppearing();
        }

        private async Task UpdateMessageListAsync()
        {
            List<Message> tmpMsgList = await GetMessages(messages.Count, user);
            tmpMsgList.Reverse();

            foreach (Message msg in tmpMsgList)
            {
                messages.Insert(0, msg);
            }
        }

        private async Task<List<Message>> GetMessages(int index, User user)
        {
            WebRequest request = WebRequest.Create($"{polichatServerURI}/{getMsgsURI}");

            request.Method = "POST";

            string serializedUser = await Task.Run(() => JsonConvert.SerializeObject(user));

            string postData = $"index={index}&user={serializedUser}";
            byte[] byteArray = Encoding.UTF8.GetBytes(postData);

            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = byteArray.Length;

            Stream dataStream = request.GetRequestStream();
            dataStream.Write(byteArray, 0, byteArray.Length);
            dataStream.Close();


            WebResponse response = request.GetResponse();

            //MessageBox.Show(((HttpWebResponse)response).StatusDescription);

            string responseFromServer = string.Empty;

            using (dataStream = response.GetResponseStream())
            {
                StreamReader reader = new StreamReader(dataStream);

                responseFromServer = reader.ReadToEnd();

                //MessageBox.Show(responseFromServer);
            }

            response.Close();

            
            List<Message> msgs = await Task.Run(() => JsonConvert.DeserializeObject<List<Message>>(responseFromServer));

            msgs.Reverse();

            return msgs;

            //return new ObservableCollection<Message>(msgs);

        }

        private async Task<int> SendMesagge(Message message, User user)
        {
            WebRequest request = WebRequest.Create($"{polichatServerURI}/{sendMsgURI}");

            request.Method = "POST";

            string serializedMessage = await Task.Run(() => JsonConvert.SerializeObject(message));
            string serializedUser = await Task.Run(() => JsonConvert.SerializeObject(user));

            string postData = $"message={serializedMessage}&user={serializedUser}";
            byte[] byteArray = Encoding.UTF8.GetBytes(postData);

            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = byteArray.Length;

            Stream dataStream = request.GetRequestStream();
            dataStream.Write(byteArray, 0, byteArray.Length);
            dataStream.Close();


            WebResponse response = request.GetResponse();

            //MessageBox.Show(((HttpWebResponse)response).StatusDescription);

            string responseFromServer = string.Empty;

            using (dataStream = response.GetResponseStream())
            {
                StreamReader reader = new StreamReader(dataStream);

                responseFromServer = reader.ReadToEnd();

                //MessageBox.Show(responseFromServer);
            }

            response.Close();

            //return new ObservableCollection<Message>(await Task.Run(() => JsonConvert.DeserializeObject<List<Message>>(responseFromServer)));

            int resp = JsonConvert.DeserializeObject<int>(responseFromServer);

            return resp;
        }
    }
}