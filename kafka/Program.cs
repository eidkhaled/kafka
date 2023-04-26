using Confluent.Kafka;
using kafka;
using kafta.Services;

var builder = WebApplication.CreateBuilder(args);


// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();



// Add Kafka consumer
var consumerConfig = new ConsumerConfig
{
    BootstrapServers = "your-kafka-broker-url:9092",
    GroupId = "ConsumerGroupId"
};
builder.Services.AddSingleton(new ConsumerBuilder<Ignore, string>(consumerConfig).Build());

// Add Kafka producer
var producerConfig = new ProducerConfig
{
    BootstrapServers = "BootstrapServers:9092"
};
builder.Services.AddSingleton(new ProducerBuilder<Null, string>(producerConfig).Build());
//builder.Services.AddHostedService<KafkaService>();
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
