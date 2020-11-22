using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TodoSqlite.ViewModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TodoSqlite.View
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AddTodoPage : ContentPage
    {
        public AddTodoPage()
        {
            InitializeComponent();
            BindingContext = new AddTodoPageViewModel(Navigation);
        }
    }
}