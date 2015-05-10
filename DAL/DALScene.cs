using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.Model;
using System.Data.Entity;

namespace DAL
{
    public class DALScene : IDisposable
    {
        private DisplayContext context;

        public DALScene()
        {
            this.context = new DisplayContext();
        }

        public int AddScene(Scene scene)
        {
            try
            {
                if (scene == null)
                    return -1;
                this.context.Scenes.Add(scene);
                this.context.SaveChanges();
                return scene.Id;
            }
            catch (Exception ex)
            {
                throw new Exception("Error while adding new scene: " + ex.Message);
            }
        }

        public void UpdateScene(Scene scene)
        {
            try
            {
                var oldScene = this.context.Scenes
                    .Include(s => s.DataSources)
                    .SingleOrDefault(x => x.Id == scene.Id);

                if (oldScene == null)
                    throw new Exception("Scene not found");

                this.context.Entry(oldScene).CurrentValues.SetValues(scene);
                if (oldScene.DataSources != null)
                    this.context.DataSources.RemoveRange(oldScene.DataSources);
                oldScene.DataSources = scene.DataSources;
                this.context.SaveChanges();

            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Error while updating scene, id:{0}, error message: {1} ", scene.Id, ex.Message));
            }
        }

        public void SetSceneActiveProperty(int id, bool active)
        {
            try
            {
                var oldScene = this.context.Scenes.Find(id);

                if (oldScene == null)
                    throw new Exception("Scene not found");

                oldScene.Active = active;
                this.context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Error while changing scene Active property, id:{0}, error message: {1} ", id, ex.Message));
            }
        }
        public Scene GetSceneById(int id)
        {
            try
            {
                return this.context.Scenes
                    .Include(x => x.DataSources)
                    .SingleOrDefault(x => x.Id == id);
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Error while getting scene by id: {0}, error message: {1} ", id, ex.Message));
            }
        }

        public List<Scene> GetAllScenes()
        {
            try
            {
                return this.context.Scenes
                    .Include(x => x.DataSources)
                    .ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Error while getting active scenes, error message: {0} ", ex.Message));
            }
        }
        public List<Scene> GetSceneByActiveProperty(bool isActive)
        {
            try
            {
                return this.context.Scenes.Where(s => s.Active == isActive)
                    .Include(x => x.DataSources)
                    .ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Error while getting active scenes, error message: {0} ", ex.Message));
            }
        }

        public List<Scene> GetSceneByType(string type)
        {
            try
            {
                return this.context.Scenes.Where(s => s.SceneType == type)
                    .Include(x => x.DataSources)
                    .ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Error while getting scenes by type, error message: {0} ", ex.Message));
            }
        }

        public int AddSequenceScene(SequenceScene scene)
        {
            try
            {
                if (scene == null)
                    return -1;
                this.context.SequenceScenes.Add(scene);
                this.context.SaveChanges();
                return scene.Id;
            }
            catch (Exception ex)
            {
                throw new Exception("Error while adding new sequence scene: " + ex.Message);
            }
        }

        public void UpdateSequenceScene(SequenceScene newScene)
        {
            try
            {
                var oldSequenceScene = this.context.SequenceScenes
                    .Include(s => s.Scene)
                    .SingleOrDefault(x => x.Id == newScene.Id);

                if (oldSequenceScene == null)
                    throw new Exception("Sequence scene not found");
                if (newScene.Scene == null)
                    throw new Exception("Sequence scene has no associated scene");

                this.context.Entry(oldSequenceScene).CurrentValues.SetValues(newScene);
                this.context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Error while updating sequence scene, id:{0}, error message: {1} ", newScene.Id, ex.Message));
            }
        }

        public SequenceScene GetSequenceSceneById(int id)
        {
            try
            {
                return this.context.SequenceScenes
                    .Include(x => x.Scene)
                    .SingleOrDefault(x => x.Id == id);
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Error while getting sequence scene by id, id:{0}, error message: {1} ", id, ex.Message));
            }
        }

        public List<SequenceScene> GetSequenceSceneBySceneType(string sceneType)
        {
            try
            {
                return this.context.SequenceScenes
                    .Where(x => x.Scene.SceneType == sceneType)
                    .ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Error while getting sequence scene by scene type,  error message: {0} ", ex.Message));
            }
        }
        public void RemoveSequenceScene(int id)
        {
            try
            {
                var sequenceScene = new SequenceScene(){Id = id};
                this.context.SequenceScenes.Attach(sequenceScene);
                this.context.SequenceScenes.Remove(sequenceScene);
                this.context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Error while deleting sequence scene by id, id:{0}, error message: {1} ", id, ex.Message));
            }
        }
    
        public void Dispose()
        {
            this.context.Dispose();
        }
    }
}
