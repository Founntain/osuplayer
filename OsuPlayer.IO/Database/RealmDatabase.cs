using Realms;

namespace OsuPlayer.IO.Database;

public static partial class RealmDatabase
{
    private static async Task<Realm> GetRealm()
    {
        var config = new RealmConfiguration("osuplayer.realm");

        return await Realm.GetInstanceAsync(config);
    }
    
    public static async Task<ICollection<T>> ReadAll<T>() where T : RealmObject
    {
        var realm = await GetRealm();

        return realm.All<T>().ToList();
    }
    
    public static async Task AddObject<T>(T obj) where T : RealmObject
    {
        var realm = await GetRealm();

        realm.Write(() =>
        {
            realm.Add(obj);
        });
    }

    public static async Task DeleteObject(RealmObject obj)
    {
        var realm = await GetRealm();

        realm.Write(() =>
        {
            realm.Remove(obj);
        });
    }
    
    public static async Task DeleteAll<T>() where T : RealmObject
    {
        var realm = await GetRealm();

        realm.Write(() =>
        {
            realm.RemoveAll<T>();
        });
    }
}