using ButiEngineEditor.Models;
using ButiEngineEditor.Models.Modules;
using Livet;
using Livet.Commands;
using Livet.EventListeners;
using Livet.Messaging;
using Livet.Messaging.IO;
using Livet.Messaging.Windows;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows.Threading;

namespace ButiEngineEditor.ViewModels.Panes
{
    public class HierarchyViewModel : PaneViewModelBase
    {
        private List<string> _list_gameObjects;
        private string _serchString="";
        private string _serchTag="none";
        public List<string> List_gameObjects{  get { return _list_gameObjects; } }
        public string SerchString { get { return _serchString; } }
        public string SerchTag { get { return _serchTag; } }
        #region Title Property
        public override string Title
        {
            get { return "Hierarchy"; }
        }
        #endregion

        #region ContentId Property
        public override string ContentId
        {
            get { return "HierarchyViewModel"; }
        }
        #endregion

        public HierarchyViewModel()
        {
            _list_gameObjects = new List<string>();
        }
        protected override void Dispose(bool disposing)
        {

        }
        public Vector3 ToVector3(JToken token)
        {
            return new Vector3(token[0].Value<float>(), token[1].Value<float>(), token[2].Value<float>());
        }
        public void LoadBlenderJSON(string arg_filename)
        {
            void RecursiveToken(JToken token,string parentName)
            {
                string TokenToStageObjectName(JToken arg_stageobjectToken)
                {

                    if (arg_stageobjectToken["collider"] == null)
                    {
                        return "DecorationBlock";
                    }
                    else
                    {
                        if (arg_stageobjectToken["invisible"] != null)
                        {
                            if (arg_stageobjectToken["invisible"].Value<int>() == 0)
                            {
                                return "StageBlock";
                            }
                            else
                            {
                                return "StageBlock_invisible";
                            }
                        }
                        else
                        {
                            return "StageBlock";
                        }
                    }
                }
                var type = token["type"].ToString();
                string objectName = "";
                if (type != "MESH")
                {
                    
                }
                else
                {
                    float temp;
                    var position= ToVector3(token["transform"]["translation"]);
                    temp = position.Z;
                    position.Z = position.Y;
                    position.Y = temp;

                    var rotation= ToVector3( token["transform"]["rotation"]);
                    var scaling= ToVector3(token["transform"]["scaling"]);
                    temp = scaling.Z;
                    scaling.Z = scaling.Y;
                    scaling.Y = temp;
                    var transform = new Transform() { position = position, rotation = rotation, scaling = scaling };
                    string createObjectName = "";
                    if (token["objecttype"] != null)
                    {
                        var objectType = token["objecttype"].ToString();
                        if (objectType== "Player")
                        {
                            createObjectName = "Player";
                        }
                        else if(objectType== "SpawnPoint")
                        {
                            createObjectName = "SpawnPoint";
                        }
                        else
                        {
                            createObjectName = TokenToStageObjectName(token);
                        }
                    }
                    else
                    {
                        createObjectName = TokenToStageObjectName(token);
                    }
                    if (createObjectName.Length > 0)
                    {
                        objectName = EditorInstances.HierarchyModel.AddGameObjectFromCereal(createObjectName, transform);
                    }
                    ButiEngineIO.SelectGameObject(objectName);
                    if (parentName.Length > 0)
                    {
                        ButiEngineIO.SetTransformBase(parentName);
                    }
                }
                if(token["children"]!=null)
                    token["children"].ToList().ForEach(ch => { RecursiveToken(ch, objectName); });
            }
            if (!File.Exists(arg_filename))
            {
                return;
            }
            using (var reader = new StreamReader(arg_filename, Encoding.UTF8))
            {
                JObject data = JObject.Parse(reader.ReadToEnd());
                data["objects"].ToList().ForEach(obj =>
                {
                    RecursiveToken(obj,"");
                });
                int i = 0;
            }
             
            
        }

    }
}
