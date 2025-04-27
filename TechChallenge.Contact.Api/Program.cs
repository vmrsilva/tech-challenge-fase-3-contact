using FluentValidation.AspNetCore;
using FluentValidation;
using Prometheus.SystemMetrics;
using AutoMapper;
using TechChallenge.Contact.Api.Mapper;
using Prometheus;
using TechChallenge.IoC;
using TechChallenge.Contact.Integration.Region;
using Refit;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddValidatorsFromAssemblyContaining<Program>();
builder.Services.AddFluentValidationAutoValidation();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var regionApiUrl = builder.Configuration["Integrations:RegionApiUrl"];

builder.Services.AddRefitClient<IRegionIntegration>().ConfigureHttpClient(c =>
{
    c.BaseAddress = new Uri(regionApiUrl);

});

builder.Services.AddSystemMetrics();

DomainInjection.AddInfraestructure(builder.Services, builder.Configuration);

var mapperConfig = new MapperConfiguration(mc =>
{
    mc.AddProfile(new MappingProfile());
});

IMapper mapper = mapperConfig.CreateMapper();
builder.Services.AddSingleton(mapper);

var app = builder.Build();

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
    app.UseSwagger();
    app.UseSwaggerUI();
//}

app.UseRouting();
app.UseHttpMetrics();
app.UseMetricServer();

//var collector = DotNetRuntimeStatsBuilder.Default().StartCollecting();

app.MapMetrics();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

public partial class Program
{
}
