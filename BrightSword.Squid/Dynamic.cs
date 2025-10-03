using System;

using BrightSword.Feber.Samples;
using BrightSword.Squid.TypeCreators;

namespace BrightSword.Squid
{
    public static class Dynamic<T>
        where T : class
    {
        private static readonly BasicDataTransferObjectTypeCreator<T> _typeCreator = new BasicDataTransferObjectTypeCreator<T>();

        public static Type Type
        {
            get { return _typeCreator.Type; }
        }

        public static T NewInstance(dynamic source = null)
        {
            return _typeCreator.CreateInstance(source);
        }
    }

    public static class Dynamic<T, TTypeCreator>
        where T : class
        where TTypeCreator : BasicDataTransferObjectTypeCreator<T>, new()
    {
        private static readonly TTypeCreator _typeCreator = new TTypeCreator();

        public static Type Type
        {
            get { return _typeCreator.Type; }
        }

        public static T NewInstance()
        {
            return _typeCreator.CreateInstance();
        }
    }
}