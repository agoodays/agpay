﻿using AGooday.AgPay.Domain.Interfaces;
using AGooday.AgPay.Domain.Models;
using AGooday.AgPay.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGooday.AgPay.Infrastructure.Repositories
{
    public class PayInterfaceConfigRepository : Repository<PayInterfaceConfig, long>, IPayInterfaceConfigRepository
    {
        public PayInterfaceConfigRepository(AgPayDbContext context)
            : base(context)
        {
        }

        public bool IsExistUseIfCode(string ifCode)
        {
            return DbSet.AsNoTracking().Any(c => c.IfCode.Equals(ifCode));
        }

        public void RemoveByInfoIds(List<string> infoIds, byte infoType)
        {
            foreach (string infoId in infoIds)
            {
                var entity = DbSet.Where(w => w.InfoId.Equals(infoId)&& w.InfoType.Equals(infoType)).First();
                Remove(entity.Id);
            }
        }
    }
}
