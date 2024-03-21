using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace RunDos
{
    public partial class Form1 : Form
    {
        /// <summary>
        /// 服务端的自定义设置
        /// </summary>
        Dictionary<string, string> serverDic = new Dictionary<string, string>();
        /// <summary>
        /// 客户端的自定义设置
        /// </summary>
        Dictionary<string, string> clientDic = new Dictionary<string, string>();
        /// <summary>
        /// 文件释放类
        /// </summary>
        public ReleaseFile file = new ReleaseFile();

        /// <summary>
        /// 包含了所有的节点信息
        /// </summary>
        public List<ElementEntity> elements = new List<ElementEntity>();

        /// <summary>
        /// 配置信息，启动时写入文件
        /// </summary>
        public string IniData = "";

        /// <summary>
        /// 运行frp的进程
        /// </summary>
        Process process;

        public Form1()
        {
            InitializeComponent();
            if (comboBox1.Items.Count > 0)
                comboBox1.SelectedIndex = 0;
            Task.Run(() =>
            {
                if (file.ReleaseExe())
                {
                    AddMessage("主程序加载成功，请设置服务器配置。");
                }

            });
            //Process p = new Process();


            //p.StartInfo.CreateNoWindow = true;         // 不创建新窗口    
            //p.StartInfo.UseShellExecute = false;       //不启用shell启动进程  
            //p.StartInfo.RedirectStandardInput = true;  // 重定向输入    
            //p.StartInfo.RedirectStandardOutput = true; // 重定向标准输出    
            //p.StartInfo.RedirectStandardError = true;  // 重定向错误输出  
            //p.StartInfo.FileName = fileName;
            //p.Start();

            //p.StandardInput.AutoFlush = true;
            //p.StandardOutput.ReadToEnd();
            //string output = p.StandardError.ReadToEnd();
            ////  p.OutputDataReceived += new DataReceivedEventHandler(processOutputDataReceived);
            //p.WaitForExit();//参数单位毫秒，在指定时间内没有执行完则强制结束，不填写则无限等待
            //textBox1.Text= output;
        }

        /// <summary>
        /// 关闭窗口时，先结束进程，再删除缓存
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (process != null && !process.HasExited)
                process.Kill();
            Thread.Sleep(500);
            file.Dispose();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (comboBox1.DropDownStyle == ComboBoxStyle.DropDownList)
            {
                comboBox1.DropDownStyle = ComboBoxStyle.DropDown;
            }
            else
            {
                comboBox1.DropDownStyle = ComboBoxStyle.DropDownList;
            }
        }

        /// <summary>
        /// 回车添加服务器设置 自定义节点
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtSelf_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter)
                return;
            if (string.IsNullOrEmpty(txtSelf.Text)) return;
            var splitLs = txtSelf.Text.Split('=');
            if (splitLs.Count() != 2)
            {
                MessageBox.Show("数据可能不合法\r\n" + txtSelf.Text.Trim());
                return;
            }

            if (serverDic.ContainsKey(splitLs[0]))
            {
                MessageBox.Show("重复添加\r\n" + splitLs[0]);
                return;
            }
            serverDic.Add(splitLs[0], splitLs[1]);
            BindingDatasource(listBox1, serverDic);
        }
        /// <summary>
        /// 转发  自定义配置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBox8_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter)
                return;
            if (string.IsNullOrEmpty(txtClient.Text)) return;
            var splitLs = txtClient.Text.Split('=');
            if (splitLs.Count() != 2)
            {
                MessageBox.Show("数据可能不合法\r\n" + txtClient.Text.Trim());
                return;
            }

            if (clientDic.ContainsKey(splitLs[0]))
            {
                MessageBox.Show("重复添加\r\n" + splitLs[0]);
                return;
            }


            clientDic.Add(splitLs[0], splitLs[1]);
            BindingDatasource(listBox2, clientDic);
        }

        /// <summary>
        /// 服务端自定义配置删除选中
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button3_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex < 0)
                return;
            var key = listBox1.SelectedItem.ToString().Trim().Split('=')[0];
            serverDic.Remove(key);
            BindingDatasource(listBox1, serverDic);
        }

        /// <summary>
        /// 客户端自定义配置删除选中
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button6_Click(object sender, EventArgs e)
        {
            if (listBox2.SelectedIndex < 0)
                return;
            var key = listBox2.SelectedItem.ToString().Trim().Split('=')[0];
            clientDic.Remove(key);
            BindingDatasource(listBox2, clientDic);
        }

        /// <summary>
        /// 清除服务端的配置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button4_Click(object sender, EventArgs e)
        {
            serverDic.Clear();
            BindingDatasource(listBox1, serverDic);
        }

        /// <summary>
        /// 清除客户端的配置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button5_Click(object sender, EventArgs e)
        {
            clientDic.Clear();
            BindingDatasource(listBox2, clientDic);
        }

        /// <summary>
        /// 启动服务
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            if (file.WriteToFile("", IniData))
                AddMessage("配置文件写入成功，正在启动服务");
            Task.Run(() =>
            {
                process = new Process();
                process.StartInfo.FileName = file.ExeName;
                process.StartInfo.Arguments = " -c " + file.iniPath;
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.RedirectStandardInput = true;
                process.StartInfo.StandardOutputEncoding = Encoding.UTF8;
                process.StartInfo.CreateNoWindow = true;
                var issuccess = process.Start();
                while (!process.StandardOutput.EndOfStream)
                {
                    AddMessage(process.StandardOutput.ReadLine());
                }
            });
        }

        /// <summary>
        /// 增加节点，
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button7_Click(object sender, EventArgs e)
        {
            if (elements.Count == 0)
            {
                MessageBox.Show("基本配置未保存，先保存数据后再添加多个节点");
                return;
            }
            ClearText(new List<dynamic> { textBox5, textBox6, textBox7, txtClient });
            clientDic.Clear();
            BindingDatasource(listBox2, clientDic);

            var input = new InputeForm("请输入节点名称，需要是服务器中的唯一", elements.Select(o => o.Title).ToList());
            input.ShowDialog();
            if (!input.isEffective)
                return;

            var clientElement = new Dictionary<string, string> {
                    {"type", comboBox1.Text.Trim()},
                    {"remote_port",textBox5.Text.Trim()},
                    {"local_ip",textBox7.Text.Trim()},
                    {"local_port",textBox6.Text.Trim()}
                };
            elements.Add(new ElementEntity
            {
                Title = input.Inputer,
                Element = clientElement
            });
        }

        /// <summary>
        /// 保存节点，只保存当前界面上的，多个节点需要在添加节点那里添加
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button8_Click(object sender, EventArgs e)
        {
            if (TxtIsNull(new List<dynamic> { textBox3, textBox5, textBox6, textBox7, comboBox1 }))
            {
                MessageBox.Show("请检查 橙色 必填项");
                return;
            }

            var input = new InputeForm("请输入节点名称，需要是服务器中的唯一", elements.Select(o => o.Title).ToList());
            input.ShowDialog();
            if (!input.isEffective)
                return;

            //存在则移除，重新加载
            var exist = elements.Where(o => o.Title == "common").ToList();
            if (exist.Count > 0)
                foreach (var item in exist) elements.Remove(item);
            //服务端节点只有一个，每次添加都更新
            var common = new ElementEntity
            {
                //固定为comm
                Title = "common",
                Element = new Dictionary<string, string>{
                    {"server_addr", textBox2.Text.Trim()},
                    {"server_port",textBox3.Text.Trim() },
                }
            };
            //是否有token
            if (!string.IsNullOrEmpty(textBox4.Text.Trim()))
            {
                common.Element.Add("token", textBox4.Text.Trim());
            }
            //判断自定义节点是否存在，存在则添加
            if (serverDic.Count > 0)
            {
                var Illegal = new List<string>();
                if (serverDic.Count > 0)
                {
                    foreach (var item in serverDic)
                    {
                        if (!common.Element.ContainsKey(item.Key))
                        {
                            common.Element.Add(item.Key, item.Value);
                        }
                    }
                }
            }
            //全局变量增加服务元素
            elements.Add(common);

            //存在客户端节点 删除重新加载
            var existClient = elements.Where(o => o.Title != "common").ToList();


            var thisElement = GetClientEntityFromForm(input.Inputer);
            if (thisElement == null)
            {
                MessageBox.Show("数据转换异常");
            }
            if (thisElement != null)
                elements.Add(thisElement);
            var data = file.ReleaseIni(elements);

            if (MessageBox.Show(data, "确认配置是否准确", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                MessageBox.Show("当前配置保存成功，可继续添加客户节点后再启动服务");
                AddMessage(data);
                IniData = data;
            }
            else
            {
                MessageBox.Show("配置未保存，修改配置后再点击保存");
                elements.Clear();
            }

        }


        //分界线，下面是帮助方法

        /// <summary>
        /// 给listbox绑定数据
        /// </summary>
        /// <param name="listBox"></param>
        /// <param name="ls"></param>
        private void BindingDatasource(ListBox listBox, Dictionary<string, string> dic)
        {
            var ls = dic.Keys.Select(o => { return o + "=" + dic[o]; }).ToList();
            listBox.DataSource = null;
            listBox.DataSource = ls;
        }

        /// <summary>
        /// 整理界面的数据封装成对象
        /// </summary>
        /// <param name="title"></param>
        /// <returns></returns>
        private ElementEntity GetClientEntityFromForm(string title)
        {
            try
            {
                var element = new ElementEntity() { Title = title };
                var clientElement = new Dictionary<string, string> {
                    {"type", comboBox1.Text.Trim()},
                    {"remote_port",textBox5.Text.Trim()},
                    {"local_ip",textBox7.Text.Trim()},
                    {"local_port",textBox6.Text.Trim()}
                };
                if (comboBox1.Text == "http" || comboBox1.Text == "https")
                    clientElement.Remove("remote_port");
                if (clientDic.Count > 0)
                {
                    foreach (var item in clientDic)
                    {
                        if (!clientElement.ContainsKey(item.Key))
                        {
                            clientElement.Add(item.Key, item.Value);
                        }
                    }
                }
                element.Element = clientElement;
                return element;
            }
            catch (Exception)
            {
                return null;
            }

        }


        /// <summary>
        /// 动态对象必须要包含text属性
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private bool TxtIsNull(List<dynamic> obj)
        {
            try
            {
                foreach (var item in obj)
                {
                    //var txt = item as dynamic;
                    if (string.IsNullOrEmpty(item.Text))
                        return true;
                }
            }
            catch (Exception)
            {
                return true;
            }
            return false;
        }


        /// <summary>
        /// 清除控件的text
        /// </summary>
        /// <param name="obj"></param>
        private void ClearText(List<dynamic> obj)
        {

            try
            {
                foreach (var item in obj)
                {
                    item.Text = "";
                }
            }
            catch (Exception)
            {
            }
        }

        /// <summary>
        /// 给textbox添加消息内容
        /// </summary>
        /// <param name="msg"></param>
        private void AddMessage(string msg)
        {
            this.BeginInvoke(new Action(() =>
            {
                if (txtMsg.Text.Length >= 10000) txtMsg.Clear();
                txtMsg.Text += msg + "\r\n \r\n";
                txtMsg.SelectionStart = txtMsg.Text.Length;
                txtMsg.ScrollToCaret();
            }));
        }

        private void button7_Click_1(object sender, EventArgs e)
        {
            if (process == null || process.HasExited)
            {
                AddMessage("程序未启动，或者已停止");
                return;
            }

            process.Kill();
            AddMessage(process.StartInfo.FileName + "已关闭");
        }

        private void notifyIcon1_DoubleClick(object sender, EventArgs e)
        {
            Visible = true;
            WindowState = FormWindowState.Normal;
        }

        private void 退出ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void 显示ToolStripMenuItem_Click(object sender, EventArgs e)
        {

            Visible = true;
            WindowState = FormWindowState.Normal;
        }

        private void Form1_SizeChanged(object sender, EventArgs e)
        {
            if (this.WindowState==FormWindowState.Minimized)
            {
                this.Hide();
            }
        }
    }
}
