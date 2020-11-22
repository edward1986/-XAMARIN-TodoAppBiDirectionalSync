using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using TodoSqlite.Model;
using TodoSqlite.View;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace TodoSqlite.ViewModel
{
    class MainPageViewModel : INotifyPropertyChanged
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
        public MainPageViewModel(INavigation navigation)
        { 
            _navigation = navigation;
            IsBusy = true;
            
            GetGroupedTodoList().ContinueWith(t =>
            {
                IsBusy = false;
                GroupedTodoList = t.Result;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(GroupedTodoList)));
            });
            Delete = new Command<TodoModel>(HandleDelete);
            Logout = new Command(HandleLogout);
            ChangeIsCompleted = new Command<TodoModel>(HandleChangeIsCompleted);
            AddItem = new Command(HandleAddItem);
            NoteSelectedCommand = new Command<TodoModel>(async (item) =>
            {
                if (item is null)
                     return;

                 var detailViewModel = new DetailTodoPageViewModel
                 {
                     Title = item.title,
                     Id = item.id,
                     IsCompleted = item.isCompleted == 0 ? false : true
                 };

                 await Application.Current.MainPage.Navigation.PushAsync(new DetailTodoPage(detailViewModel));
            });
        }

        public async Task HandleNoteSelected(TodoModel todo)
        {
            if (todo is null)
                return;

            var detailViewModel = new DetailTodoPageViewModel
            {

                Title = todo.title,
                Id = todo.id,
                IsCompleted = todo.isCompleted == 0 ? false : true
            };

            await Application.Current.MainPage.Navigation.PushAsync(new DetailTodoPage(detailViewModel));
        }
        TodoModel selectedNote;
        public TodoModel SelectedNote
        {
            get => selectedNote;
            set
            {
                selectedNote = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedNote)));
            }
        }
        private INavigation _navigation;

        public event PropertyChangedEventHandler PropertyChanged;

        public ILookup<string, TodoModel> GroupedTodoList { get; set; }
        public string Title => "My Todo list";

        private async Task<ILookup<string, TodoModel>> GetGroupedTodoList()
        {
            
            
            return (await App.Database.GetTodoModelsAsync())

                             .OrderBy(t => t.isCompleted)
                             .ToLookup(t => t.isCompleted == 1 ? "Completed" : "Active");
        }

        public Command<TodoModel> Delete { get; set; }
        public Command Logout { get; set; }
        public Command<TodoModel> NoteSelectedCommand { get; }
        public async void HandleDelete(TodoModel itemToDelete)
        {
            IsBusy = true;
            await App.Database.DeleteTodoModelAsync(itemToDelete);
            // Update displayed list
         
            GroupedTodoList = await GetGroupedTodoList();
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(GroupedTodoList)));
            IsBusy = false;
        }

        public async void HandleLogout()
        {

            try
            {
                IsBusy = true;
                await App.Database.LogoutTodoModelAsync();
            }
            finally
            {
                IsBusy = false;
            }
           
            
            // Update displayed list

        }

        public Command<TodoModel> ChangeIsCompleted { get; set; }
        public async void HandleChangeIsCompleted(TodoModel itemToUpdate)
        {
            IsBusy = true;
            await App.Database.ChangeItemIsCompleted(itemToUpdate);
            // Update displayed list
            GroupedTodoList = await GetGroupedTodoList();
           
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(GroupedTodoList)));
            IsBusy = false;
        }

        public Command AddItem { get; set; }
        public async void HandleAddItem()
        {
          
            await _navigation.PushModalAsync(new AddTodoPage());
        }

        public async Task RefreshTaskList()
        {
           IsBusy = true;
            GroupedTodoList = await GetGroupedTodoList();
           
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(GroupedTodoList)));
            IsBusy = false;
        }
    }
}
