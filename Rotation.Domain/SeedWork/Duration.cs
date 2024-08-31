﻿namespace Rotation.Domain.SeedWork;

public class Duration
{
    public DurationType DurationType { get; private set; }
    public int Amount { get; private set; }
    public DateTime Begin { get; private set; }
    public DateTime CurrentBegin { get; private set; }

    public Duration(int amount, DurationType durationType, DateTime begin)
    {
        Amount = amount;
        DurationType = durationType;
        Begin = CurrentBegin = begin;
    }

    public DateTime[] GetCurrentInterval()
    {
        var result = new List<DateTime>();
        
        var end = DurationType switch
        {
            DurationType.Days => Amount,
            DurationType.Weeks => Amount * 7,
            _ => throw new NotImplementedException(),
        };

        for (var i = 0; i < end; i++)
        {
            result.Add(CurrentBegin.AddDays(i));
        }

        return result.ToArray();
    }
    
    public DateTime NextBegin()
    {
        var next = DurationType switch
        {
            DurationType.Days => CurrentBegin.AddDays(Amount),
            DurationType.Weeks => CurrentBegin.AddDays(Amount * 7),
            _ => throw new NotImplementedException(),
        };

        CurrentBegin = next;

        return next;
    }

    

    public bool IsValid()
    {
        if (Amount <= 0) return false;

        if (Begin < DateTime.Now.Date) return false;

        return true;
    }
}

public enum DurationType
{
    Days,
    Weeks
}
