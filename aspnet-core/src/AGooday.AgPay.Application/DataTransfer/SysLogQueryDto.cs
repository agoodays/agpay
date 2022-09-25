﻿using AGooday.AgPay.Common.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGooday.AgPay.Application.DataTransfer
{
    /// <summary>
    /// 系统操作日志表
    /// </summary>
    public class SysLogQueryDto : PageQuery
    {
        /// <summary>
        /// 系统用户ID
        /// </summary>
        public long UserId { get; set; }

        /// <summary>
        /// 用户姓名
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 用户IP
        /// </summary>
        [BindNever]
        public string UserIp { get; set; }

        /// <summary>
        /// 所属系统： MGR-运营平台, MCH-商户中心
        /// </summary>
        public string SysType { get; set; }

        /// <summary>
        /// 方法名
        /// </summary>
        [BindNever]
        public string MethodName { get; set; }

        /// <summary>
        /// 方法描述
        /// </summary>
        [BindNever]
        public string MethodRemark { get; set; }

        /// <summary>
        /// 请求地址
        /// </summary>
        [BindNever]
        public string ReqUrl { get; set; }

        /// <summary>
        /// 操作请求参数
        /// </summary>
        [BindNever]
        public string OptReqParam { get; set; }

        /// <summary>
        /// 操作响应结果
        /// </summary>
        [BindNever]
        public string OptResInfo { get; set; }

        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime? CreatedStart { get; set; }

        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime? CreatedEnd { get; set; }
    }
}
