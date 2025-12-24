namespace RoomBook.Core.Interfaces
{
    public interface IRoomHub
    {
        Task ReceiveUnlockCommand(int roomId);
    }
}