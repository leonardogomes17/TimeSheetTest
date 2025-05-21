namespace TimeSheetTeste.Models
{
    public class TaskDto
    {
        public int TaskId { get; set; }
        public int ParentTaskId { get; set; }
        public int ClientId { get; set; }
        public string Title { get; set; }
        public string Value { get; set; }
        public int FieldType { get; set; }
        public string FieldName { get; set; } 
    }
}
