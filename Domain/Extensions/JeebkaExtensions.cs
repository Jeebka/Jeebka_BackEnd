namespace Domain.Extensions;

public static class JeebkaExtensions
{
    public static void AddRange(this HashSet<string> set, IEnumerable<string> listToAdd)
    {
        foreach (var toAdd in listToAdd)
        {
            set.Add(toAdd);
        }
    }
}