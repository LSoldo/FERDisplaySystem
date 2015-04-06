using System;
using DAL.Interfaces;
using DAL.Model;

namespace DAL.Factories
{
    public class SequenceFactory : ISequenceFactory
    {
        public ISequence GetSequence(string type)
        {
            if (type == DataDefinition.SequenceType.MaxImage)
                return new MaxImageSequence();
            else
                throw new ArgumentException("CompositionFactory: Composition type argument not valid: {0}", type);
        }
    }
}
