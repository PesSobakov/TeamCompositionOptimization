using System;
using System.Collections.Generic;

namespace TeamCompositionOptimizationApi.Models.Database;

public class User
{
    public int Id { get; set; }
    public string Login { get; set; } = "";
    public string PasswordHash { get; set; } = "";
    public Subscription Subscription { set; get; } = new();
    public DateTime SubscriptionDueDate { set; get; } = new();
    public List<Candidate> Candidates { get; set; } = new();
    public List<Competency> Competencies { get; set; } = new();
    public List<OptimizationResult> OptimizationResults { get; set; } = new();
    public bool IsSuperuser { set; get; } = false;
}