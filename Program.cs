using Blog.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<BlogDataContext>();

builder.Services.AddControllers()
.ConfigureApiBehaviorOptions(options =>
{
    options.SuppressModelStateInvalidFilter = true;
});

var app = builder.Build();
app.MapControllers();

app.Run();
