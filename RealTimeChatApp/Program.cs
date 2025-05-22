using Microsoft.AspNetCore.Identity;
using RealTimeChatApp.Pages;
using RealTimeChatAppDAL.Repos;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddAuthentication("MyCookieAuth")
    .AddCookie("MyCookieAuth", options =>
    {
        options.Cookie.Name = "UserLoginCookie";
        options.LoginPath = "/Register"; 
        options.ExpireTimeSpan = TimeSpan.FromDays(14); 
        options.SlidingExpiration = true;
    });
builder.Services.AddScoped<ChatRepo>();
builder.Services.AddScoped<UserRepo>();
builder.Services.AddScoped<MessageRepo>();
builder.Services.AddSignalR();
builder.Services.AddAuthorization();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();
app.MapHub<ChatHub>("/chathub");
app.MapRazorPages();

app.Run();
