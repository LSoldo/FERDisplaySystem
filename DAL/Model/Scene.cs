using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Model
{
    public class Scene
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string SceneType { get; private set; }
        public bool Active { get; set; }
        public List<DataSource> DataSources { get; set; }

        protected Scene() { }

        public Scene(string name, string sceneType, List<DataSource> dataSources )
        {
            this.Name = name;
            this.SceneType = sceneType;
            this.DataSources = dataSources;
            this.Active = true;
        }


    }
}
