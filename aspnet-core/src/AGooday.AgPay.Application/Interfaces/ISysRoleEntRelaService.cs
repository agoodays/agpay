﻿using AGooday.AgPay.Application.DataTransfer;

namespace AGooday.AgPay.Application.Interfaces
{
    public interface ISysRoleEntRelaService : IDisposable
    {
        void Add(SysRoleEntRelaDto dto);
        void Remove(string recordId);
        void Update(SysRoleEntRelaDto dto);
        SysRoleEntRelaDto GetById(string recordId);
        IEnumerable<SysRoleEntRelaDto> GetAll();
        PaginatedList<SysRoleEntRelaDto> GetPaginatedData(SysRoleEntRelaQueryDto dto);
        bool UserHasLeftMenu(long userId, string sysType);
        IEnumerable<string> SelectEntIdsByUserId(long userId, byte isAdmin, string sysType);
        void ResetRela(string roleId, List<string> entIdList);
    }
}
