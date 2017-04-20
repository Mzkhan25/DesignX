#region
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using DesignX.Classes;
using Microsoft.WindowsAzure.MobileServices;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
#endregion

namespace DesignX
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LoadPage
    {
        public static MobileServiceClient MobileService =
            new MobileServiceClient("https://watg-designx.azurewebsites.net");
        private bool _entryCheck = true;
        private int ImageCount;
        private string pin;
        private List<Project> result = new List<Project>();
        public LoadPage()
        {
            InitializeComponent();
        }
        private void entryCheck_Focused(object sender, FocusEventArgs e)
        {
            if (_entryCheck)
            {
                UserPin.Text = "";
                _entryCheck = false;
            }
            Grid.SetRow(WelcomeScreen, 0);
            Grid.SetRow(UserPin, 2);
            Grid.SetRow(LoginButton, 3);
        }
        private void entryCheck_UnFocused(object sender, FocusEventArgs e)
        {
            Grid.SetRow(WelcomeScreen, 1);
            Grid.SetRow(UserPin, 4);
            Grid.SetRow(LoginButton, 5);
        }
        private void Load()
        {
            Device.StartTimer(TimeSpan.FromSeconds(0.2), OnTimer);
            GetDataFromAzure();
        }
        private void Handle_Clicked(object sender, EventArgs e)
        {
            WelcomeScreen.IsVisible = false;
            LoginButton.IsVisible = false;
            UserPin.IsVisible = false;
            ProgressControl.IsVisible = true;
            Login();
        }
        private async void UpdateTable()
        {
            var user = new User
            {
                Name = "Mark",
                Email = "mbajwa@watg.com",
                Password = "MAR007"
            };
            await MobileService.GetTable<User>().InsertAsync(user);
            user = new User
            {
                Name = "Edgard",
                Email = "mzkhan25@hotmail.com",
                Password = "EDG007"
            };
            await MobileService.GetTable<User>().InsertAsync(user);
            user = new User
            {
                Name = "Test",
                Email = "mzkhan25@hotmail.com",
                Password = "TES007"
            };
            await MobileService.GetTable<User>().InsertAsync(user);
            user = new User
            {
                Name = "Felicia",
                Email = "mzkhan25@hotmail.com",
                Password = "FEL007"
            };
            await MobileService.GetTable<User>().InsertAsync(user);
            user = new User
            {
                Name = "Tolga",
                Email = "mzkhan25@hotmail.com",
                Password = "TOL007"
            };
            await MobileService.GetTable<User>().InsertAsync(user);
        }
        private async void Login()
        {
            pin = UserPin.Text;
            if (string.IsNullOrWhiteSpace(pin))
            {
                await DisplayAlert("Alert", "Kindly Enter Pin", "OK");
                WelcomeScreen.IsVisible = true;
                LoginButton.IsVisible = true;
                UserPin.IsVisible = true;
                ProgressControl.IsVisible = false;
            }
            else
            {
                Device.StartTimer(TimeSpan.FromSeconds(0.2), OnTimer);
                var user = await MobileService.GetTable<User>().Where(p => p.Password == pin).ToListAsync();
                if (user.Count == 1)
                {
                    AzureResult.UserName = user[0].Name;
                    await GetDataFromAzure();
                }
                else
                {
                    await DisplayAlert("Alert", "Invalid Pin", "OK");
                    WelcomeScreen.IsVisible = true;
                    LoginButton.IsVisible = true;
                    UserPin.IsVisible = true;
                    ProgressControl.IsVisible = false;
                }
            }
        }
        private bool OnTimer()
        {
            var progress = ProgressControl.Progress + .01;
            if (progress > 1) progress = 0;
            ProgressControl.Progress = progress;
            return true;
        }
        private async void DataUpload()
        {
            var name = "Design Talk 2016 Projects -";
            var url = "https://watgdevops.blob.core.windows.net/designtinder/";
            var projects = new List<Project>();        
            //var result=await MobileService.GetTable<CardStackView.Project>().ToListAsync();
            //foreach (var item in result)
            //{
            //	await MobileService.GetTable<CardStackView.Project>().DeleteAsync(item);
            //}
            for (var i = 1; i < 56; i++)
            {
                var project = new Project
                {
                    UpVote = 0,
                    DownVote = 0,
                    Url = url + i + ".jpg",
                    Deleted = false,
                    Name = name + i + ".jpg"
                };
                await MobileService.GetTable<Project>().InsertAsync(project);
            }
            var user = new User
            {
                Name = "Muhammad Bajwa",
                Email = "mbajwa@watg.com",
                Password = "ceo"
            };
            await MobileService.GetTable<User>().InsertAsync(user);
            user = new User
            {
                Name = "Muiz Khan",
                Email = "mzkhan25@hotmail.com",
                Password = "developer"
            };
            await MobileService.GetTable<User>().InsertAsync(user);
        }
        private async Task GetDataFromAzure()
        {
            //var user = await MobileService.GetTable<User>().Where(p => p.Password == pin).ToListAsync();
            //AzureResult.userName = user[0].Name;
            try
            {
                var completed = false;
                var check = false;
                var imageResult = await MobileService.GetTable<ImageTable>()
                    .Take(1000)
                    .Where(p => p.UserName == AzureResult.UserName)
                    .Select(prop => prop.ImageUrl)
                    .ToListAsync();
                var imageCount = await MobileService.GetTable<Project>()
                    .Take(1000)
                    .Select(prop => prop.Deleted == false)
                    .ToListAsync();
                result = await MobileService.GetTable<Project>()
                    .Take(1000)
                    .Where(p => p.Deleted == false)
                    .ToListAsync();
                var _result = new List<Project>();
                if (imageResult.Count >= imageCount.Count - 1)
                {
                    Application.Current.MainPage = new NavigationPage(new FinishPage());
                }
                else
                {
                    if (imageResult.Count < 1)
                        check = true;
                    if (!check)
                        foreach (var project in result)
                        foreach (var image in imageResult)
                            if (image == project.Url)
                                project.Url = "";
                    if (result.Count < 1)
                    {
                        Application.Current.MainPage = new NavigationPage(new FinishPage());
                    }
                    else
                    {
                        foreach (var item in result)
                            if (item.Url != "")
                            {
                                var webClient = new WebClient();
                                webClient.DownloadDataCompleted += (s, e) =>
                                {
                                    var bytes = e.Result; // get the downloaded data
                                    var documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                                    var localFilename = item.Id;                                   
                                    var localPath = Path.Combine(documentsPath, localFilename+".jpg");
                                    File.WriteAllBytes(localPath, bytes); // writes to local storage
                                    ImageCount++;
                                    Navigate();
                                };
                                var url = new Uri(item.Url);
                                webClient.DownloadDataAsync(url);                                   
                                
                                _result.Add(item);
                            }
                        AzureResult.Count = _result.Count;
                        AzureResult.AzureResults = _result;
                    }
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("alert", "Error connecting to Server. Kindly try again", "Dismiss");
                WelcomeScreen.IsVisible = true;
                LoginButton.IsVisible = true;
                UserPin.IsVisible = true;
                ProgressControl.IsVisible = false;
            }
        }
        private void Navigate()
        {
            if (ImageCount == AzureResult.Count)
                Application.Current.MainPage = new NavigationPage(new DesignXPage());
        }  
    }
}