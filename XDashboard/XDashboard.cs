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
        Entry SecretKeyEntry { get; set; }
        Editor ChargeListEditor { get; set; }
        Button CallButton { get; set; }

        public App()
        {
            SecretKeyEntry = new Entry { Placeholder = "skey_xxx" };
            ChargeListEditor = new Editor
            {
                Text = "chrg_xxx",
                VerticalOptions = LayoutOptions.FillAndExpand,
                MinimumHeightRequest = 176.0
            };

            CallButton = new Button { Text = "Call Omise API" };
            CallButton.Clicked += (sender, e) =>
            {
                DispatchQueue.DefaultGlobalQueue.DispatchAsync(() =>
                {
                    var client = new Client(skey: SecretKeyEntry.Text);
                    var task = client.Charges.GetList(
                        order: Ordering.ReverseChronological,
                        limit: 3
                    );

                    task.Wait();

                    DispatchQueue.MainQueue.DispatchAsync(() =>
                    {
                        if (task.IsFaulted)
                        {
                            ChargeListEditor.Text = task.Exception.Message;
                            return;
                        }

                        ChargeListEditor.Text = task.Result
                            .Select(chrg => $"{chrg.Id} {chrg.Currency} {chrg.Amount}")
                            .Aggregate((a, b) => $"{a}\n{b}");
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
                        new Label { Text = "Secret Key:" },
                        SecretKeyEntry,
                        CallButton,
                        new Label { Text = "Recent Charges:" },
                        ChargeListEditor,
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
