namespace TimeSheetTeste.Models
{
    public class TimeSheetExportDto
    {
        public int? ClientId { get; set; }
        public string ClientName { get; set; }
        public int? ProjectId { get; set; }
        public string ProjectName { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; }
        public DateTime? WorkfedDate { get; set; }
        public string TotalTimeSpent { get; set; }
    }
}
