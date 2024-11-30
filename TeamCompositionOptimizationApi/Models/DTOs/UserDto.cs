using System;
using System.Collections.Generic;

namespace TeamCompositionOptimizationApi.Models.Database;

public class UserDto
{
    public string Login { get; set; } = "";
    public SubscriptionDto Subscription { set; get; } = new();
    public DateTime SubscriptionDueDate { set; get; } = new();
    public bool IsSuperuser { set; get; } = false;
}