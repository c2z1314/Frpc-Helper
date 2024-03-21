using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RunDos
{
    public class ElementEntity
    {
        /// <summary>
        /// [common] [ssh] 标题名字
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 名字里的元素 "type = tcp" 键值对 
        /// </summary>
        public Dictionary<string, string> Element { get; set; }
    }
}
