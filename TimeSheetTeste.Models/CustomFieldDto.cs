namespace TimeSheetTeste.Models
{
    public class CustomFieldDto
    {
        public int CustomFieldId { get; set; }
        public int FieldType { get; set; }
        public string FieldName { get; set; }
        public TaskFieldDto Field { get; set; }
    }
}
