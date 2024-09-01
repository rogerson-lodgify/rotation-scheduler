﻿using Rotation.Domain.Activities;
using Rotation.Domain.SeedWork;
using Rotation.Domain.Users;

namespace Rotation.Infra.Features.Activities;

public class Activity : IActivity
{
    public Activity(string name, string description, Duration duration)
    {
        Name = name;
        Description = description;
        Duration = duration;
        Id = Guid.NewGuid();
    }

    public string Name { get; private set; }
    public string Description { get; private set; }
    public Duration Duration { get; private set; }
    public IEnumerable<IUser> Users { get; private set; } = [];

    public Guid Id {  get; private set; }

    public bool TryAddUser(IUser user)
    {
        if (Users.Any(u => u.Id == user.Id))
        {
            return false;
        }

        Users = Users.Concat([user]);

        return true;
    }

    public (IUser Main, IUser? Replacer) GetNextUsersOnRotation()
    {
        IUser? main = default, replacer = default;
        var unavailableUsers = new List<IUser>();
        foreach (var user in Users)
        {
            if (user.IsAvailable(Duration))
            {
                if (main is null)
                {
                    main = user;
                    continue;
                }

                if (replacer is null)
                {
                    replacer = user; 
                    continue;
                }

                break;
            }

            unavailableUsers.Add(user);
            //se o usuario nao estiver disponivel, manter ele na ponta da lista
            //se o usuario for escalado como MAIN, colocar ele no final da lista
        }

        MoveMainUserToEnd(main!);
        MoveNotAvailableUsersTopList(unavailableUsers);

        return (main!, replacer);
    }

    private void MoveMainUserToEnd(IUser user)
    {
        var currentUsers = Users.ToList();
        currentUsers.Remove(user!);

        Users = currentUsers.Append(user!);
    }

    private void MoveNotAvailableUsersTopList(List<IUser> users)
    {
        if (!users.Any()) return;
        
        var currentUsers = Users.ToList();
        users.Reverse();

        foreach (var user in users)
        {
            currentUsers.Remove(user!);
            currentUsers.Insert(0, user!);
        }

        Users = currentUsers;
    }
}
