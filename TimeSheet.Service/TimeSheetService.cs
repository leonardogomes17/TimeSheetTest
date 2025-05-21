using System.Data;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using TimeSheetTest.Data.Interfaces;
using TimeSheetTest.Service.Interfaces;
using TimeSheetTeste.Models;

namespace TimeSheetTest.Service
{
    public class TimeSheetService : ITimeSheetService
    {
        private readonly ITimeSheetRepository _timeSheetRepository;
        public TimeSheetService(ITimeSheetRepository timeSheetRepository)
        {
            _timeSheetRepository = timeSheetRepository;
        }

        public async Task<bool> InsertTimeSheet(TimeSheetDto dto) 
        {
            return await _timeSheetRepository.InsertTimeSheet(dto);
        }

        public async Task<List<TimeSheetExportDto>> GetTimeSheetExport(TimeSheetExportRequestDto dto)
        {
            return await _timeSheetRepository.GetTimeSheetExport(dto);
        }

        public byte[] ExportExcel(List<TimeSheetExportDto> dto, TimeSheetExportRequestDto dtoRequest)
        { 
            DataTable tableExcel = new DataTable("TimeSheet");

            if (dtoRequest.ShowClient)
            {
                tableExcel.Columns.Add("Client", typeof(string));
                tableExcel.Columns.Add("Client_Name", typeof(string));
            }

            if (dtoRequest.ShowProject)
            {
                tableExcel.Columns.Add("Project", typeof(string));
                tableExcel.Columns.Add("Project Name", typeof(string));
            }

            if (dtoRequest.ShowWorkFedDate)
            {
                tableExcel.Columns.Add("WorkFedDate", typeof(string));
            }

            tableExcel.Columns.Add("Total Time Spent", typeof(string));

            dto.ForEach(timeSheet =>
                {
                    int columnIndex = 0;
                    var newLine = tableExcel.Rows.Add();

                    if (dtoRequest.ShowClient)
                    {
                        newLine[columnIndex] =  timeSheet.ClientId;
                        columnIndex++;
                        newLine[columnIndex] = timeSheet.ClientName;
                        columnIndex++;
                    }

                    if (dtoRequest.ShowProject)
                    {
                        newLine[columnIndex] = timeSheet.ProjectId;
                        columnIndex++;
                        newLine[columnIndex] = timeSheet.ProjectName;
                        columnIndex++;
                    }

                    if (dtoRequest.ShowWorkFedDate)
                    {
                        newLine[columnIndex] = timeSheet.WorkfedDate?.ToString("dd/MM/yyyy");
                        columnIndex++;
                    }

                    newLine[columnIndex] = timeSheet.TotalTimeSpent;
                }
            );

            using (XLWorkbook workBook = new XLWorkbook())
            {
                workBook.AddWorksheet(tableExcel, "TimeSheet");

                using (MemoryStream memoryStream = new MemoryStream())
                {
                    workBook.SaveAs(memoryStream);
                    memoryStream.Position = 0;
                    return memoryStream.ToArray();
                }
            }
        }

        public List<ClientDto> GetAllClients() 
        {
            return _timeSheetRepository.GetAllClients();
        }

        public List<ProjectDto> GetAllProjects() 
        {
            return _timeSheetRepository.GetAllProjects();
        }

        public List<UserDto> GetAllUsers()
        {
            return _timeSheetRepository.GetAllUsers();
        }

    }
}
