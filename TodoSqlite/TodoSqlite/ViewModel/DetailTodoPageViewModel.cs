using System;
using System.ComponentModel;
using System.Windows.Input;
using TodoSqlite.Model;
using Xamarin.Forms;

namespace TodoSqlite.ViewModel
{
    public class DetailTodoPageViewModel : INotifyPropertyChanged
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
        public DetailTodoPageViewModel()
        {
            ExitCommand = new Command(async () => await Application.Current.MainPage.Navigation.PopAsync());
            UpdateCommand = new Command(HandleSave);
        }
        public event PropertyChangedEventHandler PropertyChanged;
        public async void HandleSave()
        {

            await App.Database.SaveTodoModelAsync(new TodoModel { id=Id,  title = Title, isCompleted = IsCompleted == true ? 1 : 0 });

            await Application.Current.MainPage.Navigation.PopAsync();
        }
        string title;
        public string Title
        {
            get => title;
            set
            {
                title = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Title)));
            }
        }

        int id;
        public int Id
        {
            get => id;
            set
            {
                id = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Id)));
            }
        }

        bool isCompleted;
        public bool IsCompleted
        {
            get => isCompleted;
            set
            {
                isCompleted = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsCompleted)));
            }
        }

        public Command UpdateCommand { get; set; }
        public ICommand ExitCommand { get; }
    }
}
