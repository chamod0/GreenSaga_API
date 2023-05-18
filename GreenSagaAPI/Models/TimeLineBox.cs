using System.ComponentModel.DataAnnotations;

namespace GreenSagaAPI.Models
{
    public class TimeLineBox
    {
        [Key]
        public int Id { get; set; }
        public int ProjectId { get; set; }
        public int FarmerId { get; set; }
        public int SupervisorId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
        public Boolean IsActive { get; set; }
        public DateTimeOffset CreateAt { get; set; }
        public int Createby { get; set; }
        public int UserID { get; set; }
        public DateTimeOffset ModifyAt { get; set; }
        public int ModifyBy { get; set; }
        public DateTimeOffset DeleteAt { get; set; }
        public int DeleteBy { get; set; }
        public timeLineStatus TimeLineStatus { get; set; }
    }
}
