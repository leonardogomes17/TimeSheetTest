namespace TimeSheetTeste.Models
{
    public class TimeSheetExportRequestDto
    {
        public int? ClientId { get; set; }
        public bool ShowClient { get; set; }
        public int? ProjectId { get; set; }
        public bool ShowProject { get; set; }
        public bool Prospect { get; set; }
        public bool ShowProspect { get; set; }
        public DateTime? WorkfedDate { get; set; }
        public bool ShowWorkFedDate { get; set; }

        public List<ClientDto> listClient { get; set; } = new List<ClientDto>();
        public List<ProjectDto> listProject { get; set; } = new List<ProjectDto>();
        public List<UserDto> listUser { get; set; } = new List<UserDto>();
    }
}
