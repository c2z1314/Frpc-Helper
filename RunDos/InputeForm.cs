using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RunDos
{
    public partial class InputeForm : Form
    {
        private readonly List<string> list;
        public string Inputer = "";
        /// <summary>
        /// 输入数值是否有效
        /// </summary>
        public bool isEffective = false;
        /// <summary>
        /// 输入框
        /// </summary>
        /// <param name="title">新建标题</param>
        /// <param name="list">当前已经存在的标题</param>
        public InputeForm(string title, List<string> list)
        {
            InitializeComponent();
            this.Text = title;
            this.list = list;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            isEffective = !string.IsNullOrEmpty(textBox1.Text.Trim());
            if (isEffective)
                Inputer = textBox1.Text.Trim();
            else
            {
                MessageBox.Show("请输入有效的节点名称");
            }
            if (list.Contains(textBox1.Text.Trim()))
            {
                MessageBox.Show("当前节点已存在");
            }
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {

            this.Close();
        }
    }
}
