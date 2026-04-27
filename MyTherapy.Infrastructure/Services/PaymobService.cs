
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using MyTherapy.Application.Interfaces;

namespace MyTherapy.Infrastructure.Services;

public class PaymobService : IPaymobService
{
    // HttpClient is intended to be reused throughout the life of an application. Instantiating an HttpClient class for every request will exhaust the number of sockets available under heavy loads. This will result in SocketException errors. 
    private readonly HttpClient _http;
    // IConfiguration is used to access configuration settings, such as API keys and endpoints, from a configuration file (e.g., appsettings.json) or environment variables. This allows for flexible and secure management of sensitive information without hardcoding it in the codebase.
    private readonly IConfiguration _config;

    // Injection
    private string BaseUrl => _config["Paymob:BaseUrl"];
    private string ApiKey => _config["Paymob:ApiKey"];
    private string IntegrationId => _config["Paymob:IntegrationId"];
    private string IframeId => _config["Paymob:IframeId"];

    public PaymobService(HttpClient http, IConfiguration config)
    {
        _http = http;
        _config = config;
    }

    public async Task<string> InitiatePaymentAsync(Guid appointmentId, decimal amount, string customerEmail, string customerName)
    {
        var authToken = await GetAuthTokenAsync();
        var orderId = await CreateOrderAsync(authToken, appointmentId, amount);
        var paymentKey = await GetPaymentKeyAsync(authToken, orderId, amount, customerEmail, customerName);

        return $"https://accept.paymob.com/api/acceptance/iframes/{IframeId}?payment_token={paymentKey}";
    }

    private async Task<string> GetAuthTokenAsync()
    {
        var body = JsonSerializer.Serialize(new { api_key = ApiKey });
        var response = await _http.PostAsync(
                $"{BaseUrl}/auth/tokens",
                new StringContent(body, Encoding.UTF8, "application/json")
            );

        await EnsureSuccessAsync(response, "Auth");

        var json = await response.Content.ReadAsStringAsync();
        var doc = JsonDocument.Parse(json);

        return doc.RootElement.GetProperty("token").GetString()!;
    }

    private async Task<int> CreateOrderAsync(string authToken, Guid appointmentId, decimal amount)
    {
        var body = JsonSerializer.Serialize(new
        {
            auth_token = authToken,
            delivery_needed = false,
            amount_cents = (int)(amount * 100), // Paymob expects amount in cents
            currency = "EGP",
            merchant_order_id = appointmentId.ToString(), // our reference to the appointment
            items = Array.Empty<object>() // No items needed for a simple payment
        });

        var response = await _http.PostAsync(
                $"{BaseUrl}/ecommerce/orders",
                new StringContent(body, Encoding.UTF8, "application/json"));

        await EnsureSuccessAsync(response, "CreateOrder");

        var json = await response.Content.ReadAsStringAsync();
        var doc = JsonDocument.Parse(json);

        return doc.RootElement.GetProperty("id").GetInt32();
    }

    private async Task<string> GetPaymentKeyAsync(
           string authToken, int orderId, decimal amount,
           string customerEmail, string customerName)
    {
        var nameParts = customerName.Split(' ', 2);
        var firstName = nameParts[0];
        var lastName = nameParts.Length > 1 ? nameParts[1] : "-";

        var body = JsonSerializer.Serialize(new
        {
            auth_token = authToken,
            amount_cents = (int)(amount * 100),
            expiration = 3600, // 1 hour
            order_id = orderId,
            billing_data = new
            {
                first_name = firstName,
                last_name = lastName,
                email = customerEmail,
                phone_number = "NA",
                apartment = "NA",
                floor = "NA",
                street = "NA",
                building = "NA",
                shipping_method = "NA",
                postal_code = "NA",
                city = "NA",
                country = "NA",
                state = "NA"
            },
            currency = "EGP",
            integration_id = int.Parse(IntegrationId)
        });

        var response = await _http.PostAsync(
            $"{BaseUrl}/acceptance/payment_keys",
            new StringContent(body, Encoding.UTF8, "application/json"));

        await EnsureSuccessAsync(response, "PaymentKey");

        var json = await response.Content.ReadAsStringAsync();
        var doc = JsonDocument.Parse(json);
        return doc.RootElement.GetProperty("token").GetString()!;
    }

    private async Task EnsureSuccessAsync(HttpResponseMessage response, string step)
    {
        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            throw new Exception($"Paymob error at {step}: {error}");
        }
    }
}
