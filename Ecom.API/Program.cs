using Ecom.API.Jobs;
using Ecom.API.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Quartz;
using Telegram.Bot;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<ITelegramBotClient>(new TelegramBotClient("7154240714:AAGr9-Fcxm0ZF_Ohx8OOWWhXyR6raA82rgA"));
builder.Services.AddHttpClient();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
       options.UseMySQL(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddSwaggerGen(option =>
{
    option.SwaggerDoc("v1", new OpenApiInfo { Title = "Demo API", Version = "v1" });
    //option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    //{
    //    In = ParameterLocation.Header,
    //    Description = "Please enter a valid token",
    //    Name = "Authorization",
    //    Type = SecuritySchemeType.Http,
    //    BearerFormat = "JWT",
    //    Scheme = "Bearer"
    //});
    option.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type=ReferenceType.SecurityScheme,
                    Id="Bearer"
                }
            },
            new string[]{}
        }
    });
});

builder.Services.AddQuartz(q =>
{
    q.UseMicrosoftDependencyInjectionJobFactory();

    //#region Поставки

    //var jobKeyLoadIncomes = new JobKey("LoadIncomes");
    //q.AddJob<LoadIncomes>(opts => opts.WithIdentity(jobKeyLoadIncomes));

    //q.AddTrigger(opts => opts
    //.ForJob(jobKeyLoadIncomes)
    //.WithIdentity($"{jobKeyLoadIncomes}-trigger")
    //.StartNow()
    //.WithSimpleSchedule(x => x
    //.WithIntervalInMinutes(30)
    //.RepeatForever()
    //.Build())
    //);
    //#endregion

    //#region Склад

    //var jobKeyLoadStocks = new JobKey("LoadStocks");
    //q.AddJob<LoadStocks>(opts => opts.WithIdentity(jobKeyLoadStocks));

    //q.AddTrigger(opts => opts
    //.ForJob(jobKeyLoadStocks)
    //.WithIdentity($"{jobKeyLoadStocks}-trigger")
    //.StartNow()
    //.WithSimpleSchedule(x => x
    //.WithIntervalInMinutes(60)
    //.RepeatForever()
    //.Build())
    //);
    //#endregion

    #region Заказы

    var jobKeyLoadOrders = new JobKey("LoadOrders");
    q.AddJob<LoadOrders>(opts => opts.WithIdentity(jobKeyLoadOrders));

    q.AddTrigger(opts => opts
    .ForJob(jobKeyLoadOrders)
    .WithIdentity($"{jobKeyLoadOrders}-trigger")
    .StartNow()
    .WithSimpleSchedule(x => x
    .WithIntervalInMinutes(30)
    .RepeatForever()
    .Build())
    );
    #endregion

    //#region Юнит

    //var jobKeyLoadUnits = new JobKey("LoadUnits");
    //q.AddJob<LoadUnits>(opts => opts.WithIdentity(jobKeyLoadUnits));

    //q.AddTrigger(opts => opts
    //.ForJob(jobKeyLoadUnits)
    //.WithIdentity($"{jobKeyLoadUnits}-trigger")
    //.StartNow()
    //.WithSimpleSchedule(x => x
    //.WithIntervalInMinutes(30)
    //.RepeatForever()
    //.Build())
    //);
    //#endregion

    //  #region Рекламные кампании
    //  var jobKeyLoadAdverts = new JobKey("LoadAdverts");
    //  q.AddJob<LoadAdverts>(opts => opts.WithIdentity(jobKeyLoadAdverts));
    //  q.AddTrigger(opts => opts
    //  .ForJob(jobKeyLoadAdverts)
    //  .WithIdentity($"{jobKeyLoadAdverts}-trigger")
    //  .StartNow()
    //  .WithSimpleSchedule(x => x
    //  .WithIntervalInHours(12)
    //  .RepeatForever()
    //  .Build())
    //  );
    //  #endregion

    //  #region Карточки

    //  var jobKeyLoadCardsWildberries = new JobKey("LoadCardsWildberries");
    //  q.AddJob<LoadCards>(opts => opts.WithIdentity(jobKeyLoadCardsWildberries));

    //  q.AddTrigger(opts => opts
    // .ForJob(jobKeyLoadCardsWildberries)
    // .WithIdentity($"{jobKeyLoadCardsWildberries}-trigger-now")
    // .StartNow()
    // .WithSimpleSchedule(x => x.Build())
    // );

    //  q.AddTrigger(opts => opts
    //  .ForJob(jobKeyLoadCardsWildberries)
    //  .WithIdentity($"{jobKeyLoadCardsWildberries}-trigger")
    //  .WithCronSchedule("0 1 0 * * ?")
    //  .StartNow()
    //   );
    //  #endregion

    //  #region Конкуренты

    //  var jobKeyLoadCompetitors = new JobKey("LoadCompetitors");
    //  q.AddJob<LoadCompetitors>(opts => opts.WithIdentity(jobKeyLoadCompetitors));

    //  q.AddTrigger(opts => opts
    //.ForJob(jobKeyLoadCompetitors)
    //.WithIdentity($"{jobKeyLoadCompetitors}-trigger-now")
    //.StartNow()
    //.WithSimpleSchedule(x => x.Build())
    //);

    //  q.AddTrigger(opts => opts
    //      .ForJob(jobKeyLoadCompetitors)
    //      .WithIdentity($"{jobKeyLoadCompetitors}-trigger")
    //      .WithCronSchedule("0 1 0 * * ?")
    //      );
    //  #endregion

});


builder.Services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);
builder.Services.AddScoped<IDataRepository, DataRepository>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
