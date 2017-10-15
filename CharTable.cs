using System.Collections;

public class CharTable
{
    private Hashtable table;
    protected CharTable prev;

    public CharTable(CharTable prev)
    {
        table = new Hashtable();
        this.prev = prev;
    } 

    public void Put<T>(string id, T value)
    {
        table.Add(id, value);
    }

    public T Get<T>(string id) where T: class
    {
        for (CharTable ct = this; ct != null; ct = ct.prev)
        {
            T found = (T)ct.table[id];
            if (found != null)
                return found;
        }
        return null;
    }
}
