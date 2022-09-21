﻿using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Domain.Commands.SysUsers;
using AGooday.AgPay.Domain.Core.Bus;
using AGooday.AgPay.Domain.Interfaces;
using AGooday.AgPay.Domain.Models;
using AGooday.AgPay.Infrastructure.Repositories;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace AGooday.AgPay.Application.Services
{
    public class SysUserRoleRelaService : ISysUserRoleRelaService
    {
        // 注意这里是要IoC依赖注入的，还没有实现
        private readonly ISysUserRoleRelaRepository _sysUserRoleRelaRepository;
        // 用来进行DTO
        private readonly IMapper _mapper;
        // 中介者 总线
        private readonly IMediatorHandler Bus;

        public SysUserRoleRelaService(IMapper mapper, IMediatorHandler bus, ISysUserRoleRelaRepository sysUserRoleRelaRepository)
        {
            _mapper = mapper;
            Bus = bus;
            _sysUserRoleRelaRepository = sysUserRoleRelaRepository;
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public void Add(SysUserRoleRelaDto dto)
        {
            var m = _mapper.Map<SysUserRoleRela>(dto);
            _sysUserRoleRelaRepository.Add(m);
            _sysUserRoleRelaRepository.SaveChanges();
        }

        public void Remove(string recordId)
        {
            _sysUserRoleRelaRepository.Remove(recordId);
            _sysUserRoleRelaRepository.SaveChanges();
        }

        public void Update(SysUserRoleRelaDto dto)
        {
            var m = _mapper.Map<SysUserRoleRela>(dto);
            _sysUserRoleRelaRepository.Update(m);
            _sysUserRoleRelaRepository.SaveChanges();
        }

        public SysUserRoleRelaDto GetById(string recordId)
        {
            var entity = _sysUserRoleRelaRepository.GetById(recordId);
            var dto = _mapper.Map<SysUserRoleRelaDto>(entity);
            return dto;
        }

        public IEnumerable<SysUserRoleRelaDto> GetAll()
        {
            var sysUserRoleRelas = _sysUserRoleRelaRepository.GetAll();
            return _mapper.Map<IEnumerable<SysUserRoleRelaDto>>(sysUserRoleRelas);
        }

        public PaginatedList<SysUserRoleRelaDto> GetPaginatedData(SysUserRoleRelaDto dto, int pageIndex = 1, int pageSize = 20)
        {
            var sysUserRoleRelas = _sysUserRoleRelaRepository.GetAll()
                .Where(w => dto.UserId.Equals(0) || w.UserId.Equals(dto.UserId));
            var records = PaginatedList<SysUserRoleRela>.Create<SysUserRoleRelaDto>(sysUserRoleRelas.AsNoTracking(), _mapper, pageIndex, pageSize);
            return records;
        }

        /// <summary>
        /// 根据用户查询全部角色集合
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public IEnumerable<string> SelectRoleIdsByUserId(long userId)
        {
            return _sysUserRoleRelaRepository.GetAll()
                .Where(w => w.UserId == userId)
                .Select(s => s.RoleId);
        }

        /// <summary>
        /// 根据角色查询全部用户集合
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public IEnumerable<long> SelectRoleIdsByRoleId(string roleId)
        {
            return _sysUserRoleRelaRepository.GetAll()
                .Where(w => w.RoleId.Equals(roleId))
                .Select(s => s.UserId);
        }
    }
}
