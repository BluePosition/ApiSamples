using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using BluePosition.Samples.ContactsSample;
using BluePosition.Samples.LoginApi;
using Newtonsoft.Json;

namespace ContactsSampleWpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //private const string OcpApimSubscriptionKey = "INSERTSUBSCRIPTIONKEY";

        public MainWindow()
        {
            InitializeComponent();
        }

        private async void PasswordTxb_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                var token = await Login(UsernameTxb.Text, PasswordTxb.Password);
                var contacts = await GetContacts(token);
                lvDataBinding.ItemsSource = contacts;
            }
        }

        private async Task<string> Login(string username, string password)
        {
            StatusLbl.Content = "Logging in...";
#if DEBUG
            return string.Empty;
#else
            return await new LoginTask(OcpApimSubscriptionKeyTxb.Text).Execute(username, password);
#endif
        }

        private async Task<Contact[]> GetContacts(string token)
        {
            // ReSharper disable once JoinDeclarationAndInitializer
            Contact[] contacts;
            StatusLbl.Content = "Loading contacts...";
#if DEBUG
            contacts = new[]
            {
                new Contact
                {
                    Id = "1",
                    ScaleUserId = "user@vk123456.hvoip.dk",
                    AvailableFeatures = new []
                    {
                        new Availablefeature { Enabled = true, Id = "1", Name = "CalendarPresence"},
                        new Availablefeature { Enabled = true, Id = "2", Name = "CallPresence"},
                        new Availablefeature { Enabled = true, Id = "3", Name = "ExtendedPresence"},
                        new Availablefeature { Enabled = true, Id = "4", Name = "JobPrivatePresence"},
                    },
                },
            };
#else
            const string url = "https://api.cloud.mobileservices.dk/contact/api/contact";
            var client = HttpClientHelpers.CreateWithToken(OcpApimSubscriptionKeyTxb.Text, token);
            var response = await client.GetAsync(url);
            var stringResponse = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
            {
                contacts = JsonConvert.DeserializeObject<Contact[]>(stringResponse);
            }
            else
            {
                StatusLbl.Content = "HTTP " + ((int)response.StatusCode) + " " + response.StatusCode;
                contacts = new Contact[0];
            }
#endif
            if (!StatusLbl.Content.ToString().StartsWith("HTTP"))
                StatusLbl.Content = $"Done! Got {contacts.Length} contact(s).";

            return contacts;
        }
    }
}
