using RoomBook.IotClient;

var settings = new DeviceSettings();
var iotService = new IotService(settings);

await iotService.StartAsync();

bool exit = false;
while (!exit)
{
    Console.WriteLine("\n--- IoT Client Control Panel ---");
    Console.WriteLine("1. Стан пристрою");
    Console.WriteLine("2. Змінити Room ID (Конфігурація)");
    Console.WriteLine("3. Скинути до заводських налаштувань");
    Console.WriteLine("0. Вихід");
    Console.Write("Оберіть дію: ");

    var choice = Console.ReadLine();
    switch (choice)
    {
        case "1":
            Console.WriteLine($"Пристрій: {settings.DeviceId}, Кімната: {settings.RoomId}, Сервер: {settings.ServerUrl}");
            break;
        case "2":
            Console.Write("Введіть новий номер кімнати: ");
            if (int.TryParse(Console.ReadLine(), out int newId))
            {
                settings.RoomId = newId;
                Console.WriteLine("Налаштування збережено.");
            }
            break;
        case "3":
            settings.RoomId = 1;
            Console.WriteLine("Налаштування скинуто до початкових.");
            break;
        case "0":
            exit = true;
            break;
        default:
            Console.WriteLine("Невірний вибір.");
            break;
    }
}