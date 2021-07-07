
using AdminPanel.Models.Category;
using Microsoft.AspNetCore.Mvc;
using Spots.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;


namespace AdminPanel.Models
{
    public sealed class VendorModel
    {
        VendorModel()
        {

        }
        private static readonly object _lock = new object();
        private static VendorModel instance = null;
        public static VendorModel Instance
        {
            get
            {
                lock (_lock)
                {
                    if (instance == null)
                    {
                        instance = new VendorModel();
                    }
                    return instance;
                }

                //string name = "singleton";
                //if (System.Web.HttpContext.Current.Session[name] == null)
                //    HttpContext.Current.Session[name] = new MyClass();
                //return (singleton)HttpContext.Current.Session[name];
            }
        }
        public List<CategoryListModel> Categories { get; set; }
        private Guid? VendorId { get; set; }

        public async Task<Guid?> GetVendorId(IHttpClientFactory httpClientFactory)
        {
            if(VendorId != null)
            {
                return VendorId;
            }
            else
            {
                var httpClient = httpClientFactory.CreateClient("APIClient");
                var request = new HttpRequestMessage(
                    HttpMethod.Get,
                    $"/api/vendor/{Guid.Empty}");

                var response = await httpClient.SendAsync(
                    request, HttpCompletionOption.ResponseHeadersRead).ConfigureAwait(false);
                try
                {
                    response.EnsureSuccessStatusCode();
                    using (var responseStream = await response.Content.ReadAsStreamAsync())
                    {
                        var deserializedResponse = await JsonSerializer
                            .DeserializeAsync<DeserializedResponseModel<VendorDomainModel>>(responseStream);
                        var vendor = deserializedResponse.Data.FirstOrDefault();
                        VendorId = vendor.Id;
                    }
                    return VendorId;
                }
                catch (Exception)
                {
                    return null;
                }
                
            }
        }

    }
}
