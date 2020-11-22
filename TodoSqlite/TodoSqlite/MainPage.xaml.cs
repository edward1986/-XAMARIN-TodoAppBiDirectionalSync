using Newtonsoft.Json;
using Plugin.Connectivity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using TodoSqlite.Model;
using TodoSqlite.Service;
using TodoSqlite.View;
using TodoSqlite.ViewModel;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace TodoSqlite
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
            BindingContext = new MainPageViewModel(Navigation);
        }
        protected override async void OnAppearing()
        {
            base.OnAppearing();

            await (BindingContext as MainPageViewModel).RefreshTaskList();
            CrossConnectivity.Current.ConnectivityChanged += UpdateNetworkInfo;
        }

        protected override void OnDisappearing()
        {
            CrossConnectivity.Current.ConnectivityChanged -= UpdateNetworkInfo;
        }

        private void UpdateNetworkInfo(object sender, Plugin.Connectivity.Abstractions.ConnectivityChangedEventArgs e)
        {
            (BindingContext as MainPageViewModel).RefreshTaskList();
        }

        private async void TodoDisplayList_ItemSelected(object sender, SelectedItemChangedEventArgs e) { 
            await (BindingContext as MainPageViewModel).HandleNoteSelected((TodoModel)e.SelectedItem);
        }
    }
}
