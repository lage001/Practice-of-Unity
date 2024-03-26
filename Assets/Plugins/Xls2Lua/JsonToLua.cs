// using System;
// using System.Collections.Generic;
// using UnityEditor;
// using UnityEngine;
//
// #if UNITY_EDITOR
// public class JsonToLua : Editor
// {
//     private static List<int> spacePosList = new List<int>();
//     private static string recordLevelName ;
//     private static string  recordPlotDesc;
//     
//
//     [MenuItem("Assets/JsonToLua")]
//     private static void ConvertToLua()
//     {
//
//         var Select = Selection.activeObject;
//         var path = AssetDatabase.GetAssetPath(Select);
//         Debug.Log("ConvertToLua "+path);
//         var jsonStr = AssetDatabase.LoadAssetAtPath<TextAsset>(path);
//         string sb = ConvertLua(jsonStr.text);
//         string folder = "/FPS/Resources/Lua/Configs/Levels/";
//         string path2 = $"{folder}{Select.name}.txt";
//         FileStaticAPI.Write(path2, sb);
//     }
//
//     public static void InputJsonPathToLua(string fileName,string extra =null) //extra为额外内容  当前Json未添加 plotEvents 内容  直接在lua中添加
//     {
//         var jsPath = "Assets/LevelConfigs/" + fileName + ".json";
//         var jsonStr = EditorGUIUtility.Load(jsPath) as TextAsset;
//         string sb = ConvertLua(jsonStr.text,extra);
//         CommonExtendFunction.WriteConfigLua(fileName, sb);
//         Debug.Log($"+++++++ ExportNpcConfigLua +++++++ config:{fileName}");
//             
//         
//     }
//
//     static string ConvertLua(string jsonStr,string extra =null)
//     {
//         RecordLevelNameAndPlotDesc(jsonStr);         //TODO 空格处理  没想出对文本统一空格去除和恢复的办法   暂时提前提取文本内容 然后原文放回        
//         jsonStr = jsonStr.Replace(" ", string.Empty);//去掉所有空格
//         jsonStr =  LastItemCommaRemove(jsonStr);   
//         
//         string lua = "local data ="+"\n";
//         string jsonType = ConvertJsonType(jsonStr);
//         
//         jsonType += "\n"+"return data";
//         jsonType = jsonType.Replace("[", "{");
//         jsonType = jsonType.Replace("]", "}");
//
//         jsonType = RemoveSpecifiedContent(jsonType);
//
//         jsonType = AddNewline(jsonType);     //todo 加换行要再进行一次全文遍历  优化：将加换行加入到第一次遍历中
//         if (extra!=null)
//         {
//             int insertExtraPos = 0;
//             insertExtraPos = jsonType.LastIndexOf("}",jsonType.Length-1,StringComparison.Ordinal);
//             jsonType = jsonType.Insert(insertExtraPos, ","+extra);
//         }
//         
//         lua += jsonType;
//
//         return lua;
//     }
//     
//     /// <summary>
//     /// 添加换行符
//     /// </summary>
//     /// <param name="jsonStr"></param>
//     /// <returns></returns>
//     static string AddNewline(string jsonStr)
//     {
//         string opStr = (string)jsonStr.Clone();
//
//         int cPos = 0;
//         bool inStr = false;
//         bool inBrackets = false;
//         while (cPos <= opStr.Length)
//         {
//             if (cPos >opStr.Length-3)
//             {
//                 break;
//             }
//             char c = opStr[cPos];
//             if (c == '{')
//             {
//                 if ( cPos+1 >=0 && cPos+1 <=opStr.Length)
//                 {
//                     opStr = opStr.Insert(cPos+1, "\n");
//                 }
//                 
//                 
//             }
//             else if (c == '[')
//             {
//                 if ( cPos+1 >=0 && cPos+1 <=opStr.Length)
//                 {
//                     opStr = opStr.Insert(cPos+1, "\n");
//                 }
//                 
//             }
//             else if (c == '}')
//             {
//                 if ( cPos+1 >=0 && cPos+1 <=opStr.Length)
//                 {
//                     opStr = opStr.Insert(cPos+1, "\n");
//                 }
//                 
//             }
//             else if (c == ']')
//             {
//                 if ( cPos+1 >=0 && cPos+1 <=opStr.Length)
//                 {
//                     opStr = opStr.Insert(cPos+1, "\n");
//                 }
//                 
//             }
//             else if (c == '"')
//             {
//                 inStr = !inStr;
//             }
//             else if (c == '(')
//             {
//                 inBrackets = true;
//             }
//             else if (c == ')')
//             {
//                 inBrackets = false;
//             }
//             else if (c == ',')
//             {
//                 
//                 if ( cPos+1 >=0 && cPos+1 <=opStr.Length && !inStr && !inBrackets)
//                 {
//                     opStr = opStr.Insert(cPos+1, "\n");
//                 }
//                 
//             }
//
//             cPos++;
//             if (cPos >=8000)
//             {
//                 break;
//             }
//         }
//         
//         return opStr;
//     }
//     
//     
//     /// <summary>
//     /// 该方法用于删除 转换后的lua文件中不想要的内容
//     /// </summary>
//     /// <param name="str"></param>
//     /// <returns></returns>
//     static string RemoveSpecifiedContent(string str)   
//     {
//         string opObject = (string)str.Clone();
//         
//         int startPos = opObject.IndexOf("totalMovePath", StringComparison.Ordinal);
//         while(startPos != -1)
//         {
//             int endPos = opObject.IndexOf("}",startPos, StringComparison.Ordinal)+2;
//             if (opObject[startPos-1] == ',')
//             {
//                 startPos -= 1;
//             }
//
//             opObject = opObject.Remove(startPos, endPos - startPos);
//             startPos = opObject.IndexOf("totalMovePath", StringComparison.Ordinal);
//         }
//         
//         startPos = opObject.IndexOf("pathComponentArr", StringComparison.Ordinal);
//         while(startPos != -1)
//         {
//             int endPos = FindArrayItemEndPos(startPos,opObject);
//             if (opObject[startPos-1] == ',')
//             {
//                 startPos -= 1;
//             }
//
//             opObject = opObject.Remove(startPos, endPos - startPos);
//             startPos = opObject.IndexOf("pathComponentArr", StringComparison.Ordinal);
//         }
//         
//         startPos = opObject.IndexOf("color", StringComparison.Ordinal);
//         while(startPos != -1)
//         {
//             if (opObject[startPos-1] == '<' ||(opObject[startPos-1] == '/' && opObject[startPos-2] == '<'))
//             {
//                 startPos = opObject.IndexOf("color",startPos+6, StringComparison.Ordinal);
//                 continue;
//             }
//             int endPos = opObject.IndexOf("}",startPos, StringComparison.Ordinal);
//             if (opObject[startPos-1] == ',')
//             {
//                 startPos -= 1;
//             }
//
//             opObject = opObject.Remove(startPos, endPos - startPos);
//             startPos = opObject.IndexOf("color", StringComparison.Ordinal);
//         }
//         
//         return opObject;
//     }
//
//     /// <summary>
//     /// 查找一个列表元素的结束位置
//     /// </summary>
//     /// <param name="startPos"></param>
//     /// <param name="str"></param>
//     /// <returns></returns>
//     static int FindArrayItemEndPos(int startPos,string str)  
//     {
//         string opObject = (string)str.Clone();
//         var endPos = opObject.IndexOf("{", startPos, StringComparison.Ordinal);
//         var matchNum = 1;
//         while (matchNum !=0)
//         {
//             endPos++;
//             var c = opObject[endPos];
//             if (c == '{' )
//             {
//                 matchNum++;
//             }
//
//             if (c == '}')
//             {
//                 matchNum--;
//             }
//         }
//
//         if (opObject[endPos+1]==',')
//         {
//             endPos = endPos + 1;
//         }else if (opObject[endPos+2]==','&& opObject[endPos+1] !='}' && opObject[endPos+1] !=']')
//         {
//             endPos = endPos + 2;
//         }
//
//         return endPos+1;
//     }
//     
//     /// <summary>
//     /// 该方法用于解决一个列表中  最后一个元素带有 ，  导致其后的 } 被错误遗漏 （单纯的在转lua格式之前，把最后一个元素的 ， 删除）
//     /// </summary>
//     /// <param name="str"></param>
//     /// <returns></returns>
//     static string LastItemCommaRemove(string str)
//     {
//         string opObject = (string)str.Clone();
//         int troublePos = opObject.IndexOf(",]", StringComparison.Ordinal);
//         
//         while (troublePos !=-1)
//         {
//             opObject = opObject.Remove(troublePos , 1);
//             troublePos = opObject.IndexOf(",]", StringComparison.Ordinal);
//         }
//         
//         troublePos = opObject.IndexOf(",}", StringComparison.Ordinal);
//
//         while (troublePos !=-1)
//         {
//             opObject = opObject.Remove(troublePos , 1);
//             troublePos = opObject.IndexOf(",}", StringComparison.Ordinal);
//         }
//
//         return opObject;
//     }
//
//     /// <summary>
//     /// levelName 和PlotDesc两个文本内容中需要保留空格   是直接截取记录下来的  (改进思路：将每个空格位置记录，当内容增减时 将内容增减其后的空格位置进行增减，在转lua完成后
//     /// 将空格按记录的位置放入)
//     /// </summary>
//     /// <param name="str"></param>
//     static void RecordLevelNameAndPlotDesc(string str)   
//     {
//         recordLevelName = null;
//         recordPlotDesc = null;
//         string opObject = (string)str.Clone();
//
//         int levelNameValueStart = opObject.IndexOf("levelName", StringComparison.Ordinal);
//         if (levelNameValueStart != -1)
//         {
//             levelNameValueStart += 11;
//             int levelNameValueEnd = opObject.IndexOf(",",levelNameValueStart, StringComparison.Ordinal);
//             recordLevelName = opObject.Substring(levelNameValueStart,  levelNameValueEnd-levelNameValueStart);
//         }
//         
//         
//         int plotDescValueStart = opObject.IndexOf("plotDesc", StringComparison.Ordinal);
//         if (plotDescValueStart != -1)
//         {
//             plotDescValueStart += 10;
//             int plotDescValueEnd = opObject.IndexOf("mapId",plotDescValueStart, StringComparison.Ordinal); 
//             plotDescValueEnd = opObject.LastIndexOf(",", plotDescValueEnd, StringComparison.Ordinal);
//             recordPlotDesc = opObject.Substring(plotDescValueStart, plotDescValueEnd -plotDescValueStart);
//         }
//         
//     }
//     
//     
//     static string ConvertJsonType(string jsonStr)
//     {
//         string tempStr = jsonStr.Replace("\n", "").Replace("\r", "");
//         string firstChar = "";
//         try
//         {
//             firstChar = tempStr.Substring(0, 2);
//         }
//         catch (System.Exception)
//         {
//             Debug.Log(tempStr);
//         }
//
//         if (firstChar == "[{")
//         {
//             return ConvertJsonArray(jsonStr);
//         }
//         else if (firstChar[0] == '{')
//         {
//             return ConvertJsonArray(jsonStr);
//         }
//         else
//         {
//             return ConvertJsonArrayNoKey(jsonStr);
//         }
//     }
//
//     /// <summary>
//     /// 没有key的 例如[1,2,3]
//     /// </summary>
//     /// <returns></returns>
//     static string ConvertJsonArrayNoKey(string jsonStr)
//     {
//         string lastJsonStr = jsonStr.Replace("[", "{").Replace("]", "}");
//         
//         bool isInVector = false;
//         bool isInString = false;
//         int vectorNum = 1;
//         int vectorOperateLeft = 0;
//         int vectorOperateRight = 0;
//         for (int i = 0; i < lastJsonStr.Length; i++)
//         {
//             char c = lastJsonStr[i];
//
//             if (c == '(')
//             {
//                 vectorOperateLeft = i - 1;
//                 isInVector = true;
//             }
//
//             if (c == ')')
//             {
//                 vectorOperateRight = i + 1;
//                 isInVector = false;
//                     
//             }
//
//             if (c == '"')
//             {
//                 isInString = !isInString;
//             }
//
//             if (c == ',' && isInVector )
//             {
//                 vectorNum++;
//             }
//                 
//             if (c == '}' && isInVector == false && isInString ==false)
//             {
//                 if (vectorNum != 1)
//                 {
//                     lastJsonStr = lastJsonStr.Remove(vectorOperateRight,1);
//                     lastJsonStr = lastJsonStr.Insert(vectorOperateRight,"");
//                         
//                     lastJsonStr = lastJsonStr.Remove(vectorOperateLeft,1);
//                     lastJsonStr = lastJsonStr.Insert(vectorOperateLeft,"Vector"+vectorNum);
//                     vectorNum = 1;
//                 }
//             }
//             
//             if (c == ',' && isInVector == false && isInString ==false)
//             {
//                 if (vectorNum != 1)
//                 {
//                     lastJsonStr = lastJsonStr.Remove(vectorOperateRight,1);
//                     lastJsonStr = lastJsonStr.Insert(vectorOperateRight,"");
//                         
//                     lastJsonStr = lastJsonStr.Remove(vectorOperateLeft,1);
//                     lastJsonStr = lastJsonStr.Insert(vectorOperateLeft,"Vector"+vectorNum);
//                     vectorNum = 1;
//                 }
//             }
//         }
//         
//         return lastJsonStr;
//     }
//
//     static string ConvertJsonArray(string jsonStr)
//     {
//         string lastJsonStr = "";
//         var separatorIndex = jsonStr.IndexOf(':'); //通过:取得所有对象
//         while (separatorIndex >= 0)
//         {
//             separatorIndex += 1; //加上冒号
//             string cutStr = jsonStr.Substring(0, separatorIndex);
//             string tempKey = "";
//             string tempValue = "";
//             for (int i = 0; i < cutStr.Length; i++)
//             {
//                 char c = cutStr[i];
//                 if (c == '[')
//                 {
//                     c = '{';
//                 }
//                 else if (c == '"')
//                 {
//                     continue;
//                 }
//                 else if (c == ':')
//                 {
//                     c = '=';
//                 }
//
//                 tempKey += c;
//             }
//
//             jsonStr = jsonStr.Substring(separatorIndex);
//             int index = 0;
//             bool isInVector = false;
//             bool isInString = false;
//             int vectorNum = 1;
//             int vectorOperateLeft = 0;
//             int vectorOperateRight = 0;
//             for (int i = 0; i < jsonStr.Length; i++)
//             {
//                 char c = jsonStr[i];
//
//                 if (c == '(')
//                 {
//                     vectorOperateLeft = i - 1;
//                     isInVector = true;
//                 }
//
//                 if (c == ')')
//                 {
//                     vectorOperateRight = i + 1;
//                     isInVector = false;
//                     
//                 }
//
//                 if (c == '"')
//                 {
//                     isInString = !isInString;
//                 }
//
//                 if (c == ',' && isInVector )
//                 {
//                     vectorNum++;
//                 }
//                 
//                 if (c == '{')
//                 {
//                     //新对象的开始
//                     string surplusStr = jsonStr.Substring(index);
//                     int bracketNum = 0;
//                     for (int j = 0; j < surplusStr.Length; j++)
//                     {
//                         if (surplusStr[j] == '{')
//                         {
//                             bracketNum++;
//                         }
//                         else if (surplusStr[j] == '}')
//                         {
//                             if (bracketNum == 1)
//                             {
//                                 string tempStr = jsonStr.Substring(index, index + j + 1);
//                                 string strResult = ConvertJsonType(tempStr);
//                                 tempValue += strResult;
//                                 index = index + j;
//                                 break;
//                             }
//
//                             bracketNum--;
//                         }
//                     }
//
//                     i = index;
//                     continue;
//                 }
//                 else if (c == '[')
//                 {
//                     string surplusStr = jsonStr.Substring(index);
//                     int bracketNum = 0;
//                     for (int j = 0; j < surplusStr.Length; j++)
//                     {
//                         if (surplusStr[j] == '[')
//                         {
//                             bracketNum++;
//                         }
//                         else if (surplusStr[j] == ']')
//                         {
//                             if (bracketNum == 1)
//                             {
//                                 string tempStr = jsonStr.Substring(index, index + j + 1);
//                                 string strResult = ConvertJsonType(tempStr);
//                                 tempValue += strResult;
//                                 index = index + j;
//                                 break;
//                             }
//
//                             bracketNum--;
//                         }
//                     }
//
//                     i = index;
//                     continue;
//                 }
//                 else if (c == ',' && isInVector == false && isInString ==false)
//                 {
//                     if (vectorNum != 1)
//                     {
//                         tempValue = tempValue.Remove(vectorOperateRight,1);
//                         tempValue = tempValue.Insert(vectorOperateRight,"");
//                         
//                         tempValue = tempValue.Remove(vectorOperateLeft,1);
//                         tempValue = tempValue.Insert(vectorOperateLeft,"Vector"+vectorNum);
//                     }
//                     break;
//                 }
//                 
//
//                 index = i;
//                 tempValue += c;
//                 if (c == ',' && isInVector == false && isInString ==false)
//                 {
//                     if (jsonStr.Length >i+2)
//                     {
//                         if (jsonStr[i + 1] == '}' || jsonStr[i + 1] == ']' &&jsonStr[i + 2] == ',')
//                         {
//                             tempValue += "},";
//                         }
//                     }
//                 }
//             }
//
//             if (tempKey.Contains("levelName") && recordLevelName != null)
//             {
//                 tempValue = recordLevelName;
//             }
//
//             if (tempKey.Contains("plotDesc") && recordPlotDesc != null)
//             {
//                 tempValue = recordPlotDesc;
//             }
//             lastJsonStr += tempKey + tempValue;
//             jsonStr = jsonStr.Substring(index + 1);
//             separatorIndex = jsonStr.IndexOf(':');
//         }
//
//         return lastJsonStr;
//     }
// }
// #endif