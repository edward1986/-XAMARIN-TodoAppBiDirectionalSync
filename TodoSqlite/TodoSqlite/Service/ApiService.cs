using Newtonsoft.Json;
using RestSharp;
using SQLite;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using TodoSqlite.Model;
using Xamarin.Essentials;

namespace TodoSqlite.Service
{
    public class ApiService
    {
        public static async Task<bool> RegisterUser(string name = "", string email = "", string password = "", string password_confirmation = "")
        {
            var register = new RegisterModel()
            {
                name = name ,
                email = email,
                password = password,
                password_confirmation = password_confirmation
            };

            var httpClient = new HttpClient();
            var json = JsonConvert.SerializeObject(register);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await httpClient.PostAsync(AppSettings.ApiUrl + "api/register", content);
            if (!response.IsSuccessStatusCode) { 
                var jsonResult = await response.Content.ReadAsStringAsync();

        
                Preferences.Set("errors", jsonResult);
                return false;
                }

            
            return true;
        }
        public  static async Task<bool> LoginUser(string email, string password)
        {
            var login = new LoginModel()
            {
                email = email,
                password = password
            };
            var httpClient = new HttpClient();
            var json = JsonConvert.SerializeObject(login);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await httpClient.PostAsync(AppSettings.ApiUrl + "api/login", content);
            if (!response.IsSuccessStatusCode) return false;

            var jsonResult = await response.Content.ReadAsStringAsync();

            var result = JsonConvert.DeserializeObject<TokenModel>(jsonResult);
            Preferences.Set("access_token", result.access_token);
            Preferences.Set("userId", result.user.id.ToString());
            Preferences.Set("userName", result.user.name);
            return true;
        }

        

        public static async Task<bool> AllUpdateTodo(List<TodoModel> todo)
        {
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", Preferences.Get("access_token", string.Empty));
            var json = JsonConvert.SerializeObject(todo);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await httpClient.PostAsync(AppSettings.ApiUrl + "api/todos/all/update", content);
            var jsonResult = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode) return false;



            return true;
        }

        
        public static async Task<bool> AllStoreTodo(List<TodoModel> todo)
        {
           
               
                var httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", Preferences.Get("access_token", string.Empty));
                var json = JsonConvert.SerializeObject(todo);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await httpClient.PostAsync(AppSettings.ApiUrl + "api/todos/all/store", content);
                var jsonResult = await response.Content.ReadAsStringAsync();
                if (!response.IsSuccessStatusCode){
                    return false;
                }
               
            return true;
        }

        public static async Task<TodoModel> PostTodo(TodoModel todo)
        {
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", Preferences.Get("access_token", string.Empty));
            var json = JsonConvert.SerializeObject(todo);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await httpClient.PostAsync(AppSettings.ApiUrl + "api/todos", content);
            var jsonResult = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode) return JsonConvert.DeserializeObject<TodoModel>("[]");

            
            
            return JsonConvert.DeserializeObject<TodoModel>(jsonResult);
        }


        public static async Task<bool> AllDeleteTodo(List<int> todo)
        {
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", Preferences.Get("access_token", string.Empty));
            var json = JsonConvert.SerializeObject(todo);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await httpClient.PostAsync(AppSettings.ApiUrl + "api/todos/all/delete", content);
            var jsonResult = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode) return false;



            return true;
        }
        public static async Task<TodoModel> PutTodo(TodoModel todo)
        {
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", Preferences.Get("access_token", string.Empty));
            var json = JsonConvert.SerializeObject(todo);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await httpClient.PutAsync(AppSettings.ApiUrl + "api/todos/" + todo.id, content);
            var jsonResult = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode) return JsonConvert.DeserializeObject<TodoModel>(jsonResult);



            return JsonConvert.DeserializeObject<TodoModel>(jsonResult);
        }
        public static async Task<List<TodoModel>> GetTodos()
        {



            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", Preferences.Get("access_token", string.Empty));
            var response = await httpClient.GetStringAsync(AppSettings.ApiUrl + "api/todos");
            Preferences.Set("getTodo", response);
            return JsonConvert.DeserializeObject<List<TodoModel>>(response);

            

          









        }

        public static async Task<bool> DeleteTodos(TodoModel todo)
        {
            Console.WriteLine("[todo id]");
            Console.WriteLine(todo.id);
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", Preferences.Get("access_token", string.Empty));
            var response = await httpClient.DeleteAsync(AppSettings.ApiUrl + "api/todos/" + todo.id);
            if (!response.IsSuccessStatusCode) return false;
            return true;
        }

        public static async Task<bool> LogoutAccount()
        {
            
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", Preferences.Get("access_token", string.Empty));
            var response = await httpClient.GetAsync(AppSettings.ApiUrl + "api/logout");
            if (!response.IsSuccessStatusCode) return false;

            Preferences.Set("access_token", string.Empty);
            Preferences.Set("userId", string.Empty);
            Preferences.Set("userName", string.Empty);

            

            return true;
        }
    }
}
