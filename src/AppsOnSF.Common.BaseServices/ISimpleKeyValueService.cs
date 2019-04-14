using Microsoft.ServiceFabric.Data;
using Microsoft.ServiceFabric.Services.Remoting;
using Microsoft.ServiceFabric.Services.Remoting.FabricTransport;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AppsOnSF.Common.BaseServices
{
    /// <summary>
    /// 简单KVS远程服务
    /// </summary>
    public interface ISimpleKeyValueService : IService
    {
        /// <summary>
        /// 存入新数据或更新现有数据
        /// </summary>
        /// <param name="container"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        Task AddOrUpdate(string container, string key, string value);
        /// <summary>
        /// 存入数据
        /// </summary>
        /// <param name="container"></param>
        /// <param name="key"></param>
        /// <param name="value">不能为null</param>
        /// <exception cref="ArgumentNullException" />
        /// <returns></returns>
        Task<bool> Add(string container, string key, string value);
        /// <summary>
        /// 获取数据（不管时间有效性）
        /// </summary>
        /// <param name="container"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        Task<string> Get(string container, string key);
        /// <summary>
        /// 检查并获取数据，如果不存在或者失效就返回null
        /// </summary>
        /// <param name="container"></param>
        /// <param name="key"></param>
        /// <param name="validity">相对当前时间的有效时间段，假设数据是10分钟前存入，如果validity是3分钟，那么就判断为失效</param>
        /// <returns></returns>
        Task<string> CheckAndGet(string container, string key, TimeSpan validity);
        /// <summary>
        /// 得到同一个容器内的所有数据
        /// </summary>
        /// <param name="container"></param>
        /// <returns></returns>
        Task<Dictionary<string, string>> GetAll(string container);
        /// <summary>
        /// 删除数据
        /// </summary>
        /// <param name="container"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        Task<string> Remove(string container, string key);
        /// <summary>
        /// 更新数据
        /// </summary>
        /// <param name="container"></param>
        /// <param name="key"></param>
        /// <param name="value">不能为null</param>
        /// <exception cref="ArgumentNullException" />
        /// <returns></returns>
        Task<bool> Update(string container, string key, string value);
        /// <summary>
        /// 判断是否包含数据
        /// </summary>
        /// <param name="container"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        Task<bool> ContainsKey(string container, string key);
        /// <summary>
        /// 清除容器内的所有数据
        /// </summary>
        /// <param name="container"></param>
        /// <returns></returns>
        Task<long> ClearAll(string container);
        /// <summary>
        /// 清理before之前的数据
        /// </summary>
        /// <param name="container"></param>
        /// <param name="before">要清理的时间点</param>
        /// <returns></returns>
        Task<long> Clear(string container, DateTimeOffset before);
    }
}
