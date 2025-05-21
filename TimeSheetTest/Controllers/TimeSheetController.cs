using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;
using TimeSheetTest.Service.Interfaces;
using TimeSheetTeste.Models;

namespace TimeSheetTest.Controllers
{
    public class TimeSheetController : Controller
    {
        private readonly ITimeSheetService _timeSheetService;   
        public TimeSheetController(ITimeSheetService timeSheetService)
        {
            _timeSheetService = timeSheetService;
        }
        public IActionResult Index()
        {
            var sheetDto = new TimeSheetDto();
            sheetDto.listClient = _timeSheetService.GetAllClients();
            sheetDto.listProject = _timeSheetService.GetAllProjects();
            sheetDto.listUser = _timeSheetService.GetAllUsers();
            return View(sheetDto);
        }

        [HttpPost]
        public async Task<bool> InsertTimeSheet(string dto)
        {
            
            var objSheetDto = JsonSerializer.Deserialize<TimeSheetDto>(dto);

            bool result = false;
            if (ModelState.IsValid)
            {
                result = await _timeSheetService.InsertTimeSheet(objSheetDto);
            }
            return result;
        }

        public IActionResult Report()
        {
            var sheetExport = new TimeSheetExportRequestDto();
            sheetExport.listClient =  _timeSheetService.GetAllClients();

            sheetExport.listClient.Add(new ClientDto() { ClientId = 0 , ClientName =  "All" });

            sheetExport.listProject =  _timeSheetService.GetAllProjects();

            sheetExport.listProject.Add(new ProjectDto() { ProjectId = 0, ProjectName = "All" });

            sheetExport.listUser =  _timeSheetService.GetAllUsers();
            return View(sheetExport);
        }

        [HttpPost]
        public async Task<IActionResult> ExportTimeSheet(TimeSheetExportRequestDto dto)
        {
            if (ModelState.IsValid)
            {
                var returnTimeSheetList =  await _timeSheetService.GetTimeSheetExport(dto);
                var file = _timeSheetService.ExportExcel(returnTimeSheetList, dto);
                return File(file, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "TimeSheet.xlsx");
            }

            return View();
        }
    }
}
