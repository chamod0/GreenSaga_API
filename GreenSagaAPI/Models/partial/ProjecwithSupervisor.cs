namespace GreenSagaAPI.Models.partial
{
    public class ProjecwithSupervisor
    {
        public int Id { get; set; }
        public string ProjectCode { get; set; }
        public string ProjectName { get; set; }
        public string Description { get; set; }
        public DateTime CreateAt { get; set; }
        public int Createby { get; set; }
        public int UserID { get; set; }
        public int SupervisorID { get; set; }
        public DateTime ModifyAt { get; set; }
        public int ModifyBy { get; set; }
        public DateTime DeleteAt { get; set; }
        public int DeleteBy { get; set; }
        public string? SupervisorName { get; set; }
        public projectStatus Status { get; set; }
    }
}
