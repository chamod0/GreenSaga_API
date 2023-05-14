namespace GreenSagaAPI.Models
{
    public class cultivationProjects
    {
        public int Id { get; set; }
        public string ProjectCode { get; set; }
        public string ProjectName { get; set; }    
        public string Description { get; set; }
        public DateTime CreateAt { get; set; }
        public int Createby { get; set; }
        public int UserID { get; set; }
        public DateTime ModifyAt { get; set; }
        public DateTime ModifyBy { get; set; }
        public projectStatus Status { get; set; }

        



    }
}
