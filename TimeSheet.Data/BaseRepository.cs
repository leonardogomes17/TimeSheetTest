namespace TimeSheetTest.Data
{
    public abstract class BaseRepository
    {
        protected readonly DatabaseConnection _connection;

        protected BaseRepository(DatabaseConnection connection)
        {
            _connection = connection;
        }
    }
}
