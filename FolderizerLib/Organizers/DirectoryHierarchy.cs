using FolderizerLib.DirectoryStructureValidators;
using System.Collections.Generic;

namespace FolderizerLib.Organizers
{
    public class DirectoryHierarchy<T>
    {
        private IList<T> _directoryHierarchy = new List<T>();

        public T this[int i]
        {
            get => Get()[i];
        }

        public IReadOnlyList<T> Get()
        {
            return _directoryHierarchy as IReadOnlyList<T>;
        }

        public void Append(T item)
        {
            if (_directoryHierarchy.Contains(item))
            {
                throw new InvalidDirectoryStructureException("The directory hierarchy cannot be composed of duplicate tags.");
            }

            _directoryHierarchy.Add(item);
        }




    }
}
