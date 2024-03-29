﻿using AGooday.AgPay.Domain.Interfaces;
using AGooday.AgPay.Domain.Models;
using AGooday.AgPay.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace AGooday.AgPay.Infrastructure.Repositories
{
    public class MchDivisionReceiverRepository : Repository<MchDivisionReceiver, long>, IMchDivisionReceiverRepository
    {
        public MchDivisionReceiverRepository(AgPayDbContext context)
            : base(context)
        {
        }

        public bool IsExistUseReceiverGroup(long receiverGroupId)
        {
            return DbSet.AsNoTracking().Any(c => c.ReceiverGroupId.Equals(receiverGroupId));
        }
    }
}
