﻿namespace AGooday.AgPay.Domain.Interfaces
{
    /// <summary>
    /// 定义泛型仓储接口，并继承IDisposable，显式释放资源
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public interface IRepository<TEntity, TPrimaryKey> : IDisposable
        where TEntity : class
        where TPrimaryKey : struct
    {
        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="obj"></param>
        void Add(TEntity obj);
        Task AddAsync(TEntity entity);
        /// <summary>
        /// 根据id获取对象
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        TEntity GetById(TPrimaryKey id);

        Task<TEntity> FindByIdAsync(TPrimaryKey id);
        /// <summary>
        /// 获取列表
        /// </summary>
        /// <returns></returns>
        IQueryable<TEntity> GetAll();
        IQueryable<T> GetAll<T>() where T : class;
        Task<IEnumerable<TEntity>> ListAsync();
        /// <summary>
        /// 根据对象进行更新
        /// </summary>
        /// <param name="obj"></param>
        void Update(TEntity obj);
        /// <summary>
        /// 根据id删除
        /// </summary>
        /// <param name="id"></param>
        void Remove(TPrimaryKey id);
        /// <summary>
        /// 保存或更新
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="id"></param>
        void SaveOrUpdate(TEntity obj, TPrimaryKey? id);
        /// <summary>
        /// 保存
        /// </summary>
        /// <returns></returns>
        int SaveChanges();
        bool SaveChanges(out int count);
    }
    public interface IRepository<TEntity> : IDisposable
        where TEntity : class
    {
        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="obj"></param>
        void Add(TEntity obj);
        Task AddAsync(TEntity entity);
        /// <summary>
        /// 根据id获取对象
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        TEntity GetById<TPrimaryKey>(TPrimaryKey id);

        Task<TEntity> FindByIdAsync<TPrimaryKey>(TPrimaryKey id);
        /// <summary>
        /// 获取列表
        /// </summary>
        /// <returns></returns>
        IQueryable<TEntity> GetAll();
        IQueryable<T> GetAll<T>() where T : class;
        Task<IEnumerable<TEntity>> ListAsync();
        /// <summary>
        /// 根据对象进行更新
        /// </summary>
        /// <param name="obj"></param>
        void Update(TEntity obj);
        /// <summary>
        /// 根据id删除
        /// </summary>
        /// <param name="id"></param>
        void Remove<TPrimaryKey>(TPrimaryKey id);
        /// <summary>
        /// 保存或更新
        /// </summary>
        /// <typeparam name="TPrimaryKey"></typeparam>
        /// <param name="obj"></param>
        /// <param name="id"></param>
        void SaveOrUpdate<TPrimaryKey>(TEntity obj, TPrimaryKey id);
        /// <summary>
        /// 保存
        /// </summary>
        /// <returns></returns>
        int SaveChanges();
        bool SaveChanges(out int count);
    }
    public interface IRepository : IDisposable
    {
    }
}
