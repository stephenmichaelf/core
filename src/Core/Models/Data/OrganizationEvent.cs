using System;
using Bit.Core.Enums;
using Bit.Core.Models.Table;
using Bit.Core.Utilities;

namespace Bit.Core.Models.Data
{
    public class OrganizationEvent : EventTableEntity
    {
        public OrganizationEvent(Organization organization, Guid actingUserId, EventType type)
        {
            OrganizationId = organization.Id;
            Type = (int)type;
            ActingUserId = actingUserId;

            Timestamp = DateTime.UtcNow;
            PartitionKey = $"OrganizationId={OrganizationId}";
            RowKey = string.Format("Date={0}__ActingUserId={1}__Type={2}",
                CoreHelpers.DateTimeToTableStorageKey(Timestamp.DateTime), ActingUserId, Type);
        }
    }
}
