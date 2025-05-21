
using TimeSheetTeste.Models;

namespace TimeSheetTest.Data.Interfaces
{
    public interface ITimeSheetRepository
    {
        Task<bool> InsertTimeSheet(TimeSheetDto dto);

        Task<List<TimeSheetExportDto>> GetTimeSheetExport(TimeSheetExportRequestDto dto);

        List<ClientDto> GetAllClients();

        List<ProjectDto> GetAllProjects();

        List<UserDto> GetAllUsers();

    }
}
