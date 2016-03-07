using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using BluePosition.Samples.ContactsSample.Properties;
using BluePosition.Samples.LoginApi;
using Newtonsoft.Json;

namespace BluePosition.Samples.ContactsSample
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //private const string OcpApimSubscriptionKey = "INSERTSUBSCRIPTIONKEY";
        private string token;


        public MainWindow()
        {
            InitializeComponent();

            LoadUserSettings();
        }

        private void LoadUserSettings()
        {
            UsernameTxb.Text = Settings.Default.LastKnownUsername;
            OcpApimSubscriptionKeyTxb.Text = Settings.Default.OcpApiKey;
        }

        private async void PasswordTxb_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                await LoadContacts();
            }
        }

        private async Task<bool> RefreshTokenIfRequired()
        {
            if (string.IsNullOrEmpty(token))
            {
                var tmpToken = await Login(UsernameTxb.Text, PasswordTxb.Password);
                if (string.IsNullOrEmpty(tmpToken))
                {
                    StatusLbl.Content = "Login error";
                    return false;
                }

                token = tmpToken;
                Settings.Default.LastKnownUsername = UsernameTxb.Text;
                Settings.Default.OcpApiKey = OcpApimSubscriptionKeyTxb.Text;
                Settings.Default.Save();
            }
            return true;
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

        private async Task LoadContacts()
        {
            if (!await RefreshTokenIfRequired())
                return;
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

            lvDataBinding.ItemsSource = contacts.OrderBy(c => c.ScaleUserId).ToArray();
        }

        private async void GetContactsButton_Click(object sender, RoutedEventArgs e)
        {
            await LoadContacts();
        }

        private async void lvDataBinding_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectedContactIdLbl.Content = string.Empty;
            SelectedContactUsernameLbl.Content = string.Empty;
            SelectedContactPresenceLbl.Content = string.Empty;

            var lv = e.OriginalSource as ListView;
            var selectedContact = lv?.SelectedItem as Contact;
            if (selectedContact != null)
            {
                SelectedContactIdLbl.Content = selectedContact.Id;
                SelectedContactUsernameLbl.Content = selectedContact.ScaleUserId;
                var presence = await GetPresence(selectedContact);
                var presenceDisplayText = string.Empty;
                if (presence.BusyState == "Idle")
                {
                    presenceDisplayText = presence.BusyState;
                }
                else
                {
                    presenceDisplayText = $"{presence.Source}: {presence.BusyState}";
                }

                SelectedContactPresenceLbl.Content = presenceDisplayText;
            }
        }



        private async Task<Presence> GetPresence(Contact selectedContact)
        {
            if (!await RefreshTokenIfRequired())
                return null;
            string url = $"https://api.cloud.mobileservices.dk/presence/api/presence/{selectedContact.Id}";
            var client = HttpClientHelpers.CreateWithToken(OcpApimSubscriptionKeyTxb.Text, token);
            var response = await client.GetAsync(url);
            var stringResponse = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
            {
                return JsonConvert.DeserializeObject<Presence>(stringResponse);
            }
            StatusLbl.Content = "HTTP " + ((int)response.StatusCode) + " " + response.StatusCode;

            return null;
        }
    }
}
