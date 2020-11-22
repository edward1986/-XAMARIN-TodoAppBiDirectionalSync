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
    public partial class DetailTodoPage : ContentPage
    {
        public DetailTodoPage(DetailTodoPageViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }

        private void CheckBox_CheckedChanged(object sender, CheckedChangedEventArgs e)
        {

        }
    }
}