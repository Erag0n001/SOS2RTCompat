using Shared;

public class SpaceFactionGoodwillData  : FactionGoodwillData
{
    public int _id;
    public SpaceFactionGoodwillData(int id) 
    {
        _id = id; //If we set the id to -1, it means we assume the client sending is the owner of the ship and we'll use UID
    }
}