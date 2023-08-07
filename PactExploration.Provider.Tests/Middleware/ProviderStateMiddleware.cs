using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Provider.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace PactExploration.Provider.Tests.Middleware
{
    public class ProviderStateMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly FakeDatabase _fakeDatabase;
        private readonly IDictionary<string, Action> _providerStates;

        public ProviderStateMiddleware(RequestDelegate next, FakeDatabase fakeDatabase)
        {
            this._next = next;
            this._fakeDatabase = fakeDatabase;
            this._providerStates = new Dictionary<string, Action>
            {
                { "2 initial products created", ProductsExists },
                { "Id not stored", NoProductsExists },
                { "Products with id 993a1ad5-7f7a-4a91-91fb-c0ee62755a2d exist", ProductWithIdExist}
            };
        }
        //
        //se fosse um banco de verdade como eu faria pra criar esses dados mocados? com o setup do MOQ da vida?
        //                                                          \/

        private void ProductsExists()
        {
            var fakeData = new List<GetSomeDataByIdResponse>
            {
                new() {
                    Id = new Guid("993a1ad5-7f7a-4a91-91fb-c0ee62755a2d"),
                    Name = "Name1",
                    LastName = "LastName1",
                    Age = "Age1",
                },
                new() {
                    Id = new Guid("8518eb80-c2c4-4c1f-8573-905af0d7f3e2"),
                    Name = "Name2",
                    LastName = "LastName2",
                    Age = "Age3",
                }
            };

            this._fakeDatabase.OurFakeData = fakeData;
        }

        private void ProductWithIdExist()
        {
            var fakeData = new List<GetSomeDataByIdResponse>
            {
                new() {
                    Id = new Guid("993a1ad5-7f7a-4a91-91fb-c0ee62755a2d"),
                    Name = "Name1",
                    LastName = "LastName1",
                    Age = "Age1",
                }
            };

            this._fakeDatabase.OurFakeData = fakeData;
        }

        private void NoProductsExists()
        {
            this._fakeDatabase.OurFakeData = null;
        }

        public async Task Invoke(HttpContext context)
        {
            if (context.Request.Path.StartsWithSegments("/provider-states"))
            {
                await HandleProviderStatesRequest(context);
                await context.Response.WriteAsync(string.Empty);
            }
            else
            {
                await this._next(context);
            }
        }

        private async Task HandleProviderStatesRequest(HttpContext context)
        {
            context.Response.StatusCode = (int)HttpStatusCode.OK;

            if (context.Request.Method.ToUpper() == HttpMethod.Post.ToString().ToUpper() &&
               context.Request.Body is not null)
            {
                string jsonRequestBody = String.Empty;
                using (var reader = new StreamReader(context.Request.Body, Encoding.UTF8))
                {
                    jsonRequestBody = await reader.ReadToEndAsync();
                }

                var providerState = JsonConvert.DeserializeObject<ProviderState>(jsonRequestBody);

                if (providerState is not null && !String.IsNullOrEmpty(providerState.State))
                {
                    this._providerStates[providerState.State].Invoke();
                }
            }
        }
    }
}

