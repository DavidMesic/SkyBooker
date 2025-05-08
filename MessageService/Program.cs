using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Serilog;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

var builder = WebApplication.CreateBuilder(args);

// Serilog konfigurieren
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateLogger();
builder.Host.UseSerilog();

// unseren Background‐Service registrieren
builder.Services.AddHostedService<RabbitListenerService>();

var app = builder.Build();
// RunAsync statt Run, damit Main async bleiben kann
await app.RunAsync();


// ────────────────────────────────────────────────────────────────────────────────
// Innerhalb von Program.cs: RabbitListenerService
// ────────────────────────────────────────────────────────────────────────────────
public class RabbitListenerService : BackgroundService
{
    private readonly IConfiguration _config;
    private readonly ILogger<RabbitListenerService> _logger;

    public RabbitListenerService(IConfiguration config, ILogger<RabbitListenerService> logger)
    {
        _config = config;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // 1) ConnectionFactory und Connection asynchron anlegen
        var factory = new ConnectionFactory
        {
            HostName = _config["RabbitMq:Host"]!,
            UserName = _config["RabbitMq:Username"]!,
            Password = _config["RabbitMq:Password"]!
        };

        using var connection = await factory.CreateConnectionAsync(stoppingToken);
        // 2) Channel asynchron anlegen: Null-Options plus CancellationToken
        using var channel = await connection.CreateChannelAsync(
            options: null,
            cancellationToken: stoppingToken
        );

        // 3) Queue deklarieren
        await channel.QueueDeclareAsync(
            queue: _config["RabbitMq:Queue"]!,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null
        );

        // 4) AsyncEventingBasicConsumer konfigurieren
        var consumer = new AsyncEventingBasicConsumer(channel);
        consumer.ReceivedAsync += async (model, ea) =>
        {
            var json = Encoding.UTF8.GetString(ea.Body.ToArray());
            _logger.LogInformation("Event erhalten: {json}", json);

            try
            {
                var evt = JsonSerializer.Deserialize<BookingCreatedEvent>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (evt is not null)
                    await SendWhatsAppNotificationAsync(evt);

                // 5) Manuelles Ack
                channel.BasicAckAsync(ea.DeliveryTag, multiple: false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Fehler beim Verarbeiten der Nachricht");
                // optional: await channel.BasicNackAsync(ea.DeliveryTag, multiple: false, requeue: true);
            }
        };

        // 6) Asynchron starten
        await channel.BasicConsumeAsync(
            queue: _config["RabbitMq:Queue"]!,
            autoAck: false,
            consumer: consumer
        );

        // 7) Bis zum Stopp durchlaufen lassen
        await Task.Delay(Timeout.Infinite, stoppingToken);
    }

    private Task SendWhatsAppNotificationAsync(BookingCreatedEvent evt)
    {
        // Statt builder.Configuration: Environment-Variablen auslesen
        var sid = Environment.GetEnvironmentVariable("TWILIO_ACCOUNT_SID");
        var token = Environment.GetEnvironmentVariable("TWILIO_AUTH_TOKEN");
        var from = _config["Twilio:FromWhatsApp"];

        if (string.IsNullOrWhiteSpace(sid) || string.IsNullOrWhiteSpace(token))
            throw new InvalidOperationException("Twilio-Credentials fehlen!");

        TwilioClient.Init(sid, token);

        var message = MessageResource.Create(
            body: evt.TicketCount > 1
                ? $"Deine Buchung {evt.BookingId} für Flug {evt.FlightId} mit {evt.TicketCount} Tickets war erfolgreich!"
                : $"Deine Buchung {evt.BookingId} für Flug {evt.FlightId} war erfolgreich!",
            from: new PhoneNumber(from),
            to: new PhoneNumber($"whatsapp:{evt.PassengerPhoneNumber}")
        );

        _logger.LogInformation("WhatsApp gesendet: SID={sid}", message.Sid);
        return Task.CompletedTask;
    }
}


    // DTO für das Event
    public record BookingCreatedEvent(
    int BookingId,
    string FlightId,
    int TicketCount,
    string PassengerPhoneNumber
);
