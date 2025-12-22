using Aplicacion.Interfaces;


namespace Shared.Services
{
    public class CastMappingUpdate : ICastMappingUpdate
    {
        public void CastMapping<T>(object Source, ref T target)
        {        
            foreach (var prop in Source.GetType().GetProperties())
            {
                target.GetType().GetProperty(prop.Name).SetValue(target, prop.GetValue(Source, null), null);
            }
        }
    }
}
