﻿using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AGooday.AgPay.Domain.Models
{
    /// <summary>
    /// 系统配置表
    /// </summary>
    [Comment("系统配置表")]
    [Table("t_sys_config")]
    public class SysConfig
    {
        /// <summary>
        /// 配置KEY
        /// </summary>
        [Comment("配置KEY")]
        [Key, Required, Column("config_key", TypeName = "varchar(50)")]
        public string ConfigKey { get; set; }

        /// <summary>
        /// 配置名称
        /// </summary>
        [Comment("配置名称")]
        [Required, Column("config_name", TypeName = "varchar(50)")]
        public string ConfigName { get; set; }

        /// <summary>
        /// 描述信息
        /// </summary>
        [Comment("描述信息")]
        [Required, Column("config_desc", TypeName = "varchar(200)")]
        public string ConfigDesc { get; set; }

        /// <summary>
        /// 分组key
        /// </summary>
        [Comment("分组key")]
        [Required, Column("group_key", TypeName = "varchar(50)")]
        public string GroupKey { get; set; }

        /// <summary>
        /// 分组名称
        /// </summary>
        [Comment("分组名称")]
        [Required, Column("group_name", TypeName = "varchar(50)")]
        public string GroupName { get; set; }

        /// <summary>
        /// 配置内容项
        /// </summary>
        [Comment("配置内容项")]
        [Required, Column("config_val", TypeName = "text")]
        public string ConfigVal { get; set; }

        /// <summary>
        /// 类型: text-输入框, textarea-多行文本, uploadImg-上传图片, switch-开关
        /// </summary>
        [Comment("类型: text-输入框, textarea-多行文本, uploadImg-上传图片, switch-开关")]
        [Required, Column("type", TypeName = "varchar(20)")]
        public string Type { get; set; }

        /// <summary>
        /// 显示顺序
        /// </summary>
        [Comment("显示顺序")]
        [Required, Column("sort_num", TypeName = "bigint")]
        public long SortNum { get; set; }

        /// <summary>
        /// 更新时间
        /// </summary>
        [Comment("更新时间")]
        [Required, Column("updated_at", TypeName = "timestamp(6)")]
        public DateTime UpdatedAt { get; set; }
    }
}
