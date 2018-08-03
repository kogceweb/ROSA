﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using ROSA.Models;



namespace ROSA.Services
{
    public class TopicRestService : ITopicRestService
    {
        HttpClient client;

        public List<Topic> Items { get; private set; }


        public TopicRestService()
        {
            var authData = string.Format("{0}:{1}", Constants.Username, Constants.Password);
            var authHeaderValue = Convert.ToBase64String(Encoding.UTF8.GetBytes(authData));

            client = new HttpClient();
            client.MaxResponseContentBufferSize = 256000;
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authHeaderValue);
        }
        
        public Task DeleteTopicAsync(string id)
        {
            throw new NotImplementedException();
        }

        public async Task<List<Topic>> RefreshDataAsync()
        {
            Items = new List<Topic>();

            var uri = new Uri(string.Format(Constants.RestTopicUrl, string.Empty));

            try
            {
                var response = await client.GetAsync(uri);
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    Items = JsonConvert.DeserializeObject<List<Topic>>(content);
                    foreach (Topic element in Items)
                    {

                        Debug.WriteLine($"Element #{element.Code}: {element.Title}");
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(@"ERROR {0}", ex.Message);
            }

            return Items;
         
        }

        public async Task SaveTopicAsync(Topic item, bool isNewItem)
        {
            var uri = new Uri(string.Format(Constants.RestTopicUrl, string.Empty));

            try
            {
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
                    Debug.WriteLine(@"				Comment successfully saved.");
                }

            }
            catch (Exception ex)
            {
                Debug.WriteLine(@"				ERROR {0}", ex.Message);
            }
        }
    }


}
