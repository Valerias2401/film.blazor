using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Prilozshuha;
using Prilozshuha.ApiRequests;
using Prilozshuha.ApiRequests.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped<ApiRequestService>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<MovieRequests>();
builder.Services.AddScoped<Prilozshuha.ApiRequests.ChatService>();

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("http://localhost:5130/") });

await builder.Build().RunAsync();