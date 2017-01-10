using XamarinApp.Common;
using XamarinApp.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace XamarinApp.Pages
{
    public partial class LogInPage : GenericPage
    {
        public XappViewModel AppVM => Application.Current.BindingContext as XappViewModel;

        //Because we are bypassing the normal binding, we have to provide these for the header where they normally would come from the VM
        public string TileName => AppVM.LogInVM.TileName;
        public string TileIcon => AppVM.LogInVM.TileIcon;


        #region Constructors
        public LogInPage()
        {
            //BindingContext = AppVM;
            BindingContext = AppVM.LogInVM;
            AppVM.navBackCommand = new Command(NavBack);
            InitializeComponent();
        }
        #endregion

        protected async override void OnAppearing()
        {
            try
            {
                BtnSubmit.IsVisible = true;
                base.OnAppearing();
                //EntName.Focus();
            }
            catch (Exception)
            {
            }

            var bc = BindingContext as ViewModelBase;
            bc.notificationManagerCommand = null;//We don't go to other pages from login, that includes notifications which can in-turn send you to other pages.
            await AppVM.HomePageVM.RemoveAllPopups();
        }


        #region ShowSubmit (Bindable bool)
        public static readonly BindableProperty ShowSubmitProperty =
BindableProperty.Create("ShowSubmit", typeof(bool), typeof(LogInPage), false);

        public bool ShowSubmit
        {
            get { return (bool)GetValue(ShowSubmitProperty); }
            set { SetValue(ShowSubmitProperty, value); }
        }
        #endregion ShowSubmit  (Bindable bool)


        protected override bool OnBackButtonPressed()
        {
            return true;
            //True being that its been handled
            //Keeps an Android device from responding to the hardware Back button
            //and backing out of the Application completely and going to the desktop.
        }

        private void ListView_OnItemTapped(object sender, ItemTappedEventArgs e)
        {
            EntPassword.Text = String.Empty;
            BtnSubmit.IsVisible = true;
            EntName_OnCompleted(null, null);
        }

        private void EntPassword_OnCompleted(object sender, EventArgs e)
        {
            //this.Focus();// Try to drop the keyboard
            EntPassword.Unfocus();
            if (Device.OS == TargetPlatform.Android)
                EntPasswordDroid.Unfocus();
            AppVM.LogInCommand.Execute(null);
        }

        private void EntName_OnCompleted(object sender, EventArgs e)
        {
            EntPassword.Focus();
            if (Device.OS == TargetPlatform.Android)
                EntPasswordDroid.Focus();
        }

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();
            var bc = BindingContext as ViewModelBase;
            //if (bc != null) bc.navBackCommand = new Command(NavBack);
            //if (bc != null) bc.devTestCommand = new Command(On_DeveloperTestCommand);
            if (bc != null) bc.notificationManagerCommand = null;
            //if (bc != null) bc.showSettingsCommand = new Command(On_ShowSettingsCommand);
        }


        private void UserName_Clicked(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            AppVM.EnteredLogInName = btn.Text;
            EntPassword.Text = String.Empty;
            EntName_OnCompleted(null, null);
        }
    }
}
