namespace DDBMSP.Interfaces.PODs.Core
{
    public interface IComponentOf<out TComponent, in TPod>
    {
        TComponent Populate(TPod component);
    }
    
    public interface IComposedBy<out TPod, in TComponent>
    {
        TPod Populate(TComponent component);
    }
}