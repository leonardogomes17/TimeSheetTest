namespace TimeSheetTeste.Models
{
    public class TimeSheetDto
    {
        public int TimesheetId { get; set; }
        public int TaskId { get; set; }
        public int UserId { get; set; }
        public int ProjectId { get; set; }
        public DateTime WorkfedDate { get; set; }
        public TimeSpan TimeSpent { get; set; }
        public List<TaskDto> listTask { get; set; } = new List<TaskDto>();
        public List<ClientDto> listClient { get; set; } = new List<ClientDto>();
        public List<ProjectDto> listProject { get; set; } = new List<ProjectDto>();
        public List<UserDto> listUser { get; set; } = new List<UserDto>();
    }
}
