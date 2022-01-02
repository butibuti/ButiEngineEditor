using Livet;
using Newtonsoft.Json;
using System.IO;
using System.Text;
namespace ButiEngineEditor.Models
{
    enum WindowPopType { 
        normal=1,max=3
    }

    public class ProjectSettingsModel: NotificationObject
    {
        public string projFilePath;
        public string resourceDir;
        public string projectName;
        public ProjectSettingsModel(string arg_filePath)
        {
            projFilePath = arg_filePath;
            
            FileInput();
        }
        public void FileInput()
        {
            if (!File.Exists(projFilePath))
            {
                resourceDir = "Resources\\";
                projectName = "Sample!";
                return;
            }

            using (var reader = new StreamReader(projFilePath, Encoding.UTF8))
            {
                var deserializeData = JsonConvert.DeserializeObject<ProjectData>(reader.ReadToEnd());

                resourceDir = deserializeData.resourceDir;
                projectName = deserializeData.projectName;
            }
        }
        public void FileOutput()
        {
            var data = new ProjectData();
            data.resourceDir = resourceDir;
            data.projectName= projectName;

            string jsonStr = JsonConvert.SerializeObject(data);

            using (var writer = new StreamWriter(projFilePath, false, Encoding.UTF8))
            {
                writer.Write(jsonStr);
            }
            
        }

        public string GetResourceAbsoluteDirectory()
        {
            return GetProjFilePathDirectory ()+ resourceDir;
        }
        public string GetProjFilePathDirectory()
        {
            var splitedProjectPath = projFilePath.Split('\\');
            return App.GetArgments()[0].Replace(splitedProjectPath[splitedProjectPath.Length - 1], "");
        }
        [JsonObject("ProjectData")]
        class ProjectData
        {
            [JsonProperty("resourceDir")]
            public string resourceDir;
            [JsonProperty("projectName")]
            public string projectName;
        }
    }
    class ApplicationInitData {
        public string windowName;
        public string initSceneName;
        public WindowPopType windowPop;
        public int windowWidth;
        public int windowHeight;
        public bool isFullScreen;
    }

}
