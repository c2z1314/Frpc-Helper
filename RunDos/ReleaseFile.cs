using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RunDos
{
    public class ReleaseFile : IDisposable
    {
        /// <summary>
        /// exe的位置
        /// </summary>
        public string ExeName = "";
        /// <summary>
        /// 随机生成的guid
        /// </summary>
        public string guid = "";

        /// <summary>
        /// 缓存的文件夹，配置文件是固定的，frpc.ini
        /// </summary>
        string TempName = "";


        /// <summary>
        /// frpc.ini位置
        /// </summary>
        public string iniPath = "";

        /// <summary>
        /// 是否释放过配置文件
        /// </summary>
        bool isReleaseIni = false;
        public ReleaseFile()
        {
            guid = Guid.NewGuid().ToString();
            TempName = Path.GetTempPath();
        }
        /// <summary>
        /// 释放文件到temp文件夹
        /// </summary>
        /// <returns></returns>
        public bool ReleaseExe()
        {
            try
            {
                var fileBytes = Properties.Resources.frpc;
                if (TempName.EndsWith("\\"))
                    ExeName = TempName + guid + "exfrpc.exe";
                else
                    ExeName = TempName + guid + "\\exfrpc.exe";
                BinaryWriter bw = new BinaryWriter(new FileStream(ExeName, FileMode.Create, FileAccess.ReadWrite));
                bw.Write(fileBytes);
                bw.Flush();
                bw.Dispose();
                return !string.IsNullOrEmpty(ExeName);
            }
            catch (Exception ee)
            {
                throw ee;
            }
        }

        /// <summary>
        /// 释放配置文件
        /// </summary>
        /// <param name="entities"></param>
        /// <returns></returns>
        public string ReleaseIni(List<ElementEntity> entities)
        {
            try
            {
                string data = "";
                if (entities.Count == 0)
                    return data;
                foreach (var item in entities)
                {
                    //字典里没数据，这个节点就不写
                    if (item.Element.Count == 0)
                        continue;
                    //标题
                    data += $"[{item.Title}]\r\n";

                    foreach (var dic in item.Element)
                        data += $"{dic.Key}={dic.Value}\r\n";
                }
                if (TempName.EndsWith("\\"))
                    iniPath = TempName + guid + "frpc.ini";
                else
                    iniPath = TempName + guid + "\\frpc.ini";
                isReleaseIni = true;
                return data;
            }
            catch (Exception ee)
            {
                MessageBox.Show("解析对象错误\r\n" + ee.Message);
                return "";
                //throw ee;
            }
        }

        public bool WriteToFile(string path, string data)
        {
            try
            {
                if (string.IsNullOrEmpty(path))
                {
                    path = iniPath;
                }
                File.WriteAllText(path, data);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }


        /// <summary>
        /// 删除exe和配置文件
        /// </summary>
        /// <returns></returns>
        public bool DeleteExe()
        {
            try
            {
                File.Delete(ExeName);
                return true;
            }
            catch (Exception ee)
            {
                throw ee;
            }
        }


        public void DeleteIni()
        {
            try
            {

                File.Delete(iniPath);
            }
            catch (Exception ee)
            {
                throw ee;
            }
        }

        /// <summary>
        /// 释放的时候删除文件，删不掉也没关系，windows的temp文件夹好像会自动删除
        /// </summary> 
        public void Dispose()
        {
            try
            {
                DeleteExe();
                if (isReleaseIni)
                    DeleteIni();
            }
            catch (Exception ee)
            {

                if (MessageBox.Show("缓存文件删除失败，是否打开缓存目录手动删除？", "重要", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                {
                    System.Diagnostics.Process.Start("Explorer.exe", TempName);
                };
            }


        }
    }
}
