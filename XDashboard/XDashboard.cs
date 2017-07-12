using System;
using System.Linq;
using Xamarin.Forms;
using Omise;
using Omise.Models;
using CoreFoundation;
using System.Runtime.Remoting.Channels;

namespace XDashboard
{
    public class App : Application
    {
        Entry PublicKeyEntry { get; set; }
        Entry NameEntry { get; set; }
        Entry NumberEntry { get; set; }
        Entry ExpirationMonthEntry { get; set; }
        Entry ExpirationYearEntry { get; set; }
        Entry SecurityCodeEntry { get; set; }

        Button TokenizeButton { get; set; }
        Entry TokenEntry { get; set; }

        public App()
        {
            PublicKeyEntry = new Entry { Text = "pkey_xxx" };
            NameEntry = new Entry { Text = "John Appleseed" };
            NumberEntry = new Entry { Text = "4242424242424242" };
            ExpirationMonthEntry = new Entry { Text = "1" };
            ExpirationYearEntry = new Entry { Text = "2020" };
            SecurityCodeEntry = new Entry { Text = "123" };

            TokenizeButton = new Button { Text = "Tokenize" };
            TokenEntry = new Entry { Text = "tokn_xxx" };

            TokenizeButton.Clicked += (sender, e) =>
            {
                DispatchQueue.DefaultGlobalQueue.DispatchAsync(() =>
                {
                    var client = new Client(PublicKeyEntry.Text);
                    var task = client.Tokens.Create(new CreateTokenRequest
                    {
                        Name = NameEntry.Text,
                        Number = NumberEntry.Text,
                        ExpirationMonth = int.Parse(ExpirationMonthEntry.Text),
                        ExpirationYear = int.Parse(ExpirationYearEntry.Text),
                        SecurityCode = SecurityCodeEntry.Text
                    });

                    task.Wait();

                    DispatchQueue.MainQueue.DispatchAsync(() =>
                    {
                        if (task.IsFaulted)
                        {
                            TokenEntry.Text = task.Exception.Message;
                            return;
                        }

                        TokenEntry.Text = task.Result.Id;
                    });
                });
            };

            // The root page of your application
            var content = new ContentPage
            {
                Title = "XDashboard",
                Content = new StackLayout
                {
                    VerticalOptions = LayoutOptions.Center,
                    Orientation = StackOrientation.Vertical,
                    Children = {
                        new Label { Text = "Public Key:" },
                        PublicKeyEntry,
                        new Label { Text = "Card Data:" },
                        NameEntry,
                        NumberEntry,
                        ExpirationMonthEntry,
                        ExpirationYearEntry,
                        SecurityCodeEntry,
                        TokenizeButton,
                        TokenEntry,
                    }
                }
            };

            MainPage = new NavigationPage(content);
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
