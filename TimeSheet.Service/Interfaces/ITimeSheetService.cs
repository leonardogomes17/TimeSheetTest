using TimeSheetTeste.Models;

namespace TimeSheetTest.Service.Interfaces
{
    public interface ITimeSheetService
    {
        Task<bool> InsertTimeSheet(TimeSheetDto dto);

        Task<List<TimeSheetExportDto>> GetTimeSheetExport(TimeSheetExportRequestDto dto);

        List<ClientDto> GetAllClients();

        List<ProjectDto> GetAllProjects();

        List<UserDto> GetAllUsers();

        byte[] ExportExcel(List<TimeSheetExportDto> dto, TimeSheetExportRequestDto dtoRequest);
    }
}
