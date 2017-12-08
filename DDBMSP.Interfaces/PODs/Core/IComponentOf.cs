namespace DDBMSP.Interfaces.PODs.Core
{
    public interface IComponentOf<in TComponent, out TPod>
    {
        void Populate(TComponent component);
    }
}