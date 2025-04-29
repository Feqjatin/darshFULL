namespace trySupa.Models
{
    public class ExamInputModel
    {
        public string Subject { get; set; }
        public DateTime ExamDate { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
    }
    public class ExamPreviewViewModel
    {
        public string ClassName { get; set; }
        public string Department { get; set; }
        public string ExamType { get; set; }
        public List<ExamInputModel> Timetable { get; set; }
        public bool IsPdf { get; set; }
    }
    public class SubjectRequest
    {
        public string ClassName { get; set; }
        public string Department { get; set; }
    }
}
