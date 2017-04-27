#region
using System;
using System.Collections.Generic;
using DesignX.Classes;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.IO;
using System.Linq;
using Microsoft.WindowsAzure.MobileServices;
#endregion

namespace DesignX
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DesignXPage : ContentPage
    {   
        public static MobileServiceClient MobileService = new MobileServiceClient("https://watg-designx.azurewebsites.net");
        CardStackView cardStack;
        MainPageViewModel viewModel;   
        int countCheck = 0;
        public DesignXPage()
        {

            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
            viewModel = new MainPageViewModel();         
            this.BindingContext = viewModel;

            //this.BackgroundColor = Color.White;

            cardStack = new CardStackView();
            cardStack.SetBinding(CardStackView.ItemsSourceProperty, "ItemsList");
            cardStack.SwipedLeft += SwipedLeft;
            cardStack.SwipedRight += SwipedRight;

            view.Children.Add(cardStack,
                Constraint.Constant(0),
                Constraint.Constant(0),
                Constraint.RelativeToParent((parent) =>
                {
                    return parent.Width;
                }),
                Constraint.RelativeToParent((parent) =>
                {
                    return parent.Height;
                })
            );

            this.LayoutChanged += (object sender, EventArgs e) =>
            {
                cardStack.CardMoveDistance = (int)(this.Width * 0.60f);
            };


        }

        void Load()
        {
            Xamarin.Forms.Device.StartTimer(TimeSpan.FromSeconds(0.2), OnTimer);
        }

        private bool OnTimer()
        {
            var progress = (ProgressControl.Progress + .01);
            if (progress > 1) progress = 0;
            ProgressControl.Progress = progress;

            return true;
        }


        async void SwipedLeft(int index)
        {
            try
            {
				string result = "";
				if (InformationClass.Dict.Count == 1)
				{
					var _result = InformationClass.Dict.First();
					result = _result.Value;
				}
				else 
				{
					InformationClass.Dict.TryGetValue(index-2 , out result);
					InformationClass.Dict.Remove(index - 2);
                
				}

                var item = await MobileService.GetTable<Project>().LookupAsync(result);
                if (item==null)
                    Application.Current.MainPage = new NavigationPage(new FinishPage());
                else
                {
                    item.DownVote = item.DownVote + 1;
                    await MobileService.GetTable<Project>().UpdateAsync(item);
                    ImageTable image = new ImageTable
                    {
                        UserName = AzureResult.UserName,
                        UpVote = false,
                        ImageUrl = item.Url
                    };
                    await MobileService.GetTable<ImageTable>().InsertAsync(image);
                    countCheck++;
                    if (countCheck == AzureResult.Count || InformationClass.Dict.Count < 1)
                        Application.Current.MainPage = new NavigationPage(new FinishPage());
                }




            }
            catch (Exception ex)
            {
				await DisplayAlert("Alert", "Unable to cast vote. Kindly restart the application and try again", "OK");
            }
        }

        async void SwipedRight(int index)
        {
            try
            {        
				string result = "";
				if (InformationClass.Dict.Count == 1)
				{
					var _result = InformationClass.Dict.First();
					result = _result.Value;
				}
				else 
				{
					InformationClass.Dict.TryGetValue(index-2 , out result);
				InformationClass.Dict.Remove(index - 2);
                
				}




				

                var item = await MobileService.GetTable<Project>().LookupAsync(result);
                if (item == null)
                    Application.Current.MainPage = new NavigationPage(new FinishPage());
                else
                {
                    item.UpVote = item.UpVote + 1;
                    await MobileService.GetTable<Project>().UpdateAsync(item);
                    ImageTable image = new ImageTable
                    {
                        UserName = AzureResult.UserName,
                        UpVote = true,
                        ImageUrl = item.Url

                    };
                    await MobileService.GetTable<ImageTable>().InsertAsync(image);
                    countCheck++;
                    if (countCheck == AzureResult.Count ||InformationClass.Dict.Count <1)
                        Application.Current.MainPage = new NavigationPage(new FinishPage());
                }





            }
            catch (Exception ex)
            {
				await DisplayAlert("Alert", "Unable to cast vote. Kindly restart the application and try again", "OK");
            }

        }
    }
}