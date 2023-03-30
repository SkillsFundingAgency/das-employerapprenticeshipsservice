using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Builder;
using Microsoft.Identity.Web;
using SFA.DAS.EAS.Account.Api.AuthPolicies;
using SFA.DAS.EAS.Domain.Configuration;
using Microsoft.Extensions.Configuration;
using SFA.DAS.EAS.Application.Services.EmployerAccountsApi.Http;
using SFA.DAS.NLog.Logger;
using SFA.DAS.EAS.Application.Services.EmployerAccountsApi;
using SFA.DAS.HashingService;
using AutoMapper;
using System;

var builder = WebApplication.CreateBuilder(args);

//builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
//    .AddMicrosoftIdentityWebApi(builder.Configuration.GetSection("AzureAd"));

builder.Services.AddControllersWithViews()
    .AddNewtonsoftJson(opts => opts.UseMemberCasing());

//builder.Services.AddSingleton<IAuthorizationHandler, LoopBackHandler>();
//builder.Services.AddAuthorization(options => 
//{
//    options.AddPolicy("LoopBack", policy =>
//        {
//            policy.Requirements.Add(new LoopBackRequirement());
//        }
//    );
//});


builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddSingleton(builder.Configuration.GetSection("EmployerAccountsApi").Get<EmployerAccountsApiConfiguration>());
builder.Services.AddSingleton(builder.Configuration.GetSection("EmployerFinanceApi").Get<EmployerFinanceApiConfiguration>());
builder.Services.AddSingleton<IEmployerAccountsApiHttpClientFactory, EmployerAccountsApiHttpClientFactory>();
builder.Services.AddSingleton<IEmployerAccountsApiService, EmployerAccountsApiService>();

var hashstringChars = builder.Configuration.GetValue<string>("AllowedHashstringCharacters");
var hashstring = builder.Configuration.GetValue<string>("Hashstring");

builder.Services.AddSingleton<IHashingService, HashingService>(c => new HashingService(hashstringChars, hashstring));


builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseStaticFiles();
app.UseRouting();
//app.UseAuthorization();
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");
});

app.Run();
