using System.ComponentModel.DataAnnotations;

namespace GreenSagaAPI.Models
{
    public class cultivationProjects
    {
        [Key]
        public int Id { get; set; }
        public string ProjectCode { get; set; }
        public string ProjectName { get; set; }    
        public string Description { get; set; }
        public string Image { get; set; }
        public DateTimeOffset CreateAt { get; set; }
        public int Createby { get; set; }
        public int UserID { get; set; }
        public int SupervisorID { get; set; }
        public DateTimeOffset ModifyAt { get; set; }
        public int ModifyBy { get; set; }
        public DateTimeOffset DeleteAt { get; set; }
        public int DeleteBy { get; set; }
        public projectStatus Status { get; set; }

        



    }
}
