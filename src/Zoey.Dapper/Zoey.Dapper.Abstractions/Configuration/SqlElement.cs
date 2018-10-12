using System;
using System.ComponentModel;
using System.Data;
using System.Xml.Serialization;

namespace Zoey.Dapper.Abstractions.Configuration
{
    public abstract class SqlElement
    {
        protected SqlElement()
        {
        }

        public SqlElement Clone()
        {
            var node = CreateInstance();
            node.CommandText = this.CommandText;
            //node.Domain = this.Domain;
            node.Name = this.Name;
            node.TimeOut = this.TimeOut;
            return node;
        }

        protected abstract SqlElement CreateInstance();

        /// <summary>
        /// SQL 语句
        /// </summary>
        [XmlElement("text")]
        public string CommandText { get; set; }

        /// <summary>
        /// 默认为:CommandType.Text
        /// <see cref="CommandType"/>
        /// </summary>
        [XmlAttribute("type"), DefaultValue(CommandType.Text)]
        public CommandType CommandType { get; set; }

        /// <summary>
        /// 数据库`域`
        /// </summary>
        [XmlAttribute("domain")]
        public string Domain { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        [XmlAttribute("name")]
        public string Name { get; set; }

        /// <summary>
        /// 超时时间
        /// 默认为:300毫秒
        /// </summary>
        [XmlAttribute("timeout"), DefaultValue(300)]
        public int TimeOut { get; set; }
    }
}