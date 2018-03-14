using UnityEngine;
using System.Collections;
using System.Xml.Linq;
using System.Xml;
using System.IO;
using System;
using System.Collections.Generic;
using System.Linq;  
  
/// <summary>  
/// By:略知CSharp   
//</strong></span><span style = "font-family:Microsoft YaHei;font-size:14px;color:#006600;" >< strong >/// </summary>  
public class HelperXML : MonoBehaviour {
    [HideInInspector]
    public string filePath = string.Empty;

    void Awake() {
        //此属性用于返回程序的数据文件所在文件夹的路径。例如在Editor中就是Assets了。  
        filePath = Application.dataPath + @"/XML/score.xml";
    }

    /// <summary>  
    /// 1.遍历读取XML  
    /// </summary>  
    /// <returns></returns>  
    public List<GameInfoModel> VCLoadXMLManage() {
        XElement root = XElement.Load(filePath);
        List<GameInfoModel> list = new List<GameInfoModel>();
        foreach (var item in root.Element("Group").Elements("Item"))    //遍历XElement  
        {
            list.Add(new GameInfoModel {
                Id = item.Element("ID").Value,
                Score = Convert.ToInt32(item.Element("Score").Value),
                Time = Convert.ToInt32(item.Element("Time").Value)
            }
           );
        }
        return list;
    }
    /// <summary>  
    /// 2.删除第三级所有节点   
    /// </summary>  
    public void VCRemoveXmlElement() {
        XElement root = XElement.Load(filePath);
        XElement node = root.Descendants().Where(p => p.Name == "Group").Last();
        node.Elements().Remove();
        root.Save(filePath);
    }
    /// <summary>  
    /// 3.添加第三级  
    /// </summary>  
    /// <param name="list"></param>  
    public void VCAddXml(List<GameInfoModel> list) {
        XElement root = XElement.Load(filePath);
        XElement node = root.Descendants().Where(p => p.Name == "Group").Last();
        for (int i = 0, count = list.Count; i < count; i++) {
            XElement child = new XElement("Item",
                       new XElement("ID", list[i].Id),
                       new XElement("Score", list[i].Score),
                       new XElement("Time", list[i].Time)
                   );
            node.Add(child);
        }
        root.Save(filePath);
    }

}