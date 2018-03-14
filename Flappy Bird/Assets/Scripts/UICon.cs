using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Text;


/// <summary>  
/// UI管理器  
/// </summary>  
public class UICon : HelperXML {
    public GameObject UI_Info; //游戏结束图片//计分板   
    public Text[] countText;     //计分板  

    public List<GameInfoModel> GetXMLInfo() {
        List<GameInfoModel> list = base.VCLoadXMLManage();
        return list;
    }
    public float GetBestScore() {
        List<GameInfoModel> list = base.VCLoadXMLManage();
        if (list.Count == 0) {
            return 0;
        }
        return list[0].Score;
    }

    public void ClearRankList() {
        foreach (var x in countText) {
            x.text = string.Empty;
        }
    }

    public void UpdateRankList() {
        //遍历获取XML  
        List<GameInfoModel> list = base.VCLoadXMLManage();
        //当前分数  
        int nowScore = (int)FindObjectOfType<GameController>().score;
        //添加当前分数信息  
        var mod = new GameInfoModel {
            Id = FindObjectOfType<GameController>().playerID,
            Score = nowScore,
            Time = FindObjectOfType<GameController>().surviveTime
        };
        list.Add(mod);
        //倒序  
        list = list.OrderByDescending(p => p.Score).ThenBy(p => p.Time).ToList();
        //删除最后一个最少的  
        if (list.Count > 5)
            list.RemoveAt(list.Count - 1);

        base.VCRemoveXmlElement();
        base.VCAddXml(list);
    }

    public void ShowRankList() {
        List<GameInfoModel> list = base.VCLoadXMLManage();
        //StringBuilder sb = new StringBuilder();
        //list.ForEach(p => sb.Append(
        //    p.Id + "    " + p.Score + "    " + p.Time + "\n")
        //    );
        //countText.text = sb.ToString();
        list = list.OrderByDescending(p => p.Score).ThenBy(p => p.Time) .ToList();
        for (int i = 0; i < list.Count; i++) {
            countText[i].text = list[i].Id;
            countText[i + 5].text = list[i].Score.ToString();
            countText[i + 10].text = list[i].Time.ToString() + "″";
        }
        UI_Info.SetActive(true);
    }

    public void UpdateAndShowRankList() {
        //显示记分板  
        UI_Info.SetActive(true);
        //遍历获取XML  
        List<GameInfoModel> list = base.VCLoadXMLManage();
        //当前分数  
        int nowScore = (int)FindObjectOfType<GameController>().score;
        //添加当前分数信息  
        var mod = new GameInfoModel {
            Id = FindObjectOfType<GameController>().playerID,
            Score = nowScore,
            Time = FindObjectOfType<GameController>().surviveTime
        };
        list.Add(mod);
        //倒序  
        list = list.OrderByDescending(p => p.Score).ThenBy(p => p.Time).ToList();
        //删除最后一个最少的  
        if (list.Count > 5)
            list.RemoveAt(list.Count - 1);
        //显示最新排名版  
        //StringBuilder sb = new StringBuilder();
        //list.ForEach(p => sb.Append(p.Score == mod.Score ?
        //    ("<color=#ff0000ff>" + p.Id + "    " + p.Score + "    " + p.Time + "</color>\n") :
        //    (p.Id + "    " + p.Score + "    " + p.Time + "\n"))
        //    );
        //countText.text = sb.ToString();
        for (int i = 0; i < list.Count; i++) {
            countText[i].text = list[i].Id;
            countText[i + 5].text = list[i].Score.ToString();
            countText[i + 10].text = list[i].Time.ToString();
        }
        //更新XML  
        base.VCRemoveXmlElement();
        base.VCAddXml(list);
    }

    public void BackClicked() {
        UI_Info.SetActive(false);
        ClearRankList();
    }
}