namespace TimeSheetTeste.Models
{
    public class TaskFieldDto
    {
        public int FieldId { get; set; }
        public int TaskId { get; set; }
        public string Value { get; set; }
        public TaskDto Task { get; set; }

    }
}
