using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Plugin.Connectivity;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TodoSqlite.Model;
using TodoSqlite.Service;
using TodoSqlite.View;
using TodoSqlite.ViewModel;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace TodoSqlite.Data
{
    public class TodoDatabase
    {
        readonly SQLiteAsyncConnection _database;
        int databaseCount = 1;
        public TodoDatabase(string dbPath)
        {
            _database = new SQLiteAsyncConnection(dbPath);

            //_database.DropTableAsync<TodoModel>().Wait();
            _database.CreateTableAsync<TodoModel>().Wait();
           

        }
        
        public async Task<List<TodoModel>> GetTodoModelsAsync()
        {
            if (CrossConnectivity.Current.IsConnected)
            {
                var deleteTodo = _database.Table<TodoModel>().Where(i => i.Delete == true);
                var updateTodo = _database.Table<TodoModel>().Where(i => i.Update == true);
                var storeTodo = _database.Table<TodoModel>().Where(i => i.Store == true);
                if (await storeTodo.CountAsync() > 0)
                {

                    var todo = storeTodo.ToListAsync();
                    var todostore = JsonConvert.SerializeObject(todo).ToString();
                    JObject todoJson = JObject.Parse(todostore);
                    List<TodoModel> listStore = new List<TodoModel> { };
                    foreach (var row in todoJson["Result"])
                    {


                        listStore.Add(new TodoModel
                        {
                            isCompleted = (int)row["isCompleted"],
                            title = (string)row["title"]
                        });
                    }
                    
                   await storeTodo.DeleteAsync();
                    await ApiService.AllStoreTodo(listStore);
                  



                }
             
               
                if (await updateTodo.CountAsync() > 0)
                {


                    var todo = updateTodo.ToListAsync();
                    var todostore = JsonConvert.SerializeObject(todo).ToString();
                    JObject todoJson = JObject.Parse(todostore);
                    List<TodoModel> listStore = new List<TodoModel> { };
                    foreach (var row in todoJson["Result"])
                    {


                        listStore.Add(new TodoModel
                        {
                            id = (int)row["id"],
                            isCompleted = (int)row["isCompleted"],
                            title = (string)row["title"]
                        });
                    }
                    await storeTodo.DeleteAsync();
                    await ApiService.AllUpdateTodo(listStore);
                    

                }

                if (await deleteTodo.CountAsync() > 0)
                {
                    var todo = deleteTodo.ToListAsync();
                    var tododelete = JsonConvert.SerializeObject(todo).ToString();
                    JObject todoJson = JObject.Parse(tododelete);
                    List<int> deleteId = new List<int> { };
                    foreach (var row in todoJson["Result"])
                    {
                        deleteId.Add((int)row["id"]);
                    }
                    await deleteTodo.DeleteAsync();
                    await ApiService.AllDeleteTodo(deleteId);

                }


                var server = await ApiService.GetTodos();
                var client = await _database.Table<TodoModel>().ToListAsync();
                var result = "";
                var jsonResult = Preferences.Get("getTodo", string.Empty);
                var clientjsonResult = JsonConvert.SerializeObject(client).ToString();
                JArray jsonParse = JArray.Parse(jsonResult);
                JArray clientJson = JArray.Parse(clientjsonResult);
                List<int> serverId = new List<int> { };
                List<int> clientId = new List<int> { };
                List<int> clientDeletedId = new List<int> { };
                foreach (var row in clientJson)
                {
                    clientId.Add((int)row["id"]); ;
                }
                foreach (var jp in jsonParse)
                {

                    var todo = new TodoModel
                    {
                        title = (string)jp["title"],
                        isCompleted = (int)jp["isComplete"],
                        id = (int)jp["id"],
                        created_at = (DateTime)jp["created_at"],
                        updated_at = (DateTime)jp["updated_at"]
                    };
                    if (!client.Any(x => x.id == (int)jp["id"]))
                    {
                        await _database.InsertAsync(todo);
                    }
                    else if (client.Any(x => x.id == (int)jp["id"]))
                    {
                        JObject updateClient = JObject.Parse(JsonConvert.SerializeObject(client.Single(x => x.id == (int)jp["id"])));
                        todo.cid = (int)updateClient["cid"];
                        await _database.UpdateAsync(todo);
                    }
                    serverId.Add((int)jp["id"]);

                }
                IEnumerable<int> res = clientId.AsQueryable().Except(serverId);
                foreach (int id in res)
                {
                    await DeleteTodoModelAsync(new TodoModel { id = id });
                }



            }
            return await _database.Table<TodoModel>().Where(i => i.Delete == false).ToListAsync();
        }

        public async Task InitializeTodo()
        {

           

        }
        public Task<TodoModel> GetTodoModelAsync(int id)
        {
            return _database.Table<TodoModel>()
                            .Where(i => i.id == id)
                            .FirstOrDefaultAsync();
        }

        public async Task<int> SaveTodoModelAsync(TodoModel note)
        {
            
            if (note.id != 0)
            {
                if (CrossConnectivity.Current.IsConnected)
                {
                    var todo = await ApiService.PutTodo(note);
                    
                    return await _database.UpdateAsync(todo);

                }
                else
                {
                    note.Update = true;
                    note.updated_at = DateTimeOffset.Now;
                    return await _database.UpdateAsync(note);
                }

            }
            else
            {
                if (CrossConnectivity.Current.IsConnected)
                {
                    var todo = await ApiService.PostTodo(note);
                    var result = await _database.InsertAsync(todo);
                    return result;
                }
                else
                {
                    note.Store = true;
                    return await _database.InsertAsync(note); ;
                }

            }
        }

        public async Task<int> DeleteTodoModelAsync(TodoModel note)
        {
            if (CrossConnectivity.Current.IsConnected)
            {

                await _database.DeleteAsync(note);
                await ApiService.DeleteTodos(note);
                return 1;
            }
            else
            {
                note.Delete = true;
                return await _database.UpdateAsync(note);
            }
        }
        public async Task<bool> LogoutTodoModelAsync()
        {
            if (CrossConnectivity.Current.IsConnected)
            {
                var todo = await ApiService.LogoutAccount();
                Application.Current.MainPage = new NavigationPage(new SignupView());
                return true;
            }
            else
            {
                Preferences.Set("access_token", string.Empty);
                Preferences.Set("userId", string.Empty);
                Preferences.Set("userName", string.Empty);
                return true;
            }
            // (App.Current.MainPage.BindingContext as MainPageViewModel).RefreshTaskList();
            

        }
        public async Task ChangeItemIsCompleted(TodoModel itemToChange)
        {
            if (CrossConnectivity.Current.IsConnected)
            {
                if (itemToChange.isCompleted == 0)
                {
                    itemToChange.isCompleted = 1;
                }
                else
                {
                    itemToChange.isCompleted = 0;
                }
                var todo = await ApiService.PutTodo(itemToChange);
                await _database.UpdateAsync(itemToChange);
              
            }
            else
            {
                if (itemToChange.isCompleted == 0)
                {
                    itemToChange.isCompleted = 1;
                }
                else
                {
                    itemToChange.isCompleted = 0;
                }

                itemToChange.Update = true;
                itemToChange.updated_at = DateTimeOffset.Now;
               await _database.UpdateAsync(itemToChange);
            }

        }
    }
}
