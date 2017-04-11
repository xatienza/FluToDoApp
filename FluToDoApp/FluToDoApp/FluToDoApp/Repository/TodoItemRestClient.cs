using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text;

namespace FluToDoApp.Repository
{
    using Models;    

    public class TodoItemRestClient
    {
        /// <summary>
        /// Http client
        /// </summary>
        private HttpClient client;

        /// <summary>
        /// Rest service base uri
        /// </summary>
        public string RestAPIUri { get; set; }

        /// <summary>
        /// Default Ctor.
        /// </summary>
        public TodoItemRestClient()
        {
            client = new HttpClient();
            client.MaxResponseContentBufferSize = 256000;
        }

        public async Task<List<TodoItem>> GetAllItemsAsync(string uri = "")
        {
            List<TodoItem> result = null;
            var actionUri = string.IsNullOrEmpty(uri) ? RestAPIUri : uri;

            // Create an HTTP web request using the URL:
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(new Uri(actionUri));
            request.ContentType = "application/json";
            request.Method = "GET";

            // RestUrl = http://localhost:8080/api/todo{0}
            var clientUri = new Uri(string.Format(actionUri, string.Empty));

            var response = await client.GetAsync(clientUri);

            if (response.IsSuccessStatusCode)
            {
                // async read of http response
                var content = await response.Content.ReadAsStringAsync();
                result = JsonConvert.DeserializeObject<List<TodoItem>>(content);
            }

            return result;
        }

        public async Task SaveTodoItemAsync(TodoItem item, bool isNewItem=false)
        {
            // RestUrl = http://localhost:8080/api/todo{0}
            var uri = new Uri(string.Format(RestAPIUri, item.Key));

            var json = JsonConvert.SerializeObject(item);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            HttpResponseMessage response = null;
            if (isNewItem)
            {
                response = await client.PostAsync(uri, content);
            }
            else
            {
                response = await client.PutAsync(uri, content);
            }

            if (response.IsSuccessStatusCode)
            {
                Debug.WriteLine(@"TodoItem successfully saved.");

            }
        }

        public async Task DeleteTodoItemAsync(string id)
        {
            // RestUrl = http://localhost:8080/api/todo{0}
            var uri = new Uri(string.Format(RestAPIUri, id));

            var response = await client.DeleteAsync(uri);
            if (response.IsSuccessStatusCode)
            {
                Debug.WriteLine(@" TodoItem successfully deleted.");
            }
        }
    }
}