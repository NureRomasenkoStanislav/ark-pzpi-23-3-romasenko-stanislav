namespace RoomBook.IotClient
{
    public class DeviceSettings
    {
        public string DeviceId { get; set; } = "LOCK_UNIT_01";
        public int RoomId { get; set; } = 1;
        public string ServerUrl { get; set; } = "https://localhost:7242/roomHub";
        public int HeartbeatInterval { get; set; } = 5000; 
    }
}