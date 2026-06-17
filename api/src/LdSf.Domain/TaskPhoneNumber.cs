using System;
using Volo.Abp.Domain.Entities;

namespace LdSf;

public class TaskPhoneNumber : Entity<Guid>
{
    public Guid TaskId { get; private set; }
    public string PhoneNumber { get; private set; } = string.Empty;
    public bool ConfirmedSent { get; private set; }
    public DateTime? ConfirmedAt { get; private set; }

    protected TaskPhoneNumber()
    {
    }

    public TaskPhoneNumber(Guid id, Guid taskId, string phoneNumber) : base(id)
    {
        TaskId = taskId;
        PhoneNumber = phoneNumber;
    }

    public void Confirm()
    {
        ConfirmedSent = true;
        ConfirmedAt = DateTime.UtcNow;
    }
}

