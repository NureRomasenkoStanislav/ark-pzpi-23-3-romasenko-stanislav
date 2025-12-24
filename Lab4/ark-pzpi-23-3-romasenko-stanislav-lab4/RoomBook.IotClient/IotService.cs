using Microsoft.AspNetCore.SignalR.Client;
using System.Net.Http;

namespace RoomBook.IotClient
{
    public class IotService
    {
        private readonly HubConnection _connection;
        private readonly DeviceSettings _settings;

        public IotService(DeviceSettings settings)
        {
            _settings = settings;

            _connection = new HubConnectionBuilder()
                .WithUrl(_settings.ServerUrl, options => {
 
                    options.HttpMessageHandlerFactory = (handler) =>
                    {
                        if (handler is HttpClientHandler clientHandler)
                        {
                            clientHandler.ServerCertificateCustomValidationCallback =
                                (message, cert, chain, errors) => true;
                        }
                        return handler;
                    };
                })
                .WithAutomaticReconnect()
                .Build();

            _connection.On<int>("ReceiveUnlockCommand", (roomId) =>
            {
                if (roomId == _settings.RoomId)
                {
                    _ = ExecuteUnlockAsync();
                }
            });
        }

        public async Task StartAsync()
        {
            try
            {
                await _connection.StartAsync();
                Console.WriteLine($"[CONNECTED] Пристрій {_settings.DeviceId} підключено до сервера за адресою: {_settings.ServerUrl}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Помилка підключення: {ex.Message}");
                Console.WriteLine("[HINT] Переконайтеся, що порт у DeviceSettings збігається з портом запущеного API (напр. 7242).");
            }
        }

        private async Task ExecuteUnlockAsync()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"\n[{DateTime.Now:T}] СИГНАЛ: Двері розблоковано для кімнати {_settings.RoomId}!");
            Console.ResetColor();

            await Task.Delay(5000);

            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"[{DateTime.Now:T}] СИГНАЛ: Двері автоматично заблоковано.");
            Console.ResetColor();
        }
    }
}