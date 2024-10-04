using System;
using System.Collections.Generic;

[Serializable]
public class GuildRewardTier
{
    public string tierID;
    public string tierName;
    public string description;
    public List<IdIntPair> requirements;
    public List<IdIntPair> rewards;
}
