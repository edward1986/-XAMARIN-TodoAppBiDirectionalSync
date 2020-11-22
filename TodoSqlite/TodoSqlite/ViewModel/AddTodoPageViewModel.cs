using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using TodoSqlite.Model;
using Xamarin.Forms;

namespace TodoSqlite.ViewModel
{
    public class AddTodoPageViewModel : INotifyPropertyChanged
    {
        bool isBusy;
        public bool IsBusy
        {
            get => isBusy;
            set
            {
                isBusy = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsBusy)));
            }
        }
        public AddTodoPageViewModel(INavigation navigation)
        {
            _navigation = navigation;
            Save = new Command(HandleSave);
            Cancel = new Command(HandleCancel);
        }

        private INavigation _navigation;
        private string title;

        public string Title
        {
            get { return title; }
            set { 
                title = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsCompleted)));
            }
        }

        private bool isCompleted;

        public event PropertyChangedEventHandler PropertyChanged;

        public bool IsCompleted
        {
            get { return isCompleted; }
            set { 
                isCompleted = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsCompleted)));
            }
        }

        public Command Save { get; set; }
        public async void HandleSave()
        {
            IsBusy = true;
            await App.Database.SaveTodoModelAsync(new TodoModel{ title = Title, isCompleted = IsCompleted == true ? 1 : 0 });
            IsBusy = false;
            await _navigation.PopModalAsync();
        }

        public Command Cancel { get; set; }
        public async void HandleCancel()
        {
            await _navigation.PopModalAsync();
        }
    }
}
