using GreenSagaAPI.Models;

namespace GreenSagaAPI.Service
{
    public class projectService
    {
        public List<cultivationProjects> GetCultivationProjects()
        {
            var project = new List<cultivationProjects>();

            var pro1 = new cultivationProjects
            {
                Id = 1,
                ProjectCode = "a",
                ProjectName = "aaa",
                Description = "aaaaaa",
                CreateAt = DateTime.Now,
                Createby = 1,
                Status = projectStatus.Inprogres


            };
            project.Add(pro1);
            var pro2 = new cultivationProjects
            {
                Id = 2,
                ProjectCode = "da",
                ProjectName = "aaffa",
                Description = "aaa",
                CreateAt = DateTime.Now,
                Createby = 1,
                Status = projectStatus.Inprogres


            };
            project.Add(pro2);
            return project;

        }
    }
}
